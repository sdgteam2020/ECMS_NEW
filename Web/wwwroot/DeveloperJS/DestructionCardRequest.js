$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    let oldText = "";
    let oldMoment = null;
    const now = moment();                 // current date-time
    const max = moment().add(1, 'month'); // +1 month

    if ($('#txtDestructiononinp').data('DateTimePicker')) {
        $('#txtDestructiononinp').data('DateTimePicker').destroy();
    }

    $('#txtDestructiononinp').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        sideBySide: true,
        stepping: 5,
        useCurrent: false,
        minDate: now,
        maxDate: max,
        showClear: false,
        showClose: false
    }).on('dp.show', function () {

        const picker = $(this).data('DateTimePicker');

        oldText = $(this).val();
        oldMoment = picker.date() ? picker.date().clone() : null;

        picker.minDate(moment());

        setTimeout(function () {
            const $widget = $('.bootstrap-datetimepicker-widget:visible').last();
            if (!$widget.length) return;

            // add buttons once
            if ($widget.find('.dtp-okcancel').length === 0) {
                $widget.append(`
                <div class="dtp-okcancel">
                    <button type="button" class="btn btn-sm btn-secondary dtp-cancel">Cancel</button>
                    <button type="button" class="btn btn-sm btn-success ms-2 dtp-ok">OK</button>
                </div>
            `);

                // OK
                $widget.on('click', '.dtp-ok', function () {
                    picker.hide();
                });

                // Cancel
                $widget.on('click', '.dtp-cancel', function () {
                    if (oldMoment) picker.date(oldMoment);
                    else picker.clear();
                    $('#txtDestructiononinp').val(oldText);
                    picker.hide();
                });
            }
        }, 0);
    });
    $('#txtDestructiononinp').on('keydown', (e) => {
        e.preventDefault();
        return false;
    });    


    var RemarkTypeID = [6];
    GetRemarks("ddlDestructionRemark", 0, RemarkTypeID);

    $('.select2').select2({
        placeholder: "Please select a Reason",
        allowClear: true,
        closeOnSelect: false // Only needed for multi-select
    });

    $("#btnSubmit").on("click", function () {
        Proceed();
    });

    $("#btnReset").on("click", function () {
        Reset();
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId($("#spnDestructionCardRequestId").html());
    });

    $("#btnDestructionCardsList").on("click", function () {
        window.location.href = '/BasicDetail/DestructionCard';
    });
    $("#btnXMLDownload").on("click", function () {
        DownloadPdf($("#spnDestructionCardRequestId").html());
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory($("#spnDestructionCardRequestId").html());
        $("#exampleModal").modal('show');
    });

    $("#btnCardHistory").on("click", function () {
        GetMovementHistory($("#spnDestructionCardRequestId").html());
        $("#exampleModal").modal('show');
    });

    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/BasicDetail/DestructionCard';
    });

    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(DestructionCardRequest);
    });



    $('#declarationCheckbox').on('change', function () {
        $('#btnSubmit').prop('disabled', !this.checked);
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
                    let MaxTrnFwdId = response.Value.MaxTrnFwdId

                    $("#spnArmyNo").html(ArmyNo);
                    $("#spnDestructionCardRequestId").html(RequestIdForFaulty);
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

    let formId = '#SaveDestructionCardRequest';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        let inputVal = $("#txtDestructiononinp").val();
        const parsedDate = new Date(inputVal);
        if (!isValidDate(parsedDate)) {
            $(formId).validate().showErrors({
                "txtDestructiononinp": "Invalid Date Of Destruction"
            });
            return false;
        }

        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtDestructionRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following card destruction details:',
            html: `
                    <div class="swal-details">
                        <p><strong>Card Holder Name:</strong> ${ApplicantNameWithRank}</p>
                        <p><strong>Date Of Destruction:</strong> ${DateFormateddMMyyyyhhmmss(parsedDate)}</p>
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
    let inputDate = $("#txtDestructiononinp").val();
    var DestructionlistRemarkIds = "" + $("#ddlDestructionRemark").val() + "";
    $.ajax({
        url: '/BasicDetail/SaveDestructionCardRequest' ,
        type: 'POST',
        data: {
            "RequestId": $("#spnDestructionCardRequestId").html(),
            "RemarksIds": $("#ddlDestructionRemark").val().length > 0 ? DestructionlistRemarkIds : null,
            "DestructedOn": formatDateToSqlString(inputDate),
            "Remark": $("#txtDestructionRemark").val()
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
            'RequestVerificationToken': globalThis.RequestVerificationToken
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
    $("#txtDestructiononinp-error").text("");
    $("#txtDestructionRemark-error").text("");
}


function Reset() {
    ResetErrorMessage();
    //$('.select2-selection__clear').trigger('click');
    $('.select2').val(null).trigger('change');
    $('#SaveDestructionCardRequest').find(':input')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .prop('checked', false);
    $('#btnSubmit').prop('disabled', true);
}