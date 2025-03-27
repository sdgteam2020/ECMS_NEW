    $(function () {
        if (sessionStorage.getItem("ArmyNoForFaulty") != null && sessionStorage.getItem("RequestIdForFaulty") != null) {
            $("#spnArmyNo").html(sessionStorage.getItem("ArmyNoForFaulty"));
            GetFaultyCardDataByRequestId(sessionStorage.getItem("RequestIdForFaulty"));
            var RemarkTypeID = [5];
            GetRemarks("ddlFaultyRemark", 0, RemarkTypeID);
            mMsater(0, "ddlStage", FaultyStage, "");
            $('.select2').select2({
                placeholder: "Please select a Reason",
                allowClear: true,
                closeOnSelect: false // Only needed for multi-select
            });
        }
        $("#btnSubmit").on("click", function () {
            Proceed();
        });
    });
function Proceed() {
    //ResetErrorMessage();
    if ($("#ddlFaultyRemark").val().length == 0 ) {
        toastr.error('Reason is required.');
        return false;
    }

    let formId = '#SaveFaultyCardRequest';
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
    function Save() {
        var FaultyRemarkIds = "" + $("#ddlFaultyRemark").val() + "";
        $.ajax({
            url: '/BasicDetail/SaveFaultyCardRequest',
            type: 'POST',
            data: {
                "TrnFaultyCardId": $("#spnTrnFaultyCardId").html(),
                "RequestId": $("#spnvpFaultyRequestId").html(),
                "RemarksIds": $("#ddlFaultyRemark").val().length > 0 ? FaultyRemarkIds : null,
                "OtherRemark": $("#txtOtherRemark").val(),
                "FaultyStageId": $("#ddlStage").val(),
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
    function GetFaultyCardDataByRequestId(RequestId) {
        let param = new URLSearchParams({ RequestId: RequestId });

        fetch('/BasicDetail/GetFaultyCardDataByRequestId', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        })
            .then(response => response.text())
            .then(html =>{
                document.getElementById("partialContainerBD").innerHTML = html;
                //$("#lblpvPaperIcardNo").html(response.PaperIcardNo);
                //$("#lblpvNameAsPerRecord").html(response.NameAsPerRecord);
                //$("#lblpvFName").html(response.FName);
                //$("#lblpvLName").html(response.LName == null ? "" : response.LName);
                //$("#lblpvCategory").html(response.ApplyFor);
                //$("#lblpvRank").html(response.RankName);
                //$("#lblpvarm").html(response.ArmedName);
                //$("#lblpvArmyNo").html(response.ModifiedServiceNo);
                //$("#lblpvMarks").html(response.IdenMark1);
                //$("#lblpvdob").html(DateFormateMMMM_dd_yyyy(response.DOB));
                //$("#lblpvheight").html(response.Height);
                //$("#lblpvadhar").html(response.AadhaarNo.replace(/\d(?=\d{4})/g, "X"));
                //$("#lblpvBloodGroup").html(response.BloodGroup);
                //$("#lblpvpoi").html(response.PlaceOfIssue);
                //$("#lblpvdoi").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                //$("#lblpvissuA").html(response.IssuingAuthorityName);
                //$("#lblpvdateo").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                //$("#lblpvaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);

                //$("#pvPhotoImagePath").attr("src", response.PhotoImagePath);
                //$("#lblvpFaultyRequestId").html(response.RequestId);
                //$("#spnvpFaultyRequestId").html(response.RequestId);
                //$("#lblvpFaultyRequestDate").html(DateFormateMMMM_dd_yyyy(response.RequestDate));

          })
          .catch(error => {
            alert("Error: " + error.message);
        });
    }