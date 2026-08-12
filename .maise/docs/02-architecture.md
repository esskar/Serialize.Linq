# 02 — Architecture

## System Context

Serialize.Linq is a single embedded **API** component. A host application
loads the library into its own process and calls it directly, in the same
way it calls any other library code. There is no network hop between the
host application and Serialize.Linq.

```
┌───────────────────────────────┐
│      Host Application          │
│  (any host-platform application)│
│                                 │
│   ┌─────────────────────────┐  │
│   │   Serialize.Linq API     │  │
│   │   (embedded component)   │  │
│   └─────────────────────────┘  │
└───────────────────────────────┘
        │                    ▲
        │ resolves type names │ (reads, at rebuild time only)
        ▼                    │
┌───────────────────────────────┐
│  Types already loaded in the   │
│  host process                  │
└───────────────────────────────┘
```

**Confidence: High.**

## Component Overview

| Component | Category | Description |
|---|---|---|
| Expression Serialization API | API (embedded, in-process) | The only runtime component. Converts an expression tree to and from JSON, XML, or plain text. |
| Continuous Integration Pipeline | (build/release automation, not a runtime component) | Builds, tests, packages, and publishes the library. See [10-operational-requirements.md](10-operational-requirements.md). |

No other component exists. See "Components Not Present" below.

**Confidence: High.**

## Backend Services

None. See [00-overview.md](00-overview.md) — "Major Backend Services."

## Microservices

None.

## Frontends

None. See [04-ui-specification.md](04-ui-specification.md).

## Micro-Frontends

None.

## APIs

One API surface, entirely in-process (not reachable over a network):

- **Conversion operations** — turn an expression tree into an entity tree,
  and the reverse.
- **Text serialization operations** — turn an entity tree into JSON, XML, or
  plain text, and the reverse.
- **Convenience operations** — single-call shortcuts that combine conversion
  and text serialization (for example, "give me the JSON text for this
  expression tree" in one call).

Full operation list: [06-apis-and-integrations.md](06-apis-and-integrations.md).

**Confidence: High.**

## Data Flow

### Serialization direction

1. The host application hands the library a live expression tree.
2. If the tree contains a very long chain of "and"/"or" conditions, the
   library reshapes it into a balanced shape first. (Rule: Deep Expression
   Reshaping.)
3. The library walks the tree and builds a matching entity tree. While doing
   so, it replaces references to captured local values (from the host
   application's own closures) with plain constant entities, so the entity
   tree does not depend on the host application's internal, non-serializable
   objects.
4. The library hands the entity tree to the chosen format layer (JSON, XML,
   or plain text), which walks the entity tree and produces text.
5. The text is returned to the host application. Serialize.Linq keeps no copy.

### Deserialization direction

1. The host application hands the library a text value (and, optionally, a
   rebuild context carrying a type restriction rule).
2. The format layer parses the text back into an entity tree.
3. The library walks the entity tree and rebuilds a live expression tree.
   Every time it must resolve a type by name, it checks the type restriction
   rule, if one was supplied. A rejected type stops the rebuild with an
   exception. (Rule: Type Access Control.)
4. The rebuilt expression tree is returned to the host application, which
   may compile and run it.

**Confidence: High.** Traced directly from the conversion and format-layer
components.

## Authentication

Serialize.Linq performs no authentication of its own. It has no concept of a
signed-in user, a session, or a credential.

The library's **release pipeline** authenticates itself to the package
registry using a short-lived token obtained from the source control host's
identity mechanism, instead of a long-lived stored secret. See
[10-operational-requirements.md](10-operational-requirements.md).

**Confidence: High.**

## Authorization

Serialize.Linq has one authorization-like control: the **type restriction
rule**, applied only during rebuild (deserialization). The host application
supplies an allow-list of permitted types, or a custom rule function. The
library checks every type it resolves against this rule and rejects a type
that fails the check, even if that type name was already resolved earlier in
the same rebuild.

Without an explicit rule, every type resolves without restriction. This
default favors compatibility with existing callers over safety, so the host
application must opt in to the restriction.

See [07-user-roles-and-permissions.md](07-user-roles-and-permissions.md) and
[08-business-rules.md](08-business-rules.md).

**Confidence: High.**

## Deployment Topology

Serialize.Linq has no deployment topology of its own. It is packaged as a
distributable library unit and becomes part of whatever host application
includes it. The host application's own deployment topology governs where
and how the combined result runs.

**Confidence: High.**

## Runtime Environments

The library builds for nine target platform versions, covering both the
older Windows-only platform lineage and the newer, cross-platform lineage,
plus two portable baseline profiles. This lets one release run inside host
applications built on very different platform versions, from long-lived
Windows-only systems to the newest cross-platform runtime.

**Confidence: High.**

## Scaling

Serialize.Linq holds no shared state between calls, other than settings
objects the host application creates and owns itself. Each conversion call is
independent. Scaling the library means scaling the host application; the
library places no additional constraint on that.

**Confidence: High.**

## High Availability

Not applicable. The library has no running instance, no uptime, and no
health check. Availability is entirely a property of the host application.

**Confidence: High.**

## Disaster Recovery Assumptions

Not applicable to the library's runtime behavior, because it holds no data.
The only disaster-recovery-relevant asset is the source code and release
history, held by the source control host and the package registry.

**Confidence: High.**

## Security Boundaries

The one meaningful security boundary in the whole system is drawn **during
rebuild (deserialization) of untrusted text**:

```
 Untrusted text  ──▶  Format layer  ──▶  Entity tree  ──▶  Rebuild step
                                                              │
                                                              ▼
                                            Type restriction rule (optional,
                                            host-supplied) checks every
                                            resolved type name
                                                              │
                                              ┌───────────────┴───────────────┐
                                              ▼                               ▼
                                     Type allowed → rebuild continues   Type rejected → exception,
                                                                          rebuild stops
```

Without a type restriction rule, text from an untrusted source can cause the
rebuild step to construct **any type already loaded in the host process**,
because the default type-resolution behavior searches every loaded assembly
by name. This is the library's documented equivalent of a well-known, older
security weakness in the host platform's own, now-removed binary
serialization mechanism (see [01-functional-specification.md](01-functional-specification.md) —
"Out-of-Scope Functionality").

See [08-business-rules.md](08-business-rules.md) — Rule: Type Access Control,
and [13-open-questions.md](13-open-questions.md).

**Confidence: High.**

## Components Not Present

The following component categories, listed in the documentation template,
were checked for and were not found in Serialize.Linq:

| Category | Present? | Why not |
|---|---|---|
| Backend Service / Microservice | No | No network listener or hosted process exists. |
| Frontend / Micro-Frontend | No | No user interface exists. |
| Data Store, Cache, Search Engine, Object Storage, Message Queue | No | The library holds no persistent or shared runtime state. |
| Identity Provider | No | The library authenticates no user. |
| Scheduler, Background Worker, Event Processor | No | Every operation is synchronous and caller-driven; nothing runs on a timer or a queue. |

**Confidence: High.**
