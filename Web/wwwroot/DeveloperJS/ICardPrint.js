$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    $("#btn-i-card-print").on("click", function () {
        PrintData("section-to-print-popup");
    });
});
function GetICardPrintPreviewByRequestId(RequestId) {
    var userdata =
    {
        "Request": encryptPayloadData(RequestId),
    }; 
    $.ajax({
        url: '/BasicDetail/GetICardPrintPreviewByRequestId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

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
function GetBasicDetailByRequestId(RequestId) {
    let param = new URLSearchParams({ Request: encryptPayloadData(RequestId) });

    fetch('/BasicDetail/GetBasicDetailForParitalViewByRequestId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
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
    const divToPrint = document.getElementById('ICardPrint');
    if (!divToPrint) return;

    const newWin = window.open('', 'Print-Window');

    newWin.document.open();
    newWin.document.write('<html><head><title>Print</title></head><body>');
    newWin.document.write(divToPrint.innerHTML);
    newWin.document.write('</body></html>');
    newWin.document.close();

    // Attach onload handler via JS (CSP-safe)
    newWin.onload = function () {
        newWin.focus();
        newWin.print();
        newWin.close();
    };
}