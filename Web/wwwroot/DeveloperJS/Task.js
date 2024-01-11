
$(document).ready(function () {

    GetTaskIcardRequestCount(1,1)
    GetTaskIcardRequestCount(2,1)
    GetTaskIcardRequestCount(1, 2)
    GetTaskIcardRequestCount(2, 2)
    GetNotification(1, 1);
    GetNotification(1, 2);
    GetNotificationRequestId(1, 1);
    GetNotificationRequestId(1, 2);
});
function GetTaskIcardRequestCount(Id, applyForId) {
    var userdata =
    {
        "Id": Id,
        "applyForId": applyForId
    };
    $.ajax({
        url: '/Home/GetTaskCountICardRequest',
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
                    if (applyForId == 1) {
                        if (Id == 1) {
                            $(".IOPending").html(response.IOPending);
                            $(".IOApproved").html(response.IOApproved);
                            $(".IOReject").html(response.IOReject);
                            $(".GSOPending").html(response.GSOPending);
                            $(".GSOApproved").html(response.GSOApproved);
                            $(".GSOReject").html(response.GSOReject);
                            $(".MIPending").html(response.MIPending);
                            $(".MIApproved").html(response.MIApproved);
                            $(".MIReject").html(response.MIReject);
                            $(".HQPending").html(response.HQPending);
                            $(".HQApproved").html(response.HQApproved);
                            $(".HQReject").html(response.HQReject);
                        }
                        else if (Id == 2) {
                            $("#IOPending").html(response.IOPending);
                            $("#IOApproved").html(response.IOApproved);
                            $("#IOReject").html(response.IOReject);
                            $("#GSOPending").html(response.GSOPending);
                            $("#GSOApproved").html(response.GSOApproved);
                            $("#GSOReject").html(response.GSOReject);
                            $("#MIPending").html(response.MIPending);
                            $("#MIApproved").html(response.MIApproved);
                            $("#MIReject").html(response.MIReject);
                            $("#HQPending").html(response.HQPending);
                            $("#HQApproved").html(response.HQApproved);
                            $("#HQReject").html(response.HQReject);
                        }
                    } else {
                        if (Id == 1) {
                            $(".COPending").html(response.IOPending);
                            $(".COApproved").html(response.IOApproved);
                            $(".COReject").html(response.IOReject);
                            $(".DIDPending").html(response.MIPending);
                            $(".DIDApproved").html(response.MIApproved);
                            $(".DIDReject").html(response.MIReject);
                            //$(".MIPending").html(response.MIPending);
                            //$(".MIApproved").html(response.MIApproved);
                            //$(".MIReject").html(response.MIReject);
                            $(".ExportPending").html(response.HQPending);
                            $(".ExportApproved").html(response.HQApproved);
                            $(".ExportReject").html(response.HQReject);
                        }
                        else if (Id == 2) {
                            $("#COPending").html(response.IOPending);
                            $("#COApproved").html(response.IOApproved);
                            $("#COReject").html(response.IOReject);
                            $("#DIDPending").html(response.GSOPending);
                            $("#DIDApproved").html(response.MIApproved);
                            $("#DIDReject").html(response.GSOReject);                      
                            $("#ExportPending").html(response.HQPending);
                            $("#ExportApproved").html(response.HQApproved);
                            $("#ExportReject").html(response.HQReject);
                        }
                    }
                }
            }
            else {
               
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });

}





