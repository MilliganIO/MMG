# Skill: Write a Spec
> Used by the Business Analyst agent after completing discovery.
> This skill defines exactly how to turn gathered requirements into a finished spec file.

---

## When This Runs
- After the BA agent completes Phase 1 (Discovery) and Phase 2 (Confirm Understanding)
- When a human asks to "write up" or "document" a feature that has already been discussed
- When creating a v2 spec for a change to an existing approved feature

---

## Step-by-Step

### Step 1 — Assign a Feature ID
Check `specs/_index.md` for the highest existing `FEAT-###` number and increment by 1.
If no features exist yet, start at `FEAT-001`.

### Step 2 — Determine the File Name
Convert the feature name to `kebab-case`:
- "User Login" → `user-login.md`
- "Shopping Cart Checkout" → `shopping-cart-checkout.md`
- v2 of an existing spec → `user-login-v2.md`

### Step 3 — Copy the Template
Copy `specs/_template.md` to `specs/features/[kebab-case-name].md`.
Never write a spec from scratch — always start from the template.

### Step 4 — Fill in Every Section
Go section by section. Do not leave any section blank. If a section genuinely doesn't apply, write "N/A — [reason]" rather than deleting it.

| Section | Source |
|---|---|
| Metadata | Assign FEAT ID, today's date, priority from human |
| Overview | Summarize from discovery Phase 2 confirmation |
| Actors | From discovery Q2 |
| User Stories | One per distinct use case identified |
| Happy Path | From discovery Q3, written step by step |
| Alternative Flows | From discovery Q3/Q4 |
| Error States | From discovery Q4 — every error must have an HTTP status + user message |
| Schema Changes | From discovery Q7 — use C# class/record syntax |
| API Contract | From discovery Q8 — include method, route, request, all response shapes |
| Validation Rules | Derived from business rules and edge cases |
| Auth & Security | From discovery Q9 |
| Non-Functional | From discovery or project defaults in CLAUDE.md |
| Out of Scope | From discovery Q5 — must be non-empty |
| Acceptance Criteria | Given/When/Then — one per user story + one per error state |
| Dependencies & Risks | From discovery Q10 |
| Open Questions | Any unresolved items — must be empty before spec can be APPROVED |

### Step 5 — Self-Review
Before presenting the spec to the human, verify:
- [ ] Every user action has a system response
- [ ] Every error state has an HTTP status code and a user-facing message
- [ ] All acceptance criteria are objectively pass/fail (no subjective language like "works well" or "looks good")
- [ ] API contract matches acceptance criteria — no gaps
- [ ] "Out of Scope" section is non-empty
- [ ] Open questions section is empty OR contains clearly listed unresolved items
- [ ] No implementation decisions are prescribed (architecture is the developer's call)
- [ ] Playwright E2E testability: each UI acceptance criterion can be automated

### Step 6 — Add to Index
Add a row to `specs/_index.md` under **Active**:

```markdown
| FEAT-### | [Feature Name] | `specs/features/[name].md` | 🟡 Pending Review | — | — |
```

### Step 7 — Notify Human
Tell the human:
1. The spec has been written at `specs/features/[name].md`
2. What to review (call out anything you're uncertain about)
3. How to approve it:
   > "When you're ready, change the STATUS line in the spec to:
   > `**STATUS: APPROVED** — [your name], [date]`
   > Then tell me `@dev Implement specs/features/[name].md`"

---

## V2 Specs (Changes to Approved Features)
If a human wants to change an already-approved and implemented feature:
- Never edit the original spec
- Create `specs/features/[name]-v2.md` with a new FEAT ID
- Add a header note: `> Supersedes: specs/features/[name].md (FEAT-###)`
- Add the v2 row to `specs/_index.md`
- The original spec remains unchanged as a historical record