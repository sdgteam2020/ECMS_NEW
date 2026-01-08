var table; // Declare table variable outside the function to preserve the instance
var tableView; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    //mMsater(0, "ddlRank", Rank, "");
    //mMsater(0, "ddlArmType", ArmyType, "");
    BindData();
});

function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        $("#tbldata").DataTable().destroy();
        $("#tbldata").empty(); // Clear old thead/tbody
    }

    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false, // Let us handle width via CSS
        responsive: false, // ✅ IMPORTANT (disable)
        order: [[0, 'desc']], // Default sorting on the first column
        searching: false ,
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
                let response = await fetch("/Account/GetAllClaims", {
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
            {
                title: "",
                data: "TotalUsers",
                name: "TotalUsers",
                visible: false,        // hidden
                searchable: false,
                width: "0px",
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
                title: "Claim",
                data: "ClaimType",
                name: "ClaimType"
            },
            {
                title: "User Claim Count",
                data: "TotalUsers",
                name: "TotalUsers"
            }
            ,
            {
                title: "View",
                data: null,
                className: "noExport",
                orderable: false,
                render: function (data, type, row) {
                    return "<span id='btneyetotalusers'><button type='button' class='cls-btneyetotalusers btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i></button></span>";
                }
            }
        ],
        /* ===== FORCE WIDTHS (IMPORTANT) ===== */
        columnDefs: [
            {
                targets: 0,
                visible: false,
                width: "0px",
                searchable: false
            },
            { targets: 1, width: "60px" },
            { targets: 2, width: "200px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "200px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Type / Value" // Add custom placeholder
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
                orientation: 'portrait',
                pageSize: 'A4',
                title: 'E-IASC_Claim',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btneyetotalusers").on("click", ".cls-btneyetotalusers", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null)
                {
                    BindDialog(rowData.ClaimType);
                }
            });
            
        }
    });
}
function BindDialog(claimValue) {
    if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
        $("#tbldatadialog").DataTable().destroy();
        $("#tbldatadialog").empty(); // Clear old thead/tbody
    }
    $("#lblModelTitle").html(claimValue);
    $("#DataTableDialog").modal('show');
    tableView = $("#tbldatadialog").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false, // Let us handle width via CSS
        responsive: false, // ✅ IMPORTANT (disable)
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '',
                choice: claimValue,
            };
            try {   
                let response = await fetch("/Account/GetAllUsersByClaim", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                //$("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            {
                title: "S No",
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + (meta.settings?._iDisplayStart || 0) + 1;
                }
            },
            {
                title: "Domain ID",
                data: "DomainId",
                name: "DomainId",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "IC No",
                data: "ArmyNo",
                name: "ArmyNo",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Rank",
                data: "Rank",
                name: "Rank"
            },
            {
                title: "Name",
                data: "Name",
                name: "Name",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Unit",
                data: "Unit",
                name: "Unit",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Appt",
                data: "AppointmentName",
                name: "AppointmentName",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Role of User",
                data: "RoleNames",
                name: "RoleNames",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row) {
                    return data ? data.join(', ') : '';  // Convert array to string
                }
            }
        ],
        /* ===== FORCE WIDTHS (IMPORTANT) ===== */
        columnDefs: [
            { targets: 0, width: "60px" },
            { targets: 1, width: "200px" },
            { targets: 2, width: "150px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "200px" },
            { targets: 5, width: "200px" },
            { targets: 6, width: "200px" },
            { targets: 7, width: "100px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search IC No" // Add custom placeholder
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
                orientation: 'portrait',
                pageSize: 'A4',
                title: 'E-IASC_UsersByClaim',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
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