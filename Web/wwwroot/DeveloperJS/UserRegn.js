$(document).ready(function () {
    BindData("");
    AccountCount();
    $("#AddNewDomain input[name='txtapproval']").click(function () {
        $("#txtapproval-error").html("");
    });
    $("#AddNewDomain input[name='txtactive']").click(function () {
        $("#txtactive-error").html("");
    });

    $("#txtArmyNo").autocomplete({
        source: function (request, response) {
            $("#lblName").html('');
            $("#lblRank").html('');
            if (request.term.length > 2) {
                $("#spnUserProfileId").html('');
                var param = { "ArmyNo": request.term };
                $("#spnUserProfileId").html(0);
                $.ajax({
                    url: '/UserProfile/GetTopByArmyNo',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {
                            $("#loading").addClass("d-none");
                            return { label: item.ArmyNo, value: item.UserId };

                        }))
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

            $("#spnUserProfileId").html(i.item.value);
            $("#txtArmyNo").val(i.item.label);
            var param1 = { "UserId": i.item.value };
            $.ajax({
                url: '/UserProfile/GetProfileByUserId',
                method:'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype:'json',
                success: function (data) {
                    $("#lblName").html(data.Name);
                    $("#lblRank").html(data.RankName);
                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $('#txtArmyNo').keyup(function (e) {
        if (e.keyCode == 46) {
            $("#spnUserProfileId").html('0');
            $("#txtArmyNo").val('');
            $("#lblName").html('');
            $("#lblRank").html('');
        }
    });

    $("#txtSearch").keyup(function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            var ChoiceValue = $("input[type='radio'][name=choice]:checked").val();
            if (ChoiceValue == "Id") {
                var num_val = parseInt(eThis.val());
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData(ChoiceValue);
                }
            }
            else {
                BindData(ChoiceValue);
            }
        }
        else {
            alert("Select Choice");
        }
    });

    $("#btnUser").click(function () {
        BindData("User");
    });
    $("#btnMappedUser").click(function () {
        BindData("MappedUser");
    });
    $("#btnUnMappedUser").click(function () {
        BindData("UnMappedUser");
    });
    $("#btnActiveUser").click(function () {
        BindData("ActiveUser");
    });
    $("#btnInActiveUser").click(function () {
        BindData("InActiveUser");
    });
    $("#btnVerified").click(function () {
        BindData("Verified");
    });
    $("#btnNotVerifiedUser").click(function () {
        BindData("NotVerifiedUser");
    });
    $("$btnDialog").click(function () {
        BindDialog();
    });
});
function BindDialog() {
    $("#tbldatadialog").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Account/GetDataForDataTable",
            type: "post"
        },
        "columns": [
            { "data": "FullName" },
            { "data": "PhoneNumber" },
            { "data": "FaxNumber" },
            { "data": "EmailAddress" }
        ]    
    });
}
function BindData(Choice) {
    var listItem = "";
    var userdata =
    {
        "Search": $("#txtSearch").val(),
        "Choice": Choice
    };
    $.ajax({
        url: '/Account/GetAllUserRegn',
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
                        listItem += "<td class='d-none'><span id='regId'>" + response[i].Id + "</span><span id='regTrnDomainMappingId'>" + response[i].TrnDomainMappingId + "</span><span id='regTrnDomainMappingApptId'>" + response[i].TrnDomainMappingApptId + "</span><span id='regTrnDomainMappingUnitId'>" + response[i].TrnDomainMappingUnitId + "</span><span id='regUserId'>" + response[i].UserId + "</span><span id='spnAdminMsg'>" + response[i].AdminMsg + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='reg_no'>" + response[i].Id + "</span></td>";
                        listItem += "<td class='align-middle'><span id='domainId'>" + response[i].DomainId + "</span></td>";
                        if (response[i].ArmyNo != null && response[i].ArmyNo != "null")
                            listItem += "<td class='align-middle'><span id='armyNo'>" + response[i].ArmyNo + "</span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_approval'>IC No Not Mapped</span></span></td>";

                        listItem += "<td class='align-middle'><span id='roleName'>" + response[i].RoleNames + "</span></td>";



                        if (response[i].Id != null && response[i].Id != "null")
                            listItem += "<td class='align-middle'><span id='updatedOn'>" + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + "</span></td>";
                        else
                            listItem += "<td class='align-middle'>NA</td>";
                        if (response[i].Mapped == true)
                            listItem += "<td class='align-middle'><span id='btneditMapping'><button type='button' class='cls-btneditMapping btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-link'></i></button></span></td>";
                        else
                            listItem += "<td class='align-middle'><span id='btneditMapping'><button type='button' class='cls-btneditMapping btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-unlink'></i></button></span></td>";

                        if (response[i].Active == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='domain_active'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_active'>No</span></span></td>";

                        if (response[i].AdminFlag == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='domain_approval'>Verifed</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_approval'>Not Verify</span></span></td>";

                        if (response[i].Id != null && response[i].Id != "null")
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span></td>";
                        else
                            listItem += "<td class='align-middle'>NA</td>";
                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";

                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length - 1);

                    memberTable = $('#tbldata').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        searching: false,
                        "order": [[1, "asc"]],
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
                    $("body").on("click", ".cls-btneditMapping", function () {
                        ResetForMapping();
                        ResetErrorMessageForMapping();
                        $("#lblDomainIdForMapping").html($(this).closest("tr").find("#domainId").html());
                        $("#lblRoleForMapping").html($(this).closest("tr").find("#roleName").html());
                        $("#spnDomainRegIdForMapping").html($(this).closest("tr").find("#regId").html());
                        //alert($(this).closest("tr").find("#domain_approval").html())
                        if ($(this).closest("tr").find("#domain_approval").html() == 'Verifed') {
                            $("#txtapprovalyesForMapping").prop("checked", true);
                        }
                        else {
                            $("#txtapprovalnoForMapping").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#domain_active").html() == 'Yes') {
                            $("#txtactiveyesForMapping").prop("checked", true);
                        }
                        else {
                            $("#txtactivenoForMapping").prop("checked", true);
                        }
                        if ($(this).closest("tr").find("#regUserId").html() > 0) {
                            GetProfileByUserId($(this).closest("tr").find("#regUserId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingId").html() > 0) {
                            $("#spnTrnDomainMappingIdForMapping").html($(this).closest("tr").find("#regTrnDomainMappingId").html());
                            GetALLByUnitByIdForMapping($(this).closest("tr").find("#regTrnDomainMappingUnitId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingApptId").html() > 0) {
                            GetNameByApptIdForMapping($(this).closest("tr").find("#regTrnDomainMappingApptId").html());
                        }

                        $("#btnAddMapping").val("Update");
                        $("#AddMapping").modal('show');
                    });
                    $("body").on("click", ".cls-btnedit", function () {
                        Reset();
                        ResetErrorMessage();
                        $("#lblDomainId").html($(this).closest("tr").find("#domainId").html());
                        $("#lblRole").html($(this).closest("tr").find("#roleName").html());
                        $("#spnDomainRegId").html($(this).closest("tr").find("#regId").html());
                        if ($(this).closest("tr").find("#spnAdminMsg").html() != "null") {
                            $("#txtadminmessage").val($(this).closest("tr").find("#spnAdminMsg").html());
                        }
                        else {
                            $("#txtadminmessage").val("");
                        }

                        //alert($(this).closest("tr").find("#domain_approval").html())
                        if ($(this).closest("tr").find("#domain_approval").html() == 'Verifed') {
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

                        if ($(this).closest("tr").find("#regTrnDomainMappingId").html() > 0) {
                            GetALLByUnitById($(this).closest("tr").find("#regTrnDomainMappingUnitId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingApptId").html() > 0) {
                            GetNameByApptId($(this).closest("tr").find("#regTrnDomainMappingApptId").html());
                        }

                        $("#btnDomainFlag").val("Update");
                        $("#AddDomainFlag").modal('show');
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
function ProceedForMapping() {
    ResetErrorMessageForMapping();
    ValidateMappingInput();
    let formId = '#SaveMapping';
    $.validator.unobtrusive.parse($(formId));

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
                SaveMapping();
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
function SaveMapping() {
    //alert($("#spnDomainRegId").html());
    $.ajax({
        url: '/Account/SaveMapping',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegIdForMapping").html(),
            "TDMId": $("#spnTrnDomainMappingIdForMapping").html(),
            "UserId": $("#spnUserProfileId").html(),
            "ArmyNo": $("#txtArmyNo").val(),

        }, //get the search string
        success: function (response) {
            var obj = jQuery.parseJSON(response);
            if (obj.Result == true)
            {
                toastr.success(obj.Message);

                $("#AddMapping").modal('hide');
                AccountCount();
                BindData("");
                ResetForMapping();
                ResetErrorMessageForMapping();
            }
            else if (obj.Result == false)
            {
                toastr.error(obj.Message);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: obj.Message,

                })
            }
            else if (obj.Result == false && obj.Message.length > 1)
            {
                for (var i = 0; i < obj.Message.length; i++) {
                        toastr.error(result[i][0].Message)
                    }
            }
        }
    });
}
function ValidateMappingInput() {

    var UserProfileId = $("#spnUserProfileId").html();

    if ((UserProfileId == 0 || UserProfileId == '') && $("#txtArmyNo").val().length > 0) {
        $("#txtArmyNo").val('');
        $("#txtArmyNo-error").html("ArmyNo is invalid.");
        toastr.error('ArmyNo is invalid.');
    }
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
            $("#lblRank").html(data.RankName);
            $("#lblName").html(data.Name);
        }
    });
}
function GetALLByUnitByIdForMapping(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        success: function (data) {
            $("#lblUnitNameForMapping").html(data.UnitName);
            $("#lblSusnoForMapping").html(data.Sus_no + '' + data.Suffix);


            if (data.UnitType == 1) {
                $("#lblComdForMapping").html(data.ComdName);
                $("#lblCorpsForMapping").html(data.CorpsName);
                $("#lblDivForMapping").html(data.DivName);
                $("#lblBdeForMapping").html(data.BdeName);
                $("#lblM1").addClass("d-none");
                $("#lblM2").addClass("d-none");
                $("#lblM3").removeClass("d-none");
                $("#lblM4").removeClass("d-none");
                $("#lblM5").removeClass("d-none");
                $("#lblM6").removeClass("d-none");
                $("#lblM7").addClass("d-none");
            }
            else if (data.UnitType == 2) {
                $("#lblComdForMapping").html(data.ComdName);
                $("#lblCorpsForMapping").html(data.CorpsName);
                $("#lblDivForMapping").html(data.DivName);
                $("#lblBdeForMapping").html(data.BdeName);
                $("#lblFmnForMapping").html(data.BranchName);
                $("#lblM1").addClass("d-none");
                $("#lblM2").addClass("d-none");
                $("#lblM3").removeClass("d-none");
                $("#lblM4").removeClass("d-none");
                $("#lblM5").removeClass("d-none");
                $("#lblM6").removeClass("d-none");
                $("#lblM7").removeClass("d-none");
            }
            else if (data.UnitType == 3) {
                $("#lblPsoForMapping").html(data.PSOName);
                $("#lblDGForMapping").html(data.SubDteName);
                $("#lblM1").removeClass("d-none");
                $("#lblM2").removeClass("d-none");
                $("#lblM3").addClass("d-none");
                $("#lblM4").addClass("d-none");
                $("#lblM5").addClass("d-none");
                $("#lblM6").addClass("d-none");
                $("#lblM7").addClass("d-none");
            }


        }
    });
}
function GetNameByApptIdForMapping(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        success: function (data) {
            $("#lblAppointmentNameForMapping").html(data.AppointmentName);
        }
    });
}
function ResetForMapping() {
    $("#txtSearch").val("");

    $("#spnDomainRegIdForMapping").html("0");
    $("#lblDomainIdForMapping").html("");
    $("#lblRoleForMapping").html("");

    $("#spnTrnDomainMappingIdForMapping").html("");
    $("#lblUnitNameForMapping").html("");
    $("#lblSusnoForMapping").html("");
    $("#lblPsoForMapping").html("");
    $("#lblDGForMapping").html("");
    $("#lblComdForMapping").html("");
    $("#lblCorpsForMapping").html("");
    $("#lblDivForMapping").html("");
    $("#lblBdeForMapping").html("");
    $("#lblFmnForMapping").html("");


    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#lblRank").html("");
    $("#lblName").html("");

    $("#lblAppointmentNameForMapping").html("");

    $("#txtapprovalyesForMapping").prop("checked", false);
    $("#txtapprovalnoForMapping").prop("checked", false);

    $("#txtactiveyesForMapping").prop("checked", false);
    $("#txtactivenoForMapping").prop("checked", false);
}
function ResetErrorMessageForMapping() {
    $("#txtArmyNo-error").html("");
}

function Proceed() {
    ResetErrorMessageForMapping();
    let formId = '#UpdateDomainFlag';
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
                UpdateDomainFlag();
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
function UpdateDomainFlag() {
    $.ajax({
        url: '/Account/UpdateDomainFlag',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegId").html(),
            "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
            "Active": $('input:radio[name=txtactive]:checked').val(),
            "AdminMsg": $('#txtadminmessage').val().length > 0 ? $('#txtadminmessage').val() : null,
        }, 
        success: function (result) {
            if (result == DataUpdate) {
                toastr.success('Domain Flag has been Updated');

                $("#AddDomainFlag").modal('hide');
                AccountCount();
                BindData("");
                Reset();
                ResetErrorMessage();
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
                        toastr.error(result[i][0].Message)
                    }


                }


            }
        }
    });
}
function ValidateRadioButton() {
    if ($("input[type='radio'][name=txtapproval]:checked").length == 0) {
        $("#txtapproval-error").html("Approval is required.");
    }
    else {
        $("#txtapproval-error").html("");
    }

    if ($("input[type='radio'][name=txtactive]:checked").length == 0) {
        $("#txtactive-error").html("Active is required.");
    }
    else {
        $("#txtactive-error").html("");
    }
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        success: function (data) {
            $("#lblUnitName").html(data.UnitName);
            $("#lblSusno").html(data.Sus_no + '' + data.Suffix);
            if (data.UnitType == 1) {
                $("#lblComd").html(data.ComdName);
                $("#lblCorps").html(data.CorpsName);
                $("#lblDiv").html(data.DivName);
                $("#lblBde").html(data.BdeName);
                $("#lbl1").addClass("d-none");
                $("#lbl2").addClass("d-none");
                $("#lbl3").removeClass("d-none");
                $("#lbl4").removeClass("d-none");
                $("#lbl5").removeClass("d-none");
                $("#lbl6").removeClass("d-none");
                $("#lbl7").addClass("d-none");
            }
            else if (data.UnitType == 2) {
                $("#lblComd").html(data.ComdName);
                $("#lblCorps").html(data.CorpsName);
                $("#lblDiv").html(data.DivName);
                $("#lblBde").html(data.BdeName);
                $("#lblFmn").html(data.BranchName);
                $("#lbl1").addClass("d-none");
                $("#lbl2").addClass("d-none");
                $("#lbl3").removeClass("d-none");
                $("#lbl4").removeClass("d-none");
                $("#lbl5").removeClass("d-none");
                $("#lbl6").removeClass("d-none");
                $("#lbl7").removeClass("d-none");
            }
            else if (data.UnitType == 3) {
                $("#lblPso").html(data.PSOName);
                $("#lblDG").html(data.SubDteName);
                $("#lbl1").removeClass("d-none");
                $("#lbl2").removeClass("d-none");
                $("#lbl3").addClass("d-none");
                $("#lbl4").addClass("d-none");
                $("#lbl5").addClass("d-none");
                $("#lbl6").addClass("d-none");
                $("#lbl7").addClass("d-none");
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
            $("#lblAppointmentName").html(data.AppointmentName);
        }
    });
}
function Reset() {
    $("#txtSearch").val("");

    $("#spnDomainRegId").html("0");
    $("#txtadminmessage").val("");
    $("#lblDomainId").html("");
    $("#lblRole").html("");

    $("#lblUnitName").html("");
    $("#lblSusno").html("");
    $("#lblPso").html("");
    $("#lblDG").html("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblFmn").html("");


    $("#lblAppointmentName").html(""); 

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
}
function AccountCount() {
    $.ajax({
        url: "/Account/AccountCount",
        type: "POST",
        success: function (response, status) {
            $("#lblUser").html(response.User);
            $("#lblActiveUser").html(response.ActiveUser);
            $("#lblInActiveUser").html(response.InActiveUser);
            $("#lblMappedUser").html(response.MappedUser);
            $("#lblUnMappedUser").html(response.UnMappedUser);
            $("#lblVerifiedUser").html(response.VerifiedUser);
            $("#lblNotVerifiedUser").html(response.NotVerifiedUser);
        }
    });
}