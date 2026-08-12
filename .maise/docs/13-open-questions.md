# 13 — Open Questions

## Missing Information

| Question | Why it matters | Suggested validation step |
|---|---|---|
| Which kinds of host applications actually use Serialize.Linq, and for what business purpose? | Shapes priority for future feature work and for this specification's assumed usage scenarios. | Ask the Library Maintainer, and review public issue history on the source control host for real-world usage descriptions. |
| Is there a numeric test-coverage target? | Affects confidence in "Maintainability" and "Testability" in [11-non-functional-requirements.md](11-non-functional-requirements.md). | Run the test suite with a coverage tool and compare against the requesting organization's own standard. |
| Are the legacy package-restore tool and configuration file (found alongside the modern lock-file-based restore setup) still needed by anything? | Left unresolved, they are project clutter; if load-bearing for some overlooked scenario, removing them would break something. | Ask the Library Maintainer; attempt a clean build after removing them, on a branch. |
| Is there a defined succession plan if the current Library Maintainer becomes unavailable? | The release pipeline's publishing identity is tied to one named individual. | Ask the Library Maintainer; check the source control host's listed collaborators/owners. |
| What is the actual adopted meaning of "restricted deserialization" in existing host applications — do any currently rely on the unrestricted default against untrusted input? | The unrestricted default is a real security exposure for any caller who has not explicitly opted into the restriction. | Survey known adopters, or add prominent guidance to the product's own README (a Medium-confidence recommendation, not a finding from the code itself). |

## Ambiguous Behavior

| Question | Why it matters | Suggested validation step |
|---|---|---|
| One historical test case (nullable local variable comparison) contains a commented-out assertion describing a related scenario that appears never to have been fully resolved. | May indicate an unresolved edge case in nullable-value handling that was deliberately left disabled rather than fixed. | Ask the Library Maintainer; re-enable the assertion on a branch and observe whether it still fails. |

## Unknown Workflows

No workflow beyond serialize, deserialize (trusted and untrusted), and
publish-a-release was found. If additional host-application-side workflows
exist (for example, a specific pattern for passing a filter from a web tier
to a data tier), they live entirely in host application code outside this
repository and are not visible here.

## Unknown Business Rules

No business rule beyond those listed in
[08-business-rules.md](08-business-rules.md) was found. Any additional rule
would need to come from a defect report or a feature request not yet
reflected in the current source code or test suite.

## Unknown Infrastructure

Whether any adopting organization runs its own private package feed (rather
than the public package registry) for internal distribution is unknown and
not visible from this repository.

## Unknown Deployment

Not applicable in the traditional sense — see
[02-architecture.md](02-architecture.md) — "Deployment Topology." Any
"deployment" of Serialize.Linq is really the host application's own
deployment, which this repository does not describe.

## Unknown Permissions

Not applicable beyond what is documented in
[07-user-roles-and-permissions.md](07-user-roles-and-permissions.md). No
additional permission concept was found.

## Unknown Integrations

Whether any host application pairs Serialize.Linq with a specific transport
(a specific message queue product, a specific web framework, a specific
database) is unknown; the library itself is transport-agnostic and makes no
assumption about this.

## Unknown Operational Assumptions

| Question | Why it matters |
|---|---|
| Is the Windows-based build runner a hosted, provider-managed runner, or a self-managed one? | Affects the operational risk and maintenance burden of the release pipeline, and was not fully determinable from the workflow file alone. |
| Is there a defined process for responding to a security report against this library specifically (versus the general code-of-conduct contact address found in the repository)? | A dedicated library, especially one with a documented untrusted-deserialization concern, benefits from a clear, separate security-contact process. |

## Recommended Next Steps

1. **Product owner / maintainer review** — confirm the assumed usage
   scenarios in [00-overview.md](00-overview.md) and
   [01-functional-specification.md](01-functional-specification.md) against
   real adopter feedback.
2. **Security review** — specifically validate the default-unrestricted
   deserialization behavior against the requesting organization's own risk
   tolerance, and confirm whether existing callers already apply a type
   restriction rule in practice.
3. **Architecture review** — confirm that no additional runtime component
   exists outside the reviewed source tree (for example, a companion tool or
   service published from a different repository) before treating this
   specification as complete.
4. **Independent redevelopment planning** — use
   [12-reproduction-plan.md](12-reproduction-plan.md) as the starting
   blueprint, and validate each milestone's acceptance criteria against the
   business rules in [08-business-rules.md](08-business-rules.md) before
   committing to a rebuild timeline.
