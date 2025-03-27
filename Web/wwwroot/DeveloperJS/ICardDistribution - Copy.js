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
        //if ($("input[type='radio'][name=choice]:checked").length > 0) {
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
        //}
        //else {
        //    alert("Select Choice");
        //}
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
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                    return;
                }

                if (response == BadRequest) {
                    Swal.fire({
                        text: baderrormsg
                    });
                    return;
                }

                let swalOptions = {
                    icon: 'info',
                    showCancelButton: true,
                    cancelButtonText: 'Cancel',
                    title: 'Upload Summary',
                };

                let validStr = `<p style="color:green;"><strong>${response.ValidRecordsCount}</strong> records are ready to be upload.</p>`;
                let inValidStr = `<p ><strong>${response.InValidRecordsCount}</strong> records are invalid records.</p>`;


                // Case 1: All valid
                if (response.ValidRecordsCount > 0 && response.InValidRecordsCount === 0) {
                    swalOptions.html = validStr;
                    swalOptions.confirmButtonText = 'Proceed';

                    Swal.fire(swalOptions).then((result) => {
                        if (result.isConfirmed) {
                            proceedUpload();
                        }
                    });
                }

                // Case 2: Some valid, some invalid
                else if (response.ValidRecordsCount > 0 && response.InValidRecordsCount > 0) {
                    swalOptions.html = `${validStr}
                                        ${inValidStr}
                                    <p style="color:black;" >You can upload valid records or download invalid records with remarks.</p>`;
                    swalOptions.showDenyButton = true;
                    swalOptions.confirmButtonText = 'Proceed Upload';
                    swalOptions.denyButtonText = 'Download Invalid Records';

                    Swal.fire(swalOptions).then((result) => {
                        if (result.isConfirmed) {
                            proceedUpload();
                        } else if (result.isDenied) {
                            downloadInvalidRecords();
                        }
                    });
                }

                // Case 3: All invalid
                else if (response.ValidRecordsCount === 0 && response.InValidRecordsCount > 0) {
                    swalOptions.html = `${inValidStr}
                                    <p style="color:black;">You can download invalid records with remarks.</p>`;
                    swalOptions.confirmButtonText = 'Download';
                    swalOptions.showCancelButton = true;

                    Swal.fire(swalOptions).then((result) => {
                        if (result.isConfirmed) {
                            downloadInvalidRecords();
                        }
                    });
                }
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

function downloadInvalidRecords() {
    $.ajax({
        url: '/BasicDetail/ICardDistibutionInValidRecordsDownload',
        type: 'GET',
        xhrFields: {
            responseType: 'blob' // Important!
        },
        success: function (data, status, xhr) {
            var blob = new Blob([data], { type: 'text/csv' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "CardDistribution_Invalid.csv";
            link.click();
        },
        error: function () {
            alert('Error downloading file');
        }
    });
}

function proceedUpload() {
    $.ajax({
        url: '/BasicDetail/ICardDistibutionValidRecordsUpload',
        type: 'GET',
        dataType: "json",
        success: function (response) {
            if (response.Result === 1) {
                toastr.success(response.Message);
            }
            else
            {
                toastr.error(response.Message);
            }
        },
        error: function () {
            alert('Error while uploading csv!');
        }
    });
}

function showFileValidationError(message)
{
    toastr.error(message);
}

