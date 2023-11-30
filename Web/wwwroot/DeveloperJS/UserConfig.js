$(document).ready(function () {
    
    $("#btnConfigsave").click(function () {
       
        if ($("#txtArmyNo").val() != "")
        {
            //   Gotodashboard($("#txtArmyNo").val());
        }
        else
            $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Offrs Army No Not Blank</span>.</div>');
    });
});
function SaveMapping(ArmyNo) {

    var examdata =
    {
        "ICNO": ArmyNo,

    };
    $.ajax({
        url: '/ConfigUser/GotoDashboard',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {

                if (response == 1)
                    window.location.href = "/Home/Dashboard";
                else {
                    $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">This ICNO Profile Not Available</span>.</div>');

                }
            }
        }
    });
}
function Gotodashboard(ArmyNo) {

    var examdata =
    {
        "ICNO": ArmyNo,

    };
    $.ajax({
        url: '/ConfigUser/GotoDashboard',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {

                if (response == 1)
                    window.location.href = "/Home/Dashboard";
                else {
                    $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">This ICNO Profile Not Available</span>.</div>');

                }
            }
        }
    });
}