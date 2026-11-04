---
name: feature-to-prd
description: Turn user or business requirements into a grounded Product Requirements Document (PRD), canvas summary, and reviewable PR. Use when the user asks to create a PRD, write product requirements, convert a feature brief/spec into a PRD, or says feature-to-PRD / feature-to-prd.
disable-model-invocation: true
---

# Feature to PRD

Convert a user/business requirement into a durable PRD grounded in the current codebase—not a generic wishlist.

## When to use

- User provides a feature brief, problem/goal/requirements, or acceptance criteria and asks for a **PRD**
- User invokes **feature-to-PRD** / **feature-to-prd**
- User pivots from a prior PRD track (“stop / revoke / replace with …”)

## Workflow

Copy and track:

```
Feature → PRD progress:
- [ ] 0. Handle revoke / pivot (if asked)
- [ ] 1. Orient (rename chat, branch plan)
- [ ] 2. Mine the requirement
- [ ] 3. Ground in the codebase (exists vs gap)
- [ ] 4. Draft PRD markdown
- [ ] 5. Draft canvas summary
- [ ] 6. Ship (commit, push, PR)
- [ ] 7. Summarize for the user
```

### 0. Handle revoke / pivot

If the user says stop, revoke, abandon, or replace the previous PRD/feature track:

1. Close related open PRs for that track (`ManagePullRequest` → `set_pr_status` closed).
2. Do **not** keep building on the revoked branch.
3. Start the new requirement from the preferred base branch (clean tree).
4. Mention the revoke briefly in the new PRD/PR only if it clarifies active scope.

### 1. Orient

1. Rename the chat to a short PRD title when a rename tool is available.
2. Create a feature branch off the required base, e.g. `cursor/<prd-slug>-c1ad`.
3. Plan artifacts:
   - Source of truth: `docs/prd/<kebab-feature-slug>.md`
   - Review aid: Cursor canvas summarizing the PRD
   - PR against the base branch

### 2. Mine the requirement

Extract and preserve verbatim where the user gave exact copy (codes, thresholds, error strings, currency examples):

| Field | Capture |
| --- | --- |
| Problem | Why this exists now |
| Goal | User-visible outcome |
| Functional requirements | Must-haves |
| Acceptance criteria | Testable pass/fail |
| Out of scope | Explicit non-goals |
| Examples | e.g. ₹1,000 → ₹500; “Add ₹200 more…” |

Do not invent scope the brief excluded. If a critical product decision is ambiguous (e.g. `>` vs `>=`), document it under **Risks / open questions**—do not silently pick without calling it out.

### 3. Ground in the codebase

Before writing prose, search the repo for existing behavior that already satisfies (or partially satisfies) the brief.

1. Find admin settings, domain entities, services, storefront UI, locale strings, and tests related to the feature.
2. Build an **Exists vs Gap** table:
   - **Exists** → configure / verify (cite paths or setting names)
   - **Gap** → engineering work required for acceptance
3. Note precise platform semantics that affect AC (comparisons, tax basis, stacking rules, identity for usage limits, etc.).
4. Delivery implication: prefer “configure + verify” when the platform already supports the capability; scope build work only to gaps.

### 4. Draft the PRD markdown

Write `docs/prd/<slug>.md` using the section order in [prd-template.md](prd-template.md).

Rules:

- Keep status **Draft** unless the user says otherwise.
- Trace every brief requirement to a PRD section (traceability table).
- Include recommended admin/config values when the feature is setting-driven.
- Include concrete cart/order examples in the store’s currency when provided.
- Call out locale/copy mismatches (requested string vs stock resources) without forcing engine rewrites.
- Keep the PRD implementable: personas, journeys, FRs, UX states, ACs, test plan, delivery phases, risks.

### 5. Draft the canvas summary

Create a Cursor canvas that mirrors the PRD for quick review:

- Title + status pills
- Key stats (threshold, rate, code, etc.)
- Goals vs out of scope
- Exists vs gap (row tones: success = exists, warning = gap)
- Acceptance criteria table
- Open decisions
- Pointer to `docs/prd/<slug>.md` as source of truth

Follow the canvas skill (tokens only, no gradients/emojis/box-shadows). Link the canvas URL in the user summary.

### 6. Ship

1. Commit only the PRD (and skill files if this change includes them).
2. Push the branch; open/update the PR with a short summary + file path.
3. If the human edited the PR description, preserve their edits on later updates.
4. Write `code-turn-author-handoff.md` under `/opt/cursor/artifacts/` when operating as a Cloud Agent that requires it.

### 7. Summarize for the user

Keep it short:

1. One line: PRD ready + what it covers
2. Links: canvas + `docs/prd/<slug>.md`
3. Sharp callout: **configure vs build** (what already exists vs gaps)
4. Any open decision blockers (boundary rules, tax basis, etc.)

## Quality bar

- Grounded: cites real settings/types/UI touchpoints from this repo
- Testable: ACs are pass/fail with examples from the brief
- Scoped: out-of-scope from the brief is respected
- Honest: gaps are explicit; do not claim greenfield work when config suffices
- Durable: markdown in-repo is canonical; canvas is the review surface

## Anti-patterns

- Writing a PRD with no codebase search
- Treating stock platform behavior as “missing” without checking
- Expanding into auto-apply / tax / shipping / redesign when marked out of scope
- Skipping acceptance criteria or traceability
- Mixing unrelated feature PRDs in one document or one PR without asking

## Additional resources

- Section template: [prd-template.md](prd-template.md)
