

//See Project >> Properties >> PreBuildEvent >> (Event Before Creation)
//See Project >> Properties >> PostBuildEvent >> (Event After Creation)

//Siehe Projekteigenschaften >> Ereignis vor dem Erstellen (PreBuildEvent)
//Siehe Projekteigenschaften >> Ereignis nach dem Erstellen (PostBuildEvent)

//Zuerst durchlesen:
//Proceed-PInvoke-de.txt
//Proceed-NativeAOT-de.txt
//Proceed-LibraryImport-de.txt
//Proceed-NativeLibrary-de.txt

//Read through first:
//Proceed-PInvoke-en.txt
//Proceed-NativeAOT-en.txt
//Proceed-LibraryImport-en.txt
//Proceed-NativeLibrary-en.txt

namespace NetLanguageInteroperability.Test;


using LanguageInteroperability;

public class Program
{
  public static void Main()
  {
    TestCsToC();
  }


  #region C# to C

  private static void TestCsToC()
  {

    //CSharp →  C - P/Invoke	
    TestCsToCPInvoke();


    //CSharp →  C - LibraryImport	
    TestCsToCLibraryImport();


    //CSharp →  C - NativeAOT	
    TestCsToCNativeAOT();
  }



  private static void TestCsToCPInvoke()
  {
    CsToC.StartPInvoke();
  }


  private static void TestCsToCLibraryImport()
  {
    CsToC.StartLibraryImport();
  }

  private static void TestCsToCNativeAOT()
  {
    CsToC.StartNativeAOT();
  }

  #endregion CSharp to C
}