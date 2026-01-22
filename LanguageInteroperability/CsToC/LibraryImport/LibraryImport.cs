


using System.Runtime.InteropServices;

//Switch to x64 architecture.

namespace LanguageInteroperability;

partial class CsToC
{

  private const string CRandFile = @"CsToC\LibraryImport\crandom.dll";

 [LibraryImport(CRandFile)]
  private static partial void fill_random_lib_import(Span<byte> buffer, int length);
 

  public static void StartLibraryImport()
  {
    TestRandom();
  }

  private static void TestRandom()
  {
    Console.WriteLine("LibraryImport");
    
    Span<byte> data = stackalloc byte[16];
    fill_random_lib_import(data, data.Length);
    Console.WriteLine($"rng hex:\t{Convert.ToHexString(data)}");

    Console.WriteLine($"rng bytes:\t{string.Join(" ", data.ToArray())}");
    Console.WriteLine();
  }
}
