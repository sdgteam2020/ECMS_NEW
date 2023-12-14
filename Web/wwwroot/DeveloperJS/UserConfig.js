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
$("#btncheckarmyno").click(function () {
        if ($("#txtArmyNo").val() != "")
        {
            $("#UserId").val("");
            $("#txtName").val("");
            $("#ddlProRank").val("");
            $("#ddlProFormation").val("");
            $('#ddlProAppointment').find('option').not(':first').remove();
            $("#intoffsyes").prop("checked", false);
            $("#intoffsno").prop("checked", false);

            CheckICNumberInProfile("txtArmyNo");
        }
        else
            toastr.error('Offrs Army/Unit  No Not Blank!');
    });
function Proceed() {
    let formId = '#msform';
    $.validator.unobtrusive.parse($(formId));
    if ($(formId).valid()) {
        SaveMapping()
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

    var examdata =
    {
        "UnitId": $("#spnUnitIdMap").html(),
        "UserId": $("#UserId").val(),
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
    alert($("#UserId").html())
    var profiledata =
    {
        "UserId": $("#UserId").val(),
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
function CheckICNumberInProfile(txt) {
    $.ajax({
        url: "/UserProfile/CheckArmyNoInUserProfile",
        type: "POST",
        data: {
            "ArmyNo": $("#" + txt).val()
        },
        success: function (response, status) {

            if (response.StatusCode == 2) {
                Swal.fire({
                    icon: 'info',
                    title: response.Title,
                    text: response.Message,

                })
                $("#UserId").val(response.UserId);
                $("#txtName").val(response.Name);
                $("#ddlProRank").val(response.RankId);
                $("#ddlProFormation").val(response.FormationId);

                mMsater(response.ApptId, "ddlProAppointment", Appt, response.FormationId);
              
                if (response.IntOffr == true) {
                    $("#intoffsyes").prop("checked", true);
                }
                else {
                    $("#intoffsno").prop("checked", true);
                }

            }
            else if (response.StatusCode == 3) { 
                Swal.fire({
                    icon: 'error',
                    title: response.Title,
                    text: response.Message,

                })
            }
        }
    });  
}