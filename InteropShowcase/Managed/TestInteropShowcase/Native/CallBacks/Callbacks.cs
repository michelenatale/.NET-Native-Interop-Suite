
  
using System.Runtime.InteropServices;

namespace michele.natale.RingBufferNative;
 

/// <summary>
/// Provides managed wrappers for native callback functions.
/// This class encapsulates all Reverse P/Invoke entry points used to:
/// export unmanaged function pointers from C# (NativeAOT),
/// register them inside native code, and trigger native operations
/// that call back into managed code.
/// </summary>
internal static partial class Callbacks
{
  /// <summary>
  /// Name of the native library that exposes the callback functions.
  /// </summary>
  private const string DllName = "InteropShowcaseLib.dll";

  /// <summary>
  /// Calls the native function <c>native_do_work</c>.
  /// This function triggers a previously registered managed callback.
  /// </summary>
  [LibraryImport(DllName, EntryPoint = "native_do_work")]
  internal static partial void NativeDoWork();

  /// <summary>
  /// Calls the native function <c>native_do_work_msg</c> and passes
  /// a UTF‑8 encoded string to native code.  
  /// The native function then invokes the registered managed callback
  /// with the provided message.
  /// </summary>
  /// <param name="msg">The UTF‑8 text to send to the native function.</param>
  [LibraryImport(DllName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "native_do_work_msg")]
  internal static partial void NativeDoWork(string msg);

  /// <summary>
  /// Registers a managed callback inside native code.  
  /// The provided function pointer must be a true unmanaged function pointer,
  /// typically created via <c>delegate* unmanaged&lt;...&gt;</c> referencing a method
  /// marked with <see cref="UnmanagedCallersOnlyAttribute"/>.
  /// </summary>
  /// <param name="fn">A function pointer to an unmanaged C# method.</param>
  [LibraryImport(DllName, EntryPoint = "register_managed_callback")]
  internal static partial void RegisterCallback(IntPtr fn);
}
