#include "pch.h"
#include <stdlib.h>
#include <string.h>
#include "ringbuffer.h"

//
// This source file implements a lock‑free ring buffer using C11 atomics.
// The buffer supports concurrent producer/consumer access without locks,
// making it ideal for high‑performance interop scenarios (e.g., C# ↔ C++).
//

/// <summary>
/// Allocates and initializes a new ring buffer with the given capacity.
/// The head and tail indices start at zero and are updated atomically.
/// </summary>
/// <param name="capacity">Number of bytes the buffer can hold.</param>
/// <returns>A pointer to the newly created ring buffer.</returns>
EXP32 ringbuffer_t* rb_create(uint32_t capacity)
{
  auto* rb = new ringbuffer_t;

  rb->capacity = capacity;
  rb->buffer = new uint8_t[capacity];
  rb->head.store(0, std::memory_order_relaxed);
  rb->tail.store(0, std::memory_order_relaxed);

  return rb;
}

/// <summary>
/// Frees a previously created ring buffer and its internal storage.
/// </summary>
/// <param name="rb">Pointer to the ring buffer to destroy.</param>
EXP32 void rb_free(ringbuffer_t* rb)
{
  if (!rb) return;
  delete[] rb->buffer;
  delete rb;
}

/// <summary>
/// Returns the number of bytes currently available to read.
/// Computed as (head - tail), using atomic loads.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <returns>Readable byte count.</returns>
EXP32 uint32_t rb_available_to_read(ringbuffer_t* rb)
{
  uint32_t head = rb->head.load(std::memory_order_acquire);
  uint32_t tail = rb->tail.load(std::memory_order_acquire);
  return head - tail;
}

/// <summary>
/// Returns the number of bytes currently available to write.
/// Computed as (capacity - readable).
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <returns>Writable byte count.</returns>
EXP32 uint32_t rb_available_to_write(ringbuffer_t* rb)
{
  return rb->capacity - rb_available_to_read(rb);
}

/// <summary>
/// Writes up to <c>length</c> bytes into the ring buffer.
/// The write may wrap around the end of the buffer.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <param name="data">Source data to write.</param>
/// <param name="length">Requested number of bytes to write.</param>
/// <returns>The number of bytes actually written.</returns>
EXP32 uint32_t rb_write(ringbuffer_t* rb, const uint8_t* data, uint32_t length)
{
  uint32_t writable = rb_available_to_write(rb);
  if (length > writable)
    length = writable;

  uint32_t head = rb->head.load(std::memory_order_relaxed);
  uint32_t pos = head % rb->capacity;

  // First contiguous block
  uint32_t first = rb->capacity - pos;
  if (first > length) first = length;

  memcpy(rb->buffer + pos, data, first);
  memcpy(rb->buffer, data + first, length - first);

  rb->head.store(head + length, std::memory_order_release);
  return length;
}

/// <summary>
/// Reads up to <c>length</c> bytes from the ring buffer.
/// The read may wrap around the end of the buffer.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <param name="dest">Destination buffer to fill.</param>
/// <param name="length">Requested number of bytes to read.</param>
/// <returns>The number of bytes actually read.</returns>
EXP32 uint32_t rb_read(ringbuffer_t* rb, uint8_t* dest, uint32_t length)
{
  uint32_t readable = rb_available_to_read(rb);
  if (length > readable)
    length = readable;

  uint32_t tail = rb->tail.load(std::memory_order_relaxed);
  uint32_t pos = tail % rb->capacity;

  // First contiguous block
  uint32_t first = rb->capacity - pos;
  if (first > length) first = length;

  memcpy(dest, rb->buffer + pos, first);
  memcpy(dest + first, rb->buffer, length - first);

  rb->tail.store(tail + length, std::memory_order_release);
  return length;
}
