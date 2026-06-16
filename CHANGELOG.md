# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

This changelog was reconstructed from the project's commit history and published
NuGet releases; entries for older versions are a best-effort summary.

## [4.3.0] - 2026-06-16

### Added
- `ITypeFilter` extension point on `ExpressionContext` to restrict which types may be
  resolved while deserializing an expression tree — the equivalent of `BinaryFormatter`'s
  `SerializationBinder` for guarding against untrusted payloads. A rejected type throws
  `TypeNotAllowedException`. Built-in implementations `AllowedTypesFilter` (allow-list by
  type/open-generic/namespace) and `DelegateTypeFilter` (predicate) are provided, plus new
  `ExpressionContext` constructor overloads accepting an `ITypeFilter` and a
  `Deserialize(Stream, IExpressionContext)` overload ([#151]).

### Notes
- The filter is consulted for every resolved type, including the open generic definition
  and each generic argument of a closed generic (e.g. `List<Customer>` is checked as
  `List<>` and `Customer`). It is applied on cache hits too, so a restricted context cannot
  be bypassed. With no filter set (the default), behavior is unchanged and all types resolve.

## [4.2.0] - 2026-06-16

### Added
- Option to pass a custom `ISerializationSurrogateProvider` to the serializers via the new
  `DataSerializer.SerializationSurrogateProvider` property, allowing callers to customize
  serialization and add support for otherwise unsupported types ([#178]).

### Notes
- The surrogate provider is honored by the XML serializer only. The JSON serializer
  (`DataContractJsonSerializer`) does not apply surrogate providers
  ([dotnet/runtime#100553](https://github.com/dotnet/runtime/issues/100553)).
- The property is available on the `net6.0`–`net10.0` and `netstandard2.1` targets;
  it is not exposed on `net48`/`net481`/`netstandard2.0`, which lack
  `ISerializationSurrogateProvider`.

## [4.1.0] - 2026-06-16

### Added
- Support for .NET 10 (`net10.0`).

### Changed
- Replaced AppVeyor with GitHub Actions for CI; publishing now uses NuGet trusted
  publishing (OIDC) instead of a stored API key.
- Added Dependabot configuration for NuGet updates.
- Documentation updates (README supported platforms, project guidance).

## [4.0.0] - 2025-01-19

### Added
- Support for .NET 9 (`net9.0`).

## [3.1.0] - 2023-10-21

### Added
- Support for .NET 8 (`net8.0`) and .NET Framework 4.8.1 (`net481`).
- `DateOnly` and `TimeOnly` added to the known types.

### Fixed
- Serialization of unbound `ToString` expressions ([#169]).

### Removed
- Dropped .NET 5 target (out of support).

## [3.0.0] - 2023-06-10

### Added
- Support for .NET 7 (`net7.0`).
- `netstandard2.0` and `netstandard2.1` targets.
- README is now included in the NuGet package.

### Changed
- Suppressed `SYSLIB0011` warnings during the transition away from `BinaryFormatter`.

### Fixed
- Do not explode interface types as nullable types ([#163]).

### Removed
- Removed `BinaryFormatSerializer` / `BinaryFormatter`-based serialization due to
  security concerns.

## [2.0.0] - 2020-12-12

### Added
- Support for .NET 5 (`net5.0`).
- Symbol package (`.snupkg`) support.

### Changed
- License changed to MIT; removed legacy copyright/license headers.
- Added support for `DefaultExpression` ([#146]).
- `DateTimeOffset` added to the known types ([#107]).

### Removed
- Removed deprecated target frameworks and example projects.

## [1.8.1] - 2019-04-25

### Fixed
- Follow-up fix for finding the real constant value ([#113]).
- Support for `IndexExpression` serialization ([#119]).

## [1.8.0] - 2019-04-25

### Fixed
- Try harder to find the real constant value ([#113]).

## [1.7.3] - 2018-10-26

### Fixed
- Fix for member access on constant values ([#105]).
- Removed obsolete Silverlight/Windows Phone compiler switches.

## [1.7.2] - 2018-10-26

### Added
- `DateTimeOffset` added as a known type ([#107]).

## [1.7.1] - 2018-06-02

### Fixed
- Updated `Microsoft.Data.OData` reference (security alert).

## [1.7.0] - 2018-03-20

- Baseline release.

[Unreleased]: https://github.com/esskar/Serialize.Linq/compare/main...HEAD
[#178]: https://github.com/esskar/Serialize.Linq/issues/178
[#151]: https://github.com/esskar/Serialize.Linq/issues/151
[#169]: https://github.com/esskar/Serialize.Linq/issues/169
[#163]: https://github.com/esskar/Serialize.Linq/pull/163
[#146]: https://github.com/esskar/Serialize.Linq/issues/146
[#119]: https://github.com/esskar/Serialize.Linq/issues/119
[#113]: https://github.com/esskar/Serialize.Linq/issues/113
[#107]: https://github.com/esskar/Serialize.Linq/issues/107
[#105]: https://github.com/esskar/Serialize.Linq/issues/105
