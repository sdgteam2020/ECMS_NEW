$(document).ready(function () {

    GetSubDashboardCount();
})
function GetSubDashboardCount() {
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Home/GetSubDashboardCount',
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
                    $("#ToDraftedOffrs").html(response.ToDraftedOffrs);
                    $("#ToDraftedJCO").html(response.ToDraftedJCO);
                    $("#ToSubmittedOffrs").html(response.ToSubmittedOffrs);
                    $("#ToSubmittedJCO").html(response.ToSubmittedJCO);
                    $("#ToClosedOffrs").html(response.ToClosedOffrs);
                    $("#ToClosedJCO").html(response.ToClosedJCO);
                    $("#ToCompletedOffrs").html(response.ToCompletedOffrs);
                    $("#ToCompletedJCO").html(response.ToCompletedJCO);
                    $("#ToRejectedOffrs").html(response.ToRejectedOffrs);
                    $("#ToRejectedJCO").html(response.ToRejectedJCO);

                    $('.counter-value').each(function () {
                        $(this).prop('Counter', 0).animate({
                            Counter: $(this).text()
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

            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}