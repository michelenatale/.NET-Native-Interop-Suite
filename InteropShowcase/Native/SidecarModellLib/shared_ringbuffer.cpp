#include "pch.h"

#include <atomic>
#include <cstring>
#include <windows.h>
#include "shared_ringbuffer.h"

/*
 * Internal representation of the shared ring buffer.
 *
 * This structure is intentionally opaque to the outside world.
 * Only this translation unit knows the actual layout.
 */
struct shared_rb_t
{
  HANDLE mapping = nullptr;      // Handle to the shared memory mapping
  uint32_t capacity = 0;         // Size of the ring buffer (payload area)

  // Shared memory layout:
  // [uint32_t head]
  // [uint32_t tail]
  // [uint8_t  buffer[capacity]]
  std::atomic<uint32_t>* head = nullptr; // Producer index
  std::atomic<uint32_t>* tail = nullptr; // Consumer index
  uint8_t* buffer = nullptr;             // Pointer to the byte payload region
};

/*
 * Computes the total size of the shared memory region.
 * Includes head + tail + payload buffer.
 */
static size_t calc_total_size(uint32_t capacity)
{
  return sizeof(uint32_t) * 2 + capacity;
}


/*
 * Creates a new shared-memory ring buffer.
 *
 * Steps:
 *   - Allocate a file mapping (backed by system paging file)
 *   - Map the entire region into the process
 *   - Initialize head and tail to zero
 */
EXP32 shared_rb_t* shared_rb_create(const char* name, uint32_t capacity)
{
  auto* rb = new shared_rb_t();
  rb->capacity = capacity;

  const size_t totalSize = calc_total_size(capacity);

  // Create a new shared memory region
  rb->mapping = CreateFileMappingA(
    INVALID_HANDLE_VALUE,   // Use system paging file
    nullptr,
    PAGE_READWRITE,
    0,
    static_cast<DWORD>(totalSize),
    name);

  if (!rb->mapping)
  {
    delete rb;
    return nullptr;
  }

  // Map the region into this process
  uint8_t* base = static_cast<uint8_t*>(
    MapViewOfFile(rb->mapping, FILE_MAP_ALL_ACCESS, 0, 0, totalSize));

  if (!base)
  {
    CloseHandle(rb->mapping);
    delete rb;
    return nullptr;
  }

  // Assign pointers into the shared memory layout
  rb->head = reinterpret_cast<std::atomic<uint32_t>*>(base);
  rb->tail = reinterpret_cast<std::atomic<uint32_t>*>(base + sizeof(uint32_t));
  rb->buffer = base + sizeof(uint32_t) * 2;

  // Initialize indices
  rb->head->store(0, std::memory_order_relaxed);
  rb->tail->store(0, std::memory_order_relaxed);

  return rb;
}


/*
 * Opens an existing shared-memory ring buffer.
 *
 * Steps:
 *   - Open the file mapping by name
 *   - Map the entire region (size unknown at compile time)
 *   - Query the region size using VirtualQuery
 *   - Derive capacity from total size
 */
EXP32 shared_rb_t* shared_rb_open(const char* name)
{
  auto* rb = new shared_rb_t();

  rb->mapping = OpenFileMappingA(FILE_MAP_ALL_ACCESS, FALSE, name);
  if (!rb->mapping)
  {
    delete rb;
    return nullptr;
  }

  // Map the entire region (size = 0 means "map all")
  uint8_t* base = static_cast<uint8_t*>(
    MapViewOfFile(rb->mapping, FILE_MAP_ALL_ACCESS, 0, 0, 0));

  if (!base)
  {
    CloseHandle(rb->mapping);
    delete rb;
    return nullptr;
  }

  rb->head = reinterpret_cast<std::atomic<uint32_t>*>(base);
  rb->tail = reinterpret_cast<std::atomic<uint32_t>*>(base + sizeof(uint32_t));

  // Query the mapped region to determine its total size
  MEMORY_BASIC_INFORMATION mbi{};
  VirtualQuery(base, &mbi, sizeof(mbi));

  const size_t totalSize = mbi.RegionSize;
  rb->capacity = static_cast<uint32_t>(totalSize - sizeof(uint32_t) * 2);
  rb->buffer = base + sizeof(uint32_t) * 2;

  return rb;
}


/*
 * Closes a shared ring buffer.
 *
 * Steps:
 *   - Unmap the shared memory view
 *   - Close the file mapping handle
 *   - Free the wrapper structure
 */
EXP32 void shared_rb_close(shared_rb_t* rb)
{
  if (!rb) return;

  if (rb->head)
    UnmapViewOfFile(rb->head);

  if (rb->mapping)
    CloseHandle(rb->mapping);

  delete rb;
}


/*
 * Writes data into the ring buffer.
 *
 * Behavior:
 *   - Lock-free single-producer logic
 *   - Computes available space
 *   - Writes as much as possible (may be less than requested)
 *   - Handles wrap-around in two memcpy operations
 */
EXP32 uint32_t shared_rb_write(shared_rb_t* rb, const uint8_t* data, uint32_t length)
{
  const uint32_t head = rb->head->load(std::memory_order_acquire);
  const uint32_t tail = rb->tail->load(std::memory_order_acquire);

  const uint32_t used = head - tail;
  const uint32_t free = rb->capacity - used;

  // Clamp to available space
  if (length > free)
    length = free;

  const uint32_t pos = head % rb->capacity;
  uint32_t first = rb->capacity - pos;
  if (first > length) first = length;

  // Write first segment
  std::memcpy(rb->buffer + pos, data, first);

  // Write second segment (wrap-around)
  std::memcpy(rb->buffer, data + first, length - first);

  // Publish new head index
  rb->head->store(head + length, std::memory_order_release);
  return length;
}


/*
 * Reads data from the ring buffer.
 *
 * Behavior:
 *   - Lock-free single-consumer logic
 *   - Computes available data
 *   - Reads as much as possible (may be less than requested)
 *   - Handles wrap-around in two memcpy operations
 */
EXP32 uint32_t shared_rb_read(shared_rb_t* rb, uint8_t* dest, uint32_t length)
{
  const uint32_t head = rb->head->load(std::memory_order_acquire);
  const uint32_t tail = rb->tail->load(std::memory_order_acquire);

  const uint32_t used = head - tail;

  // Clamp to available data
  if (length > used)
    length = used;

  const uint32_t pos = tail % rb->capacity;
  uint32_t first = rb->capacity - pos;
  if (first > length) first = length;

  // Read first segment
  std::memcpy(dest, rb->buffer + pos, first);

  // Read second segment (wrap-around)
  std::memcpy(dest + first, rb->buffer, length - first);

  // Publish new tail index
  rb->tail->store(tail + length, std::memory_order_release);
  return length;
}
