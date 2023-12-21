$(document).ready(function () {
    
    $("#btnfetchtoken").click(function () {
        GetTokenDetails1("FetchUniqueTokenDetails","txtArmyNo");
    });
});
function GetTokenDetails1(ApiId, txt) {
    alert(ApiId)
    var examdata =
    {
        "ApiName": ApiId,

    };
    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/' + ApiId,
        type: 'GET',
       
        success: function (response) {
            if (response) {

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
function GetTokenDetails(ApiId,txt) {

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
                    $("#tokenmsg").html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
                    $("#" + txt).val("");
                }

                else if (response[0].Status == '200') {///&& response[0].TokenValid=='true'
                    // $("#error-msg").html(response.message);
                    var datef2 = new Date();
                    if (response[0].ValidTo >= datef2) {
                        $("#tokenmsg").html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Token Expired</span>.</div>');
                        $("#" + txt).val("");
                    }
                    else {
                        $("#tokenmsg").html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');
                        if (response[0].ArmyNo = "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996")
                            $("#" + txt).val("IC-00203");
                          //$("#" + txt).val("IC-00002");
                    }

                   
                }
                else {
                    if (response[0].Status == '404') {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response[0].Remarks + '</span>.</div>');
                        $("#" + txt).val("");
                        $("#" + txt).val("IC-00005");
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