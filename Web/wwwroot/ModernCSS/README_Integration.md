# ECMS Global Common CSS Theme

This package separates the UI into common CSS modules so that design changes can be done from one place.

## File purpose

| File | Purpose |
|---|---|
| `ecms.00.tokens.css` | Main theme variables: colors, radius, heights, background image, shadows |
| `ecms.01.layout.css` | Header/nav/breadcrumb/footer/global background |
| `ecms.02.pages.css` | Page shells, cards, sections, dashboard/summary tiles |
| `ecms.03.forms.css` | Labels, inputs, selects, Select2, radio, checkbox |
| `ecms.04.buttons-badges.css` | Buttons, badges, edit/delete/action icons |
| `ecms.05.datatables.css` | DataTables header, scroll, footer, pagination, double-header fix |
| `ecms.06.modals.css` | All Bootstrap/global modals, close button, modal forms/tables |
| `ecms.07.utilities.css` | Helper classes |
| `ecms.theme.bundle.css` | One-file import bundle |

## Most important rule

Load ECMS CSS **after** Bootstrap, DataTables, Select2 and your old page CSS.

## One-place changes

### Change full system color
Edit only `ecms.00.tokens.css`:

```css
--ecms-primary-600: #2f67e6;
--ecms-primary-500: #3f7cff;
```

### Change all form control height
```css
--ecms-control-height: 36px;
```

### Change all datatable row height
```css
--ecms-dt-row-height: 46px;
```

### Change background image
```css
--ecms-bg-image: url('/Images/HomePage/indexbackgroundimg.png');
```

## Recommended page wrapper

For new or migrated pages:

```html
<div class="ecms-page ecms-fit-screen">
    <div class="ecms-page-card ecms-grid-page">
        <div class="ecms-page-title">
            <div>
                <h2>Page Title</h2>
                <p class="ecms-title-subtitle">Page subtitle</p>
            </div>
        </div>

        <div class="ecms-section ecms-dt-contained">
            <div class="ecms-section-title">Records</div>
            <table id="tblExample" class="table table-striped table-bordered dataTable">
                ...
            </table>
        </div>
    </div>
</div>
```

## DataTable initialization recommendation

Keep your existing ajax/functionality. Add or keep these UI options when a page needs table-only scrolling:

```javascript
scrollX: true,
scrollY: 'calc(100vh - 520px)',
scrollCollapse: false,
autoWidth: false,
fixedHeader: false
```

Do not enable both `fixedHeader: true` and DataTables `scrollY` on these styled pages unless specifically needed, because that is a common reason for duplicate headers.

## Body scroll control

For pages where browser scrollbar must be removed:

```javascript
document.body.classList.add('ecms-lock-page-scroll');
```

For long content pages that need normal browser scroll, do not add that class.

## Why the double header fix works

DataTables creates a cloned header inside `.dataTables_scrollBody` when `scrollY` is used. The real visible header is inside `.dataTables_scrollHead`.  
`ecms.05.datatables.css` hides only the cloned body header:

```css
.dataTables_scrollBody thead { visibility: collapse; height: 0; }
```

## Modal size classes

Use these on `.modal-dialog` only when needed:

```html
<div class="modal-dialog ecms-modal-sm">
<div class="modal-dialog ecms-modal-md">
<div class="modal-dialog ecms-modal-lg">
<div class="modal-dialog ecms-modal-xl">
```

Existing modals work without changing IDs or JavaScript.
