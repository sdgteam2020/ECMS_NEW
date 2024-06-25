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
                $("#Name_ICardPrint").html(response.Name);
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