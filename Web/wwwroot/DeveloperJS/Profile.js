
$(document).ready(function () {
    GetByArmyNo("")
    
    $("#btnProfilesave").click(function () {

       /* alert($("#intoffsyes").prop("checked") )*/
        if ($("#SaveFormProfile")[0].checkValidity()) {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be Save!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Save it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    var ArmyNo = $("#txtProArmy").val();
                    var Rank = $("#ddlProRank").val();
                    var Name = $("#txtName").val();
                  
                   
                    var userid = $("#spnUserId").html();
                   

                    IntOffr = $("#Intoffrs").prop("checked");
                    IsIO = $("#chkIO").prop("checked");
                    IsCo = $("#chkCO").prop("checked")
                    
                        SaveUserProfile(ArmyNo, Rank, Name, IntOffr, IsIO, IsCo, userid);
                        //SaveUserProfile(ArmyNo, Rank, Name, Appt, Unit, $("#intoffsyes").prop("checked"), 3, $("#spnUserIdIO").html(), $("#spnUserIdGSO").html(), userid)
                   
                    
                }
            })
             } else {
            $("#SaveFormProfile")[0].reportValidity();
        }

    });

    mMsater(0, "ddlProRank", Rank, "");
   
   
  
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

function SaveUserProfile(ArmyNo, Rank, Name, IntOffr, IsIO, IsCo, UserId) {

    /*  alert($('#bdaymonth').val());*/
    
    $.ajax({
        url: '/UserProfile/SaveUserProfile',
        type: 'POST',
        data: { "ArmyNo": ArmyNo, "RankId": Rank, "Name": Name, "UserId": UserId, "IntOffr": IntOffr, "IsIO": IsIO, "IsCo": IsCo }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('User has been saved');
               
                
                
               
            }
            else if (result == DataUpdate) {
                toastr.success('User has been Updated');
               
               
               
            }
            else if (result == DataExists) {

                toastr.error('Army No Exits!');

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
        url: '/UserProfile/GetByArmyNo',
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
                        $("#txtProArmy").val(response.ArmyNo);
                        $("#lblicno").html(response.ArmyNo);
                        $(".lblAppt").html(response.AppointmentName);
                        $("#lblrole").html(response.RoleName);
                         GetALLByUnitById(response.UnitId);
                        mMsater(response.RankId, "ddlProRank", Rank, "");
                       
                        if (response.IntOffr == false) {

                            $("#Intoffrs").prop("checked", false); 
                        }
                        else {
                            
                            $("#Intoffrs").prop("checked", true); 
                           
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
                        $("#txtName").val(response.Name);
                        //GetALLByUnitById($("#aspndomainUnitID").html());
                        $("#lblDomainId").html(response.DomainId);
                        $("#lblMappedDate").html(DateFormateMMMM_dd_yyyy(response.MappedDate));
                        $("#lblMappedBy").html(response.MappedBy);

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
