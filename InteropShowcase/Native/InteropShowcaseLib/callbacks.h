#pragma once
#include <cstdint>
#include "EXP32IMP32.h"


// This header defines the native-side interface for Reverse P/Invoke.
// C# (via NativeAOT) provides unmanaged function pointers that native code
// can call directly. These declarations describe how native code registers
// and invokes those managed callbacks.


/// <summary>
/// Represents a function pointer to a managed callback.
/// The callback receives a UTF‑8 encoded message string.
/// This pointer is provided by C# via NativeAOT using UnmanagedCallersOnly.
/// </summary>
using managed_callback_t = void(*)(const char* msg);

/// <summary>
/// Invokes the previously registered managed callback.
/// This function is called from C# and triggers a call back into managed code.
/// </summary>
EXP32 void native_do_work();

/// <summary>
/// Registers a managed callback function pointer.
/// The pointer must reference a method compiled with UnmanagedCallersOnly,
/// ensuring it is a true native entry point callable from C++.
/// </summary>
/// <param name="cb">The managed callback function pointer.</param>
EXP32 void register_managed_callback(managed_callback_t cb);
