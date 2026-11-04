# PRD markdown template

Use this section order in `docs/prd/<slug>.md`. Omit a section only if it truly does not apply; prefer empty open questions over deleting Risks.

```markdown
# PRD: <Feature name>

| Field | Value |
| --- | --- |
| **Status** | Draft |
| **Feature name** | <short name> |
| **Owner** | <role> |
| **Primary system** | <admin area / module> |
| **Currency context** | <if relevant> |

---

## 1. Problem statement

<Why customers/admins hurt today. 1–3 short paragraphs.>

---

## 2. Goals and non-goals

### Goals

- <user-visible outcomes>

### Non-goals (out of scope)

- <explicit exclusions from the brief>

---

## 3. Success metrics

| Metric | Target |
| --- | --- |
| <scenario> | <measurable result> |

---

## 4. Users and personas

| Persona | Need |
| --- | --- |
| <persona> | <need> |

---

## 5. Current platform capability (baseline)

### Already available

| Capability | Location |
| --- | --- |
| <capability> | <setting / type / UI path> |

### Gap (requires product/engineering work)

| Missing today | Needed |
| --- | --- |
| <gap> | <build> |

**Implication:** <configure + verify vs build>

---

## 6. Recommended configuration

| Setting | Value | Notes |
| --- | --- | --- |
| <setting> | <value> | <why> |

---

## 7. User journeys

### 7.1 <Admin / setup>

1. …

### 7.2 <Happy path>

1. …

### 7.3 <Failure / edge path>

1. …

---

## 8. Functional requirements

### FR-1 …

- …

---

## 9. UX and messaging

| Scenario | Expected UX |
| --- | --- |
| <state> | <copy / UI> |

---

## 10. Acceptance criteria

| ID | Criterion | Pass condition |
| --- | --- | --- |
| AC-1 | … | … |

---

## 11. Test plan

### Happy path

1. …

### Negative paths

1. …

### Regression (out-of-scope guards)

1. …

---

## 12. Delivery plan

| Phase | Work | Outcome |
| --- | --- | --- |
| 1. Spec lock | … | … |
| 2. Configure / Build | … | … |
| 3. Verify | … | … |
| 4. Go-live | … | … |

---

## 13. Risks and open questions

| Risk / question | Impact | Mitigation / decision needed |
| --- | --- | --- |
| … | … | … |

---

## 14. Requirements traceability

| Requirement (from brief) | Covered by |
| --- | --- |
| … | § / FR / AC |

---

## Appendix A — Example math / scenarios

| Input | Result |
| --- | --- |
| … | … |

---

## Appendix B — Key platform touchpoints

- <paths, types, locale resource keys>
```

## Naming

- File: `docs/prd/<kebab-case-feature>.md`
- Branch: `cursor/<kebab-case-feature>-prd-c1ad` (or repo’s required branch prefix/suffix)
- PR title: `PRD: <feature one-liner>`
