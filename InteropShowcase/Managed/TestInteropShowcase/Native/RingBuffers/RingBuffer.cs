
using System.Runtime.InteropServices;


namespace michele.natale.RingBufferNative;

 

/// <summary>
/// Provides managed wrappers for the native lock‑free ring buffer implementation.
/// This class exposes creation, disposal, read/write operations, and capacity
/// queries for a high‑performance producer‑consumer buffer implemented in C.
/// </summary>
internal static partial class RingBuffer
{
  private const string DllName = "InteropShowcaseLib.dll";

  /// <summary>
  /// Represents a pointer to a native ring buffer instance.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct RingBufferPtr
  {
    public IntPtr Ptr;
  }

  /// <summary>
  /// Creates a new native ring buffer with the specified capacity.
  /// </summary>
  /// <param name="capacity">The number of bytes the ring buffer can hold.</param>
  /// <returns>A pointer to the newly created native ring buffer.</returns>
  [LibraryImport(DllName, EntryPoint = "rb_create")]
  public static partial IntPtr Create(uint capacity);

  /// <summary>
  /// Frees a previously created native ring buffer.
  /// </summary>
  /// <param name="rb">A pointer to the ring buffer to free.</param>
  [LibraryImport(DllName, EntryPoint = "rb_free")]
  public static partial void Free(IntPtr rb);

  /// <summary>
  /// Writes data into the native ring buffer.
  /// </summary>
  /// <param name="rb">A pointer to the ring buffer.</param>
  /// <param name="data">The data to write.</param>
  /// <param name="length">The number of bytes to write.</param>
  /// <returns>The number of bytes actually written.</returns>
  [LibraryImport(DllName, EntryPoint = "rb_write")]
  public static partial uint Write(IntPtr rb, ReadOnlySpan<byte> data, uint length);

  /// <summary>
  /// Reads data from the native ring buffer.
  /// </summary>
  /// <param name="rb">A pointer to the ring buffer.</param>
  /// <param name="dest">The destination buffer to fill.</param>
  /// <param name="length">The maximum number of bytes to read.</param>
  /// <returns>The number of bytes actually read.</returns>
  [LibraryImport(DllName, EntryPoint = "rb_read")]
  public static partial uint Read(IntPtr rb, ReadOnlySpan<byte> dest, uint length);

  /// <summary>
  /// Gets the number of bytes currently available to read from the ring buffer.
  /// </summary>
  /// <param name="rb">A pointer to the ring buffer.</param>
  /// <returns>The number of readable bytes.</returns>
  [LibraryImport(DllName, EntryPoint = "rb_available_to_read")]
  public static partial uint AvailableToRead(IntPtr rb);

  /// <summary>
  /// Gets the number of bytes currently available to write into the ring buffer.
  /// </summary>
  /// <param name="rb">A pointer to the ring buffer.</param>
  /// <returns>The number of writable bytes.</returns>
  [LibraryImport(DllName, EntryPoint = "rb_available_to_write")]
  public static partial uint AvailableToWrite(IntPtr rb);
}


