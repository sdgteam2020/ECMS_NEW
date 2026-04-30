let UnitTyperdi = 0;
$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    var selectionButton;

    let MapUnitChangeRequestId = parseInt($("#spnMapUnitChangeRequestId").html());
    let RN = $("#spnRN").html()?.toLowerCase();

    if (MapUnitChangeRequestId > 0 && RN === "admin") {
        await GetChangeMapUnitDetails(MapUnitChangeRequestId);
    }
    else {
        mMsater(0, "ddlCommand", 1, "");
        await GetUnitDetails(parseInt($("#spnUnitMapId").html()));
        await GetCurrentUserDetails();
    }

    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
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

    if (MapUnitChangeRequestId > 0 && RN === "admin") {
        $("#btnSubmit").addClass("d-none");
        $("#btnAccept").removeClass("d-none");
        $("#btnReject").removeClass("d-none");

        $(".AdminRemark").removeClass("d-none");

    } else {
        $("#btnSubmit").removeClass("d-none");
        $("#btnAccept").addClass("d-none");
        $("#btnReject").addClass("d-none");

        $(".AdminRemark").addClass("d-none");

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
        let UnitName = $("#lblUnit").text();
        let SUSNo = $("#lblSUSNo").text();
        let Remarks = $("#txtRemark").val();
        let AdminRemark = $("#txtAdminRemark").val();
        let UserName = $(".dropdown-user-details-name").html();
        Swal.fire({
            title: 'Please Confirm the following Unit Move Request details',
            html: `
                    <div class="unit-move-details">
                        <p><strong>Unit Name : </strong> ${UnitName.trim()}</p>
                        <p><strong>SUS NO : </strong> ${SUSNo.trim()}</p>
                        <p><strong>${choice === 1 ? "Remarks" : "Admin Remark"}:</strong> ${choice === 1 ? Remarks : AdminRemark}</p>
                        <p><strong>Logged In Details : </strong> ${UserName}</p>
                    </div>
                  `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm',
            cancelButtonText: 'Cancel',
            width: '500px', // optional: customize popup width
            customClass: {
                popup: 'unit-move-popup'   // optional: style popup via CSS instead of width
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
    let urladd;
    
    if (choice === 1) {
        const payload = {
            "MapUnitChangeRequestId": document.getElementById('spnMapUnitChangeRequestId')?.textContent || "",
            "UnitMapId": document.getElementById('spnUnitMapId')?.textContent || "",
            "Remark": document.getElementById('txtRemark')?.value || "",
            "AdminRemark": document.getElementById('txtAdminRemark')?.value || "",
            "Choice": parseInt(choice || "0"),
            "UnitType": UnitTyperdi,
            "ComdId": document.getElementById('ddlCommand')?.value || "",
            "CorpsId": document.getElementById('ddlCorps')?.value || "",
            "DivId": document.getElementById('ddlDiv')?.value || "",
            "BdeId": document.getElementById('ddlBde')?.value || "",
            "PsoId": document.getElementById('ddlPSODte')?.value || "",
            "FmnBranchID": document.getElementById('ddlFmnBranch')?.value || "",
            "SubDteId": document.getElementById('ddlDgSubDte')?.value || ""

        }
        let jsonData = JSON.stringify(payload);

        let encrypted = encryptPayloadData(jsonData);
        urladd = '/Master/SaveMapUnitChangeRequest';
        $.ajax({
            url: urladd,
            type: 'POST',
            data: { request: encrypted }, //get the search string
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
            success: function (result) {
                if (result.Result == true) {
                    const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                    const btnSearchNew = document.getElementById("btnSearchNew");
                    const btnBackDashboard = document.getElementById("btnBackDashboard");
                    let Message;
                    if (parseInt($("#spnMapUnitChangeRequestId").html()) == 0)
                        Message = `Your Unit Move Request placed successfully with Admin for necy Approval.<br/> Record successfully inserted in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;

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
    else {
        const payload = {
            "MapUnitChangeRequestId": document.getElementById('spnMapUnitChangeRequestId')?.textContent || "",
            "UnitMapId": document.getElementById('spnUnitMapId')?.textContent || "",
            "Remark": document.getElementById('txtRemark')?.value || "",
            "AdminRemark": document.getElementById('txtAdminRemark')?.value || "",
            "Choice": parseInt(choice || "0"),
            "UnitType": UnitTyperdi,
            "ComdId": document.getElementById('ddlCommand')?.value || "",
            "CorpsId": document.getElementById('ddlCorps')?.value || "",
            "DivId": document.getElementById('ddlDiv')?.value || "",
            "BdeId": document.getElementById('ddlBde')?.value || "",
            "PsoId": document.getElementById('ddlPSODte')?.value || "",
            "FmnBranchID": document.getElementById('ddlFmnBranch')?.value || "",
            "SubDteId": document.getElementById('ddlDgSubDte')?.value || ""
        }
        let jsonData = JSON.stringify(payload);

        let encrypted = encryptPayloadData(jsonData);
        urladd = '/Master/UpdateMapUnitChangeRequest';
        $.ajax({
            url: urladd,
            type: 'POST',
            data: { request: encrypted }, //get the search string
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
            success: function (result) {
                if ($("#spnRN").html().toLowerCase() === "admin" && result.Result == true) {
                    $("#spnMapUnitChangeRequestId").html(result.Id);
                }

                if (result.Result == true) {
                    const myModal = new bootstrap.Modal(document.getElementById("ConfirmationDialog"));
                    const btnSearchNew = document.getElementById("btnSearchNew");
                    const btnBackDashboard = document.getElementById("btnBackDashboard");
                    const ConfirmationDialog_Data = document.getElementById("ConfirmationDialog_Data");
                    let Message;
                    //choice = 2 Accept, choice =3 Reject
                    if (parseInt($("#spnMapUnitChangeRequestId").html()) > 0 && choice === 2) {
                        Message = `Unit Move Request Accept & update in Mapping Unit successfully.<br/>Record successfully updated in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;
                    }
                    else if (parseInt($("#spnMapUnitChangeRequestId").html()) > 0 && choice === 3) {
                        Message = `Unit Move Request Rejected successfully.<br/>Record successfully updated in DB with ID : <strong>${result.Id}</strong><br/> Timestamp : <strong>${DateFormateddMMyyyyhhmmss(result.CurrentTime)}</strong>.`;
                    }

                    ConfirmationDialog_Data.innerHTML = Message;
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

}
async function GetUnitDetails(UnitMapId) {
    let param = new URLSearchParams({ UnitMapId: encryptPayloadData(UnitMapId) });

    fetch('/Master/GetALLByUnitMapId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
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
                UnitTyperdi = result.UnitType;

                $("#lblUnit").html(result.UnitName);
                $("#lblSUSNo").html(`${result.Sus_no}${result.Suffix}`);
                if (result.UnitType == 1) {
                    $("#lblUnitType").html(`Unit`);

                    $(".ExistingCh-UnitType").removeClass("d-none");
                    $(".ExistingCh-FmnBranch").addClass("d-none");
                    $(".ExistingCh-DteBranch").addClass("d-none");
                }
                else if (result.UnitType == 2) {
                    $("#lblUnitType").html(`Fmn HQ`);

                    $(".ExistingCh-UnitType").removeClass("d-none");
                    $(".ExistingCh-FmnBranch").removeClass("d-none");
                    $(".ExistingCh-DteBranch").addClass("d-none");
                }
                else if (result.UnitType == 3) {
                    $("#lblUnitType").html(`Dte / Sub Dte Branch`);

                    $(".ExistingCh-UnitType").addClass("d-none");
                    $(".ExistingCh-FmnBranch").addClass("d-none");
                    $(".ExistingCh-DteBranch").removeClass("d-none");
                }

                $("#lblComd").html(result.ComdName);
                $("#lblCorps").html(result.CorpsName);
                $("#lblDiv").html(result.DivName);
                $("#lblBde").html(result.BdeName);
                $("#lblFmnBranch").html(result.BranchName);
                $("#lblPSODte").html(result.PSOName);
                $("#lblDgSubDte").html(result.SubDteName);


                var lst = '<option value="1">Please Select</option>';

                if (UnitTyperdi == 1) {
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
                else if (UnitTyperdi == 2) {

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
                else if (UnitTyperdi == 3) {
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

                $("#CurrentUnitHierarchy").removeClass("d-none");
                $("#ChangeUnitHierarchy").removeClass("d-none");

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
async function GetChangeMapUnitDetails(MapUnitChangeRequestId) {
    let param = new URLSearchParams({ MapUnitChangeRequestId: MapUnitChangeRequestId });

    fetch('/Master/GetChangeMapUnitDetails', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
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
                $("#spnUnitMapId").html(result.UnitMapId);
                $("#lblUnit").html(result.UnitName);
                $("#lblSUSNo").html(result.Sus_no);
                $("#txtRemark").val(result.Remark);
                $("#lblRequestBy").text(result.RequestBy);

                if (result.ExistingCh_UnitType == 1) {
                    $("#lblUnitType").html(`Unit`);

                    $(".ExistingCh-UnitType").removeClass("d-none");
                    $(".ExistingCh-FmnBranch").addClass("d-none");
                    $(".ExistingCh-DteBranch").addClass("d-none");
                }
                else if (result.ExistingCh_UnitType == 2) {
                    $("#lblUnitType").html(`Fmn HQ`);

                    $(".ExistingCh-UnitType").removeClass("d-none");
                    $(".ExistingCh-FmnBranch").removeClass("d-none");
                    $(".ExistingCh-DteBranch").addClass("d-none");
                }
                else if (result.ExistingCh_UnitType == 3) {
                    $("#lblUnitType").html(`Dte / Sub Dte Branch`);

                    $(".ExistingCh-UnitType").addClass("d-none");
                    $(".ExistingCh-FmnBranch").addClass("d-none");
                    $(".ExistingCh-DteBranch").removeClass("d-none");
                }

                $("#lblComd").html(result.ComdName);
                $("#lblCorps").html(result.CorpsName);
                $("#lblDiv").html(result.DivName);
                $("#lblBde").html(result.BdeName);
                $("#lblFmnBranch").html(result.BranchName);
                $("#lblPSODte").html(result.PSOName);
                $("#lblDgSubDte").html(result.SubDteName);

                var lst = '<option value="1">Please Select</option>';



                if (result.RequestCh_UnitType == 1) {

                    mMsater(result.ComdId, "ddlCommand", 1, "");
                    mMsater(result.CorpsId, "ddlCorps", 2, result.ComdId);
                    mMsaterByParent(result.DivId, "ddlDiv", 3, result.ComdId, result.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                    mMsaterByParent(result.BdeId, "ddlBde", 4, result.ComdId, result.CorpsId, result.DivId, 0);///ComdId,CorpsId,DivId,BdeId

                    $(".unittype").removeClass("d-none");
                    $(".FmnBranch").addClass("d-none");
                    $(".DteBranch").addClass("d-none");

                    $("#ddlFmnBranch").html(lst);
                    $("#ddlPSODte").html(lst);
                    $("#ddlDgSubDte").html(lst);
                }
                else if (result.RequestCh_UnitType == 2) {

                    mMsater(result.ComdId, "ddlCommand", 1, "");
                    mMsater(result.CorpsId, "ddlCorps", 2, result.ComdId);
                    mMsaterByParent(result.DivId, "ddlDiv", 3, result.ComdId, result.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                    mMsaterByParent(result.BdeId, "ddlBde", 4, result.ComdId, result.CorpsId, result.DivId, 0);///ComdId,CorpsId,DivId,BdeId
                    mMsater(result.FmnBranchID, "ddlFmnBranch", FmnBranches, "");

                    $("#ddlPSODte").html(lst);
                    $("#ddlDgSubDte").html(lst);

                    $(".unittype").removeClass("d-none");
                    $(".FmnBranch").removeClass("d-none");
                    $(".DteBranch").addClass("d-none");

                }
                else if (result.RequestCh_UnitType == 3) {

                    mMsater(result.PsoId, "ddlPSODte", PSO, "");
                    mMsater(result.SubDteId, "ddlDgSubDte", SubDte, "");

                    $(".unittype").addClass("d-none");
                    $(".FmnBranch").addClass("d-none");
                    $(".DteBranch").removeClass("d-none");

                    $("#ddlFmnBranch").html(lst);
                    $("#ddlCommand").html(lst);
                    $("#ddlCorps").html(lst);
                    $("#ddlCorps").html(lst);
                    $("#ddlBde").html(lst);
                    $("#ddlDiv").html(lst);
                }

                $("#ddlCommand").prop("disabled", true);
                $("#ddlCorps").prop("disabled", true);
                $("#ddlDiv").prop("disabled", true);
                $("#ddlBde").prop("disabled", true);
                $("#ddlFmnBranch").prop("disabled", true);
                $("#ddlPSODte").prop("disabled", true);
                $("#ddlDgSubDte").prop("disabled", true);

                $("#txtRemark").prop("disabled", true);

                UnitTyperdi = result.RequestCh_UnitType;

                $("#CurrentUnitHierarchy").removeClass("d-none");
                $("#ChangeUnitHierarchy").removeClass("d-none");
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
async function GetCurrentUserDetails() {
    var userdata = {
        "Id": 0
    };

    try {
        const response = await fetch('/ConfigUser/GetTokenArmyNo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: new URLSearchParams(userdata)
        });

        const result = await response.json();

        if (result !== "null" && result !== null) {
            if (result == 0) {
                // alert("Please Add Profile");
            } else {
                let UserName = result.Name;
                let ArmyNo = result.ICNO;
                let Rank = result.RankName;
                let RequestBy = `${Rank} ${UserName} (${ArmyNo})`;
                $("#lblRequestBy").text(RequestBy);
            }
        }
    } catch (error) {
        console.error('Error fetching token army no:', error);
    }
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