#include "pch.h"
#include <atomic>
#include <thread>
#include "sidecar_api.h"
#include "shared_ringbuffer.h"


// Global state for the sidecar worker thread.
// These are intentionally kept internal to this translation unit.
static std::thread g_thread;                          // Worker thread running the command loop
static shared_rb_t* g_rb = nullptr;                   // Shared ring buffer handle
static std::atomic<bool> g_running{ false };          // Controls the lifetime of the worker loop
static const sidecar_host_vtable_t* g_host = nullptr; // Host-provided callback table


/*
 * The main worker loop executed by the sidecar thread.
 *
 * Responsibilities:
 *   - Call host->Init() once at startup
 *   - Continuously read commands from the shared ring buffer
 *   - Forward commands to host->Process()
 *   - Send example events back via host->OnEvent()
 *   - Call host->Dispose() before shutting down
 *
 * This loop runs until g_running becomes false.
 */
static void sidecar_loop()
{
  // Notify host that the sidecar is starting
  g_host->Init();

  uint8_t buffer[1024];

  while (g_running.load(std::memory_order_acquire))
  {
    // Try to read a command from the ring buffer
    uint32_t read = shared_rb_read(g_rb, buffer, sizeof(buffer));
    if (read > 0)
    {
      // Forward the command to the host
      g_host->Process(buffer, static_cast<int>(read));

      // Send a simple example event back to the host
      const char msg[] = "OK";
      g_host->OnEvent(1, reinterpret_cast<const uint8_t*>(msg), 2);
    }
    else
    {
      // No data available → avoid busy spinning
      std::this_thread::sleep_for(std::chrono::milliseconds(1));
    }
  }

  // Notify host that the sidecar is shutting down
  g_host->Dispose();
}


/*
 * Starts the sidecar worker thread.
 *
 * Parameters:
 *   host   - Pointer to the host-provided callback table
 *   rbDesc - Descriptor containing the shared ring buffer name and capacity
 *
 * Behavior:
 *   - Opens the shared ring buffer
 *   - Stores the host vtable
 *   - Spawns the worker thread
 *   - Raises the thread priority for more responsive processing
 *
 * Requirements:
 *   - Must not be called twice without calling sidecar_stop()
 */
EXP32 void sidecar_start(const sidecar_host_vtable_t* host, const sidecar_rb_desc_t* rbDesc)
{
  if (g_running.load()) return; // Already running

  g_host = host;

  // Open the shared ring buffer created by the host
  g_rb = shared_rb_open(rbDesc->name);
  if (!g_rb) return;

  g_running.store(true, std::memory_order_release);

  // Launch the worker thread
  g_thread = std::thread(sidecar_loop);

  // Optional: Increase thread priority for more responsive IPC
  // THREAD_PRIORITY_ABOVE_NORMAL is usually enough,
  // but THREAD_PRIORITY_HIGHEST gives maximum responsiveness.
  SetThreadPriority(g_thread.native_handle(), THREAD_PRIORITY_HIGHEST);
}


/*
 * Stops the sidecar worker thread.
 *
 * Behavior:
 *   - Signals the worker loop to exit
 *   - Joins the worker thread
 *   - Closes the shared ring buffer
 *   - Clears global state
 *
 * Safe to call multiple times.
 */
EXP32 void sidecar_stop()
{
  if (!g_running.load()) return;

  // Signal the worker loop to exit
  g_running.store(false, std::memory_order_release);

  // Wait for the worker thread to finish
  if (g_thread.joinable())
    g_thread.join();

  // Release shared memory resources
  shared_rb_close(g_rb);
  g_rb = nullptr;

  // Clear host callback table
  g_host = nullptr;
}
