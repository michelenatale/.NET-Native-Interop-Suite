#include <stdio.h> 
#include <stdlib.h> 

//Zuerst Proekt 'NativeLibrary' erstellen.
//Windows-Startmenu und Suchen "x64 Native Tools"
//Den Pfad dort hin setzen wo add_aot.c und NativeLibrary.lib sind.
//Command-Befehl wie folgt, es wird "add_aot.dll" erstellt.
//cl /LD add_aot.c NativeLibrary.lib /Fe:add_aot.dll

//'NativeLibrary.dll' und 'add_aot.dll' in das Verzeichnis
//x:\[..]\NetLanguageInteroperability\TestLanguageInteroperability\bin\x64\Debug\net10.0\CsToC\NativeAOT
//>> !!!!weil add_aot.dll nachher auf NativeLibrary.dll zugreift !!!!!


__declspec(dllimport) int aot_add(int a, int b);


__declspec(dllexport) int add_aot(int a, int b)
{
  return aot_add(a, b);
}


////Das funktioniert auch, einbindung der dll wäre dann dynamisch
//typedef int (*aot_add_fn)(int, int);
//
//__declspec(dllexport) int add_aot(int a, int b)
//{
//  HMODULE lib = LoadLibraryA("NativeLibrary.dll");
//  if (!lib)
//  {
//    printf("DLL nicht gefunden\n"); return -1;
//  }
//
//  aot_add_fn fn = (aot_add_fn)GetProcAddress(lib, "aot_add");
//  if (!fn)
//  {
//    printf("Funktion nicht gefunden\n");
//    return -1;
//  }
//
//  int result = fn(a, b);
//  FreeLibrary(lib);
//  return result;
//}