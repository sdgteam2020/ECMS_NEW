$(function () {
        var dtToday = new Date();

        var month = dtToday.getMonth() + 1;
        var day = dtToday.getDate();
        var year = dtToday.getFullYear() + 1;

        if (month < 10)
            month = '0' + month.toString();
        if (day < 10)
            day = '0' + day.toString();

        var minDate = dtToday.getFullYear() + '-' + month + '-' + day;
        var maxDate = year + '-' + month + '-' + day;

        $('#txtSosDate').attr('min', minDate);
        $('#txtSosDate').attr('max', maxDate);

        $('#txtSosDate').on('change', function () {
            var dtToday = new Date();

            var month = dtToday.getMonth() + 1;
            var day = dtToday.getDate();
            var year = dtToday.getFullYear() + 1;

            if (month < 10)
                month = '0' + month.toString();
            if (day < 10)
                day = '0' + day.toString();
            var minDate = dtToday.getFullYear() + '-' + month + '-' + day;
            var maxDate = year + '-' + month + '-' + day;
            $('#txtSosDate').attr('min', minDate);
            $('#txtSosDate').attr('max', maxDate);
    });

    $('#txtSosDate').on('keydown', (e) => {
        e.preventDefault();
        return false;
    });
   
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
            
            GetAllOffsByUnitId("ddlaspnetiserpostout", 0, i.item.value,0,0,0,0)
        },
        appendTo: '#suggesstion-box'
    });

    var $dropdown = $("#ddlaspnetiserpostout");

    $dropdown.on("change", function () {
        var selectedValue = $dropdown.val();  // Cache the value
        if (selectedValue !== "") {  // Check if the value is not empty
            GetByArmyNo(selectedValue);  // Call the function with the value
        }
    });

    $("#btnPostingOut").on("click", function () {
        if ($("#txtDispatchDate").val() != ''){
            if ($("#txtRefNo").val() == '') {
                toastr.error('Please enter reference number!');
                return;
            }
        }


        if ($("#SaveForm")[0].checkValidity()) {
            

            if ($("#postingoutUnitId").html() == $(".spnFromUnitID").html())
            {
                toastr.error("From Unit And To Unit Cannot Be Same!");
                return;
            }

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

    return new Promise((resolve, reject) => {
        fetch('/BasicDetail/DataRecForGetSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
            }
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    let ArmyNo = response.Value.ArmyNo;

                    $("#iarmynopostingin").html(ArmyNo);
                    GetdataPostingData(ArmyNo);

                    mMsater(0, "ddlpostingReason", PostingReason, "1");

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
    const trnFwdId = parseInt($(".spnTrnFwdId").html());
    $.ajax({
        url: '/Posting/SavePoasingOut',
        type: 'POST',
        data: {
            "Id": $("#spnPostingOutID").html(),
            "BasicDetailId": $(".spnBasicDetailIdOutID").html(),
            "ReasonId": $("#ddlpostingReason").val(),
            "Authority": $("#txtAuthority").val(),
            "SOSDate": convertToISOWithTime($("#txtSosDate").val()),
            "FromAspNetUsersId": $(".spnFromAspNetUsersId").html(),
            "FromUnitID": $(".spnFromUnitID").html(),
            "FromUserID": $(".spnFromUserID").html(),
            "ToAspNetUsersId": $("#ddlaspnetiserpostout").val(),
            "ToUnitID": $("#postingoutUnitId").html(),
            "ToUserID": $(".spnToUserID").html(),
            "RequestId": $(".spnRequestId").html(),
            "TrnFwdId": trnFwdId > 0 ? trnFwdId : null,
            "DispatchedOn": formatDateToSqlString($("#txtDispatchDate").val()),
            "RefNo": $("#txtRefNo").val(),
        }, //get the search string
        success: function (result) {


            if (result == DataSave) {


                toastr.success('Data has been saved');

                alert("Posting Out successfully");
                location.href = '/Home/RequestDashboard/UG9zdGluZyBPdXQ=';

            }
            else if (result == DataUpdate) {


                toastr.success('Data has been Updated');
                alert("Posting Out successfully");
                location.href = '/Home/RequestDashboard/UG9zdGluZyBPdXQ=';

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
                $("#lblFName").html(response.FName);
                $("#lblLName").html(response.LName == null ? "" : response.LName);
                if (response.Status == 'False')
                    $("#lblStatusofInds").html('Under Process');
                else
                    $("#lblStatusofInds").html('Complete');

                $("#lblApplId").html(response.RequestId);
                $("#pstimage").attr("src", response.PhotoImagePath);
                $("#lblUnitname").html(response.UnitName + ' (' + response.Sus_no + '' + response.Suffix+')');

                $("#lblRegdUser").html(response.Users_ArmyNo);
                $("#lblFromName").html(response.Users_RankName + ' ' + response.Users_Name );
                $("#lblFromDomainId").html(response.Users_DomainId);

                $(".spnFromAspNetUsersId").html(response.FromAspNetUsersId);
                $(".spnFromUnitID").html(response.FromUnitID);
                $(".spnFromUserID").html(response.FromUserID);
                $(".spnRequestId").html(response.RequestId);
                $(".spnTrnFwdId").html(response.MaxTrnFwdId ?? 0);

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