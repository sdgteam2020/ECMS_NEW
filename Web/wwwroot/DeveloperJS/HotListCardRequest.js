var spnHotlistCardRequestId = 0;
$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    var RemarkTypeID = [6];
    GetRemarks("ddlHotlistRemark", 0, RemarkTypeID);

    $('.select2').select2({
        placeholder: "Please select a Reason",
        allowClear: true,
        closeOnSelect: false // Only needed for multi-select
    });

    $("#btnAccept").on("click", function () {
        Proceed();
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId(spnHotlistCardRequestId);
    });


    $("#btnXMLDownload").on("click", function () {
        DownloadPdf(spnHotlistCardRequestId);
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory(spnHotlistCardRequestId);
        $("#exampleModal").modal('show');
    });

    $("#btnHotlistCardsList").on("click", function () {
        window.location.href = '/BasicDetail/HotlistCard';
    });

    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(HoltlistCardRequest);
    });

    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/BasicDetail/HotlistCard';
    });

    return new Promise((resolve, reject) => {
        fetch('/BasicDetail/DataRecForGetSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
                'RequestVerificationToken': globalThis.RequestVerificationToken
            }
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    let ArmyNo = response.Value.ArmyNo;
                    let RequestIdForFaulty = response.Value.RequestIdForFaulty;

                    $("#spnArmyNo").html(ArmyNo);
                    spnHotlistCardRequestId = RequestIdForFaulty;
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
    if ($("#ddlHotlistRemark").val().length == 0) {
        toastr.error('Reason is required.');
        return false;
    }
    if ($("#txtHotlistRemark").val().length == 0) {
        toastr.error('Remark is required.');
        return false;
    }

    let formId = '#SaveHotlistCardRequest';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtHotlistRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following hotlist card details:',
            html: `
                    <div class="text-start fs-6">
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
    var HotlistRemarkIds = $("#ddlHotlistRemark").val();
    $.ajax({
        url: '/BasicDetail/SaveHotlistCardRequest',
        type: 'POST',
        data: {
            "RequestId": spnHotlistCardRequestId,
            "RemarksIds": HotlistRemarkIds && HotlistRemarkIds.length > 0 ? HotlistRemarkIds.map(Number) : null,
            "Remark": $("#txtHotlistRemark").val()
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                const btnSearchNew = document.getElementById("btnSearchNew");
                const btnBackDashboard = document.getElementById("btnBackDashboard");
                let Message = `Record successfully inserted in DB with ID : <strong>${result.Value.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.Value.CurrentTime)}</strong>.`;

                document.getElementById("ConfirmationDialog_Data").innerHTML = Message;
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
            document.getElementById("partialContainerBD").innerHTML = html;
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
}
function DownloadPdf(RequestId) {
    try {
        const encryptedRequest = encryptPayloadData(RequestId);

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Log/CreatePdf';
        form.target = '_blank';
        form.style.display = 'none';

        const requestInput = document.createElement('input');
        requestInput.type = 'hidden';
        requestInput.name = 'Request';
        requestInput.value = encryptedRequest;
        form.appendChild(requestInput);

        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = globalThis.RequestVerificationToken;
        form.appendChild(tokenInput);

        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    } catch (e) {
        Swal.fire({
            text: errormsg002
        });
    }
}