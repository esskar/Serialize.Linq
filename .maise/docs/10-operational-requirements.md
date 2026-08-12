# 10 — Operational Requirements

## Required Services

None at run time. A host application that includes Serialize.Linq needs no
additional running service to use it. See
[00-overview.md](00-overview.md) — "Major Backend Services."

**Confidence: High.**

## Required Frontends

None. See [04-ui-specification.md](04-ui-specification.md).

## Required Infrastructure

| Infrastructure | When required | Confidence |
|---|---|---|
| None, at run time | Not applicable | High |
| Source control host | To build and release a new library version | High |
| Continuous integration runner, Windows-based | To build and release a new library version — required because two of the nine build targets are older Windows-only platform versions | High |
| Package registry | To distribute a new library version | High |

**Confidence: High.**

## Configuration

The library has no external configuration file, environment variable, or
connection string. Every setting is a value the Integrating Developer sets
directly on the library's own objects in code:

| Setting group | Examples |
|---|---|
| Conversion settings | Relaxed type names, private-member access. |
| Known-type settings | Manually registered types, automatic array/list expansion, automatic discovery. |
| Rebuild context settings | Private-member access on rebuild, type restriction rule, custom assembly-loader. |

**Confidence: High.**

## Scheduling

No runtime scheduling exists. The release pipeline runs on a push event, not
on a timer. See [09-workflows.md](09-workflows.md) — "Publish a New Library
Version."

## Logging

The library performs no logging of its own. Every failure is raised as an
exception; the host application decides whether and how to log it.

**Confidence: High.**

## Monitoring

Not applicable at run time — there is no running instance to monitor. Build
and release health is visible through the continuous integration pipeline's
own run history and uploaded artifacts.

**Confidence: High.**

## Alerting

None built in. Any alerting on a failed release pipeline run is the source
control host's own notification behavior, not a Serialize.Linq feature.

## Security

- The library ships a specific security control (the type restriction rule)
  for the one identified runtime security risk (unrestricted type
  reconstruction during rebuild of untrusted text). See
  [08-business-rules.md](08-business-rules.md).
- The built package is signed with a private cryptographic key on the
  Windows build runner, and with a public-only (delay) signature on any
  non-Windows build host.
- The release pipeline authenticates to the package registry using a
  short-lived token obtained through the source control host's identity
  mechanism, instead of a stored long-lived credential.

**Confidence: High.**

## Backup

Not applicable to the library's own runtime, which holds no data. The
project's source history and release history are backed up by the source
control host and the package registry, outside the library's own control.

## Restore

The library's own build uses a locked, reproducible dependency list, and the
release pipeline fails outright if that list is missing or does not match
the resolved dependency graph. This is a reproducibility control for
building the library, not a data-restore control.

**Confidence: High.**

## Scaling

Not applicable. See [02-architecture.md](02-architecture.md) — "Scaling."

## Availability

Not applicable to the library's own runtime. See
[02-architecture.md](02-architecture.md) — "High Availability."

## Deployment

The library is "deployed" only in the sense of being published as a package
and pulled into a host application's own build. There is no separate
deployment step, environment, or runtime to stand up for the library itself.

**Confidence: High.**

## Runtime Requirements

The library needs a host process built on one of nine supported target
platform versions: two older Windows-only host-platform versions, five
recent cross-platform host-platform versions, and two portable baseline
profiles. No other runtime prerequisite exists.

**Confidence: High.**

## Cross-References

- Release workflow: [09-workflows.md](09-workflows.md)
- The pipeline component: [03-services-and-frontends.md](03-services-and-frontends.md)
- Security control detail: [08-business-rules.md](08-business-rules.md)
- Rebuild plan for this operational model: [12-reproduction-plan.md](12-reproduction-plan.md)
