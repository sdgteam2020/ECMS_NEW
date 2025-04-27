//const { debug } = require("util");

var table; // Declare table variable outside the function to preserve the instance
$(function () {
    BindData()
});

function BindData() {
    $("#tbldata").DataTable().destroy();
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
            };
            try {
                let response = await fetch("/Master/GetAllMapUnitChange", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams(requestData).toString()
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
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "ChangeMapUnitId", name: "ChangeMapUnitId" },
            { data: "UnitName", name: "UnitName" },
            { data: "FromArmyNo", name: "FromArmyNo" },
            {
                data: null,
                name: "FromNameWithRank",
                 render: function (data, type, row) {
                     return (row.FromRankAbbreviation || '') + " " + (row.FromName || '');
                }
            },
            { data: "FromDID", name: "FromDID" },
            // Display user-friendly value for FromUpdatedOn
            {
                data: "FromUpdatedOn",
                name: "FromUpdatedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                },
            },
            { data: "Remark", name: "Remark" },
            { data: "AdminRemark", name: "AdminRemark" },

            // Additional column for Edit action
            //{
            //    data: null,
            //    orderable: false,
            //    render: function (data, type, row) {
            //        return "<span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span>";
            //    }
            //},
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search IC No" // Add custom placeholder
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
            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {

                }
            });
        }
    });
}

function Reset() {
    $("#txtSearch").val("");

    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#ddlRank").val("");
    $("#txtName").val("");
    /*    $("#txtMobileNo").val("");*/
    $("#ddlArmType").val("");
    $("#IsTokenWaiverYes").prop("checked", false);
    $("#IsTokenWaiverNo").prop("checked", false);
    $("#txtMessage").val("");
    $("#isTokenyes").prop("checked", false);
    $("#isTokenno").prop("checked", false);
    $("#IsWithTokenApplyyes").prop("checked", false);
    $("#IsWithTokenApplyno").prop("checked", false);
    $("#btnProfileAddButton").val("Save");
    $("#exampleModalLabel").html("Enter Profile Details");
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#ddlRank-error").html("");
    $("#txtArmyNo-error").html("");
    /*    $("#txtMobileNo-error").html("");*/
    $("#ddlArmType-error").html("");
    $("#IsTokenWaiver-error").html("");
    $("#txtMessage-error").html("");
    $("#IsToken-error").html("");
    $("#IsWithTokenApply-error").html("");

}