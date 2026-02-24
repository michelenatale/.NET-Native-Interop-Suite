#include "pch.h"
#include <cstdio>
#include "callbacks.h"


// This source file implements the native side of the Reverse P/Invoke bridge.
// Managed code (via NativeAOT) provides an unmanaged function pointer that
// native code can store and invoke at any time. This enables event-style
// communication from C++ back into C#.
 

/// <summary>
/// Holds the managed callback function pointer registered from C#.
/// Once set, native code can invoke this callback at any time.
/// </summary>
static managed_callback_t g_callback = nullptr;


/// <summary>
/// Calls the registered managed callback with a fixed message.
/// Demonstrates a simple native → managed invocation.
/// </summary>
EXP32 void native_do_work()
{
  if (g_callback)
    g_callback("Hello World from Native C++!");
}


/// <summary>
/// Calls the registered managed callback with a caller‑provided message.
/// This allows native code to forward arbitrary UTF‑8 text to managed code.
/// </summary>
/// <param name="msg">UTF‑8 encoded message string.</param>
EXP32 void native_do_work_msg(const char* msg)
{
  if (g_callback)
    g_callback(msg);
}


/// <summary>
/// Registers a managed callback function pointer.  
/// The pointer must reference a method compiled with UnmanagedCallersOnly,
/// ensuring it is a valid native entry point callable from C++.
/// </summary>
/// <param name="cb">Managed callback function pointer.</param>
EXP32 void register_managed_callback(managed_callback_t cb)
{
  g_callback = cb;
}
