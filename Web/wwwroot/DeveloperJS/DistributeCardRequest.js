$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

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
        $("#armynosearchTypeId").val(DistributeCardRequest);
    });



    $('#declarationCheckbox').on('change', function () {
        $('#btnSubmit').prop('disabled', !this.checked);
    });

    updateDateTime();
    setInterval(updateDateTime, 1000);
    return new Promise((resolve, reject) => {
        fetch('/BasicDetail/DataRecForGetSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
                'RequestVerificationToken': globalThis.RequestVerificationToken,
            }
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    let ArmyNo = response.Value.ArmyNo;
                    let RequestIdForFaulty = response.Value.RequestIdForFaulty;
                    let MaxTrnFwdId = response.Value.MaxTrnFwdId

                    $("#spnArmyNo").html(ArmyNo);
                    $("#spnDistributeCardRequestId").html(RequestIdForFaulty);
                    $("#spnMaxTrnFwdId").html(MaxTrnFwdId);
                    $("#lblFaultyRequestId").html(RequestIdForFaulty);

                    GetBasicDetailForParitalViewByRequestId(RequestIdForFaulty);

                    resolve(response);
                } else {
                    toastr.error("Failed to Fetch Session Value: " + response.Message);
                    reject(new Error(response.Message));
                }
            })
            .catch((error) => {
                toastr.error("Failed to Fetch Session Value : " + response.Message);
                reject(new Error("Failed to Fetch Session Value : " + error.message));
            });
    });

});
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveDistributeCardRequest';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        //let inputVal = $("#txtDistributeoninp").val();
        //const parsedDate = new Date(inputVal);
        //if (!isValidDate(parsedDate)) {
        //    $(formId).validate().showErrors({
        //        "txtDistributeoninp": "Invalid Date Of Distribution"
        //    });
        //    return false;
        //}

        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtDistributeRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following card distribution details:',
            html: `
                    <div class="text-start fs-6">
                        <p><strong>Card Holder Name:</strong> ${ApplicantNameWithRank}</p>
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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken,
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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
function updateDateTime() {
    const now = new Date();
    const formatted = now.getFullYear() + '-' +
        ('0' + (now.getMonth() + 1)).slice(-2) + '-' +
        ('0' + now.getDate()).slice(-2) + ' ' +
        ('0' + now.getHours()).slice(-2) + ':' +
        ('0' + now.getMinutes()).slice(-2) + ':' +
        ('0' + now.getSeconds()).slice(-2);

    $('#txtDistributeoninp').text(formatted);
}