var table; // Declare table variable outside the function to preserve the instance
var tableView; // Declare table variable outside the function to preserve the instance

/*
 * Keep the modal directly below <body>.
 * This is a UI-only stacking fix: a modal inside a transformed layout container
 * can appear below its backdrop and look faded/disabled.
 */
function PrepareClaimsModalRoot() {
    var $modal = $("#DataTableDialog");

    if ($modal.length && !$modal.parent().is("body")) {
        $modal.appendTo(document.body);
    }
}

/*
 * Common CSS handles the cloned DataTable header and footer layout.
 * JavaScript now only asks DataTables to recalculate column widths.
 */
function RefreshClaimsDataTable(tableSelector, delay) {
    window.setTimeout(function () {
        try {
            $("#loading").addClass("d-none").hide();
            $(".dataTables_processing, " + tableSelector + "_processing").hide();

            if ($.fn.DataTable.isDataTable(tableSelector)) {
                $(tableSelector).DataTable().columns.adjust();
            }
        } catch (error) {
            console.warn("Claims DataTable refresh skipped:", error);
        }
    }, delay || 0);
}
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    PrepareClaimsModalRoot();

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
                RefreshClaimsDataTable('#tbldata', 30);


            } catch (error) {
                console.error("Error fetching data:", error);
                $("#loading").addClass("d-none").hide();
                $(".dataTables_processing").hide();
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                RefreshClaimsDataTable('#tbldata', 30);
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
                name: "ClaimType",
                className: "col-claim"
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
        dom: "<'dt-top ecms-dt-toolbar'lBf>rt<'dt-bottom ecms-dt-footer'ip>", // Common ECMS toolbar/footer classes
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
            RefreshClaimsDataTable('#tbldata', 0);

            // Handle zoom/resize
            var resizeTimer;
            $(window).off('resize.claimsMain').on('resize.claimsMain', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table.columns.adjust();
                }, 100);
            });
        },
        drawCallback: function (settings) {
            // Recalculate widths on each data load
            this.api().columns.adjust();
            RefreshClaimsDataTable('#tbldata', 0);

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
    PrepareClaimsModalRoot();

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
                    RefreshClaimsDataTable('#tbldatadialog', 30);
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
            dom: "<'dt-top ecms-dt-toolbar'lBf>rt<'dt-bottom ecms-dt-footer'ip>", // Common ECMS toolbar/footer classes
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
                RefreshClaimsDataTable('#tbldatadialog', 0);

                // Handle zoom/resize
                var resizeTimer;
                $(window).off('resize.claimsDialog').on('resize.claimsDialog', function () {
                    clearTimeout(resizeTimer);
                    resizeTimer = setTimeout(function () {
                        tableView.columns.adjust();
                    }, 100);
                });
            },
            drawCallback: function (settings) {
                // Recalculate widths on each data load
                this.api().columns.adjust();
                RefreshClaimsDataTable('#tbldatadialog', 0);

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
    PrepareClaimsModalRoot();
    $("#DataTableDialog").modal("show");

}

$('#DataTableDialog')
    .off('.claimsUi')
    .on('shown.bs.modal.claimsUi', function () {
        RefreshClaimsDataTable('#tbldatadialog', 120);
    })
    .on('hidden.bs.modal.claimsUi', function () {
        /* Remove a stale backdrop only when no other Bootstrap modal is open. */
        if (!$('.modal.show').length) {
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css('padding-right', '');
        }
    });
