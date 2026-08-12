# 01 — Functional Specification

## Product Scope

Serialize.Linq converts an expression tree to text, and text back to an
expression tree. It does one job, and it does that job in three text formats.
It has no other product scope.

**Confidence: High.**

## Functional Scope

| In scope | Confidence |
|---|---|
| Convert an expression tree to an intermediate entity tree, and back. | High |
| Convert the entity tree to JSON text, XML text, or plain text, and back. | High |
| Simplify captured local values into plain constant entities during conversion. | High |
| Reshape very long chained conditions to avoid a stack overflow during conversion. | High |
| Register extra types the format layer must know about ahead of time, manually or automatically. | High |
| Restrict which types may be rebuilt when the entity tree comes from an untrusted source. | High |
| Allow a caller-supplied plug-in for extra type conversion, custom serialization behavior, or a custom list of loaded assemblies to search when rebuilding a type. | High |

## Out-of-Scope Functionality

| Out of scope | Confidence |
|---|---|
| Converting an expression tree to a binary format. | High — removed in version 4.0 because the binary approach used a mechanism with known security weaknesses. |
| Running or evaluating a query against a real data source. | High — the library only converts and rebuilds the expression tree. Running the rebuilt tree against data is the host application's job. |
| Any user interface, report, dashboard, or visual output. | High — see [04-ui-specification.md](04-ui-specification.md). |
| Any network transport (sending the serialized text to another machine). | High — the host application must move the text itself, for example over its own network call or storage layer. |
| Any scheduled or background processing inside the library. | High — every operation runs synchronously, on the calling thread, when the host application calls it. |
| User authentication or user permission management. | High — the library has no concept of a signed-in user. |

## Major Features

### Feature: Serialize an expression tree to text

Convert a query or a filter, held in memory as an expression tree, into JSON
text, XML text, or plain text, so the host application can store it or send
it elsewhere.

See [09-workflows.md](09-workflows.md) — Serialization Workflow.

### Feature: Deserialize text back to an expression tree

Convert JSON text, XML text, or plain text back into a working expression
tree, and optionally compile it into a runnable delegate.

See [09-workflows.md](09-workflows.md) — Deserialization Workflow.

### Feature: Restrict rebuildable types

Let the host application supply an allow-list of types, or a custom rule, so
that rebuilding text from an untrusted source cannot construct an unexpected
or dangerous type.

See [08-business-rules.md](08-business-rules.md) — Rule: Type Access Control.

### Feature: Automatic and manual type registration

Let the host application either list every extra type the format layer needs
to know about ahead of time, or let the library discover those types on its
own by inspecting the constant values inside the expression tree.

See [08-business-rules.md](08-business-rules.md) — Rules: Known Type
Registration, Automatic Known Type Discovery.

### Feature: Safe handling of very large expressions

Automatically reshape a very long chain of "and"/"or" conditions into a
balanced shape before conversion, so that converting, rebuilding, or
compiling the expression never overflows the call stack.

See [08-business-rules.md](08-business-rules.md) — Rule: Deep Expression
Reshaping.

### Feature: Extension points for custom behavior

Let the host application plug in a custom rule for finding loaded assemblies,
a custom conversion rule for values that do not convert automatically, or a
custom serialization behavior for a type the format layer cannot handle on
its own.

See [06-apis-and-integrations.md](06-apis-and-integrations.md).

## User Capabilities

Serialize.Linq has one category of user capability, exercised entirely
through the calling code the developer writes:

- Convert an expression tree to text (any of the three formats).
- Convert text back to an expression tree.
- Configure conversion settings (which members to include, how relaxed type
  names should be, whether to restrict rebuildable types).
- Register or discover extra types needed by the format layer.

There is no other user-facing capability, because there is no other user
than the developer. See [07-user-roles-and-permissions.md](07-user-roles-and-permissions.md).

## Administrative Capabilities

None inside the library itself. The closest equivalent is the **library
maintainer's** release process: bump the version number, and the release
pipeline publishes a new package. See
[10-operational-requirements.md](10-operational-requirements.md).

**Confidence: High.**

## Business Capabilities

The core business capability is **moving filter and query logic across a
process boundary without losing its exact shape and meaning**. This supports
scenarios such as:

- A client application builds a filter and sends it to a server for
  execution.
- An application stores a filter as text and re-applies it later.
- An application passes a filter to a background job or a separate service.

**Confidence: Medium** — these usage scenarios are inferred from the
product's stated purpose and are not directly observable in the source code.

## Reporting

None. The library produces no report of any kind.

**Confidence: High.**

## Import/Export

The whole library is, in effect, an import/export mechanism for expression
trees:

- **Export**: convert an expression tree to JSON, XML, or plain text.
- **Import**: convert JSON, XML, or plain text back to an expression tree.

No other import or export format or capability exists.

**Confidence: High.**

## Notifications

None. The library raises no notification, sends no message, and calls no
notification service.

**Confidence: High.**

## Scheduled Behavior

None inside the library. The **release pipeline** runs on a push event to the
main line of source control, not on a schedule. See
[10-operational-requirements.md](10-operational-requirements.md).

**Confidence: High.**

## Error Handling

The library signals a failure by raising an exception. It defines three
specific exception types, each tied to one failure condition:

| Failure condition | Result | Confidence |
|---|---|---|
| A constant value does not match its declared type during rebuild. | The library raises an exception naming the mismatched type. | High |
| A referenced method, field, property, or constructor cannot be found again on the resolved type during rebuild. | The library raises an exception naming the declaring type and the missing member's signature text. | High |
| A resolved type is rejected by the host application's type restriction rule. | The library raises an exception naming the rejected type. | High |
| The host application asks for text output/input using a format that does not support text (for example, a binary-only format). | The library raises a general invalid-operation failure. | High |
| An unrecognized expression kind reaches the assembly layer. | The library raises a general argument failure naming the unrecognized kind. | High |
| Reading or writing the underlying text stream fails. | The library wraps the failure in a general serialization failure. | High |

See [08-business-rules.md](08-business-rules.md) for the full rule list.

## Validation Behavior

- A constant entity's value is checked against its declared type before it is
  accepted; a mismatch is rejected. (Rule: Constant Type Validation.)
- A resolved type is checked against the host application's type
  restriction rule, if one is set, every time the type is resolved — even if
  the type name was already resolved once before. (Rule: Type Access
  Control.)
- No other structural validation exists; the library assumes the entity tree
  it receives correctly mirrors a real expression tree.

**Confidence: High.**

## Data Ownership

Serialize.Linq owns no persistent data. Every entity tree, and every wire-format
text value, exists only for the duration of one conversion call, inside the
calling application's own memory. The host application owns all data before,
during, and after calling the library.

**Confidence: High.**

## Business Constraints

| Constraint | Description | Confidence |
|---|---|---|
| No binary serialization. | Removed in version 4.0 for security reasons; must not be reintroduced. | High |
| Restricted deserialization must be opt-in and explicit. | The host application must construct and pass a restriction rule; without one, all types resolve as before, to preserve compatibility with existing callers. | High |
| Supported platform-version range must stay wide. | The library targets nine different platform versions, including two old Windows-only host-platform versions and two portable baseline profiles, to stay usable in old and new host applications alike. | High |
| New bug fixes need a reproducing test. | The project's own contribution guidance requires this. | High |
