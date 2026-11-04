# PRD: Free Shipping Above a Threshold

| Field | Value |
| --- | --- |
| **Status** | Draft |
| **Feature name** | Free shipping over ₹999 |
| **Owner** | Merchandising / Store Admin |
| **Primary system** | nopCommerce Configuration → Settings → Shipping |
| **Currency context** | INR (₹) |
| **Related setting** | `FreeShippingOverXEnabled` / `FreeShippingOverXValue` |

---

## 1. Problem statement

The store wants customers to get **free shipping when the order qualifies above ₹999**, and to see a clear cart nudge when they are close—for example, **“Add ₹200 more for free shipping.”**

Today, stock nopCommerce can grant free shipping over an amount via shipping settings, but it does **not** show a remaining-amount cart message. Without that nudge, customers often abandon or check out just under the threshold and pay shipping they would have avoided with a small add-on purchase.

---

## 2. Goals and non-goals

### Goals

- Orders whose qualifying cart amount is **over ₹999** receive **₹0 shipping** (free shipping).
- Cart (and checkout, where totals are shown) displays a progress message when the customer is below the threshold, e.g. **“Add ₹200 more for free shipping.”**
- When the threshold is met, the message updates to a success state (e.g. “You’ve unlocked free shipping”) and shipping shows as free.
- Admin can enable/disable the rule and set the threshold value (₹999 for this campaign).
- Message amount updates live as the cart changes (add/update/remove qty).

### Non-goals (out of scope)

- Changing tax calculation rules.
- Redesigning the full shipping-method UI or estimate-shipping popup.
- Free shipping by product flag, customer role, or coupon (those already exist separately).
- Geographic carrier-rate engine changes (beyond zeroing shipping when the threshold rule applies).
- Multi-currency threshold localization beyond the store’s primary currency display.

---

## 3. Success metrics

| Metric | Target |
| --- | --- |
| Cart subtotal ₹799 (example) | Message: “Add ₹200 more for free shipping.” Shipping still charged (if a method is selected) |
| Qualifying amount **over** ₹999 | Shipping total = ₹0 |
| Cart at/under threshold | No free shipping from this rule; remaining amount shown |
| Threshold disabled | No free-shipping-over-X behavior; no progress message |
| Cart qty change | Remaining amount recalculates correctly |

---

## 4. Users and personas

| Persona | Need |
| --- | --- |
| **Store admin** | Turn on free shipping over ₹999 without custom carrier logic. |
| **Shopper below threshold** | See exactly how much more to add to unlock free shipping. |
| **Shopper at/above threshold** | See free shipping applied on cart/checkout totals and on the order. |

---

## 5. Current platform capability (baseline)

### Already available

| Capability | Location |
| --- | --- |
| Enable free shipping over amount X | Admin → **Configuration → Settings → Shipping** → “Free shipping over 'X'” (`FreeShippingOverXEnabled`) |
| Threshold value | `FreeShippingOverXValue` (set to `999` for this feature) |
| Evaluate X incl/excl tax | `FreeShippingOverXIncludingTax` |
| Apply ₹0 shipping when qualified | `OrderTotalCalculationService.IsFreeShippingAsync` → `AdjustShippingRateAsync` |
| Product-level “free shipping” badge | Catalog setting `ShowFreeShippingNotification` (per-product free shipping only—**not** the cart threshold nudge) |

### Platform rule detail (important)

In `IsFreeShippingAsync`, qualification uses:

```text
subTotal > FreeShippingOverXValue
```

So with `FreeShippingOverXValue = 999`:

- ₹999.00 → **does not** qualify  
- ₹999.01+ → **does** qualify  

This matches the product language **“orders over ₹999.”** If merchandising later wants **“₹999 and above”** (`>=`), that is a separate decision (would require a small code change or setting the value to `998.99`).

### Gap (requires product/engineering work)

| Missing today | Needed |
| --- | --- |
| Cart/checkout “Add ₹X more for free shipping” message | Compute `remaining = threshold − currentQualifyingSubtotal` (when remaining &gt; 0), format in store currency, show in cart UI |
| Success message when unlocked | Show unlocked state when free-shipping-over-X applies |

**Implication:** Enabling the ₹999 rule is **configuration**. The progress message is a **storefront enhancement** on top of existing settings.

---

## 6. Recommended configuration

| Setting | Value | Notes |
| --- | --- | --- |
| Free shipping over 'X' | **Enabled** | Turns on the rule |
| Value of 'X' | **999** | INR primary currency |
| Calculate 'X' including tax | **No** (recommended default) unless finance requires tax-inclusive threshold | Aligns with “cart products” mental model; document the choice |
| Estimate shipping on cart | Optional | Independent of this feature |

---

## 7. User journeys

### 7.1 Admin enables free shipping over ₹999

1. Admin opens **Configuration → Settings → Shipping**.
2. Enables **Free shipping over 'X'**.
3. Sets **Value of 'X'** to `999`.
4. Chooses whether X includes tax.
5. Saves.
6. (After engineering delivery) Confirms cart message appears for under-threshold carts.

### 7.2 Customer below threshold

1. Customer has a cart totaling ₹799 (qualifying subtotal).
2. Cart shows: **“Add ₹200 more for free shipping.”**
3. Shipping method still shows a non-zero rate (unless another free-shipping rule applies).
4. Customer adds ₹200+ of products → message clears / success state; shipping becomes free.

### 7.3 Customer over threshold

1. Customer cart qualifying subtotal is ₹1,200.
2. Shipping line shows free (₹0 / “Free”).
3. Optional success copy: “You’ve unlocked free shipping.”
4. Placed order persists ₹0 shipping.

### 7.4 Edge: other free-shipping sources

If all products are free-shipping or the customer role has free shipping, shipping is already free. Progress message SHOULD NOT urge the customer to add more (already free).

---

## 8. Functional requirements

### FR-1 Threshold free shipping

- When `FreeShippingOverXEnabled` is true and qualifying subtotal **&gt;** `FreeShippingOverXValue` (999), shipping MUST be free for that cart/order.
- When below or equal to the threshold (per platform `>` rule), this rule MUST NOT zero shipping by itself.

### FR-2 Remaining-amount cart message

- When the rule is enabled, the cart requires shipping, the customer is not already free-shipping for another reason, and qualifying subtotal is ≤ threshold:
  - Compute `remaining = FreeShippingOverXValue − qualifyingSubtotal` for display purposes when using “over X” messaging, **or** display based on amount still needed to surpass X (see §13).
  - Show copy equivalent to: **“Add {0} more for free shipping.”** with `{0}` formatted as currency (e.g. ₹200).
- Message MUST update after cart item add/update/remove (full postback or AJAX totals refresh, matching existing cart patterns).

### FR-3 Unlocked state

- When free shipping applies because of the over-X rule, show shipping as free and optionally show success copy.
- Do not show the “Add ₹X more…” message once unlocked.

### FR-4 Admin control

- Admin MUST be able to enable/disable and change X without a deploy (existing settings UI).
- Message feature SHOULD respect the same enable flag (no orphaned message when over-X is off).

### FR-5 Checkout / order consistency

- Checkout totals MUST match cart once a shipping method is selected.
- Order record MUST store shipping as zero when free shipping applied.

---

## 9. UX and messaging

| Cart state | Message |
| --- | --- |
| Rule off | No threshold message |
| Already free (role / all items free ship) | No “add more” nudge |
| Below threshold (e.g. ₹799 of ₹999+) | `Add ₹200 more for free shipping.` |
| Just unlocked / over threshold | Shipping free; optional `You've unlocked free shipping!` |
| Digital / shipping not required | No message |

**Placement:** Adjacent to cart order totals / shipping row (shopping cart and checkout summary), not only on product pages.

**Accessibility:** Message is plain text (or status region); currency formatted via existing price formatting helpers.

---

## 10. Acceptance criteria

| ID | Criterion | Pass condition |
| --- | --- | --- |
| AC-1 | Over threshold | Qualifying amount &gt; ₹999 → shipping = ₹0 |
| AC-2 | At exactly ₹999 | Per platform `>` rule: does **not** unlock over-X free shipping |
| AC-3 | Below threshold message | Cart ₹799 → shows “Add ₹200 more for free shipping.” (or locale equivalent with formatted ₹200) |
| AC-4 | Message updates | Increasing cart from ₹799 to ₹1,050 removes “add more” and shows free shipping |
| AC-5 | Rule disabled | No over-X free shipping; no progress message |
| AC-6 | Order persistence | Placed over-threshold order has free shipping stored |
| AC-7 | Non-interference | Product-only free shipping / role free shipping still work; no incorrect “add more” when already free |

---

## 11. Test plan

### Configuration

1. Enable Free shipping over X = 999, including-tax = off.
2. Confirm admin values save and load.

### Calculation

1. Cart ₹799 → shipping not free via over-X; remaining message ₹200.
2. Cart ₹999.00 → not free via over-X (`>`).
3. Cart ₹999.01 / ₹1,000 → free shipping.
4. Toggle including-tax and repeat with tax-on products if used.

### Message / UX

1. Verify message on shopping cart.
2. Verify message on checkout totals (if in scope for placement).
3. Change quantities; confirm AJAX/full refresh updates remaining.
4. Empty cart / non-shippable cart → no message.

### Regression

1. Customer role with FreeShipping = true → free; no add-more nudge.
2. All cart items `IsFreeShipping` → free; no add-more nudge.
3. Estimate shipping popup still functions.

---

## 12. Delivery plan

| Phase | Work | Outcome |
| --- | --- | --- |
| **1. Spec lock** | Confirm threshold ₹999, `>` vs `>=`, tax-incl choice, message copy | Config + UX contract |
| **2. Configure** | Enable Free shipping over X = 999 in staging/prod | Rule live for totals |
| **3. Build message** | Storefront remaining-amount banner/line + locale resource | AC-3, AC-4 |
| **4. Verify** | Execute §11 | AC-1…AC-7 green |
| **5. Go-live** | Enable in production; monitor support tickets | Feature live |

### Suggested engineering scope for FR-2/FR-3

1. Extend shopping cart / order totals model with `FreeShippingProgress` (remaining amount, unlocked flag, display strings).
2. Compute remaining using the same subtotal basis as `IsFreeShippingAsync` (`FreeShippingOverXIncludingTax`).
3. Render in cart/checkout totals partial.
4. Add locale resources, e.g. `ShoppingCart.FreeShipping.AddMore` = `Add {0} more for free shipping.` and `ShoppingCart.FreeShipping.Unlocked`.
5. Unit tests around remaining calculation and boundary at exactly 999.

---

## 13. Risks and open questions

| Risk / question | Impact | Decision needed |
| --- | --- | --- |
| `>` vs `>=` at exactly ₹999 | Customers at ₹999 still pay shipping | Confirm “over ₹999” vs “₹999 and above” with merchandising |
| Remaining display math | For `>` rule, unlocking needs amount **strictly greater** than 999; “Add ₹200” from ₹799 is clear, but at ₹999.00 remaining display should imply needing more than ₹0 (e.g. smallest currency step) | Define copy for “₹0 remaining but not yet over” edge |
| Subtotal basis (pre/post discount, tax) | Message vs actual unlock can disagree if bases differ | Reuse exact subtotal used in `IsFreeShippingAsync` |
| Discounts lowering subtotal under 999 | Customer may lose free shipping after coupon | Expected; message should reflect post-discount qualifying amount |
| Pickup in store | Free shipping messaging may confuse | Hide nudge when shipping not required / pickup selected |

---

## 14. Requirements traceability

| Requirement (from brief) | Covered by |
| --- | --- |
| Free shipping on orders over ₹999 | §6, FR-1, AC-1 |
| Cart message “Add ₹200 more for free shipping.” | FR-2, AC-3 |
| Out of scope / prior coupon PRD revoked | This document replaces coupon PRD work for this agent track |

---

## Appendix A — Example math

| Cart qualifying subtotal | Remaining to show | Free shipping via over-X? |
| --- | --- | --- |
| ₹799 | ₹200 | No |
| ₹999.00 | Need &gt; ₹0 more (edge) | No |
| ₹1,000 | — | Yes |
| ₹1,200 | — | Yes |

---

## Appendix B — Key platform touchpoints

- Settings: `Nop.Core.Domain.Shipping.ShippingSettings` (`FreeShippingOverXEnabled`, `FreeShippingOverXValue`, `FreeShippingOverXIncludingTax`)
- Admin UI: `Areas/Admin/Views/Setting/_Shipping.Common.cshtml`
- Calculation: `OrderTotalCalculationService.IsFreeShippingAsync`, `AdjustShippingRateAsync`
- Cart totals UI: `Views/Shared/Components/OrderTotals/Default.cshtml` (likely message host)
- Existing product badge (do not confuse): `ShowFreeShippingNotification` / `_DeliveryInfo.cshtml`
