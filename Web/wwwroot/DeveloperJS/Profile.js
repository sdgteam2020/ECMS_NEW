$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    await GetByArmyNo(null);
    $("#btntokenrefresh").on("click",async function () {
       
        await GetTokenvalidatepersid2fawiththumbprint($("#txtProArmy").val(), "tokenmsg", "txtspnTokenArmyNo", "Thumbprint");
    });
    $("#btnProfilesave").on("click",function () {

       /* alert($("#intoffsyes").prop("checked") )*/
        if ($("#SaveFormProfile")[0].checkValidity()) {
            Swal.fire({
                title: 'Are you sure?',
                text: "",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Save it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    var RankId = $("#ddlProRank").val();
                    var Name = $("#txtName").val();
                    var Thumbprint = $("#Thumbprint").val();
                

                    let  IsRO = false;//$("#chkRO").prop("checked");
                    let  IsIO = $("#chkIO").prop("checked");
                    let IsCO = $("#chkCO").prop("checked");
                    let IsORO = false;// $("#chkORO").prop("checked");
                    
                    UpdateProfileWithMapping(RankId, Name, IsRO, IsIO, IsCO, IsORO, Thumbprint); 
                }
            })
             } else {
            $("#SaveFormProfile")[0].reportValidity();
        }

    });

    mMsater(0, "ddlProRank", Rank, "");

    $(".allow-number").on("keypress", function (event) {
        const key = event.which;

        // Allow: backspace (8), delete (46), left (37), right (39), 0-9 (48-57)
        if (key === 8 || key === 46 || key === 37 || key === 39 || (key >= 48 && key <= 57)) {
            // Allowed key, do nothing
            return;
        } else {
            // Block everything else
            event.preventDefault();
        }
    });
   
    $("#btnderegprofile").on("click", function () {
        $("#DeRegisterConfirmModal").modal('show');
    });

    $("#btnDeRegisterConfirmModalSubmit").on("click", function () {
        $("#DeRegisterConfirmModal").modal('hide');
        $.ajax({
            url: '/UserProfile/DeRegisterUserId',
            contentType: 'application/x-www-form-urlencoded',
            type: 'POST',
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
            success: function (result) {
                if (result == true) {
                    toastr.success('User successfully Unmapped');
                    $.ajax({
                        url: '/Account/Logout',
                        contentType: 'application/x-www-form-urlencoded',
                        type: 'POST',
                        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                        success: function () {
                            window.location.href = "/Account/Logout";
                        }
                    });

                }
                else if (result == false) {
                    toastr.error('Incorrect Data!');
                }
            }
        });
    });
  
});

function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": encryptPayloadData(param1) },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data)
        {
            
            //$("#lblunittype").html();

            $("#spnUnitIdUserPro").html(data.UnitMapId);
            $("#lblProSusno").html(data.Sus_no + '' + data.Suffix);
            $("#lblProUnit").html(data.UnitName);
            $(".spnUnit").addClass('d-none');
            $(".spnFmnBranch").addClass('d-none');
            $(".spnDteBranch").addClass('d-none');



            if (data.UnitType == 1) {
                $(".spnUnit").removeClass('d-none');
                $("#lblunittype").html("Unit");
                $("#lblProComd").html(data.ComdName);
                $("#lblProCorps").html(data.CorpsName);
                $("#lblProDiv").html(data.DivName);
                $("#lblPrBde").html(data.BdeName);
            }
            else if (data.UnitType == 2) {
                $(".spnUnit").removeClass('d-none');
                $(".spnFmnBranch").removeClass('d-none');
                $("#lblunittype").html("Unit is Fmn HQ");
                $("#lblProComd").html(data.ComdName);
                $("#lblProCorps").html(data.CorpsName);
                $("#lblProDiv").html(data.DivName);
                $("#lblPrBde").html(data.BdeName);
                $("#lblFmnBranch").html(data.BranchName);
            }
            else if (data.UnitType == 3) {
                $(".spnDteBranch").removeClass('d-none');
                $("#lblunittype").html("Unit is Dte/Branch");
                $("#lblpso").html(data.PSOName);
                $("#lblDg").html(data.SubDteName);
            }
        }
    });
}

function UpdateProfileWithMapping(RankId, Name, IsRO, IsIO, IsCO, IsORO, Thumbprint) { 

    $.ajax({
        url: '/UserProfile/UpdateProfileWithMapping',
        type: 'POST',
        data: {
            "RankId": RankId, "Name": Name, "IsRO": IsRO, "IsIO": IsIO, "IsCO": IsCO, "IsORO": IsORO, "Thumbprint": Thumbprint
        }, 
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {

            if (response.Result == true) {
                toastr.success(response.Message);
                location.reload();
            }
            else {
                toastr.error(response.Message);
            }
        }
    });
}
async function GetByArmyNo(ArmyNo) {
    const userdata = new URLSearchParams();
    userdata.append("ArmyNo", ArmyNo);

    fetch('/UserProfile/GetByArmyNoOrAspnetuserId', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: userdata
    })
        .then(response => response.json())
        .then(response => {
            if (response !== "null" && response !== null) {
                if (response === InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else if (response === 0) {
                    // Do nothing
                } else {
                    $("#txtProArmy").val(response.ArmyNo);
                    $("#Thumbprint").val(response.Thumbprint);
                    /*  $("#lblThumbPrint").html(response.Thumbprint != null ? response.Thumbprint : "-");*/
                    $("#lblicno").html(response.ArmyNo);
                    $(".lblAppt").html(response.AppointmentName);
                    $("#lblrole").html(response.RoleName);
                    GetALLByUnitById(response.UnitId);
                    mMsater(response.RankId, "ddlProRank", Rank, "");

                    //$("#chkRO").prop("checked", response.IsRO == true);

                    $("#chkIO").prop("checked", response.IsIO === true);
                    $("#chkCO").prop("checked", response.IsCO === true);
                    //$("#chkORO").prop("checked", response.IsORO === true);


                    $("#txtName").val(response.Name);
                    //GetALLByUnitById($("#aspndomainUnitID").html());
                    $("#lblDomainId").html(response.DomainId);
                    $("#lblMappedDate").html(DateFormateMMMM_dd_yyyy(response.MappedDate));
                    $("#lblMappedBy").html(response.MappedBy);

                    if (response.IsToken === false)
                        $("#btntokenrefresh").addClass("d-none");
                    else
                        $("#btntokenrefresh").removeClass("d-none");

                    // GetALLByUnitById(response.UnitId);
                    //$("#AddNewProfile").modal('hide');
                }
            }
        })
        .catch(() => {
            Swal.fire({
                text: errormsg002
            });
        });
}
