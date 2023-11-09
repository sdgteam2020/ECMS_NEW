$(document).ready(function () {
    
    $("#btnfetchtoken").click(function () {
        GetTokenDetails("FetchUniqueTokenDetails");
    });
});
function GetTokenDetails(ApiId) {

    var examdata =
    {
        "ApiName": ApiId,

    };
    $.ajax({
        url: '/ConfigUser/GetTokenDetails',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {
                if (response == '') {
                    $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');

                }

               else if (response[0].Status == '200') {
                    // $("#error-msg").html(response.message);
                    $("#tokenmsg").html('<div class="alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');
                   
                    $("#txtArmyNo").html(response[0].subject);
                }
                else {
                    if (response[0].Status == '404') {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response[0].Remarks + '</span>.</div>');
                    }
                    
                    
                }
            }
            else {
                $("#tokenmsg").html(errormsg001);
                return 0;
            }
        },
        error: function (result) {
            $("#tokenmsg").html(errormsg002);
            return 0;
        }
    });
    //Swal.fire({
    //    position: 'top-end',
    //    icon: 'success',
    //    title: 'Your work has been saved',
    //    showConfirmButton: false,
    //    timer: 1500
    //})


}