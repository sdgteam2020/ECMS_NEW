$(async function () {
    var RemarkTypeID = [6];
    GetRemarks("ddlLostRemark", 0, RemarkTypeID);
      
    $('.select2').select2({
        placeholder: "Please select a Reason",
        allowClear: true,
        closeOnSelect: false // Only needed for multi-select
    });

    
    if (sessionStorage.getItem("ArmyNo") != null && sessionStorage.getItem("RequestIdForFaulty") != null && sessionStorage.getItem("MaxTrnFwdId") != null) {

        var encryptedArmyNo = sessionStorage.getItem("ArmyNo");
        var encryptedRequestId = sessionStorage.getItem("RequestIdForFaulty");
        var encryptedMaxTrnFwdId = sessionStorage.getItem("MaxTrnFwdId");

        var secretKey = document.getElementById("spnUniqueSecretKey").innerText;

        var bytes = CryptoJS.AES.decrypt(encryptedArmyNo, secretKey);
        var decryptedArmyNo = bytes.toString(CryptoJS.enc.Utf8);

        var bytes = CryptoJS.AES.decrypt(encryptedRequestId, secretKey);
        var decryptedRequestId = bytes.toString(CryptoJS.enc.Utf8);

        var bytes = CryptoJS.AES.decrypt(encryptedMaxTrnFwdId, secretKey);
        var decryptedMaxTrnFwdId = bytes.toString(CryptoJS.enc.Utf8);


        $("#spnArmyNo").html(decryptedArmyNo);
        $("#spnLostCardRequestId").html(decryptedRequestId);
        $("#spnMaxTrnFwdId").html(decryptedMaxTrnFwdId);
        $("#lblFaultyRequestId").html(decryptedRequestId);

        GetBasicDetailForParitalViewByRequestId(decryptedRequestId);

    }

    $("#btnAccept").on("click", function () {
        Proceed();
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId($("#spnLostCardRequestId").html());
    });


    $("#btnXMLDownload").on("click", function () {
        DownloadPdf($("#spnLostCardRequestId").html());
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory($("#spnLostCardRequestId").html());
        $("#exampleModal").modal('show');
    });

    $("#btnLostCardsList").on("click", function () {
        window.location.href = '/BasicDetail/LostCard';
    });

    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(LostCardRequest);
    });

    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/BasicDetail/LostCard';
    });
});

function Proceed() {
    //ResetErrorMessage();
    let inputVal = $("#txtlostoninp").val();
    if (inputVal.length == 0) {
        toastr.error('Lost On is required.');
        return false;
    }

    const parsedDate = new Date(inputVal);
    if (!isValidDate(parsedDate)) {
        toastr.error('Lost On value is invalid.');
        return false;
    } 

    if ($("#txtLostRemark").val().length == 0) {
        toastr.error('Remark is required.');
        return false;
    }

    let formId = '#SaveLostCardRequest';
    $.validator.unobtrusive.parse($(formId));
    
    if ($(formId).valid()) {
        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtLostRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following Lost card details:',
            html: `
                    <div style="text-align: left; font-size: 16px;">
                        <p><strong>Applicant Name:</strong> ${ApplicantNameWithRank}</p>
                        <p><strong>Remarks:</strong> ${Remarks}</p>
                        <p><strong>Logged In Details:</strong> ${UserName}</p>
                    </div>
                  `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm',
            cancelButtonText: 'Cancel',
            width: '500px', // optional: customize popup width
        }).then((result) => {
            if (result.isConfirmed) {
                Save();
            }
        })
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        })
        toastr.error('Please fill required field.');
        return false;
    }
}
function Save() {
    let inputDate = $("#txtlostoninp").val();
    $.ajax({
        url: '/BasicDetail/SaveLostCardRequest' ,
        type: 'POST',
        data: {
            "LostCardId": 0,
            "RequestId": $("#spnLostCardRequestId").html(),
            "TrnFwdId": $("#spnMaxTrnFwdId").html(),
            "Remark": $("#txtLostRemark").val(),
            "LostOn": formatDateToSqlString(inputDate)
        },
        success: function (result) {

            if (result.Result == true) {
                const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                const btnSearchNew = document.getElementById("btnSearchNew");
                const btnBackDashboard = document.getElementById("btnBackDashboard");
                let Message = `Record successfully inserted in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;

                document.getElementById("ConfirmationDialog_Data").innerHTML= Message;
                btnSearchNew.textContent = "Search New";
                btnBackDashboard.textContent = "Back to Dashboard";
                myModal.show();
            }
            else {
                toastr.error(result.Message);
            }
        }
    });
}
function GetBasicDetailForParitalViewByRequestId(RequestId) {
    let param = new URLSearchParams({ RequestId: RequestId });

    fetch('/BasicDetail/GetBasicDetailForParitalViewByRequestId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: param
    })
    .then(response => response.text())
    .then(html =>{
            document.getElementById("partialContainerBD").innerHTML = html;
    })
    .catch(error => {
       alert("Error: " + error.message);
    });
}
function DownloadPdf(RequestId) {
    var userdata = {
        "RequestId": RequestId,
    };
    $.ajax({
        url: '/Log/CreatePdf',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else {

                    var url = "https://" + window.location.host + '/DigitallysignaturePdf/' + response;
                    window.open(url, '_blank');

                }
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}