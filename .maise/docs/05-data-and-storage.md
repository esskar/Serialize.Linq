# 05 — Data and Storage

## Summary

Serialize.Linq holds **no persistent data**. Its entire data model is a set
of transient business entities that exist only for the duration of one
conversion call, entirely in the calling application's own memory. This
document describes that transient entity model, because it is the core
"business data" of the product, even though none of it is stored.

**Confidence: High.**

## Business Entities

The product's business entity is the **Expression Entity** family: a set of
entity types that together mirror every kind of node a query or filter
expression tree can contain. Each entity type converts one way from a real
expression node, and converts back the other way into a real expression
node.

### Expression Entities (mirror one kind of expression node each)

| Entity | Represents | Confidence |
|---|---|---|
| Binary Operation Entity | A two-sided operation, such as a comparison or an arithmetic operation. | High |
| Conditional Entity | An if/then/else choice between two values. | High |
| Constant Value Entity | A literal or captured fixed value. | High |
| Default Value Entity | "The default value of this type." | High |
| Indexer Access Entity | Access through an index, such as an item lookup by position or key. | High |
| Invocation Entity | Calling a stored function value with arguments. | High |
| Lambda (Function Definition) Entity | A parameterized function definition — the outermost shape of most filters. | High |
| List Initializer Entity | Building a collection and adding items to it in one step. | High |
| Member Access Entity | Reading a field or a property from a value. | High |
| Member Initializer Entity | Constructing an object and then setting some of its members. | High |
| Method Call Entity | Calling a named operation on a value or a type. | High |
| New Array Entity | Constructing an array, either with a fixed size or from listed values. | High |
| New Object Entity | Constructing a new instance of a type. | High |
| Parameter Reference Entity | A reference to one of the function definition's own input values. | High |
| Type Test Entity | Testing whether a value is, or is exactly, a given type. | High |
| Unary Operation Entity | A one-sided operation, such as negation or a type conversion. | High |

### Reference Entities (describe a reflection fact, not an operation)

| Entity | Represents | Confidence |
|---|---|---|
| Type Reference Entity | The identity of a type, by name, including its generic arguments. | High |
| Method Reference Entity | The identity of a method, resolved again later by matching its full signature text. | High |
| Constructor Reference Entity | The identity of a constructor, resolved the same way. | High |
| Field Reference Entity | The identity of a field, resolved the same way. | High |
| Property Reference Entity | The identity of a property, resolved the same way. | High |
| General Member Reference Entity | The identity of a member that does not fit the four categories above (used, for example, inside a member-initializer entity). | High |

### Supporting Entities

| Entity | Represents | Confidence |
|---|---|---|
| Element Initializer Entity | One "add this item" step inside a list-initializer entity. | High |
| Member Binding Entity (three kinds: direct assignment, list-add, nested) | One "set this member" step inside a member-initializer entity. | High |
| Entity List | An ordered group of expression entities, such as a call's argument list. | High |
| Reference Entity List | An ordered group of reference entities. | High |

**Confidence: High.** This inventory is a direct, complete listing of the
entity types found in the source tree's entity layer.

## Entity Relationships

Every expression entity may hold other expression entities as children (for
example, a binary-operation entity holds a left entity and a right entity).
This produces a tree shape that mirrors the original expression tree
exactly, one entity per node. A reference entity never holds an expression
entity as a child; it only carries identifying facts (a name, a declaring
type reference, and, for a method, its signature text and generic
arguments).

**Confidence: High.**

## Ownership

The calling host application owns every entity instance, for the lifetime of
one conversion call. Serialize.Linq creates entities on demand and discards
its own references to them once the call returns. No entity survives past
the call that created it, inside the library itself.

**Confidence: High.**

## Lifecycle

1. **Created** — during serialization, when the assembly layer walks a real
   expression tree.
2. **Converted to text** — by the chosen format layer.
3. **Discarded** — the library keeps nothing after returning the text.
4. **Re-created from text** — during deserialization, by the same format
   layer, in reverse.
5. **Converted back to a real expression** — by asking each entity to
   rebuild its matching expression node, using a shared rebuild context so
   that repeated references (for example, to the same function parameter)
   resolve to the exact same rebuilt object.
6. **Discarded** — again, the library keeps nothing after returning the
   rebuilt expression tree.

**Confidence: High.**

## Persistence

None. Serialize.Linq never writes an entity, or its text form, to disk, to a
database, or to any other durable store on its own. Any persistence is the
host application's own choice and own responsibility, performed entirely
outside the library.

**Confidence: High.**

## Retention

Not applicable — there is nothing to retain.

## Search

Not applicable — no search capability exists over expression entities.

## Cache

Two small, in-memory lookup caches exist, but only for the duration of one
rebuild context object (not across separate calls, and not shared across
threads doing unrelated work):

- A cache of resolved types, keyed by type name, so the same type name is not
  resolved twice in one rebuild.
- A cache of rebuilt function-parameter objects, keyed by parameter name and
  type, so every reference to "the same" parameter inside one rebuild
  resolves to the exact same object, which real expression trees require for
  correctness.

Both caches are created fresh for each rebuild context and discarded with it.

**Confidence: High.**

## Files

None. Serialize.Linq reads no file and writes no file.

## Objects (Blob/Object Storage)

None. See "Infrastructure" below.

## Data Synchronization

Not applicable — there is no second copy of any data to synchronize.

## Data Migration Assumptions

None apply to the library's runtime behavior. The one migration-shaped
concern in the product is **wire-format stability**: the entity types are
described in the source as "the serialization contract," meaning a change to
an entity's shape, or to the short internal names used in the size-optimized
build variant, is a breaking change for any text already produced by an
older version. See [08-business-rules.md](08-business-rules.md) and
[11-non-functional-requirements.md](11-non-functional-requirements.md).

**Confidence: High.**

## Infrastructure — Data Stores

None mandatory or optional at run time. Serialize.Linq needs no relational
database, no document database, no cache server, no message queue, no
search engine, and no object storage service to operate.

| Storage technology | Purpose | Owner | Consumers | Backup | Scaling | Availability | Deployment options |
|---|---|---|---|---|---|---|---|
| None | Not applicable | Not applicable | Not applicable | Not applicable | Not applicable | Not applicable | Not applicable |

**Confidence: High.**

## Cross-References

- Entities are produced and consumed by the Serialization and Deserialization
  workflows: [09-workflows.md](09-workflows.md).
- Entities are governed by the business rules in
  [08-business-rules.md](08-business-rules.md).
- The single component that owns entity handling is documented in
  [03-services-and-frontends.md](03-services-and-frontends.md).
