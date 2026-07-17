
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    GetTaskIcardRequestCount($("#Id").html(), $("#applyForId").html())

    //GetNotificationRequestId(1, 1); //this notification method for self notifiy
    //GetNotificationRequestId(1, 2); //this notification method for self notifiy
});
function GetTaskIcardRequestCount(Id, applyForId) {
    // show loader until server data received
    $(".counter-value").html('<span class="count-loader"></span>');

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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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

                            $('.counter-value').each(function () {
                                let finalValue = parseInt($(this).text()) || 0;

                                $(this).prop('Counter', 0).animate({
                                    Counter: finalValue
                                }, {
                                    duration: 200,
                                    easing: 'swing',
                                    step: function (now) {
                                        $(this).text(Math.ceil(now));
                                    }
                                });
                            });

                        }
                        else if (Id == 2) { // Pending
                            $("#_2ndLevelPending").html(response._2ndLevelPending);
                            $("#_2ndLevelApproved").html(response._2ndLevelApproved);
                            $("#_2ndLevelReject").html(response._2ndLevelReject);
                            $("#_2ndLevelClosed").html(response._2ndLevelClosed);
                            $("#_3rdLevelPending").html(response._3rdLevelPending);
                            $("#_3rdLevelApproved").html(response._3rdLevelApproved);
                            $("#_3rdLevelReject").html(response._3rdLevelReject);
                            $("_3rdLevelClosed").html(response._3rdLevelClosed);
                            $("#_4thLevelPending").html(response._4thLevelPending);
                            $("#_4thLevelApproved").html(response._4thLevelApproved);
                            $("#_4thLevelClosed").html(response._4thLevelClosed);
                            $(".csvUploadCount").html(response.CsvUploadCount);

                            $('.counter-value').each(function () {
                                let finalValue = parseInt($(this).text()) || 0;

                                $(this).prop('Counter', 0).animate({
                                    Counter: finalValue
                                }, {
                                    duration: 200,
                                    easing: 'swing',
                                    step: function (now) {
                                        $(this).text(Math.ceil(now));
                                    }
                                });
                            });
                        }
                    } else {
                        if (Id == 1) { // Submitted
                            $("#ToDrafted").html(response.ToDrafted);
                            $("#ToSubmitted").html(response.ToSubmitted);
                            $("#ToCompleted").html(response.ToCompleted);
                            $("#ToRejected").html(response.ToRejected);

                            $('.counter-value').each(function () {
                                let finalValue = parseInt($(this).text()) || 0;

                                $(this).prop('Counter', 0).animate({
                                    Counter: finalValue
                                }, {
                                    duration: 200,
                                    easing: 'swing',
                                    step: function (now) {
                                        $(this).text(Math.ceil(now));
                                    }
                                });
                            });
                        }
                        else if (Id == 2) { // Pending
                            $("#_2ndLevelPending").html(response._2ndLevelPending);
                            $("#_2ndLevelApproved").html(response._2ndLevelApproved);
                            $("#_2ndLevelReject").html(response._2ndLevelReject);
                            $("#_2ndLevelClosed").html(response._2ndLevelClosed);
                            $("#_3rdLevelPending").html(response._3rdLevelPending);
                            $("#_3rdLevelApproved").html(response._3rdLevelApproved);
                            $("#_3rdLevelReject").html(response._3rdLevelReject);
                            $("_3rdLevelClosed").html(response._3rdLevelClosed);
                            $("#_4thLevelPending").html(response._4thLevelPending);
                            $("#_4thLevelApproved").html(response._4thLevelApproved);
                            $("#_4thLevelClosed").html(response._4thLevelClosed);
                            $("#ToInternalForward").html(response.ToInternalForward);
                            $(".csvUploadCount").html(response.CsvUploadCount);

                            $('.counter-value').each(function () {
                                let finalValue = parseInt($(this).text()) || 0;

                                $(this).prop('Counter', 0).animate({
                                    Counter: finalValue
                                }, {
                                    duration: 200,
                                    easing: 'swing',
                                    step: function (now) {
                                        $(this).text(Math.ceil(now));
                                    }
                                });
                            });
                        }
                    }
                }
            }
            else {
                // keep loader, do not show zero
                $(".counter-value").html('<span class="count-loader"></span>');
            }
        },
        error: function (result) {
            // keep loader, do not show zero
            $(".counter-value").html('<span class="count-loader"></span>');

            Swal.fire({
                text: errormsg002
            });
        }
    });

}