#pragma once
#include <stdint.h>
#include "EXP32IMP32.h"   // Contains EXP32 macro (extern "C" + dllexport)


// Forward declaration of the internal ring buffer structure.
// The actual layout is intentionally hidden to enforce encapsulation.
struct shared_rb_t;


/*
 * Creates a new shared-memory ring buffer.
 *
 * Parameters:
 *   name     - Unique name of the shared memory object (e.g., "SidecarRB")
 *   capacity - Size of the ring buffer in bytes
 *
 * Returns:
 *   Pointer to shared_rb_t on success
 *   nullptr on failure (e.g., name already in use or allocation error)
 *
 * Notes:
 *   - Allocates a shared memory region (CreateFileMapping + MapViewOfFile)
 *   - Initializes head and tail indices to zero
 *   - Typically called by the host process
 */
EXP32 shared_rb_t* shared_rb_create(const char* name, uint32_t capacity);


/*
 * Opens an existing shared-memory ring buffer.
 *
 * Parameters:
 *   name - Name of the already created ring buffer
 *
 * Returns:
 *   Pointer to shared_rb_t on success
 *   nullptr if the ring buffer does not exist or cannot be opened
 *
 * Notes:
 *   - Typically called by the sidecar process
 *   - Reads the capacity from the shared memory region
 */
EXP32 shared_rb_t* shared_rb_open(const char* name);


/*
 * Closes a previously created or opened ring buffer.
 *
 * Parameters:
 *   rb - Handle returned by shared_rb_create or shared_rb_open
 *
 * Notes:
 *   - Unmaps the shared memory view
 *   - Closes the underlying file mapping handle
 *   - Frees all associated resources
 */
EXP32 void shared_rb_close(shared_rb_t* rb);


/*
 * Writes data into the ring buffer.
 *
 * Parameters:
 *   rb     - Ring buffer handle
 *   data   - Pointer to the bytes to write
 *   length - Number of bytes to write
 *
 * Returns:
 *   Number of bytes actually written (may be less than requested)
 *
 * Notes:
 *   - Lock-free
 *   - Zero-copy (only memcpy into shared memory)
 *   - Automatically handles wrap-around
 *   - Non-blocking: if not enough space is available, writes as much as possible
 */
EXP32 uint32_t shared_rb_write(shared_rb_t* rb, const uint8_t* data, uint32_t length);


/*
 * Reads data from the ring buffer.
 *
 * Parameters:
 *   rb     - Ring buffer handle
 *   dest   - Destination buffer
 *   length - Maximum number of bytes to read
 *
 * Returns:
 *   Number of bytes actually read (may be less than requested)
 *
 * Notes:
 *   - Lock-free
 *   - Zero-copy (only memcpy from shared memory)
 *   - Automatically handles wrap-around
 *   - Non-blocking: if not enough data is available, reads as much as possible
 */
EXP32 uint32_t shared_rb_read(shared_rb_t* rb, uint8_t* dest, uint32_t length);
