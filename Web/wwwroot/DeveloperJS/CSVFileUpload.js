var lstUpdate = new Array();
$(function () {
    $("#btnsave").on("click", function () {
        validateCsvFileOnChange();
    });
});

function validateCsvFileOnChange() {
    // Get the file input element and the file selected by the user
    var fileInput = $('#CSVFile')[0];
    var file = fileInput.files[0];

    if (!file) {
        toastr.error('Please select a CSV file.');
        return;
    }

    // 2. Check if the file is a CSV
    var fileType = file.name.split('.').pop().toLowerCase();
    if (fileType !== 'csv') {
        toastr.error('Only CSV files are allowed.');
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
            toastr.error('The CSV file must contain at least 1 data row.');
            return;
        }
        // Split the first row (headers) and trim each column
        var headers = rows[0].split(',').map(header => header.trim());

        // 4. Validate columns (missing or duplicate columns)
        var expectedColumns = ['RequestId', 'ArmyNo', 'CardSerialNo', 'ChipNo']; // Modify this based on your required columns
        var missingColumns = expectedColumns.filter(col => !headers.includes(col));
        var duplicateColumns = headers.filter((value, index, self) => self.indexOf(value) !== index);

        if (missingColumns.length > 0) {
            toastr.error('Missing columns: ' + missingColumns.join(', '));
            return;
        }

        if (duplicateColumns.length > 0) {
            toastr.error('Duplicate columns found: ' + duplicateColumns.join(', '));
            return;
        }


        var formData = new FormData();
        formData.append("CSVFile", file);
        // 3. Send the form data using AJAX
        $.ajax({
            url: '/BasicDetail/ICardPrintUploadCsv', // Controller action URL
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            xhrFields: {
                responseType: 'blob' // Important!
            },
            success: function (data, status, xhr) {
                if (data == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                    return;
                }

                if (data == BadRequest) {
                    Swal.fire({
                        text: baderrormsg
                    });
                    return;
                }
                var disposition = xhr.getResponseHeader('Content-Disposition');
                var fileName = "Import.csv"; // default fallback

                if (disposition) {
                    var fileNameRegex = /filename\*?=[^']*''([^;]+)|filename="?([^;"]+)"?/i;
                    var matches = fileNameRegex.exec(disposition);
                    if (matches) {
                        fileName = matches[1] || matches[2];
                    }
                }

                Swal.fire({
                    title: "Validation Complete!",
                    text: "Please download validated CSV with remarks.",
                    icon: "success",
                    showCancelButton: true,
                    confirmButtonText: "Download"
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Download file
                        var url = window.URL.createObjectURL(data);
                        var a = document.createElement('a');
                        a.href = url;
                        a.download = fileName;
                        document.body.appendChild(a);
                        a.click();
                        a.remove();

                        // Show next SweetAlert to proceed
                        Swal.fire({
                            title: "Save Valid Records",
                            text: "Do you wants to save valid records?",
                            icon: "question",
                            showCancelButton: true,
                            cancelButtonText: "No",
                            confirmButtonText: "Yes"
                        }).then(async (proceed) => {
                            if (proceed.isConfirmed) {
                                $.ajax({
                                    url: '/BasicDetail/ICardPrintValidRecordsUpload',
                                    type: 'GET',
                                    dataType: 'json',
                                    success: function (data) {
                                        if (data.Result) {
                                            Swal.fire({
                                                title: "Success!",
                                                text: data.Message,
                                                icon: "success",
                                                confirmButtonText: "OK"
                                            });
                                        }
                                        else {
                                            Swal.fire({
                                                title: "OOPs!",
                                                text: data.Message,
                                                icon: "error",
                                                confirmButtonText: "Ok"
                                            });
                                        }
                                    },
                                    error: function (xhr, status, error) {
                                        console.error('Error while uploading valid records:', error);
                                    }
                                });
                            }
                        });
                    }
                });

            },
            error: function (xhr) {
                // Show error messages
                console.log(xhr);
            }
        });

    };

    // Trigger the reading of the CSV file
    reader.readAsText(file);
}

function Save() {
    
}

function ResetErrorMessage() {
    $("#lblCSVFile").html("");
    $("#CSVFile-error").html("");
}
function Reset() {
    $("#CSVFile").val("");
}