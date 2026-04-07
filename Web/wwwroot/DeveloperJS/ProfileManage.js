//const { debug } = require("util");

var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    mMsater(0, "ddlRank", Rank, "");
    mMsater(0, "ddlArmType", ArmyType, "");
    BindData()

    $('#TokenWaiverInfo').on({
        mouseenter: function () {
            $('#hoverModal').modal('show'); // Show the modal on hover
        },
        mouseleave: function () {
            // Delay hiding the modal slightly to check if the mouse is over the modal
            setTimeout(function () {
                if (!$('#hoverModal').is(':hover')) {
                    $('#hoverModal').modal('hide'); // Hide the modal if the mouse is not over it
                }
            }, 200); // Adjust the delay as needed
        }
    });
    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });
    $("#btnProfileAddButton").on("click", function () {
        Proceed();
    });

    $("#IsTokenWaiverYes").on("click", function () { 
        $("#spnReasonTokenWaiver").removeClass("d-none");
        $('#txtMessage').prop('required', true);
        $("#txtMessage-error").html('Reason for IACA Token Waiver is required.');
    });
    $("#IsTokenWaiverNo").on("click", function () { 
        $("#spnReasonTokenWaiver").addClass("d-none"); 
        $('#txtMessage').prop('required', false);
        $('#txtMessage').val('');
        $("#txtMessage-error").html('');
    });

    $("#btnProfileAdd").on("click",function () {
        Reset();
        ResetErrorMessage();
        $("#btnProfileAddButton").val("Save");
        $("#spnReasonTokenWaiver").addClass("d-none"); 
        $("#AddNewProfile").modal('show');
    });
    $("#btnProfileAddReset").on("click",function () {
        Reset();
        ResetErrorMessage();
    });
    
    $("#txtSearch").on("keyup",function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            if ($("input[type='radio'][name=choice]:checked").val() == "UserId") {
                var num_val = parseInt(eThis.val()); 
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData()
                }
            }
            else {
                BindData()
            }
        }
        else {
            alert("Select Choice");
        }
    });
});

function Proceed() {
    ResetErrorMessage();
    let formId = '#SaveProfile';
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
        order: [[1, 'desc']], // Default sorting on the first column
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
                let response = await fetch("/Account/GetAllProfileManage", {
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
                title: "User ID",
                data: "UserId",
                name: "UserId",
                className: "nowrap",
                width: "100px",
            },
            {
                title: "Domain ID",
                data: "DomainId",
                name: "DomainId",
                className: "nowrap",
                width: "180px",
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
                render: function (data, type, row) {
                    return data ? data : "<span class='badge badge-pill badge-danger'>IC No Not Mapped</span>";
                }
            },
            {
                title: "Rank",
                data: "RankAbbreviation",
                name: "RankAbbreviation",
                className: "nowrap",
                width: "100px",
            },
            {
                title: "Name",
                data: "Name",
                name: "Name",
                orderable: false,
                className: "nowrap",
                width: "150px",
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
                width: "180px",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            // Display user-friendly value for IsVerify
            {
                title: "Token Waiver",
                data: "IsTokenWaiver",
                name: "IsTokenWaiver",
                className: "",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>" ;
                }
            },
            {
                title: "Token Required",
                data: "IsToken",
                name: "IsToken",
                className: "",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            {
                title: "Apply For I-Card With Token",
                data: "IsWithTokenApply",
                name: "IsWithTokenApply",
                className: "",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
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
                    return "<span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>";
                }
            }
        ],
        columnDefs: [
            { targets: 0, width: "60px", },
            { targets: 1, width: "100px" },
            { targets: 2, width: "180px" },
            { targets: 3, width: "120px" },
            { targets: 4, width: "100px" },
            { targets: 5, width: "150px" },
            { targets: 6, width: "180px" },
            { targets: 7, width: "100px" },
            { targets: 8, width: "100px" },
            { targets: 9, width: "100px" },
            { targets: 10, width: "120px" },
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
                title: 'E-IASC_User_Profile',
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

            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    Reset();
                    ResetErrorMessage();
                    UserProfileId = rowData.UserId;
                    $("#txtArmyNo").val(rowData.ArmyNo);
                    $("#txtName").val(rowData.Name);
                    $("#ddlRank").val(rowData.RankId);
                    $("#ddlArmType").val(rowData.ArmedId);

                    if (rowData.IsTokenWaiver) {
                        $("#IsTokenWaiverYes").prop("checked", true);
                        $("#spnReasonTokenWaiver").removeClass("d-none");
                    }
                    else {
                        $("#IsTokenWaiverNo").prop("checked", true);
                        $("#spnReasonTokenWaiver").addClass("d-none");
                    }

                    if (rowData.IsToken) {
                        $("#isTokenyes").prop("checked", true);
                    }
                    else {
                        $("#isTokenno").prop("checked", true);
                    }

                    if (rowData.ReasonTokenWaiver != "null") {
                        $("#txtMessage").val(rowData.ReasonTokenWaiver);
                    }
                    else {
                        $("#txtMessage").val("");
                    }

                    if (rowData.IsWithTokenApply) {
                        $("#IsWithTokenApplyyes").prop("checked", true);
                    }
                    else {
                        $("#IsWithTokenApplyno").prop("checked", true);
                    }

                    $("#btnProfileAddButton").val("Update");
                    $("#AddNewProfile").modal('show');
                    $("#exampleModalLabel").html("Edit Profile Details");
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
                            Delete(rowData.UserId);
                        }
                    });
                }
            });
        }
    });
}

function ProfileCount() {
    $.ajax({
        url: '/Account/TotalProfileCount',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {
            if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })
            }
            else {
                $("#lblTotal").html(result);
            }
        }
    });
}
function Save() {
    let param ={
            "UserId": UserProfileId,
            "ArmyNo": $("#txtArmyNo").val(),
            "Name": $("#txtName").val(),
            "RankId": $("#ddlRank").val(),
            "ArmedId": $("#ddlArmType").val(),
            "IsTokenWaiver": $('input:radio[name=IsTokenWaiver]:checked').val(),
            "ReasonTokenWaiver": $("#txtMessage").val().length > 0 ? $("#txtMessage").val() : null,
            "IsToken": $('input:radio[name=IsToken]:checked').val(),
            "IsWithTokenApply": $('input:radio[name=IsWithTokenApply]:checked').val(),
    }
    $.ajax({
        url: '/Account/SaveProfileManage',
        type: 'POST',
        data: { "request": encryptPayloadData(JSON.stringify(param)) }, //get the search string
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Profile has been saved');

                $("#AddNewProfile").modal('hide');
                //ProfileCount();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Profile has been Updated');

                $("#AddNewProfile").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {

                toastr.error('Army No. Exits!');

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
function Delete(UserId) {
    var userdata =
    {
        "UserId": UserId,

    };
    $.ajax({
        url: '/Account/DeleteProfile',
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

                    //toastr.error('This profile currently in use and cannot be deleted.');
                    Swal.fire({
                        icon: 'error',
                        title: 'Warning...',
                        text: 'This profile currently in use and cannot be deleted.',
                    })
                }
                else if (response == Success) {
                    toastr.success('Profile deleted successfully.');
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

function Reset() {
    $("#txtSearch").val("");
    UserProfileId = 0;
    $("#txtArmyNo").val("");
    $("#ddlRank").val("");
    $("#txtName").val("");
    $("#ddlArmType").val("");
    $("#IsTokenWaiverYes").prop("checked", false);
    $("#IsTokenWaiverNo").prop("checked", false);
    $("#txtMessage").val("");
    $("#isTokenyes").prop("checked", false);
    $("#isTokenno").prop("checked", false);
    $("#IsWithTokenApplyyes").prop("checked", false);
    $("#IsWithTokenApplyno").prop("checked", false);
    $("#btnProfileAddButton").val("Save");
    $("#exampleModalLabel").html("Enter Profile Details");
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#ddlRank-error").html("");
    $("#txtArmyNo-error").html("");
    $("#ddlArmType-error").html("");
    $("#IsTokenWaiver-error").html("");
    $("#txtMessage-error").html("");
    $("#IsToken-error").html("");
    $("#IsWithTokenApply-error").html("");

}