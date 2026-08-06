
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
            if (response.Result == true) {
                if (Id == 1) { // Submitted
                    $("#ToDrafted").html(response.Value.ToDrafted);
                    $("#ToSubmitted").html(response.Value.ToSubmitted);
                    $("#ToCompleted").html(response.Value.ToCompleted);
                    $("#ToRejected").html(response.Value.ToRejected);

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
                    $("#_2ndLevelPending").html(response.Value._2ndLevelPending);
                    $("#_2ndLevelApproved").html(response.Value._2ndLevelApproved);
                    $("#_2ndLevelReject").html(response.Value._2ndLevelReject);                  
                    $("#_3rdLevelPending").html(response.Value._3rdLevelPending);
                    $("#_3rdLevelApproved").html(response.Value._3rdLevelApproved);
                    $("#ToInternalForward").html(response.Value.ToInternalForward);
                    $("#_3rdLevelReject").html(response.Value._3rdLevelReject);
                    $("_3rdLevelClosed").html(response.Value._3rdLevelClosed);
                    $("#_4thLevelPending").html(response.Value._4thLevelPending);
                    $("#_4thLevelApproved").html(response.Value._4thLevelApproved);
                    $("#_4thLevelClosed").html(response.Value._4thLevelClosed);
                    $(".csvUploadCount").html(response.Value.CsvUploadCount);

                    $("#Closed_IO").html(response.Value.Closed_IO);
                    $("#Closed_ADC").html(response.Value.Closed_ADC);
                    $("#Closed_ORO").html(response.Value.Closed_ORO);
                    $("#Closed_RO").html(response.Value.Closed_RO);
                    $("#Closed_RO_2").html(response.Value.Closed_RO_2);

                    $("#Completed_IO").html(response.Value.Completed_IO);
                    $("#Completed_ADC").html(response.Value.Completed_ADC);
                    $("#Completed_ORO").html(response.Value.Completed_ORO);
                    $("#Completed_RO").html(response.Value.Completed_RO);
                    $("#Completed_RO_2").html(response.Value.Completed_RO_2);

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
            else {
                Swal.fire({
                    text: response.Message
                });
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