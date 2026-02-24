


using System.Runtime.InteropServices;

namespace michele.natale.Native;


/// <summary>
/// Provides low-level P/Invoke bindings for the native shared ring buffer API.
/// </summary>
/// <remarks>
/// This class exposes raw interop calls and is not intended for direct use.
/// A higher-level managed wrapper (e.g., <c>RingBuffer</c>) should be used instead.
/// </remarks>
internal static partial class RingBufferNative
{
  private const string DllName = "SidecarModelLib.dll";

  /// <summary>
  /// Creates a new shared-memory ring buffer.
  /// </summary>
  /// <param name="name">Pointer to a null-terminated ASCII string representing the shared memory name.</param>
  /// <param name="capacity">The size of the ring buffer in bytes.</param>
  /// <returns>
  /// A native handle to the ring buffer, or <see cref="IntPtr.Zero"/> if creation failed.
  /// </returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_create")]
  public static unsafe partial IntPtr RbCreate(sbyte* name, uint capacity);

  /// <summary>
  /// Opens an existing shared-memory ring buffer.
  /// </summary>
  /// <param name="name">Pointer to a null-terminated ASCII string representing the shared memory name.</param>
  /// <returns>
  /// A native handle to the ring buffer, or <see cref="IntPtr.Zero"/> if the buffer does not exist.
  /// </returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_open")]
  public static unsafe partial IntPtr RbOpen(sbyte* name);

  /// <summary>
  /// Closes a previously created or opened ring buffer.
  /// </summary>
  /// <param name="rb">The native ring buffer handle.</param>
  [LibraryImport(DllName, EntryPoint = "shared_rb_close")]
  public static partial void RbClose(IntPtr rb);

  /// <summary>
  /// Writes data into the ring buffer.
  /// </summary>
  /// <param name="rb">The native ring buffer handle.</param>
  /// <param name="data">Pointer to the data to write.</param>
  /// <param name="length">The number of bytes to write.</param>
  /// <returns>
  /// The number of bytes actually written.  
  /// This may be less than <paramref name="length"/> if insufficient space is available.
  /// </returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_write")]
  public static unsafe partial uint RbWrite(IntPtr rb, byte* data, uint length);

  /// <summary>
  /// Reads data from the ring buffer.
  /// </summary>
  /// <param name="rb">The native ring buffer handle.</param>
  /// <param name="dest">Pointer to the destination buffer.</param>
  /// <param name="length">The maximum number of bytes to read.</param>
  /// <returns>
  /// The number of bytes actually read.  
  /// This may be less than <paramref name="length"/> if insufficient data is available.
  /// </returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_read")]
  public static unsafe partial uint RbRead(IntPtr rb, byte* dest, uint length);

  /// <summary>
  /// Gets the number of bytes currently available to read.
  /// </summary>
  /// <param name="rb">The native ring buffer handle.</param>
  /// <returns>The number of readable bytes.</returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_available_to_read")]
  public static partial uint RbAvailableToRead(IntPtr rb);

  /// <summary>
  /// Gets the number of bytes currently available to write.
  /// </summary>
  /// <param name="rb">The native ring buffer handle.</param>
  /// <returns>The number of writable bytes.</returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_available_to_write")]
  public static partial uint RbAvailableToWrite(IntPtr rb);

  /// <summary>
  /// Gets the total capacity of the ring buffer in bytes.
  /// </summary>
  /// <param name="rb">The native ring buffer handle.</param>
  /// <returns>The ring buffer capacity.</returns>
  [LibraryImport(DllName, EntryPoint = "shared_rb_capacity")]
  public static partial uint RbCapacity(IntPtr rb);
}

