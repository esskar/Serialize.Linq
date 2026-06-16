# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Serialize.Linq is a .NET library that serializes and deserializes LINQ `Expression` trees to/from XML, JSON, and plain text. It is multi-targeted and published as a NuGet package (`Serialize.Linq`). The solution and all projects live under `src/`.

## Commands

All commands run from the repository root; the solution is `src/Serialize.Linq.sln`.

```bash
# Restore + build (all target frameworks, all configurations)
dotnet build src/Serialize.Linq.sln

# Build a single configuration
dotnet build src/Serialize.Linq.sln -c Release

# Run the full test suite (test project targets net9.0)
dotnet test src/Serialize.Linq.sln

# Run a single test class or method (MSTest filter syntax)
dotnet test src/Serialize.Linq.Tests/Serialize.Linq.Tests.csproj --filter "FullyQualifiedName~ExpressionSerializerTests"
dotnet test src/Serialize.Linq.Tests/Serialize.Linq.Tests.csproj --filter "Name=YourTestMethodName"
```

- The main library multi-targets `net48;net481;net6.0;net7.0;net8.0;net9.0;netstandard2.0;netstandard2.1` (see `src/Serialize.Linq/Serialize.Linq.csproj`). Building all targets requires the full set of .NET SDKs / targeting packs installed; restrict with `-f <tfm>` when iterating locally.
- `GeneratePackageOnBuild` is true, so building the library produces a `.nupkg`. The assembly is strong-name signed with `Serialize.Linq Signing Key.snk`.
- Custom configurations `Debug Optimize Size` / `Release Optimize Size` define the `SERIALIZE_LINQ_OPTIMIZE_SIZE` constant, which shortens serialized XML/JSON property names. When editing `Node`/`ExpressionNode` classes, check for `#if SERIALIZE_LINQ_OPTIMIZE_SIZE` blocks — serialization member names differ between the two builds.

## Architecture

The serialization pipeline is: **`Expression` → node tree (`ExpressionNode`) → wire format (XML/JSON/text)**, and the reverse for deserialization. Three layers cooperate:

1. **Nodes** (`src/Serialize.Linq/Nodes/`) — DTOs mirroring the `System.Linq.Expressions` hierarchy. Each expression kind has a corresponding `*ExpressionNode` (e.g. `BinaryExpressionNode`, `LambdaExpressionNode`), plus reflection-info nodes (`MethodInfoNode`, `MemberInfoNode`, `TypeNode`, etc.). `ExpressionNode` is the abstract root. A node both converts *from* an `Expression` (in its constructor) and *back* via a `ToExpression(ExpressionContext)` method. These types are the serialization contract — changing their shape or member names is a breaking change to the wire format.

2. **Factories** (`src/Serialize.Linq/Factories/`) — `NodeFactory` walks an `Expression` and produces the matching `ExpressionNode` tree (big type switch in `NodeFactory.Create`). `DefaultNodeFactory` adds parameter-type awareness for lambdas. `TypeResolverNodeFactory` and `FactorySettings` control type handling. The compression step (`Internals/ExpressionCompressor`) runs before factory conversion to collapse closure/constant access.

3. **Serializers** (`src/Serialize.Linq/Serializers/`) — `ExpressionConverter` does `Expression ⇄ ExpressionNode`. `ExpressionSerializer` extends it and adds `ISerializer`-based reading/writing to streams/strings. Concrete `ISerializer` implementations: `JsonSerializer`, `XmlSerializer` (both via `DataSerializer` using `DataContractSerializer`/`DataContractJsonSerializer`), and `TextSerializer`. `SerializerBase` manages the "known types" set (`AddKnownType`, `AutoAddKnownTypesAsArrayTypes/ListTypes`) needed by the data-contract serializers to resolve concrete types.

**Public entry points:** users construct `new ExpressionSerializer(new JsonSerializer())` (or `XmlSerializer`), then call `SerializeText` / `DeserializeText` / `Serialize` / `Deserialize`. Convenience extension methods live in `Extensions/ExpressionExtensions.cs` (`ToJson`, `ToXml`, `ToText`, `ToExpressionNode`).

**Type resolution & security:** deserialization reconstructs types and members by name via `Internals/` helpers (`DefaultAssemblyLoader`, `KnownTypes`, `ValueConverter`, `TypeExtensions`). `IAssemblyLoader` / `INodeFactory` / the `Factories` settings are the extension points for constraining or customizing which types can be resolved. Note: `BinaryFormatSerializer` was removed in v4.0 due to `BinaryFormatter` security concerns — do not reintroduce `BinaryFormatter`-based serialization.

## Projects

- `src/Serialize.Linq/` — the library.
- `src/Serialize.Linq.Tests/` — MSTest suite (net9.0). Regression tests for reported bugs go in `Serialize.Linq.Tests/Issues/` named `Issue<number>.cs`; per the README, **new bug fixes should come with a reproducing test**.
- `src/Serialize.Linq.Tests.Container/` — a netstandard2.0 helper assembly (`ExpressionContainer`) referenced by the tests; it provides expressions defined in a *separate* assembly so cross-assembly type resolution can be exercised. It is not test code itself.

## CI / Release

`.github/workflows/build.yml` (GitHub Actions) builds and tests `src/Serialize.Linq.sln` in Release on a **Windows** runner — Windows is required because the library targets `net48`/`net481`. On pushes to `main` it packs the library and publishes the `.nupkg` (+ `.snupkg` symbols) to NuGet via `dotnet nuget push --skip-duplicate`. Authentication uses **NuGet trusted publishing (OIDC)** — the `NuGet/login` action exchanges the workflow's GitHub OIDC token (`id-token: write` permission) for a short-lived key, so no long-lived `NUGET_API_KEY` secret is stored. The trusted-publishing policy on nuget.org is bound to this repo and the `build.yml` workflow. The published version comes from `<Version>` in `src/Serialize.Linq/Serialize.Linq.csproj`, so bump it before releasing — `--skip-duplicate` means a push without a version bump is silently skipped rather than failing.
