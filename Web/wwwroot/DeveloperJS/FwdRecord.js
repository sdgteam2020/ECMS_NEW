$(document).ready(function () {
    var spnStepId = 0;
    $(".fwdrecord").click(function () {
       // ResetMapUnit();
        $("#FwdRecord").modal('show');
        $(".spnFname").html($(this).closest("tr").find(".PersName").html());
        $(".spnFarmyno").html($(this).closest("tr").find(".ServiceNo").html());
        $("#spnStepCounter").html($(this).closest("tr").find(".spnStepCounterId").html());
        var spnRequestId = $(this).closest("tr").find(".spnRequestId").html();
        spnStepId = $(this).closest("tr").find(".spnStepId").html();
        GetForwardHHierarchy($(this).closest("tr").find(".ServiceNo").html(), $(this).closest("tr").find(".spnStepCounterId").html(), spnRequestId)
       
    });
    $("#btnForward").click(function () {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be Forward!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Forward it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var spnRequestId = $("#spnCurrentspnRequestId").html();
                var Counter = parseInt($("#spnStepCounter").html()) + 1;


                UpdateStepCounter(spnStepId, spnRequestId, Counter);

                ForwardTo(spnRequestId);
            }
        })  
    });

});
function GetForwardHHierarchy(ArmyNo, StepCounter, spnRequestId) {
    if (StepCounter == 1) {
        $(".gsoio").html("IO");
        $("#btnForward").html("Forward To IO");

    }
    else {
        $(".gsoio").html("GSO");
        $("#btnForward").html("Forward To GSO");
    }
        

    var userdata =
    {
        "ArmyNo": ArmyNo,

    };
    $.ajax({
        url: '/UserProfile/GetAllByArmyNo',
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
                    $(".HProfileDetails").removeClass("d-none");
                    $("#ForwardDetails").html("");
                    $("#btnForward").removeClass("d-none");
                    $("#spnCurrentspnRequestId").html(spnRequestId);
                    if (StepCounter == 1) {
                        $(".spnFtoarmyno").html(response.IOArmyNo);
                        $(".spnFtoname").html(response.IOName);


                        $("#spnFrom").html(response.UserId);
                        $("#spnForwardTo").html(response.IOUserId);

                    } else if (StepCounter == 2) {
                        $(".spnFtoarmyno").html(response.GSOArmyNo);
                        $(".spnFtoname").html(response.GSOName);

                        $("#spnFrom").html(response.IOUserId);
                        $("#spnForwardTo").html(response.GSOUserId);
                    }
                    else if (StepCounter == 3) {

                    }

                }
            }
            else {
                $(".HProfileDetails").addClass("d-none");
                $("#btnForward").addClass("d-none");
                $("#ForwardDetails").html("Please Add Self Profile");

                $(".spnFtoarmyno").html("");
                $(".spnFtoname").html("");
                $("#spnForwardTo").html(0);
                $("#spnCurrentspnRequestId").html(0);
               

            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function ForwardTo(RequestId) {
    var userdata =
    {
        "TrnFwdId": 0,
        "RequestId": RequestId,
        "FromUserId": $("#spnFrom").html(),
        "ToUserId": $("#spnForwardTo").html(),
        "SusNo": 0,
        "Remark": "",
        "Status": true,

    };
    $.ajax({
        url: '/BasicDetail/IcardFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {

            }
        }

    });
}
function UpdateStepCounter(stepId, spnRequestId, Counter) {
    var userdata =
    {
        "Id": stepId,
        "RequestId": spnRequestId,
        "Step": Counter

    };
    $.ajax({
        url: '/BasicDetail/UpdateStepCounter',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                $("#FwdRecord").modal('hide');
            }
        }

    });
}