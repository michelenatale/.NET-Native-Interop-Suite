

namespace michele.natale;

using RingBufferNative;

/// <summary>
/// Demonstrates reverse P/Invoke: C# exports true native function pointers 
/// (via <see cref="System.Runtime.InteropServices.UnmanagedCallersOnlyAttribute"/>) 
/// that can be called directly from C++.
/// This pattern forms the basis for event systems, logging bridges, 
/// telemetry, sidecar communication, and V-table interop.
/// </summary>
internal class ReversePInvokeTest
{
  /*
   * Warum Reverse‑P/Invoke wichtig ist:
   * • Event‑Systeme → Native Code sendet Ereignisse an Managed Code
   * • Logging‑Bridges → Native schreibt Logs direkt in Managed
   * • Telemetrie → Native Engine meldet Metriken an Managed
   * • Crypto‑Pipelines → Native Crypto Engine ruft Managed Hooks auf
   * • Sidecar‑Modell → Separater Prozess kommuniziert mit Managed
   * • V‑Table‑Interop → Native ruft Managed Interface‑Methoden
   */

  /*
   * Why Reverse‑P/Invoke is important:
   * • Event systems → Native code sends events to managed code
   * • Logging bridges → Native writes logs directly to managed
   * • Telemetry → Native engine reports metrics to managed
   * • Crypto pipelines → Native crypto engine calls managed hooks
   * • Sidecar model → Separate process communicates with managed code
   * • V-table interoperability → Native calls managed interface methods
  */

  /// <summary>
  /// Start the Reverse‑P/Invoke test and run the demonstration.
  /// </summary>
  public static void Start()
  {
    /*
     * Ablauf:
     * • C# exportiert eine Funktion (UnmanagedCallersOnly)
     * • C# erzeugt einen echten nativen Funktionszeiger
     * • C++ registriert diesen Callback
     * • C++ ruft später diesen Managed‑Callback auf
     */

    /*
     * Procedure:
     * • C# exports a function (UnmanagedCallersOnly)
     * • C# generates a genuine native function pointer
     * • C++ registers this callback
     * • C++ later calls this managed callback
     */

    TestReversePInvoke();
    Console.WriteLine();
  }

  /// <summary>
  /// Performs the actual reverse P/Invoke test:
  /// C# creates a function pointer, passes it to C++, 
  /// and C++ then calls this managed callback.
  /// </summary>
  private static void TestReversePInvoke()
  {
    Console.WriteLine($"{nameof(TestReversePInvoke)}:");

    /*
     * Schritt 1:
     * C# erzeugt einen echten nativen Funktionszeiger (delegate* unmanaged)
     * auf eine Methode, die mit [UnmanagedCallersOnly] markiert ist.
     * Dieser Zeiger kann von C++ direkt aufgerufen werden.
     */

    /*
     * Step 1:
     * C# generates a true native function pointer (delegate* unmanaged)
     * to a method marked with [UnmanagedCallersOnly].
     * This pointer can be called directly from C++.
     */
    var func_ptr = ReverseCallbacks.FunctionPtr;
    Console.WriteLine("C# (Managed) exports a function (UnmanagedCallersOnly)");

    // Pointer an C++ übergeben → C++ speichert ihn für spätere Aufrufe
    // Pass pointer to C++ → C++ stores it for later calls
    Callbacks.RegisterCallback(func_ptr);

    /*
     * Schritt 2:
     * C# ruft eine native Funktion auf.
     * Die native Funktion ruft den zuvor registrierten Managed‑Callback auf.
     */

    /*
     * Step 2:
     * C# calls a native function.
     * The native function calls the previously registered managed callback.
     */
    Console.WriteLine("A: C# calls native function");
    Callbacks.NativeDoWork();

    /*
     * Schritt 3:
     * Gleicher Ablauf, aber mit zusätzlichem Textparameter.
     * C++ ruft den Managed‑Callback mit dem übergebenen Text auf.
     */

    /*
     * Step 3:
     * Same procedure, but with an additional text parameter.
     * C++ calls the managed callback with the transferred text.
     */
    Console.WriteLine("B: C# calls native function with text transfer");
    Callbacks.NativeDoWork("--- Hello World from Native C++! ---");
  }
}
