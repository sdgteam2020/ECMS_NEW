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

               
            }
        })  
    });

});
function GetForwardHHierarchy(ArmyNo, StepCounter, spnRequestId) {
    if (StepCounter == 1) {
        $(".gsoio").html("IO");
        $("#btnForward").html("Forward To IO");

    }
    else if (StepCounter == 2) {
        $(".gsoio").html("GSO");
        $("#btnForward").html("Forward To GSO");
    }
    else if (StepCounter == 3) {
        $(".gsoio").html("MI 11");
        $("#btnForward").html("Forward To MI 11");
    }
    else if (StepCounter == 4) {
        $(".gsoio").html("HQ 54");
        $("#btnForward").html("Forward To HQ 54");
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
                    $("#spnCurrentspnRequestId").html(response.RequestId);
                    if (StepCounter == 1) {
                        $(".spnFtoarmyno").html(response.IOArmyNo);
                        $(".spnFtoname").html(response.IOName);


                        $("#spnFrom").html(response.UserId);
                        $("#spnForwardTo").html(response.IOUserId);
                        $("#spnFwssusno").html(0);
                    } else if (StepCounter == 2) {
                        $(".spnFtoarmyno").html(response.GSOArmyNo);
                        $(".spnFtoname").html(response.GSOName);

                        $("#spnFrom").html(response.IOUserId);
                        $("#spnForwardTo").html(response.GSOUserId);
                        $("#spnFwssusno").html(0);
                    }
                    else if (StepCounter == 3) {

                        $(".HProfileDetails").addClass("d-none");
                        $("#spnFrom").html(response.GSOUserId);
                        $("#spnFwssusno").html(101);

                    }
                    else if (StepCounter == 4) {

                        $(".HProfileDetails").addClass("d-none");
                        $("#spnFrom").html(101);
                        $("#spnForwardTo").html(29);
                        $("#spnFwssusno").html(0);

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
function ForwardTo(RequestId, HType) {

    var userdata =
    {
        "TrnFwdId": 0,
        "RequestId": RequestId,
        "FromUserId": $("#spnFrom").html(),
        "ToUserId": $("#spnForwardTo").html(),
        "SusNo": $("#spnFwssusno").html(),
        "Remark": "",
        "Status": true,
        "HType": HType,

    };
    $.ajax({
        url: '/BasicDetail/IcardFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                location.reload();
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
                var HType = 0;
                if (Counter == 3) {
                    HType = 1;
                } else if (Counter == 4) {
                    HType = 2;
                } 

                ForwardTo(spnRequestId, HType);
            }
        }

    });
}