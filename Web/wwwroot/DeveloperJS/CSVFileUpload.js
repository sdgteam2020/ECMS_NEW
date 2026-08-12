var table; // Declare table variable outside the function to preserve the instance

var lstUpdate = new Array();
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    $("#btnsave").on("click", function () {
        validateCsvFileOnChange();
    });

    applyDataTableSearchValidation('#tbldata');

    BindData();
});

function validateCsvFileOnChange() {
    var fileInput = $('#CSVFile')[0];
    var file = fileInput.files[0];

    if (!file) {
        toastr.error('Please select a CSV file.');
        return;
    }

    var fileType = file.name.split('.').pop().toLowerCase();
    if (fileType !== 'csv') {
        toastr.error('Only CSV files are allowed.');
        return;
    }

    function escapeHtml(text) {
        return String(text ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

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

        var headers = lines[0].split(",").map(x => x.trim());

        var expectedColumns = ['ApplId', 'ServiceNo', 'CardSerialNo', 'ChipNo'];
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

        let tableHeader = '';
        headers.forEach(h => {
            tableHeader += `<th>${escapeHtml(h)}</th>`;
        });

        let tablelines = '';
        previewData.forEach(line => {
            const cells = line.split(",");
            tablelines += '<tr>';

            for (let i = 0; i < headers.length; i++) {
                const cellValue = cells[i] !== undefined ? cells[i].trim() : '';
                tablelines += `<td>${escapeHtml(cellValue)}</td>`;
            }

            tablelines += '</tr>';
        });

        Swal.fire({
            title: 'Preview of Uploaded Data!',
            html: `
                <p class="csv-preview-intro">
                    These are the top records from the uploaded CSV file. Please ensure that the correct file has been uploaded.
                </p>
                <div class="csv-preview-wrapper">
                    <table id="myTable" class="csv-preview-table">
                        <thead>
                            <tr>${tableHeader}</tr>
                        </thead>
                        <tbody>
                            ${tablelines}
                        </tbody>
                    </table>
                </div>
            `,
            customClass: {
                popup: 'csv-preview-popup',
                title: 'csv-preview-title'
            },
            showCancelButton: true,
            confirmButtonText: 'Proceed',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                var formData = new FormData();
                formData.append("CSVFile", file);

                $.ajax({
                    url: '/BasicDetail/ICardPrintUploadCsv',
                    method: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data, status, xhr) {
                        if (data.Result) {

                            let responseHtml = `
                                <p><strong>Total Records:</strong> ${data.TotalRecords}</p>
                                <p><strong>Valid Records:</strong> ${data.ValidRecords}</p>
                                <p><strong>SheetInvalid Records:</strong> ${data.SheetInValidRecords}</p>
                                <p><strong>DbInvalid Records:</strong> ${data.DbInValidRecords}</p>
                            `;

                            Swal.fire({
                                title: "Validation Complete!",
                                html: responseHtml,
                                icon: "success",
                                showConfirmButton: false,
                                showCancelButton: false,
                                allowOutsideClick: false,
                                didOpen: () => {
                                    const swal = Swal.getPopup();

                                    const btnGroup = document.createElement('div');
                                    btnGroup.className = 'csv-btn-group';

                                    const downloadBtn = document.createElement('button');
                                    downloadBtn.textContent = 'Download';
                                    downloadBtn.className = 'swal2-confirm swal2-styled csv-btn-download';
                                    downloadBtn.onclick = function () {
                                        window.open(`/CardPrinitngCSVs/CSVWithRemarks/${data.FileName}`, '_blank');
                                    };

                                    const proceedBtn = document.createElement('button');
                                    proceedBtn.textContent = 'Proceed';
                                    proceedBtn.className = 'swal2-confirm swal2-styled csv-btn-proceed';
                                    proceedBtn.onclick = function () {
                                        Swal.close();

                                        $.ajax({
                                            url: '/BasicDetail/ICardPrintValidRecordsUpload',
                                            type: 'GET',
                                            dataType: 'json',
                                            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                                            success: function (data) {
                                                if (data.Result) {
                                                    Swal.fire({
                                                        title: "Success!",
                                                        text: data.Message,
                                                        icon: "success",
                                                        confirmButtonText: "OK"
                                                    });
                                                } else {
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
                                    cancelBtn.className = 'swal2-cancel swal2-styled csv-btn-cancel';
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
                        } else {
                            Swal.fire({
                                title: "OOPs!",
                                text: data.Message,
                                icon: "error",
                                confirmButtonText: "Ok"
                            });
                        }
                    },
                    error: function (xhr) {
                        Swal.fire({
                            title: "OOPs!",
                            text: "Error while uploading CSV file.",
                            icon: "error",
                            confirmButtonText: "Ok"
                        });
                    }
                });
            }
        });
    };

    reader.readAsText(file);
}

function BindData() {
    if ($.fn.DataTable.isDataTable("#tblData")) {
        // Destroy the DataTable and clear the table content
        $("#tblData").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tblData thead").empty(); // Clear old thead
        $("#tblData tbody").empty(); // Clear old tbody
    }

    table = $("#tblData").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        scroller: true,           // ✅ Enable virtual scrolling for better performance
        deferScroll: true,        // ✅ Improve scrolling performance
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: true,  //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
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
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
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
                title: "S No",
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                className: "text-center",
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                title: "File Name",
                data: "FileName",
                name: "FileName",
                className: "nowrap",
            },
            {
                title: "Total Records",
                data: "TotalRecords",
                name: "TotalRecords",
                className: "nowrap text-center",
            },
            {
                title: "Valid Records",
                data: "ValidRecords",
                name: "ValidRecords",
                className: "nowrap text-center",
            },
            {
                title: "DB Invalid",
                data: "DbInvalidRecords",
                name: "DbInvalidRecords",
                className: "nowrap text-center",
            },
            {
                title: "Sheet Invalid",
                data: "SheetInvalidRecords",
                name: "SheetInvalidRecords",
                className: "nowrap text-center",
            },
            {
                title: "Inject Status",
                data: "DBUpdated",
                name: "DBUpdated",
                className: "nowrap text-center",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            {
                title: "Uploaded On",
                data: "ImportedOn",
                name: "ImportedOn",
                className: "nowrap text-center",
                render: function (data, type, row) {
                    return data ? DateFormateddMMyyyyhhmmss(data) : "NA";
                },
            }
            //,
            //{
            //    title: "Uploaded CSV",
            //    data: null,
            //    orderable: false,
            //    className: "nowrap text-center",
            //    render: function (data, type, row, meta) {
            //        return `
            //        <button class="cls-uploadedCsv btn btn-sm btn-success download-btn" title="Download">
            //            <i class="fa fa-download"></i>
            //        </button>`;
            //    }
            //},
            //{
            //    title: "Validated CSV",
            //    data: null,
            //    orderable: false,
            //    className: "nowrap text-center",
            //    render: function (data, type, row, meta) {
            //        return `
            //        <button class="cls-validatedCsv btn btn-sm btn-success download-btn" title="Download">
            //            <i class="fa fa-download"></i>
            //        </button>`;
            //    }
            //}
        ],
        columnDefs: [
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Type / Value" // Add custom placeholder
        },
        dom: "<'dt-top ecms-dt-toolbar d-flex justify-content-between align-items-center flex-wrap'lBf>rt<'ecms-dt-footer row no-gutters'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>", // Shared ModernCSS DataTable toolbar/footer
        buttons: [
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: "thead th:not(.noExport)"
            //    }
            //},
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
        // ✅ ADD: initComplete for zoom handling
        initComplete: function () {
            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table.columns.adjust().responsive.recalc();
                }, 100);
            });
        },
        drawCallback: function (settings) {
            // Recalculate widths on each data load
            this.api().columns.adjust().responsive.recalc();

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