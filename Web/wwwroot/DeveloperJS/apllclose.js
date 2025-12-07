var applyforId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    $("#btnApplicationClose").on("click", function () {
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
    });
    return new Promise((resolve, reject) => {
        fetch('/BasicDetail/DataRecForGetSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
                'RequestVerificationToken': globalThis.RequestVerificationToken
            }
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    let ArmyNo = response.Value.ArmyNo;

                    $("#iarmynopostingin").html(ArmyNo);
                    GetdataPostingData(ArmyNo);

                    mMsater(0, "ddlpostingReason", PostingReason, "2");

                    resolve(response);
                } else {
                    toastr.error("Failed to Fetch Session Value: " + response.Message);
                    reject(new Error(response.Message));
                }
            })
            .catch((error) => {
                toastr.error("Failed to Fetch Session Value : " + response.Message);
                reject(new Error("Failed to Fetch Session Value : " + error.message));
            });
    });
});
function Save() {
 
    $.ajax({
        url: '/Posting/SaveApplicationClose',
        type: 'POST',
        data: {
            "ReasonId": $("#ddlpostingReason").val(),
            "Authority": $("#txtAuthority").val(),
            "RequestId": $(".spnRequestId").html(),
            "Remarks": $("#txtremarks").val(),
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                toastr.success(result.Message);
                if (applyforId == 1) {
                    window.location.href = "/Posting/AppCloseList/MQ==";
                }
                else {
                    window.location.href = "/Posting/AppCloseList/?Id=MQ==&jcoor=SmNvL09ycw==";
                }
            }
             else {
                toastr.error(result.Message);
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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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

                $("#lblApplId").html(response.RequestId);
                $("#pstimage").attr("src", response.PhotoImagePath);
                $("#lblUnitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix+')');

                $("#lblRegdUser").html(response.Users_ArmyNo);
                $("#lblFromName").html(response.Users_RankName + ' ' + response.Users_Name );
                $("#lblFromDomainId").html(response.Users_DomainId);

        
                $(".spnRequestId").html(response.RequestId);



            }

        }
    });
}