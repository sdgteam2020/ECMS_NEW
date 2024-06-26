﻿$(document).ready(function () {
    $("#loadingToken").hide();
    $("#btnfetchtoken").click(function () {
        GetTokenDetails1("FetchUniqueTokenDetails","txtArmyNo");
    });

});

function GetTokenvalidatepersid2fawiththumbprint(IcNo, msgid, txticno, thumbprint) {

    $("#loadingToken").show();
    IcNo = "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996";
    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/FetchUniqueTokenDetails',
        type: "GET",
        dataType: "json",

        success: function (response) {
            $("#loadingToken").hide();
            if (response) {

                if (response[0].Status == '200') {
                    

                    GetTokenvalidatepersid2fa(IcNo, msgid, txticno, thumbprint);
                }
                else {
                    if (response[0].Status == '404') {
                       
                        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">' + response[0].Remarks +' </span></div>');
                        $("#" + txticno).val("");

                    }
                }
            }

        },
        error: function (result) {

            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
            $("#" + txticno).val("");
            $("#loadingToken").hide();
        }
    });

  
}
function GetTokenvalidatepersid2fa(IcNo, msgid, txticno, thumbprint) {
    $("#loadingToken").show();
    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/validatepersid2fa',
        type: "Post",
        contentType: 'application/json; charset=utf-8',
        'data': JSON.stringify({
            "inputPersID": IcNo,
        }),
        success: function (response) {
            $("#loadingToken").hide();
            if (response) {
                var data = response.ValidatePersID2FAResult;

                if (data == true) {
                    $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');

                    if (txticno != "") {
                        GetTokenDetails1('FetchUniqueTokenDetails', txticno, thumbprint);
                    }
                }
                else {
                    if (data == false) {
                        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">ICNO Not Match Inserted Token </span></div>');
                        $("#" + txticno).val("");
                        $("#txtspnIsToken").val("");
                    }
                }
            }

        },
        error: function (result) {

            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
            $("#loadingToken").hide();

        }
    });
}
function GetTokenValidate(ApiId, IcNo, msgid) {
    $("#loadingToken").show();
   

    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/' + ApiId,
        type: "Post",
        contentType: 'application/json; charset=utf-8',
        'data': JSON.stringify({
            "inputpersId": IcNo,
            
        }),
        success: function (response) {
            $("#loadingToken").hide();
            if (response) {
                var data = response.ValidatePersIDResult;
              
                if (data[0].Status == '200') {
                    $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">' + data[0].Remark + ' </span></div>');
               
                }
                else {
                    if (data[0].Status == '404') {

                        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">' + data[0].Remark + ' </span></div>');
                        $("#txtspnIsToken").val("");
                    }
                }
            }

        },
        error: function (result) {

            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
            $("#loadingToken").hide();

        }
    });
}
function GetTokenDetails1(ApiId, txt, thumbprint, msgid) {
    $("#loadingToken").show();
    var examdata =
    {
        "ApiName": ApiId,

    };
   
    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/' + ApiId,
        type: "GET",
        dataType: "json",
        
        success: function (response) {
            $("#loadingToken").hide();
            if (response) {
               
                if (response[0].Status == '200') {
                    var CRL_OCSPCheck = response[0].CRL_OCSPCheck;
                    var CRL_OCSPMsg = response[0].CRL_OCSPMsg;
                    var Remarks = response[0].Remarks;
                    var Thumbprint = response[0].Thumbprint;
                    var Status = response[0].Status;
                    var TokenValid = response[0].TokenValid;
                    var ValidFrom = response[0].ValidFrom;
                    var ValidTo = response[0].ValidTo;
                    var issuer = response[0].issuer;
                    var subject = response[0].subject;

                    GetTokenDetails(CRL_OCSPCheck, CRL_OCSPMsg, Remarks, Thumbprint, Status, TokenValid, ValidFrom, ValidTo, issuer, subject, txt, thumbprint, msgid);
                }
                else {
                    if (response[0].Status == '404') {
                         $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">' + response[0].Remarks + ' </span></div>');
                         $("#" + txt).val("");
                        $("#txtspnIsToken").val("");
                    }
                }
            }
            
        },
        error: function (result) {
           
            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
            $("#" + txt).val("");
            $("#loadingToken").hide();
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
function GetTokenDetails(CRL_OCSPCheck, CRL_OCSPMsg, Remarks, Thumbprint, Status, TokenValid, ValidFrom, ValidTo, issuer, subject, txt, thumbprint, msgid) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $("#loadingToken").show();
    var examdata =
    {
        "CRL_OCSPCheck": CRL_OCSPCheck,
        "CRL_OCSPMsg": CRL_OCSPMsg,
        "Remarks": Remarks,
        "Thumbprint": Thumbprint,
        "Status": Status,
        "TokenValid": TokenValid,
        "ValidFrom": ValidFrom,
        "ValidTo": ValidTo,
        "issuer": issuer,
        "subject": subject,
        /*__RequestVerificationToken: token, */
    };
    $.ajax({
        url: '/ConfigUser/GetTokenDetails',
        data: examdata,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            $("#loadingToken").hide();
            if (response != "null" && response != null) {
                if (response == '') {
                    $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');
                    $("#" + txt).val("");
                    if (thumbprint == "")
                        $("#" + thumbprint).val("");
                    $("#txtspnIsToken").val("");
                }

                else if (response.Status == '200') {///&& response[0].TokenValid=='true'
                    // $("#error-msg").html(response.message);
                    var datef2 = new Date();
                    if (response.ValidTo >= datef2) {
                        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">Token Expired</span>.</div>');
                        $("#" + txt).val("");
                        if (thumbprint != "")
                            $("#" + thumbprint).val("");
                        $("#txtspnIsToken").val("");
                    }
                    else {
                        $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected </span></div>');
                        //  if (response.ArmyNo = "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996")
                        
                        // $("#" + txt).val(response.ArmyNo);
                       // $("#" + txt).val(response.ArmyNo);
                        if (thumbprint!="")
                            $("#" + thumbprint).val(response.Thumbprint);
                        $("#txtspnIsToken").val("Ok");
                     
                        //let foo = prompt('Enter Army No');
                        //let bar = confirm('Confirm or deny');
                        //$("#" + txt).val(foo);

                        $("#" + txt).val(response.ArmyNo.trim());
                    }

                   
                }
                else {
                    if (response.Status == '404') {
                        //$("#error-msg").html(response.message);
                        $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">' + response.Remarks + '</span>.</div>');
                        $("#" + txt).val("");
                        if (thumbprint != "")
                            $("#" + thumbprint).val("");
                        $("#txtspnIsToken").val("");
                    }
                    
                    
                }
            }
            else {
                $("#" + msgid).html(errormsg001);
                return 0;
            }
        },
        error: function (result) {
            $("#" + msgid).html(errormsg002);
            return 0; $("#loadingToken").hide();
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
