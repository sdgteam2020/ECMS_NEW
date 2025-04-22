$(async function () {
    var selectionButton;

    var RemarkTypeID = [5];
    GetRemarks("ddlFaultyRemark", 0, RemarkTypeID);

    $('.select2').select2({
        placeholder: "Please select a Reason",
        allowClear: true,
        closeOnSelect: false // Only needed for multi-select
    });

    let TrnFaultyCardId = parseInt($("#spnTrnFaultyCardId").html());

    if (TrnFaultyCardId > 0) {

        await GetTrnFaultyCardDetail(TrnFaultyCardId);
    }
    else {
        if (sessionStorage.getItem("ArmyNo") != null && sessionStorage.getItem("RequestIdForFaulty") != null && sessionStorage.getItem("MaxTrnFwdId") != null) {
            debugger;
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
            $("#spnFaultyCardRequestId").html(decryptedRequestId);
            $("#spnMaxTrnFwdId").html(decryptedMaxTrnFwdId);
            $("#lblFaultyRequestId").html(decryptedRequestId);

            GetBasicDetailForParitalViewByRequestId(decryptedRequestId);

            
        }
    }

    if ($("#spnClaimValue").html().toLowerCase() === "true") {
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
        GetICardPrintPreviewByRequestId($("#spnFaultyCardRequestId").html());
    });


    $("#btnXMLDownload").on("click", function () {
        DownloadPdf($("#spnFaultyCardRequestId").html());
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory($("#spnFaultyCardRequestId").html());
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
        let Remarks = $("#txtFromRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following faulty card details:',
            html: `
                    <div style="text-align: left; font-size: 16px;">
                        <p><strong>Applicant Name:</strong> ${ApplicantNameWithRank}</p>
                        <p><strong>Request ID:</strong> ${$("#spnFaultyCardRequestId").html() }</p>
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
        var FaultyRemarkIds = "" + $("#ddlFaultyRemark").val() + "";
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
            data: {
                "TrnFaultyCardId": $("#spnTrnFaultyCardId").html(),
                "RequestId": $("#spnFaultyCardRequestId").html(),
                "TrnFwdId": $("#spnMaxTrnFwdId").html(),
                "RemarksIds": $("#ddlFaultyRemark").val().length > 0 ? FaultyRemarkIds : null,
                "FromRemark": $("#txtFromRemark").val(),
                "ToRemark": $("#txtToRemark").val(),
                "CategoryId": $("#ddlStage").val(),
                "Choice": choice
            }, //get the search string
            success: function (result) {

                if (result.Result == true) {
                    const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                    const btnSearchNew = document.getElementById("btnSearchNew");
                    const btnBackDashboard = document.getElementById("btnBackDashboard");
                    let Message;
                    if (parseInt($("#spnTrnFaultyCardId").html()) > 0)
                        Message = `Record successfully updated in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;
                    else
                        Message = `Record successfully inserted in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;

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
async function GetTrnFaultyCardDetail(TrnFaultyCardId) {
    let param = new URLSearchParams({ TrnFaultyCardId: TrnFaultyCardId });

    fetch('/BasicDetail/GetTrnFaultyCardDetail', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: param
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(result => {
            if (result != null) {

                $("#spnArmyNo").html(result.ServiceNo);
                $("#spnFaultyCardRequestId").html(result.RequestId);
                $("#lblFaultyRequestId").html(result.RequestId);
                $("#txtFromRemark").text(result.FromRemark);
                $("#txtFromRemark").prop("disabled", true);

                GetBasicDetailForParitalViewByRequestId(result.RequestId);

                mMsater(result.CategoryId, "ddlStage", FaultyStage, "");

                let RemarksIds = result.RemarksIds;
                let arr2 = RemarksIds.split(',');
                $("#ddlFaultyRemark").val(arr2);
                $("#ddlFaultyRemark").trigger("change");
                $("#ddlFaultyRemark").prop("disabled", true)
            }
            else {
                toastr.error('Invalid Input.');
                window.location.href = '/BasicDetail/FaultyCard';
            }
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