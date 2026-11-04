# PRD: 50% Off Coupon Code (nopCommerce)

| Field | Value |
| --- | --- |
| **Status** | Draft |
| **Feature name** | 50% Off Coupon Code |
| **Primary system** | nopCommerce → Promotions → Discounts |
| **Example code** | `SAVE50` |
| **Currency context** | INR (₹) |
| **Out of scope** | Auto-applied discounts; tax or shipping rule changes |

---

## 1. Problem statement

The store wants to give **selected customers 50% off** with a coupon code. Right now there is **no coupon set up**, so customers have no way to claim the discount at checkout.

Without a configured, coupon-required discount, shoppers cannot enter a code, see a reduced cart total, or receive an order confirmation that includes the discount.

---

## 2. Goal

A customer who enters a **valid coupon code** gets **50% off their products**.

---

## 3. Goals and non-goals

### In scope

- Admin creates a 50% discount under **Promotions → Discounts**, with **Requires coupon code** enabled and a code such as `SAVE50`.
- Customer can enter and apply the code in the **cart** or at **checkout**.
- Valid code → cart shows 50% off and the new total.
- Invalid or expired code → error (e.g. “This coupon code is invalid or expired.”) and price unchanged.
- Admin can set **start/end dates** and a **usage limit** (once per customer or a total number of uses).
- Coupon **does not stack** with other discounts.
- Customer can **remove** the coupon before placing the order.
- **Order total** and **order email** show the discount.

### Out of scope

- Auto-applied discounts (no code required).
- Any change to tax calculation.
- Any change to shipping calculation.

---

## 4. Success metrics / acceptance criteria

| ID | Criterion | Pass condition |
| --- | --- | --- |
| AC-1 | Valid `SAVE50` on a ₹1,000 cart | Shows ₹500 (50% off products) |
| AC-2 | Invalid or expired code | Error shown; price unchanged |
| AC-3 | Usage limit reached | Code no longer works for that customer (or globally, if total-uses limit) |
| AC-4 | Order total | Discount reflected on the placed order |
| AC-5 | Order email | Discount shown in the confirmation email |

Additional behavioral checks implied by requirements:

| ID | Criterion | Pass condition |
| --- | --- | --- |
| AC-6 | Remove coupon | Customer can clear the code before place-order; totals restore |
| AC-7 | Non-stacking | Coupon does not combine additively with other discounts when cumulative is off |

---

## 5. Users

| Persona | Need |
| --- | --- |
| **Store admin** | Configure a 50% coupon with dates and usage limits under Promotions → Discounts. |
| **Customer** | Enter a valid code at cart/checkout, see 50% off, remove it if needed, and see the discount on the order and email. |

---

## 6. Platform baseline

nopCommerce already supports this feature path:

| Capability | Mechanism |
| --- | --- |
| Admin discount setup | Promotions → Discounts |
| Coupon required + code | `RequiresCouponCode`, `CouponCode` |
| 50% off | `UsePercentage = true`, `DiscountPercentage = 50` |
| Start / end dates | `StartDateUtc`, `EndDateUtc` |
| Usage limits | `DiscountLimitationType`: Unlimited, N Times Only, N Times Per Customer + `LimitationTimes` |
| No stacking | `IsCumulative = false` |
| Cart / checkout apply & remove | Storefront discount coupon box |
| Validation messages | Locale resources under `ShoppingCart.DiscountCouponCode.*` |
| Order + email | Order discount fields and order confirmation templates |

**Delivery implication:** Prefer **configure + verify** against existing behavior. Open engineering work only if acceptance testing finds a gap (messaging, email, validation, etc.).

---

## 7. Recommended discount configuration

| Setting | Recommended value |
| --- | --- |
| Name | e.g. `50% Off Coupon` |
| Discount type | **Assigned to order subtotal** (product money off without changing shipping) |
| Use percentage | Yes |
| Discount percentage | `50` |
| Requires coupon code | Yes |
| Coupon code | `SAVE50` (or store-chosen equivalent) |
| Cumulative | **No** |
| Start date (UTC) | Campaign start |
| End date (UTC) | Campaign end |
| Limitation | N Times Per Customer (e.g. 1) and/or N Times Only for a global cap |
| Is active | Yes |

If merchandising must limit eligibility to specific products or categories, use **Assigned to SKUs / categories** instead and document the assignments in the runbook.

---

## 8. User journeys

### 8.1 Admin creates the coupon

1. Open **Promotions → Discounts → Add new**.
2. Set 50% percentage, require coupon, set code (e.g. `SAVE50`).
3. Set start/end dates and usage limitation.
4. Leave cumulative **off**.
5. Save (and assign products/categories if that discount type requires it).

### 8.2 Customer applies a valid code

1. Cart products total ₹1,000.
2. Enter `SAVE50` in cart or checkout and apply.
3. Validation passes (code, dates, active, usage).
4. Cart shows ₹500 off and ₹500 product total (tax/shipping unchanged).
5. Place order → order details and email include the discount.

### 8.3 Invalid or expired code

1. Customer enters a wrong, inactive, or past-end-date code.
2. No discount applied; error shown; totals unchanged.

### 8.4 Usage limit reached

1. Customer (or store-wide pool) has used the allowed number of times.
2. Further apply attempts fail; no discount; error shown.

### 8.5 Remove coupon

1. With code applied, customer removes it.
2. Totals recalculate without the discount before place-order.

---

## 9. Functional requirements

### FR-1 Admin configuration

- Admin can create a 50% discount under Promotions → Discounts.
- Admin can enable Requires coupon code and set a code such as `SAVE50`.
- Admin can set start and end dates.
- Admin can set usage limits: once per customer and/or a total number of uses.
- Admin can prevent stacking by leaving cumulative disabled.

### FR-2 Customer apply

- Customer can enter the code in cart or checkout.
- Valid code → cart shows 50% off and new total.
- Invalid/expired → error; price unchanged.

### FR-3 Validation

Reject apply (no discount) when the code is unknown, inactive, outside the date window, usage-exhausted, or not applicable to the cart.

### FR-4 Non-stacking

With cumulative off, the coupon must not stack with other discounts beyond platform non-cumulative rules.

### FR-5 Remove

Customer can remove the applied coupon before placing the order; totals restore.

### FR-6 Order and email

Placed order and order confirmation email show the discount.

---

## 10. UX and messaging

| Scenario | Expected UX |
| --- | --- |
| Valid apply | Success message; applied code visible with remove control; totals updated |
| Invalid / expired | Error such as “This coupon code is invalid or expired.” (or existing locale equivalent); totals unchanged |
| Empty / unknown / not applicable | Clear failure message; totals unchanged |

Stock locale strings (e.g. `ShoppingCart.DiscountCouponCode.Invalid`) are acceptable; stores may override copy without changing discount logic.

---

## 11. Test plan

1. Configure discount per §7 with `SAVE50`, dates covering now, limit = 1 per customer, cumulative = false.
2. ₹1,000 cart + `SAVE50` → expect ₹500 (AC-1).
3. Invalid and expired codes → error, unchanged total (AC-2).
4. Exhaust per-customer (or total) limit → code stops working as configured (AC-3).
5. Place order → order total and email show discount (AC-4, AC-5).
6. Apply then remove → totals restore (AC-6).
7. Confirm no auto-apply without a code; tax and shipping rules unchanged.

---

## 12. Risks

| Risk | Mitigation |
| --- | --- |
| Wrong discount type (subtotal vs SKU/category) | Confirm merchandising scope before go-live |
| Other live discounts compete (“best discount wins”) | Pause overlapping offers or accept platform behavior |
| Guest “per customer” limits are weaker | Prefer registered customers for strict 1× limits |
| Requested error copy vs default locale string | Optional locale override |

---

## 13. Delivery plan

| Phase | Work | Outcome |
| --- | --- | --- |
| 1. Spec lock | Approve type, dates, limits, code | Config contract |
| 2. Configure | Create/activate discount per §7 | Coupon ready in admin |
| 3. Verify | Run §11 / AC-1…AC-7 in staging | Acceptance green |
| 4. Go-live | Activate within date window; share code with selected customers | Feature live |
| 5. Eng follow-up | Only if AC fails | Patch as needed |

---

## 14. Requirements traceability

| Brief requirement | Covered by |
| --- | --- |
| Admin 50% discount + Requires coupon + `SAVE50` | §7, FR-1 |
| Enter code in cart or checkout | §8.2, FR-2 |
| Valid → 50% off + new total | AC-1, FR-2 |
| Invalid → error, price unchanged | AC-2, FR-3 |
| Start/end dates + usage limits | §7, FR-1, FR-3, AC-3 |
| Does not stack | FR-4, AC-7 |
| Customer can remove coupon | §8.5, FR-5, AC-6 |
| Order total + email show discount | FR-6, AC-4, AC-5 |
| Out of scope: auto-apply, tax, shipping | §3 |

---

## Appendix A — Example cart math

| Line | Amount (₹) |
| --- | --- |
| Product subtotal | 1,000 |
| `SAVE50` (50%) | −500 |
| Product total after discount | 500 |
| Tax | Unchanged (out of scope) |
| Shipping | Unchanged (out of scope) |

---

## Appendix B — Platform touchpoints (QA / implementers)

- Admin: Promotions → Discounts (`DiscountController`, discount admin models)
- Domain: `Nop.Core.Domain.Discounts.Discount`, `DiscountType`, `DiscountLimitationType`
- Services: `DiscountService`, `OrderTotalCalculationService`, customer coupon apply/parse/remove helpers
- Storefront: `_DiscountBox.cshtml`, shopping cart apply/remove actions
- Messages: `ShoppingCart.DiscountCouponCode.*`
