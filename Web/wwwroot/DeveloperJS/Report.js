var table; // Declare table variable outside the function to preserve the instance
$(function () {
    if ($('#ddlCommand').length > 0) {
        mMsater(0, "ddlCommand", 1, "");
    }
    //$("#btnRequisition").on("click", function (event) {
    //    event.preventDefault(); // Prevent anchor default behavior
    //    GetReportReturnHistory('Requisition');
    //})
    $("#btnRequisition").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('New Requisition');
        $("#CardReport").modal("show");
        $('#CardReport').one('shown.bs.modal', function () {
            GetReportReturnHistory('Requisition');
        });
    });
    $("#btnNonFunctionalCard").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('Non Functional Card');
        $("#CardReport").modal("show");
        $('#CardReport').one('shown.bs.modal', function () {
            GetReportReturnHistory('NonFunctional');
        });
    });
});
function GetReportReturnHistory(Choice) {
    if ($.fn.DataTable.isDataTable("#CardReport_tbldatadialog")) {
        $("#CardReport_tbldatadialog").DataTable().destroy();
        $("#CardReport_tbldatadialog").empty(); // Clear old thead/tbody
    }
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
    const columns = getColumnsByChoice(Choice);

    table = $("#CardReport_tbldatadialog").DataTable({
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
                //$("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
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
                title: 'E-IASC_Report',
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
function getColumnsByChoice(choice) {
    let columns = [];

    switch (choice) {
        case 'Requisition':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Request ID",
                    data: 'RequestId',
                    name: 'RequestId',
                },
                {
                    title: "Tracking Id",
                    data: 'TrackingId',
                    name: 'TrackingId',
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo"
                },
                {
                    title: "Rank,Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `${row.RankName} ${row.FName} ${row.LName != null ? row.LName : ""}`;
                    }
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Status",
                    data: "StepId",
                    name: "StepId",
                    render: function (data, type, row) {
                        let color;
                        if (data == 1 || data == 2 || data == 3 || data == 4 || data == 5 || data == 6) {
                            color = 'warning';
                        }
                        else {
                            color = 'dangers';
                        }
                        return `<span class='badge badge-${color} mr-1' >${row.Status}</span></span>`;
                    }
                }
            ];
            break;

        case 'NonFunctional':
            columns = [
                { title: "S No", data: null, orderable: false, render: (data, type, row, meta) => meta.row + meta.settings._iDisplayStart + 1 },
                { title: "Issue ID", data: 'IssueId' },
                { title: "Item Name", data: 'ItemName' },
                { title: "Issued To", data: 'IssuedTo' },
                { title: "Date", data: 'IssueDate' }
            ];
            break;

        case 'Returned':
            columns = [
                { title: "S No", data: null, orderable: false, render: (data, type, row, meta) => meta.row + meta.settings._iDisplayStart + 1 },
                { title: "Return ID", data: 'ReturnId' },
                { title: "Returned Item", data: 'ItemName' },
                { title: "Returned By", data: 'ReturnedBy' },
                { title: "Return Date", data: 'ReturnDate' },
                { title: "Condition", data: 'ConditionStatus' }
            ];
            break;

        default:
            columns = [
                { title: "S No", data: null, orderable: false, render: (data, type, row, meta) => meta.row + meta.settings._iDisplayStart + 1 },
                { title: "ID", data: 'Id' },
                { title: "Description", data: 'Description' }
            ];
    }

    return columns;
}
