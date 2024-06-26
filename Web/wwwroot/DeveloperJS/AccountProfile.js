﻿$(document).ready(function () {
    $("#btntokenrefresh").click(function () {
        
        GetTokenvalidatepersid2fawiththumbprint($("#ArmyNo").val(), "tokenmsg", "txtProfileForArmyNo","Thumbprint");
    });
    $("#IsTokenWaiverYes").on("click",function () {
        $("#spnReasonTokenWaiver").removeClass("d-none");
        $('#ReasonTokenWaiver').prop('required', true);
        $("#ReasonTokenWaiver-error").html('Reason for IACA Token Waiver is required.');
    });
    $("#IsTokenWaiverNo").on("click", function () {
        $("#spnReasonTokenWaiver").addClass("d-none");
        $('#ReasonTokenWaiver').prop('required', false);
        $('#ReasonTokenWaiver').val('');
        $("#ReasonTokenWaiver-error").html('');
    });
    $("input[name='IsTokenWaiver']").click(function () {
        $("#IsTokenWaiver-error").html("");
    });

    $("input[name='IsIO']").click(function () {
        $("#IsIO-error").html("");
    });
    $("input[name='IsCO']").click(function () {
        $("#IsCO-error").html("");
    });
    $("input[name='IsRO']").click(function () {
        $("#IsRO-error").html("");
    });
    $("input[name='IsORO']").click(function () {
        $("#IsORO-error").html("");
    });

    $("#btnUnitMapReset").click(function () {
        Reset();    
        ResetErrorMessage();
    });
    $("#btnSubmit").on("click", function () {
        Proceed();
    });
    $("#btnUnitMapsave").on("click", function () {
        ProceedUnitSave();
    });

    if ($("#spnUnitMapId").html() > 0) {
        GetNameByApptId($("#spnUnitMapId").html());
    }

    if ($("#spnUnitMapId").html() > 0) {
        GetALLByUnitById($("#spnUnitMapId").html());
    }

    $("#txtAppointmentName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                $("#spnApptIdMap").html('');
                var param = { "AppointmentName": request.term };
                $("#spnApptIdMap").html(0);
                $.ajax({
                    url: '/Master/GetALLByAppointmentName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {

                                $("#loading").addClass("d-none");
                                return { label: item.AppointmentName, value: item.ApptId };

                            }))
                        }
                        else {

                            $("#txtAppointmentName").val("");
                            $("#spnApptIdMap").html("");
                            $("#ApptId").val("");
                            alert("Appointment not found.")
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
            $("#txtAppointmentName").val(i.item.label);
            $("#spnApptIdMap").html(i.item.value);
            $("#ApptId").val(i.item.value);
            //alert(i.item.value)
         },
        appendTo: '#suggesstion-box'
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            $("#spnUnitMapId").html(''); 
            $("#lblSusno").html('');
            $("#lblPso").html('');
            $("#lblDG").html('');
            $("#lblComd").html('');
            $("#lblCorps").html('');
            $("#lblDiv").html('');
            $("#lblBde").html('');
            $("#lblFmn").html('');
            if (request.term.length > 2) {
                var param = { "UnitName": request.term };
                $("#spnUnitMapId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.Sus_no + item.Suffix +' '+ item.UnitName, value: item.UnitMapId };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            $("#spnUnitMapId").html("");
                            $("#UnitMapId").val("");
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
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {
                    $("#spnUnitMapId").html(data.UnitMapId);
                    $("#UnitMapId").val(data.UnitMapId);
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
        },
        appendTo: '#suggesstion-box'
    });

    $("#btnAddUnit").click(function () {
        if ($("#Name").val().length > 0 && $("#RankId").val() > 0) {
            Reset();
            ResetErrorMessage();
            $("#spnName").html($("#Name").val());
            $("#spnRank").html($("#RankId option:selected").text());
            $("#AddNewUnitmap").modal('show');
        }
        else {
            if ($("#Name").val().length == 0 && $("#RankId").val() == 0)
                alert("Please Select Rank & Type Name");
            else if ($("#RankId").val() == 0)
                alert("Please Select Rank.");
            else
                alert("Please Type Name");
        }

    });

    mMsater(0, "ddlCommand", Command, "");

    $('#ddlCommand').on('change', function () {
        mMsater(0, "ddlCorps", Corps, $('#ddlCommand').val());
    });

    $('#ddlCorps').on('change', function () {
        mMsaterByParent(0, "ddlDiv", Div, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
    });
    $('#ddlDiv').on('change', function () {
        mMsaterByParent(0, "ddlBde", Bde, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId
    });
    $('#ddlBde').on('change', function () {

    });

    $('input[name="UnitTyperdi"]').click(function () {
        var lst = '<option value="1">Please Select</option>';
        var val = $("input[type='radio'][name=UnitTyperdi]:checked").val();
        if (val == "1") {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();

            mMsater(0, "ddlCommand", 1, "");

            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

        }
        else if (val == "2") {

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();
            $('#ddlFmnBranch option').remove();

            mMsater(0, "ddlCommand", 1, "");
            mMsater(0, "ddlFmnBranch", FmnBranches, "");

            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
            $(".DteBranch").addClass("d-none");
        }
        else if (val == "3") {
            $(".unittype").addClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").removeClass("d-none");

            $('#ddlPSODte option').remove();
            $('#ddlDgSubDte option').remove();

            $("#ddlCommand").html(lst);
            $("#ddlCorps").html(lst);
            $("#ddlBde").html(lst);
            $("#ddlDiv").html(lst);
            $("#ddlFmnBranch").html(lst);

            mMsater(0, "ddlPSODte", PSO, "");
            mMsater(0, "ddlDgSubDte", SubDte, "");

        }
    });

    $(".allow-number").on("keypress", function (event) {
        // Allow only backspace , delete, numbers               
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 39 || event.keyCode == 37
            || (event.keyCode >= 48 && event.keyCode <= 57)) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the key press
            event.preventDefault();
        }
    });
});

function Proceed() {
    ResetErrorMessage();
    let formId = '#Profile';
    $.validator.unobtrusive.parse($(formId));

    ValidateRadioButton();

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
            if (result.isConfirmed && $("input[type='radio'][name=IsIO]:checked").length > 0 && $("input[type='radio'][name=IsCO]:checked").length > 0 && $("input[type='radio'][name=IsRO]:checked").length > 0 && $("input[type='radio'][name=IsORO]:checked").length > 0 && $("input[type='radio'][name=IsTokenWaiver]:checked").length > 0) {
                $(formId).submit();
            }
            else {
                ValidateRadioButton();
            }
        })
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        })
        return false;
    }
}
function ValidateRadioButton() {
    if ($("input[type='radio'][name=IsIO]:checked").length == 0) {
        $("#IsIO-error").html("Initating Offr is required.");
    }
    else {
        $("#IsIO-error").html("");
    }

    if ($("input[type='radio'][name=IsCO]:checked").length == 0) {
        $("#IsCO-error").html("Commanding Offr is required.");
    }
    else {
        $("#IsCO-error").html("");
    }

    if ($("input[type='radio'][name=IsRO]:checked").length == 0) {
        $("#IsRO-error").html("Record Office is required.");
    }
    else {
        $("#IsRO-error").html("");
    }

    if ($("input[type='radio'][name=IsORO]:checked").length == 0) {
        $("#IsORO-error").html("Officer Record Office is required.");
    }
    else {
        $("#IsORO-error").html("");
    }

    if ($("input[type='radio'][name=IsTokenWaiver]:checked").length == 0) {
        $("#IsTokenWaiver-error").html("Token Waiver to access Appl is required.");
    }
    else {
        $("#IsTokenWaiver-error").html("");
    }
}
function ResetErrorMessage() {
    $("#IsRO-error").html("");
    $("#IsORO-error").html("");
    $("#IsIO-error").html("");
    $("#IsCO-error").html("");
    $("#IsTokenWaiver-error").html("");
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUnitMapId").html(data.UnitMapId);
            $("#lblSusno").html(data.Sus_no + '' + data.Suffix);
            $("#txtUnitName").val(data.UnitName);
            $("#txtUnitName").prop('readonly', true);

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
            $("#spnUnitAppointmentId").html(data.ApptId);
            $("#txtAppointmentName").val(data.AppointmentName);
            $("#txtAppointmentName").prop('readonly', true);
        }
    });
}

function ProceedUnitSave() {
    let formId = '#SaveUnitWithMap';
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
                UnitSave();
            }
        })
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        })
        return false;
    }
}
function UnitSave() {
    $.ajax({
        url: '/Account/SaveUnitWithMapping',
        type: 'POST',
        data: {
            "ServiceNo": $("#spnServiceNo").html(),
            "Name": $("#spnName").html(),
            "Rank": $("#spnRank").html(),
            "MobileNo": $("#txtMobileNo").val(),
            "DialingCode": $("#txtDialingCode").val(),
            "Extension": $("#txtExtension").val(),
            "DomainId": $("#spnDomainId").html(),
            "Sus_no": $("#txtSusno").val().substring(0, 7),
            "Suffix": $("#txtSusno").val().substring(8, 7),
            "UnitName": $("#txtUnit").val(),
            "UnitType": $("input[type='radio'][name=UnitTyperdi]:checked").val(),
            "ComdId": $("#ddlCommand").val(),
            "CorpsId": $("#ddlCorps").val(),
            "DivId": $("#ddlDiv").val(),
            "BdeId": $("#ddlBde").val(),
            "PsoId": $("#ddlPSODte").val(),
            "FmnBranchID": $("#ddlFmnBranch").val(),
            "SubDteId": $("#ddlDgSubDte").val()
        }, 
        success: function (result) {
            if (result == DataSave) {
                Swal.fire({
                    icon: 'info',
                    title: 'Unit',
                    html: 'Unit has been saved.<br/>Please wait Admin for necy Approval..',
                })
                $("#AddNewUnitmap").modal('hide');
                Reset();
            }
            else if (result == DataExists) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Unit Name Exits!',
                })

            }
            else if (result == 5) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: 'Unit Name Exits!.<br/>Please wait  Admin for necy Approval..',
                })

            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    var err="";
                    for (var i = 0; i < result.length; i++) {
                        err = err + result[i][0].ErrorMessage + '<br />';
                    }
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        html: err,
                    })
                }
            }
        }
    });
}
function Reset() {
    $("#spnDomainRegId").html("0");
    $("#txtSusno").val(""); 
    $("#txtUnit").val("");
    $("#txtMobileNo").val("");
    $("#txtDialingCode").val("");
    $("#txtExtension").val("");
    $("#ddlCommand").val("");
    $("#ddlCorps").val("");
    $("#ddlDiv").val("");
    $("#ddlBde").val("");

    $("#UnitType1").prop("checked", true);
    $(".unittype").removeClass("d-none");
    $(".FmnBranch").addClass("d-none");
    $(".DteBranch").addClass("d-none");
    mMsater(0, "ddlCommand", 1, "");

    var lst = '<option value="1">Please Select</option>';
    $("#ddlFmnBranch").html(lst);
    $("#ddlPSODte").html(lst);
    $("#ddlDgSubDte").html(lst);
}
function ResetErrorMessage() {
    $("#txtSusno-error").html("");
    $("#txtUnit-error").html("");
    $("#txtMobileNo-error").html("");
    $("#txtDialingCode-error").html("");
    $("#txtExtension-error").html("");
    $("#ddlCommand-error").html("");
    $("#ddlCorps-error").html("");
    $("#ddlDiv-error").html("");
    $("#ddlBde-error").html("");
    $("#ddlFmnBranch-error").html("");
    $("#ddlPSODte-error").html("");
    $("#ddlDgSubDte-error").html("");
}