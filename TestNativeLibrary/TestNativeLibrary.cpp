
#include "pch.h"
#include <iostream>

//A NativeAOT .Net DLL is accessed here.

//Please also take a look at the macros of the PreBuildEvents.
//>> Project >> Property > BuildEvents >> PreBuildEvents


//#define EXP32 extern "C" __declspec(dllexport)
#define IMP32 extern "C" __declspec(dllimport)

#pragma comment(lib, "NativeLibrary.lib")
//#pragma comment(lib, "x64\Release\NativeLibrary.lib")


//#ifdef _INTELLISENSE_ 
////So motzt Intellisense nicht
//IMP32 int32_t rng_crypto_int_32() { return 0; } 
//IMP32 int64_t rng_crypto_int_64() { return 0; } 
//#endif


//A NativeAOT .Net DLL is accessed here.
IMP32 int32_t rng_crypto_int_32();
IMP32 int64_t rng_crypto_int_64();

int main()
{
  printf("Crypto Random Int32: %d\n", rng_crypto_int_32());
  printf("Crypto Random Int64: %lld\n", rng_crypto_int_64()); 
  return 0;
}

