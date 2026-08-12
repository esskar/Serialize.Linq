# 03 — Backend Services and Frontends

## Summary

Serialize.Linq has **no backend service, no microservice, no frontend, and no
micro-frontend**. It has exactly one runtime component: an embedded API that
runs inside the host application's own process. This document records that
finding against every field the documentation template requires, so the gap
is explicit rather than silent.

**Confidence: High.**

## Component: Expression Serialization API

| Field | Value |
|---|---|
| Name | Expression Serialization API |
| Category | API (embedded, in-process — not network-reachable) |
| Purpose | Convert an expression tree to JSON, XML, or plain text, and convert that text back into an expression tree. |
| Why it exists | To let a host application move query or filter logic across a process boundary — over a network call, into storage, or between components — without losing its exact structure. |
| Responsibilities | Walk an expression tree and build a matching entity tree; reshape overly deep condition chains; simplify captured local values into constants; convert the entity tree to and from text; manage the list of extra types the format layer must know ahead of time; rebuild an expression tree from an entity tree, applying an optional type restriction rule. |
| Inputs | An expression tree (for serialization); JSON, XML, or plain text, plus optional settings and an optional rebuild context (for deserialization). |
| Outputs | JSON, XML, or plain text (for serialization); a rebuilt expression tree, optionally compiled into a runnable delegate (for deserialization). |
| Dependencies | The host process's own loaded types, discovered through the platform's built-in reflection facility. No external service dependency. |
| Data owned | None persisted. All data is transient, scoped to one call, and owned by the caller before and after the call. |
| External interactions | Reads the list of types already loaded in the host process when resolving a type name during rebuild. No outbound network call, no file access, no external service call. |
| APIs exposed | See [06-apis-and-integrations.md](06-apis-and-integrations.md) for the full operation list. |
| APIs consumed | None. |
| Background processing | None. Every operation is synchronous and runs on the calling thread. |
| Security responsibilities | Enforce the host-supplied type restriction rule during rebuild, for every resolved type, on every resolution (including repeat resolutions of an already-seen type name). |
| Failure behavior | Raises a typed exception for each of the specific failure conditions in [01-functional-specification.md](01-functional-specification.md) — "Error Handling." Does not retry, does not degrade gracefully, and does not log. The failure always surfaces directly to the calling code. |
| Scaling considerations | None beyond the host application's own scaling. The component holds no shared or cross-call state. |

**Confidence: High.**

## Component: Continuous Integration Pipeline

This is a build/release automation component, not a runtime component of the
delivered library. It is documented here for completeness because it is the
only other identifiable "component" in the repository.

| Field | Value |
|---|---|
| Name | Continuous Integration Pipeline |
| Category | Not a listed runtime category — a build and release automation process. |
| Purpose | Build the library for every supported target, run the automated test suite, package the library, and publish the package to the package registry. |
| Why it exists | To make every release repeatable, tested, and traceable, without a manual, error-prone release step. |
| Responsibilities | Restore dependencies using a locked, reproducible dependency list; build; run tests; package; publish to the package registry only from the main line of source control. |
| Inputs | Source code changes pushed to, or proposed against, the main line of source control. |
| Outputs | A built and tested package, uploaded as a build artifact; on a successful push to the main line, a published package on the package registry. |
| Dependencies | The source control host's build-runner service; the package registry's publishing endpoint; a short-lived authentication token obtained through the source control host's identity mechanism. |
| Data owned | None business-relevant. Owns only its own build artifacts and logs. |
| External interactions | Publishes to the package registry; authenticates through the source control host's identity mechanism. |
| APIs exposed | None. |
| APIs consumed | The package registry's publish operation. |
| Background processing | Runs on each qualifying push or proposed change; not on a fixed schedule. |
| Security responsibilities | Uses a short-lived, narrowly scoped token instead of a stored long-lived credential, to reduce the impact of a leaked credential. |
| Failure behavior | A duplicate version publish is silently skipped rather than treated as an error; every other step fails the pipeline outright on error. |
| Scaling considerations | Not applicable; one pipeline run per triggering change. |

See [10-operational-requirements.md](10-operational-requirements.md) for
full detail.

**Confidence: High.**

## Cross-References

- Runtime architecture: [02-architecture.md](02-architecture.md)
- Full API operation list: [06-apis-and-integrations.md](06-apis-and-integrations.md)
- Business rules enforced by this component: [08-business-rules.md](08-business-rules.md)
- Workflows this component participates in: [09-workflows.md](09-workflows.md)
- Operational detail for the pipeline: [10-operational-requirements.md](10-operational-requirements.md)
- Rebuild plan entry for this component: [12-reproduction-plan.md](12-reproduction-plan.md)
