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
    });
    function Save() {

        $.ajax({
            url: '/BasicDetail/SaveFaultyCardRequest',
            type: 'POST',
            data: {
                "TrnFaultyCardId": $("#spnTrnFaultyCardId").html(),
                "RequestId": $("#spnRequestId").html(),
                "RemarksIds": $("#ddlFaultyRemark").val(),
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
         .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! Status: ${res.status}`);
            }
            return res.json();
         })
            .then(response => {
                if (response != null) {
                    $("#lblvpFaultyNameAsPerRecord").html(response.NameAsPerRecord);
                    $("#lblvpFaultyFName").html(response.FName);
                    $("#lblvpFaultyLName").html(response.LName == null ? "" : response.LName);
                    $("#lblvpFaultyCategory").html(response.ApplyFor);
                    $("#lblvpFaultyRank").html(response.RankName);
                    $("#lblvpFaultyarm").html(response.ArmedName);
                    $("#lblvpFaultyArmyNo").html(response.ModifiedServiceNo);
                    $("#lblvpFaultyMarks").html(response.IdenMark1);
                    $("#lblvpFaultydob").html(DateFormateMMMM_dd_yyyy(response.DOB));
                    $("#lblvpFaultyheight").html(response.Height);
                    $("#lblvpFaultyadhar").html(response.AadhaarNo.replace(/\d(?=\d{4})/g, "X"));
                    $("#lblvpFaultyBloodGroup").html(response.BloodGroup);
                    $("#lblvpFaultypoi").html(response.PlaceOfIssue);
                    $("#lblvpFaultydoi").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                    $("#lblvpFaultyissuA").html(response.IssuingAuthorityName);
                    $("#lblvpFaultydateo").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                    $("#lblvpFaultyaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);

                    $("#vpFaultyPhotoImagePath").attr("src", response.PhotoImagePath);
                    $("#lblvpFaultyRequestId").html(response.RequestId);
                    $("#spnvpFaultyRequestId").html(response.RequestId);
                    $("#lblvpFaultyRequestDate").html(response.RequestDate);
            }
            else {
                alert("No data found.");
            }
          })
          .catch(error => {
            alert("Error: " + error.message);
        });
    }