$(document).ready(function () {
   
    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#iarmynopostingin").html(sessionStorage.getItem("ArmyNo"));
        GetdataPostingData(sessionStorage.getItem("ArmyNo"));

        mMsater(0, "ddlpostingReason", PostingReason, "");
    }
   
    $("#postingoutUnitName").autocomplete({


        source: function (request, response) {

            var param = { "UnitName": request.term };
            $(".spnToUserID").html(0);
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
    $("#btnPostingOut").click(function () {
        if ($("#SaveForm")[0].checkValidity()) {

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

        } else {
            $("#SaveForm")[0].reportValidity();
        }



        // 

    });
});
function Save() {

    /*  alert($('#bdaymonth').val());*/
   
    $.ajax({
        url: '/Posting/SavePoasingOut',
        type: 'POST',
        data: {
            "Id": $("#spnPostingOutID").html(),
            "BasicDetailId": $(".spnBasicDetailIdOutID").html(),
            "ReasonId": $("#ddlpostingReason").val(),
            "Authority": $("#txtAuthority").val(),
            "SOSDate": $("#txtSosDate").val(),
            "FromAspNetUsersId": $(".spnFromAspNetUsersId").html(),
            "FromUnitID": $(".spnFromUnitID").html(),
            "FromUserID": $(".spnFromUserID").html(),
            "ToAspNetUsersId": $("#ddlaspnetiserpostout").val(),
            "ToUnitID": $("#postingoutUnitId").html(),
            "ToUserID": $(".spnToUserID").html(),
            "RequestId": $(".spnRequestId").html(),
        }, //get the search string
        success: function (result) {


            if (result == DataSave) {


                toastr.success('Data has been saved');

                alert("Posting Out successfully");
                location.href = '/Posting/GetAllPostingOut';

            }
            else if (result == DataUpdate) {


                toastr.success('Data has been Updated');
                alert("Posting Out successfully");
                location.href = '/Posting/GetAllPostingOut';

            }
            else if (result == DataExists) {

                toastr.error(' Exits!');
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
                    $(".spnToUserID").html(response.UserId);
                    
                   
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
                    $("#lblStatusofInds").html('Under Process');
                else
                    $("#lblStatusofInds").html('Complete');

                $("#lblTracking").html(response.TrackingId);
                $("#pstimage").attr("src", "/WriteReadData/Photo/" + response.PhotoImagePath);
                $("#lblUnitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix+')');

                $("#lblRegdUser").html(response.Users_ArmyNo);
                $("#lblFromName").html(response.Users_RankName + ' ' + response.Users_Name );
                $("#lblFromDomainId").html(response.Users_DomainId);

                $(".spnFromAspNetUsersId").html(response.FromAspNetUsersId);
                $(".spnFromUnitID").html(response.FromUnitID);
                $(".spnFromUserID").html(response.FromUserID);
                $(".spnRequestId").html(response.RequestId);

                $(".spnBasicDetailIdOutID").html(response.BasicDetailId)

                //if ($("#RegistrationId").val() == '3' || $("#RegistrationId").val() == '7') {
                //    $("#lblunitname").html(response.Registraion);
                //} else {
                //    $("#lblunitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix + ')');
                //}







            }

        }
    });
}