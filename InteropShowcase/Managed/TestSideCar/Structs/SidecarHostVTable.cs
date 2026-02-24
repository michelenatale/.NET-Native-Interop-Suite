
using System.Runtime.InteropServices;

namespace michele.natale;

/// <summary>
/// Represents the unmanaged virtual function table used by the native Sidecar worker.
/// </summary>
/// <remarks>
/// The Sidecar runtime calls these function pointers to invoke managed callbacks
/// during its lifecycle.  
/// All delegates use the unmanaged calling convention and must remain valid for the
/// entire lifetime of the Sidecar.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SidecarHostVTable
{
  /// <summary>
  /// Pointer to the callback invoked when the Sidecar thread starts.
  /// </summary>
  /// <remarks>
  /// This is called exactly once before any processing occurs.
  /// </remarks>
  public delegate* unmanaged<void> Init;

  /// <summary>
  /// Pointer to the callback invoked when the Sidecar thread is shutting down.
  /// </summary>
  /// <remarks>
  /// This is called exactly once after the worker loop exits.
  /// </remarks>
  public delegate* unmanaged<void> Dispose;

  /// <summary>
  /// Pointer to the callback invoked whenever the Sidecar receives a command
  /// from the host via the shared ring buffer.
  /// </summary>
  /// <param name="data">Pointer to the command payload.</param>
  /// <param name="length">Number of bytes in the payload.</param>
  public delegate* unmanaged<byte*, int, void> Process;

  /// <summary>
  /// Pointer to the callback invoked by the Sidecar to send events back to the host.
  /// </summary>
  /// <param name="eventId">Numeric event identifier.</param>
  /// <param name="data">Pointer to the event payload.</param>
  /// <param name="length">Number of bytes in the payload.</param>
  public delegate* unmanaged<int, byte*, int, void> OnEvent;
}
