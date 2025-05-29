var IsDigitalSignReq = true;
var StepCounter = 0;
var table; // Declare table variable outside the function to preserve the instance
$(function () {
    let Type = parseInt($("#spnType").html());
    let StepCounter = parseInt($("#spnStepCounter").html());
    let JCOOR = $("#spnJCOOR").html();
    let VBId = $("#spnVBId").html();
    BindData(Type, StepCounter, JCOOR, VBId);
});
function BindData(Type, StepCounter, JCOOR, VBId) {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        $("#tbldata").DataTable().destroy();
    }
 
    table = $("#tbldata").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
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
                apply: 0,
                JCOOR: JCOOR
            };
            try {
                let response = await fetch("/BasicDetail/GetAllIndexData", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
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
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "ServiceNo",
                name: "ServiceNo",
                render: function (data, type, row) {
                    // Check if first two characters are alphabets
                    if (/^[A-Za-z]{2}/.test(data)) {
                        // Insert space after first two characters
                        return `<a href="#" onclick="GetBasicDetailByRequestId(${row.RequestId});event.preventDefault();">${data.slice(0, 2) + ' ' + data.slice(2)}</a>`;
                        
                    } else {
                        // No space needed
                        return `<a href="#" onclick="GetBasicDetailByRequestId(${row.RequestId});event.preventDefault();">${data}</a>`;;
                    }
                }
            },
            {
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
                title: "Appl ID",
                data: "TrackingId",
                name: "TrackingId",
            },
            {
                title: "Type",
                data: "ApplyFor",
                name: "ApplyFor"
            },
            {
                data: "ICardType",
                name: "ICardType",
            },
            {
                data: "IsPosting",
                name: "IsPosting",
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
                data: null,
                name: "History",
                render: function (data, type, row) {
                    return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" aria-hidden="true"></i></button>`
                }
            },
            // Additional column for Edit action
            {
                data: null,
                name: "Action",
                orderable: false,
                render: function (data, type, row) {
                    // Always include the Print Preview button
                    let html = `<button class="btn btn-icon btn-round btn-primary mr-2" onclick="GetICardPrintPreviewByRequestId(${row.RequestId})"><i class="fa fa-print mt-2"></i></button>`;

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
                                <button class=" btndownloadpdf" id="btndownloadpdf" data-toggle="tooltip" data-placement="top" title="Download Details">
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
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
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
                title: 'E-IASC_MapUnitChange',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
            $("#tbldata tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetRequestHistory(rowData.RequestId);
                }
            });
        }
    });
}