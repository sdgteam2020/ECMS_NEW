$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    GetDashboardCount($("#Type").html())
})
function GetDashboardCount(type) {
    var userdata =
    {
        "Id": type,

    };
    $.ajax({
        url: '/Home/GetRequestDashboardCount',
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
                    $("#ToPostingInOffrs").html(response.ToPostingInOffrs);
                    $("#ToPostingInJCO").html(response.ToPostingInJCO);
                    $("#ToPostingOutOffrs").html(response.ToPostingOutOffrs);
                    $("#ToPostingOutJCO").html(response.ToPostingOutJCO);
                    $("#ToCourseJCO").html(0);
                    $("#ToObsnRaisedOASIS").html(0);
                    $("#ToObsnRaisedINDRA").html(0);
                    $("#ToHotlistedICard").html(0);
                    $("#ToBlockExistingICard").html(0);
                    $("#ToDepositICard").html(0);


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