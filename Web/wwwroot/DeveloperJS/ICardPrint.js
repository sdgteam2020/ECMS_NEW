﻿$(document).ready(function () {
$("#btn-i-card-print").click(function () {
    //    window.scrollTo(0, 0);
    //var datef2 = new Date();
    //    $(".watermark").html($("#IpaddresGloble").html() + ' ' + DateFormateddMMyyyyhhmmss(datef2))
    //    /*$(".section-to-print-popup").focus();*/

    //    setTimeout(function () {
    //        window.print();
    //    }, 300); // 300 milliseconds delay

        PrintData("section-to-print-popup");
});
});
function GetICardPrintPreviewByRequestId(RequestId) {
    var userdata =
    {
        "RequestId": RequestId,
    };
    $.ajax({
        url: '/BasicDetail/GetICardPrintPreviewByRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                $(".PhotoImagePath_ICardPrint").attr('src', "/WriteReadData/photo/" + response.PhotoImagePath);
                $(".SignaturePath_ICardPrint").attr('src', "/WriteReadData/Signature/" + response.SignatureImagePath);
                $("#FName_ICardPrint").html(response.FName);
                $("#LName_ICardPrint").html(response.LName);
                $("#RankName_ICardPrint").html(response.RankName);
                $("#ArmedName_ICardPrint").html(response.ArmedName);
                $("#ServiceNo_ICardPrint").html(response.ServiceNo);
                $("#IdenMark1_ICardPrint").html(response.IdenMark1);
                $("#DOB_ICardPrint").html(DateFormateMMMM_dd_yyyy(response.DOB));
                $("#Height_ICardPrint").html(response.Height + ' CM');
                $("#AadhaarNo_ICardPrint").html(response.AadhaarNo.replace(/\d(?=\d{4})/g, "X"));
                $("#BloodGroup_ICardPrint").html(response.BloodGroup);
                $("#PlaceOfIssue_ICardPrint").html(response.PlaceOfIssue);
                $("#DateOfIssue_ICardPrint").html(response.DateOfIssue == null ? 'DEPENDS ON UNIT OF SECOND LEVEL APPROVER.' :  DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                $(".IssuingAuth_ICardPrint").html(response.IssuingAuth);
                $(".DateOfCommissioning_ICardPrint").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                $("#ICardPrint").modal('show');
                //$("#lblfdaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);
            }
        }
    })
}
function GetBasicDetailByRequestId(RequestId) {
    var userdata = {
        "RequestId": RequestId,
    };
    $.ajax({
        url: '/BasicDetail/GetBasicDetailByRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                $("#basicphotosVP").attr('src', "/WriteReadData/photo/" + response.PhotoImagePath);
                $("#BasicsingVP").attr('src', "/WriteReadData/Signature/" + response.SignatureImagePath);
                $("#lblvpNameAsPerRecord").html(response.NameAsPerRecord);
                $("#lblvpFName").html(response.FName);
                $("#lblvpLName").html(response.LName);
                $("#lblvpRank").html(response.RankName);
                $("#lblvparm").html(response.ArmedName);
                $("#lblvpArmyNo").html(response.ServiceNo);
                $("#lblvpMarks").html(response.IdenMark1);
                $("#lblvpdob").html(DateFormateMMMM_dd_yyyy(response.DOB));
                $("#lblvpheight").html(response.Height);
                $("#lblvpadhar").html(response.AadhaarNo);
                $("#lblvpBloodGroup").html(response.BloodGroup);
                $("#lblvppoi").html(response.PlaceOfIssue);
                $("#lblvpdoi").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                $("#lblvpissuA").html(response.IssuingAuthorityName);
                $("#lblvpdateo").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                $("#lblvpaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);
                $("#BasicDetailViewPurpose").modal('show');
            }
        }
    })
}
function printDiv() {

    var divToPrint = document.getElementById('ICardPrint');

    var newWin = window.open('', 'Print-Window');

    newWin.document.open();

    newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');

    newWin.document.close();

    setTimeout(function () { newWin.close(); }, 10);

    //var printContents = document.getElementById('ICardPrint').innerHTML;
    //var originalContents = document.body.innerHTML;

    //document.body.innerHTML = printContents;

    //window.print();

    //document.body.innerHTML = originalContents;
}