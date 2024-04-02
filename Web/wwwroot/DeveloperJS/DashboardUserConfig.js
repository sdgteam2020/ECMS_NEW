$(document).ready(function () {

    GetDashboardUserConfigCount();
})
function GetDashboardUserConfigCount() {
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Master/GetDashboardUserConfigCount',
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
                    $("#TotDomainRegn").html(response.TotDomainRegn);
                    $("#TotUserRegn").html(response.TotDomainRegn);
                    $("#TotUserProfile").html(response.TotUserProfile);

                    $('.counter-value').each(function () {
                        $(this).prop('Counter', 0).animate({
                            Counter: $(this).text()
                        }, {
                            duration: 3500,
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