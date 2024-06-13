
$(function () {

    GetTaskIcardRequestCount($("#Id").html(), $("#applyForId").html())
   
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
                        if (Id == 1) { // Submitted
                            $("#ToDrafted").html(response.ToDrafted);
                            $("#ToSubmitted").html(response.ToSubmitted);
                            $("#ToCompleted").html(response.ToCompleted);
                            $("#ToRejected").html(response.ToRejected);
                        }
                        else if (Id == 2) { // Pending
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
                        if (Id == 1) { // Submitted
                            $("#ToDrafted").html(response.ToDrafted);
                            $("#ToSubmitted").html(response.ToSubmitted);
                            $("#ToCompleted").html(response.ToCompleted);
                            $("#ToRejected").html(response.ToRejected);
                        }
                        else if (Id == 2) { // Pending
                            $("#_2ndLevelPending").html(response._2ndLevelPending);
                            $("#_2ndLevelApproved").html(response._2ndLevelApproved);
                            $("#_2ndLevelReject").html(response._2ndLevelReject);
                            $("#_3rdLevelPending").html(response._3rdLevelPending);
                            $("#_3rdLevelApproved").html(response._3rdLevelApproved);
                            $("#_3rdLevelReject").html(response._3rdLevelReject);                     
                            $("#_4thLevelPending").html(response._4thLevelPending);
                            $("#_4thLevelApproved").html(response._4thLevelApproved);
                            $("#ToInternalForward").html(response.ToInternalForward);
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