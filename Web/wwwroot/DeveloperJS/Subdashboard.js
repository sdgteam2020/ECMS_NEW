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

                    $("#TotDrafted").html(response.TotDrafted);
                    $("#TotSubmitted").html(response.TotSubmitted);
                    $("#TotPrinted").html(response.TotPrinted);
                    $("#TotRaisedObsn").html(response.TotRaisedObsn);
                    $("#TotRejected").html(response.TotRejected);


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