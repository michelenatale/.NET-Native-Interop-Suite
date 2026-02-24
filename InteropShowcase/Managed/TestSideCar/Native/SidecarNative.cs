

using System.Runtime.InteropServices;

namespace michele.natale.Native;


/// <summary>
/// Provides low-level P/Invoke bindings for the native Sidecar API.
/// </summary>
/// <remarks>
/// These methods start and stop the native sidecar worker thread.
/// They are internal infrastructure and should only be used through
/// higher-level managed abstractions such as <c>SidecarHost</c>.
/// </remarks>
internal static partial class SidecarNative
{
  private const string DllName = "SidecarModelLib.dll";

  /// <summary>
  /// Starts the native sidecar worker thread.
  /// </summary>
  /// <param name="host">
  /// Pointer to a <see cref="SidecarHostVTable"/> structure containing
  /// the managed callback functions (Init, Dispose, Process, OnEvent).
  /// </param>
  /// <param name="rb">
  /// Pointer to a <see cref="SidecarRingBufferDesc"/> structure describing
  /// the shared ring buffer used for host–sidecar communication.
  /// </param>
  /// <remarks>
  /// This function:
  /// <list type="bullet">
  /// <item>Opens the shared ring buffer</item>
  /// <item>Stores the host callback table</item>
  /// <item>Spawns the sidecar worker thread</item>
  /// <item>Immediately invokes <c>Init()</c> on the host</item>
  /// <item>Begins processing commands in a continuous loop</item>
  /// </list>
  /// The sidecar must be stopped using <see cref="SidecarStop"/> before
  /// the host shuts down.
  /// </remarks>
  [LibraryImport(DllName, EntryPoint = "sidecar_start")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  internal static unsafe partial void SidecarStart(SidecarHostVTable* host, SidecarRingBufferDesc* rb);

  /// <summary>
  /// Stops the native sidecar worker thread.
  /// </summary>
  /// <remarks>
  /// This function:
  /// <list type="bullet">
  /// <item>Signals the worker loop to exit</item>
  /// <item>Waits for the thread to finish</item>
  /// <item>Invokes <c>Dispose()</c> on the host</item>
  /// <item>Releases shared memory resources</item>
  /// </list>
  /// Safe to call multiple times.
  /// </remarks>
  [LibraryImport(DllName, EntryPoint = "sidecar_stop")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  internal static partial void SidecarStop();
}
