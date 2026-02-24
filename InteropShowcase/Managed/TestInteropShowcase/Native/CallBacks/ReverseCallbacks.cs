

using System.Runtime.InteropServices;

namespace michele.natale.RingBufferNative;
 

/// <summary>
/// Provides a managed callback that can be exposed to native code as a true
/// unmanaged function pointer.  
/// The callback is compiled by NativeAOT and marked with
/// <see cref="UnmanagedCallersOnlyAttribute"/>, allowing C++ to invoke it
/// directly without any delegate wrappers or runtime stubs.
/// </summary>
internal static unsafe class ReverseCallbacks
{
  /// <summary>
  /// Gets a native function pointer to the managed callback method.
  /// The returned pointer references a method compiled as an unmanaged
  /// entry point, making it safe for direct invocation from native code.
  /// </summary>
  public static IntPtr FunctionPtr =>
      (IntPtr)(delegate* unmanaged<IntPtr, void>)&Log;


  [UnmanagedCallersOnly]
  private static void Log(IntPtr msgPtr)
  {
    string msg = Marshal.PtrToStringUTF8(msgPtr)!;
    Console.WriteLine($"[Managed] Received: {msg}");
  }
}
