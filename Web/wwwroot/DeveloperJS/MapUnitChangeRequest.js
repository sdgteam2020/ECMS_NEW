$(async function () {
    var selectionButton;

    let ChangeMapUnitId = parseInt($("#spnChangeMapUnitId").html());
    let RoleName = $("#spnRoleName").html()?.toLowerCase();

    if (ChangeMapUnitId > 0 && RoleName === "admin") {

    }
    else {
        mMsater(0, "ddlCommand", 1, "");
        await GetUnitDetails(parseInt($("#spnUnitMapId").html()));

    }

    $('input[name="UnitTyperdi"]').on("click", function () {
        var lst = '<option value="1">Please Select</option>';
        var val = $("input[type='radio'][name=UnitTyperdi]:checked").val();
        if (val == "1") {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();

            mMsater(0, "ddlCommand", 1, "");

            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

        }
        else if (val == "2") {

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();
            $('#ddlFmnBranch option').remove();

            mMsater(0, "ddlCommand", 1, "");
            mMsater(0, "ddlFmnBranch", FmnBranches, "");

            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
            $(".DteBranch").addClass("d-none");
        }
        else if (val == "3") {
            $(".unittype").addClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").removeClass("d-none");

            $('#ddlPSODte option').remove();
            $('#ddlDgSubDte option').remove();

            $("#ddlCommand").html(lst);
            $("#ddlCorps").html(lst);
            $("#ddlBde").html(lst);
            $("#ddlDiv").html(lst);
            $("#ddlFmnBranch").html(lst);

            mMsater(0, "ddlPSODte", PSO, "");
            mMsater(0, "ddlDgSubDte", SubDte, "");

        }
    });

    $('#ddlCommand').on('change', function () {
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
    });

    $('#ddlCorps').on('change', function () {
        mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0); ///ComdId,CorpsId,DivId,BdeId
    });
    $('#ddlDiv').on('change', function () {
        mMsaterByParent(0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0); ///ComdId,CorpsId,DivId,BdeId
    });

    if (ChangeMapUnitId > 0 && RoleName === "admin") {
        $("#btnSubmit").addClass("d-none");
        $("#btnAccept").removeClass("d-none");
        $("#btnReject").removeClass("d-none");

    } else {
        $("#btnSubmit").removeClass("d-none");
        $("#btnAccept").addClass("d-none");
        $("#btnReject").addClass("d-none");

    }
    $("#btnBackDashboard").on("click", function () {
        window.location.href = '/Master/MapUnitChange';
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

});
function Proceed(choice) {
    //ResetErrorMessage();

    if ((choice == 2 || choice == 3) && $("#txtAdminRemark").val().length == 0) {
        toastr.error('Admin Remark is required.');
        return false;
    }

    let formId = '#SaveMapUnitChange';
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
        let UnitName = $("#lblUnit").html();
        let SUSNo = $("#lblSUSNo").html();
        let Remarks = $("#txtRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please confirm the following Unit Mapping Change Request details:',
            html: `
                    <div style="text-align: left; font-size: 16px;">
                        <p><strong>Unit Name:</strong> ${UnitName.trim()}</p>
                        <p><strong>SUS NO.:</strong> ${SUSNo.trim()}</p>
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
    let urladd;

    if (choice === 1) {
        urladd = '/Master/SaveMapUnitChangeRequest';
    }
    else {
        urladd = '/Master/SaveMapUnitChange';
    }
    $.ajax({
        url: urladd,
        type: 'POST',
        data: {
            "ChangeMapUnitId": document.getElementById('spnChangeMapUnitId').textContent,
            "UnitMapId": document.getElementById('spnUnitMapId').textContent,
            "Remark": document.getElementById('txtRemark').value,
            "AdminRemark": document.getElementById('txtAdminRemark').value,
            "Choice": choice,
            "UnitType": document.querySelector('input[type="radio"][name="UnitTyperdi"]:checked')?.value,
            "ComdId": document.getElementById('ddlCommand').value,
            "CorpsId": document.getElementById('ddlCorps').value,
            "DivId": document.getElementById('ddlDiv').value,
            "BdeId": document.getElementById('ddlBde').value,
            "PsoId": document.getElementById('ddlPSODte').value,
            "FmnBranchID": document.getElementById('ddlFmnBranch').value,
            "SubDteId": document.getElementById('ddlDgSubDte').value
        }, //get the search string
        success: function (result) {
            if ($("#spnRoleName").html().toLowerCase() === "admin" && result.Result == true) {
                $("#spnChangeMapUnitId").html(result.Id);
            }

            if (result.Result == true) {
                const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                const btnSearchNew = document.getElementById("btnSearchNew");
                const btnBackDashboard = document.getElementById("btnBackDashboard");
                let Message;
                if (parseInt($("#spnChangeMapUnitId").html()) > 0)
                    Message = `Record successfully updated in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;
                else
                    Message = `Record successfully inserted in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;

                document.getElementById("ConfirmationDialog_Data").innerHTML = Message;
                btnSearchNew.classList.add("d-none");
                btnBackDashboard.textContent = "Back to Dashboard";
                myModal.show();
            }
            else {
                toastr.error(result.Message);
            }
        }
    });
}
async function GetUnitDetails(UnitMapId) {
    let param = new URLSearchParams({ UnitMapId: UnitMapId });

    fetch('/Master/GetALLByUnitMapId', {
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
                $("#lblUnit").html(result.UnitName);
                $("#lblSUSNo").html(`${result.Sus_no}${result.Suffix}`);
                if (result.UnitType == 1) {
                    $("#lblUnitType").html(`Unit`);
                }
                else if (result.UnitType == 2) {
                    $("#lblUnitType").html(`Fmn HQ`);
                }
                else if (result.UnitType == 3) {
                    $("#lblUnitType").html(`Dte / Sub Dte Branch`);
                }

                $("#lblComd").html(result.ComdName);
                $("#lblCorps").html(result.CorpsName);
                $("#lblDiv").html(result.DivName);
                $("#lblBde").html(result.BdeName);
                $("#lblFmnBranch").html(result.BranchName);
                $("#lblPSODte").html(result.PSOName);
                $("#lblDgSubDte").html(result.SubDteName);

            }
            else {
                toastr.error('Invalid Input.');
                window.location.href = '/Home/Dashboard';
            }
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