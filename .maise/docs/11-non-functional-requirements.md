# 11 — Non-Functional Requirements

| Quality attribute | Requirement | Status | Confidence |
|---|---|---|---|
| Performance | Converting and rebuilding an expression tree must not overflow the call stack, even for a very long chained condition (observed regression case: fifteen thousand terms), handled by reshaping the chain into a balanced shape before conversion. | Explicit | High |
| Scalability | Not meaningfully applicable — the library holds no shared state and each call is independent; scaling is entirely the host application's concern. | Inferred | High |
| Availability | Not applicable — no running instance exists. | Inferred | High |
| Reliability | Every documented failure condition raises one specific, typed exception rather than failing silently or returning a wrong result; a mismatched constant type and a missing member are explicitly rejected rather than tolerated. | Explicit | High |
| Security | Rebuilding an expression tree from untrusted text can be restricted to an explicit allow-list of types, closing a known class of type-confusion / untrusted-deserialization risk that a previous, now-removed binary format in this same product line was vulnerable to. | Explicit | High |
| Privacy | Not directly addressed. The library serializes whatever constant values the expression tree contains, including any sensitive data the host application chose to capture as a constant; the library provides no data classification or redaction feature. | Inferred | Medium |
| Compliance | No compliance framework, standard, or certification is referenced anywhere in the project. | Unknown | Unknown |
| Auditability | The library performs no logging and keeps no audit trail. Any audit trail is the host application's own responsibility. | Explicit | High |
| Maintainability | New defect fixes are required by project policy to ship with a reproducing automated test; the test suite already covers roughly two dozen previously reported defects, one file per defect, each testing one specific scenario. | Explicit | High |
| Extensibility | Five distinct extension points let a host application customize type discovery, type restriction, value conversion, entity-tree construction, and (for XML only) custom serialization behavior, without modifying the library. | Explicit | High |
| Testability | The project maintains an automated test suite, including a second, separate compiled unit used specifically to test type resolution across a compiled-unit boundary. No numeric test-coverage target is stated in the project's own documentation. | Explicit (suite exists); Unknown (coverage target) | High / Unknown |
| Localization | Not applicable — the library produces no human-facing text, only machine-readable JSON, XML, or plain text. | Inferred | High |
| Internationalization | Not applicable, for the same reason. | Inferred | High |
| Accessibility | Not applicable — no user interface exists. | Inferred | High |
| Disaster recovery | Not applicable to the library's own runtime, which holds no data. The project's source and release history depend on the source control host's and package registry's own disaster-recovery posture, which is outside this specification's visibility. | Unknown | Unknown |
| Business continuity | The release process depends on one named individual's ownership of the package registry publishing identity, as recorded in the release pipeline configuration; no documented succession or backup-maintainer process was found. | Inferred | Medium |
| Operational support | No support channel commitment (response time, coverage hours) is documented; the project accepts community contributions and issue reports on its source-control host. | Inferred | Medium |
| Portability | The library targets nine distinct platform versions, spanning two decades of the host platform's evolution, specifically to remain usable across very old and very new host applications alike. | Explicit | High |
| Backward compatibility | The default behavior for type resolution during rebuild remains fully unrestricted unless a host application explicitly opts into the newer type-restriction control, preserving behavior for callers written before that control existed. | Explicit | High |

## Cross-References

- Security requirement detail: [08-business-rules.md](08-business-rules.md), [02-architecture.md](02-architecture.md)
- Extensibility points: [06-apis-and-integrations.md](06-apis-and-integrations.md)
- Test-driven defect history: [08-business-rules.md](08-business-rules.md)
- Open items needing validation: [13-open-questions.md](13-open-questions.md)
