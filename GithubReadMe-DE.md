# Net-Language-Interoperability

Eine Sammlung kleiner Beispiele und Anleitungen, wie .NET (C#) mit nativen Bibliotheken (C/C++) interagiert: P/Invoke, LibraryImport und NativeAOT. Dieses Repository enthält mehrere kleine Projekte, Testfälle und ausführliche Schritt‑für‑Schritt‑Anleitungen (Deutsch / English).

Kurz (DE): Beispiele für Interoperabilität .NET ↔ native C, mit Fokus auf Windows x64 und NativeAOT.  
Short (EN): Small, focused examples for .NET ↔ native C interoperability (P/Invoke, LibraryImport, NativeAOT). Windows/x64 oriented.

---

Badges (optional)
- Build / CI: (add when you enable GitHub Actions)
- License: MIT (or your chosen license)

Inhaltsverzeichnis
- Overview
- Repository structure (Projekt- und Dateiauflistung)
- Architekturdiagramm (ASCII + Mermaid)
- Projects — kurze Einführung & technische Details
  - NativeLibrary
  - LanguageInteroperability/CsToC
    - CsToC/LibraryImport
    - CsToC/NativeAOT
  - TestLanguageInteroperability
  - TestNativeLibrary
- Wie die Teile zusammenarbeiten (technisch)
- Quickstart (Build & Run) — Windows (empfohlen)
- Troubleshooting & Tipps
- Empfehlungen / Verbesserungen
- Contributing & License
- Zusammenfassung / Conclusion

---

Overview
========
Dieses Repo zeigt verschiedene Wege, wie C# Code mit nativen Bibliotheken interagiert:
- P/Invoke (klassisches DllImport)
- LibraryImport (Source-generated/library import available in modern .NET)
- NativeAOT (kompilieren eines .NET-Library zu nativer DLL und Verwendung aus nativen Komponenten oder umgekehrt)

Das Ziel ist Lern- und Referenzmaterial: verständliche Beispiele, die zeigen, was jeweils benötigt wird und welche Fallstricke es gibt.

Repository structure (high level)
================================
- /NativeLibrary
  - NativeAOT example: Library.cs, .csproj and textual how‑to (Proceed-NativeLibrary-*.txt)
- /LanguageInteroperability/CsToC
  - /LibraryImport
    - crandom.c (native C random example), instructions and LibraryImport example (LibraryImport.cs)
  - /NativeAOT
    - instructions to produce add_aot.dll that links to NativeLibrary.lib
- /TestLanguageInteroperability
  - Test harness (Program.cs) that runs the three approaches (P/Invoke, LibraryImport, NativeAOT)
- /TestNativeLibrary
  - Native-side test harness (C/C++ test scaffolding)
- Proceed-*.txt (EN / DE): ausführliche Build-Anleitungen
- WhatCanBeDeleted.txt: Hinweise zu temporären Build-Ordnern
- LICENSE (changeable by you)

Architekturdiagramm
===================
ASCII diagram (simple):

LanguageInteroperability (C#)                       NativeLibrary (C# NativeAOT)
+-----------------------------+                    +----------------------+
| TestLanguageInteroperability |  <---uses---       | NativeLibrary (AOT)  |
| - CsToC.StartPInvoke()       |  <---P/Invoke-->   | - exports aot_add    |
| - CsToC.StartLibraryImport() |  <---LibraryImport->| - NativeLibrary.lib  |
| - CsToC.StartNativeAOT()     |  <---native dll--->| - aot_add.c          |
+-----------------------------+                    +----------------------+
            ^
            |
       builds / runs
       (Windows x64)

Mermaid (if supported on GitHub, uncomment to render):
```mermaid
flowchart LR
  A[TestLanguageInteroperability C#] -->|P/Invoke| B[crandom.dll (native)]
  A -->|LibraryImport| C[crandom.dll]
  A -->|NativeAOT (published)| D[NativeLibrary native DLL]
  D -->|provides| E[aot_add entry point]
```

Projects — detaillierte Beschreibung
====================================

NativeLibrary
------------
Kurz: Ein kleines .NET-Projekt, das mit NativeAOT in eine native DLL kompiliert werden kann.  
Was es macht:
- Definiert exportierte Methoden mit [UnmanagedCallersOnly(EntryPoint="...")] (z. B. aot_add).
- Wird via `dotnet publish -c Release -r win-x64` als native DLL (NativeAOT) erzeugt.
Warum nützlich:
- Zeigt, wie man .NET-Code in native Form bringt, der von anderen nativen Komponenten oder Tools verwendet werden kann.
Wichtiges:
- csproj verwendet <PublishAot>true</PublishAot>, <DisableRuntimeMarshalling>true</DisableRuntimeMarshalling>, Platform x64.
- Build gehört auf Windows x64 (Anleitungen enthalten dumpbin checks).

LanguageInteroperability/CsToC
------------------------------
Dies ist die zentrale Sammlung von C#→C-Interoperabilitätsbeispielen.

CsToC/LibraryImport
- Kurz: Demonstriert moderne LibraryImport-Attribute in C# (source-gen style) zum Aufruf nativer DLLs.
- Wichtige Datei: LanguageInteroperability/CsToC/LibraryImport/LibraryImport.cs
  - Beispiel: [LibraryImport(CRandFile)] private static partial void fill_random_lib_import(Span<byte> buffer, int length);
- Native-Baustein: crandom.c → crandom.dll (gebaut mit cl /LD).
- Hinweis: Die Beispiel-Implementierung verwendet Span<byte> & stackalloc — modernes, allocation-arm.

CsToC/NativeAOT
- Kurz: Zeigt den Ablauf, wie man eine NativeAOT-compiled .NET DLL (NativeLibrary) erstellt und wie man daraus ggf. eine weitere native DLL (add_aot.dll) generiert, die auf der NativeLibrary.lib linkt.
- Wichtig: Die erzeugten nativen Artefakte (NativeLibrary.lib, aot_add.c) werden gebraucht, um native Wrapper zu bauen.

TestLanguageInteroperability
----------------------------
Kurz: Konsolen-Testprojekt, das die drei Ansätze sequenziell ausführt:
- TestCsToCPInvoke() → ruft P/Invoke-Beispiel auf.
- TestCsToCLibraryImport() → ruft LibraryImport-Beispiel auf.
- TestCsToCNativeAOT() → ruft NativeAOT-Beispiel auf.
Warum es nützlich ist:
- Einfache Smoke-Tests, die demonstrieren, ob die nativen DLLs korrekt in die Ausgabe kopiert und aufrufbar sind.

TestNativeLibrary
-----------------
Kurz: (Native) C/C++ Testprojekt; Hilft beim Ausprobieren und Debuggen der nativen Komponenten.

Wie die Teile zusammenarbeiten (technisch)
=========================================
- P/Invoke (DllImport): C# deklariert extern Methoden, der Runtime lädt zur Laufzeit eine native DLL (crandom.dll) und ruft Funktionen auf. Parameter werden per Platform Invoke marshalled; Entwickler muss auf genaue Signaturen & Calling Conventions achten.
- LibraryImport: Moderner Mechanismus (source-gen / LibraryImport attribute) ermöglicht effizientere, manchmal statischeren Import mit besseren Performanzoptionen. Im Beispiel werden Span<byte> und stackalloc verwendet, um Heap-Allocations zu vermeiden.
- NativeAOT: .NET-Projekt wird mit NativeAOT zu einer nativen DLL/Library kompiliert. Methoden können mit [UnmanagedCallersOnly] exportiert werden. Zusätzlich werden .lib-Artefakte erzeugt, die native Linker verwenden können (z. B. cl /LD add_aot.c NativeLibrary.lib /Fe:add_aot.dll).
- Die Testprojekte orchestrieren das Zusammenspiel: Sie erwarten, dass native DLLs im Output-Ordner liegen oder per PostBuildEvent dorthin kopiert werden.

Quickstart — Build & Run (Windows x64)
=====================================
Voraussetzungen
- .NET 10 SDK
- Visual Studio (Build tools) / cl, dumpbin, x64 Native Tools Command Prompt
- Windows x64 runner / machine

Schnelles Build (manuell)
1. Klonen:
   git clone https://github.com/michelenatale/Net-Language-Interoperability.git
2. Build crandom.dll (native):
   - Öffne "x64 Native Tools Command Prompt"
   - cd LanguageInteroperability\CsToC\LibraryImport
   - cl /LD crandom.c /Fe:crandom.dll
   (Für Release-like build: cl /LD /O2 /GL /Gy /DNDEBUG crandom.c /Fe:crandom.dll)
3. Build NativeLibrary (NativeAOT):
   - cd NativeLibrary
   - dotnet publish -c Release -r win-x64
   - Ergebnis liegt üblicherweise in bin\x64\Release\net10.0\win-x64\publish (native DLLs / .lib)
4. Build & Run Test Project:
   - dotnet build TestLanguageInteroperability
   - dotnet run --project TestLanguageInteroperability

Hinweise zu Pfaden
- Wenn Testprojekte native Dlls erwarten, kopiere crandom.dll und ggf. add_aot.dll / NativeLibrary.dll in den Ausgabeordner von TestLanguageInteroperability (z. B. bin\x64\Debug\net10.0\CsToC\NativeAOT).
- Alternativ: füge PostBuildEvent in csproj hinzu, das die nativen Artefakte automatisch kopiert.

Praktische csproj PostBuild snippet (Beispiel)
```xml
<Target Name="CopyNativeLibs" AfterTargets="Build">
  <ItemGroup>
    <NativeFiles Include="$(SolutionDir)LanguageInteroperability\CsToC\LibraryImport\crandom.dll" Condition="Exists('$(SolutionDir)LanguageInteroperability\CsToC\LibraryImport\crandom.dll')" />
  </ItemGroup>
  <Copy SourceFiles="@(NativeFiles)" DestinationFolder="$(OutputPath)" SkipUnchangedFiles="true" />
</Target>
```

Troubleshooting & Tips
======================
- "Dll not found" → Prüfe, ob die native DLL im Runtime-Ausgabeordner liegt (bin/.../net10.0/...).
- "BadImageFormatException" oder falsche arch → Stelle sicher, dass die native DLL x64 ist, nicht x86.
- dumpbin /headers <dll> | findstr machine → prüft Architektur (8664 → x64).
- Verwende Path.Combine statt harter Backslashes in Code.
- Für Debug: baue crandom.dll ohne /O2 oder mit /Zi /Od, damit Symbols bleiben.
- Wenn LibraryImport/Span verwendet werden: achte auf Lifetimes (stackalloc vs heap) und sichere Marshaling-Größen.

Empfehlungen / Verbesserungen (kurz)
====================================
- README: (done) zentrale Übersicht (dieses Dokument).
- .gitignore: entferne bin/obj aus Repo history falls committet (git rm --cached).
- CI: GitHub Actions Workflow (windows-latest) zur Automatisierung des Builds & Smoke Tests.
- Packaging: Release-Builds als Assets bereitstellen (z. B. native DLLs, ZIP).
- Cross-platform: Ergänzende Anleitungen für mingw/clang falls du Linux/macOS unterstützen willst.
- Tests: Unit/Integration tests, die native Interop-Pfade automatisch testen (assert output format).
- Dokumentation: Single Source of Truth (englisch & deutsch), Zusammenfassung zu Marshaling-Regeln.

Contributing & License
======================
- Du bist alleiniger Rechteinhaber; wenn du die Lizenz änderst → ersetze LICENSE mit der MIT-Datei (du hast das erwähnt).  
- Wenn andere beitragen, empfiehlt sich CONTRIBUTING.md und ISSUE/PR Templates.

Summary / Conclusion
====================
Dieses Repository ist ein kompaktes, praxisorientiertes Set von Beispielen, das drei häufige Wege der .NET ↔ native Interoperabilität demonstriert: P/Invoke, LibraryImport und NativeAOT. Die Beispiele sind gut dokumentiert (EN/DE) und enthalten konkrete Build‑Anleitungen für Windows/x64.  
Nächste sinnvolle Schritte: README zentralisieren (dieses Dokument), .gitignore bereinigen, einfache CI (Windows) einrichten und PostBuild-Scripts hinzufügen, die native Artefakte automatisch in die erwarteten Output-Ordner kopieren. Dadurch wird das Repo zuverlässiger, leichter nutzbar und ein besseres Lernbeispiel für andere Entwickler.

Wenn du willst, kann ich das README direkt als PR in dein Repo anlegen (branch + PR) oder zusätzlich eine .gitignore, GitHub Actions Workflow und eine Beispiel csproj-PostBuild-Task hinzufügen. Sag mir kurz, ob ich das als Pull Request anlegen soll — dann erstelle ich Branch + Commit + PR-Titel & Beschreibung für dich.