    $(function () {
        if (sessionStorage.getItem("ArmyNoForFaulty") != null && sessionStorage.getItem("RequestIdForFaulty") != null) {
            $("#spnArmyNo").html(sessionStorage.getItem("ArmyNoForFaulty"));
            GetFaultyCardDataByRequestId(sessionStorage.getItem("RequestIdForFaulty"));

            //mMsater(0, "ddlpostingReason", PostingReason, "1");
        }
        $("#btnPostingOut").on("click", function () {
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
          .then(data => {
            if (data != null) {
                $("#lblCategory").html(data.ApplyFor);
                $("#lblFName").html(data.FName);
                $("#lblLName").html(data.LName == null ? "" : data.LName);
                $("#PhotoImagePath").attr("src", data.PhotoImagePath);
                $("#lblRequestId").html(data.RequestId);
                $("#lblRequestDate").html(data.RequestDate);
            }
            else {
                alert("No data found.");
            }
          })
          .catch(error => {
            alert("Error: " + error.message);
        });
    }