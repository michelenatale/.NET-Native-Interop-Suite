 


namespace michele.natale;

using Native;

/// <summary>
/// Provides a managed wrapper around the native shared-memory ring buffer.
/// </summary>
/// <remarks>
/// This class encapsulates the lifetime and usage of a native ring buffer created
/// through <see cref="RingBufferNative"/>.  
/// It offers safe, high-level read/write operations while ensuring proper cleanup
/// of unmanaged resources.
/// </remarks>
internal sealed unsafe class RingBuffer : IDisposable
{
  private IntPtr MHandle;

  /// <summary>
  /// Gets the total capacity of the ring buffer in bytes.
  /// </summary>
  public uint Capacity { get; } = 0;

  /// <summary>
  /// Indicates whether the ring buffer has already been disposed.
  /// </summary>
  public bool IsDisposed => this.MHandle == IntPtr.Zero;

  /// <summary>
  /// Creates a new shared-memory ring buffer.
  /// </summary>
  /// <param name="capacity">The size of the ring buffer in bytes.</param>
  /// <param name="name">The unique shared memory name used to create the buffer.</param>
  /// <exception cref="InvalidOperationException">
  /// Thrown when the native ring buffer cannot be created.
  /// </exception>
  public RingBuffer(uint capacity, string name)
  {
    var name_bytes = System.Text.Encoding.ASCII.GetBytes(name + "\0");
    fixed (byte* name_ptr = name_bytes)
    {
      this.MHandle = RingBufferNative.RbCreate((sbyte*)name_ptr, capacity);
    }

    if (this.MHandle == IntPtr.Zero)
      throw new InvalidOperationException("Failed to create shared ring buffer.");

    this.Capacity = capacity;
  }

  /// <summary>
  /// Opens an existing shared-memory ring buffer.
  /// </summary>
  /// <param name="name">The shared memory name of the existing ring buffer.</param>
  /// <exception cref="InvalidOperationException">
  /// Thrown when the ring buffer cannot be opened.
  /// </exception>
  public RingBuffer(string name)
  {
    var name_bytes = System.Text.Encoding.ASCII.GetBytes(name + "\0");
    fixed (byte* name_ptr = name_bytes)
    {
      this.MHandle = RingBufferNative.RbOpen((sbyte*)name_ptr);
    }

    if (this.MHandle == IntPtr.Zero)
      throw new InvalidOperationException("Failed to open shared ring buffer.");

    this.Capacity = RingBufferNative.RbCapacity(this.MHandle);
  }

  /// <summary>
  /// Writes data into the ring buffer.
  /// </summary>
  /// <param name="data">The data to write.</param>
  /// <returns>
  /// The number of bytes actually written.  
  /// This may be less than <paramref name="data"/> length if insufficient space is available.
  /// </returns>
  public uint Write(ReadOnlySpan<byte> data)
  {
    fixed (byte* ptr = data)
      return RingBufferNative.RbWrite(this.MHandle, ptr, (uint)data.Length);
  }

  /// <summary>
  /// Reads data from the ring buffer.
  /// </summary>
  /// <param name="dest">The buffer to receive the data.</param>
  /// <returns>
  /// The number of bytes actually read.  
  /// This may be less than <paramref name="dest"/> length if insufficient data is available.
  /// </returns>
  public uint Read(Span<byte> dest)
  {
    fixed (byte* ptr = dest)
      return RingBufferNative.RbRead(this.MHandle, ptr, (uint)dest.Length);
  }

  /// <summary>
  /// Gets the number of bytes currently available to read.
  /// </summary>
  public uint AvailableToRead =>
      RingBufferNative.RbAvailableToRead(this.MHandle);

  /// <summary>
  /// Gets the number of bytes currently available to write.
  /// </summary>
  public uint AvailableToWrite =>
      RingBufferNative.RbAvailableToWrite(this.MHandle);

  /// <summary>
  /// Releases the native ring buffer handle.
  /// </summary>
  /// <remarks>
  /// This method is safe to call multiple times.  
  /// After disposal, the ring buffer handle becomes invalid.
  /// </remarks>
  public void Dispose()
  {
    var h = Interlocked.Exchange(ref this.MHandle, IntPtr.Zero);
    if (h != IntPtr.Zero)
      RingBufferNative.RbClose(h);
  }
}


