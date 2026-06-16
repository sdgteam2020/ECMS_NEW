var table_Fwd; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldatatabledata_Fwd');

    let Type = parseInt($("#spnType").html());
    let StepCounter = parseInt($("#spnStepCounter").html());
    let JCOOR = $("#spnJCOOR").html();
    let cvalue = parseInt($("#spnClaim").html());
    BindData(Type, StepCounter, JCOOR, cvalue, function () {
        // Reset global variables as explained
        globalThis.selectedIds = [];
        globalThis.previousSearchText = "";
        globalThis.isFirstSelectAll = true;
        globalThis.searchChanged = false;
        globalThis.globalAllChecked = false;
    });
    $(window).on('resize', function () {
        // Check if element exists AND is a DataTable
        if ($('#tbldatatabledata_Fwd').length && $.fn.DataTable.isDataTable('#tbldatatabledata_Fwd')) {
            $('#tbldatatabledata_Fwd').DataTable().columns.adjust();
        }
    });
});
function BindData(Type, StepCounter, JCOOR, cvalue) {
    globalThis.selectedIds = [];

    if ($.fn.DataTable.isDataTable("#tbldatatabledata_Fwd")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_Fwd").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_Fwd thead").empty(); // Clear old thead
        $("#tbldatatabledata_Fwd tbody").empty(); // Clear old tbody
    }


    const columns = getColumnsForApprovalForIO(cvalue, JCOOR, Type);
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
        order: [[2, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let searchStatus = getSearchStatusForBindData(data.search.value);

            // Clear old selectedIds on search change, but keep globalAllChecked state
            if (searchStatus.searchChanged) {
                globalThis.selectedIds = [];

                // Mark for re-fetch if needed
                if (globalThis.globalAllChecked) {
                    globalThis.isFirstSelectAll = true;
                }
            }

            // ✅ Determine if a fetch is needed
            const shouldFetchSelectedIds =
                globalThis.globalAllChecked && (globalThis.isFirstSelectAll || searchStatus.searchChanged) ||
                (!globalThis.globalAllChecked && searchStatus.searchChanged && globalThis.isFirstSelectAll);

            // If fetch is needed, manually set searchChanged to true
            if (shouldFetchSelectedIds) {
                searchStatus.searchChanged = true; // Manually set to true to ensure data fetch
            }


            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: searchStatus.currentSearchText,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                UserId: 0,
                stepcount: StepCounter,
                TypeId: Type,
                applyForId: 0,
                JCOOR: JCOOR,
                AllChecked: shouldFetchSelectedIds ? true : globalThis.globalAllChecked,
                searchTextChanged: searchStatus.searchChanged
            };
            let encryptedPayload = "";
            if (requestData) {
                const jsonData = JSON.stringify(requestData);
                encryptedPayload = encryptPayloadData(jsonData);

            }
            try {
                let response = await fetch("/BasicDetail/GetAllApprovalForIOData", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify({ data: encryptedPayload })
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    globalThis.selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }

                // Only update selectedIds if server returns new ones
                if (shouldFetchSelectedIds) {
                    if (result.selectedIds != null && result.selectedIds.length > 0) {
                        //selectedIds = result.selectedIds;
                        globalThis.selectedIds = result.selectedIds.map(x => x.toString());
                        console.log("Fetched selectedIds from server:", globalThis.selectedIds);
                        // If user hadn’t checked Select All, now we just load into selectedIds silently
                        if (globalThis.globalAllChecked) globalThis.isFirstSelectAll = false;
                    }
                    else {
                        //selectedIds = [];
                        if (globalThis.globalAllChecked) {
                            globalThis.globalAllChecked = false;
                            $('#chkAll_ApprovalForIO').prop('checked', false);
                        }
                        console.warn("⚠️ No valid Pending IDs found.");
                    }
                }


                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        columnDefs: [
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]  // Only global settings
            }
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
                title: 'E-IASC_Approved I-Card',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        // 👇 Show modal only after table (header + data) is fully rendered
        initComplete: function () {
            if (typeof callback === "function") {
                callback(); // show modal now
            }
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

            updateUICheckboxes('#tbldatatabledata_Fwd', 'chkRequestId', '#chkAll_ApprovalForIO');

            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetRequestHistory(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-btndownloadpdf").on("click", ".cls-btndownloadpdf", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    DownloadPdf(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-btndownloadxml").on("click", ".cls-btndownloadxml", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    DownloadXml(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-BasicDetail").on("click", ".cls-BasicDetail", function (event) {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData && rowData.RequestId) {
                    GetBasicDetailByRequestId(rowData.RequestId);
                    event.preventDefault();
                } else {
                    console.error("RequestId not found in row data");
                }
                event.preventDefault(); // Prevent default action
            });
            $("#tbldatatabledata_Fwd tbody").off("click", ".cls-BasicDetail-PrintPreview").on("click", ".cls-BasicDetail-PrintPreview", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData && rowData.RequestId) {
                    GetICardPrintPreviewByRequestId(rowData.RequestId);
                }
                else {
                    console.error("RequestId not found in row data");
                }
            });
        }
    });
    $(document).on('change', '.chkRequestId', async function () {
        await updateSelectedIds('#tbldatatabledata_Fwd', 'chkRequestId');
        updateUICheckboxes('#tbldatatabledata_Fwd', 'chkRequestId', '#chkAll_ApprovalForIO'); // Sync master checkbox state
    });
    $('#chkAll_ApprovalForIO').on('change', function () {
        globalThis.selectedIds = [];
        globalThis.globalAllChecked = $(this).prop('checked');
        if (globalThis.globalAllChecked) {
            globalThis.isFirstSelectAll = true; // Force fresh fetch
        }
        table_Fwd.ajax.reload();
    });
}

function getColumnsForApprovalForIO(cvalue, JCOOR, Type) {
    let columns = [];
    switch (JCOOR) {
        case "0":
            columns = [
                {
                    title: `<div class="wd-30-f"><div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll_ApprovalForIO">
                    <label class="custom-control-label" for="chkAll_ApprovalForIO"></label>
                    </div></div>`,
                    className: "noExport",
                    width: "40px",
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll_ApprovalForIO").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        }
                    }
                },
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
                    title: "Appl Id",
                    data: "RequestId",
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
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return `<a href="#" class="cls-BasicDetail">${data.slice(0, 2) + ' ' + data.slice(2)}</a>`;

                        } else {
                            // No space needed
                            return `<a href="#" class="cls-BasicDetail">${data}</a>`;;
                        }
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
                    title: "Regtl Centre",
                    data: "RegimentalName",
                    name: "RegimentalName",
                    className: "nowrap",
                    width: "150px",
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
                },
                {
                    title: `<div>History</div>`,
                    data: null,
                    name: "History",
                    className: "noExport",
                    width: "100px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                    }
                },
                // Additional column for Edit action
                {
                    title: `<div>Preview | Fwd</div>`,
                    data: null,
                    name: "Action",
                    className: "noExport",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, row) {
                        // Always include the Print Preview button
                        let html = `<button class="btn btn-icon btn-round btn-primary mr-2 cls-BasicDetail-PrintPreview" onclick="GetICardPrintPreviewByRequestId(${row.RequestId})"><i class="fa fa-eye mt-2"></i></button>`;

                        if (Type == 1 && (row.StepCounter == 2 || row.StepCounter == 3)) {
                            html += `<button class="btn btn-primary mr-1 cls-fwdrecord">Verify And Send</button>`;
                        }
                        // Case 2: Processed + Download
                        else if (row.StepCounter != 1 && row.StepCounter != 7 && row.StepCounter != 8 && row.StepCounter != 9 && row.StepCounter != 10) {
                            html += `<button class="cls-btndownloadpdf" id="btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details"><img src="/Images/digitalsign.png" width="40" /></button>`;
                        }
                        return html; // Return the full HTML string
                    }
                }
            ];
            break;
        default:
            columns = [
                {
                    title: `<div class="wd-30-f"><div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll_ApprovalForIO">
                    <label class="custom-control-label" for="chkAll_ApprovalForIO"></label>
                    </div></div>`,
                    className: "noExport",
                    width: "40px",
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll_ApprovalForIO").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        }
                    }
                },
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
                    title: "Appl Id",
                    data: "RequestId",
                    name: "ApplId",
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "ServiceNo",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return `<a href="#" class="cls-BasicDetail">${data.slice(0, 2) + ' ' + data.slice(2)}</a>`;

                        } else {
                            // No space needed
                            return `<a href="#" class="cls-BasicDetail">${data}</a>`;;
                        }
                    }
                },
                {
                    title: "Rank & Name",
                    data: null,
                    name: "Name",
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
                    title: `<div>History</div>`,
                    className: "noExport",
                    width: "100px",
                    data: null,
                    name: "History",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                    }
                },
                // Additional column for Edit action
                {
                    title: `<div>Preview | Fwd</div>`,
                    className: "noExport",
                    width: "210px",
                    data: null,
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        // Always include the Print Preview button
                        let html = `<button class="btn btn-icon btn-round btn-primary mr-2 cls-BasicDetail-PrintPreview"><i class="fa fa-eye mt-2"></i></button>`;

                        if (Type == 1 && (row.StepCounter == 2 || row.StepCounter == 3)) {
                            html += `<button class="btn btn-primary mr-1 cls-fwdrecord">Verify And Send</button>`;
                        }
                        // Case 2: Processed + Download
                        else if (row.StepCounter != 1 && row.StepCounter != 7 && row.StepCounter != 8 && row.StepCounter != 9 && row.StepCounter != 10) {
                            html += `<button class="cls-btndownloadpdf" id="btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details"><img src="/Images/digitalsign.png" width="40" /></button>`;
                        }
                        return html; // Return the full HTML string
                    }
                }
            ];
    }
    return columns;
}
function getSearchStatusForBindData(search) {
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