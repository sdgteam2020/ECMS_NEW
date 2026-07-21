var table; // Declare table variable outside the function to preserve the instance
var tableView; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    //mMsater(0, "ddlRank", Rank, "");
    //mMsater(0, "ddlArmType", ArmyType, "");

    applyDataTableSearchValidation('#tbldata');

    BindData();
});

function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
        $("#tbldata").empty(); // UI fix: remove old DataTables cloned header/body markup
    }

    table = $("#tbldata").DataTable({
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

        autoWidth: false,  //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        searching: false,
        order: [[0, 'desc']], // Default sorting on the first column
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
                setTimeout(function () { FixClaimsDataTableUI('#tbldatadialog'); }, 30);
                setTimeout(function () { FixClaimsDataTableUI('#tbldata'); }, 30);


            } catch (error) {
                console.error("Error fetching data:", error);
                $("#loading").addClass("d-none").hide();
                $(".dataTables_processing").hide();
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                setTimeout(function () { FixClaimsDataTableUI("#tbldata"); }, 30);
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
                name: "TotalUsers",
                className: "text-center",
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
        dom: "<'dt-top'lBf>rt<'dt-bottom'ip>", // Add buttons to the DOM
        buttons: [
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: "thead th:not(.noExport)"
            //    }
            //},
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
        initComplete: function () {
            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();
            FixClaimsDataTableUI('#tbldata');

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table.columns.adjust();
                }, 100);
            });
        },
        drawCallback: function (settings) {
            // Recalculate widths on each data load
            this.api().columns.adjust();
            FixClaimsDataTableUI('#tbldata');

            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btneyetotalusers").on("click", ".cls-btneyetotalusers", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    BindDialog(rowData.ClaimType);
                }
            });

        }
    });
}
function BindDialog(claimValue) {
    // STEP 1: Move ALL DataTable code into shown.bs.modal
    $("#DataTableDialog").one('shown.bs.modal', function () {

        if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
            // Destroy the DataTable and clear the table content
            $("#tbldatadialog").DataTable().clear().destroy(); // Clear and destroy DataTable properly
            $("#tbldatadialog thead").empty(); // Clear old thead
            $("#tbldatadialog tbody").empty(); // Clear old tbody
            $("#tbldatadialog").empty(); // UI fix: remove old DataTables cloned header/body markup
        }

        $("#lblModelTitle").html(claimValue);

        tableView = $("#tbldatadialog").DataTable({
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

            autoWidth: false,  //Set autoWidth to true (let DataTables decide)
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
                    $("#loading").addClass("d-none").hide();
                    $(".dataTables_processing").hide();
                    callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                    setTimeout(function () { FixClaimsDataTableUI("#tbldatadialog"); }, 30);
                }
            },
            columns: [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    className: "text-center col-sno",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + (meta.settings?._iDisplayStart || 0) + 1;
                    }
                },
                {
                    title: "Domain ID",
                    data: "DomainId",
                    name: "DomainId",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row, meta) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "IC No",
                    data: "ArmyNo",
                    name: "ArmyNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row, meta) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Rank",
                    data: "Rank",
                    name: "Rank",
                    className: "nowrap",
                    width: "120px",
                },
                {
                    title: "Name",
                    data: "Name",
                    name: "Name",
                    className: "nowrap",
                    width: "120px",
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
                    className: "nowrap",
                    width: "150px",
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
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row, meta) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Role of User",
                    data: "RoleNames",
                    name: "RoleNames",
                    className: "nowrap",
                    width: "100px",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row) {
                        return data ? data.join(', ') : '';  // Convert array to string
                    }
                }
            ],
            /* ===== FORCE WIDTHS (IMPORTANT) ===== */
            columnDefs: [
                { targets: 0, width: "60px" },
                { targets: 1, width: "150px" },
                { targets: 2, width: "120px" },
                { targets: 3, width: "120px" },
                { targets: 4, width: "120px" },
                { targets: 5, width: "150px" },
                { targets: 6, width: "120px" },
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
            dom: "<'dt-top'lBf>rt<'dt-bottom'ip>", // Add buttons to the DOM
            buttons: [
                //{
                //    extend: 'copy',
                //    exportOptions: {
                //        columns: "thead th:not(.noExport)"
                //    }
                //},
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
            initComplete: function () {
                // Force DataTables to calculate optimal widths
                this.api().columns.adjust();
                FixClaimsDataTableUI('#tbldatadialog');

                // Handle zoom/resize
                var resizeTimer;
                $(window).on('resize', function () {
                    clearTimeout(resizeTimer);
                    resizeTimer = setTimeout(function () {
                        tableView.columns.adjust();
                    }, 100);
                });
            },
            drawCallback: function (settings) {
                // Recalculate widths on each data load
                this.api().columns.adjust();
                FixClaimsDataTableUI('#tbldatadialog');

                const tooltipTriggerList = [].slice.call(
                    document.querySelectorAll('[data-bs-toggle="tooltip"]')
                );
                if (window.bootstrap && bootstrap.Tooltip) {
                    tooltipTriggerList.forEach(el => {
                        try { new bootstrap.Tooltip(el); } catch (e) { }
                    });
                }

            }
        });
    });

    // STEP 2: Show modal (this triggers the above)
    $("#DataTableDialog").modal("show");

}

function FixClaimsDataTableUI(tableSelector) {
    try {
        const $table = $(tableSelector);
        const $wrapper = $(tableSelector + "_wrapper");

        $("#loading").addClass("d-none").hide();
        $(".dataTables_processing, " + tableSelector + "_processing").hide();

        // Hide only the cloned sizing header inside the DataTables scroll body.
        // This removes the duplicate/blank blue header row without removing the real header.
        $wrapper.find(".dataTables_scrollBody table thead, .dt-scroll-body table thead").css({
            display: "none",
            height: "0",
            maxHeight: "0",
            minHeight: "0",
            visibility: "collapse",
            overflow: "hidden"
        });

        $wrapper.find(".dataTables_scrollBody table thead tr, .dataTables_scrollBody table thead th, .dataTables_scrollBody table thead td, .dt-scroll-body table thead tr, .dt-scroll-body table thead th, .dt-scroll-body table thead td").css({
            display: "none",
            height: "0",
            maxHeight: "0",
            minHeight: "0",
            lineHeight: "0",
            fontSize: "0",
            padding: "0",
            margin: "0",
            border: "0",
            overflow: "hidden",
            visibility: "collapse"
        });

        $wrapper.find(".dt-bottom, .dataTables_info, .dataTables_paginate, .pagination").css({
            visibility: "visible",
            opacity: "1"
        });

        const $bottom = $wrapper.children(".dt-bottom");
        if ($bottom.length) {
            $bottom.css({
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                width: "100%"
            });
        }

        if ($.fn.DataTable.isDataTable(tableSelector)) {
            setTimeout(function () {
                $table.DataTable().columns.adjust();
            }, 80);
        }
    } catch (e) {
        console.warn("Claims DataTable UI fix skipped:", e);
    }
}


$('#DataTableDialog').on('shown.bs.modal.claimsUi', function () {
    setTimeout(function () {
        FixClaimsDataTableUI('#tbldatadialog');
    }, 120);
});
