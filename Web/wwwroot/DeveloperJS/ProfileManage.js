//const { debug } = require("util");

var table; // Declare table variable outside the function to preserve the instance
$(function () {
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
    $("#tbldata").DataTable().destroy();
    table = $("#tbldata").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
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
                let response = await fetch("/Account/GetAllProfileManage", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                $("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables
                

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            { data: "UserId", name: "UserId", visible: false },
            // Serial number column
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "UserId", name: "UserId" },
            { data: "DomainId", name: "DomainId" },
            { data: "ArmyNo", name: "ArmyNo" },
            { data: "RankAbbreviation", name: "RankAbbreviation" },
            { data: "Name", name: "Name" },
            { data: "ArmedName", name: "ArmedName" },
            // Display user-friendly value for IsVerify
            {
                data: "IsTokenWaiver",
                name: "IsTokenWaiver",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>" ;
                }
            },
            {
                data: "IsToken",
                name: "IsToken",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            {
                data: "IsWithTokenApply",
                name: "IsWithTokenApply",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            // Additional column for Edit action
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    return "<span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>";
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search IC No" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
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
        drawCallback: function (settings) {
            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    Reset();
                    ResetErrorMessage();

                    $("#spnUserProfileId").html(rowData.UserId);
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
    $.ajax({
        url: '/Account/SaveProfileManage',
        type: 'POST',
        data: {
            "UserId": $("#spnUserProfileId").html(),
            "ArmyNo": $("#txtArmyNo").val(),
            "Name": $("#txtName").val(),
/*            "MobileNo": $("#txtMobileNo").val(),*/
            "RankId": $("#ddlRank").val(),
            "ArmedId": $("#ddlArmType").val(),
            "IsTokenWaiver": $('input:radio[name=IsTokenWaiver]:checked').val(),
            "ReasonTokenWaiver": $("#txtMessage").val().length > 0 ? $("#txtMessage").val() : null,
            "IsToken": $('input:radio[name=IsToken]:checked').val(),
            "IsWithTokenApply": $('input:radio[name=IsWithTokenApply]:checked').val(),

        }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Profile has been saved');

                $("#AddNewProfile").modal('hide');
                ProfileCount();
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

    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#ddlRank").val("");
    $("#txtName").val("");
/*    $("#txtMobileNo").val("");*/
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
/*    $("#txtMobileNo-error").html("");*/
    $("#ddlArmType-error").html("");
    $("#IsTokenWaiver-error").html("");
    $("#txtMessage-error").html("");
    $("#IsToken-error").html("");
    $("#IsWithTokenApply-error").html("");

}