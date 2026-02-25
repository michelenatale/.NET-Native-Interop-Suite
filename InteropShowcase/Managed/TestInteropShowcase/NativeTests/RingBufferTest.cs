
using System.Text;

namespace michele.natale;

using RingBufferNative;


/// <summary>
/// Demonstrates the use of a lock-free ring buffer for 
/// high-performance producer-consumer scenarios between 
/// managed and native code.
/// The ring buffer operates without locks, uses atomic head/tail pointers, 
/// and is ideal for Interop, NativeAOT, and sidecar architectures.
/// </summary>
internal class RingBufferTest
{
  /*
   * Warum dieses Ringbuffer‑Design Best Practice ist:
   * • Keine Locks → extrem schnell
   * • Atomare head/tail‑Zeiger → thread‑safe ohne Synchronisationskosten
   * • Zero‑Copy → keine unnötigen Marshaling‑ oder Kopierkosten
   * • Plattformunabhängig → C11‑Atomics + CMake
   * • NativeAOT‑freundlich → keine GC‑Abhängigkeiten
   * • Erweiterbar → Shared Memory, Sidecar‑Prozesse, Callbacks, Streaming
   */

  /*
   * Why this ring buffer design is best practice:
   * • No locks → extremely fast
   * • Atomic head/tail pointers → thread-safe without synchronization costs
   * • Zero-copy → no unnecessary marshalling or copying costs
   * • Platform-independent → C11 Atomics + CMake
   * • NativeAOT-friendly → no GC dependencies
   * • Extensible → shared memory, sidecar processes, callbacks, streaming
   */

  /// <summary>
  /// Starts the ring buffer interoperability test.
  /// </summary>
  public static void Start() 
  {
    TestRingBuffer();
  }

  /// <summary>
  /// Performs the basic ring buffer test and shows how producers 
  /// and consumers work independently of each other.
  /// </summary>
  private static void TestRingBuffer()
  {
    Console.WriteLine($"{nameof(TestRingBuffer)}:");

    /*
     * Ein Ringbuffer ist immer ein Producer‑Consumer‑Konstrukt:
     * • Producer schreibt Daten in den Buffer
     * • Consumer liest Daten aus dem Buffer
     * Beide arbeiten unabhängig voneinander.
     * Der Buffer sorgt dafür, dass sie sich nicht blockieren.
     */

    /*
     * A ring buffer is always a producer-consumer construct:
     * • Producer writes data to the buffer
     * • Consumer reads data from the buffer
     * Both work independently of each other.
     * The buffer ensures that they do not block each other.
     */

    TestProducerConsumer();
    Console.WriteLine();
  }

  /// <summary>
  /// Demonstrates a simple producer-consumer scenario:
  /// <para>• C# writes data to the ring buffer (producer)</para>
  /// <para>• C# reads the same data back out (consumer)</para>
  /// In a real scenario, the native part would read or write.  
  /// </summary>
  private static void TestProducerConsumer()
  {
    var capacity = 4096u;

    var rb = RingBuffer.Create(capacity);

    var txt = "Hello World from C#!";
    var data = Encoding.UTF8.GetBytes(txt);
    Console.WriteLine($"My Text = {txt}; length = {data.Length}");

    // Producer (Managed) → Consumer (Native)
    var length = RingBuffer.Write(rb, data, (uint)data.Length);
    Console.WriteLine($"Producer: Wrote {length} bytes into the ring buffer");

    // Producer (Native) → Consumer (Managed)
    var sz = 256u;
    var buffer = new byte[sz];
    var read = RingBuffer.Read(rb, buffer, sz);

    Console.WriteLine($"Consumer: Received text = {Encoding.UTF8.GetString(buffer, 0, (int)read)}");
  }
}
