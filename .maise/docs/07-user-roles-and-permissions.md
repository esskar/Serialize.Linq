# 07 — User Roles and Permissions

## Summary

Serialize.Linq has no sign-in, no session, and no traditional permission
system. It has two roles, neither of which is an "end user" in the usual
sense, plus one security control that behaves like an authorization rule.

**Confidence: High.**

## User Roles

| Role | Description | Confidence |
|---|---|---|
| Integrating Developer | Writes the host application's code that calls Serialize.Linq. Chooses the format, sets conversion and known-type settings, and — if handling untrusted input — configures the type restriction rule. This is the library's only true "user." | High |
| Library Maintainer | Reviews contributions, fixes reported defects (each with a reproducing test, per project policy), decides the version number, and triggers a release by merging to the main line of source control. | High |

No other role exists. There is no administrator role, no read-only role, and
no end-user role, because the library has no user interface and no
multi-tenant runtime state to separate between users.

**Confidence: High.**

## Permissions

Serialize.Linq defines no permission model in the access-control sense. The
nearest equivalent is a set of **settings the Integrating Developer chooses
in code**, each of which changes what the library is allowed to do on the
developer's own behalf:

| Setting | Effect |
|---|---|
| Allow private member access | Lets the library read and rebuild non-public fields and properties, not only public ones. |
| Relaxed type names | Chooses a shorter, more portable type-name form in the produced text, except for compiler-generated types, which always use the fully qualified form. |
| Known-type registration | Declares which extra types the format layer must be ready to handle. |
| Type restriction rule | Declares which types are allowed to be reconstructed when rebuilding from text. |

**Confidence: High.**

## Authentication

Not applicable at the library level — see
[02-architecture.md](02-architecture.md) — "Authentication." The release
pipeline authenticates itself to the package registry using a short-lived
token; that is a build-time concern, not a runtime permission concern for
the library's own API.

## Authorization

The library's one authorization-like control is the **type restriction
rule**, checked during rebuild (deserialization):

- If the Integrating Developer supplies a rule, every resolved type is
  checked against it, and a rejected type stops the rebuild with an
  exception naming the rejected type.
- If no rule is supplied, every type resolves without restriction, for
  compatibility with code written before this control existed.

This is the library's documented way of preventing an untrusted payload from
causing the rebuild step to construct an unexpected or dangerous type — the
same problem a different, now-removed serialization mechanism in the
platform is known to have had.

See [08-business-rules.md](08-business-rules.md) — Rule: Type Access
Control.

**Confidence: High.**

## Administrative Users

Not applicable to the runtime library. At the project level, the Library
Maintainer plays this role for the source code and release process only. See
[10-operational-requirements.md](10-operational-requirements.md).

## Read-Only Users

Not applicable. The library has no multi-user state to read.

## Privileged Operations

The only "privileged" runtime operation is rebuilding an expression tree
from text obtained from a source the Integrating Developer does not fully
trust. The project's documentation explicitly recommends that this operation
always be paired with a type restriction rule.

**Confidence: High.**

## Visibility Rules

Not applicable — there is no multi-user data to hide or reveal.

## Security Assumptions

| Assumption | Confidence |
|---|---|
| The Integrating Developer, not the library, decides whether input text is trusted or untrusted. | High |
| The library will not, on its own, add a type restriction rule; the default is unrestricted, for backward compatibility. | High |
| A type restriction rule, once attached to a rebuild context, cannot be bypassed by resolving the same type name twice — the check re-runs even on a cache hit. | High |
| The library never evaluates or enumerates a constant value's contents while discovering extra types automatically, specifically to avoid triggering a side effect (for example, materializing a lazily-evaluated data source) as an unintended consequence of type discovery. | High |

## Cross-References

- Security boundary detail: [02-architecture.md](02-architecture.md) — "Security Boundaries"
- Full rule text: [08-business-rules.md](08-business-rules.md)
- Restricted-deserialize workflow: [09-workflows.md](09-workflows.md)
- Open question about real-world trust boundaries: [13-open-questions.md](13-open-questions.md)
