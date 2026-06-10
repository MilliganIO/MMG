# Skill: Update Documentation
> Used by the Developer agent after every feature implementation.
> Can also be triggered directly: "Update docs for [feature]."

---

## When This Runs
- Automatically after every feature implementation
- When a human explicitly asks to update or sync docs
- When an architectural decision changes outside of a feature (e.g., swapping databases, adding a package)

---

## Files to Update — Decision Tree

Work through each file below. Update it if the condition is true. Skip it if not. Never leave docs stale.

---

### 1. `docs/changelog.md`
**Always update.** Every completed feature gets an entry, no exceptions.

Prepend a new entry at the top of the file:

```markdown
## [Feature Name] — [Date]
**Spec:** `specs/features/[feature-name].md`
**Feature ID:** `FEAT-###`

### Added
- [What was added — user-facing language]

### Changed
- [What existing behavior changed, if any]

### Files
- Created: `[list files]`
- Modified: `[list files]`
- Migration: `[migration name]` or None
```

---

### 2. `docs/architecture.md`
**Update if any of the following changed:**
- A new EF Core entity was added → update the **Domain Entities** table
- A new service interface/implementation was added → update the **Services** table
- A new endpoint group was added → update the **API Routes** table
- A new project or folder was introduced → update the **Projects** table or folder structure
- A new NuGet package was added → update the **Key Packages** table

Do NOT update architecture.md for purely UI or styling changes.

---

### 3. `docs/api.md`
**Update if any endpoints were added or changed.**

Format for each endpoint:

```markdown
## [Feature Group]

### `POST /api/[resource]`
**Auth:** Required (Bearer) | None
**Role:** Admin | User | Any
**Summary:** [One line description]

**Request**
\`\`\`json
{
  "field": "type"
}
\`\`\`

**Responses**
| Status | Description | Body |
|---|---|---|
| 201 | Created successfully | `{ "id": 1, "field": "value" }` |
| 400 | Validation failure | `{ "errors": { "field": ["msg"] } }` |
| 401 | Unauthenticated | — |
| 404 | Resource not found | — |
| 500 | Server error | `{ "error": "..." }` |

**Added in:** `FEAT-###`
```

---

### 4. `specs/_index.md`
**Always update.** Move the feature row from **Active** to **Completed** and set status to ✅ Done.

```markdown
| FEAT-### | [Feature Name] | `specs/features/[name].md` | [Date] |
```

---

### 5. `CLAUDE.md` — Current Sprint Table
**Always update.** Mark the feature row as ✅ Complete.

---

## Quality Check Before Finishing
- [ ] `changelog.md` has a new entry at the top
- [ ] `architecture.md` reflects any structural changes
- [ ] `api.md` reflects any endpoint additions or changes
- [ ] `specs/_index.md` row is ✅ Done
- [ ] `CLAUDE.md` sprint table is updated
- [ ] No doc file references a feature as "in progress" that is now complete
- [ ] All file paths in docs are accurate (no stale references)