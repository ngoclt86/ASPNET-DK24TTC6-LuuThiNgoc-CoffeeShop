# Design System Strategy: The Sensorial Brew

## 1. Overview & Creative North Star
Modern e-commerce has become a sea of generic grids and sterile white boxes. To elevate the "CoffeeShop" experience, we are moving away from "Template Minimalism" toward **The Artisanal Editorial**. 

Our Creative North Star is **"Organic Warmth."** We treat the digital interface like a high-end coffee table book—prioritizing large-scale typography, intentional white space, and a tactile sense of depth. We don't just sell beans; we curate a ritual. The design breaks the rigid 12-column tradition by using asymmetrical image placements and overlapping elements to mimic the steam and fluid nature of a fresh pour.

---

## 2. Colors & Tonal Architecture
This system utilizes a "Coffee-to-Cream" spectrum. We avoid the clinical feel of pure greys, opting instead for tinted neutrals that feel sun-drenched and inviting.

### The "No-Line" Rule
**Explicit Instruction:** Designers are prohibited from using 1px solid borders for sectioning or containment. Boundaries must be defined through background color shifts or tonal transitions. For example, a `surface-container-low` section should sit against a `surface` background to define its edge. Lines feel like wireframes; tonal shifts feel like architecture.

### Surface Hierarchy & Nesting
Treat the UI as physical layers of fine paper.
- **Surface (Base):** `#fbfbe2` (Cream) – The foundation.
- **Surface-Container-Low:** `#f5f5dc` – Used for subtle secondary zones.
- **Surface-Container-Highest:** `#e4e4cc` – Used for prominent interactive cards.
- **Nesting Logic:** To create focus, place a `surface-container-lowest` (#ffffff) card inside a `surface-container-low` section. This creates a natural "lift" without the need for heavy shadows.

### The "Glass & Gradient" Rule
To add "soul" to the digital shop:
- **CTAs:** Use subtle linear gradients for Primary buttons, transitioning from `primary` (#553722) to `primary-container` (#6f4e37) at a 135° angle.
- **Floating Elements:** Use Glassmorphism for navigation bars and overlays. Use the `surface-variant` color at 70% opacity with a `backdrop-blur` of 12px.

---

## 3. Typography: The Editorial Voice
We use typography as a structural element, not just for legibility.

- **Display & Headlines (Plus Jakarta Sans):** These are our "bold" moments. Use `display-lg` (3.5rem) for hero statements to create an authoritative, premium feel. The slight roundness of this font mirrors the organic shape of a coffee bean.
- **Body & Labels (Be Vietnam Pro):** This provides a clean, rhythmic counterpoint. Use `body-lg` (1rem) for product descriptions to ensure a leisurely reading experience.
- **Hierarchy Tip:** Contrast a `display-sm` headline with a `label-md` uppercase sub-header in `tertiary` (#682d00) to create a sophisticated, "branded" look.

---

## 4. Elevation & Depth: Beyond the Shadow
We convey importance through **Tonal Layering** rather than traditional structural lines.

### The Layering Principle
Depth is achieved by "stacking" surface tiers.
1. **Level 0 (Back):** `surface`
2. **Level 1 (Mid):** `surface-container`
3. **Level 2 (Front):** `surface-container-lowest`

### Ambient Shadows
When a card must "float" (e.g., a cart drawer or a featured product), use **Ambient Shadows**.
- **Color:** Tint the shadow using the `on-surface` color (#1b1d0e) at 6% opacity.
- **Blur:** Use a large spread (32px to 64px) to mimic natural light diffusing through a window. Never use harsh, dark grey drop shadows.

### The "Ghost Border" Fallback
If an element (like a text input) requires a container but a color shift is too subtle, use a **Ghost Border**.
- **Token:** `outline-variant` (#d4c3ba)
- **Opacity:** 20%
- **Rule:** This should feel like a suggestion of a container, not a cage.

---

## 5. Components

### Buttons: The Tactile Touch
- **Primary:** Background: `primary` (#553722) to `primary-container` (#6f4e37) gradient. Text: `on-primary`. Radius: `DEFAULT` (0.5rem/8px).
- **Secondary:** Background: `secondary-container`. Text: `on-secondary-container`. No border.
- **Tertiary:** Text only in `primary`. Use for low-priority actions like "Cancel" or "View All."

### Input Fields: The Soft Focus
- **Style:** Background: `surface-container-lowest` (#FFFFFF). 
- **Border:** Ghost Border (20% `outline-variant`).
- **Radius:** `0.375rem` (6px) to maintain a slightly sharper, professional look compared to buttons.
- **State:** On focus, transition the ghost border to 100% `primary` (#553722) with a 2px "glow" using 10% opacity of the primary color.

### Cards & Lists: The Open Layout
- **Constraint:** Forbid the use of divider lines. 
- **Separation:** Use vertical white space (32px or 48px from the Spacing Scale) to separate list items.
- **Product Cards:** Use a `surface-container-lowest` background. Images should have a 1:1 aspect ratio with a `0.5rem` radius. Text should be left-aligned with a high-contrast hierarchy (Headline-sm for price, Body-md for title).

### Signature Component: The "Brew Progress" Chip
- **Use:** To show coffee roasting levels or order status.
- **Style:** A semi-transparent pill using `tertiary-container` with `on-tertiary-container` text. It should feel integrated into the image it overlays.

---

## 6. Do’s and Don’ts

### Do:
- **Do** embrace "Active White Space." If a section feels crowded, double the padding.
- **Do** use asymmetrical image layouts. Overlap a product image across two background color sections to create depth.
- **Do** use `primary-fixed-dim` for hover states on light backgrounds to maintain warmth.

### Don’t:
- **Don't** use pure black (#000000) for text. Always use `on-background` (#1b1d0e) to keep the "organic" feel.
- **Don't** use 1px dividers between list items. Use tonal shifts or increased padding.
- **Don't** use high-intensity shadows. If the shadow is the first thing you notice, it's too dark.
- **Don't** use standard 12px "all-around" padding. Use asymmetrical padding (e.g., 40px top, 24px sides) to create an editorial flow.