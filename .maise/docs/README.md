# Serialize.Linq — Reverse-Engineered Functional Specification

## Purpose

This documentation is a reverse-engineered functional specification for
Serialize.Linq. It was built directly from the product's own source code and
tests, not from any external design document.

The goal is to let an independent engineering team understand, maintain, or
rebuild Serialize.Linq without needing access to the original source code.

## Scope

### Covered

- Functional behavior and business capability
- Architecture and component structure
- The library's one API surface and its extension points
- Business rules extracted from the source code and from historical defect
  tests
- Workflows (serialize, deserialize, restricted deserialize, release)
- The data (entity) model
- Infrastructure needed to build and release the library
- Operational requirements
- Security model
- Non-functional requirements

### Intentionally Not Covered

- Source code, in any form
- Programming language names, framework names, or build-tool names
- Internal implementation detail not needed to understand behavior
- Package-manager or dependency-specific detail beyond what operational
  understanding requires

## Analysis Summary

| Item | Count | Confidence |
|---|---|---|
| Backend services | 0 | High |
| Frontend applications | 0 | High |
| Micro-frontends | 0 | High |
| APIs | 1 (embedded, in-process) | High |
| External integrations | 2 (package registry; source-control identity mechanism) | High |
| Data stores | 0 | High |
| User roles | 2 (Integrating Developer; Library Maintainer) | High |
| Business workflows | 4 | High |
| Business entities | 26 (16 expression entities, 6 reference entities, 4 supporting entities) | High |
| Business rules | 16 | High |
| Infrastructure components (build/release only) | 3 (source control host; continuous integration runner; package registry) | High |

Serialize.Linq is a single embedded code library with no networked
component. Most template categories built for a multi-service application
(frontends, data stores, message queues, identity providers) do not apply
here, and are recorded as explicitly not applicable, with reasoning, rather
than left blank.

## Documentation Index

| File | Purpose |
|---|---|
| [00-overview.md](00-overview.md) | Executive summary |
| [01-functional-specification.md](01-functional-specification.md) | Complete functional specification |
| [02-architecture.md](02-architecture.md) | System architecture |
| [03-services-and-frontends.md](03-services-and-frontends.md) | The one backend component; confirms no frontend exists |
| [04-ui-specification.md](04-ui-specification.md) | Confirms no user interface exists, against every template subsection |
| [05-data-and-storage.md](05-data-and-storage.md) | Data (entity) model and storage |
| [06-apis-and-integrations.md](06-apis-and-integrations.md) | API operations and external integrations |
| [07-user-roles-and-permissions.md](07-user-roles-and-permissions.md) | Roles and the security model |
| [08-business-rules.md](08-business-rules.md) | Extracted business rules |
| [09-workflows.md](09-workflows.md) | Business workflows |
| [10-operational-requirements.md](10-operational-requirements.md) | Operational requirements |
| [11-non-functional-requirements.md](11-non-functional-requirements.md) | Quality attributes |
| [12-reproduction-plan.md](12-reproduction-plan.md) | Blueprint for rebuilding the product |
| [13-open-questions.md](13-open-questions.md) | Assumptions and unresolved items |

## Architectural Summary

Serialize.Linq converts a query or filter, held as an expression tree in a
host application's memory, into JSON text, XML text, or plain text — and
converts that text back into a working expression tree. It is not a hosted
service; it is code the host application links directly into its own
process and calls like any other library function.

- **Major component**: one embedded API (the Expression Serialization API),
  covering conversion, formatting, known-type management, and restricted
  rebuild.
- **Major data stores**: none. All data is transient and owned by the
  calling host application.
- **External systems**: a package registry (distribution) and the source
  control host's identity mechanism (used only by the release pipeline, to
  authenticate a publish action without a stored long-lived credential).
- **Authentication model**: not applicable at run time; the release pipeline
  uses short-lived, token-exchange-based authentication to the package
  registry.
- **Deployment model**: the library is published as a versioned package and
  pulled into whatever host application includes it; the host application's
  own deployment model governs the combined result.

See [00-overview.md](00-overview.md) for the full narrative and a diagram.

## Confidence Statement

Nearly every statement in this documentation carries **High confidence**,
because Serialize.Linq is a small, single-purpose library whose complete
behavior is directly observable in its source code and in its automated test
suite — including roughly two dozen tests that each document one specific,
previously reported defect and its fix.

A small number of statements carry **Medium confidence**, where the source
code shows the mechanism clearly but the real-world business motivation
behind it is inferred rather than stated outright (for example, which kinds
of host applications actually use the library). No statement in this
documentation carries an unlabeled assumption presented as fact; every
inferred or unknown item is called out explicitly, most fully in
[13-open-questions.md](13-open-questions.md).

## Assumptions

- The reader is treated as unfamiliar with the original source code, and
  every functional claim is traceable to an observed behavior, not to
  external knowledge about the product.
- "The host application" is used throughout as the generic term for any
  program that includes Serialize.Linq; no specific host application was
  identified or assumed.
- Where the template requested information about a component category this
  product does not have (a frontend, a data store, a scheduler, and
  similar), this documentation states "not applicable" with a reason, rather
  than omitting the section.

## Known Gaps

- Real-world adoption context (who uses this library, and why) is not
  visible from source code alone. See
  [13-open-questions.md](13-open-questions.md).
- No stated numeric test-coverage target was found.
- A small number of legacy build-tooling artifacts exist in the repository
  whose current relevance could not be confirmed from the code alone.
- Business-continuity planning around the release pipeline's publishing
  identity was not found and is flagged as a gap, not confirmed as either
  present or absent in practice.

## Recommendations

- **Product owner review** — confirm the assumed business purpose and usage
  scenarios in [00-overview.md](00-overview.md).
- **Business stakeholder review** — confirm there is no unseen consumer-
  facing product built on top of Serialize.Linq that would need its own,
  separate specification.
- **User acceptance review** — not applicable in the traditional sense;
  substitute a developer-experience review of the API surface in
  [06-apis-and-integrations.md](06-apis-and-integrations.md).
- **Architecture review** — confirm no additional component exists outside
  the reviewed repository.
- **Operations review** — validate the release pipeline description in
  [10-operational-requirements.md](10-operational-requirements.md) against
  its live configuration.
- **Security review** — specifically evaluate the unrestricted-by-default
  deserialization behavior described in
  [08-business-rules.md](08-business-rules.md) and
  [02-architecture.md](02-architecture.md) against the requesting
  organization's own risk tolerance.
- **Independent redevelopment planning** — use
  [12-reproduction-plan.md](12-reproduction-plan.md) as the starting
  blueprint.

## Document Conventions

- **Confidence levels**:
  - **High** — directly observed in the source code or an automated test.
  - **Medium** — the mechanism is observed directly, but the stated business
    reason behind it is inferred.
  - **Low** — a plausible inference with limited direct support.
  - **Unknown** — no evidence was found either way.
- **Terminology** — one approved term is used per concept across every
  document (for example, "entity" always means a serializable business
  object in the data model; "workflow" always means a named, multi-step
  business process). The same term is never reused for two different
  concepts.
- **Cross-referencing** — every document links to related documents using
  its file name; follow these links rather than expecting one document to
  repeat another's full detail.
- **Assumptions and open questions** — every assumption is marked inline
  where it appears, and every open question is collected in
  [13-open-questions.md](13-open-questions.md).

## Revision Information

| Version | Date | Description |
|---|---|---|
| 1.0 | 2026-08-12 | Initial reverse-engineered specification, generated from the source tree at commit `e7197c4` (Serialize.Linq version 4.4.1). |
