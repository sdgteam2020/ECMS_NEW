var errormsg = "Error.Due to network issue, please try after some time.";
var errormsg001 = "Error 001. Due to network issue, please try after some time.";
var errormsg002 = "Error 002. Due to network issue, please try after some time.";

$(document).ready(function () {

    GetTokenDetails();
    var myInterval1 = setInterval(function () {
       
        $('.news-slider-next').trigger('click');
    }, 8000); 
    var myInterval = setInterval(function () {
       
        GetTokenDetails();
    }, 4000); 
    $("#btnLogin").click(function () {

        if ($("#txtUserId").val() == "") {
            $("#error-msg").html("Please Enter SusNo / ArmyNo.");
            $(".spnmsg").removeClass("d-none");
        } else if ($("#txtPassword").val() == "") {
            $("#error-msg").html("Please Enter Password.");
            $(".spnmsg").removeClass("d-none");
        }
        else {
            $(".spnmsg").addClass("d-none");

            window.location.href = "/Home/Dashboard";
            //
           //$("#txtUserId").val()
           // GetTokenDetails1($("#txtUserId").val());
            
           /// Verify($("#txtUserId").val(), $("#txtPassword").val())
           
        }

    });
});
function GetTokenDetails(LoginId, Type) {

    var examdata =
    {
        "ICNo": LoginId,

    };
    $.ajax({
        url: '/Login/GetTokenDetails',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {

            if (response != "null" && response != null) {

                if (response == -2) {
                    $("#error-msg").html("Mandatory fields missing.");
                }
                else if (response == -1) {
                    $("#error-msg").html(errormsg);
                }
                else if (response == -4) {
                    $("#error-msg").html("Invalid credential.");
                }
                if (response.isToken == true && response.messageCode != 0) {
                    // $("#error-msg").html(response.message);
                    $("#tokenmsg").html('<div class="alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');
                    return 1;

                }
                else {
                    if (response.isToken == false && response.messageCode == 100) {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response.message + '</span>.</div>');
                    }
                    else if (response.isToken == false && response.messageCode == 101) {
                            //$("#error-msg").html(response.message);
                            $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Please Insert Token</span>.</div>');
                        }
                    else
                        if (response.isToken == false && response.messageCode == 0) {
                            //$("#error-msg").html(response.message);
                            $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> DGIS APP Not Running This Pc. Make Sure Digital Signature Exe Run on this PC.</span>.</div>');
                        }
                    return 0;
                }
            }
            else {
                $("#error-msg").html(errormsg001);
                return 0;
            }
        },
        error: function (result) {
            $("#error-msg").html(errormsg002);
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

function GetTokenDetails1(LoginId, Type) {

    var examdata =
    {
        "ICNo": LoginId,

    };
    $.ajax({
        url: '/Login/GetTokenDetails',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',

        success: function (response) {

            if (response != "null" && response != null) {

                if (response == -2) {
                    $("#error-msg").html("Mandatory fields missing.");
                }
                else if (response == -1) {
                    $("#error-msg").html(errormsg);
                }
                else if (response == -4) {
                    $("#error-msg").html("Invalid credential.");
                }
                if (response.isToken == true && response.messageCode == 100) {
                    Verify($("#txtUserId").val(), $("#txtPassword").val(), response.armyNo)
                }
                else {
                    if (response.isToken == false && response.messageCode == 0) {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response.message + '</span>.</div>');
                    }
                    else if (response.isToken == false && response.messageCode == 101) {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Please Insert Token</span>.</div>');
                    }
                    else
                        if (response.isToken == false && response.messageCode == 0) {
                            //$("#error-msg").html(response.message);
                            $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> Digital Signature Exe Not Running This Pc. Make Sure Digital Signature Exe Run on this PC.</span>.</div>');
                        }
                    return 0;
                }
            }
            else {
                $("#error-msg").html(errormsg001);
                return 0;
            }
        },
        error: function (result) {
            $("#error-msg").html(errormsg002);
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
function GetIsTokenbyoffrs(LoginId) {
    
    var examdata =
    {
        "ICNo": LoginId,

    };
    $.ajax({
        url: '/Login/GetIsToken',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',

        success: function (response) {

            if (response != "null" && response != null) {

                if (response == -2) {
                    $("#error-msg").html("Mandatory fields missing.");
                }
                else if (response == -1) {
                    $("#error-msg").html(errormsg);
                }
                else if (response == -4) {
                    $("#error-msg").html("Invalid credential.");
                }
                if (response.isToken == true && response.messageCode != 0) {
                    // $("#error-msg").html(response.message);
                    $("#tokenmsg").html('<div class="alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');
                    window.location.href = "/Home/Index";
                    return 1;

                }
                else {
                    if (response.isToken == false && response.messageCode != 0) {
                        //$("#error-msg").html(response.message);
                        $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response.message + '</span>.</div>');
                    }
                    else
                        if (response.isToken == false && response.messageCode == 0) {
                            //$("#error-msg").html(response.message);
                            $("#tokenmsg").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> Digital Signature Exe Not Running This Pc. Make Sure Digital Signature Exe Run on this PC.</span>.</div>');
                        }
                    return 0;
                }
            }
            else {
                $("#error-msg").html(errormsg001);
                return 0;
            }
        },
        error: function (result) {
            $("#error-msg").html(errormsg002);
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
function GetIsTokenbySusno() {
   
    var examdata =
    {
        "id": 0,

    };
    $.ajax({
        url: '/Login/GetIsTokenbySusno',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',

        success: function (response) {

            if (response != "null" && response != null) {

                if (response == -2) {
                    $("#error-msg").html("Mandatory fields missing.");
                }
                else if (response == -1) {
                    $("#error-msg").html(errormsg);
                }
                else if (response == -4) {
                    $("#error-msg").html("Invalid credential.");
                }
                if (response == 0) {
                    // $("#error-msg").html(response.message);
                   
                    $("#tokenmsgafterverify").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> The officer, whose token is connected, does not belong to this Unit SUS </span>.</div>');
                }
               else if (response == 1) {
                    // $("#error-msg").html(response.message);

                    $("#tokenmsgafterverify").html('<div class="alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> Token Not connected </span>.</div>');
                }
                else if (response.persId != 0) {
                   
                    $("#tokenmsgafterverify").html("");
                    window.location.href = "/Home/Index";
                }
               

            }
            else {
                $("#error-msg").html(errormsg001);
            }
        },
        error: function (result) {
            $("#error-msg").html(errormsg002);
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
function Verify(LoginId, Password, armyNo) {
   
  
    var examdata =
    {
        "username": LoginId,
        "password": Password
    };
    $.ajax({
        url: '/Login/Authenticate',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
       
        success: function (response) {

            if (response != "null" && response != null) {
               
                if (response == -2) {
                    $(".spnmsg").removeClass("d-none");
                    $("#error-msg").html("Mandatory fields missing.");
                }
                else if (response == -1) {
                    $(".spnmsg").removeClass("d-none");
                    $("#error-msg").html(errormsg);
                }
                else if (response == -4) {
                    $(".spnmsg").removeClass("d-none");
                    $("#error-msg").html("Invalid credential.");
                }
                else if (response == 0 || response == 1 || response == 2) {
                    $(".spnmsg").addClass("d-none");
                    window.location.href = "/UnitConfiguration";
                }
                else if (response == 3 || response == 4) {
                    GetIsTokenbySusno();
                   // window.location.href = "/Home/Index";

                } else if (response == 5) {
                    GetIsTokenbyoffrs(armyNo);
                    
                   // window.location.href = "/Home/Index";
                }
                
                else {
                    $(".spnmsg").removeClass("d-none");
                    $("#error-msg").html(response);
                }
          

            }
            else {
                $(".spnmsg").removeClass("d-none");
                $("#error-msg").html(errormsg001);
            }
        },
        error: function (result) {
            $("#error-msg").html(errormsg002);
        }
    });
}