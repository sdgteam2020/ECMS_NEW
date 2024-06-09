
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
                            $("._2ndLevelPending").html(response._2ndLevelPending);
                            $("._2ndLevelApproved").html(response._2ndLevelApproved);
                            $("._2ndLevelReject").html(response._2ndLevelReject);
                            $("._3rdLevelPending").html(response._3rdLevelPending);
                            $("._3rdLevelApproved").html(response._3rdLevelApproved);
                            $("._3rdLevelReject").html(response._3rdLevelReject);
                            $("._4thLevelPending").html(response._4thLevelPending);
                            $("._4thLevelApproved").html(response._4thLevelApproved);
                            $("._4thLevelReject").html(response._4thLevelReject);
                            $(".ExportPending").html(response.ExportPending);
                            $(".ExportApproved").html(response.ExportApproved);
                            $(".ExportReject").html(response.ExportReject);
                        }
                        else if (Id == 2) {
                            $("#_2ndLevelPending").html(response._2ndLevelPending);
                            $("#_2ndLevelApproved").html(response._2ndLevelApproved);
                            $("#_2ndLevelReject").html(response._2ndLevelReject);
                            $("#_3rdLevelPending").html(response._3rdLevelPending);
                            $("#_3rdLevelApproved").html(response._3rdLevelApproved);
                            $("#_3rdLevelReject").html(response._3rdLevelReject);
                            $("#_4thLevelPending").html(response._4thLevelPending);
                            $("#_4thLevelApproved").html(response._4thLevelApproved);
                            $("#_4thLevelReject").html(response._4thLevelReject);
                            $("#ExportPending").html(response.ExportPending);
                            $("#ExportApproved").html(response.ExportApproved);
                            $("#ExportReject").html(response.ExportReject);
                        }
                    } else {
                        if (Id == 1) {
                            $(".COPending").html(response._2ndLevelPending);
                            $(".COApproved").html(response._2ndLevelApproved);
                            $(".COReject").html(response._2ndLevelReject);
                            $(".DIDPending").html(response._3rdLevelPending);
                            $(".DIDApproved").html(response._3rdLevelReject);
                            $(".DIDReject").html(response._3rdLevelReject);
                            $(".ExportPending").html(response.ExportPending);
                            $(".ExportApproved").html(response.ExportApproved);
                        }
                        else if (Id == 2) {
                            $("#COPending").html(response._2ndLevelPending);
                            $("#COApproved").html(response._2ndLevelApproved);
                            $("#COReject").html(response._2ndLevelReject);
                            $("#DIDPending").html(response._3rdLevelPending);
                            $("#DIDApproved").html(response._3rdLevelApproved);
                            $("#DIDReject").html(response._3rdLevelReject);                      
                            $("#ExportPending").html(response.ExportPending);
                            $("#ExportApproved").html(response.ExportPending);
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