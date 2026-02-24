
using System.Runtime.InteropServices;



namespace michele.natale.RingBufferNative;

 

/// <summary>
/// Provides the managed wrapper for invoking the native function that consumes
/// a managed service V‑Table.  
/// This class enables C# to pass a pointer to a table of unmanaged function
/// pointers (generated via NativeAOT) to native C++ code.
/// </summary>
internal static partial class VTableNative
{
  private const string DllName = "InteropShowcaseLib.dll";

  /// <summary>
  /// Calls the native function <c>use_managed_service</c> and passes a pointer
  /// to a managed V‑Table containing unmanaged function pointers.  
  /// Native code can then invoke the managed methods directly through the table.
  /// </summary>
  /// <param name="vtable">A pointer to the managed service V‑Table.</param>
  [LibraryImport(DllName, EntryPoint = "use_managed_service")]
  internal static partial void UseManagedService(IntPtr vtable);
}

