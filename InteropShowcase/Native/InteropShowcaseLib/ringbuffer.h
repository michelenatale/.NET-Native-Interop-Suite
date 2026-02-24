#pragma once

#include <atomic>
#include <cstdint>

#include "EXP32IMP32.h"

 
// This header defines a lock‑free ring buffer used for high‑performance
// producer‑consumer communication between native and managed code.
// The implementation uses C11 atomics for thread‑safe, wait‑free access.
 

/// <summary>
/// Represents a lock‑free ring buffer.
/// The buffer uses atomic head/tail indices to allow concurrent
/// producer and consumer operations without locks.
/// </summary>
struct ringbuffer_t
{
  uint8_t* buffer;                 // Raw byte storage
  uint32_t capacity;               // Total size of the buffer
  std::atomic<uint32_t> head;      // Read position (consumer)
  std::atomic<uint32_t> tail;      // Write position (producer)
};

/// <summary>
/// Creates a new ring buffer with the specified capacity.
/// </summary>
/// <param name="capacity">Number of bytes the buffer can hold.</param>
/// <returns>A pointer to the newly allocated ring buffer.</returns>
EXP32 ringbuffer_t* rb_create(uint32_t capacity);

/// <summary>
/// Frees a previously created ring buffer.
/// </summary>
/// <param name="rb">Pointer to the ring buffer to destroy.</param>
EXP32 void rb_free(ringbuffer_t* rb);

/// <summary>
/// Writes data into the ring buffer.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <param name="data">Pointer to the source data.</param>
/// <param name="length">Number of bytes to write.</param>
/// <returns>The number of bytes actually written.</returns>
EXP32 uint32_t rb_write(ringbuffer_t* rb, const uint8_t* data, uint32_t length);

/// <summary>
/// Reads data from the ring buffer.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <param name="dest">Destination buffer to fill.</param>
/// <param name="length">Maximum number of bytes to read.</param>
/// <returns>The number of bytes actually read.</returns>
EXP32 uint32_t rb_read(ringbuffer_t* rb, uint8_t* dest, uint32_t length);

/// <summary>
/// Returns the number of bytes currently available to read.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <returns>Readable byte count.</returns>
EXP32 uint32_t rb_available_to_read(ringbuffer_t* rb);

/// <summary>
/// Returns the number of bytes currently available to write.
/// </summary>
/// <param name="rb">Pointer to the ring buffer.</param>
/// <returns>Writable byte count.</returns>
EXP32 uint32_t rb_available_to_write(ringbuffer_t* rb);
