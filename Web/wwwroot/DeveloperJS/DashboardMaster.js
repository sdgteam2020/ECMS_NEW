$(document).ready(function () {

    GetDashboardMasterCount();
})
function GetDashboardMasterCount() {
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Master/GetDashboardMasterCount',
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
                    $("#TotUnit").html(response.TotUnit);
                    $("#TotRank").html(response.TotRank);
                    $("#TotAppointment").html(response.TotAppointment);
                    $("#TotArms").html(response.TotArms);
                    $("#TotRegtCentre").html(response.TotRegtCentre);

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