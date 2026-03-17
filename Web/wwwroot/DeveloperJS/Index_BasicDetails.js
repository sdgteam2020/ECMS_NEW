var table_Fwd; // Declare table variable outside the function to preserve the instance
$(function () {
    sessionStorage.clear();
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    let Type = parseInt($("#spnType").html());
    let StepCounter = parseInt($("#spnStepCounter").html());
    let JCOOR = $("#spnJCOOR").html();
    let VBId = $("#spnVBId").html();
    BindData(Type, StepCounter, JCOOR, VBId, function () {
    });

    $(window).on('resize', function () {
        // Check if element exists AND is a DataTable
        if ($('#tbldatatabledata_Fwd').length && $.fn.DataTable.isDataTable('#tbldatatabledata_Fwd')) {
            $('#tbldatatabledata_Fwd').DataTable().columns.adjust();
        }
    });
});
function BindData(Type, StepCounter, JCOOR, VBId) {
    if ($.fn.DataTable.isDataTable("#tbldatatabledata_Fwd")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_Fwd").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_Fwd thead").empty(); // Clear old thead
        $("#tbldatatabledata_Fwd tbody").empty(); // Clear old tbody
    }
 
    table_Fwd = $("#tbldatatabledata_Fwd").DataTable({
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

        autoWidth: false, //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                UserId: 0,
                stepcount: StepCounter,
                TypeId: Type,
                applyForId: 0,
                JCOOR: JCOOR
            };
            try {
                let response = await fetch("/BasicDetail/GetAllIndexData", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify(requestData)
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
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
                className: "text-center col-sno",
                width: "60px",
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                title: "Appl ID",
                data: "ApplId",
                name: "ApplId",
                className: "nowrap",
                width: "100px",
            },
            {
                title: "ServiceNo",
                data: "ServiceNo",
                name: "ServiceNo",
                className: "nowrap",
                width: "120px",
                render: function (data, type, row) {
                    let displayText = data;

                    // If first two chars are alphabets, insert space
                    if (/^[A-Za-z]{2}/.test(data)) {
                        displayText = data.slice(0, 2) + ' ' + data.slice(2);
                    }

                    return `<a href="#" class="service-no-link">${displayText}</a>`;
                }
            },
            {
                title: "Rank & Name",
                data: null,
                name: "Name",
                className: "nowrap",
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    if (!fullName) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                }
            },
            {
                title: "Unit",
                data: "UnitName",
                name: "UnitName",
                className: "nowrap",
                width: "150px",
                orderable: false,
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Type",
                data: "ApplyFor",
                name: "ApplyFor",
                className: "nowrap",
                width: "100px",
            },
            {
                title: "Reason for Requisition",
                data: "ICardType",
                name: "ICardType",
                className: "nowrap",
                width: "180px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Posting",
                data: "IsPosting",
                name: "IsPosting",
                className: "noExport nowrap",
                width: "100px",
                render: function (data, type, row) {
                    if (data > 0) {
                        return ` <span class="badge badge-pill badge-danger">Yes</span> `;
                    }
                    else {
                        return `<span class="badge badge-pill badge-success">No</span>`;
                    }
                }
            },
            {
                title: "Application History",
                data: null,
                name: "Application History",
                className: "noExport",
                width: "100px",
                orderable: false,
                render: function (data, type, row) {
                    return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                }
            },
            {
                title: "Card History",
                data: null,
                name: "Card History",
                orderable: false,
                className: "noExport",
                width: "100px",
                render: function (data, type, row) {
                    return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                }
            },
            // Additional column for Edit action
            {
                title: "Print | Edit | Fwd",
                data: null,
                name: "Action",
                orderable: false,
                className: "noExport nowrap",
                width: "210px",
                render: function (data, type, row) {
                    // Always include the Print Preview button
                    let html = `<button class="btn btn-icon btn-round btn-primary mr-2 cls-ICardPrintPreviewByRequestId"><i class="fa fa-print mt-2"></i></button>`;

                    // Case 1: Editable + Forward
                    if ((row.StepCounter == 1 || row.StepCounter == 7 || row.StepCounter == 8 || row.StepCounter == 9 || row.StepCounter == 10) && (VBId == 0 || VBId == 1 || VBId == 11 || row.IsFwdStatusId == 3))
                    {
                        html += `<a href="/BasicDetail/BasicDetail?Id=${row.EncryptedId}" class="btn btn-icon btn-round btn-warning mr-2"><i class="fas fa-edit mt-2"></i></a>
                                <button class="btn btn-icon btn-round btn-primary mr-1 cls-fwdrecord"><i class="fa fa-step-forward"></i></button>`;
                    }
                    // Case 2: Processed + Download
                    else if (row.StepCounter >= 2 && row.StepCounter <= 6)
                    {
                        html += `<span class="badge rounded-pill bg-light text-primary mt-3">Processed</span>
                                <button class="cls-btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details">
                                <img src="/Images/digitalsign.png" width="40" />
                                </button>`;
                    }
                    else
                    {
                        // Case 3: Rejected only
                        if (row.IsFwdStatusId == 3) {
                            html += `<span class="badge rounded-pill bg-light text-danger mt-3" data-toggle="tooltip" data-placement="left" title="${row.Remark}">Rejected</span>`;
                        }
                    }
                    return html; // Return the full HTML string
                }
            }
        ],
        columnDefs: [
            { targets: 0, width: "60px", },
            { targets: 1, width: "100px" },
            { targets: 2, width: "120px" },
            { targets: 3, width: "180px" },
            { targets: 4, width: "150px" },
            { targets: 5, width: "100px" },
            { targets: 6, width: "180px" },
            { targets: 7, width: "100px" },
            { targets: 8, width: "100px" },
            { targets: 9, width: "100px" },
            { targets: 10, width: "210px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
        buttons: [
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible:not(.noExport)'
            //    }
            //},
            {
                extend: 'excel',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'portrait',
                pageSize: 'A4', //A3 , A5 , A6 , legal , letter
                title: 'E-IASC_Appl',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
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
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table_Fwd.columns.adjust().responsive.recalc();
                }, 100);
            });
        },
        drawCallback: function (settings) {

            // Recalculate widths on each data load
            this.api().columns.adjust().responsive.recalc();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });

            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetRequestHistory(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-cardhistoryRequest").on("click", ".cls-cardhistoryRequest", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetMovementHistory(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-btndownloadpdf").on("click", ".cls-btndownloadpdf", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    DownloadPdf(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".service-no-link").on("click", ".service-no-link", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetBasicDetailByRequestId(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-ICardPrintPreviewByRequestId").on("click", ".cls-ICardPrintPreviewByRequestId", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetICardPrintPreviewByRequestId(rowData.RequestId);
                }
            });
        }
    });
}