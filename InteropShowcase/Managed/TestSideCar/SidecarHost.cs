
using System.Runtime.InteropServices;


namespace michele.natale;

using Native;


/// <summary>
/// Represents the managed host-side controller for the native Sidecar worker.
/// </summary>
/// <remarks>
/// This class is responsible for:
/// <list type="bullet">
/// <item>Creating and managing the shared ring buffer</item>
/// <item>Pinning the shared memory name for native interop</item>
/// <item>Providing unmanaged callback functions via <see cref="SidecarHostVTable"/></item>
/// <item>Starting and stopping the native Sidecar worker thread</item>
/// <item>Forwarding commands and receiving events</item>
/// </list>
/// The <see cref="SidecarHost"/> must remain alive for the entire lifetime of the
/// Sidecar worker, as it owns the pinned memory and callback table.
/// </remarks>
internal sealed unsafe class SidecarHost : IDisposable
{
  private GCHandle MNameHandle;
  private readonly RingBuffer MRb;
  private readonly byte[] MNameBytes;
  private readonly SidecarHostVTable MVTable;

  /// <summary>
  /// Gets a value indicating whether the Sidecar worker has been started.
  /// </summary>
  public bool IsStarted { get; private set; } = false;

  /// <summary>
  /// Initializes a new instance of the <see cref="SidecarHost"/> class.
  /// </summary>
  /// <param name="name">The shared memory name used for the ring buffer.</param>
  /// <param name="capacity">The size of the ring buffer in bytes.</param>
  /// <remarks>
  /// This constructor:
  /// <list type="bullet">
  /// <item>Creates a new shared ring buffer</item>
  /// <item>Pins the shared memory name for native use</item>
  /// <item>Initializes the unmanaged callback table</item>
  /// </list>
  /// </remarks>
  public SidecarHost(string name, uint capacity)
  {
    this.MRb = new RingBuffer(capacity, name);

    this.MNameBytes = System.Text.Encoding.ASCII.GetBytes(name + "\0");
    this.MNameHandle = GCHandle.Alloc(this.MNameBytes, GCHandleType.Pinned);

    this.MVTable = new SidecarHostVTable
    {
      Init = &InitImpl,
      Dispose = &DisposeImpl,
      Process = &ProcessImpl,
      OnEvent = &OnEventImpl
    };
  }

  /// <summary>
  /// Starts the native Sidecar worker thread.
  /// </summary>
  /// <remarks>
  /// This method:
  /// <list type="bullet">
  /// <item>Builds a <see cref="SidecarRingBufferDesc"/> pointing to the shared memory</item>
  /// <item>Passes the callback table and descriptor to the native <c>sidecar_start</c></item>
  /// <item>Ensures the worker thread begins processing commands</item>
  /// </list>
  /// Calling this method multiple times has no effect.
  /// </remarks>
  public void Start()
  {
    if (this.IsStarted) return;
    this.IsStarted = true;

    var desc = new SidecarRingBufferDesc
    {
      Name = this.MNameHandle.AddrOfPinnedObject(),
      Capacity = this.MRb.Capacity
    };

    fixed (SidecarHostVTable* v = &this.MVTable)
    {
      SidecarNative.SidecarStart(v, &desc);
    }
  }

  /// <summary>
  /// Stops the native Sidecar worker thread.
  /// </summary>
  /// <remarks>
  /// This method signals the worker loop to exit and waits for the native
  /// thread to shut down.  
  /// Safe to call multiple times.
  /// </remarks>
  public void Stop()
  {
    if (!this.IsStarted) return;
    SidecarNative.SidecarStop();
    this.IsStarted = false;
  }

  /// <summary>
  /// Sends a command to the Sidecar worker via the shared ring buffer.
  /// </summary>
  /// <param name="command">The command payload to write.</param>
  public void SendCommand(ReadOnlySpan<byte> command)
  {
    this.MRb.Write(command);
  }

  /// <summary>
  /// Releases all resources associated with the Sidecar host.
  /// </summary>
  /// <remarks>
  /// This method:
  /// <list type="bullet">
  /// <item>Stops the Sidecar worker if it is running</item>
  /// <item>Frees the pinned shared memory name</item>
  /// <item>Disposes the underlying ring buffer</item>
  /// </list>
  /// </remarks>
  public void Dispose()
  {
    this.Stop();
    if (this.MNameHandle.IsAllocated)
      this.MNameHandle.Free();
    this.MRb.Dispose();
  }

  // ---------------------------------------------------------------------
  // Unmanaged callback implementations
  // ---------------------------------------------------------------------

  /// <summary>
  /// Called by the native Sidecar when the worker thread starts.
  /// </summary>
  [UnmanagedCallersOnly]
  private static void InitImpl()
  {
    Console.WriteLine("[Host] Init");
  }

  /// <summary>
  /// Called by the native Sidecar when the worker thread is shutting down.
  /// </summary>
  [UnmanagedCallersOnly]
  private static void DisposeImpl()
  {
    Console.WriteLine("[Host] Dispose");
  }

  /// <summary>
  /// Called by the native Sidecar when a command is received.
  /// </summary>
  /// <param name="data">Pointer to the command payload.</param>
  /// <param name="length">Number of bytes in the payload.</param>
  [UnmanagedCallersOnly]
  private static void ProcessImpl(byte* data, int length)
  {
    var span = new ReadOnlySpan<byte>(data, length);
    Console.WriteLine("[Host] Process: " + BitConverter.ToString(span.ToArray()));
  }

  /// <summary>
  /// Called by the native Sidecar when an event is emitted.
  /// </summary>
  /// <param name="eventId">Numeric event identifier.</param>
  /// <param name="data">Pointer to the event payload.</param>
  /// <param name="length">Number of bytes in the payload.</param>
  [UnmanagedCallersOnly]
  private static void OnEventImpl(int eventId, byte* data, int length)
  {
    var span = new ReadOnlySpan<byte>(data, length);
    Console.WriteLine($"[Host] Event {eventId}: {System.Text.Encoding.ASCII.GetString(span)}");
  }
}


