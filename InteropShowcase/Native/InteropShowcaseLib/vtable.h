#pragma once
#include <stdint.h>
#include "EXP32IMP32.h"


// This header defines the native‑side view of the managed service V‑Table.
// The V‑Table contains unmanaged function pointers exported from C# via
// NativeAOT and UnmanagedCallersOnly. Native code can call these functions
// directly as if they were implemented in C or C++.


/// <summary>
/// Represents a table of unmanaged function pointers implemented in managed code.
/// Each entry corresponds to a method exported from C# using NativeAOT.
/// </summary>
struct managed_service_vtable
{
  /// <summary>
  /// Pointer to a managed logging function.
  /// Accepts a UTF‑8 encoded message string.
  /// </summary>
  void (*log)(const char* msg);

  /// <summary>
  /// Pointer to a managed addition function.
  /// Accepts two 32‑bit integers and returns their sum.
  /// </summary>
  int32_t(*add)(int32_t a, int32_t b);
};

/// <summary>
/// Consumes a managed service V‑Table and invokes its exported functions.
/// Native code can call the managed methods through the provided function pointers.
/// </summary>
/// <param name="svc">Pointer to the managed service V‑Table.</param>
EXP32 void use_managed_service(managed_service_vtable* svc);
