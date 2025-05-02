function GetUnitMoveHistory(MapUnitChangeRequestId) {
    let param = new URLSearchParams({ MapUnitChangeRequestId: MapUnitChangeRequestId });
    var listItem = "";
    fetch('/Master/GetUnitMoveHistory', {
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
                listItem += `<div class="fw-bold">Unit Name - ${result.UnitAbbreviation} (${result.Sus_no })</div>`;
                listItem += `<div class="fw-bold">Request By - ${result.RankAbbreviation} ${result.RequestBy} (${result.ArmyNo})</div>`;
                listItem += `<div class="fw-bold">Approve By - ${result.IsEditAction == true ? result.AproverRankAbbreviation + " " + result.AprovedBy + " (" + result.AproverArmyNo + ")" : ""} </div>`;
                listItem += `<div class="fw-bold">Status - ${result.IsEditAction === false ? "<span class='badge bg-warning'>Pendding</span>" : result.RequestStatus === true ? "<span class='badge bg-success'>Accepted</span>" : "<span class='badge badge-pill badge-danger'>Rejected</span>"}</div>`;
                listItem += `<div class="fw-bold">Approve Dt & Time - ${result.IsEditAction == true ? DateFormateddMMyyyyhhmmss(result.ApproverUpdatedOn) : ""} </div>`;
                listItem += `<div class="col-sm-12 mt-2">
                                    <div class="card p-1">
                                        <div class="feature-box3">
                                            <div class="top-block_ind d-flex">
                                                <div class="text-block">
                                                    <h5 class="mb-1 text-font2 font-weight600">Current Hierarchy</h5>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0">
                                            <label class="col-form-label col-sm-5 fw-bold">Unit Type</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingUnitType === 1 ? "Unit" : result.ExistingUnitType === 2 ? "Fmn HQ" : result.ExistingUnitType === 3 ? "Dte / Sub Dte Branch":""}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Comd / PSO</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingComdName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Corps / Dte / Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingCorpsName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Bde</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingDivName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Div / Sub Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingBdeName}</label>
                                            </div>
                                        </div>

                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 1 || result.ExistingUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Fmn / Branch</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingBranchName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 1 || result.ExistingUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">PSO / Dte </label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingPSOName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.ExistingUnitType === 1 || result.ExistingUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">DG / Sub Dte</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.ExistingSubDteName}</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                listItem += `<div class="col-sm-12 mt-2">
                                    <div class="card p-1">
                                        <div class="feature-box3">
                                            <div class="top-block_ind d-flex">
                                                <div class="text-block">
                                                    <h5 class="mb-1 text-font2 font-weight600">Unit Hierarchy Recalibration</h5>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0">
                                            <label class="col-form-label col-sm-5 fw-bold">Unit Type</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestUnitType === 1 ? "Unit" : result.RequestUnitType === 2 ? "Fmn HQ" : result.RequestUnitType === 3 ? "Dte / Sub Dte Branch" : ""}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Comd / PSO</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestComdName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Corps / Dte / Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestCorpsName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Bde</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestDivName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Div / Sub Area</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestBdeName}</label>
                                            </div>
                                        </div>

                                        <div class="form-group row mb-0 ${result.RequestUnitType === 1 || result.RequestUnitType === 3 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">Fmn / Branch</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestBranchName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.RequestUnitType === 1 || result.RequestUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">PSO / Dte </label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestPSOName}</label>
                                            </div>
                                        </div>
                                        <div class="form-group row mb-0 ${result.RequestUnitType === 1 || result.RequestUnitType === 2 ? "d-none" : ""}">
                                            <label class="col-form-label col-sm-5 fw-bold">DG / Sub Dte</label>
                                            <div class="col-sm-7">
                                                <label class="col-form-label">${result.RequestSubDteName}</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                const myModal = new bootstrap.Modal(document.getElementById("HistoryModal"));
                let HistoryModal_Header = document.getElementById("HistoryModal_Header");
                let HistoryModal_Title = document.getElementById("HistoryModal_Title");
                let HistoryModal_Body = document.getElementById("HistoryModal_Body");

                HistoryModal_Header.innerHTML = "Unit Relocation  Request";
                HistoryModal_Title.classList.add("d-none");
                HistoryModal_Body.innerHTML = listItem;
                myModal.show();
            }
            else {
                toastr.error('Invalid Input.');
            }
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
}