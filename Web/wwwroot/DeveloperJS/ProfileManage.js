﻿$(document).ready(function () {
    mMsater(0, "ddlRank", Rank, "");
    BindData()

    $("#AddNewProfile input[name='InitatingOffr']").click(function () {
        $("#InitatingOffr-error").html("");
    });
    $("#AddNewProfile input[name='CommandingOffr']").click(function () {
        $("#CommandingOffr-error").html("");
    });
    $("#AddNewProfile input[name='IntOffr']").click(function () {
        $("#IntOffr-error").html("");
    });

    $("#btnProfileAdd").click(function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewProfile").modal('show');
    });
    $("#btnProfileAddReset").click(function () {
        Reset();
        ResetErrorMessage();
    });
    
    $("#txtSearch").keyup(function () {
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

    ValidateRadioButton();

    if ($(formId).valid()) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be Save!",
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
function ValidateRadioButton(){
    if ($("input[type='radio'][name=InitatingOffr]:checked").length == 0) {
        $("#InitatingOffr-error").html("Initating Offr is required.");
    }
    else {
        $("#InitatingOffr-error").html("");
    }

    if ($("input[type='radio'][name=CommandingOffr]:checked").length == 0) {
        $("#CommandingOffr-error").html("Commanding Offr is required.");
    }
    else {
        $("#CommandingOffr-error").html("");
    }

    if ($("input[type='radio'][name=IntOffr]:checked").length == 0) {
        $("#IntOffr-error").html("IntOffr is required.");
    }
    else {
        $("#IntOffr-error").html("");
    }
}

function BindData() {
    var listItem = "";
    var userdata =
    {
        "Search": $("#txtSearch").val(),
        "Choice": $("input[type='radio'][name=choice]:checked").val()
    };
    $.ajax({
        url: '/Account/GetAllProfileManage',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });

                }
                else if (response.length == 0) {
                    $("#tbldata").DataTable().destroy();

                    $("#DetailBody").html(listItem);
                    memberTable = $('#tbldata').DataTable({
                        "language": {
                            "emptyTable": "No data available"
                        }
                    });


                }

                else {

                    $("#tbldata").DataTable().destroy();

                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='regId'>" + response[i].Id + "</span><span id='userId'>" + response[i].UserId + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='userId'>" + response[i].UserId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='armyNo'>" + response[i].ArmyNo + "</span></td>";
                        listItem += "<td class='align-middle'><span id='name'>" + response[i].Name + "</span></td>";
                        listItem += "<td class='align-middle'><span id='domainId'>" + response[i].DomainId + "</span></td>";
                        if (response[i].IsIO == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isIO'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isIO'>No</span></span></td>";

                        if (response[i].IsCO == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isCO'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isCO'>No</span></span></td>";

                        if (response[i].IntOffr == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isInt'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isInt'>No</span></span></td>";

                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span></td>";


                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";

                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length - 1);

                    memberTable = $('#tbldata').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        searching: false,
                        "order": [[2, "asc"]],
                        buttons: [{
                            extend: 'copy',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }, {
                            extend: 'excel',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }, {
                            extend: 'pdfHtml5',
                            orientation: 'landscape',
                            pageSize: 'LEGAL',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }]
                    });

                    memberTable.buttons().container().appendTo('#tbldata_wrapper .col-md-6:eq(0)');

                    var rows;

                    $("body").on("click", ".cls-btnedit", function () {
                        Reset();
                        ResetErrorMessage();
                        $("#txtDomainId").val($(this).closest("tr").find("#domainId").html());
                        $("#txtArmyNo").val($(this).closest("tr").find("#roleName").html());
                        $("#spnUserProfileId").html($(this).closest("tr").find("#userId").html());
                        //alert($(this).closest("tr").find("#domain_approval").html())
                        if ($(this).closest("tr").find("#domain_approval").html() == 'Verifed' ) {
                            $("#txtapprovalyes").prop("checked", true);
                        }
                        else {
                            $("#txtapprovalno").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#domain_active").html() == 'Yes') {
                            $("#txtactiveyes").prop("checked", true);
                        }
                        else {
                            $("#txtactiveno").prop("checked", true);
                        }
                        if ($(this).closest("tr").find("#regUserId").html() > 0) {
                            GetProfileByUserId($(this).closest("tr").find("#regUserId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingId").html() > 0) {
                            $("spnTrnDomainMappingId").val($(this).closest("tr").find("#regTrnDomainMappingId").html());
                            GetALLByUnitById($(this).closest("tr").find("#regTrnDomainMappingUnitId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingApptId").html() > 0) {
                            GetNameByApptId($(this).closest("tr").find("#regTrnDomainMappingApptId").html());
                        }
                        
                        $("#btnDomainAdd").val("Update");
                        $("#AddNewDomain").modal('show');
                    });
                }
            }
            else {
                $("#tbldata").DataTable().destroy();

                $("#DetailBody").html(listItem);
                memberTable = $('#tbldata').DataTable({
                    "language": {
                        "emptyTable": "No data available"
                    }
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
function Save() {
    /*  alert($('#bdaymonth').val());*/
    alert($("txtapproval").val());
    $.ajax({
        url: '/Account/SaveDomain',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegId").html(),
            "DomainId": $("#txtDomainId").val(),
            "RoleName": $("#txtRole").val(),
            "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
            "Active": $('input:radio[name=txtactive]:checked').val(),
            "UserId": $("#spnUserProfileId").html(),
            "ArmyNo": $("#txtArmyNo").val(),
            "Name": $("#txtName").val(),
            "RankId": $("#ddlRank").val(),
            "IntOffr": $('input:radio[name=txtactive]:checked').val(),
            "TrnDomainMappingId": $("#spnTrnDomainMappingId").html(),
            "ApptId": $("#spnUnitAppointmentId").html(),
            "UnitMappId": $("#spnUnitMapId").html(),
        }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Unit has been saved');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Unit has been Updated');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {

                toastr.error('Unit Name Exits!');

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
    $("#txtSearch").val("");

    $("#spnDomainRegId").html("");
    $("#txtDomainId").val("");
    $("#txtRole").val("");

    $("#spnTrnDomainMappingId").html("");
    $("#spnUnitMapId").html("");
    $("#txtUnitName").val("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblSusno").html("");

    $("#spnUserProfileId").html("");
    $("#txtArmyNo").val("");
    $("#ddlRank").val("");
    $("#txtName").val("");

    $("#spnUnitAppointmentId").html("");
    $("#txtAppointmentName").val("");

    $("#intoffsyes").prop("checked", false);
    $("#intoffsno").prop("checked", false);

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtDomainId-error").html("");
    $("#txtRole-error").html("");
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
    $("#txtName-error").html("");
    $("#ddlRank-error").html("");
    $("#txtArmyNo-error").html("");
    $("#IntOffr-error").html("");
    $("#txtAppointmentName-error").html("");
    $("#txtUnitName-error").html("");
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUnitMapId").html(data.UnitMapId);
            $("#txtUnitName").val(data.UnitName);
            $("#lblComd").html(data.ComdName);
            $("#lblCorps").html(data.CorpsName);
            $("#lblDiv").html(data.DivName);
            $("#lblBde").html(data.BdeName);
            $("#lblSusno").html(data.Sus_no + '' + data.Suffix);

        }
    });
}
function GetProfileByUserId(param1) {
    $.ajax({
        url: '/UserProfile/GetProfileByUserId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UserId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUserProfileId").html(data.UserId);
            $("#txtArmyNo").val(data.ArmyNo);
            $("#ddlRank").val(data.RankId);
            $("#txtName").val(data.Name);
            if (data.IntOffr == true) {
                $("#intoffsyes").prop("checked", true);
            }
            else {
                $("#intoffsno").prop("checked", true);
            }
        }
    });
}
function GetNameByApptId(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUnitAppointmentId").html(data.ApptId);
            $("#txtAppointmentName").val(data.AppointmentName);
        }
    });
}