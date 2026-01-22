


using System.Runtime.InteropServices;

//Switch to x64 architecture.

namespace LanguageInteroperability;

partial class CsToC
{

  private const string CAddPath = @"CsToC\PInvoke\cadd.dll";

  [DllImport(CAddPath, CallingConvention = CallingConvention.Cdecl)]
  public static extern int add(int a, int b);

  public static void StartPInvoke()
  {
    TestAdd();
  }

  private static void TestAdd()
  {
    Console.WriteLine("P/Invoke");

    var rand = Random.Shared;
    var a = rand.Next(10, 100);
    var b = rand.Next(10, 100);

    Console.WriteLine($"{a} + {b} = {add(a, b)}");
    Console.WriteLine();
  }
}
