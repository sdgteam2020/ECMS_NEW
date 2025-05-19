$(async function () {
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
        $("#spnDistributeCardRequestId").html(decryptedRequestId);
        $("#spnMaxTrnFwdId").html(decryptedMaxTrnFwdId);
        $("#lblFaultyRequestId").html(decryptedRequestId);

        GetBasicDetailForParitalViewByRequestId(decryptedRequestId);

    }

    $("#btnSubmit").on("click", function () {
        Proceed();
    });

    $("#btnReset").on("click", function () {
        Reset();
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId($("#spnDistributeCardRequestId").html());
    });

    $("#btnDistributeCardsList").on("click", function () {
        window.location.href = '/BasicDetail/DistributeCard';
    });
    $("#btnXMLDownload").on("click", function () {
        DownloadPdf($("#spnDistributeCardRequestId").html());
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory($("#spnDistributeCardRequestId").html());
        $("#exampleModal").modal('show');
    });

    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/BasicDetail/DistributeCard';
    });

    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(HoltlistCardRequest);
    });

    

    $('#declarationCheckbox').on('change', function () {
        $('#btnSubmit').prop('disabled', !this.checked);
    });
});
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveDistributeCardRequest';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        let inputVal = $("#txtDistributeoninp").val();
        const parsedDate = new Date(inputVal);
        if (!isValidDate(parsedDate)) {
            $(formId).validate().showErrors({
                "txtDistributeoninp": "Invalid Date Of Distribution"
            });
            return false;
        }

        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtDistributeRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following card distribution details:',
            html: `
                    <div style="text-align: left; font-size: 16px;">
                        <p><strong>Card Holder Name:</strong> ${ApplicantNameWithRank}</p>
                        <p><strong>Date Of Distribution:</strong> ${DateFormateddMMyyyyhhmmss(parsedDate)}</p>
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
        return false;
    }
}
function Save() {
    let inputDate = $("#txtDistributeoninp").val();
    $.ajax({
        url: '/BasicDetail/SaveDistributeCardRequest' ,
        type: 'POST',
        data: {
            "RequestId": $("#spnDistributeCardRequestId").html(),
            "DistributedOn": formatDateToSqlString(inputDate),
            "Remark": $("#txtDistributeRemark").val()
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
        //let $html = $('<div>').html(html);
        //$html.find('#basicDetailsButtons').append('');
        //let updatedHtmlString = $html.html();
        document.getElementById("partialContainerBD").innerHTML = html;
        //BindParitalViewEvents();
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

function ResetErrorMessage() {
    $("#txtDistributeoninp-error").text("");
    $("#txtDistributeRemark-error").text("");
}


function Reset() {
    ResetErrorMessage();
    //$('.select2-selection__clear').trigger('click');
    $('.select2').val(null).trigger('change');
    $('#SaveDistributeCardRequest').find(':input')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .prop('checked', false);
    $('#btnSubmit').prop('disabled', true);
}