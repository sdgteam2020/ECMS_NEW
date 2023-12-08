// Set the options that I want
toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
$(document).ready(function () {

   

    Getaspntokenarmyno()
    if (window.location.pathname !="/UserProfile/Profile")
    CheckProfileExist();
});
function CheckProfileExist() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/ConfigUser/CheckProfileExist',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response.UserId == 0 || response.UserId == null) {
                    alert('Please Add Profile First !');
                    window.location = "/UserProfile/Profile";
                }

            } else {
                alert('Please Add Profile First !');
                window.location = "/UserProfile/Profile";
            }
        }
    });
}
function Getaspntokenarmyno() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/ConfigUser/GetTokenArmyNo',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == 0) {
                  //  alert("Plase Add Profile")
                }
                else {
                    $("#aspntokenarmyno").html(response.ICNO)
                    $("#aspndomainUnitID").html(response.UnitId)
                }
            }
        }
    });
}
