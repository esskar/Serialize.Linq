# 06 — APIs and Integrations

## Summary

Serialize.Linq exposes **one API**: a set of in-process operations a host
application calls directly. The API is not reachable over a network, has no
authentication of its own, and has no rate limit, because every call happens
inside the caller's own process on the caller's own thread.

**Confidence: High.**

## API: Expression Serialization API

| Field | Value |
|---|---|
| Purpose | Convert an expression tree to text, and text back to an expression tree. |
| Provider | Serialize.Linq, embedded in the host application. |
| Consumer | The host application's own code. |
| Authentication | Not applicable — in-process call, no identity boundary. |
| Authorization | The optional type restriction rule, applied only on the deserialize (text-to-expression) direction. See [08-business-rules.md](08-business-rules.md) — Rule: Type Access Control. |
| Internal or external | Internal (in-process). |
| Versioning | Governed by the package version number; a breaking change to an entity's shape is a breaking change to previously produced text. |
| Rate limits | Not applicable. |
| Retry behavior | Not applicable — the library performs no retry; a failure is a single exception raised to the caller. |
| Timeouts | Not applicable — every operation is synchronous and unbounded by the library itself. |
| Error handling | See [01-functional-specification.md](01-functional-specification.md) — "Error Handling." |

### Operations

| Operation | Direction | Input | Output |
|---|---|---|---|
| Convert expression to entity tree | Serialize (step 1) | An expression tree, plus optional conversion settings. | An entity tree. |
| Convert expression directly to JSON text | Serialize (combined) | An expression tree, plus optional settings or a custom assembly layer / format layer. | JSON text. |
| Convert expression directly to XML text | Serialize (combined) | Same as above. | XML text. |
| Convert entity tree, or expression, to text through a chosen format layer | Serialize (combined, format-agnostic) | An expression tree (or entity tree), plus the chosen format layer. | Text, in whichever format the chosen format layer produces. |
| Serialize to a stream | Serialize | An expression-derived entity tree and an output stream. | The entity tree's JSON, XML, or other stream-based format written to the stream. |
| Serialize to text | Serialize | An expression-derived entity tree. | A JSON or XML string (only for a format layer that supports text). |
| Deserialize from a stream | Deserialize | An input stream, plus optional rebuild context. | A rebuilt expression tree. |
| Deserialize from text | Deserialize | JSON or XML text, plus optional rebuild context. | A rebuilt expression tree. |
| Configure known-type registration | Configuration | One or more types, or a toggle to auto-register array/list variants, or a toggle to auto-discover types by inspecting constant values. | Updated registration state on the serializer. |
| Register a custom value-conversion rule | Configuration | A target type (or "any type") and a conversion function. | Updated conversion state on the internal value converter. |
| Register a custom assembly-loader | Configuration | An implementation supplying the list of assemblies to search when resolving a type name. | Used automatically during every subsequent rebuild through that context. |
| Register a custom serialization behavior for otherwise-unsupported types | Configuration (XML format only) | An implementation of the host platform's serialization-surrogate contract. | Used automatically by the XML format layer; not honored by the JSON format layer, due to a limitation in the underlying JSON engine. |

**Confidence: High.** Enumerated directly from the library's public entry
points.

### Binary Format — Removed

An extension point for a binary format still exists in the API shape (a
contract for byte-array input/output), but **no built-in binary format
implementation ships with the library**. A previous built-in binary
implementation was removed in version 4.0 because its underlying mechanism
carried a well-known security weakness. A host application that wants a
binary format must supply its own implementation of the extension point.

**Confidence: High.**

## External Integration: Package Registry

| Field | Value |
|---|---|
| Purpose | Distribute the built library package to host applications. |
| Data exchanged | The built package (compiled library plus a matching symbols package) and its version-numbered metadata. |
| Trigger | A push of a new commit to the main line of source control. |
| Frequency | On demand, whenever a maintainer merges a change and bumps the version number. |
| Failure behavior | Publishing an already-published version number is silently skipped, not treated as an error. Any other publish failure fails the release pipeline. |

**Confidence: High.**

## External Integration: Source Control Host's Identity Mechanism

| Field | Value |
|---|---|
| Purpose | Let the release pipeline prove its identity to the package registry without a stored, long-lived secret. |
| Data exchanged | A short-lived identity token, exchanged for a short-lived package-registry credential. |
| Trigger | Every release pipeline run that reaches the publish step. |
| Frequency | Once per qualifying pipeline run. |
| Failure behavior | If the token exchange fails, the publish step fails and no package is published. |

**Confidence: High.**

## Internal Extension Points (Not External Integrations, but Integration-Shaped)

These let a host application customize the library's behavior without
modifying the library itself. They are documented here because they are the
library's designed "seams" for integration with a larger system:

| Extension point | Purpose |
|---|---|
| Custom assembly-loader | Control which loaded assemblies the library searches when resolving a type name during rebuild — for example, to narrow the search in a security-sensitive host. |
| Custom type restriction rule | Restrict which resolved types are acceptable during rebuild. See [08-business-rules.md](08-business-rules.md). |
| Custom value-conversion rule | Teach the library how to convert a value to a type it cannot convert automatically. |
| Custom assembly layer (node factory) | Change how the library decides which parts of an expression tree become entities versus which parts are simplified into plain constants. |
| Custom serialization behavior (XML only) | Let a type that the format layer cannot serialize on its own participate anyway, through the host platform's serialization-surrogate mechanism. |

**Confidence: High.**

## Cross-References

- Business rules enforced through these operations: [08-business-rules.md](08-business-rules.md)
- Workflows built from these operations: [09-workflows.md](09-workflows.md)
- The component providing this API: [03-services-and-frontends.md](03-services-and-frontends.md)
- Release pipeline detail: [10-operational-requirements.md](10-operational-requirements.md)
