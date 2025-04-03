$(function () {

        if (sessionStorage.getItem("ArmyNo") != null && sessionStorage.getItem("RequestIdForFaulty") != null) {
            $("#spnArmyNo").html(sessionStorage.getItem("ArmyNo"));
            GetBasicDetailForParitalViewByRequestId(sessionStorage.getItem("RequestIdForFaulty"));
            $("#spnFaultyCardRequestId").html(sessionStorage.getItem("RequestIdForFaulty"));
            $("#lblFaultyRequestId").html(sessionStorage.getItem("RequestIdForFaulty"));
            var RemarkTypeID = [5];
            GetRemarks("ddlFaultyRemark", 0, RemarkTypeID);
            mMsater(2, "ddlStage", FaultyStage, "");
            $('.select2').select2({
                placeholder: "Please select a Reason",
                allowClear: true,
                closeOnSelect: false // Only needed for multi-select
            });
        }

    $("#btnSubmit").on("click", function () {
        Proceed();
    });

    $("#btnCardPreview").on("click", function () {
        GetICardPrintPreviewByRequestId(sessionStorage.getItem("RequestIdForFaulty"));
    });

    $("#btnXMLDownload").on("click", function () {
        DownloadPdf(sessionStorage.getItem("RequestIdForFaulty"));
    });

    $("#btnApplMoveHistory").on("click", function () {
        GetRequestHistory(sessionStorage.getItem("RequestIdForFaulty"));
        $("#exampleModal").modal('show');
    });

    $("#btnFaultyCardsList").on("click", function () {
        location.href = '/BasicDetail/FaultyCard';
    });

    $("#btnSearchNew").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(FaultyCardRequest);
    });

    $("#btnBackDashboard").on("click", function () {
        location.href = '/BasicDetail/FaultyCard';
    });
});
function Proceed() {
    //ResetErrorMessage();
    if ($("#ddlFaultyRemark").val().length == 0 ) {
        toastr.error('Reason is required.');
        return false;
    }

    let formId = '#SaveFaultyCardRequest';
    $.validator.unobtrusive.parse($(formId));
    
    if ($(formId).valid()) {
        let ApplicantName = $("#lblpvFName").html() + $("#lblpvLName").html();
        let ApplicantNameWithRank = $("#lblpvRank").html() + " " + ApplicantName.trim();
        let Remarks = $("#txtFromRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        let UnitAbbreviation = $("#spnUnitAbbreviation").html();
        Swal.fire({
            title: 'Pl confirm the fwg faulty card:-',
            html: "Applicant Name - " + ApplicantNameWithRank + "<br/>Request Id - " + sessionStorage.getItem("RequestIdForFaulty") + "<br/>Remarks - " + Remarks + "<br/>User Details - " + UserName +"," +UnitAbbreviation +"" ,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm'
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
        var FaultyRemarkIds = "" + $("#ddlFaultyRemark").val() + "";
        $.ajax({
            url: '/BasicDetail/SaveFaultyCardRequest',
            type: 'POST',
            data: {
                "TrnFaultyCardId": $("#spnTrnFaultyCardId").html(),
                "RequestId": $("#spnFaultyCardRequestId").html(),
                "RemarksIds": $("#ddlFaultyRemark").val().length > 0 ? FaultyRemarkIds : null,
                "FromRemark": $("#txtFromRemark").val(),
                "CategoryId": $("#ddlStage").val(),
            }, //get the search string
            success: function (result) {

                if (result.Result == true) {
                    const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                    const btnSearchNew = document.getElementById("btnSearchNew");
                    const btnBackDashboard = document.getElementById("btnBackDashboard");
                    let Message = "Record successfully inserted in DB with ID - " + result.Id + " & TS - " + DateFormateddMMyyyyhhmmss(result.CurrentTime) +".";
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