var table; // Declare table variable outside the function to preserve the instance
let BdeId = 0;
function refreshBdeDataTable(tableSelector, delay) {
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
                safeAdjustBdeDataTable($(tableSelector).DataTable());
            }
        } catch (error) {
            console.warn("Brigade DataTable refresh skipped:", error);
        }
    }, wait);
}

function safeAdjustBdeDataTable(api) {
    if (!api) {
        return;
    }

    api.columns.adjust();

    if (api.responsive && typeof api.responsive.recalc === "function") {
        api.responsive.recalc();
    }
}

$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    mMsater(0, "ddlCommand", 1, "");

    applyDataTableSearchValidation('#tbldata');

    BindData(function () { });


    $('#ddlCommand').on('change', function () {

        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
    });

    $('#ddlCorps').on('change', function () {

        mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
    });

    $("#btnReset").on("click", function () {
        Reset();
    });

    $("#btnsave").on("click", function () {
        if ($("#SaveForm")[0].checkValidity()) {

            Swal.fire({
                title: 'Are you sure?',
                text: "",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Save it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    Save();
                }
            })

        } else {
            $("#SaveForm")[0].reportValidity();
        }



        // 

    });

    $('#btnMultiDelete').on("click", function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);

            });

            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Delete",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Delete it!'
            }).then((result) => {
                if (result.value) {

                    DeleteMultiple(lst);

                }
            });
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 data to Delete."
            });
        }
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
    const columns = getColumnsForBde();
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
                let response = await fetch("/Master/GetAllBde_Pagination", {
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
                refreshBdeDataTable("#tbldata", 30);

            } catch (error) {
                console.error("Error fetching data:", error);
                $("#loading").addClass("d-none").hide();
                $(".dataTables_processing, .dt-processing").hide();
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                refreshBdeDataTable("#tbldata", 30);
            }
        },
        columns: columns,
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
            { targets: -1, width: "120px" },
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
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
                orientation: 'portrait',
                pageSize: 'A4',
                title: 'E-IASC_Bde',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
            let searchBox = $("#tbldata_wrapper div.dataTables_filter input");
            searchBox.attr("title", "Search Comd, Corps, Division or Brigade");

            safeAdjustBdeDataTable(this.api());
            refreshBdeDataTable("#tbldata", 20);

            $(window)
                .off("resize.bdeDataTable")
                .on("resize.bdeDataTable", function () {
                    window.clearTimeout(window.__bdeResizeTimer);
                    window.__bdeResizeTimer = window.setTimeout(function () {
                        refreshBdeDataTable("#tbldata", 0);
                    }, 120);
                });
        },
        drawCallback: function (settings) {
            safeAdjustBdeDataTable(this.api());
            refreshBdeDataTable("#tbldata", 20);

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
                        console.warn("Brigade tooltip skipped:", error);
                    }
                });
            }

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.BdeId != null) {

                    $("#ddlCommand").val(rowData.ComdId);

                    mMsater(rowData.CorpsId, "ddlCorps", 2, rowData.ComdId);

                    mMsaterByParent(rowData.DivId, "ddlDiv", 3, rowData.ComdId, rowData.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId

                    BdeId = rowData.BdeId;

                    $("#txtBdeName").val(rowData.BdeName);
                    $("#btnsave").val("Update");

                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.BdeId != null) {
                    Swal.fire({
                        title: 'Are you sure?',
                        text: "You want to Delete ",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#072697',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Yes, Delete It!'
                    }).then((result) => {
                        if (result.value) {
                            Delete(rowData.BdeId);
                        }
                    });
                }
                else {
                    //Invalid Data
                }
            });


        }
    });

    // Force hide the column
    table.column(0).visible(false);
}
function Save() {

    const payload = {
        "BdeName": $("#txtBdeName").val(),
        "ComdId": $("#ddlCommand").val(),
        "CorpsId": $("#ddlCorps").val(),
        "DivId": $("#ddlDiv").val(),
        "BdeId": BdeId
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    $.ajax({
        url: '/Master/SaveBde',
        type: 'POST',
        data: { Request: encrypted },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                toastr.success(result.Message);
                Reset();
                BindData();
            }
            else {
                const Message = result.Message || "Something went wrong.";

                const errors = Message
                    .split(";")
                    .map(x => x.trim())
                    .filter(x => x !== "");

                const list = document.createElement("ul");
                list.classList.add("error-list"); // ✅ use CSS class

                errors.forEach(function (error) {
                    const item = document.createElement("li");
                    item.textContent = error;
                    list.appendChild(item);
                });

                Swal.fire({
                    icon: "error",
                    title: "Message",
                    html: list
                });
            }
        }
    });
}

function Reset() {
    $("#ddlCommand").val("");
    $("#ddlCorps").val("");
    $("#ddlDiv").val("");
    BdeId = 0;
    $("#btnsave").val("Save");
    $("#txtBdeName").val("");
}

function Delete(BdeId) {
    var userdata =
    {
        "BdeId": BdeId,

    };
    $.ajax({
        url: '/Master/DeleteBde',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }
                else if (response == "5") {
                    toastr.error('BdeId is used in child table.');
                }

                else if (response == Success) {
                    //lol++;
                    //if (lol == Tot) {

                    toastr.success('Deleted Selected!');

                    BindData();
                }

                //}
            }
            else {
                Swal.fire({
                    text: errormsg001
                });
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function DeleteMultiple(BdeCatId) {

    var userdata =
    {
        "ints": BdeCatId,

    };
    $.ajax({
        url: '/Master/DeleteBdeMultiple',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }

                else if (response == Success) {
                    //lol++;
                    //if (lol == Tot) {

                    toastr.success('Deleted Selected!');

                    BindData();
                }

                //}
            }
            else {
                Swal.fire({
                    text: errormsg001
                });
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function getColumnsForBde() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "BdeId",
            name: "BdeId",
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
            title: "Comd / PSO",
            data: "ComdName",
            name: "ComdName",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Corps / Dte / Area",
            data: "CorpsName",
            name: "CorpsName",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Div  /Branch / SubArea",
            data: "DivName",
            name: "DivName",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Bde",
            data: "BdeName",
            name: "BdeName",
            orderable: false, // Disable sorting for this column
            className: "nowrap",
            width: "200px",
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        // Additional column for Edit action
        {
            title: "Action",
            data: null,
            className: "noExport",
            name: "Action",
            orderable: false,
            className: "noExport text-center col-action",
            width: "120px",
            render: function (data, type, row) {
                let Action = `<button type='button' class='cls-btnedit btn ecms-action-btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn ecms-action-btn btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>`;
                return Action;
            }
        }
    ];
    return columns;
}

/* ==============================================================
   PAGE-LOCAL UI EVENTS
   No global ModernCSS file is changed.
================================================================ */

$(document)
    .off("draw.dt.bdeUi")
    .on("draw.dt.bdeUi", function (event, settings) {
        var tableId = settings && settings.nTable ? settings.nTable.id : "";

        if (tableId === "tbldata") {
            refreshBdeDataTable("#tbldata", 20);
        }
    });
