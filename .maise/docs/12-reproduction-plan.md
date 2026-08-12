# 12 — Reproduction Plan

## Purpose

This is a blueprint for rebuilding Serialize.Linq from this specification
alone, without access to the original source code. It orders the work into
milestones, states what each milestone must deliver, and states how to know
each milestone is done.

**Confidence: High** for the component, data-model, and API scope of each
milestone below, since each one is a direct restatement of a High-confidence
finding elsewhere in this specification. **Confidence: Medium** for the
milestone ordering itself, since a rebuilding team could reasonably resequence
some steps (for example, building both text formats together rather than one
per milestone) without changing the outcome.

## Required Components

| Component | Required? | Reference |
|---|---|---|
| Expression Serialization API (the only runtime component) | Required | [03-services-and-frontends.md](03-services-and-frontends.md) |
| Continuous Integration Pipeline | Required for release, not for the library's own runtime behavior | [10-operational-requirements.md](10-operational-requirements.md) |
| Backend Service, Frontend, Data Store, Message Queue, or any other infrastructure category | Not required | [02-architecture.md](02-architecture.md) — "Components Not Present" |

## Required Frontends

None. Do not budget design or front-end engineering time for this product.
See [04-ui-specification.md](04-ui-specification.md).

## Required Data Model

Implement the full Expression Entity family, plus the Reference Entity and
Supporting Entity families, exactly as inventoried in
[05-data-and-storage.md](05-data-and-storage.md). Every entity must convert
one way from a real expression node, and back the other way, given a shared
rebuild context.

## Required APIs

Implement every operation listed in
[06-apis-and-integrations.md](06-apis-and-integrations.md), across at least
two text formats (a straightforward format and a more compact, structured
format), plus the five extension points listed there.

## Required Workflows

Implement, in this order, all four workflows in
[09-workflows.md](09-workflows.md): Serialize, Deserialize, Deserialize
Untrusted Text Safely, and Publish a New Library Version.

## Required Security Model

Implement the Type Access Control rule
([08-business-rules.md](08-business-rules.md)) before shipping any version
intended for use with untrusted input. Default to the unrestricted behavior
only if backward compatibility with an existing caller base is a stated
project goal; otherwise, consider defaulting to restricted, since an
unrestricted default is the specification's one identified security risk.

## Required Operational Capabilities

- A reproducible, locked dependency-restore mechanism, enforced by the build
  pipeline (fail the build if the lock does not match).
- A release pipeline that builds, tests, packages, and — only from the main
  line of source control — publishes, using a short-lived credential rather
  than a stored long-lived one.
- A duplicate-version-publish guard that does not fail the pipeline, paired
  with a way for the maintainer to notice a skipped publish (this
  specification's rebuild should improve on the original by adding this
  missing feedback signal — see
  [13-open-questions.md](13-open-questions.md)).

## Suggested Implementation Order

1. **Milestone 1 — Entity model and pure conversion.**
   Build the full Expression Entity and Reference Entity family, and the
   pure, in-memory conversion step (real expression tree to entity tree, and
   back), with no text format yet.
   **Acceptance criteria**: every expression kind in
   [05-data-and-storage.md](05-data-and-storage.md) converts to an entity
   and back to an equal expression, verified by an automated structural
   comparison, not by a superficial equality check.

2. **Milestone 2 — Text formats.**
   Add at least one text format layer (for example, a JSON format), plus the
   known-type registration mechanism (manual, automatic array/list
   expansion, and automatic discovery).
   **Acceptance criteria**: a representative expression, including one using
   a custom enumeration type and one using a containment check against a
   list, serializes and deserializes correctly through the chosen format.

3. **Milestone 3 — Captured-value simplification and deep-chain safety.**
   Add the logic that simplifies captured local values into constants, and
   the logic that reshapes very long chained conditions before conversion.
   **Acceptance criteria**: an expression referencing a local variable
   serializes correctly; a very long chained condition (test with at least
   ten thousand terms) converts, rebuilds, and compiles without a stack
   overflow.

4. **Milestone 4 — Type restriction and security hardening.**
   Add the rebuild context, the type restriction extension point, and the
   specific exception raised on a rejected type.
   **Acceptance criteria**: an allow-listed type round-trips; a
   non-allow-listed type, including as a generic argument of an otherwise
   allowed generic type, is rejected with the correct exception.

5. **Milestone 5 — Remaining extension points and second text format.**
   Add the custom assembly-loader, custom value-conversion, custom
   entity-construction, and (for the structured-markup format only) custom
   serialization-behavior extension points; add the second text format.
   **Acceptance criteria**: each extension point has at least one automated
   test proving the host application's custom logic is actually invoked.

6. **Milestone 6 — Defect-driven hardening pass.**
   Work through every rule in [08-business-rules.md](08-business-rules.md)
   that traces to a historical defect (nullable values, date/time kind and
   epoch handling, compiler-generated types, anonymous/dynamic results,
   indexer access, default-value expressions, self-referential types,
   private-member read timing, chained method calls, interface-typed
   collections combined with bitwise operators), and add one reproducing
   automated test per rule before implementing the fix for that rule.
   **Acceptance criteria**: every rule in
   [08-business-rules.md](08-business-rules.md) has a passing, named,
   reproducing test.

7. **Milestone 7 — Release pipeline.**
   Stand up the build, test, package, and publish pipeline described in
   [10-operational-requirements.md](10-operational-requirements.md),
   including the locked-dependency restore gate and the short-lived
   publishing credential.
   **Acceptance criteria**: a version bump on the main line of source
   control results in a new package on the target package registry, with no
   long-lived credential stored anywhere in the pipeline configuration.

## Risks

| Risk | Mitigation |
|---|---|
| Underestimating the breadth of expression kinds to support. | Treat the entity inventory in [05-data-and-storage.md](05-data-and-storage.md) as a fixed checklist, not a starting sketch. |
| Treating the type-restriction control as an afterthought. | Build it in Milestone 4, before any general availability release, not as a later patch. |
| Skipping the defect-driven test set because the underlying platform "obviously" handles these cases. | Every rule in [08-business-rules.md](08-business-rules.md) exists because a real, working system got that case wrong once; assume the same risk applies to a fresh implementation. |
| Wide platform-version support becoming a maintenance burden. | Decide explicitly, before Milestone 1, which of the nine original target platform versions the rebuilt product must actually support, rather than defaulting to all of them. |

## Assumptions

- The rebuilding team has access to a platform with an equivalent
  expression-tree query technology; without one, Milestone 1 is not
  meaningful, since the entire product exists to serialize that technology's
  own expression trees.
- The rebuilding team treats "the same wire-format shape as the original" as
  optional, not required, unless interoperating with existing text produced
  by the original product is an explicit goal.

## Cross-References

Every milestone above links back to its full specification: architecture in
[02-architecture.md](02-architecture.md), the data model in
[05-data-and-storage.md](05-data-and-storage.md), the API surface in
[06-apis-and-integrations.md](06-apis-and-integrations.md), the rule set in
[08-business-rules.md](08-business-rules.md), the workflows in
[09-workflows.md](09-workflows.md), and operations in
[10-operational-requirements.md](10-operational-requirements.md).
