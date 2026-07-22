var table; // Declare table variable outside the function to preserve the instance
let AfsacCellMappingId = 0;
let UnitMapId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldata');

    BindData();
    $("#btnAdd").on("click", function () {
        Reset();
        ResetErrorMessage();
        $("#btnAfsacCellMappingAdd").val("Save");
        $("#AddNewAfsacCellMapping").modal('show');
    });
    $("#btnAfsacCellMappingAdd").on("click", function () {
        Proceed();
    });
    $("#btnAfsacCellMappingReset").on("click", function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                UnitMapId = 0;
                var param = { "UnitName": request.term };
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.Sus_no + item.Suffix + ' ' + item.UnitName, value: item.UnitMapId };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            UnitMapId = 0;
                            $("#ddlTDMId").find("option").not(":first").remove();
                            $("#ddlTDMId").val("0");
                            alert("Unit not found.")
                        }
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            UnitMapId = i.item.value;
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetDDMappedForRecord',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

                success: function (response) {
                    if (response != "null" && response != null) {
                        if (response == InternalServerError) {
                            Swal.fire({
                                text: errormsg
                            });
                        }

                        else {

                            var listItemddl = "";

                            listItemddl += '<option value="0">Please Select</option>';

                            for (var i = 0; i < response.length; i++) {
                                listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                            }
                            $("#ddlTDMId").html(listItemddl);
                        }
                    }
                    else {
                        //Swal.fire({
                        //    text: "No data found Offrs"
                        //});
                    }
                },
                error: function (result) {
                    Swal.fire({
                        text: errormsg002
                    });
                }
            });
        },

    });
});

function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveAfsacCellMapping';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
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
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        })
        toastr.error('Please fill required field.');
        return false;
    }
}
function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
        $("#tbldata").empty(); // UI fix: remove old DataTables cloned header/body markup
    }
    const columns = getColumnsForAfsacCell();
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
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search?.value || '',  // ✅ Safe access
                sortColumn: data.order?.[0]?.column >= 0 && data.columns?.[data.order[0].column]?.data || '',
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
            };
            try {
                let response = await fetch("/Master/GetAllAfsacCellMapping_Pagination", {
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
                setTimeout(function () {
                    FixAfsacCellMappingUI();
                    FixAfsacCellDoubleHeaderAndThemeUI();
                    FixAfsacCellDoubleHeaderAndThemeUI();
                }, 30);


            } catch (error) {
                console.error("Error fetching data:", error);
                $("#loading").addClass("d-none").hide();
                $(".dataTables_processing").hide();
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                setTimeout(function () { FixAfsacCellMappingUI(); FixAfsacCellDoubleHeaderAndThemeUI(); }, 30);
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
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
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
                title: 'E-IASC_AfsacCellMapping',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
            // Add tooltip to the search input box
            let searchBox = $('div.dataTables_filter input');
            searchBox.attr('title', 'Search');

            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();
            FixAfsacCellMappingUI();

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
            FixAfsacCellMappingUI();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            if (window.bootstrap && bootstrap.Tooltip) {
                tooltipTriggerList.forEach(el => {
                    try { new bootstrap.Tooltip(el); } catch (e) { }
                });
            }

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.AfsacCellMappingId != null) {
                    Reset();
                    ResetErrorMessage();
                    AfsacCellMappingId = rowData.AfsacCellMappingId;

                    if (rowData.UnitId != null) {
                        UnitMapId = rowData.UnitId;
                        $("#txtUnitName").val(`${rowData.Sus_no}${rowData.Suffix} ${rowData.UnitName}`);
                    }
                    else {
                        UnitMapId = 0;
                        $("#txtUnitName").val("");
                    }
                    if (rowData.TDMId != null) {
                        GetDDMappedForRecord(rowData.UnitId, rowData.TDMId);
                    }
                    else {
                        $("#ddlTDMId").val("0");
                    }

                    $("#btnAfsacCellMappingAdd").val("Update");
                    $("#AddNewAfsacCellMapping").modal('show');
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.AfsacCellMappingId != null) {
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
                            Delete(rowData.AfsacCellMappingId);
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
        "AfsacCellMappingId": AfsacCellMappingId,
        "TDMId": $("#ddlTDMId").val() == 0 ? null : $("#ddlTDMId").val(),
        "UnitId": UnitMapId == 0 ? null : UnitMapId,
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    $.ajax({
        url: '/Master/SaveAfsacCellMapping',
        type: 'POST',
        data: { Request: encrypted },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {


            if (result.Result == true) {
                toastr.success(result.Message);
                $("#AddNewAfsacCellMapping").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
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
    AfsacCellMappingId = 0;
    $("#txtUnitName").val("");
    $("#ddlTDMId").val("0");
    UnitMapId = 0;
}
function ResetErrorMessage() {
    $("#txtUnitName-error").html("");
    $("#ddlTDMId-error").html("");
}

function GetDDMappedForRecord(UnitId, TDMId) {
    var param1 = { "UnitMapId": UnitId };
    $.ajax({
        url: '/Master/GetDDMappedForRecord',
        contentType: 'application/x-www-form-urlencoded',
        data: param1,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="0">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                    }
                    $("#ddlTDMId").html(listItemddl);
                    if (TDMId != '') {
                        $("#ddlTDMId").val(TDMId);
                    }
                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function Delete(Id) {
    var userdata =
    {
        "AfsacCellMappingId": Id,

    };
    $.ajax({
        url: '/Master/DeleteAfsacCellMapping',
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

                else if (response == Success) {
                    //lol++;
                    //if (lol == Tot) {

                    toastr.success('Deleted Selected');
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
function getColumnsForAfsacCell() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "AfsacCellMappingId",
            name: "AfsacCellMappingId",
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
            title: "Linked Domain ID & Pers Details",
            data: "Name",
            name: "Name",
            className: "nowrap",
            width: "200px",
            orderable: false,
            render: function (data, type, row, meta) {
                if (row.TDMId != null) {
                    let name = `${row.DomainId} & ${row.ArmyNo} ${row.RankAbbreviation} ${row.Name}`;
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${name}">${name}</span>`;
                }
                else {
                    return ``;
                }

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
            width: "200px",
            render: function (data, type, row) {
                let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>`;
                return Action;
            }
        }
    ];
    return columns;
}

function FixAfsacCellMappingUI() {
    try {
        const $page = $(".ecms-afsac-cell-page");
        const $wrapper = $("#tbldata_wrapper");

        $("#loading").addClass("d-none").hide();
        $(".dataTables_processing, #tbldata_processing").hide();

        // Hide DataTables cloned sizing header inside the scroll body.
        $page.find(".dataTables_scrollBody table thead, .dt-scroll-body table thead").css({
            height: "0",
            maxHeight: "0",
            visibility: "collapse",
            overflow: "hidden"
        });

        $page.find(".dataTables_scrollBody table thead tr, .dataTables_scrollBody table thead th, .dataTables_scrollBody table thead td, .dt-scroll-body table thead tr, .dt-scroll-body table thead th, .dt-scroll-body table thead td").css({
            height: "0",
            maxHeight: "0",
            minHeight: "0",
            lineHeight: "0",
            fontSize: "0",
            paddingTop: "0",
            paddingBottom: "0",
            paddingLeft: "0",
            paddingRight: "0",
            borderTop: "0",
            borderBottom: "0",
            overflow: "hidden",
            visibility: "collapse"
        });

        // Keep footer and pagination visible.
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

        if (table && $.fn.DataTable.isDataTable("#tbldata")) {
            setTimeout(function () {
                table.columns.adjust();
            }, 80);
        }
    } catch (e) {
        console.warn("Afsac Cell Mapping UI fix skipped:", e);
    }
}


function FixAfsacCellDoubleHeaderAndThemeUI() {
    try {
        const $page = $(".ecms-afsac-cell-page");
        const $wrapper = $("#tbldata_wrapper");

        $("#loading").addClass("d-none").hide();
        $(".dataTables_processing, #tbldata_processing").hide();

        // IMPORTANT: DataTables creates a cloned header inside scrollBody.
        // Hide/remove only the cloned sizing header, not the real scrollHead header.
        $page.find(".dataTables_scrollBody thead, .dt-scroll-body thead").each(function () {
            $(this).css({
                display: "none",
                height: "0",
                maxHeight: "0",
                minHeight: "0",
                visibility: "collapse",
                overflow: "hidden"
            });
        });

        $page.find(".dataTables_scrollBody thead tr, .dataTables_scrollBody thead th, .dataTables_scrollBody thead td, .dt-scroll-body thead tr, .dt-scroll-body thead th, .dt-scroll-body thead td").css({
            display: "none",
            height: "0",
            maxHeight: "0",
            minHeight: "0",
            lineHeight: "0",
            fontSize: "0",
            padding: "0",
            margin: "0",
            border: "0",
            visibility: "collapse",
            overflow: "hidden"
        });

        $wrapper.find(".dt-bottom, .dataTables_info, .dataTables_paginate, .pagination").css({
            visibility: "visible",
            opacity: "1"
        });
    } catch (e) {
        console.warn("Afsac Cell theme/header UI fix skipped:", e);
    }
}
