# 09 — Workflows

## Workflow: Serialize an Expression Tree

- **Goal**: Turn a query or filter, held as a live expression tree, into
  JSON text, XML text, or plain text.
- **Actors**: Integrating Developer (through the host application's own
  code).
- **Preconditions**: The host application already holds a valid expression
  tree in memory.
- **Trigger**: The host application calls a serialize operation.
- **Services involved**: Expression Serialization API.
- **Frontends involved**: None.
- **Step-by-step flow**:
  1. The host application passes the expression tree, and optional settings,
     to the library.
  2. The library checks for a very long chain of "and"/"or" conditions and
     reshapes it into a balanced shape if needed. (Rule: Deep Expression
     Reshaping.)
  3. The library walks the expression tree and builds a matching entity
     tree, simplifying any captured local value into a plain constant along
     the way. (Rule: Captured Value Simplification.)
  4. If automatic type discovery is on, the library inspects the entity
     tree's constant values and registers any extra types it finds. (Rule:
     Automatic Known Type Discovery.)
  5. The chosen format layer converts the entity tree into text.
  6. The library returns the text to the host application.
- **Data changes**: None persisted. The entity tree and the text both exist
  only for this call.
- **Success**: Text is returned, ready to store or transmit.
- **Failure**: An unrecognized expression kind, an unregistered custom type,
  or a stream failure raises an exception, and no text is returned. See
  [01-functional-specification.md](01-functional-specification.md) —
  "Error Handling."
- **Alternative paths**: The developer may call a single combined operation
  that performs steps 1 through 6 in one call, instead of converting to an
  entity tree first and formatting it second.
- **Recovery behavior**: None automatic. The host application's own calling
  code decides whether to retry, fall back, or surface the failure.

**Confidence: High.**

## Workflow: Deserialize Text Back to an Expression Tree

- **Goal**: Turn previously produced JSON, XML, or plain text back into a
  working expression tree, ready to compile and run.
- **Actors**: Integrating Developer.
- **Preconditions**: The host application holds text previously produced by
  this same product (or a compatible producer of the same wire format).
- **Trigger**: The host application calls a deserialize operation.
- **Services involved**: Expression Serialization API.
- **Frontends involved**: None.
- **Step-by-step flow**:
  1. The host application passes the text, and optionally a rebuild context,
     to the library.
  2. The chosen format layer parses the text back into an entity tree.
  3. The library walks the entity tree and asks each entity to rebuild its
     matching expression node.
  4. Every time a type must be resolved by name, the library resolves it
     against the types already loaded in the host process (or against a
     custom-supplied list), and — if a rebuild context carries a type
     restriction rule — checks the resolved type against that rule. (Rule:
     Type Access Control.)
  5. Every reference to the same function parameter resolves to the same
     rebuilt object. (Rule: Parameter Identity Preservation.)
  6. Every referenced method, field, property, or constructor is re-resolved
     by matching its signature text against the declaring type's members.
     (Rule: Member Resolution by Signature Text.)
  7. The library returns the rebuilt expression tree to the host
     application.
- **Data changes**: None persisted.
- **Success**: A working expression tree is returned. The host application
  may compile it into a runnable delegate and run it.
- **Failure**: A rejected type, a missing member, or a malformed constant
  value raises an exception, and no expression tree is returned.
- **Alternative paths**: None beyond choosing the input format (JSON, XML,
  or plain text) and whether to supply a rebuild context.
- **Recovery behavior**: None automatic.

**Confidence: High.**

## Workflow: Deserialize Untrusted Text Safely

This is the same workflow as above, with one additional precondition and one
additional actor responsibility, because it is the security-sensitive case
the project's own documentation calls out explicitly.

- **Goal**: Rebuild an expression tree from text the host application does
  not fully trust, without letting the rebuild construct an unexpected or
  dangerous type.
- **Actors**: Integrating Developer.
- **Preconditions**: The Integrating Developer has decided the input text
  is untrusted, and has built a type restriction rule listing exactly the
  types the expected expressions may legitimately use.
- **Trigger**: The host application calls the deserialize operation, passing
  a rebuild context that carries the type restriction rule.
- **Services involved**: Expression Serialization API.
- **Step-by-step flow**: Identical to "Deserialize Text Back to an
  Expression Tree," with the type-restriction check at step 4 now active for
  every resolved type, including every generic argument of a generic type,
  checked individually, and re-checked even for a type name already
  resolved once earlier in the same rebuild.
- **Data changes**: None persisted.
- **Success**: The expression tree is rebuilt, using only allowed types.
- **Failure**: The first disallowed type encountered stops the rebuild with
  an exception naming that type.
- **Alternative paths**: The Integrating Developer may instead supply a
  custom rule function rather than an explicit allow-list, for cases the
  allow-list shape cannot express cleanly.
- **Recovery behavior**: None automatic. The host application must decide
  how to respond to a rejected payload (for example, refuse the request and
  log the attempt, in the host application's own logging — Serialize.Linq
  itself logs nothing).

**Confidence: High.**

## Workflow: Publish a New Library Version

- **Goal**: Ship a new, tested version of the library to the package
  registry.
- **Actors**: Library Maintainer.
- **Preconditions**: A change is ready to merge to the main line of source
  control, and the version number has been increased.
- **Trigger**: The Library Maintainer merges (pushes) the change to the main
  line of source control.
- **Services involved**: Continuous Integration Pipeline, Package Registry,
  Source Control Host's Identity Mechanism.
- **Frontends involved**: None.
- **Step-by-step flow**:
  1. The pipeline restores dependencies using the locked, reproducible
     dependency list, failing outright if the lock file is missing or
     out of date.
  2. The pipeline builds the library for every supported target.
  3. The pipeline runs the full automated test suite.
  4. The pipeline packages the library and its symbols.
  5. The pipeline uploads the package as a build artifact.
  6. Only if the triggering event was a push to the main line: the pipeline
     exchanges a short-lived identity token for a short-lived package-
     registry credential, then publishes the package.
- **Data changes**: A new package version becomes available on the package
  registry.
- **Success**: The new version is visible on the package registry.
- **Failure**: A failed restore, build, or test step stops the pipeline
  before packaging. A publish attempt for an already-published version
  number is silently skipped, not treated as a failure.
- **Alternative paths**: A pull request against the main line runs steps 1
  through 5 only, as a check, without publishing.
- **Recovery behavior**: The Library Maintainer must notice a skipped
  publish (due to a forgotten version bump) manually; the pipeline gives no
  distinct signal for this case.

**Confidence: High.**

## Cross-References

- Rules exercised by these workflows: [08-business-rules.md](08-business-rules.md)
- Services and their responsibilities: [03-services-and-frontends.md](03-services-and-frontends.md)
- Operations called during these workflows: [06-apis-and-integrations.md](06-apis-and-integrations.md)
- Operational detail for the publish workflow: [10-operational-requirements.md](10-operational-requirements.md)
