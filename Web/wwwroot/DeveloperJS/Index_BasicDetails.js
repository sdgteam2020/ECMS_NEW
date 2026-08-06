var table_Fwd; // Preserve the shared DataTable instance

function safeAdjustBasicIndexTable(api) {
    if (!api) {
        return;
    }

    api.columns.adjust();

    if (api.responsive && typeof api.responsive.recalc === "function") {
        api.responsive.recalc();
    }
}

function prepareBasicIndexModal(modalElement) {
    if (!modalElement) {
        return;
    }

    if (modalElement.parentElement !== document.body) {
        document.body.appendChild(modalElement);
    }
}

function cleanupBasicIndexModalState() {
    if (document.querySelector(".modal.show")) {
        document.body.classList.add("modal-open");
        return;
    }

    document.querySelectorAll(".modal-backdrop").forEach(function (element) {
        element.remove();
    });

    document.body.classList.remove("modal-open");
    document.body.style.removeProperty("overflow");
    document.body.style.removeProperty("padding-right");
}

function refreshBasicIndexTable(delay) {
    var wait = Number.isFinite(delay) ? delay : 0;

    window.setTimeout(function () {
        try {
            if (
                $("#tbldatatabledata_Fwd").length &&
                $.fn.DataTable.isDataTable("#tbldatatabledata_Fwd")
            ) {
                safeAdjustBasicIndexTable(
                    $("#tbldatatabledata_Fwd").DataTable()
                );
            }
        } catch (error) {
            console.warn("Basic Detail table adjustment skipped:", error);
        }
    }, wait);
}

$(function () {
    sessionStorage.clear();
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    let StepCounter = parseInt($("#spnStepCounter").html());
    let JCOOR = $("#spnJCOOR").html();

    applyDataTableSearchValidation('#tbldatatabledata_Fwd');

    document.body.classList.add("ecms-basic-index-page-active");

    BindData(StepCounter, JCOOR, function () {
    });

    $(document)
        .off("show.bs.modal.basicIndexPage", ".modal")
        .on("show.bs.modal.basicIndexPage", ".modal", function () {
            prepareBasicIndexModal(this);
        })
        .off("shown.bs.modal.basicIndexPage", ".modal")
        .on("shown.bs.modal.basicIndexPage", ".modal", function () {
            prepareBasicIndexModal(this);
            document.body.classList.add("modal-open");
        })
        .off("hidden.bs.modal.basicIndexPage", ".modal")
        .on("hidden.bs.modal.basicIndexPage", ".modal", function () {
            cleanupBasicIndexModalState();
        });

    $(window)
        .off("resize.basicIndexTable")
        .on("resize.basicIndexTable", function () {
            window.clearTimeout(window.__basicIndexResizeTimer);
            window.__basicIndexResizeTimer = window.setTimeout(function () {
                refreshBasicIndexTable(0);
            }, 120);
        });
});
function BindData(StepCounter, JCOOR) {
    if ($.fn.DataTable.isDataTable("#tbldatatabledata_Fwd")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_Fwd").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_Fwd thead").empty(); // Clear old thead
        $("#tbldatatabledata_Fwd tbody").empty(); // Clear old tbody
        $("#tbldatatabledata_Fwd").empty(); // Remove generated sizing markup
    }

    table_Fwd = $("#tbldatatabledata_Fwd").DataTable({
        scrollY: '100%',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: false,
        scroller: false,           // ✅ Enable virtual scrolling for better performance
        deferScroll: false,        // ✅ Improve scrolling performance
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: false,
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
                TypeId: 0,
                applyForId: 0,
                JCOOR: JCOOR
            };
            let encryptedPayload = "";
            if (requestData) {
                const jsonData = JSON.stringify(requestData);
                encryptedPayload = encryptPayloadData(jsonData);

            }
            try {
                let response = await fetch("/BasicDetail/GetAllIndexData", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify({ data: encryptedPayload })

                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                callback(result); // Sends data to DataTables
                refreshBasicIndexTable(30);
                refreshBasicIndexTable(150);

            } catch (error) {
                console.error("Error fetching data:", error);
                callback({
                    draw: data.draw,
                    recordsTotal: 0,
                    recordsFiltered: 0,
                    data: []
                });
                refreshBasicIndexTable(30);
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
                    if (data == 1) {
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
                    return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
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
                    return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                }
            },
            // Additional column for Edit action
            {
                title: "Preview | Edit | Fwd",
                data: null,
                name: "Action",
                orderable: false,
                className: "noExport nowrap",
                width: "210px",
                render: function (data, type, row) {
                    // Always include the Print Preview button
                    let html = `<button class="btn btn-icon btn-round btn-primary mr-2 cls-ICardPrintPreviewByRequestId"><i class="fa fa-eye mt-2"></i></button>`;

                    // Case 1: Editable + Forward
                    if ((row.StepCounter == 1 || row.StepCounter == 7 || row.StepCounter == 8 || row.StepCounter == 9 || row.StepCounter == 10) && row.IsLock == false) {
                        html += `<a href="/BasicDetail/BasicDetail?Id=${row.EncryptedId}" class="btn btn-icon btn-round btn-warning mr-2"><i class="fas fa-edit mt-2"></i></a>
                                <button class="btn btn-icon btn-round btn-primary mr-1 cls-fwdrecord"><i class="fa fa-step-forward"></i></button>`;
                    }
                    // Case 2: Processed + Download
                    else if (row.StepCounter >= 2 && row.StepCounter <= 15)
                    {
                        if (row.StatusId == 1) {
                            html += `<span class="badge rounded-pill bg-light text-primary mt-3">Processed</span>`;
                        }
                        else if (row.StatusId == 2) {
                            html += `<span class="badge rounded-pill bg-light text-success mt-3">Completed</span>`;
                        }
                        else if (row.StatusId == 3) {
                            html += `<span class="badge rounded-pill bg-light text-danger mt-3">Closed</span>`;
                        }
                        html += `<button class="cls-btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details">
                                <img src="/Images/digitalsign.png" width="40" />
                                </button>`;
                    }
                    // Case 3: Rejected only
                    if (row.IsFwdStatusId == 3) {
                        html += `<span class="badge rounded-pill bg-light text-danger mt-3" data-toggle="tooltip" data-placement="left">Rejected</span>`;
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
        dom: "<'row g-2 align-items-center ecms-dt-toolbar'<'col-12 col-md-4 d-flex justify-content-start dt-length-col'l><'col-12 col-md-4 d-flex justify-content-center dt-buttons-col'B><'col-12 col-md-4 d-flex justify-content-md-end dt-filter-col'f>>" +
            "rt" +
            "<'row g-2 align-items-center ecms-dt-footer'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 d-flex justify-content-md-end dt-page-col'p>>",
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
        initComplete: function () {
            let searchBox = $("#tbldatatabledata_Fwd_wrapper div.dataTables_filter input");
            searchBox.attr("title", "Search Army No or Application ID");

            safeAdjustBasicIndexTable(this.api());
            refreshBasicIndexTable(20);
            refreshBasicIndexTable(150);
        },
        drawCallback: function (settings) {
            safeAdjustBasicIndexTable(this.api());
            refreshBasicIndexTable(20);
            refreshBasicIndexTable(120);

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"], [data-toggle="tooltip"]')
            );

            if (window.bootstrap && bootstrap.Tooltip) {
                tooltipTriggerList.forEach(function (element) {
                    try {
                        if (bootstrap.Tooltip.getOrCreateInstance) {
                            bootstrap.Tooltip.getOrCreateInstance(element);
                        } else {
                            new bootstrap.Tooltip(element);
                        }
                    } catch (error) {
                        console.warn("Basic Detail tooltip skipped:", error);
                    }
                });
            }

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

// UI-only fix: DataTables with scrollX/scrollY keeps a hidden THEAD inside scrollBody.
// Some project CSS makes that hidden THEAD visible as a blank blue row, so hide it after DataTables draws.
(function () {
    function fixBasicDetailDataTableHeader() {
        var $wrapper = $('#tbldatatabledata_Fwd_wrapper');
        if (!$wrapper.length) return;

        var $bodyHead = $wrapper.find('div.dataTables_scrollBody table.dataTable thead');
        $bodyHead.css({
            'visibility': 'hidden',
            'height': '0px',
            'max-height': '0px',
            'line-height': '0px',
            'overflow': 'hidden'
        });

        $bodyHead.find('tr, th, td').css({
            'height': '0px',
            'max-height': '0px',
            'min-height': '0px',
            'line-height': '0px',
            'padding-top': '0px',
            'padding-bottom': '0px',
            'border-top': '0px',
            'border-bottom': '0px',
            'font-size': '0px',
            'color': 'transparent',
            'background': 'transparent'
        });

        $bodyHead.find('.dataTables_sizing, div, span').css({
            'height': '0px',
            'max-height': '0px',
            'line-height': '0px',
            'overflow': 'hidden',
            'font-size': '0px',
            'padding': '0px',
            'margin': '0px'
        });
    }

    $(document).on('init.dt draw.dt column-sizing.dt', '#tbldatatabledata_Fwd', function () {
        setTimeout(fixBasicDetailDataTableHeader, 0);
        setTimeout(fixBasicDetailDataTableHeader, 100);
    });

    $(window).on('load resize', function () {
        setTimeout(fixBasicDetailDataTableHeader, 150);
    });
})();