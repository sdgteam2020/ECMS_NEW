var table_Fwd; // Declare table variable outside the function to preserve the instance
$(function () {
    let Type = parseInt($("#spnType").html());
    let StepCounter = parseInt($("#spnStepCounter").html());
    let JCOOR = $("#spnJCOOR").html();
    let VBId = $("#spnVBId").html();
    let cvalue = parseInt($("#spnClaim").html());
    BindData(Type, StepCounter, JCOOR, cvalue, function () {
        // Reset global variables as explained
        selectedIds = [];
        previousSearchText = "";
        isFirstSelectAll = true;
        searchChanged = false;
        globalAllChecked = false;
    });
});
function BindData(Type, StepCounter, JCOOR, cvalue) {
    selectedIds = [];

    if ($.fn.DataTable.isDataTable("#tbldatatabledata_Fwd")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_Fwd").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_Fwd thead").empty(); // Clear old thead
        $("#tbldatatabledata_Fwd tbody").empty(); // Clear old tbody
    }


    const columns = getColumnsForApprovalForIO(cvalue, JCOOR);
    table_Fwd = $("#tbldatatabledata_Fwd").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,
        order: [[2, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let searchStatus = getSearchStatusForBindData(data.search.value);

            // Clear old selectedIds on search change, but keep globalAllChecked state
            if (searchStatus.searchChanged) {
                selectedIds = [];

                // Mark for re-fetch if needed
                if (globalAllChecked) {
                    isFirstSelectAll = true;
                }
            }

            // ✅ Determine if a fetch is needed
            const shouldFetchSelectedIds =
                globalAllChecked && (isFirstSelectAll || searchStatus.searchChanged) ||
                (!globalAllChecked && searchStatus.searchChanged && isFirstSelectAll);

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
                AllChecked: shouldFetchSelectedIds ? true : globalAllChecked,
                searchTextChanged: searchStatus.searchChanged
            };
            try {
                let response = await fetch("/BasicDetail/GetAllApprovalForIOData", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(requestData)
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }

                // Only update selectedIds if server returns new ones
                if (shouldFetchSelectedIds) {
                    if (result.selectedIds != null && result.selectedIds.length > 0) {
                        //selectedIds = result.selectedIds;
                        selectedIds = result.selectedIds.map(x => x.toString());
                        console.log("Fetched selectedIds from server:", selectedIds);
                        // If user hadn’t checked Select All, now we just load into selectedIds silently
                        if (globalAllChecked) isFirstSelectAll = false;
                    }
                    else {
                        //selectedIds = [];
                        if (globalAllChecked) {
                            globalAllChecked = false;
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
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                }
            },
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
        },
        drawCallback: function (settings) {

            updateUICheckboxes('#tbldatatabledata_Fwd', 'chkRequestId', '#chkAll_ApprovalForIO', selectedIds);

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
        }
    });
    $(document).on('change', '.chkRequestId', async function () {
        await updateSelectedIds('#tbldatatabledata_Fwd', 'chkRequestId');
        updateUICheckboxes('#tbldatatabledata_Fwd', 'chkRequestId', '#chkAll_ApprovalForIO', selectedIds); // Sync master checkbox state
    });
    $('#chkAll_ApprovalForIO').on('change', function () {
        selectedIds = [];
        globalAllChecked = $(this).prop('checked');
        if (globalAllChecked) {
            isFirstSelectAll = true; // Force fresh fetch
        }
        table_Fwd.ajax.reload();
    });
}

function getColumnsForApprovalForIO(cvalue, JCOOR) {
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
                    title: "Sno",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl Id",
                    data: "ApplId",
                    name: "ApplId",
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
                        return (fullName);
                    }
                },
                {
                    title: "Unit",
                    data: "UnitName",
                    name: "UnitName",
                    orderable: false,
                },
                {
                    title: "Regtl Centre",
                    data: "RegimentalName",
                    name: "RegimentalName",
                    render: function (data, type, row) {
                        return data != null ? data : "";
                    }
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Reason for Requisition",
                    data: "ICardType",
                    name: "ICardType",
                },
                {
                    title: `<div>History</div>`,
                    className: "noExport",
                    data: null,
                    name: "History",
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" aria-hidden="true"></i></button>`
                    }
                },
                // Additional column for Edit action
                {
                    title: `<div>Print | Fwd</div>`,
                    className: "noExport",
                    data: null,
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        // Always include the Print Preview button
                        let html = `<button class="btn btn-icon btn-round btn-primary mr-2" onclick="GetICardPrintPreviewByRequestId(${row.RequestId})"><i class="fa fa-print mt-2"></i></button>`;

                        if (parseInt($("#spnVBId").html()) == 1 && (row.StepCounter == 2 || row.StepCounter == 3)) {
                            html += `<button class="btn btn-primary mr-1 cls-fwdrecord">Verify And Send</button>`;
                        }
                        // Case 2: Processed + Download
                        else if (row.StepCounter != 1 && row.StepCounter != 7 && row.StepCounter != 8 && row.StepCounter != 9 && row.StepCounter != 10) {
                            html += `<button class="cls-btndownloadpdf" id="btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details"><img src="/Images/digitalsign.png" width="40" /></button>
                                <button class="cls-btndownloadxml ml-2" id="btndownloadxml" data-toggle="tooltip" data-placement="top" title="Download Details">Xml</button>
                                `;
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
                    title: "Sno",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl Id",
                    data: "ApplId",
                    name: "ApplId",
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
                        return (fullName);
                    }
                },
                {
                    title: "Unit",
                    data: "UnitName",
                    name: "UnitName",
                    orderable: false,
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Reason for Requisition",
                    data: "ICardType",
                    name: "ICardType",
                },
                {
                    title: `<div>History</div>`,
                    className: "noExport",
                    data: null,
                    name: "History",
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" aria-hidden="true"></i></button>`
                    }
                },
                // Additional column for Edit action
                {
                    title: `<div>Print | Fwd</div>`,
                    className: "noExport",
                    data: null,
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        // Always include the Print Preview button
                        let html = `<button class="btn btn-icon btn-round btn-primary mr-2" onclick="GetICardPrintPreviewByRequestId(${row.RequestId})"><i class="fa fa-print mt-2"></i></button>`;

                        if (parseInt($("#spnVBId").html()) == 1 && (row.StepCounter == 2 || row.StepCounter == 3)) {
                            html += `<button class="btn btn-primary mr-1 cls-fwdrecord">Verify And Send</button>`;
                        }
                        // Case 2: Processed + Download
                        else if (row.StepCounter != 1 && row.StepCounter != 7 && row.StepCounter != 8 && row.StepCounter != 9 && row.StepCounter != 10) {
                            html += `<button class="cls-btndownloadpdf" id="btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details"><img src="/Images/digitalsign.png" width="40" /></button>
                                <button class="cls-btndownloadxml ml-2" id="btndownloadxml" data-toggle="tooltip" data-placement="top" title="Download Details">Xml</button>
                                `;
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
    searchChanged = (
        (currentSearchText !== previousSearchText)
    );

    // Update previous values after comparison
    previousSearchText = currentSearchText;

    return {
        searchChanged: searchChanged,
        currentSearchText
    };
}