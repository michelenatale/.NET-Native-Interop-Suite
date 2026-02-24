# .NET Native Interop Suite

A collection of high‑performance, real‑world examples and architectural patterns for native interoperability in .NET.  
This suite brings together multiple projects that demonstrate how managed and unmanaged code can interact efficiently using modern .NET features such as:

- P/Invoke and LibraryImport  
- NativeAOT  
- Reverse P/Invoke  
- Unmanaged function pointers (V‑Tables)  
- Shared memory ring buffers  
- Lock‑free cross‑process communication  
- Sidecar worker architectures  

The goal of this suite is to serve as a **technical reference**, **learning resource**, and **foundation** for developers who want to build robust and high‑performance .NET ↔ Native integrations.

---

## 📦 Projects in this Suite

### **1. InteropShowcase**
A comprehensive demonstration of advanced .NET ↔ Native interoperability techniques, including:

- Shared memory ring buffers  
- Reverse P/Invoke  
- Unmanaged function pointers  
- Lock‑free IPC  
- A fully functional Sidecar worker model  

🔗 https://github.com/michelenatale/.NET-Native-Interop-Suite/tree/main/InteropShowcase

---

### **2. NetLanguageInteroperability**
A minimal, focused project demonstrating language interoperability between:

- C  
- C++  
- C#  

Using:

- P/Invoke  
- LibraryImport  
- NativeAOT  

This project also shows how managed C# code can be compiled into a native library and consumed from C/C++ without Visual Studio.

🔗 https://github.com/michelenatale/.NET-Native-Interop-Suite/tree/main/NetLanguageInteroperability

---

## 🧭 Purpose of the Suite

The **.NET Native Interop Suite** is designed to:

- Provide clear, practical examples of native interop  
- Demonstrate modern .NET features for low‑level integration  
- Offer architectural guidance for high‑performance systems  
- Serve as a reference for developers building native extensions, plugins, or sidecar processes  
- Explore advanced patterns such as shared memory IPC and reverse callbacks  

Each project is intentionally kept **clean**, **minimal**, and **well‑documented** to make the concepts easy to understand and reuse.

---

## 📚 Documentation

Each module contains its own documentation under `/Docs`, including:

- Architecture overviews  
- Interop best practices  
- Example output  
- Project structure  
- Technical explanations  

---

## 🛠 Technologies Used

- .NET 8 / .NET 9 / .NET 10
- NativeAOT  
- C / C++  
- Shared memory APIs  
- UnmanagedCallersOnly  
- delegate* unmanaged  
- Lock‑free data structures  

---

## 🤝 Contributions

This suite is intended to grow over time.  
Future modules may include:

- Diagnostics tools  
- Benchmarks  
- Additional IPC mechanisms  
- Native plugin architectures  
- AOT‑optimized interop helpers  

Contributions, ideas, and discussions are welcome.

---

## 📄 License

This project is open‑source.  
See the LICENSE file for details.
