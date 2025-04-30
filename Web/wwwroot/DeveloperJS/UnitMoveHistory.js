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
                listItem += `<div>Unit Relocation  Request</div><br/>`;
                listItem += `<div>Unit Name - ${result.UnitName} (${ result.Sus_no }${ result.Suffix })</div>`;
                listItem += `<div>Request By - ${result.RequestBy}</div>`;
                listItem += `<div>Current Hierarchy</div>`;

                if (result.UnitType == 1) {
                    listItem += `<div>Current Hierarchy</div>`
             listItem += `<div class="col-sm-6">
                            <div class="card p-1">
                                <div class="feature-box3">
                                    <div class="top-block_ind d-flex">
                                        <div class="text-block">
                                            <h5 class="mb-1 text-font2 font-weight600">Current Hierarchy</h5>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group row mb-0">
                                    <label class="col-form-label col-sm-5 labelUnit">Unit Type</label>
                                    <div class="col-sm-7">
                                        <label id="lblUnitType" class="col-form-label"></label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-UnitType">
                                    <label class="col-form-label col-sm-5 labelUnit">Comd / PSO</label>
                                    <div class="col-sm-7">
                                        <label id="lblComd" class="col-form-label">${result.ComdName}</label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-UnitType">
                                    <label class="col-form-label col-sm-5 labelUnit">Corps / Dte / Area</label>
                                    <div class="col-sm-7">
                                        <label id="lblCorps" class="col-form-label"></label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-UnitType">
                                    <label class="col-form-label col-sm-5 labelUnit">Div / Sub Area</label>
                                    <div class="col-sm-7">
                                        <label id="lblDiv" class="col-form-label"></label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-UnitType">
                                    <label class="col-form-label col-sm-5 labelUnit">Bde</label>
                                    <div class="col-sm-7">
                                        <label id="lblBde" class="col-form-label"></label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-FmnBranch">
                                    <label class="col-form-label col-sm-5 labelUnit">Fmn / Branch</label>
                                    <div class="col-sm-7">
                                        <label id="lblFmnBranch" class="col-form-label"></label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-DteBranch">
                                    <label class="col-form-label col-sm-5 labelUnit">PSO / Dte </label>
                                    <div class="col-sm-7">
                                        <label id="lblPSODte" class="col-form-label"></label>
                                    </div>
                                </div>
                                <div class="form-group row mb-0 ExistingCh-DteBranch">
                                    <label class="col-form-label col-sm-5 labelUnit">DG / Sub Dte</label>
                                    <div class="col-sm-7">
                                        <label id="lblDgSubDte" class="col-form-label"></label>
                                    </div>
                                </div>
                            </div>
                        </div>`
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

            }
            else {
                toastr.error('Invalid Input.');
            }
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
}