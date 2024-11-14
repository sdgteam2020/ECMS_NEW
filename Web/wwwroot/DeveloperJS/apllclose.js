var applyforId = 0;
$(document).ready(function () {
   
    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#iarmynopostingin").html(sessionStorage.getItem("ArmyNo"));
        GetdataPostingData(sessionStorage.getItem("ArmyNo"));

        mMsater(0, "ddlpostingReason", PostingReason, "2");
    }
   
   
    $("#btnApplicationClose").on("click",function () {
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
        url: '/Posting/SaveApplicationClose',
        type: 'POST',
        data: {
            "Id": $("#spnPostingOutID").html(),
            "BasicDetailId": $(".spnBasicDetailIdOutID").html(),
            "ReasonId": $("#ddlpostingReason").val(),
            "Authority": $("#txtAuthority").val(),
            "RequestId": $(".spnRequestId").html(),
            "Remarks": $("#txtremarks").val(),
        }, //get the search string
        success: function (result) {

            if (result == DataSave) {
                toastr.success('Data has been saved');
                if (applyforId == 1) {
                    window.location.href = "/Posting/AppCloseList/MQ==";
                }
                else {
                    window.location.href = "/Posting/AppCloseList/?Id=MQ==&jcoor=SmNvL09ycw==";
                }
            }
            else if (result == DataUpdate) {
                toastr.success('Data has been Updated');
                if (applyforId == 1) {
                    window.location.href = "/Posting/AppCloseList/MQ==";
                }
                else {
                    window.location.href = "/Posting/AppCloseList/?Id=MQ==&jcoor=SmNvL09ycw==";
                }
            }
            else if (result == DataExists) {

                toastr.error('Appl Allready Closed!');
            }
            else if (result == IncorrectData) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Input!',

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
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }


            }
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
                applyforId = response.ApplyForId;
                $("#lblCategory").html(response.ApplyFor);
                $("#lblAppt").html(response.Users_AppointmentName);
                $("#lblFName").html(response.FName);
                $("#lblLName").html(response.LName);
                if (response.StatusId == 1)
                    $("#lblStatusofInds").html('Under Process');
                else if (response.StatusId == 2)
                    $("#lblStatusofInds").html('Complete');
                else if (response.StatusId == 3)
                    $("#lblStatusofInds").html('Closed');

                $("#lblTracking").html(response.TrackingId);
                $("#pstimage").attr("src", "/WriteReadData/Photo/" + response.PhotoImagePath);
                $("#lblUnitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix+')');

                $("#lblRegdUser").html(response.Users_ArmyNo);
                $("#lblFromName").html(response.Users_RankName + ' ' + response.Users_Name );
                $("#lblFromDomainId").html(response.Users_DomainId);

        
                $(".spnRequestId").html(response.RequestId);

                $(".spnBasicDetailIdOutID").html(response.BasicDetailId)


            }

        }
    });
}