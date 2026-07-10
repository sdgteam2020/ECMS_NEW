$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
});
function GetUnitMoveHistory(MapUnitChangeRequestId) {

    //let param = new URLSearchParams({ MapUnitChangeRequestId: MapUnitChangeRequestId });
    var listItem = "";
    fetch('/Master/GetUnitMoveHistory', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: new URLSearchParams({
            Request: encryptPayloadData(MapUnitChangeRequestId)
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(result => {
            if (result.Result === true) {
                listItem += `<div class="fw-bold">Unit Name - ${result.Value.UnitAbbreviation} (${result.Value.Sus_no })</div>`;
                listItem += `<div class="fw-bold">Request By - ${result.Value.RankAbbreviation} ${result.Value.RequestBy} (${result.Value.ArmyNo})</div>`;
                listItem += `<div class="fw-bold">Approve By - ${result.Value.IsEditAction == true ? result.Value.AproverRankAbbreviation + " " + result.Value.AprovedBy + " (" + result.Value.AproverArmyNo + ")" : ""} </div>`;
                listItem += `<div class="fw-bold">Status - ${result.Value.IsEditAction === false ? "<span class='badge bg-warning'>Pendding</span>" : result.Value.RequestStatus === true ? "<span class='badge bg-success'>Accepted</span>" : "<span class='badge badge-pill badge-danger'>Rejected</span>"}</div>`;
                listItem += `<div class="fw-bold">Status Dt & Time - ${result.Value.IsEditAction == true ? DateFormateddMMyyyyhhmmss(result.Value.AdminUpdatedOn) : ""} </div>`;
                listItem += `<div class="col-sm-12 mt-2">
                                    <div class="card p-1">
                                        <div class="feature-box3">
                                            <div class="top-block_ind d-flex">
                                                <div class="text-block">
                                                    <h5 class="mb-1 text-font2 font-weight600">Unit Current Hierarchy</h5>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0">
                                            <label class="col-form-label col-sm-5 fw-bold">Unit Type</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingUnitType === 1 ? "Unit" : result.Value.ExistingUnitType === 2 ? "Fmn HQ" : result.Value.ExistingUnitType === 3 ? "Dte / Sub Dte Branch":""}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Comd / PSO</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingComdName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Corps / Dte / Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingCorpsName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Bde</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingDivName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Div / Sub Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingBdeName}</label>
                                            </div>
                                        </div>

                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 1 || result.Value.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Fmn / Branch</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingBranchName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 1 || result.Value.ExistingUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">PSO / Dte </label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingPSOName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.ExistingUnitType === 1 || result.Value.ExistingUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">DG / Sub Dte</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.ExistingSubDteName}</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                listItem += `<div class="col-sm-12 mt-2">
                                    <div class="card p-1">
                                        <div class="feature-box3">
                                            <div class="top-block_ind d-flex">
                                                <div class="text-block">
                                                    <h5 class="mb-1 text-font2 font-weight600">Unit Requested Hierarchy</h5>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0">
                                            <label class="col-form-label col-sm-5 fw-bold">Unit Type</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestUnitType === 1 ? "Unit" : result.Value.RequestUnitType === 2 ? "Fmn HQ" : result.Value.RequestUnitType === 3 ? "Dte / Sub Dte Branch" : ""}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Comd / PSO</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestComdName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Corps / Dte / Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestCorpsName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Bde</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestDivName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Div / Sub Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestBdeName}</label>
                                            </div>
                                        </div>

                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 1 || result.Value.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Fmn / Branch</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestBranchName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 1 || result.Value.RequestUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">PSO / Dte </label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestPSOName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.Value.RequestUnitType === 1 || result.Value.RequestUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">DG / Sub Dte</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.Value.RequestSubDteName}</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                const myModal = new bootstrap.Modal(document.getElementById("HistoryModal"));
                let HistoryModal_Header = document.getElementById("HistoryModal_Header");
                let HistoryModal_Title = document.getElementById("HistoryModal_Title");
                let HistoryModal_Body = document.getElementById("HistoryModal_Body");

                HistoryModal_Header.innerHTML = "Unit Move Request";
                HistoryModal_Title.classList.add("d-none");
                HistoryModal_Body.innerHTML = listItem;
                myModal.show();
            }
            else {
                toastr.error(result.Message);
            }
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
}