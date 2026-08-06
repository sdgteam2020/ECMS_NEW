var table; // Declare table variable outside the function to preserve the instance
let ArmedId = 0;
function ecmsArmedTypeSafeCall(name, callback) {
    try {
        return callback();
    } catch (error) {
        console.error("ArmedType startup error in " + name + ":", error);
        return null;
    }
}

function safeAdjustArmedTypeDataTable(api) {
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

    ecmsArmedTypeSafeCall("Reset", function () {
        if (typeof Reset === "function") {
            Reset();
        }
    });

    ecmsArmedTypeSafeCall("mMsater ddlArmedCat bind", function () {
        if (typeof mMsater === "function" && typeof ArmyCat !== "undefined") {
            mMsater(0, "ddlArmedCat", ArmyCat, "");
        } else {
            console.error("Required dependency missing: mMsater or ArmyCat. Check mtables.js and command.js load before ArmedType.js.");
        }
    });

    ecmsArmedTypeSafeCall("DataTable search validation", function () {
        if (typeof applyDataTableSearchValidation === "function") {
            applyDataTableSearchValidation('#tbldata');
        }
    });

    ecmsArmedTypeSafeCall("BindData", function () {
        if (typeof BindData === "function") {
            BindData(function () { });
        }
    });
    $("#btnReset").on("click", function () {
        Reset();
    });

    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });

    $("#btnsave").on("click", function (e) {
        if ($("#SaveForm")[0].checkValidity()) {
            if ($("input[name='radioInf']:checked").length === 0) {
                e.preventDefault();

                Swal.fire({
                    icon: "warning",
                    text: "Please select Infantry Yes or No."
                });

                return false;
            }

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
    }
    const columns = getColumnsForArmedType();
    table = $("#tbldata").DataTable({
        scrollY: '300px',          // DataTables vertical scroll
        scrollX: true,            // DataTables horizontal scroll when needed
        scrollCollapse: false,
        scroller: false,         // Do not use Scroller here; pagination is already enabled
        deferScroll: false,    // Avoid virtual-width calculation issues
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: true,       // Let DataTables calculate natural column widths
        responsive: false,     // Keep normal DataTable columns
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
                let response = await fetch("/Master/GetAllArmed_Pagination", {
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
        columns: columns,
        columnDefs: [
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
        },
        dom: "<'row g-2 align-items-center mb-2 ecms-dt-toolbar'<'col-12 col-md-4 d-flex justify-content-start dt-length-col'l><'col-12 col-md-4 d-flex justify-content-center dt-buttons-col'B><'col-12 col-md-4 d-flex justify-content-md-end dt-filter-col'f>>" +
            "rt" +
            "<'row g-2 align-items-center mt-2 ecms-dt-footer'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 d-flex justify-content-md-end dt-page-col'p>>",
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
                title: 'E-IASC_Arms_Service',
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
            searchBox.attr('title', 'Search Comd/Abbreviation');

            // Force DataTables to calculate optimal widths
            safeAdjustArmedTypeDataTable(this.api());
            setTimeout(function () { safeAdjustArmedTypeDataTable(table); }, 80);

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    safeAdjustArmedTypeDataTable(table);
                }, 100);
            });
        },
        drawCallback: function (settings) {

            // Recalculate widths on each data load
            safeAdjustArmedTypeDataTable(this.api());

            if (window.bootstrap && bootstrap.Tooltip) {
                const tooltipTriggerList = [].slice.call(
                    document.querySelectorAll('[data-bs-toggle="tooltip"]')
                );
                tooltipTriggerList.forEach(el => {
                    new bootstrap.Tooltip(el);
                });
            }

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ArmedId != null) {
                    $("#ddlArmedCat").val(rowData.ArmedCatId);
                    $("#txtArmedName").val(rowData.ArmedName);
                    $("#txtAbbreviation").val(rowData.Abbreviation.toUpperCase());

                    if (rowData.flagInf == true) {
                        $("#radioInfyes").prop("checked", true);
                    }
                    else {
                        $("#radioInfno").prop("checked", true);
                    }
                    ArmedId = rowData.ArmedId;
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ArmedId != null) {
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
                            Delete(rowData.ArmedId);
                        }
                    });
                }
                else {
                    //Invalid Data
                }
            });


        }
    });
}
function Save() {
    const payload = {
        "ArmedName": $("#txtArmedName").val().trim(),
        "ArmedCatId": $("#ddlArmedCat").val(),
        "ArmedId": ArmedId,
        "Abbreviation": $("#txtAbbreviation").val().trim(),
        "FlagInf": $("#radioInfyes").prop("checked")
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    $.ajax({
        url: '/Master/SaveArmed',
        type: 'POST',
        data: { Request: encrypted },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                toastr.success(result.Message);
                BindData();
                Reset();
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
    $("#ddlArmedCat").val("");
    $("#txtArmedName").val("");
    $("#txtAbbreviation").val("");
    ArmedId = 0;
}

function Delete(Id) {
    var userdata =
    {
        "ArmedId": Id,

    };
    $.ajax({
        url: '/Master/DeleteArmed',
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
                else if (response == "5") {
                    toastr.error('ArmedId is used in child table.');
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

function DeleteMultiple(ids) {

    var userdata =
    {
        "ints": ids,

    };
    $.ajax({
        url: '/Master/DeleteArmedMultiple',
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
                    toastr.error('Deleted Selected');
                    BindData();
                }

                //}
            }

        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function getColumnsForArmedType() {
    let columns = [];
    columns = [
        // Serial number column
        {
            title: "S No",
            data: null,
            name: "SerialNumber",
            orderable: false, // Disable sorting for this column
            className: "text-center col-sno",
            render: function (data, type, row, meta) {
                // Calculate serial number based on row index
                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        {
            title: "Arms / Service & Corps",
            data: "ArmedName",
            name: "ArmedName",
            className: "nowrap",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Abbreviation",
            data: "Abbreviation",
            name: "Abbreviation",
            className: "nowrap",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Infantry",
            data: "FlagInf",
            name: "FlagInf",
            className: "text-center nowrap",
            orderable: true,
            render: function (data, type, row, meta) {
                var value = String(data).toLowerCase();
                var isYes = data === true || data === 1 || value === "true" || value === "1" || value === "yes";

                if (type !== "display") {
                    return isYes ? "YES" : "NO";
                }

                return isYes
                    ? "<span class='badge rounded-pill bg-success text-white ecms-status-badge ecms-status-yes'>YES</span>"
                    : "<span class='badge rounded-pill bg-danger text-white ecms-status-badge ecms-status-no'>NO</span>";
            }
        },
        {
            title: "Type",
            data: "Name",
            name: "Name",
            className: "nowrap",
            orderable: true,
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
            render: function (data, type, row) {
                let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>`;
                return Action;
            }
        }
    ];
    return columns;
}