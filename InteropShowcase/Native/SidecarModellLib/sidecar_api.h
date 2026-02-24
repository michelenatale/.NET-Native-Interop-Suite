#pragma once
#include <stdint.h>
#include "EXP32IMP32.h"


/*
 * Host-side virtual function table.
 *
 * The host (managed side) provides a set of callbacks that the sidecar
 * (native worker process) can invoke during its lifecycle.
 *
 * All function pointers use C ABI and must remain stable across processes.
 */
struct sidecar_host_vtable_t
{
  // Called once when the sidecar thread starts.
  void (*Init)();

  // Called once when the sidecar thread is shutting down.
  void (*Dispose)();

  // Called whenever the sidecar receives a command from the host.
  // 'data'   - pointer to the command payload
  // 'length' - number of bytes in the payload
  void (*Process)(const uint8_t* data, int length);

  // Called by the sidecar to send events back to the host.
  // 'eventId' - numeric event identifier
  // 'data'    - pointer to event payload
  // 'length'  - number of bytes in the payload
  void (*OnEvent)(int eventId, const uint8_t* data, int length);
};


/*
 * Describes the shared ring buffer used for host <-> sidecar communication.
 *
 * The host provides:
 *   - the shared memory name
 *   - the capacity of the ring buffer
 *
 * The sidecar opens the same shared memory region using this descriptor.
 */
struct sidecar_rb_desc_t
{
  const char* name;   // Name of the shared memory ring buffer
  uint32_t    capacity; // Size of the ring buffer in bytes
};


/*
 * Stops the sidecar worker thread.
 *
 * Notes:
 *   - Signals the worker loop to exit
 *   - Waits for the thread to finish (join)
 *   - Calls host->Dispose() before shutting down
 *   - Releases all resources (shared memory, thread handles, etc.)
 */
EXP32 void sidecar_stop();


/*
 * Starts the sidecar worker thread.
 *
 * Parameters:
 *   host - Pointer to the host-provided vtable (Init/Dispose/Process/OnEvent)
 *   rb   - Pointer to the ring buffer descriptor (name + capacity)
 *
 * Notes:
 *   - Opens the shared ring buffer using rb->name
 *   - Spawns a dedicated worker thread
 *   - Immediately calls host->Init()
 *   - Enters the command-processing loop
 *   - Uses host->Process() for incoming commands
 *   - Uses host->OnEvent() to send events back to the host
 *
 * Requirements:
 *   - Must be called exactly once before sidecar_stop()
 *   - host and rb must remain valid for the entire lifetime of the sidecar
 */
EXP32 void sidecar_start(const sidecar_host_vtable_t* host,
  const sidecar_rb_desc_t* rb);
