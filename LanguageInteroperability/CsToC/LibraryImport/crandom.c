#include <stdlib.h> 
#include <time.h> 

__declspec(dllexport) void fill_random_lib_import(unsigned char* buffer, int length)
{ 
  srand((unsigned)time(NULL)); 
  for (int i = 0; i < length; i++) 
    buffer[i] = rand() % 256; 
}