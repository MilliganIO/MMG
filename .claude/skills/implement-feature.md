# Skill: Implement Feature from Spec

> This skill is used by the Developer agent. It defines the exact steps to go from an approved spec to merged code.

---

## Trigger
Human says:
```
@dev Implement specs/features/[feature-name].md
```

## Checklist

### Pre-Implementation
- [ ] Spec file exists and is readable
- [ ] Spec has `STATUS: APPROVED` line
- [ ] `CLAUDE.md` has been read (tech stack, coding standards)
- [ ] `docs/architecture.md` has been reviewed
- [ ] No open questions in spec Section 10

### Implementation
- [ ] Spec reference comment added to new files
- [ ] All acceptance criteria (Section 9) addressed
- [ ] Error states (Section 4.3) handled in code
- [ ] Unit tests written for business logic
- [ ] No unrelated refactoring done
- [ ] Code follows standards in CLAUDE.md

### Post-Implementation
- [ ] `docs/changelog.md` updated
- [ ] `docs/architecture.md` updated (if needed)
- [ ] `docs/api.md` updated (if needed)
- [ ] `specs/_index.md` row updated to ✅ Done
- [ ] `CLAUDE.md` sprint table updated
- [ ] Commit message drafted and provided to human

## Output Format
At the end of implementation, provide:
1. Summary of what was built
2. List of files created/modified
3. Any TODOs noted (out of scope items)
4. Suggested commit message
5. Confirmation that all docs were updated