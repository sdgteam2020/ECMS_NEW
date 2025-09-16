$(function () {
$("#btn-i-card-print").on("click",function () {
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
            if (response.Result === true) {
                $(".PhotoImagePath_ICardPrint").attr('src', response.Value.ExistingPhotoInBase64);
                $(".SignaturePath_ICardPrint").attr('src', response.Value.ExistingSignatureInBase64);
                $("#FName_ICardPrint").html(response.Value.FName);
                $("#LName_ICardPrint").html(response.Value.LName);
                $("#RankName_ICardPrint").html(response.Value.RankName);
                $("#ArmedName_ICardPrint").html(response.Value.ArmedName);
                if (/^[A-Za-z]{2}/.test(response.Value.ServiceNo)) {
                    // Insert space after first two characters
                    let temp = response.Value.ServiceNo.slice(0, 2) + ' ' + response.Value.ServiceNo.slice(2);
                    $("#ServiceNo_ICardPrint").html(temp);
                } else {
                    // No space needed
                    $("#ServiceNo_ICardPrint").html(response.Value.ServiceNo);
                }
                $("#IdenMark1_ICardPrint").html(response.Value.IdenMark1);
                $("#DOB_ICardPrint").html(DateFormateMMMM_dd_yyyy(response.Value.DOB));
                $("#Height_ICardPrint").html(response.Value.Height + ' CM');
                $("#AadhaarNo_ICardPrint").html(response.Value.AadhaarNo.replace(/\d(?=\d{4})/g, "X"));
                $("#BloodGroup_ICardPrint").html(response.Value.BloodGroup);
                $("#PlaceOfIssue_ICardPrint").html(response.Value.PlaceOfIssue);
                $("#DateOfIssue_ICardPrint").html(response.Value.DateOfIssue == null ? 'DEPENDS ON UNIT OF SECOND LEVEL APPROVER.' : DateFormateMMMM_dd_yyyy(response.Value.DateOfIssue));
                $(".IssuingAuth_ICardPrint").html(response.Value.IssuingAuthorityName);
                $(".DateOfCommissioning_ICardPrint").html(DateFormateMMMM_dd_yyyy(response.Value.DateOfCommissioning));
                $("#ICardPrint").modal('show');
                //$("#lblfdaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);
            }
            else {
                toastr.error(response.Message);
            }
        }
    })
}
//function GetBasicDetailByRequestId(RequestId) {
//    var userdata = {
//        "RequestId": RequestId,
//    };
//    $.ajax({
//        url: '/BasicDetail/GetBasicDetailByRequestId',
//        contentType: 'application/x-www-form-urlencoded',
//        data: userdata,
//        type: 'POST',

//        success: function (response) {
//            if (response != "null" && response != null) {
//                $("#basicphotosVP").attr('src', response.ExistingPhotoInBase64);
//                $("#BasicsingVP").attr('src', response.ExistingSignatureInBase64);
//                $("#lblvpNameAsPerRecord").html(response.NameAsPerRecord);
//                $("#lblvpFName").html(response.FName);
//                $("#lblvpLName").html(response.LName);
//                $("#lblvpRank").html(response.RankName);
//                $("#lblvparm").html(response.ArmedName);
//                $("#lblvpArmyNo").html(response.ModifiedServiceNo);
//                $("#lblvpMarks").html(response.IdenMark1);
//                $("#lblvpdob").html(DateFormateMMMM_dd_yyyy(response.DOB));
//                $("#lblvpheight").html(response.Height);
//                $("#lblvpadhar").html(response.AadhaarNo.replace(/\d(?=\d{4})/g, "X"));
//                $("#lblvpBloodGroup").html(response.BloodGroup);
//                $("#lblvppoi").html(response.PlaceOfIssue);
//                $("#lblvpdoi").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
//                $("#lblvpissuA").html(response.IssuingAuthorityName);
//                $("#lblvpdateo").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
//                $("#lblvpaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode);
//                $("#BasicDetailViewPurpose").modal('show');
//            }
//        }
//    })
//}
function GetBasicDetailByRequestId(RequestId) {
    let param = new URLSearchParams({ RequestId: RequestId });

    fetch('/BasicDetail/GetBasicDetailForParitalViewByRequestId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: param
    })
        .then(response => response.text())
        .then(html => {
            document.getElementById("BasicDetailViewPurpose_Data").innerHTML = html;
            $("#BasicDetailViewPurpose").modal('show');
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
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