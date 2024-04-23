$(document).ready(function () {

    GetDashboardCount();
})
function GetDashboardCount() {
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Home/GetDashboardCount',
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
                   
                    $("#TotReq").html(response.TotReq);
                    $("#TotReject").html(response.TotReject);
                    $("#TotSelfPen").html(response.TotSelfPen);
                    $("#TotIOPen").html(response.TotIOPen);
                    $("#TotGsoPen").html(response.TotGsoPen);
                    $("#TotM11Pen").html(response.TotM11Pen);
                    $("#TotGQ54Pen").html(response.TotGQ54Pen);
                    $("#TotPrintPen").html(response.TotPrintPen);
                       
                   
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