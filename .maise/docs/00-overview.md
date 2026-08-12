# 00 — Executive Overview

## Purpose

Serialize.Linq is a software library. An application adds the library to its own
code. The library converts an expression tree to a text format and back.

An expression tree is a data structure. It represents a query or a filter as
data, not as running code. The host platform's query technology (LINQ,
Language Integrated Query) builds expression trees when a developer writes a
query or a filter in code.

Serialize.Linq lets a program:

- Turn an expression tree into JSON text, XML text, or plain text.
- Store that text, or send it to another process.
- Turn the text back into a working expression tree later.
- Compile the restored expression tree into a runnable delegate.

**Confidence: High.** These facts come from the product README and from the
public entry points in the source code.

## Business Domain

The business domain is **data interchange for query and filter logic**. Many
applications build a filter in one process (for example, a web front end) and
must apply that filter in another process (for example, a database server, a
queue consumer, or a stored rule engine). Serialize.Linq exists to move that
filter safely between processes.

**Confidence: High.**

## Main Users

Serialize.Linq has one class of user: a **software developer** who adds the
library to a host application. The developer writes code that calls the
library. The library has no screen, no end-user workflow, and no direct human
user.

A second, smaller group is the **library maintainer**, who reviews
contributions, fixes reported defects, and publishes new versions.

See [07-user-roles-and-permissions.md](07-user-roles-and-permissions.md).

**Confidence: High.**

## Major Capabilities

| Capability | Description | Confidence |
|---|---|---|
| Convert expression to entity tree | Turn an expression tree into an intermediate, serializable entity tree. | High |
| Convert entity tree to wire text | Turn the entity tree into JSON, XML, or plain text. | High |
| Convert wire text back to entity tree | Parse JSON, XML, or plain text back into the entity tree. | High |
| Convert entity tree back to expression | Rebuild a working expression tree, and optionally compile it. | High |
| Restrict which types rebuild | Let the host application block dangerous or unexpected types during rebuild. | High |
| Handle very large expressions safely | Reshape long chained conditions so rebuilding never overflows the call stack. | High |
| Work across many host runtimes | Run on old and new versions of the host platform, and on non-Windows systems. | High |

## System Context

Serialize.Linq is an **embedded API**, not a network service. A host
application links the library directly into its own process. There is no
client-server relationship between the library and its caller.

The library reaches outside its own process in one narrow case: when it
rebuilds an expression tree, it must resolve type names against the types
already loaded in the host process. See
[06-apis-and-integrations.md](06-apis-and-integrations.md) and
[08-business-rules.md](08-business-rules.md) for the security rule that
controls this step.

**Confidence: High.**

## Architecture Overview

The library has one internal pipeline with two directions.

```
                 ┌─────────────────────┐
   Expression    │  Entity Tree        │   Wire Text
   Tree  ───────▶│  (mirrors the       │──────────▶  (JSON, XML,
   (input)       │   expression shape) │             or plain text)
                 └─────────────────────┘
                            ▲   │
                            │   ▼
                 Reverse direction: Wire Text → Entity Tree → Expression Tree
```

Three internal layers cooperate:

1. **Entity layer.** Holds one entity type for every kind of expression node,
   plus entity types for reflection facts (a referenced type, method, field,
   property, or constructor). See [05-data-and-storage.md](05-data-and-storage.md).
2. **Assembly layer.** Walks a live expression tree and produces the matching
   entity tree. Also simplifies captured local values into plain constant
   entities, and reshapes very long chained conditions into a balanced shape.
   See [02-architecture.md](02-architecture.md).
3. **Format layer.** Converts an entity tree to and from JSON text, XML text,
   or plain text, and manages the list of extra types the format layer must
   know about ahead of time. See [06-apis-and-integrations.md](06-apis-and-integrations.md).

**Confidence: High.**

## Major "Backend Services"

None. Serialize.Linq has no backend service and no microservice. It is a
library that runs inside the host application's own process.

**Confidence: High.** No network listener, no service host, and no
independently deployable process exist anywhere in the source tree.

## Major Frontend Applications

None. Serialize.Linq has no frontend and no micro-frontend. It has no user
interface of any kind. See [04-ui-specification.md](04-ui-specification.md)
for the full statement of this finding.

**Confidence: High.**

## Infrastructure Overview

Serialize.Linq needs no database, cache, message queue, search engine, or
object store to run. It keeps no state between calls other than settings the
host application sets on its own objects.

The library's **build and release pipeline** does depend on infrastructure
external to the library itself:

| Infrastructure | Role | Mandatory? |
|---|---|---|
| Source control host | Stores source code, runs the build pipeline. | Mandatory for building/releasing the library. |
| Continuous integration runner (Windows-based) | Builds, tests, and packages the library. | Mandatory for releasing the library. |
| Package registry | Distributes the built package to host applications. | Mandatory for distribution. |

See [05-data-and-storage.md](05-data-and-storage.md) and
[10-operational-requirements.md](10-operational-requirements.md).

**Confidence: High.**

## External Systems

| External system | Role | Confidence |
|---|---|---|
| Package registry | Where the built library is published. Host applications pull the library from here. | High |
| Source control host | Hosts the source code and the release pipeline. | High |

No other external system exists. Serialize.Linq makes no network call, reads
no configuration file, and calls no external service at run time.

**Confidence: High.**

## Risks

| Risk | Description | Confidence |
|---|---|---|
| Unrestricted type rebuild | If a host application rebuilds an expression tree from an untrusted source without setting a type restriction, an attacker-supplied payload can name any type already loaded in the host process. | High — the library ships a specific security control for this and a specific exception type for the rejection case. |
| Silent skip on release pipeline | The release pipeline skips a duplicate package version instead of failing the build. A missed version bump means no new package reaches users, with no build failure to flag it. | High — confirmed in the pipeline configuration. |
| Legacy restore artifacts | The source tree still contains an old package-restore tool and configuration file that the current build no longer uses. | Medium — the files appear unused, but no direct proof of removal safety was found. |

## Open Questions

See [13-open-questions.md](13-open-questions.md) for the full list. Key items:

- No production deployment topology exists to review, because the product is
  a library. Any "deployment" question really means "how does a host
  application configure and call the library," which this specification
  answers only from the library's own public surface.
- The real-world adoption pattern (which kinds of host applications use
  Serialize.Linq, and for what business purpose) is not visible from the
  source code and is marked as unknown.
