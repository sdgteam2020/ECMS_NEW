
$(document).ready(function () {
    GetByArmyNo("")
    $("#btntokenrefresh").click(function () {
       
        GetTokenvalidatepersid2fawiththumbprint($("#txtProArmy").val(), "tokenmsg", "txtspnTokenArmyNo", "Thumbprint");
    });
    $("#btnProfilesave").click(function () {

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
                    var MobileNo = $("#txtMobileNo").val();
                    var DialingCode = $("#txtDialingCode").val();
                    var Extension = $("#txtExtension").val();
                    var Thumbprint = $("#Thumbprint").val();
                  
                    var TDMId = $("#spnTDMId").html();                   
                    var userid = $("#spnUserId").html();
                   

                    IsRO = $("#chkRO").prop("checked");
                    IsIO = $("#chkIO").prop("checked");
                    IsCO = $("#chkCO").prop("checked");
                    IsORO = $("#chkORO").prop("checked");
                    
                    UpdateProfileWithMapping(RankId, Name, MobileNo, DialingCode, Extension, IsRO, IsIO, IsCO, IsORO, userid, TDMId, Thumbprint);
                        //SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, $("#intoffsyes").prop("checked"), 3, $("#spnUserIdIO").html(), $("#spnUserIdGSO").html(), userid)
                   
                    
                }
            })
             } else {
            $("#SaveFormProfile")[0].reportValidity();
        }

    });

    mMsater(0, "ddlProRank", Rank, "");

    $(".allow-number").on("keypress", function (event) {
        // Allow only backspace , delete, numbers               
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 39 || event.keyCode == 37
            || (event.keyCode >= 48 && event.keyCode <= 57)) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the key press
            event.preventDefault();
        }
    });
   
   
  
});

function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
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

function UpdateProfileWithMapping(RankId, Name, MobileNo, DialingCode, Extension, IsRO, IsIO, IsCO, IsORO, UserId, TDMId,Thumbprint) {

    /*  alert($('#bdaymonth').val());*/
    
    $.ajax({
        url: '/UserProfile/UpdateProfileWithMapping',
        type: 'POST',
        data: {
             "RankId": RankId, "Name": Name, "MobileNo": MobileNo, "DialingCode": DialingCode, "Extension": Extension, "UserId": UserId, "IsRO": IsRO, "IsIO": IsIO, "IsCO": IsCO, "IsORO": IsORO, "TDMId": TDMId,"Thumbprint": Thumbprint }, //get the search string
        success: function (result) {


            if (result == DataUpdate) {
                toastr.success('User has been Updated');
            }
            else if (result == IncorrectData) {
                toastr.error('Incorrect Data!');
            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',
                })
            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }
                }
            }
        }
    });
}
function GetByArmyNo(ArmyNo) {
   
    var userdata =
    {
        "ArmyNo": ArmyNo,

    };
    $.ajax({
        url: '/UserProfile/GetByArmyNoOrAspnetuserId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                   
                }

                else {
                        $("#spnUserId").html(response.UserId);
                        $("#spnTDMId").html(response.TDMId);
                        $("#txtProArmy").val(response.ArmyNo);
                        $("#txtMobileNo").val(response.MobileNo);
                        $("#txtDialingCode").val(response.DialingCode);
                        $("#txtExtension").val(response.Extension);
                        $("#Thumbprint").val(response.Thumbprint);
                      /*  $("#lblThumbPrint").html(response.Thumbprint != null ? response.Thumbprint : "-");*/
                        $("#lblicno").html(response.ArmyNo);
                        $(".lblAppt").html(response.AppointmentName);
                        $("#lblrole").html(response.RoleName);
                        GetALLByUnitById(response.UnitId);
                        mMsater(response.RankId, "ddlProRank", Rank, "");
                       
                        if (response.IsRO == false) {

                            $("#chkRO").prop("checked", false); 
                        }
                        else {
                            
                            $("#chkRO").prop("checked", true); 
                           
                        }
                        if (response.IsIO == false) {

                            $("#chkIO").prop("checked", false);
                        }
                        else {

                            $("#chkIO").prop("checked", true);

                        }
                        if (response.IsCO == false) {

                            $("#chkCO").prop("checked", false);
                        }
                        else {

                            $("#chkCO").prop("checked", true);

                        }
                        if (response.IsORO == false) {

                            $("#chkORO").prop("checked", false);
                        }
                        else {

                            $("#chkORO").prop("checked", true);

                        }
                        $("#txtName").val(response.Name);
                        //GetALLByUnitById($("#aspndomainUnitID").html());
                        $("#lblDomainId").html(response.DomainId);
                        $("#lblMappedDate").html(DateFormateMMMM_dd_yyyy(response.MappedDate));
                        $("#lblMappedBy").html(response.MappedBy);


                    if (response.IsToken == false)
                        $("#btntokenrefresh").addClass("d-none");
                    else
                        $("#btntokenrefresh").removeClass("d-none");
                       // GetALLByUnitById(response.UnitId);
                        //$("#AddNewProfile").modal('hide');

                    

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
