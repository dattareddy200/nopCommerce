# PRD: 50% Off Coupon Code (DIWALI)

| Field | Value |
| --- | --- |
| **Status** | Draft |
| **Feature name** | Diwali 50% Off Coupon (`SAVE50`) |
| **Owner** | Merchandising / Store Admin |
| **Primary system** | nopCommerce Promotions → Discounts |
| **Currency context** | INR (₹) |
| **Out of scope** | Auto-applied discounts; tax or shipping rule changes |

---

## 1. Problem statement

The store wants to offer selected customers **50% off** product prices during a Diwali promotion. Today there is **no coupon configured**, so shoppers have no way to claim the discount at cart or checkout.

Without a working coupon path, the campaign cannot run: customers cannot enter a code, see a reduced total, or receive an order confirmation that reflects the discount.

---

## 2. Goals and non-goals

### Goals

- An admin can create a **50% percentage discount** that requires a coupon code (e.g. `SAVE50`).
- A customer who enters a **valid** code in cart or checkout sees **50% off products** and an updated total.
- Invalid, expired, or over-limit codes show a clear error and leave the cart total unchanged.
- Admins can control **start/end dates** and **usage limits** (per customer and/or total uses).
- The coupon **does not stack** with other discounts.
- Customers can **remove** the coupon before placing the order.
- Placed orders and order emails **show the discount**.

### Non-goals (out of scope)

- Auto-applied discounts (no code required).
- Any change to tax calculation rules.
- Any change to shipping calculation rules.
- Gift cards, reward points, or store credits as the discount vehicle.
- Customer-segment targeting beyond what existing discount requirements already support (unless added later as a follow-up).
- New storefront UI redesign of the coupon box.

---

## 3. Success metrics

| Metric | Target |
| --- | --- |
| Valid `SAVE50` on a ₹1,000 product subtotal | Discount = ₹500; payable product total = ₹500 (before tax/shipping) |
| Invalid / expired / exhausted code | Error shown; cart total unchanged |
| Usage limit reached | Further apply attempts fail for that customer / globally as configured |
| Order record + order email | Discount line visible and consistent with cart at checkout |
| Coupon removable | Customer can clear the code and restore pre-discount totals before place-order |

---

## 4. Users and personas

| Persona | Need |
| --- | --- |
| **Store admin** | Configure the Diwali 50% coupon once, with dates and usage caps, without custom development for each campaign. |
| **Eligible customer** | Enter `SAVE50` (or the configured code) at cart/checkout, see 50% off, complete purchase, receive email showing the discount. |
| **Ineligible / mistaken customer** | Understand why a code failed (invalid, expired, or usage limit) without silent failure. |

---

## 5. Current platform capability (baseline)

nopCommerce already provides the building blocks this feature relies on:

| Capability | Where it lives |
| --- | --- |
| Admin discount CRUD | Admin → **Promotions → Discounts** |
| Requires coupon + coupon code | `RequiresCouponCode`, `CouponCode` on `Discount` |
| Percentage discount | `UsePercentage`, `DiscountPercentage` |
| Start / end dates | `StartDateUtc`, `EndDateUtc` |
| Usage limits | `DiscountLimitationType`: Unlimited, N Times Only, N Times Per Customer + `LimitationTimes` |
| Non-stacking | `IsCumulative = false` (non-cumulative discounts compete; best single discount wins) |
| Cart / checkout apply & remove | Storefront discount coupon box (`_DiscountBox.cshtml`); shopping cart apply/remove actions |
| Validation messaging | Locale resources such as `ShoppingCart.DiscountCouponCode.CannotBeFound`, `.Invalid`, `.WrongDiscount`, `.Applied` |
| Order totals & history | Order stores discount amounts; admin order details and customer emails surface discount lines |

**Implication:** Primary delivery for this PRD is **campaign configuration + verification** against existing behavior. Engineering work is limited to gaps found during acceptance testing (e.g. copy tweaks, currency display, email template confirmation), not a greenfield coupon engine.

---

## 6. Recommended discount configuration

Create (or update) one active discount with these settings:

| Setting | Recommended value | Rationale |
| --- | --- | --- |
| Name | `Diwali 50% Off` (admin-facing) | Clear campaign identity |
| Discount type | **Assigned to order subtotal** (or Assigned to products/categories if merchandising needs SKU-scoped eligibility) | Matches “50% off their products” without changing tax/shipping |
| Use percentage | Yes | Percentage off |
| Discount percentage | `50` | Campaign rate |
| Requires coupon code | Yes | Claim path is code entry |
| Coupon code | `SAVE50` | Acceptance-criteria code |
| Cumulative | **No** (`IsCumulative = false`) | Must not stack with other discounts |
| Start date (UTC) | Campaign start | Enforce window |
| End date (UTC) | Campaign end | Enforce expiry |
| Limitation | e.g. **N Times Per Customer** = 1, and/or **N Times Only** for a global cap | Usage limits |
| Is active | Yes | Live when campaign starts |

**Notes**

- Prefer **order subtotal** when the intent is “50% off products in the cart” without touching shipping.
- If the store must exclude specific SKUs or limit to categories, use **Assigned to categories / SKUs** instead and document the assignment list in the campaign runbook.
- Coupon codes should remain unique; the admin UI already warns when a code is reserved.

---

## 7. User journeys

### 7.1 Admin creates the Diwali coupon

1. Admin opens **Promotions → Discounts → Add new**.
2. Sets name, 50% percentage, requires coupon, code `SAVE50`.
3. Sets start/end dates and usage limitation.
4. Leaves **Cumulative** off.
5. Saves and (if type requires it) assigns products/categories.
6. Optionally smoke-tests with a test customer account.

### 7.2 Customer applies a valid code

1. Customer adds products totaling ₹1,000 (product subtotal).
2. On cart or checkout, enters `SAVE50` and clicks **Apply coupon**.
3. System validates code, dates, activity, and usage limits.
4. Cart shows discount of ₹500 and updated total of ₹500 for products (tax/shipping unchanged per out-of-scope).
5. Customer places order; order details and confirmation email include the discount.

### 7.3 Customer enters an invalid or expired code

1. Customer enters a wrong, inactive, or past-end-date code.
2. System does **not** apply a discount.
3. Customer sees an error in the coupon message area (e.g. code cannot be found / invalid or no longer available / could not be applied).
4. Cart totals remain unchanged.

### 7.4 Usage limit reached

1. Customer (or store-wide usage) has already consumed the configured limitation.
2. Further apply attempts fail validation.
3. No discount is applied; error messaging is shown; totals unchanged.

### 7.5 Customer removes the coupon

1. With `SAVE50` applied, customer clicks the remove control next to the applied code.
2. Coupon is cleared from the customer’s applied codes.
3. Cart totals revert to the pre-discount amounts (still before place-order).

---

## 8. Functional requirements

### FR-1 Admin configuration

- Admin MUST be able to create a percentage discount of 50% under Promotions → Discounts.
- Admin MUST be able to enable **Requires coupon code** and set a code such as `SAVE50`.
- Admin MUST be able to set start and end dates.
- Admin MUST be able to set usage limitation: unlimited, N times only, or N times per customer.
- Admin MUST be able to disable stacking via non-cumulative discount (`IsCumulative = false`).

### FR-2 Customer apply

- Customer MUST be able to enter a coupon code on the shopping cart and at checkout (wherever the discount box is displayed).
- On success, the cart MUST show the discount amount and the new total.
- On failure, the customer MUST see an error and the price MUST remain unchanged.

### FR-3 Validation rules

When applying a code, the system MUST reject (no discount applied) if any of the following hold:

- Code does not match an active coupon-required discount.
- Current time is before start or after end (UTC as stored).
- Discount is inactive.
- Usage limitation is exhausted for the customer or globally.
- Discount is not applicable to the current cart composition (wrong type / no matching products).

### FR-4 Non-stacking

- With cumulative disabled, `SAVE50` MUST NOT combine with other non-cumulative discounts of the same evaluation path; the platform’s existing “best discount wins” / non-cumulative behavior applies.
- Explicit requirement for this campaign: operators MUST leave **Cumulative** unchecked and SHOULD avoid publishing other overlapping percentage coupons during the same window unless intentional.

### FR-5 Remove coupon

- Customer MUST be able to remove an applied coupon before placing the order.
- After removal, totals MUST recalculate without that discount.

### FR-6 Order and email visibility

- After place-order, the order MUST persist the discount amount.
- The order confirmation email MUST show the discount consistently with the order total.

---

## 9. UX and messaging

| Scenario | Expected UX |
| --- | --- |
| Valid apply | Success message (existing: coupon applied / activated) + applied code shown with remove control |
| Empty code | Empty-code message |
| Unknown code | Cannot-be-found (or equivalent) error |
| Invalid / expired / unavailable | Invalid or no-longer-available style error |
| Valid code but not applicable to cart | Couldn’t-be-applied / wrong-discount style error |
| Applied state | Show entered coupon code and updated order totals including discount line |

**Copy note:** Product language in the request used *“This coupon code is invalid or expired.”* Platform default strings are similar (e.g. *“This coupon code ({0}) is invalid or no longer available.”*). Acceptance may use either the stock locale string or a store-specific locale override; no new engine is required solely for wording.

---

## 10. Acceptance criteria

| ID | Criterion | Pass condition |
| --- | --- | --- |
| AC-1 | Valid `SAVE50` on ₹1,000 cart | Product/subtotal path shows ₹500 discount and ₹500 remaining product total (excluding tax/shipping per out of scope) |
| AC-2 | Invalid code | Error displayed; totals unchanged |
| AC-3 | Expired code (past end date) | Error displayed; totals unchanged |
| AC-4 | Usage limit reached | Further use denied for that customer / globally as configured; totals unchanged |
| AC-5 | Order total | Placed order reflects the discount |
| AC-6 | Order email | Confirmation email shows the discount |
| AC-7 | Remove coupon | Customer can remove `SAVE50` before place-order; totals restore |
| AC-8 | Non-stacking | With a second non-cumulative discount present, discounts do not stack additively beyond platform non-cumulative rules |

---

## 11. Test plan

### Happy path

1. Configure discount per §6 with `SAVE50`, 50%, dates covering “now”, limitation = 1 per customer, cumulative = false.
2. As a new customer, add items totaling ₹1,000.
3. Apply `SAVE50` → expect ₹500 discount.
4. Place order → verify order details and email.

### Negative paths

1. Apply `SAVE50X` / blank / random string → error, no discount.
2. Set end date in the past → apply fails.
3. Complete one order under 1× per customer → second cart cannot re-apply for same customer.
4. If global N-times-only is set, exhaust N and confirm subsequent customers cannot apply.

### Remove & stacking

1. Apply then remove → totals restore.
2. With another non-cumulative discount eligible, confirm no additive 50%+ stacking when cumulative is off.

### Regression (out-of-scope guards)

1. Confirm tax lines behave as before (no rule change).
2. Confirm shipping lines behave as before (no rule change).
3. Confirm no discount applies without entering a code (not auto-applied).

---

## 12. Analytics and ops

| Item | Recommendation |
| --- | --- |
| Usage tracking | Use existing discount usage history in admin discount details |
| Support runbook | Document code `SAVE50`, date window, limitation, and who is eligible |
| Rollback | Deactivate discount or set end date to now; applied-but-unplaced carts will fail revalidation |
| Code hygiene | Do not reuse `SAVE50` for a different offer later without clearing customer confusion |

---

## 13. Risks and open questions

| Risk / question | Impact | Mitigation / decision needed |
| --- | --- | --- |
| Discount type choice (order subtotal vs SKU/category) | Wrong type may exclude items or surprise merchandising | Confirm with merchandising before go-live |
| Interaction with other live discounts | Customer may get a different “best” discount instead of 50% | Pause overlapping campaigns or accept best-discount behavior |
| Guest vs registered usage limits | “Per customer” behavior depends on customer identity | Prefer registered customers for strict 1× limits, or document guest limitations |
| Locale string vs requested copy | Minor messaging mismatch | Optional locale override |
| Maximum discount amount | Percentage discounts can optionally cap | Leave unset unless finance requires a cap |

---

## 14. Delivery plan

| Phase | Work | Outcome |
| --- | --- | --- |
| **1. Spec lock** | Approve this PRD (discount type, dates, limits, code) | Config contract |
| **2. Configure** | Admin creates/activates discount per §6 | Coupon live in admin |
| **3. Verify** | Execute §11 acceptance tests in staging | AC-1…AC-8 green |
| **4. Go-live** | Activate within start/end window; share code with selected customers | Campaign running |
| **5. Monitor** | Watch usage history and support tickets for invalid/expired confusion | Ops confidence |
| **6. Eng follow-ups (only if gaps)** | Locale copy, email template fix, or bugfix if AC fails | Patch PR(s) as needed |

No new domain model is required unless acceptance testing finds a platform gap against this PRD.

---

## 15. Requirements traceability

| Requirement (from brief) | Covered by |
| --- | --- |
| Admin creates 50% discount with coupon `SAVE50` | §6, FR-1 |
| Customer enters code in cart/checkout | §7.2, FR-2 |
| Valid → 50% off + new total | AC-1, FR-2 |
| Invalid → error, price unchanged | AC-2, FR-3 |
| Start/end dates + usage limits | §6, FR-1, FR-3, AC-3, AC-4 |
| Does not stack | FR-4, AC-8 |
| Customer can remove coupon | §7.5, FR-5, AC-7 |
| Order total + email show discount | FR-6, AC-5, AC-6 |
| Out of scope: auto-apply, tax, shipping | §2 Non-goals, §11 regression |

---

## Appendix A — Example cart math

| Line | Amount (₹) |
| --- | --- |
| Product subtotal | 1,000 |
| `SAVE50` (50%) | −500 |
| Product total after discount | 500 |
| Tax | *(unchanged rules — out of scope)* |
| Shipping | *(unchanged rules — out of scope)* |

---

## Appendix B — Key platform touchpoints (for implementers/QA)

- Admin: `DiscountController`, discount admin models/views under Promotions → Discounts  
- Domain: `Nop.Core.Domain.Discounts.Discount`, `DiscountType`, `DiscountLimitationType`  
- Validation / amount: `DiscountService`  
- Cart apply/remove: `ShoppingCartController`, `_DiscountBox.cshtml`  
- Customer applied codes: `CustomerService` apply/parse/remove coupon helpers  
- Totals: `OrderTotalCalculationService`  
- Messages: `ShoppingCart.DiscountCouponCode.*` locale resources  
