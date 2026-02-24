
using System.Runtime.InteropServices;

namespace michele.natale;

/// <summary>
/// Describes the shared ring buffer used for communication between the host
/// and the native Sidecar worker.
/// </summary>
/// <remarks>
/// This structure is passed to the native <c>sidecar_start</c> function and must
/// remain valid for the entire lifetime of the Sidecar.  
/// The <see cref="Name"/> field is a pointer to a null-terminated ASCII string
/// representing the shared memory object name.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public struct SidecarRingBufferDesc
{
  /// <summary>
  /// Pointer to a null-terminated ASCII string containing the name of the
  /// shared ring buffer (i.e., the shared memory object).
  /// </summary>
  /// <remarks>
  /// This value is typically obtained from a pinned managed string using
  /// <see cref="System.Runtime.InteropServices.GCHandle"/> or fixed memory.
  /// </remarks>
  public nint Name;

  /// <summary>
  /// The total capacity of the ring buffer in bytes.
  /// </summary>
  /// <remarks>
  /// This value must match the capacity used when creating the shared
  /// memory region on the host side.
  /// </remarks>
  public uint Capacity;
}

