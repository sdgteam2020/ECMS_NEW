// ✅ Allowed: A-Z, a-z, 0-9, space, ( ), -, /, &
const DT_SEARCH_ALLOWED_REGEX = /^[a-zA-Z0-9\s()\-\/&]$/;
const DT_SEARCH_CLEAN_REGEX = /[^a-zA-Z0-9\s()\-\/&]/g;
const DT_SEARCH_FULL_REGEX = /^[a-zA-Z0-9\s()\-\/&]*$/;

function applyDataTableSearchValidation(tableSelector) {

    // 1. Block invalid typing
    $(document).on('keypress', tableSelector + '_wrapper .dt-search input', function (e) {

        var keyCode = e.which;

        // allow control keys
        if (e.ctrlKey || e.metaKey || keyCode === 0) {
            return true;
        }

        var char = String.fromCharCode(keyCode);

        if (!DT_SEARCH_ALLOWED_REGEX.test(char)) {
            toastr.warning('Only alphabets, numbers, space, ( ), -, / and & are allowed.');
            e.preventDefault();
            return false;
        }
    });

    // 2. Block invalid paste
    $(document).on('paste', tableSelector + '_wrapper .dt-search input', function (e) {

        var pastedText = (e.originalEvent || e).clipboardData.getData('text');

        if (!DT_SEARCH_FULL_REGEX.test(pastedText)) {
            toastr.warning('Only alphabets, numbers, space, ( ), -, / and & are allowed.');
            e.preventDefault();
            return false;
        }
    });

    // 3. Final safety before AJAX request
    $(tableSelector).on('preXhr.dt', function (e, settings, data) {

        var searchValue = data.search.value || "";

        if (!DT_SEARCH_FULL_REGEX.test(searchValue)) {
            data.search.value = searchValue.replace(DT_SEARCH_CLEAN_REGEX, '');
            $(tableSelector + '_wrapper .dt-search input').val(data.search.value);
        }
    });
}