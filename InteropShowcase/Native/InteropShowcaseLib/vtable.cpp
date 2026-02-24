#include "pch.h"
#include <stdio.h>
#include "vtable.h"


// This source file demonstrates how native code consumes a managed V‑Table.
// The V‑Table contains unmanaged function pointers exported from C# via
// NativeAOT and UnmanagedCallersOnly. Native code can invoke these functions
// exactly as if they were implemented in C or C++.


/// <summary>
/// Uses the managed service V‑Table by invoking its exported functions.
/// Demonstrates calling managed code from native code through a function table.
/// </summary>
/// <param name="svc">Pointer to the managed service V‑Table.</param>
EXP32 void use_managed_service(managed_service_vtable* svc)
{
  // Call the managed logging function
  svc->log("Hello World from Native C++ via V-Table!");

  // Call the managed addition function and log the result
  char buf[64];
  snprintf(buf, sizeof(buf), "21 + 21 = %d", svc->add(21, 21));
  svc->log(buf);
}
