var spnFaultyCardRequestId = 0;
$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    var selectionButton;

    var RemarkTypeID = [5];
    await GetRemarks("ddlFaultyRemark", 0, RemarkTypeID);

    $('#ddlFaultyRemark').select2({
        placeholder: "Please select a Reason",
        width: '100%',
        allowClear: true,
        closeOnSelect: false
    });

    $("#btnSubmit").on("click", function () {
        selectionButton = 1;
        Proceed(selectionButton);
    });
    $("#btnAccept").on("click", function () {
        selectionButton = 2;
        Proceed(selectionButton);
    });
    $("#btnReject").on("click", function () {
        selectionButton = 3;
        Proceed(selectionButton);
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId(spnFaultyCardRequestId);
    });


    $("#btnXMLDownload").on("click", function () {
        DownloadPdf(spnFaultyCardRequestId);
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory(spnFaultyCardRequestId);
        $("#exampleModal").modal('show');
    });

    $("#btnFaultyCardsList").on("click", function () {
        window.location.href = '/BasicDetail/FaultyCard';
    });

    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(FaultyCardRequest);
    });

    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/BasicDetail/FaultyCard';
    });

    if ($("#spnCValue").html().toLowerCase() === "true") {
        $("#btnSubmit").addClass("d-none");
        $("#btnAccept").removeClass("d-none");
        $("#btnReject").removeClass("d-none");

        $(".Stage").addClass("d-none");
        $(".ToRemark").removeClass("d-none");

        mMsater(1, "ddlStage", FaultyStage, "");
    } else {
        $("#btnSubmit").removeClass("d-none");
        $("#btnAccept").addClass("d-none");
        $("#btnReject").addClass("d-none");

        $(".Stage").addClass("d-none");
        $(".ToRemark").addClass("d-none");

        mMsater(2, "ddlStage", FaultyStage, "");
    }

    let TrnFaultyCardId = parseInt($("#spnTrnFaultyCardId").html());

    if (TrnFaultyCardId > 0) {

        await GetTrnFaultyCardDetail(TrnFaultyCardId);
    }
    else {
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
                        spnFaultyCardRequestId = RequestIdForFaulty;
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
    }
});
function Proceed(choice) {
    //ResetErrorMessage();
    if ($("#ddlFaultyRemark").val().length == 0 ) {
        toastr.error('Reason is required.');
        return false;
    }
    if ((choice == 2 || choice == 3) && $("#txtToRemark").val().length == 0) {
        toastr.error('AFSAC Cell Remark is required.');
        return false;
    }

    let formId = '#SaveFaultyCardRequest';
    $.validator.unobtrusive.parse($(formId));
    
    if ($(formId).valid()) {
        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let FromRemark = $("#txtFromRemark").val();
        let ToRemark = $("#txtToRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following faulty card details:',
            html: `
                    <div class="swal-details">
                        <p><strong>Applicant Name:</strong> ${ApplicantNameWithRank}</p>
                        <p><strong>Request ID:</strong> ${spnFaultyCardRequestId}</p>
                        <p><strong>${choice === 1 ? "Issues Related to Card Misprint/Faulty" : "AFSAC Cell Remark"}:</strong> ${choice === 1 ? FromRemark : ToRemark}</p>
                        <p><strong>Logged In Details:</strong> ${UserName}</p>
                    </div>
                  `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm',
            cancelButtonText: 'Cancel',
            customClass: {
                popup: 'swal-popup-500'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                Save(choice);
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
    function Save(choice) {
        var FaultyRemarkIds = $("#ddlFaultyRemark").val();
        let data = {
            "TrnFaultyCardId": $("#spnTrnFaultyCardId").html(),
            "RequestId": spnFaultyCardRequestId,
            "RemarksIds": FaultyRemarkIds && FaultyRemarkIds.length > 0 ? FaultyRemarkIds.map(Number) : null,
            "FromRemark": $("#txtFromRemark").val(),
            "ToRemark": $("#txtToRemark").val(),
            "CategoryId": $("#ddlStage").val(),
            "Choice": choice
        };
        let urladd;

        if (choice === 1) {
            urladd = '/BasicDetail/SaveFaultyCardRequest';
        }
        else {
            urladd = '/BasicDetail/SaveFaultyCard';
        }
        $.ajax({
            url: urladd ,
            type: 'POST',
            data: JSON.stringify(data),
            contentType: "application/json",
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

            success: function (result) {
                if ($("#spnCValue").html().toLowerCase() === "true" && result.Result == true)
                {
                    $("#spnTrnFaultyCardId").html(result.Id);
                }

                if (result.Result == true) {
                    const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                    const btnSearchNew = document.getElementById("btnSearchNew");
                    const btnBackDashboard = document.getElementById("btnBackDashboard");
                    let Message;
                    if (parseInt($("#spnTrnFaultyCardId").html()) > 0)
                    {
                        if (choice === 3) {
                            Message = `Faulty Card Request Rejected successfully.<br/>Record successfully updated in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;
                        }
                        else {
                            Message = `Faulty Card Request Accept successfully.<br/>Record successfully updated in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;
                        }
                    }
                    else
                        Message = `Your Faulty Card Request placed successfully.<br/>Record successfully inserted in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;

                    document.getElementById("ConfirmationDialog_Data").innerHTML= Message;
                    btnSearchNew.textContent = "Search New";
                    btnBackDashboard.textContent = "Back to Dashboard";
                    Reset();
                    myModal.show();
                    //myModal.hide();
                    //toastr.success(result.Message);
                    //location.href = '/BasicDetail/FaultyCard';
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
        .then(html =>{
            document.getElementById("partialContainerBD").innerHTML = html;
        })
        .catch(error => {
        alert("Error: " + error.message);
    });
}
async function GetTrnFaultyCardDetail(TrnFaultyCardId) {

    let param = new URLSearchParams({ TrnFaultyCardId: TrnFaultyCardId });

    try {
        const response = await fetch('/BasicDetail/GetTrnFaultyCardDetail', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (result != null) {

            $("#spnArmyNo").text(result.ServiceNo);
            spnFaultyCardRequestId = result.RequestId;
            $("#lblFaultyRequestId").html(result.RequestId);
            $("#txtFromRemark").text(result.FromRemark);
            $("#txtFromRemark").prop("disabled", true);

            GetBasicDetailForParitalViewByRequestId(result.RequestId);

            await mMsater(result.CategoryId, "ddlStage", FaultyStage, "");

            let RemarksIds = result.RemarksIds;
            let arr2 = RemarksIds.split(',');
            $("#ddlFaultyRemark").val(arr2);
            $("#ddlFaultyRemark").trigger("change");
            $("#ddlFaultyRemark").prop("disabled", true)

        } else {
            toastr.error('Invalid Input.');
            window.location.href = '/BasicDetail/FaultyCard';
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}
function DownloadPdf(RequestId) {
    $.ajax({
        url: '/Log/CreatePdf',
        type: 'POST',
        data: {
            "Request": encryptPayloadData(RequestId)
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (response, status, xhr) {
            var blob = new Blob([response], { type: 'application/pdf' });
            var fileURL = window.URL.createObjectURL(blob);
            window.open(fileURL, '_blank');
        },
        error: function () {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function Reset() {
    //$("#spnTrnFaultyCardId").html("0");
    //$("#spnFaultyCardRequestId").html("0");
    //$("#lblFaultyRequestId").html("");
    //$('#ddlFaultyRemark').val(null).trigger('change');
    //$("#txtFromRemark").val("");
    //$("#ddlStage").val("");

    //sessionStorage.setItem("ArmyNo", null);
    //sessionStorage.setItem("RequestIdForFaulty", null);
}