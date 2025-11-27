$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    GetDashboardUserMgtCount();
})
function GetDashboardUserMgtCount() {
    $.ajax({
        url: '/Home/GetDashboardUserMgtCount',
        contentType: 'application/x-www-form-urlencoded',
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

                    $("#TotRegisterUser").html(response.TotRegisterUser);
                    $("#TotPostingIn").html(response.TotPostingIn);
                    $("#TotPostingOut").html(response.TotPostingOut);

                    $('.counter-value').each(function () {
                        $(this).prop('Counter', 0).animate({
                            Counter: $(this).text()
                        }, {
                            duration: 500,
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