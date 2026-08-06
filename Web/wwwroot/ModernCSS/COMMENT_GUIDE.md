# ECMS CSS Comment Guide

All CSS files now contain rule-level comments in this format:

```css
/* Effect: explains what this selector controls. */
.selector {
    property: value;
}
```

## How future developers should edit

1. Change colors, spacing, radii, background image and heights first in:
   `css/ecms.00.tokens.css`

2. Change all form controls in:
   `css/ecms.03.forms.css`

3. Change all DataTable behavior/appearance in:
   `css/ecms.05.datatables.css`

4. Change all modal styling in:
   `css/ecms.06.modals.css`

5. Keep the comments updated when changing a rule.  
   The comments are intentionally simple and practical, so developers can quickly find the right file and rule.

## Important

Keep `ecms.theme.bundle.css` loaded after old CSS files. This is required because the ECMS theme is designed to override older page-specific styles safely.
