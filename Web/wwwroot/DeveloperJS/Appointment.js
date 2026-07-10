var table; // Declare table variable outside the function to preserve the instance
let ApptId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    Reset();

    applyDataTableSearchValidation('#tbldata');

    BindData();

    $("#btnReset").on("click", function () {
        Reset();
        ResetErrorMessage();
    });

    $("#btnsave").on("click",function (e) {
        e.preventDefault();

        Proceed();

    });

    $("input[name='IsApproved']").on("change", function () {
        $("#Approved-error").text("");
    });

    $('#btnMultiDelete').on("click",function () {
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

function Proceed() {
    ResetErrorMessage();

    let formId = "#SaveForm";

    $.validator.unobtrusive.parse($(formId));

    let isFormValid = $(formId).valid();
    let isApprovedValid = $("input[name='IsApproved']:checked").length > 0;

    if (!isApprovedValid) {
        $("#Approved-error").text("Please select Approve Yes or No.");
    } else {
        $("#Approved-error").text("");
    }

    if (!isFormValid || !isApprovedValid) {
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
    const columns = getColumnsForAppointment();
    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        scroller: true,           // ✅ Enable virtual scrolling for better performance
        deferScroll: true,        // ✅ Improve scrolling performance
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false,  //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        order: [[2, 'desc']], // Default sorting on the first column
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
                let response = await fetch("/Master/GetAllAppointment_Pagination", {
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
        /* ===== FORCE WIDTHS (IMPORTANT) ===== */
        columnDefs: [
            {
                targets: 0,     // index of ApptId
                visible: false,
                width: "0px",
                searchable: false
            },
            { targets: 1, width: "60px" },
            { targets: 2, width: "200px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "200px" },
            { targets: 5, width: "120px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip",
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
                title: 'E-IASC_Appoinment',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
            // DataTables search input
            let searchBox = $('.dt-search input');

            // Remove default Bootstrap classes and add your custom class
            searchBox
                //.removeClass('form-control form-control-sm')
                //.addClass('form-control1')
                .attr('title', 'Search Appointment/Abbreviation');

            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table.columns.adjust().responsive.recalc();
                }, 100);
            });
        },
        drawCallback: function (settings) {

            // Recalculate widths on each data load
            this.api().columns.adjust().responsive.recalc();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ApptId != null) {
                    Reset();
                    ResetErrorMessage();

                    ApptId = rowData.ApptId;
                    $("#txtAppointment").val(rowData.AppointmentName);
                    if (rowData.AppointmentAbbreviation == "") {
                        $("#txtAbbreviation").val("");
                    }
                    else {
                        $("#txtAbbreviation").val(rowData.AppointmentAbbreviation);
                    }
                    if (rowData.Approved == true) {
                        $("#IsApprovedYes").prop("checked", true);
                    }
                    else {
                        $("#IsApprovedNo").prop("checked", true);
                    }

                    $("#btnsave").val("Update");

                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ApptId != null) {
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
                            Delete(rowData.ApptId);
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
    $.ajax({
        url: '/Master/SaveAppointment',
        type: 'POST',
        data:
        {
            "AppointmentName": $("#txtAppointment").val().trim(),
            "AppointmentAbbreviation": $("#txtAbbreviation").val().trim(),
            "ApptId": ApptId,
            "Approved": document.querySelector('input[type="radio"][name="IsApproved"]:checked')?.value,
        }, //get the search string
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {
            if (result == DataSave) {
                toastr.success('Appointment has been saved');
                BindData();
                Reset();
            }
            else if (result == DataUpdate) {
                toastr.success('Appointment has been Updated');
                BindData();
                Reset();
            }
            else if (result == DataExists) {
                toastr.error('Appointment Name Exits!');
            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }
                }
            }
        }
    });
}

function Reset() {
    ApptId = 0;
    $("#btnsave").val("Save");
    $("#txtAppointment").val("");
    $("#txtAbbreviation").val("");

    $("input[name='IsApproved']").prop("checked", false);
    $("#Approved-error").text("");
}

function ResetErrorMessage() {
    $("#txtAppointment-error").html("");
    $("#txtAbbreviation-error").html("");
}

function Delete(Id) {
  
    var userdata =
    {
        "ApptId": Id,

    };
    $.ajax({
        url: '/Master/DeleteAppointment',
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
                    toastr.error('AppId is used in child table.');
                }
                else if (response == Success) {
                    toastr.success('Deleted Selected!');
                    BindData();
                }
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

function DeleteMultiple(Ids) {
   
    var userdata =
    {
        "ints": Ids,

    };
    $.ajax({
        url: '/Master/DeleteAppointmentMultiple',
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
                    toastr.success('Deleted Selected!');
                    BindData();
                }
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
function getColumnsForAppointment() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "ApptId",
            name: "ApptId",
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
            title: "Appointment",
            data: "AppointmentName",
            name: "AppointmentName",
            className: "nowrap",
            width: "200px",
            orderable: true, 
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Abbreviation",
            data: "AppointmentAbbreviation",
            name: "AppointmentAbbreviation",
            className: "nowrap",
            width: "200px",
            orderable: true, 
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Approve",
            data: "Approved",
            name: "Approved",
            className: "nowrap",
            width: "200px",
            orderable: true, 
            render: function (data, type, row, meta) {
                return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
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
                let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>`;
                return Action;
            }
        }
    ];
    return columns;
}