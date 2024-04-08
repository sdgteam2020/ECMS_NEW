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
                    $("#TotComd").html(response.TotComd == 1 ? 0 : response.TotComd-1);
                    $("#TotCorps").html(response.TotCorps == 1 ? 0 : response.TotCorps - 1);
                    $("#TotDiv").html(response.TotDiv == 1 ? 0 : response.TotDiv - 1);
                    $("#TotBde").html(response.TotBde == 1 ? 0 : response.TotBde - 1);
                    $("#TotMapUnit").html(response.TotMapUnit);
                    $("#TotUnit").html(response.TotUnit);
                    $("#TotRank").html(response.TotRank);
                    $("#TotAppointment").html(response.TotAppointment);
                    $("#TotArms").html(response.TotArms);
                    $("#TotRegtCentre").html(response.TotRegtCentre);
                    $("#TotRecordOffice").html(response.TotRecordOffice);
                    $("#TotDomainRegn").html(response.TotDomainRegn);
                    $("#TotUserRegn").html(response.TotDomainRegn);
                    $("#TotUserProfile").html(response.TotUserProfile);

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