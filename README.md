# .NET-Native-Interop-Suite

## InteropShowcase

A comprehensive demonstration of high‑performance .NET ↔ Native interoperability using:

- Shared memory ring buffers  
- Reverse P/Invoke  
- Unmanaged function pointers (V‑Tables)  
- Lock‑free cross‑process communication  
- A fully functional Sidecar worker model  

This repository contains two major components:

1. **InteropShowcase** — foundational interop examples (callbacks, V‑Tables, ring buffers)  
2. **SidecarModel** — a production‑grade architecture for hosting a native worker thread (“Sidecar”) communicating with .NET via shared memory and reverse callbacks.

https://github.com/michelenatale/.NET-Native-Interop-Suite/tree/main/InteropShowcase


## NetLanguageInteroperability

According to Wikipedia, [LanguageInteroperability](https://en.wikipedia.org/wiki/Language_interoperability) is the ability of two different programming languages to interact natively as part of the same system and to work on the same types of data structures.

This project shows simple examples in .Net for [interoperability](https://learn.microsoft.com/de-de/dotnet/standard/native-interop/) between C, C++, and C#, including:
- P/Invoke
- LibraryImport
- NativeAOT

The goal is also to demonstrate how managed C# code can be compiled as a native library and then called from native programs (focus on NativeAOT). 

The project is deliberately kept minimal and serves as a technical reference for developers who want to link C and C# code via NativeAOT without using Visual Studio.

https://github.com/michelenatale/.NET-Native-Interop-Suite/tree/main/NetLanguageInteroperability
