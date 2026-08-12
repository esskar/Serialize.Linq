# 04 — UI Specification

## Finding

Serialize.Linq has **no user interface**. It has no screen, no page, no
dialog, no wizard, no popup, no drawer, and no view of any kind. It is a
code library with no visual output.

**Confidence: High.** No markup, styling, layout, or presentation code exists
anywhere in the source tree. The library's only outputs are JSON text, XML
text, or plain text, produced for the calling program to consume, store, or
transmit — never for a person to read on a screen.

This document records that finding against every subsection the
documentation template requires, so the absence of a user interface is
explicit rather than silent.

## General

| Field | Value |
|---|---|
| Purpose | Not applicable — no screen exists. |
| Intended users | Not applicable. |
| Entry points | Not applicable. |
| Exit points | Not applicable. |
| Navigation | Not applicable. |
| Permissions | Not applicable. |
| Related workflows | The library's own workflows have no visual step. See [09-workflows.md](09-workflows.md). |

## Layout

Not applicable. No header, footer, sidebar, toolbar, main content area,
status bar, panel, tab, or section exists.

## Components

Not applicable. No table, list, form, card, chart, tree, calendar, map,
dashboard, editor, file upload, notification, search box, filter, pagination
control, dialog, or wizard exists.

## Forms

Not applicable. No field, validation rule, default value, save action, or
cancel action exists in any visual sense. The closest equivalent — the
settings a developer sets in code before calling the library — is documented
as configuration, not as a form, in
[06-apis-and-integrations.md](06-apis-and-integrations.md).

## Tables

Not applicable.

## Screen States

Not applicable. No initial, loading, empty, success, error, permission
denied, or offline screen state exists. The library's only "state" is
whether a call succeeded or raised an exception, documented in
[01-functional-specification.md](01-functional-specification.md) —
"Error Handling."

## Responsive Behavior

Not applicable. No desktop, tablet, or mobile presentation exists.

## Accessibility

Not applicable. No keyboard navigation, focus order, label, screen reader
behavior, contrast requirement, or accessible error presentation exists,
because no visual surface exists to make accessible.

## User Journey

Not applicable in the visual sense. The closest equivalent is the
**developer's integration journey**:

1. **Primary flow**: the developer adds the library, writes code to convert
   an expression tree to text, and writes code to convert text back.
2. **Alternative flow**: the developer additionally configures a type
   restriction rule, custom type conversion, or a custom serialization
   behavior.
3. **Error flow**: a call raises one of the exceptions listed in
   [01-functional-specification.md](01-functional-specification.md), and the
   developer's own calling code decides how to handle it.
4. **Cancellation flow**: not applicable — every operation is a single,
   synchronous call with no cancellable, long-running step.

**Confidence: High.**

## Recommendation

Any future team rebuilding Serialize.Linq should not budget time for a user
interface. If a future product wraps Serialize.Linq in a visual tool (for
example, a filter-builder screen for end users), that tool is a separate
product and needs its own, separate specification.
