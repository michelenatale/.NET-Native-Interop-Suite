# Architecture Overview

This document provides a deep technical explanation of the architecture behind the InteropShowcase and SidecarModel components.

---

## 1. High‑Level Architecture

```
flowchart LR
    A[Managed Host (.NET)] <--> B[Shared Ring Buffer (Shared Memory)]
    B <--> C[Native Sidecar Worker Thread]
    A <-- Reverse P/Invoke (V‑Table) --> C
```

The system uses:

• Shared memory for high‑throughput command transfer

• Reverse P/Invoke for native → managed callbacks

• A dedicated native worker thread (“Sidecar”) for processing

## 2. Components

### 2.1 Shared Ring Buffer

• Cross‑process shared memory region

• Lock‑free head/tail indices

• Zero‑copy data transfer

• Used for commands from .NET → Native

### 2.2 V‑Table (Unmanaged Function Pointer Table)

• Struct containing unmanaged function pointers

• Passed from .NET to native

• Enables native code to call back into managed code without marshaling overhead

• Used for events from Native → .NET

### 2.3 Sidecar Worker Thread

• Created inside the native DLL

• Runs independently from the .NET thread

• Lifecycle:

  1. Init()
  2. Command loop (read → process → event)
  3. Dispose()

### 2.4 Managed Wrapper (SidecarHost)

• Owns the ring buffer

• Pins the shared memory name

• Builds the V‑Table

• Starts/stops the Sidecar

• Provides a clean .NET API

## 3. Data Flow

### 3.1 Command Flow (Host → Sidecar)
```
.NET → RingBuffer.Write() → Shared Memory → Native Loop → Process()
```
### 3.2 Event Flow (Sidecar → Host)
```
Native → OnEvent() → Reverse P/Invoke → .NET callback
```
---
## 4. Threading Model
Sidecar runs on a dedicated native thread

Thread priority can be increased for responsiveness

Host thread remains free

Thread.Yield() or small sleeps allow immediate Sidecar execution

## 5. Memory Ownership

• Shared memory is created by the host

• Native side opens the same region

• Managed strings are pinned using GCHandle

• V‑Table delegates must remain alive for the entire Sidecar lifetime

## 6. Why This Architecture?

• Extremely fast

• Predictable

• No GC pressure

• No marshaling overhead

• Perfect for real‑time or high‑frequency workloads

• This architecture is a modern, clean, and extensible pattern for .NET ↔ Native integration.

---
