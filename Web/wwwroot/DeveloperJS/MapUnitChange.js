//const { debug } = require("util");

var table; // Declare table variable outside the function to preserve the instance
function prepareMapUnitChangeModalRoots() {
    ["UnitMoveHistoryModal", "MessageDialog"].forEach(function (modalId) {
        var modalElement = document.getElementById(modalId);

        if (modalElement && modalElement.parentElement !== document.body) {
            document.body.appendChild(modalElement);
        }
    });
}

function cleanupMapUnitChangeModalState() {
    if (document.querySelector(".modal.show")) {
        document.body.classList.add("modal-open");
        return;
    }

    document.querySelectorAll(".modal-backdrop").forEach(function (element) {
        element.remove();
    });

    document.body.classList.remove("modal-open");
    document.body.style.removeProperty("overflow");
    document.body.style.removeProperty("padding-right");
}

function refreshMapUnitChangeDataTable(tableSelector, delay) {
    var wait = Number.isFinite(delay) ? delay : 0;

    window.setTimeout(function () {
        try {
            var $wrapper = $(tableSelector + "_wrapper");

            $("#loading").addClass("d-none").hide();
            $wrapper.find(".dataTables_processing, .dt-processing").hide();

            $wrapper
                .find(".dataTables_scrollBody table thead, .dt-scroll-body table thead")
                .attr("aria-hidden", "true");

            if ($.fn.DataTable && $.fn.DataTable.isDataTable(tableSelector)) {
                safeAdjustMapUnitChangeDataTable($(tableSelector).DataTable());
            }
        } catch (error) {
            console.warn("Unit Move Request DataTable refresh skipped:", error);
        }
    }, wait);
}

function safeAdjustMapUnitChangeDataTable(api) {
    if (!api) {
        return;
    }

    api.columns.adjust();

    if (api.responsive && typeof api.responsive.recalc === "function") {
        api.responsive.recalc();
    }
}

$(function () {
    prepareMapUnitChangeModalRoots();

    $("#UnitMoveHistoryModal, #MessageDialog")
        .off(".mapUnitChangeUi")
        .on("shown.bs.modal.mapUnitChangeUi", function () {
            document.body.classList.add("modal-open");
        })
        .on("hidden.bs.modal.mapUnitChangeUi", function () {
            cleanupMapUnitChangeModalState();
        });

    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldata');

    BindData();
    $("#btnAdd").on("click", function () {
        location.href = '/Master/MapUnitChangeRequest';
    });
});

function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
        $("#tbldata").empty(); // Remove old DataTables sizing markup
    }
    table = $("#tbldata").DataTable({
        scrollY: '100%',          // CSS stretches the scroll body inside the table card
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: false,
        scroller: false,          // UI only: normal DataTables scroll inside card
        deferScroll: false,        // UI only: normal scroll
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: false,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false,  //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        order: [[0, 'desc']], // Default sorting on the first column
        searching: true,
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order?.[0]?.column >= 0 && data.columns?.[data.order[0].column]?.data || '',
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
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
                refreshMapUnitChangeDataTable("#tbldata", 30);

            } catch (error) {
                console.error("Error fetching data:", error);
                $("#loading").addClass("d-none").hide();
                $(".dataTables_processing, .dt-processing").hide();
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                refreshMapUnitChangeDataTable("#tbldata", 30);
            }
        },
        columns: [
            {
                title: "Id",
                data: "MapUnitChangeRequestId",
                name: "MapUnitChangeRequestId",
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
                title: "Unit Name",
                data: "UnitName",
                name: "UnitName",
                orderable: false,
                width: "150px",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "SUS No",
                data: "Sus_no",
                name: "Sus_no",
                width: "110px",
                orderable: false,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}${row.Suffix}">${data}${row.Suffix}</span>`;
                }
            },
            {
                title: "Army No",
                data: "FromArmyNo",
                name: "FromArmyNo",
                width: "110px",
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Rank & Name",
                data: "FromName",
                name: "FromName",
                orderable: false,
                width: "150px",
                render: function (data, type, row) {
                    let FromName = (row.FromRankAbbreviation || '') + " " + (data || '');
                    if (!FromName) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${FromName}">${FromName}</span>`;
                }
            },
            {
                title: "Domain ID",
                data: "FromDID",
                name: "FromDID",
                width: "100px",
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            // Display user-friendly value for FromUpdatedOn
            {
                title: "Request Dt & Time",
                data: "FromUpdatedOn",
                name: "FromUpdatedOn",
                width: "150px",
                orderable: true,
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "User Remark",
                data: "Remark",
                name: "Remark",
                width: "150px",
                orderable: true,
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;

                }
            },
            {
                title: "Admin Remark",
                data: "AdminRemark",
                name: "AdminRemark",
                width: "150px",
                orderable: true,
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;

                }
            },
            {
                title: "Status",
                data: "IsEditAction",
                name: "Status",
                width: "100px",
                orderable: true,
                render: function (data, type, row) {
                    return data == false ? "<span class='badge bg-warning'>Pending</span>" : row.RequestStatus == true ? "<span class='badge bg-success'>Accepted</span>" : "<span class='badge badge-pill badge-danger'>Rejected</span>";
                }
            },

            // Additional column for Edit action
            {
                title: "Action",
                data: "IsEditAction",
                name: "Action",
                orderable: false,
                className: "noExport text-center col-action",
                width: "220px",
                render: function (data, type, row) {
                    let role = $("#spnRoleName").html(); // Get current role
                    if (data === false && role === "admin") {
                        return `<span id='btnedit'><button type='button' class='cls-btnedit btn ecms-action-btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><span id='btnview'><button type='button' class='cls-btnview btn ecms-action-btn btn-icon btn-round btn-warning mr-1'><i class="fa fa-eye" ></i></button></span>`;
                    }
                    else if (data === true && role === "admin") {
                        return `<span class='badge badge-pill badge-danger mr-1'>NA</span><span id='btnview'><button type='button' class='cls-btnview btn ecms-action-btn btn-icon btn-round btn-warning mr-1'><i class="fa fa-eye" ></i></button></span>`;
                    }
                    else {
                        return `<span id='btnview'><button type='button' class='cls-btnview btn ecms-action-btn btn-icon btn-round btn-warning mr-1'><i class="fa fa-eye" ></i></button></span>`;
                    }
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
            { targets: 2, width: "190px" },
            { targets: 3, width: "110px" },
            { targets: 4, width: "110px" },
            { targets: 5, width: "100px" },
            { targets: 6, width: "150px" },
            { targets: 7, width: "150px" },
            { targets: 8, width: "150px" },
            { targets: 9, width: "100px" },
            { targets: 10, width: "220px" }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search SUS No" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rt<'dt-bottom'ip>",
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
        initComplete: function () {
            let searchBox = $("#tbldata_wrapper div.dataTables_filter input");
            searchBox.attr("title", "Search SUS No or unit move request details");

            safeAdjustMapUnitChangeDataTable(this.api());
            refreshMapUnitChangeDataTable("#tbldata", 20);

            $(window)
                .off("resize.mapUnitChangeDataTable")
                .on("resize.mapUnitChangeDataTable", function () {
                    window.clearTimeout(window.__mapUnitChangeResizeTimer);
                    window.__mapUnitChangeResizeTimer = window.setTimeout(function () {
                        refreshMapUnitChangeDataTable("#tbldata", 0);
                    }, 120);
                });
        },
        drawCallback: function (settings) {
            safeAdjustMapUnitChangeDataTable(this.api());
            refreshMapUnitChangeDataTable("#tbldata", 20);

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );

            if (window.bootstrap && bootstrap.Tooltip) {
                tooltipTriggerList.forEach(function (element) {
                    try {
                        if (bootstrap.Tooltip.getOrCreateInstance) {
                            bootstrap.Tooltip.getOrCreateInstance(element);
                        } else {
                            new bootstrap.Tooltip(element);
                        }
                    } catch (error) {
                        console.warn("Unit Move Request tooltip skipped:", error);
                    }
                });
            }

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
        }
    });

    // Force hide the column
    table.column(0).visible(false);

    //if ($("#spnRoleName").html() === "admin") {
    //    table.column(9).visible(true);
    //}
    //else {
    //    table.column(9).visible(false);
    //}
}

/* ==============================================================
   PAGE-LOCAL UI EVENTS
   No global ModernCSS file is changed.
================================================================ */

$(document)
    .off("draw.dt.mapUnitChangeUi")
    .on("draw.dt.mapUnitChangeUi", function (event, settings) {
        var tableId = settings && settings.nTable ? settings.nTable.id : "";

        if (tableId === "tbldata") {
            refreshMapUnitChangeDataTable("#tbldata", 20);
        }
    });
