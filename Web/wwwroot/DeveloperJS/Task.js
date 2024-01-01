
$(document).ready(function () {

    GetTaskIcardRequestCount(1)
    GetTaskIcardRequestCount(2)

    GetNotification(1);
    GetNotificationRequestId(1);
});
function GetTaskIcardRequestCount(Id) {
    var userdata =
    {
        "Id": Id,

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
                    else if (Id == 2)
                    {
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





