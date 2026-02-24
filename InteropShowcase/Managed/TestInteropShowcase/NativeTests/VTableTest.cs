namespace michele.natale;

using RingBufferNative;

/// <summary>
/// Demonstrates building and using a V-table for native interoperability.
/// C# generates a table of real, unmanaged function pointers (NativeAOT) 
/// that can be called by C++ as if they were native functions.
/// </summary>
internal class VTableTest
{
  /*
   * Dieses Beispiel zeigt:
   * • Klassisches C‑/C++‑Interface‑Pattern über eine struct mit Funktionszeigern.
   * • C# implementiert dieses Interface vollständig, ohne COM oder Runtime‑Magie.
   * • NativeAOT + UnmanagedCallersOnly erzeugen echte, stabile Funktionszeiger.
   * • Die V‑Table kann später erweitert werden (z. B. Init, Dispose, Process, OnEvent).
   */

  /*
   * This example shows:
   * • Classic C/C++ interface pattern using a struct with function pointers.
   * • C# implements this interface completely without COM or runtime magic.
   * • NativeAOT + UnmanagedCallersOnly generate real, stable function pointers.
   * • The V-table can be extended later (e.g., Init, Dispose, Process, OnEvent).
   */

  /// <summary>
  /// Start the V-Table Interop Test and run the demonstration.
  /// </summary>
  public static void Start()
  {
    /*
     * Ablauf:
     * • C++ definiert ein Interface als struct mit Funktionszeigern.
     * • C# baut diese V‑Table zur Laufzeit zusammen.
     * • NativeAOT erzeugt echte native Funktionszeiger (UnmanagedCallersOnly).
     * • C++ ruft diese Methoden direkt auf, ohne Marshaling oder Delegates.
     */

    /*
     * Process:
     * • C++ defines an interface as a struct with function pointers.
     * • C# assembles this V-table at runtime.
     * • NativeAOT generates true native function pointers (UnmanagedCallersOnly).
     * • C++ calls these methods directly, without marshaling or delegates.
     */

    TestVTable();
    Console.WriteLine();
  }

  /// <summary>
  /// Performs the actual V-table interoperability test.
  /// Builds the V-table, passes it to C++, and lets C++ call the C# methods.  /// </summary>
  private static void TestVTable()
  {
    Console.WriteLine($"{nameof(TestVTable)}:");

    /*
     * C#:
     * • erzeugt eine V‑Table (struct mit Funktionszeigern)
     * • füllt sie mit echten unmanaged Funktionszeigern
     * • gibt einen Pointer auf diese V‑Table zurück
     */

    /*
     * C#:
     * • creates a V-table (struct with function pointers)
     * • fills it with real unmanaged function pointers
     * • returns a pointer to this V-table
     */

    var vtable_ptr = ManagedService.VTablePtr;

    /*
     * C++:
     * • erhält den Pointer auf die V‑Table
     * • ruft die C#‑Methoden über die Funktionszeiger auf,
     *   als wären es native C‑Funktionen
     */

    /*
     * C++:
     * • obtains the pointer to the V-table
     * • calls the C# methods via the function pointers,
     *   as if they were native C functions
     */

    VTableNative.UseManagedService(vtable_ptr);
  }
}
