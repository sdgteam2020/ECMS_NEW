var table;
let dataExportType = 1;

$(function () {
    document.documentElement.classList.add('ecms-appclose-scroll-lock');
    document.body.classList.add('ecms-lock-page-scroll', 'ecms-appclose-scroll-lock');

    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    applyDataTableSearchValidation('#tbldata');

    let applyFor = $('#spnapplyFor').text();
    BindData(applyFor, function () {
    });

    $(window)
        .off('pagehide.ecmsAppClose')
        .on('pagehide.ecmsAppClose', function () {
            document.documentElement.classList.remove('ecms-appclose-scroll-lock');
            document.body.classList.remove('ecms-lock-page-scroll', 'ecms-appclose-scroll-lock');
            $(window).off('resize.ecmsAppClose');
        });
});

function BindData(applyFor) {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
    }

    $(window).off('resize.ecmsAppClose');

    table = $("#tbldata").DataTable({
        // Keep scrolling inside the table so the page and footer stay fixed.
        scrollY: 'calc(100vh - 485px)',
        scrollX: true,
        scrollCollapse: false,
        fixedHeader: false,

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false,
        responsive: false,
        deferRender: true,
        order: [[1, 'desc']], // Latest updated application first
        ajax: async function (data, callback, settings) {

            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                apply: applyFor
            };
            let encryptedPayload = "";
            if (requestData) {
                const jsonData = JSON.stringify(requestData);
                encryptedPayload = encryptPayloadData(jsonData);

            }
            try {
                let response = await fetch("/Posting/GetAllAppCloseList", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify({ data: encryptedPayload })
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.Message}`);

                let result = await response.json();

                if (result.Result == false) {
                    toastr.error("Failed to Fetch Date: " + response.Message);
                }

                callback(result); // Sends data to DataTables

            } catch (error) {
                Swal.fire({
                    icon: "error",
                    title: "Data Fetch Failed",
                    text: "We couldn’t load the data right now. Please try again later.",
                    confirmButtonText: "OK"
                });
            }
        },
        columns: [
            {
                title: "S No",
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                className: "text-center col-sno",
                width: "60px",
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                title: "Updated On",
                data: "UpdatedOn",
                name: "UpdatedOn",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    return data ? DateFormateddMMyyyyhhmmss(data) : "";
                }
            },
            //{ data: "RequestId", name: "RequestId" },
            {
                title: "Army No",
                data: "ServiceNo",
                name: "ServiceNo",
                className: "nowrap",
                width: "120px",
                render: function (data, type, row) {
                    const serviceNo = data || "";

                    // Check if first two characters are alphabets
                    if (/^[A-Za-z]{2}/.test(serviceNo)) {
                        // Insert space after first two characters
                        return serviceNo.slice(0, 2) + ' ' + serviceNo.slice(2);
                    } else {
                        // No space needed
                        return serviceNo;
                    }
                }
            },
            {
                title: "Rk & Name",
                data: null,
                name: null,
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    if (!fullName) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                }
            },
            {
                title: "Reason",
                data: "Reason",
                name: "Reason",
                className: "nowrap",
                width: "150px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Remark",
                data: "Remarks",
                name: "Remarks",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Auth",
                data: "Authority",
                name: "Authority",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            }
        ],
        columnDefs: [
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Auth",
            emptyTable: "No closed application records found"
        },
        dom:
            "<'dt-top d-flex flex-column flex-md-row align-items-stretch align-items-md-center gap-2'lB<'ms-md-auto'f>>rt" +
            "<'ecms-dt-footer row g-2'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>",
        buttons: [
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: "thead th:not(.noExport)"
            //    }
            //},
            {
                extend: 'excel',
                text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Excel',
                className: 'btn btn-success btn-sm',
                titleAttr: 'Export closed applications to Excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="fa fa-file-pdf-o" aria-hidden="true"></i> PDF',
                className: 'btn btn-danger btn-sm',
                titleAttr: 'Export closed applications to PDF',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_AppClosed',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        // 👇 Show modal only after table (header + data) is fully rendered
        initComplete: function () {
            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize.ecmsAppClose', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    if (table) {
                        table.columns.adjust();
                    }
                }, 100);
            });
        },
        drawCallback: function (settings) {
            // Recalculate widths on each data load
            this.api().columns.adjust();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });
        }
    });
}