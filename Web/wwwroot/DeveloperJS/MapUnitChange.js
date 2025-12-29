//const { debug } = require("util");

var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData();
    $("#btnAdd").on("click", function (){
        location.href = '/Master/MapUnitChangeRequest';
    });
});

function BindData() {
    $("#tbldata").DataTable().destroy();
    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY
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
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
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
                data: "MapUnitChangeRequestId",
                name: "MapUnitChangeRequestId",
                visible: false,
            },
            {
                data: "UnitName",
                name: "UnitName",
                orderable: false,
            },
            {
                data: "Sus_no",
                name: "Sus_no",
            },
            { data: "FromArmyNo", name: "FromArmyNo" },
            {
                data: "FromName",
                name: "FromName",
                orderable: false,
                render: function (data, type, row) {
                    return (row.FromRankAbbreviation || '') + " " + (data || '');
                }
            },
            { data: "FromDID", name: "FromDID" },
            // Display user-friendly value for FromUpdatedOn
            {
                data: "FromUpdatedOn",
                name: "FromUpdatedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: "Remark",
                name: "Remark",
                render: function (data, type, row) {
                    if (data != null) {
                        let sentence = data;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                        return `<span class='cls-Remark'>${truncatedSentence}</span>`;
                    } else {
                        return `NA`;
                    }

                }
            },
            {
                data: "AdminRemark",
                name: "AdminRemark",
                render: function (data, type, row) {
                    if (data != null) {
                        let sentence = data;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                        return `<span class='cls-AdminRemark'>${truncatedSentence}</span>`;
                    } else {
                        return `NA`;
                    }

                }
            },
            {
                data: "IsEditAction",
                name: "Status",
                render: function (data, type, row) {
                    return data == false ? "<span class='badge bg-warning'>Pendding</span>" : row.RequestStatus == true ? "<span class='badge bg-success'>Accepted</span>" : "<span class='badge badge-pill badge-danger'>Rejected</span>";
                }
            },

            // Additional column for Edit action
            {
                data: "IsEditAction",
                name: "Action",
                orderable: false,
                render: function (data, type, row) {
                    let role = $("#spnRoleName").html(); // Get current role
                    if (data === false && role === "admin") {
                        return `<span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><span id='btnview'><button type='button' class='cls-btnview btn btn-icon btn-round btn-warning mr-1'><i class="fa fa-eye" ></i></button></span>`;
                    }
                    else if (data === true && role === "admin")
                    {
                        return `<span class='badge badge-pill badge-danger mr-1'>NA</span><span id='btnview'><button type='button' class='cls-btnview btn btn-icon btn-round btn-warning mr-1'><i class="fa fa-eye" ></i></button></span>`;
                    }
                    else {
                        return `<span id='btnview'><button type='button' class='cls-btnview btn btn-icon btn-round btn-warning mr-1'><i class="fa fa-eye" ></i></button></span>`;
                    }
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search SUS No" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
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
                    window.location.href = '/Master/MapUnitChangeRequest?Id=' + encodeURIComponent(rowData.EncryptedId);
                }
            });
            $("#tbldata tbody").off("click", ".cls-btnview").on("click", ".cls-btnview", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetUnitMoveHistory(rowData.MapUnitChangeRequestId);
                }
            });
            $("#tbldata tbody").off("click", ".cls-Remark").on("click", ".cls-Remark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html(`User Remark`);
                    $("#MessageDialogBody").html(rowData.Remark);
                    $("#MessageDialog").modal('show');
                }
            });
            $("#tbldata tbody").off("click", ".cls-AdminRemark").on("click", ".cls-AdminRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html(`Admin  Remark`);
                    $("#MessageDialogBody").html(rowData.AdminRemark);
                    $("#MessageDialog").modal('show');
                }
            });
        }
    });
    //if ($("#spnRoleName").html() === "admin") {
    //    table.column(9).visible(true);
    //}
    //else {
    //    table.column(9).visible(false);
    //}
}