$(function () {
    if ($('#ddlCommand').length > 0) {
        mMsater(0, "ddlCommand", 1, "");
    }
    $("#btnRequisition").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        GetReportReturnHistory('Requisition');
    })
});
function GetReportReturnHistory(Choice) {
    $("#CardReport_tbldatadialog").DataTable().destroy();

    var userdata =
    {
        "TableId": 0,
        "ComdId": $('#ddlCommand').length > 0 ? $('#ddlCommand').val() : 0,
        "CorpsId": $('#ddlCorps').length > 0 ? $('#ddlCorps').val() : 0,
        "DivId": $('#ddlDiv').length > 0 ? $('#ddlDiv').val() : 0,
        "BdeId": $('#ddlBde').length > 0 ? $('#ddlBde').val() : 0,
        "FmnBranchID": $('#ddlFmnBranch').length > 0 ? $('#ddlFmnBranch').val() : 0,
        "PsoId": $('#ddlPSODte').length > 0 ? $('#ddlPSODte').val() : 0,
        "SubDteId": $('#ddlDgSubDte').length > 0 ? $('#ddlDgSubDte').val() : 0,
        "UnitMapId": $('#ddlUnit').length > 0 ? $('#ddlUnit').val() : 0,
        "Choice": Choice,
    };
    tableView = $("#CardReport_tbldatadialog").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[1, 'desc']], // Default sorting on the first column
        responsive: true,
        autoWidth: false,
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                ...userdata
            };
            try {
                let response = await fetch("/Home/GetReportData", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(requestData)
                });
                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                $("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            // Serial number column
            //{ data: "RequestId", name: "RequestId", visible: false },
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "ServiceNo", name: "ServiceNo" },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.RankName} ${row.FName} ${row.LName != null ? row.LName : ""}`;
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return row.RankFrom != null ? `<span id='divName'>${row.RankFrom} ${row.NameFrom} (${row.ArmyNoFrom}) (${row.DomainIdFrom})</span>` : "<span id='divName'></span>";
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return row.RankTo != null ? `<span id='divName'>${row.RankTo} ${row.NameTo} (${row.ArmyNoTo}) (${row.DomainIdTo})</span>` : "<span id='divName'></span>";
                }
            },
            {
                data: 'TrackingId',
                name: 'TrackingId',
                render: function (data, type, row) {
                    return data ? `<span id='comdName'>${data}</span>` : '';
                }
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return `<span id='comdName'>${DateFormateddMMyyyyhhmmss(data)}</span>`;
                }
            },
            {
                data: "StatusName",
                name: "StatusName",
                render: function (data, type, row) {
                    let color = 'primary';
                    if (data == "Pending") {
                        color = 'warning';
                    }
                    else if (data == "Rejected") {
                        color = 'dangers';
                    }
                    else {
                        color = 'success'
                    }
                    return data ? `<span id='comdName'><span class='badge badge-${color} mr-1' >${row.StatusName}</span></span>` : `<span id='comdName'><span class='badge badge-primary mr-1' >Action Pending</span></span>`;
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No" // Add custom placeholder
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
        }
    });
}
