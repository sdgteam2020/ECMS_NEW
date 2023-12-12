$(document).ready(function () {
    mMsater(0, "ddlProFormation", Formation, "");
    mMsater(0, "ddlProRank", Rank, "");

    $('#ddlProFormation').on('change', function () {
        mMsater(0, "ddlProAppointment", Appt, $('#ddlProFormation').val());

    });
    
    //$("#btnConfigsave").click(function () {
    //    if ($("#txtArmyNo").val() != "" && $("#spnUnitIdMap").html() != "0")
    //    {
    //       SaveMapping();
    //    }
    //    else
    //        toastr.error('Offrs Army/Unit  No Not Blank!'); 
    //});
    $("#txtProUnit").autocomplete({


        source: function (request, response) {
            if (request.term.length > 2) {
                var param = { "UnitName": request.term };
                $("#spnUnitIdMap").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.UnitName, value: item.UnitMapId };

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
            $("#txtProUnit").val(i.item.label);
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {


                    $("#spnUnitIdMap").html(data.UnitMapId);
                    $("#lblProComd").html(data.ComdName);
                    $("#lblProCorps").html(data.CorpsName);
                    $("#lblProDiv").html(data.DivName);
                    $("#lblPrBde").html(data.BdeName);
                    $("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

                }
            });
        },
        appendTo: '#suggesstion-box'
    });
});
$("#btnfetchtoken").click(function () {
    if ($("#txtArmyNo").val() != "") {
        GetTokenDetailsFromUserConfig("FetchUniqueTokenDetails", "txtArmyNo");
    }
    else
        toastr.error('Offrs Army No Not Blank!');
});
function Proceed() {
    let formId = '#msform';
    $.validator.unobtrusive.parse($(formId));
    if ($(formId).valid()) {
        isResult = CheckICNumberInProfile("txtArmyNo");
        if (isResult == true) {
            SaveMapping();
        }
        else {
            return false;
        }

    }
    else {
        toastr.error('Please fill required field.');
        return false;
    }
}
function SaveMapping() {

    var examdata =
    {
        "UnitId": $("#spnUnitIdMap").html(),
        "ICNO": $("#txtArmyNo").val(),
    };

    $.ajax({
        url: '/ConfigUser/SaveMapping',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {

                if (response == 1 || response == 2)
                {
                    SaveProfile();
                    Gotodashboard($("#txtArmyNo").val());
                }

                else
                    toastr.error('Already Mapping!');
                //else {
                //    $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">This ICNO Profile Not Available</span>.</div>');

                //}
            }
        }
    });
}
function SaveProfile() {
    var profiledata =
    {
        "Name": $("#txtName").val(),
        "ArmyNo": $("#txtArmyNo").val(),
        "RankId": $("#ddlProRank").val(),
        "ApptId": $("#ddlProAppointment").val(),
        "IntOffr": $("#intoffsyes").prop("checked")
    };
    $.ajax({
        url: '/UserProfile/SaveUserProfile',
        data: profiledata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (result) {

            if (result == DataSave) {
                toastr.success('User has been saved');
            }
            else if (result == DataExists) {

                toastr.error('Army No Exits!');

            }
            else if (result == IncorrectData) {

                toastr.error('Incorrect Data!');

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
function Gotodashboard(ArmyNo) {

    var examdata =
    {
        "ICNO": ArmyNo,

    };
    $.ajax({
        url: '/ConfigUser/GotoDashboard',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {

                if (response == 1)
                    window.location.href = "/Home/Dashboard";
                else {
                    $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">This ICNO Profile Not Available</span>.</div>');

                }
            }
        }
    });
}
function GetTokenDetailsFromUserConfig(ApiId, txt) {

    var examdata =
    {
        "ApiName": ApiId,

    };
    $.ajax({
        url: '/ConfigUser/GetTokenDetails',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {
                if (response == '') {
                    $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
                    $("#" + txt).val("");
                }

                else if (response[0].Status == '200') {///&& response[0].TokenValid=='true'
                    // $("#error-msg").html(response.message);
                    var datef2 = new Date();
                    if (response[0].ValidTo >= datef2) {
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Token Expired</span>.</div>');
                        $("#" + txt).val("");

                    }
                    else {
                        $("#tokenmsg").html('<div class="alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');
                        if (response[0].ArmyNo = "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996") {
                            
                            isResult = CheckICNumberInProfile(txt);
                            if (isResult) {
                                $("#" + txt).val("IC-00100");
                            }
                            //$("#" + txt).val("IC-00002");
                        }
                    }


                }
                else {
                    if (response[0].Status == '404') {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response[0].Remarks + '</span>.</div>');
                        $("#" + txt).val("");
                        $("#" + txt).val("IC-00005");
                    }


                }
            }
            else {
                $("#tokenmsg").html(errormsg001);
                return 0;
            }
        },
        error: function (result) {
            $("#tokenmsg").html(errormsg002);
            return 0;
        }
    });
}
function CheckICNumberInProfile(txt) {
    $.ajax({
        url: "/UserProfile/CheckArmyNoInUserProfile",
        type: "POST",
        data: {
            "ArmyNo": $("#" + txt).val()
        },
        success: function (response, status) {
            alert(JSON.stringify(response));
            if (response.success == false) {
                toastr.error(response.message);
                return false;
            }
            else {
                return true;
            }
        }
    });   
}