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
font-family: 'Plus Jakarta Sans', 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;