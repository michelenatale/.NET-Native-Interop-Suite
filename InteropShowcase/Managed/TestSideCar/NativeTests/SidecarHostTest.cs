

namespace michele.natale;

/// <summary>
/// Provides a simple test harness for starting and interacting with the Sidecar host.
/// </summary>
/// <remarks>
/// This class is intended for manual testing and demonstration purposes.
/// It initializes the Sidecar, sends a test command, and waits for user input
/// before shutting the system down.
/// </remarks>
internal class SidecarHostTest
{
  /// <summary>
  /// Starts the Sidecar host, sends a test command, and waits for user input.
  /// </summary>
  /// <remarks>
  /// The method performs the following steps:
  /// <list type="number">
  /// <item>Creates a <see cref="SidecarHost"/> instance using a shared ring buffer.</item>
  /// <item>Starts the native Sidecar worker thread.</item>
  /// <item>Sends a test command (<c>PING</c>) to the Sidecar.</item>
  /// <item>Yields the current thread to allow the Sidecar to process the command.</item>
  /// <item>Waits for the user to press ENTER.</item>
  /// <item>Stops and disposes the Sidecar.</item>
  /// </list>
  /// This method is useful for verifying that the full host–sidecar pipeline
  /// (shared memory, callbacks, threading, and event flow) is functioning correctly.
  /// </remarks>
  public static void Start()
  {
    using var sidecar = new SidecarHost("SidecarRB", 4096);

    sidecar.Start();

    var cmd = System.Text.Encoding.ASCII.GetBytes("PING");
    sidecar.SendCommand(cmd);

    // Give the Sidecar thread a scheduling opportunity
    Thread.Yield();

    Console.WriteLine("Press ENTER to exit...\n");
    Console.ReadLine();

    sidecar.Stop();
  }
}

