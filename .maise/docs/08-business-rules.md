# 08 — Business Rules

## About This Document

Each rule below was extracted either from the library's core conversion
logic, or from a regression test that guards a specific, previously reported
defect. A rule tied to a regression test carries **High confidence**,
because the test proves the exact triggering scenario and the exact expected
outcome.

## Rule: Constant Type Validation

- **Description**: A constant-value entity's stored value must match its
  declared type.
- **Trigger**: Setting the declared type on a constant-value entity.
- **Preconditions**: A value is already attached to the entity.
- **Decision logic**: If the value is not an instance of the newly assigned
  type (or a compatible type), reject the assignment.
- **Outcome**: The library raises an exception naming the mismatched type.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Prevents a corrupted or tampered entity tree from
  silently producing a wrong value on rebuild.
- **Confidence**: High.

## Rule: Type Access Control

- **Description**: When rebuilding an expression tree from text, every type
  resolved by name is checked against an optional, host-supplied allow-list
  or custom rule.
- **Trigger**: Resolving a type name to a real type, at any point during
  rebuild — including each generic argument of a generic type, checked
  individually.
- **Preconditions**: The host application supplied a type restriction rule
  on the rebuild context. Without one, this rule does not apply, and all
  types resolve as before.
- **Decision logic**: If a rule is present and it rejects the resolved type,
  stop the rebuild immediately, even if that exact type name was already
  resolved once earlier in the same rebuild (the check is not skipped on a
  cache hit).
- **Outcome**: The library raises an exception naming the rejected type, and
  the rebuild does not complete.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Rebuilding an expression tree from text
  reconstructs types by name. Without a restriction, text from an untrusted
  source could name any type already loaded in the host process. This rule
  closes that gap, mirroring a well-known security lesson from an older,
  now-removed serialization mechanism in the platform.
- **Confidence**: High.

## Rule: Known Type Registration

- **Description**: The text-format layer must be told, ahead of time, about
  any non-obvious concrete type (for example, a custom enumeration) that may
  appear as a constant value inside an expression tree, or it cannot
  reconstruct that value correctly.
- **Trigger**: Serializing or deserializing an expression tree containing a
  constant of such a type.
- **Preconditions**: The type is not already one of the format layer's
  built-in known types (basic numbers, text, date/time, and similar simple
  types).
- **Decision logic**: If the type was registered (directly, or as an
  array/list variant of a registered type), the value serializes and
  deserializes correctly. If not, and automatic discovery (see next rule) is
  off, the operation fails.
- **Outcome**: A registered type round-trips correctly; an unregistered type
  fails serialization.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: The underlying text-format engines require every
  possible concrete type in a polymorphic value slot to be known in advance;
  this rule documents that constraint and the developer's manual escape
  hatch for it.
- **Confidence**: High.

## Rule: Automatic Known Type Discovery

- **Description**: Instead of manually registering every custom type used as
  a constant, the developer can turn on automatic discovery, and the library
  finds those types itself by inspecting the constant values already present
  in the entity tree.
- **Trigger**: Serializing an entity tree, with automatic discovery turned
  on.
- **Preconditions**: Automatic discovery is enabled on the serializer.
- **Decision logic**: Walk the entity tree, record the runtime type of every
  constant value found, decompose array element types and generic type
  arguments, skip anything already known, and register the remainder — all
  without evaluating or enumerating the value itself, to avoid an unintended
  side effect (for example, running a lazily-evaluated query).
- **Outcome**: Custom types used as constants serialize correctly without a
  manual registration step.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Reduces a historically manual, error-prone,
  easy-to-forget setup step.
- **Confidence**: High.

## Rule: Automatic Array/List Type Registration

- **Description**: When a type is registered as known, the developer can
  also ask the library to automatically register the array form, or the list
  form, of that type at the same time.
- **Trigger**: Registering a known type, with the array or list auto-register
  toggle turned on (the two toggles are mutually exclusive).
- **Preconditions**: The expression contains a containment check (for
  example, "is this value in this list") against a collection of the
  registered type.
- **Decision logic**: Expand the registered type into its array or list form
  before adding it to the known-type set.
- **Outcome**: A containment check against an array or a list of a custom or
  simple type serializes correctly, including when the list holds a
  nullable element type.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Containment checks against a collection are common
  in real filters; this rule removes a repetitive manual registration step
  for that common case.
- **Confidence**: High.

## Rule: Deep Expression Reshaping

- **Description**: A very long chain of "and"/"or" conditions is
  automatically reshaped into a balanced shape before any conversion begins.
- **Trigger**: Converting an expression tree that contains more than a small
  number of chained "and"/"or" terms (observed regression case: fifteen
  thousand terms).
- **Preconditions**: None — this check runs on every serialization.
- **Decision logic**: If the chain is long enough, flatten it into a plain
  list of terms, then rebuild it as a balanced tree, preserving the original
  evaluation order and meaning.
- **Outcome**: Converting, rebuilding, or later compiling the expression
  never overflows the call stack, regardless of how many chained terms the
  original filter had.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: A naturally built, deeply chained condition (for
  example, a large "value is one of these one thousand IDs" filter expressed
  as repeated "or" checks) can be as deep as it has terms; walking such a
  tree without reshaping it can crash the process.
- **Confidence**: High.

## Rule: Captured Value Simplification

- **Description**: A reference to a value captured from the surrounding code
  (a local variable, or a property of a captured object) is simplified into
  a plain constant-value entity, rather than serialized as a reference into
  the surrounding code's own, non-serializable state.
- **Trigger**: Converting an expression whose body refers to a value from
  outside its own declared parameters — including a value captured inside a
  guarded block, or inside a paused asynchronous method.
- **Preconditions**: The referenced value is not itself one of the
  expression's own declared input types.
- **Decision logic**: If the reference can be evaluated to a concrete value
  without side effects, replace it with a constant-value entity holding that
  value.
- **Outcome**: The filter serializes correctly and its meaning is preserved,
  even though the surrounding code's own internal state is never itself
  serialized.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: This is what lets a developer write an ordinary,
  natural filter that references a local variable, without needing to
  design that filter around the library's own limitations.
- **Confidence**: High.

## Rule: Private-Member Read Timing

- **Description**: When private-member access is turned on, a captured
  object's own (instance) field value is captured at serialization time, but
  a type's shared (static) field value is re-read at rebuild-and-run time,
  not at serialization time.
- **Trigger**: Serializing an expression that reads a non-public field or
  property, with private-member access turned on.
- **Preconditions**: Private-member access is enabled; the referenced member
  is not publicly accessible.
- **Decision logic**: An instance member's current value is fixed into the
  entity tree as a constant at the moment of serialization. A static
  member's value is looked up again, live, when the rebuilt expression is
  later compiled and run.
- **Outcome**: Two different, deliberate points in time at which a private
  value is "read," depending on whether it belongs to an instance or to the
  type itself.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: An instance the developer is filtering on may not
  exist anymore by the time the filter is rebuilt and run elsewhere, so its
  values must be captured now; a type's shared value is always available
  again later, so re-reading it live keeps the filter current.
- **Confidence**: High.

## Rule: Compiler-Generated Type Resolution

- **Description**: A type the host compiler generates automatically (for
  example, to carry the extra named values introduced by a query's "let"
  step) must resolve correctly on rebuild, whether it is declared in the same
  compiled unit as the caller, or in an entirely separate one.
- **Trigger**: Converting or rebuilding an expression that contains such a
  compiler-generated type.
- **Preconditions**: The expression was built using a language feature that
  causes the compiler to generate a supporting type behind the scenes.
- **Decision logic**: Always record such a type using its fully qualified
  identity, never the shorter, relaxed form, regardless of the developer's
  relaxed-type-name setting.
- **Outcome**: The expression round-trips correctly, even across a compiled-
  unit boundary.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Compiler-generated types are easy to miss when
  designing type-resolution logic, because a developer never names them
  directly; this rule guards a defect class specific to that blind spot.
- **Confidence**: High.

## Rule: Anonymous and Dynamically-Typed Result Support

- **Description**: An expression that constructs an anonymous, on-the-fly
  object shape, or that is declared to return a dynamically-typed result,
  must still serialize and deserialize correctly.
- **Trigger**: Converting such an expression.
- **Preconditions**: None beyond the expression shape itself.
- **Decision logic**: Treat the anonymous shape like any other constructed
  object shape; resolve the dynamic return type using the same
  fully-qualified-name handling as other compiler-generated types.
- **Outcome**: The expression round-trips correctly.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Projecting query results into a lightweight,
  purpose-built shape is a very common query pattern.
- **Confidence**: High.

## Rule: Indexer Access Support

- **Description**: Access through an index (for example, a dictionary lookup
  by key) must be representable as its own kind of entity, not silently
  dropped or misrepresented.
- **Trigger**: Converting an expression containing an indexer access,
  including one introduced by rewriting a strongly-typed member access into
  a dictionary-style lookup.
- **Outcome**: The expression serializes, deserializes, and — once rebuilt
  and compiled — behaves the same as the original.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Supports flexible, dictionary-backed filter models
  built by rewriting a typed filter into a loosely-typed one.
- **Confidence**: High.

## Rule: Default-Value Expression Support

- **Description**: "The default value of this type" must be representable as
  its own kind of entity.
- **Trigger**: Converting an expression containing such a value.
- **Outcome**: The expression round-trips to an equal expression.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: A previously unsupported expression shape that
  real filters can produce.
- **Confidence**: High.

## Rule: Self-Referential Type Safety

- **Description**: Discovering the members and known types reachable from a
  type must not loop forever, even when that type refers back to itself
  (directly, through a shared field of its own type, or through an ordinary
  instance field).
- **Trigger**: Serializing a call to a method on an object whose type
  contains such a self-reference, with the type registered as known.
- **Decision logic**: Track types already visited while walking the type
  graph, and stop revisiting them.
- **Outcome**: Serialization completes without an unbounded loop or a
  related failure.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Real object models frequently contain
  self-referential or mutually-referential shapes; the library must not
  assume a strictly tree-shaped type graph.
- **Confidence**: High.

## Rule: Custom Serialization Behavior (XML Only)

- **Description**: A developer may plug in a custom serialization behavior
  for a type the format layer cannot handle on its own, but only for the XML
  format.
- **Trigger**: Serializing or deserializing through the XML format layer,
  with a custom serialization behavior configured.
- **Decision logic**: The XML format layer passes the custom behavior
  through to the underlying XML engine. The JSON format layer does not,
  because of a limitation in its underlying engine.
- **Outcome**: A type otherwise unsupported by the format layer can still
  participate, through XML only.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Gives the developer an escape hatch for edge-case
  types, while being explicit about the one-format limitation so the
  developer does not rely on it accidentally through JSON.
- **Confidence**: High.

## Rule: Parameter Identity Preservation

- **Description**: Every reference, inside one rebuilt function definition,
  to "the same" input parameter must resolve to the exact same rebuilt
  object, not to separate, look-alike objects.
- **Trigger**: Rebuilding a function-definition entity with more than one
  reference to the same parameter.
- **Decision logic**: Cache the rebuilt parameter object by its name and
  type, for the lifetime of one rebuild context, and reuse it for every
  matching reference.
- **Outcome**: The rebuilt function definition is structurally valid and can
  be compiled and run.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: The host platform's own expression-tree rules
  require this exact-object identity; without it, the rebuilt expression is
  invalid.
- **Confidence**: High.

## Rule: Member Resolution by Signature Text

- **Description**: A referenced method, field, property, or constructor is
  never serialized directly. It is serialized as its declaring type plus its
  full signature text, and re-resolved by matching that text again on
  rebuild.
- **Trigger**: Every serialization and rebuild involving a member reference.
- **Decision logic**: On rebuild, resolve the declaring type, then scan its
  members (respecting the private-access setting) for one whose signature
  text matches exactly.
- **Outcome**: If no member matches, the rebuild fails with an exception
  naming the declaring type and the expected signature text.
- **Affected users**: Integrating Developer.
- **Affected services**: Expression Serialization API.
- **Business rationale**: Reflection facts cannot be serialized directly
  across process or version boundaries; matching by signature text is the
  library's chosen, portable substitute.
- **Confidence**: High.

## Cross-References

- Rules are exercised by the workflows in [09-workflows.md](09-workflows.md).
- Rules are enforced inside the component documented in
  [03-services-and-frontends.md](03-services-and-frontends.md).
- The entities these rules govern are documented in
  [05-data-and-storage.md](05-data-and-storage.md).
- The security-relevant rule (Type Access Control) is also discussed in
  [02-architecture.md](02-architecture.md) and
  [07-user-roles-and-permissions.md](07-user-roles-and-permissions.md).
