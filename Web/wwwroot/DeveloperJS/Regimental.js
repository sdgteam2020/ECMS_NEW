var table; // Declare table variable outside the function to preserve the instance
var RegId = 0;
var UnitMapId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    mMsater(0, "ddlArmType", 9, "");
    BindData()
    $("#btnAddRegimental").on("click",function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewRegimental").modal('show');
    });
    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });

    $("#btnResetRegimental").on("click",function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                UnitMapId = 0;
                const param = new URLSearchParams({ UnitName: request.term });

                fetch('/Master/GetALLByUnitName', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: param
                })
                    .then(res => {
                        if (!res.ok) {
                            throw new Error('Network response was not ok');
                        }
                        return res.json();
                    })
                    .then(data => {
                        if (data.length !== 0) {
                            response(data.map(item => {
                                $("#loading").addClass("d-none");
                                return {
                                    label: item.Sus_no + item.Suffix + ' ' + item.UnitName,
                                    value: item.UnitMapId
                                };
                            }));
                        } else {
                            $("#txtUnitName").val("");
                            UnitMapId = 0;
                            alert("Unit not found.");
                        }
                    })
                    .catch(error => {
                        alert(error.message);
                    });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            UnitMapId = i.item.value;
        },
        
    });

    $('#txtUnitName').on('keyup',function (e) {
        if (e.key === 'Delete') {
            $("#txtUnitName").val("");
            UnitMapId = 0;
            $("#ddlTDMId").find("option").not(":first").remove();
            $("#ddlTDMId").val("0");
        }
    });
   
    $("#btnSaveRegimental").on("click",function () {
        Proceed();
    });
});
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveRegimental';
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
    }
    const columns = getColumnsForRegimental();
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
                let response = await fetch("/Master/GetAllRegimental_Pagination", {
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
                targets: 0,
                visible: false,
                width: "0px",
                searchable: false
            },
            { targets: 1, width: "60px" },
            { targets: 2, width: "200px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "200px" },
            { targets: 5, width: "200px" },
            { targets: 6, width: "200px" },
            { targets: 7, width: "120px" },
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
                title: 'E-IASC_Regimental',
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
                if (rowData.RegId != null) {
                    Reset();
                    ResetErrorMessage();
                    $("#txtName").val(rowData.Name);
                    $("#txtAbbreviation").val(rowData.Abbreviation.toUpperCase());
                    $("#txtLocation").val(rowData.Location);
                    RegId = rowData.RegId;
                    $("#ddlArmType").val(rowData.ArmedId);

                    if (rowData.UnitId != null) {
                        UnitMapId = rowData.UnitId;
                        $("#txtUnitName").val(`${rowData.Sus_no}${rowData.Suffix} ${rowData.UnitName}`);
                    }
                    else {
                        UnitMapId = 0;
                        $("#txtUnitName").val("");
                    }
                    $("#btnSaveRegimental").val("Update");
                    $("#AddNewRegimental").modal('show');
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RegId != null) {
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
                            Delete(rowData.RegId);
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
        Name: $("#txtName").val().trim(),
        RegId: RegId,
        Abbreviation: $("#txtAbbreviation").val().trim(),
        ArmedId: $("#ddlArmType").val(),
        Location: $("#txtLocation").val().trim(),
        UnitId: (() => {
            let val = UnitMapId;
            return val === "0" || val === "" ? null : parseInt(val, 10);
        })()
    };
    fetch('/Master/SaveRegimental', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json', // change to JSON
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: JSON.stringify(payload) // send proper JSON
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // or use response.text() depending on server response type
        })
        .then(result => {
            if (result === DataSave) {
                toastr.success('Regimental has been saved');
                $("#AddNewRegimental").modal('hide');
                BindData();
                Reset();
            } else if (result === DataUpdate) {
                toastr.success('Regimental has been updated');
                $("#AddNewRegimental").modal('hide');
                BindData();
                Reset();
            } else if (result === DataExists) {
                toastr.error('Regimental / Abbreviation Name exists!');
            } else if (result === InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or invalid entry!',
                });
            } else {
                if (Array.isArray(result) && result.length > 0) {
                    for (let i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage);
                    }
                }
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
            toastr.error('Failed to save data. Please try again.');
        });
}

function Reset() {
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#txtLocation").val("");
    $("#txtUnitName").val("");
    $("#ddlArmType").val("0");
    UnitMapId = 0;
    RegId = 0;
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#txtLocation-error").html("");
    $("#txtUnitName-error").html("");
    $("#ddlArmType-error").html("");
}
function Delete(Id) {
    const userdata = new URLSearchParams({ RegId: Id });

    fetch('/Master/DeleteRegimental', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: userdata
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // Important: parse JSON instead of text
        })
        .then(response => {
            if (response !== null) {
                if (response === InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else if (response === Success) {
                    toastr.success('Deleted Selected');
                    BindData();
                }
            } else {
                Swal.fire({
                    text: errormsg001
                });
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
            Swal.fire({
                text: errormsg002
            });
        });
}

function DeleteMultiple(ids) {
   
    var userdata =
    {
        "ints": ids,

    };
    $.ajax({
        url: '/Master/DeleteRegimentalMultiple',
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
function getColumnsForRegimental() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "RegId",
            name: "RegId",
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
            title: "Regimental Centre",
            data: "Name",
            name: "Name",
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
            data: "Abbreviation",
            name: "Abbreviation",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Location",
            data: "Location",
            name: "Location",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Arms / Service",
            data: "ArmedName",
            name: "ArmedName",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Unit",
            data: "UnitAbbreviation",
            name: "UnitAbbreviation",
            className: "nowrap",
            width: "200px",
            orderable: false, 
            render: function (data, type, row, meta) {
                if (row.UnitId != null)
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                else
                    return ``;
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