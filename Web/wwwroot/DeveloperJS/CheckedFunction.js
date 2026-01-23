var selectedIds = [];
var previousSearchText = "";
var previousSearchField = "";
var isFirstSelectAll = true;
var searchChanged = false;
var globalAllChecked = false;
function updateUICheckboxes(tableSelector, checkboxClass, masterCheckboxSelector) {
    return new Promise((resolve, reject) => {
        try {
            let allCheckedOnPage = true;

            $(`${tableSelector} tbody input[type="checkbox"].${checkboxClass}`).each(function () {
                const id = $(this).val().toString();
                if (globalThis.selectedIds.includes(id)) {
                    $(this).prop('checked', true);
                } else {
                    $(this).prop('checked', false);
                    allCheckedOnPage = false; // At least one unchecked
                }
            });

            // Update master checkbox
            $(masterCheckboxSelector).prop('checked', allCheckedOnPage);

            resolve(); // Resolve the Promise when done
        } catch (error) {
            reject(error); // Reject the Promise if there's an error
        }
    });
}

async function updateSelectedIds(tableSelector, checkboxClass) {
    try {
        // Assuming these functions might be async and involve DOM fetching or network requests
        const idsOnPage = await getAllIds(tableSelector, checkboxClass);
        const idsChecked = await getSelectedIds(tableSelector, checkboxClass);

        // Remove unchecked
        globalThis.selectedIds = globalThis.selectedIds.filter(id => !idsOnPage.includes(id));

        // Add checked
        idsChecked.forEach(id => {
            if (!globalThis.selectedIds.includes(id)) {
                globalThis.selectedIds.push(id);
            }
        });

        console.log("Updated selectedIds:", globalThis.selectedIds);
    } catch (error) {
        console.error("Error updating selectedIds:", error);
    }
}

async function getSelectedIds(tableSelector, checkboxClass) {
    return new Promise((resolve, reject) => {
        try {
            let ids = [];
            $(`${tableSelector} tbody input.${checkboxClass}:checked`).each(function () {
                ids.push($(this).val().toString());
            });
            resolve(ids); // Resolve the promise with the selected ids
        } catch (error) {
            reject(error); // Reject the promise if any error occurs
        }
    });
}

async function getAllIds(tableSelector, checkboxClass) {
    return new Promise((resolve, reject) => {
        try {
            let all = [];
            $(`${tableSelector} tbody input.${checkboxClass}`).each(function () {
                all.push($(this).val().toString());
            });
            resolve(all); // Resolve the promise with the array of ids
        } catch (error) {
            reject(error); // Reject the promise if any error occurs
        }
    });
}
function getSearchStatusForBindDialog(search) {
    const currentSearchText = search.trim();

    // Ensure searchChanged is only true when the actual search field or text changes.
    globalThis.searchChanged = (
        (currentSearchText !== globalThis.previousSearchText)
    );

    // Update previous values after comparison
    globalThis.previousSearchText = currentSearchText;

    return {
        searchChanged: globalThis.searchChanged,
        currentSearchText
    };
}