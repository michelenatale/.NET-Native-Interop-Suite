
using System.Runtime.InteropServices;

//Switch to x64 architecture.

namespace LanguageInteroperability;



partial class CsToC
{
  private const string AotAddFile = @"CsToC\NativeAOT\add_aot.dll";
  //private const string AotAddFile = @"add_aot.dll";
  [LibraryImport(AotAddFile, EntryPoint = "add_aot")]
  private static partial int add_aot(int a, int b);


  public static void StartNativeAOT()
  {
    TestAotAdd();
  }



  private static void TestAotAdd()
  {
    Console.WriteLine("NativeAOT");


    var rand = Random.Shared;
    var a = rand.Next(10, 100);
    var b = rand.Next(10, 100);

    Console.WriteLine($"{a} + {b} = {add_aot(a, b)}");
    Console.WriteLine();
  }
}
