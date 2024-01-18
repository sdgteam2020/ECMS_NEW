$(document).ready(function () {
   
    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#iarmynopostingin").html(sessionStorage.getItem("ArmyNo"));
        GetdataPostingData(sessionStorage.getItem("ArmyNo"));
    }
   
    $("#postingoutUnitName").autocomplete({


        source: function (request, response) {

            var param = { "UnitName": request.term };
            
            $("#postingoutUnitId").html(0);
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
        },
        select: function (e, i) {
            e.preventDefault();
            $("#postingoutUnitName").val(i.item.label);
            $("#postingoutUnitId").html(i.item.value);
            
            GetAllOffsByUnitId("ddlaspnetiserpostout", 0, i.item.value)
        },
        appendTo: '#suggesstion-box'
    });

    $("#ddlaspnetiserpostout").change(function () {
        GetByArmyNo($("#ddlaspnetiserpostout").val());
    });
});
function GetByArmyNo(userid) {

    var userdata =
    {
        "userid": userid,

    };
    $.ajax({
        url: '/UserProfile/GetByArmyNoOrAspnetuserId',
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
                else if (response == 0) {

                }

                else {


                  
                    
                    $("#lbltoAppt").html(response.AppointmentName);
                   
                    $("#lblToName").html(response.Name);
                    
                    $("#lblToDomainId").html(response.DomainId);
                   

                }
            }

        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function GetdataPostingData(ArmyNo) {
    $.ajax({
        url: "/Posting/GetPostingIn",
        type: "POST",
        data: {
            "ArmyNo": ArmyNo
        },
        success: function (response, status) {
            if (response != null) {
                $("#lblCategory").html(response.ApplyFor);
                $("#lblAppt").html(response.Users_AppointmentName);
                $("#lblName").html(response.Name);
                if (response.Status == 'False')
                    $("#lblStatusofInds").html('User Process');
                else
                    $("#lblStatusofInds").html('Complete');

                $("#lblTracking").html(response.TrackingId);
                $("#pstimage").attr("src", "/WriteReadData/Photo/" + response.PhotoImagePath);
                $("#lblUnitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix+')');

                $("#lblRegdUser").html(response.Users_RankName + ' ' + response.Users_Name + ' (' + response.Users_ArmyNo + ') (' + response.Users_DomainId + ')');

                //if ($("#RegistrationId").val() == '3' || $("#RegistrationId").val() == '7') {
                //    $("#lblunitname").html(response.Registraion);
                //} else {
                //    $("#lblunitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix + ')');
                //}







            }

        }
    });
}