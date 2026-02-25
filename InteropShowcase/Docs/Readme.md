# InteropShowcase & SidecarModel

A comprehensive demonstration of high‑performance .NET ↔ Native interoperability using:

- Shared memory ring buffers  
- Reverse P/Invoke  
- Unmanaged function pointers (V‑Tables)  
- Lock‑free cross‑process communication  
- A fully functional Sidecar worker model  

This repository contains two major components:

1. **InteropShowcase** — foundational interop examples (callbacks, V‑Tables, ring buffers)  
2. **SidecarModel** — a production‑grade architecture for hosting a native worker thread (“Sidecar”) communicating with .NET via shared memory and reverse callbacks.

---

## 🚀 Features

- Zero‑copy shared memory IPC  
- Lock‑free ring buffer implementation  
- Native → Managed callbacks via V‑Tables  
- Managed → Native commands via shared memory  
- Sidecar worker lifecycle (Init → Loop → Dispose)  
- Clean separation of responsibilities  
- Fully documented C# and C++ code  

---

## 📂 Repository Structure

```
InteropShowcase/  
│  
├── Docs/  
│   ├── README.md  
│   ├── architecture.md  
│   └── interop-best-practices.md  
│  
├── Managed/  
│   ├── TestInteropShowcase/  
│   └── TestSideCar/  
│  
├── Native/  
│   ├── InteropShowcaseLib/  
│   ├── SidecarModelLib/  
│   └── TestSidecarModelNative/  
│  
└── Build/  
```

---

## 🧪 Running the Examples

### InteropShowcase
Demonstrates:
- Reverse P/Invoke
- Native callbacks
- Managed V‑Tables
- Basic ring buffer usage

### SidecarModel
Demonstrates:
- Full Sidecar worker thread
- Shared memory command pipeline
- Event callbacks back into .NET

Run:
TestInteropShowcase.exe
TestSidecarModel.exe

---

## 📘 Example Output

 ``` 
 [Host] Init
 [Host] Process: 50-49-4E-47
 [Host] Event 1: OK
 [Host] Dispose
```
---

## 📚 Documentation

- [Architecture Overview](https://github.com/michelenatale/.NET-Native-Interop-Suite/blob/main/InteropShowcase/Docs/Architecture.md)
- [Interop Best Practices](https://github.com/michelenatale/.NET-Native-Interop-Suite/blob/main/InteropShowcase/Docs/Interop-Best-Practices.md)

---

## 🧩 Purpose

This project is designed as a learning and reference resource for developers who want to understand:

- How to build high‑performance interop layers  
- How to design safe and efficient cross‑process communication  
- How to structure a Sidecar‑style architecture in .NET  

Enjoy exploring the internals — everything is documented and intentionally transparent.



