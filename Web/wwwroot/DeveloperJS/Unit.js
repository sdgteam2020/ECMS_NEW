var table; // Declare table variable outside the function to preserve the instance
var UnitId;
function safeAdjustUnitDataTable(api) {
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

    Reset();

    applyDataTableSearchValidation('#tbldata');

    BindData()
    $("#txtSerachunit").on("keyup", function () {
        BindData()
    });
    $("#btnReset").on("click", function () {
        Reset();
        ResetErrorMessage();
    });
    $("#btnsave").on("click", function (e) {
        e.preventDefault();

        Proceed();
    });

    $("input[name='IsVerify']").on("change", function () {
        $("#IsVerify-error").text("");
    });

    $('input.js-uppercase, textarea.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });

    $('#btnMultiDelete').on("click", function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);
                console.log(id);

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
function Proceed() {
    ResetErrorMessage();

    let formId = "#SaveForm";

    $.validator.unobtrusive.parse($(formId));

    let isFormValid = $(formId).valid();
    let isVerifyValid = $("input[name='IsVerify']:checked").length > 0;

    if (!isVerifyValid) {
        $("#IsVerify-error").text("Please select Verify Yes or No.");
    } else {
        $("#IsVerify-error").text("");
    }

    if (!isFormValid || !isVerifyValid) {
        return false;
    }

    Swal.fire({
        title: "Are you sure?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Save it!"
    }).then((result) => {
        if (result.isConfirmed) {
            Save();
        }
    });
}

function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
    }
    table = $("#tbldata").DataTable({
        scrollY: '300px',          // CSS stretches the scroll body inside the table card
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: false,
        scroller: false,          // UI only: normal DataTables scroll inside card
        deferScroll: false,        // UI only: normal scroll
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false,  //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        searching: true,
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
                let response = await fetch("/Master/GetAllUnit", {
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
                title: "Unit Id",
                data: "UnitId",
                name: "UnitId",
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
                title: "Unit SUS No",
                data: "Sus_no",
                name: "Sus_no",
                width: "110px",
                orderable: true,
            },
            {
                title: "Suffix",
                data: "Suffix",
                name: "Suffix",
                width: "80px",
                orderable: true,
            },
            {
                title: "Unit Name",
                data: "UnitName",
                name: "UnitName",
                orderable: false,
                width: "200px",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Unit Abbreviation",
                data: "Abbreviation",
                name: "Abbreviation",
                orderable: false,
                width: "100px",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            // Display user-friendly value for IsVerify
            {
                title: "Status",
                data: "IsVerify",
                name: "IsVerify",
                width: "100px",
                orderable: true,
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Verifed</span>" : "<span class='badge badge-pill badge-danger'>Not Verify</span>";
                }
            },
            // Additional column for Edit action
            {
                title: "Action",
                data: null,
                orderable: false,
                className: "noExport text-center col-action",
                width: "120px",
                render: function (data, type, row) {
                    return "<span id='btnedit'><button type='button' class='cls-btnedit btn ecms-action-btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn ecms-action-btn btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>";
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
            { targets: 2, width: "110px" },
            { targets: 3, width: "80px" },
            { targets: 4, width: "200px" },
            { targets: 5, width: "100px" },
            { targets: 6, width: "100px" },
            { targets: 7, width: "120px" },
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "UNIT SUS No" // Add custom placeholder
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
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_Unit',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        // ✅ ADD: initComplete for zoom handling
        initComplete: function () {
            // Force DataTables to calculate optimal widths
            safeAdjustUnitDataTable(this.api());

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    safeAdjustUnitDataTable(table);
                }, 100);
            });
        },
        drawCallback: function (settings) {
            // Recalculate widths on each data load
            safeAdjustUnitDataTable(this.api());

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });

            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    Reset();
                    ResetErrorMessage();

                    $("#txtSusno").val(rowData.Sus_no);
                    $("#txtSuffix").val(rowData.Suffix);
                    $("#txtUnitDesc").val(rowData.UnitName);
                    $("#txtAbbreviation").val(rowData.Abbreviation);
                    UnitId = rowData.UnitId;

                    if (rowData.IsVerify === true) {
                        $("#isverifyyes").prop("checked", true);
                    }
                    else {
                        $("#isverifyno").prop("checked", true);
                    }
                }
            });
            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
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
                            Delete(rowData.UnitId);
                        }
                    });
                }
            });
        }
    });

    // Force hide the column
    table.column(0).visible(false);
}

function Save() {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Master/SaveUnit',
        type: 'POST',
        data: {
            "Sus_no": $("#txtSusno").val().trim(),
            "UnitId": UnitId,
            "Suffix": $("#txtSuffix").val().trim(),
            "UnitName": $("#txtUnitDesc").val().trim(),
            "Abbreviation": $("#txtAbbreviation").val().trim(),
            "IsVerify": document.querySelector('input[type="radio"][name="IsVerify"]:checked')?.value,
        }, //get the search string
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
    $("#txtSusno").val("");
    $("#txtSuffix").val("");
    $("#txtUnitDesc").val("");
    $("#txtAbbreviation").val("");

    $("input[name='IsVerify']").prop("checked", false);
    $("#IsVerify-error").text("");

    UnitId = 0;
}
function ResetErrorMessage() {
    $("#txtSusno-error").html("");
    $("#txtSuffix-error").html("");
    $("#txtUnitDesc-error").html("");
    $("#txtAbbreviation-error").html("");
}

function Delete(Id) {
    var userdata =
    {
        "UnitId": Id,

    };
    $.ajax({
        url: '/Master/DeleteUnit',
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
                    toastr.error('UnitId is used in child table.');
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

function DeleteMultiple(Id) {

    var userdata =
    {
        "ints": Id,

    };
    $.ajax({
        url: '/Master/DeleteUnitMultiple',
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