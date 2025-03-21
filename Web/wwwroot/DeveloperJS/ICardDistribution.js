var table; // Declare table variable outside the function to preserve the instance
var isValidCsvFile = false;

$(function () {
    //mMsater(0, "ddlRank", Rank, "");
    //mMsater(0, "ddlArmType", ArmyType, "");
    //BindData()

    $("#btnDistributionAdd").on("click", function () {
        //Reset();
        //ResetErrorMessage();
        $("#btnDistributionAddButton").val("Upload");
        $("#AddDistributionRecords").modal('show');
    });
    $("#btnProfileAddReset").on("click", function () {
        //Reset();
        //ResetErrorMessage();
    });

    $("#txtSearch").on("keyup", function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            if ($("input[type='radio'][name=choice]:checked").val() == "UserId") {
                var num_val = parseInt(eThis.val());
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData()
                }
            }
            else {
                BindData()
            }
        }
        else {
            alert("Select Choice");
        }
    });
});

function validateCsvFile() {
    // Get the file input element and the file selected by the user
    var fileInput = $('#csvFile')[0];
    var file = fileInput.files[0];

    if (!file) {
        showFileValidationError('Please select a CSV file.');
        return;
    }

    // 2. Check if the file is a CSV
    var fileType = file.name.split('.').pop().toLowerCase();
    if (fileType !== 'csv') {
        showFileValidationError('Only CSV files are allowed.');
        return;
    }

    // 3. Read the file using FileReader to validate columns and data
    var reader = new FileReader();
    reader.onload = function (event) {
        var content = event.target.result;
        // Split the content by new lines
        var rows = content.split('\n');
        // Trim each row to remove any \r or trailing spaces
        rows = rows.map(row => row.trim());

        if (rows.length <= 2) {
            showFileValidationError('The CSV file must contain at least 1 data row.');
            return;
        }
        // Split the first row (headers) and trim each column
        var headers = rows[0].split(',').map(header => header.trim());

        // 4. Validate columns (missing or duplicate columns)
        var expectedColumns = ['RequestId', 'ArmyNo', 'CardSerialNo','ChipNo']; // Modify this based on your required columns
        var missingColumns = expectedColumns.filter(col => !headers.includes(col));
        var duplicateColumns = headers.filter((value, index, self) => self.indexOf(value) !== index);

        if (missingColumns.length > 0) {
            showFileValidationError('Missing columns: ' + missingColumns.join(', '));
            return;
        }
        if (duplicateColumns.length > 0) {
            showFileValidationError('Duplicate columns found: ' + duplicateColumns.join(', '));
            return;
        }

        var formData = new FormData();
        formData.append("file", file);

        // 3. Send the form data using AJAX
        $.ajax({
            url: '/BasicDetail/ICardDistibutionUploadCsv', // Controller action URL
            type: 'POST',
            data: formData,
            processData: false, // Do not process data
            contentType: false, // Do not set content-type header
            success: function (response) {
                // Show success messaged
                debugger;
                $('#successMessage').text(response.message).show();
            },
            error: function (xhr, status, error) {
                // Show error messages
                $('#errorMessages').html('An error occurred during file upload: ' + xhr.responseText);
            }
        });

    };

    // Trigger the reading of the CSV file
    reader.readAsText(file);
}

function showFileValidationError(message) {
    //Swal.fire({
    //    icon: 'error',
    //    title: 'Invalid File',
    //    text: message,
    //})

    toastr.error(message);
}

