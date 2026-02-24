# 📘 **3. interop-best-practices.md**

## Interop Best Practices

This document outlines recommended patterns, guidelines, and considerations for building safe and efficient .NET ↔ Native interoperability layers.

---

## 1. Memory Management

### 1.1 Pinning
- Use `GCHandle.Alloc(..., Pinned)` for strings or buffers passed to native code  
- Ensure pinned memory is freed in `Dispose()`  
- Avoid long‑term pinning of large objects  

### 1.2 Lifetime Rules
- Native code must never store pointers to temporary managed memory  
- Managed wrappers must ensure native handles are closed exactly once  

---

## 2. Unmanaged Function Pointers

### 2.1 delegate* unmanaged
- Fastest possible callback mechanism  
- No marshaling  
- ABI‑stable  

### 2.2 UnmanagedCallersOnly
- Required for reverse P/Invoke  
- Methods must be static  
- No capturing of managed state  

---

## 3. Shared Memory IPC

### 3.1 Lock‑Free Ring Buffer
- Use atomic head/tail indices  
- Avoid mutexes  
- Use modulo arithmetic for wrap‑around  

### 3.2 Zero‑Copy
- Data is written directly into shared memory  
- No intermediate buffers  
- Ideal for high‑frequency communication  

---

## 4. Threading Considerations

### 4.1 Thread Priority
- Sidecar thread can be boosted for responsiveness  
- Avoid time‑critical priority unless necessary  

### 4.2 Yield vs Sleep
- `Thread.Yield()` allows immediate Sidecar execution  
- Small sleeps (1–2 ms) prevent busy loops  

---

## 5. Error Handling

- Always check for `IntPtr.Zero`  
- Validate buffer sizes  
- Fail fast on invalid handles  
- Use exceptions only in managed code  

---

## 6. Debugging Interop

### 6.1 Native Debugging
- Use WinDbg or Visual Studio native debugger  
- Inspect shared memory regions  
- Check thread states  

### 6.2 Managed Debugging
- Use logging in callbacks  
- Validate pointer lengths  
- Check for GCHandle leaks  

---

## 7. General Recommendations

- Keep interop layers thin  
- Avoid unnecessary marshaling  
- Prefer explicit memory ownership  
- Document all ABI boundaries  
- Keep native and managed code in sync  

---

These practices ensure that your interop layer remains fast, safe, and maintainable — even as the project grows in complexity.
