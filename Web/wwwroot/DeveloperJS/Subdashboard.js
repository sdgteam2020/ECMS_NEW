$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    GetSubDashboardCount();
})
function GetSubDashboardCount() {

    // show loader until server data received
    $(".counter-value").html('<span class="count-loader"></span>');

    $.ajax({
        url: '/Home/GetSubDashboardCount',
        contentType: 'application/x-www-form-urlencoded',
        data: { Id: 0 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {

            if (response != "null" && response != null && response != 0 && response != InternalServerError) {

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
            else {
                // keep loader, do not show zero
                $(".counter-value").html('<span class="count-loader"></span>');
            }
        },

        error: function () {
            // keep loader, do not show zero
            $(".counter-value").html('<span class="count-loader"></span>');

            Swal.fire({
                text: errormsg002
            });
        }
    });
}