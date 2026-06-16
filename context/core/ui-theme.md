# UI Design System & Theme Specifications

This document serves as the single source of truth for the user interface (UI) design of our product. Every new feature, component, and screen must strictly comply with the guidelines, color palettes, typography, and visual patterns detailed below to ensure brand consistency and user experience integrity.

---

## 1. Color Palette

Our color scheme is carefully balanced to provide an organic, warm, and highly accessible user interface. It utilizes a split-complementary and natural color harmony.

### 1.1 Primary Palette
Primary colors are used for brand identity, main structural elements, navigation, and key interactive states.

* **Primary Main (`#A5CF83`):** Soft Sage / Moss Green.
  * *Usage:* Primary call-to-action (CTA) buttons, selection states, success indicators, and dominant branding elements.
* **Primary Light / Accent (`#F0E76F`):** Muted Lemon / Pastel Yellow.
  * *Usage:* Highlights, warning subtle backgrounds, active navigation tabs, and secondary focus indicators.

### 1.2 Secondary Palette
Secondary colors add warmth, depth, and assist in establishing visual hierarchy, alerting users, or categorizing distinct functional areas.

* **Secondary Main (`#ECB65F`):** Warm Ochre / Honey Amber.
  * *Usage:* Secondary buttons, feature highlights, filtering chips, and information callout borders.
* **Secondary Dark / Accent (`#E89951`):** Soft Terracotta / Burnt Orange.
  * *Usage:* Critical interactive elements, accents on dark backgrounds, alerts, and notifications.

### 1.3 Neutral & Semantic Palette
To support the primary and secondary colors, the following neutrals must be applied:

* **Canvas/Background (Light):** `#FDFCF7` (Warm Off-White)
* **Canvas/Background (Dark):** `#23272A` (Charcoal Slate)
* **Text (Primary):** `#1A1D1A` (Deep Olive Charcoal)
* **Text (Secondary):** `#5A625A` (Muted Slate)
* **Borders/Dividers:** `#E2E8E0` (Light Gray-Green)

---

## 2. Typography

We use **Inter** as our core functional typeface due to its exceptional legibility on digital displays, paired with **Plus Jakarta Sans** for headings to give our brand a modern, friendly, yet professional character.

### 2.1 Font Family
```css
/* Headings */
font-family: 'Plus Jakarta Sans', 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
/* Body / UI */
font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
```

### 2.2 Type Scale
Use a single, predictable scale. Sizes are in `rem` (root = 16px) and pair with the weights below.

| Token | Size | Weight | Usage |
|---|---|---|---|
| Display | 2.25rem | 700 (Bold) | Page hero / landing headline |
| Heading 1 | 1.75rem | 700 (Bold) | Page title (e.g. a pizza name on its details page) |
| Heading 2 | 1.375rem | 600 (SemiBold) | Section headers (e.g. "Reviews") |
| Heading 3 | 1.125rem | 600 (SemiBold) | Card titles (e.g. a pizza name on a catalog card) |
| Body | 1rem | 400 (Regular) | Default paragraph and UI text |
| Small | 0.875rem | 400 (Regular) | Secondary text, captions, review dates |
| Label | 0.875rem | 600 (SemiBold) | Buttons, badges, the cart indicator count |

### 2.3 Rules
- **Line height:** ~1.5 for body text, ~1.2 for headings.
- **Color:** headings and body use **Text (Primary)** `#1A1D1A`; secondary/caption text uses
  **Text (Secondary)** `#5A625A`. Never rely on color alone to convey meaning (see accessibility).
- **Consistency over cleverness:** use the tokens above rather than introducing one-off sizes or
  weights. New components reuse the scale.

---

## 3. Accessibility

Every feature must remain usable by everyone — this is a non-negotiable from the
[constitution](./constitution.md).

- **Contrast:** text meets WCAG AA against its background (≥ 4.5:1 for body, ≥ 3:1 for large text).
  The warm canvas (`#FDFCF7`) with `#1A1D1A` text satisfies this; verify any new pairing.
- **Keyboard:** all interactive controls (buttons, links, the cart indicator, quantity +/−) are
  focusable and operable by keyboard, with a visible focus state.
- **Assistive tech:** images carry meaningful `alt` text (e.g. the pizza name); icon-only and
  numeric controls carry an accessible label (e.g. `aria-label="Cart: 3 items"`,
  `aria-label="4 out of 5 stars"`). Don't convey state through color or icon alone.
- **Responsive:** layouts read well from mobile (single column) to desktop (multi-column grid)
  without horizontal scrolling.