var table; // Declare table variable outside the function to preserve the instance

var lstUpdate = new Array();
$(function () {
    $("#btnsave").on("click", function () {
        validateCsvFileOnChange();
    });
    BindData();
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
        const lines = content.split(/\r\n|\n/).filter(line => line.trim() !== "");
        if (lines.length === 0) {
            toastr.error('The selected file is empty.');
            return;
        }

        if (lines.length < 2) {
            toastr.error('The CSV file must contain at least 1 data row.');
            return;
        }

        // Split the first row (headers) and trim each column
        var headers = lines[0].split(",");

        // 4. Validate columns (missing or duplicate columns)
        var expectedColumns = ['ApplId', 'ServiceNo', 'CardSerialNo', 'ChipNo']; // Modify this based on your required columns
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

        const previewData = lines.slice(1, 11);
        let tableHeader = ``;
        headers.forEach(h => tableHeader += `<th class="nowrap" >${h.trim()}</th>`);

        let tablelines = ``;
        previewData.forEach(line => {
            const cells = line.split(",");
            tablelines += '<tr>';
            cells.forEach(cell => tablelines += `<td>${cell.trim()}</td>`);
            tablelines += '</tr>';
        });

        Swal.fire({
            title: 'Preview of Uploaded Data!',
            html: `<p style="margin-bottom: 10px; font-size: 14px; color: #333;">
                  These are the top records from the uploaded CSV file. Please ensure that the correct file has been uploaded.
                </p>
                <div style="overflow-x:auto; class ="table-responsive">
                     <table id="myTable" class="table border border-purple table-striped no-footer dataTable table-hover" role="grid">
                       <thead>
                         <tr>
                           ${tableHeader}
                         </tr>
                       </thead>
                       <tbody>
                            ${tablelines}
                       </tbody>
                   </div>`,
            width: '60%',
            showCancelButton: true,
            confirmButtonText: 'Proceed',
            cancelButtonText: 'Cancel',
            didOpen: () => {
                $('#myTable').DataTable({
                    paging: false,       // disables pagination
                    searching: false,    // disables search box
                    info: false,         // disables "Showing X of Y entries"
                    lengthChange: false  // disables the "Show entries" dropdown
                });
            }
        }).then((result) => {
            if (result.isConfirmed) {
                var formData = new FormData();
                formData.append("CSVFile", file);
                // 3. Send the form data using AJAX
                $.ajax({
                    url: '/BasicDetail/ICardPrintUploadCsv', // Controller action URL
                    method: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (data, status, xhr) {
                        if (data.Result) {
                            
                            let responseHtml = `
                                    <p><strong>Total Records:</strong> ${data.TotalRecords}</p>
                                    <p><strong>Valid Records:</strong> ${data.ValidRecords}</p>
                                    <p><strong>SheetInvalid Records:</strong> ${data.SheetInValidRecords}</p>
                                    <p><strong>DbInvalid Records:</strong> ${data.DbInValidRecords}</p>
                            `

                            // Convert base64 to Blob
                            const byteCharacters = atob(data.File);
                            const byteNumbers = new Array(byteCharacters.length);
                            for (let i = 0; i < byteCharacters.length; i++) {
                                byteNumbers[i] = byteCharacters.charCodeAt(i);
                            }
                            const byteArray = new Uint8Array(byteNumbers);
                            const blob = new Blob([byteArray], { type: 'text/csv' });

                            Swal.fire({
                                title: "Validation Complete!",
                                text: "Please download validated CSV with remarks.",
                                html: responseHtml,
                                icon: "success",
                                showConfirmButton: false, // We'll create custom buttons
                                showCancelButton: false,
                                allowOutsideClick: false,
                                didOpen: () => {
                                    const swal = Swal.getPopup();

                                    const btnGroup = document.createElement('div');
                                    btnGroup.style.display = 'flex';
                                    btnGroup.style.justifyContent = 'center';
                                    btnGroup.style.gap = '10px';

                                    const downloadBtn = document.createElement('button');
                                    downloadBtn.textContent = 'Download';
                                    downloadBtn.className = 'swal2-confirm swal2-styled';
                                    downloadBtn.style.backgroundColor = '#28a745'; // green
                                    downloadBtn.onclick = function () {
                                        window.open(`/CardPrinitngCSVs/CSVWithRemarks/${data.FileName}` , '_blank');
                                    };

                                    const proceedBtn = document.createElement('button');
                                    proceedBtn.textContent = 'Proceed';
                                    proceedBtn.className = 'swal2-confirm swal2-styled';
                                    proceedBtn.style.backgroundColor = '#007bff'; // blue
                                    proceedBtn.onclick = function () {
                                        Swal.close();
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
                                                BindData();
                                            },
                                            error: function (xhr, status, error) {
                                                console.error('Error while uploading valid records:', error);
                                            }
                                        });
                                    };

                                    const cancelBtn = document.createElement('button');
                                    cancelBtn.textContent = 'Cancel';
                                    cancelBtn.className = 'swal2-cancel swal2-styled';
                                    cancelBtn.style.backgroundColor = '#dc3545'; // red
                                    cancelBtn.onclick = function () {
                                        Swal.close();
                                    };

                                    btnGroup.appendChild(downloadBtn);
                                    if (data.ValidRecords > 0) {
                                        btnGroup.appendChild(proceedBtn);
                                    }
                                    
                                    btnGroup.appendChild(cancelBtn);

                                    swal.appendChild(btnGroup);
                                }
                            });
                            $("#CSVFile").val('');
                            BindData();
                        }
                        else
                        {
                            Swal.fire({
                                title: "OOPs!",
                                text: data.Message,
                                icon: "error",
                                confirmButtonText: "Ok"
                            });
                        }
                    },
                    error: function (xhr) {
                        // Show error messages
                    }
                });
            }
        });
    };

    // Trigger the reading of the CSV file
    reader.readAsText(file);
}

function BindData() {
    $("#tblData").DataTable().destroy();
    table = $("#tblData").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[7, 'desc']],// Default sorting on the first column
        searching: false,
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order

            };
            try {
                let response = await fetch("/BasicDetail/GetCSVFileUploadsHistory", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
/*                $("#lblTotal").html(result.recordsTotal);*/
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            // Serial number column
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "FileName", name: "FileName"},
            { data: "TotalRecords", name: "TotalRecords" },
            { data: "ValidRecords", name: "ValidRecords" },
            { data: "DbInvalidRecords", name: "DbInvalidRecords" },
            { data: "SheetInvalidRecords", name: "SheetInvalidRecords" },
            {
                data: "DBUpdated",
                name: "DBUpdated",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            {
                data: "ImportedOn",
                name: "ImportedOn",
                render: function (data, type, row) {
                    return data ? DateFormateddMMyyyyhhmmss(data) : "NA";
                },
            },
            {
                data: null,
                orderable: false,
                render: function (data, type, row, meta) {
                    return `
                    <button class="cls-uploadedCsv btn btn-sm btn-success download-btn" title="Download">
                        <i class="fa fa-download"></i>
                    </button>`;
                }
            },
            {
                data: null,
                orderable: false,
                render: function (data, type, row, meta) {
                    return `
                    <button class="cls-validatedCsv btn btn-sm btn-success download-btn" title="Download">
                        <i class="fa fa-download"></i>
                    </button>`;
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Type / Value" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_Claim',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
            $("#tblData tbody").off("click", ".cls-uploadedCsv").on("click", ".cls-uploadedCsv", function () {
                var rowData = table.row($(this).closest("tr")).data();
                DownloadCSV("CSVWithoutRemarks", rowData.FileName);
            });
            

            $("#tblData tbody").off("click", ".cls-validatedCsv").on("click", ".cls-validatedCsv", function () {
                var rowData = table.row($(this).closest("tr")).data();
                DownloadCSV("CSVWithRemarks", rowData.FileName);
            });
        }
    });
}

function Save() {
    
}

function DownloadCSV(fileLoc, FileName) {
    const baseUrl = window.location.origin;
    const downloadUrl = `${baseUrl}/CardPrinitngCSVs/${fileLoc}/${encodeURIComponent(FileName)}`;
    window.location.href = downloadUrl;
}

function ResetErrorMessage() {
    $("#lblCSVFile").html("");
    $("#CSVFile-error").html("");
}
function Reset() {
    $("#CSVFile").val("");
}