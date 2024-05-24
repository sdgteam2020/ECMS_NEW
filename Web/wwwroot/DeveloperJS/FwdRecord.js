//var photo = "";
//var sing = "";
var StepCounter = 0;
var applyfor = 0;
var xmlsign = 0;
var lstmultifwdarr = new Array();
var isToken = false;
$(function () {
    $("#btntokenTofwd").on("click",function () {
        $("#msgforfwd").html('');
       
        GetTokenvalidatepersid2fawiththumbprint($("#aspntokenarmyno").html(), "tokenmsgforfwd", "txtspnTokenArmyNo", "txtspnTokenthumbprint");
    });
    sessionStorage.removeItem('ArmyNo');
    $('#btnDataExports').on("click",function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);
                console.log(id);

            });

            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Export",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Export it!'
            }).then((result) => {
                if (result.value) {

                    DataExport(lst);

                }
            });
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
    });
   
    var spnStepId = 0;
    $('.select2').select2({
        dropdownParent: $('#BasicDetails'),
        closeOnSelect: true
       
    });
    $('.select3').select2({
        dropdownParent: $('#FwdRecord'),
        closeOnSelect: true
    });
    
   
    $(".historyRequest").on("click",function () {
        $("#exampleModal").modal('show'); 
        GetRequestHistory($(this).closest("tr").find(".spnRequestId").html());
    });

    $('#ddlfwdoffrs').on('change', function () {
        $("#spnFwdToAspNetUsersId").html(0);
        $("#spnFwdToUsersId").html(0);
        $(".spnFArmyNo").html("");
        $(".spnFtoname").html("");
        $(".spnFDomainName").html("");
        $(".spnFAppName").html("");

        $("#intoffsArmyNo").prop("checked", false); 
        $("#intoffDomainId").prop("checked", false); 
        $(".serchfwd").addClass("d-none");

        FwdData($('#ddlfwdoffrs').val());
    });

    //$('#ddlPhotos').on('change', function () {
    //    photo= $('#ddlPhotos').val();
    //});
    //$('#ddlsignature').on('change', function () {
    //    sing=$('#ddlsignature').val();
    //});
    //$("#btnRejected").click(function () {

    //    $("#txtFrejectedRemarks").val($("#txtFrejectedRemarks").val() + "" + photo + "" + sing);
    //});
    $("#btnMultipleForward").on("click",function () {

       

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lstmultifwdarr.push(id);
                console.log(id);

            });
            $("#FwdRecord").modal('show');
            $(".chkforserach").addClass("d-none");
            $(".gsoio").html("HQ 54");
            $("#btnForward").html("Forward To HQ 54");

            StepCounter = 4;
            applyfor = 1;
            spnStepId = 0;
            $("#multiplefed").addClass("d-none");
            GetAllOffsByUnitId("ddlfwdoffrs", 0, spnHQ54UnitId, 0, 0, 0,0,0);


            $(".Remarks").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);


            var Reject = [2];
            GetRemarks("ddlRRemarks", 0, Reject);
           
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
       
    });

    $("#btnShowForward").on("click", function () {
        $("#multiplefed").removeClass("d-none");
        $("#BasicDetails").modal('hide');

        /*if (applyfor==1)*/
        $("#FwdRecord").modal('show');

        GetByArmyNoIsToken();
    });

    $("input[name='Intoffrs']").on("change",function () {
        $(".serchfwd").removeClass("d-none");

        $("#spnFwdToAspNetUsersId").html(0);
        $("#spnFwdToUsersId").html(0);
        $(".spnFArmyNo").html("");
        $(".spnFtoname").html("");
        $(".spnFDomainName").html("");
        $(".spnFAppName").html("");
        
       
    });

    $(".fwdrecord").on("click", function () {
        Reset();
        // ResetMapUnit();
        //alert($(this).closest("tr").find(".spnRequestId").html())
        $("#multiplefed").addClass("d-none");
        $("#btntokenTofwd").addClass("d-none");
        $("#ddlRemarks").val("");
       // $("#FwdRecord").modal('show');
        $("#BasicDetails").modal('show');
        $(".spnFname").html($(this).closest("tr").find(".PersName").html());
        $(".spnFarmyno").html($(this).closest("tr").find(".ServiceNo").html());
        $("#spnStepCounter").html($(this).closest("tr").find(".spnStepCounterId").html());
        var spnRequestId = $(this).closest("tr").find(".spnRequestId").html();
        $("#spnCurrentspnRequestId").html(spnRequestId);
        spnStepId = $(this).closest("tr").find(".spnStepId").html();
        const Unitidarmy = $(this).closest("tr").find(".spnarmyUnitId").html();
        //alert(Unitidarmy)

        StepCounter = $(this).closest("tr").find(".spnStepCounterId").html();
        applyfor = $(this).closest("tr").find(".spnApplyFor").html();

        if (StepCounter == 1 || StepCounter == 7 || StepCounter == 8 || StepCounter == 9 || StepCounter == 10) {
                $(".recectopt").addClass("d-none");
            $("#btnRejected").addClass("d-none");
           
            }
        GetDataFromBasicDetails($(this).closest("tr").find(".spnBasicDetailId").html());
      
        if (StepCounter == 1 || StepCounter == 7 || StepCounter == 8 || StepCounter == 9 || StepCounter == 10 || StepCounter == 11 || StepCounter == 12 || StepCounter == 13 || StepCounter == 15) {

            if (applyfor == 1) {
                $(".gsoio").html("IO / Next Superior Offr");
                $(".gsoiotitle").html("IO / Next Superior Offr");
                $("#btnForward").html("Forward To IO / Superior");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, spnISIO,0,0,0,0);
            } else {
                $(".gsoio").html("CO /OC / OC TPS or Offr Nominated by him/ her");
                $(".gsoiotitle").html("CO / OC / OC TPS or Offr Nominated by him/ her");
                $("#btnForward").html("Forward To CO / OC / OC TPS or Offr Nominated");

                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, spnISCO,0,0,0);
            }
            $(".Remarks").removeClass("d-none");

            //$(".chkforserach").addClass("d-none");
            $("#btntokenTofwd").removeClass("d-none");

            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);
        }
        else if (StepCounter == 2) {
            $(".chkforserach").addClass("d-none");
            $(".serchfwd").addClass("d-none");
            if (applyfor == 1) {
                $(".gsoio").html("Record Office");
                $(".gsoiotitle").html("Offr Record Office (ORO) Approval");
                $("#btnForward").html("Forward To Record Office");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, 0, 0, spnIntORO, $(this).closest("tr").find(".spnBasicDetailId").html());
            }
            else {
                $(".gsoio").html("Record Office (RO)");
                $(".gsoiotitle").html("Record Office (RO) Approval");
                $("#btnForward").html("Forward To Record Office (RO)");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, Unitidarmy, 0, 0, spnIntRO, 0, $(this).closest("tr").find(".spnBasicDetailId").html());
            }
            $("#btntokenTofwd").removeClass("d-none");
            $(".Remarks").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);

            var Reject = [2];
            GetRemarks("ddlRRemarks", 0, Reject);
            
        }
        else if (StepCounter == 3 ) {
            if (applyfor == 1) {
                $(".chkforserach").addClass("d-none");

                $(".gsoio").html("AFSAC Cell");
                $(".gsoiotitle").html("AFSAC Cell");
                $("#btnForward").html("Forward To AFSAC Cell");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, spnIntAfsaccellUnitId,0,0,0,0,0);
            }
            else {
                $(".chkforserach").addClass("d-none");
                $(".gsoiotitle").html("AFSAC Cell");
                $(".gsoio").html("AFSAC Cell");
                $("#btnForward").html("Forward To AFSAC Cell ");
                GetAllOffsByUnitId("ddlfwdoffrs", 0, spnIntAfsaccellUnitId,0,0,0,0,0);
            }

            

            $(".Remarks").removeClass("d-none");
            var someNumbers = [1];
            GetRemarks("ddlRemarks", 0, someNumbers);

            var Reject = [2];
            GetRemarks("ddlRRemarks", 0, Reject);
        }
        //else if (StepCounter == 4) {


        //    $(".chkforserach").addClass("d-none");
        //    $(".gsoio").html("HQ 54");
        //    $("#btnForward").html("Forward To HQ 54");
        //    GetAllOffsByUnitId("ddlfwdoffrs", 0, spnHQ54UnitId,0,0,0,0);


        //    $(".Remarks").removeClass("d-none");
        //    var someNumbers = [1];
        //    GetRemarks("ddlRemarks", 0, someNumbers);


        //    var Reject = [2];
        //    GetRemarks("ddlRRemarks", 0, Reject);
        //    }
            //if (StepCounter == 1) {
            //    $("#btnRejected").addClass("d-none");
                
                
            //}
       // GetForwardHHierarchy($(this).closest("tr").find(".ServiceNo").html(), StepCounter , spnRequestId)
       
    });

    $("#txtFwdName").autocomplete({
       
        source: function (request, response) {
            var TypeId = 1;
            if ($("#intoffsArmyNo").prop("checked")) {
                TypeId = 1;
            } else if ($("#intoffName").prop("checked")) {
                TypeId = 2;
            } else if ($("#intoffDomainId").prop("checked")) {
                TypeId = 3;
            }
            var IsIO = 0;
            var IsCO = 0;
            var IsRO = 0;
            var IsORO = 0;
            if (applyfor == 1 && StepCounter == 1)
                IsIO = 1;
           else if (applyfor == 1 && StepCounter == 2)
                IsORO = 1;
           else if (applyfor == 2 && StepCounter == 1)
                IsCO = 1;
            else if (applyfor == 2 && StepCounter == 2)
                IsRO = 1;
            var param = { "Name": request.term, "TypeId": TypeId, "StepId": 1, "UnitId": 0, "IsIO": IsIO, "IsCO": IsCO, "IsRO": IsRO, "IsORO": IsORO };
           
            $("#spnFwdToAspNetUsersId").html(0);
            $.ajax({
                url: '/UserProfile/GetDataForFwd',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);
                    if (data.length != 0) {
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.ArmyNo + ' ' + item.RankAbbreviation + ' ' + item.Name + ' ' + item.DomainId, value: item.AspNetUsersId };

                        }))
                    }
                    else {

                        $(".spnFArmyNo").html("");
                        $(".spnFtoname").html("");
                        $(".spnFDomainName").html("");
                        $(".spnFAppName").html("");

                        $("#txtFwdName").val("");
                        $("#spnFwdToAspNetUsersId").html("0");
                        $("#spnFwdToUsersId").html("0");
                        alert("Army No/Offr Name/Domain ID not found.")
                    }
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            e.preventDefault();
            //alert(i.item.value)
            $("#txtFwdName").val(i.item.label);
            //alert(i.item.value)
            // var param1 = { "UnitMapId": i.item.value };
            //$("#btnIOProfileSerch").addClass('d-none');
            FwdData(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

    $("#btnForward").on("click", function () {

        /*  alert($("#txtspnTokenArmyNo").val());*/


        if (($("#aspntokenarmyno").html() == $("#txtspnTokenArmyNo").val()) || isToken == false) {
        $("#msgforfwd").html('');

        if (parseInt(spnStepId) != 0) {
            Swal.fire({
                title: 'Are you sure?',
                /*  text: "You want be Forward!",*/
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Forward it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    if (parseInt($("#spnFwdToAspNetUsersId").html()) != 0) {


                        var spnRequestId = $("#spnCurrentspnRequestId").html();

                        var Counter = parseInt($("#spnStepCounter").html());
                        if (Counter == 1 || Counter == 7 || Counter == 8 || Counter == 9 || Counter == 10) {


                            Counter = 2;

                        } else {

                            Counter = parseInt($("#spnStepCounter").html()) + 1;
                            //if (applyfor == 1 && Counter == 3)/// for ACG
                            //    Counter = 4;
                            //if (applyfor == 2 && Counter == 3)/// for ACG
                            //    Counter = 5;

                            //if (applyfor == 2 && parseInt($("#spnStepCounter").html()) == 3) {
                            //    Counter = 5;
                            //}
                        }


                        UpdateStepCounter(spnStepId, spnRequestId, Counter, "A");

                    }
                    else {
                        toastr.error("Please Select Officer ");
                    }
                }
            })
        }
        else {
            Swal.fire({
                title: 'Are you sure?',
                 /* text: "You want be Multiple Forward!",*/
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Forward it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    if (parseInt($("#spnFwdToAspNetUsersId").html()) != 0) {



                        
                        for (var fw = 0; fw < lstmultifwdarr.length; fw++) {
                            UpdateStepCounter(0, lstmultifwdarr[fw], 5, "A");
                        }
                       

                    }
                    else {
                        toastr.error("Please Select Officer ");
                    }
                }
            })
        }
        }
        else {
            $("#msgforfwd").html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Please Correct Token insert and Click refresh Button </span></div>');
        }
    });

    $("#btnRejected").on("click",function () {

      /*  $("#txtFrejectedRemarks").val($("#txtFrejectedRemarks").val() + "" + photo + "" + sing);*/
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be Forward!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Forward it!'
        }).then((result) => {
            if (result.isConfirmed) {
                var spnRequestId = $("#spnCurrentspnRequestId").html();
                var Counter = parseInt($("#spnStepCounter").html());
                if (Counter == 2)
                    Counter = 7
               else if (Counter == 3)
                    Counter = 8
                else if (Counter == 4)
                    Counter = 9
                else if (Counter == 5)
                    Counter = 10


                if ($("#txtFrejectedRemarks").val() != "" || $("#ddlRRemarks").val() != "")

                    UpdateStepCounter(spnStepId, spnRequestId, Counter, "R");
                else
                    toastr.error('Please Enter Remarks To Reject');

            }
        })
    });
});
function Reset() {
    $("#spnFwdToAspNetUsersId").html(0);
    $("#spnFwdToUsersId").html(0);
    $(".spnFArmyNo").html("");
    $(".spnFtoname").html("");
    $(".spnFDomainName").html("");  
    $(".spnFAppName").html("");

    $("#intoffsArmyNo").prop("checked", false);
    $("#intoffDomainId").prop("checked", false);
    $("#txtFwdName").val("");
    $(".serchfwd").addClass("d-none");
}
function GetDataFromBasicDetails(Id) {
    var userdata =
    {
        "Id": Id,


    };
    $.ajax({
        url: '/BasicDetail/GetDataByBasicDetailsId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                $("#basicphotos").attr('src',"/WriteReadData/photo/"+response.PhotoImagePath);
                $("#Basicsing").attr('src', "/WriteReadData/Signature/"+response.SignatureImagePath);
                $("#lblfdName").html(response.Name);
                $("#lblfdRank").html(response.RankName);
                $("#lblLfdarm").html(response.ArmedName);
                $("#lblfdArmyNo").html(response.ServiceNo);
                $("#lblfdMarks").html(response.IdenMark1);
                $("#lblfddob").html(DateFormateMMMM_dd_yyyy(response.DOB));
                $("#lblfdheight").html(response.Height);
                $("#lblfdadhar").html(response.AadhaarNo);
                $("#lblfdBloodGroup").html(response.BloodGroup);
                $("#lblfdpoi").html(response.PlaceOfIssue);
                $("#lblfddoi").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                $("#lblfdissuA").html(response.IssuingAuth);
                $("#lblfddateo").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                $("#lblfdaddress").html(response.Village + ',' + response.Tehsil + ',' + response.PO + ',' + response.PS + ',' + response.District + ',' + response.State + '' + response.PinCode );
            }
        }
    })
}
function FwdData(AspNetUsersId) {
    var userdata =
    {
        "AspNetUsersId": AspNetUsersId,
        

    };
    $.ajax({
        url: '/UserProfile/GetByAspnetUserIdBy',
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
                    $("#spnFwdToAspNetUsersId").html(response.AspNetUsersId);
                    $("#spnFwdToUsersId").html(response.UserId);

                    GetProfiledetailsByAspNetuserid(AspNetUsersId)
                    //$(".HProfileDetails").removeClass("d-none");
                    //$("#ForwardDetails").html("");
                    //$("#btnForward").removeClass("d-none");
                    //$("#spnCurrentspnRequestId").html(response.RequestId);
                    //if (StepCounter == 1) {
                    //    $(".spnFtoarmyno").html(response.IOArmyNo);
                    //    $(".spnFtoname").html(response.IOName);


                    //    $("#spnFrom").html(response.UserId);
                    //    $("#spnForwardTo").html(response.IOUserId);
                    //    $("#spnFwssusno").html(0);
                    //} else if (StepCounter == 2) {
                    //    $(".spnFtoarmyno").html(response.GSOArmyNo);
                    //    $(".spnFtoname").html(response.GSOName);

                    //    $("#spnFrom").html(response.IOUserId);
                    //    $("#spnForwardTo").html(response.GSOUserId);
                    //    $("#spnFwssusno").html(0);
                    //}
                    //else if (StepCounter == 3) {

                    //    $(".HProfileDetails").addClass("d-none");
                    //    $("#spnFrom").html(response.GSOUserId);
                    //    $("#spnFwssusno").html(101);

                    //}
                    //else if (StepCounter == 4) {

                    //    $(".HProfileDetails").addClass("d-none");
                    //    $("#spnFrom").html(101);
                    //    $("#spnForwardTo").html(29);
                    //    $("#spnFwssusno").html(0);

                    //}
                }
            }
            else {
                //$(".HProfileDetails").addClass("d-none");
                //$("#btnForward").addClass("d-none");
                //$("#ForwardDetails").html("Please Add Self Profile");

                //$(".spnFtoarmyno").html("");
                //$(".spnFtoname").html("");
                //$("#spnForwardTo").html(0);
                //$("#spnCurrentspnRequestId").html(0);


            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function GetProfiledetailsByAspNetuserid(AspNetUsersId) {
    //var param = "";
    //if (StepCounter == 3 && applyfor==1)
    //    var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    //else if (StepCounter == 4 && applyfor == 1)
    //    var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    //else if ((StepCounter == 2 ||StepCounter == 3) && applyfor == 2)
    //    var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    //else
        var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    $.ajax({
        url: '/UserProfile/GetDataForFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: param,
        type: 'POST',
        success: function (data) {
            if (data != null) {
                $(".spnFArmyNo").html(data[0].ArmyNo);
                $(".spnFtoname").html(data[0].RankAbbreviation +" "+ data[0].Name);
                $(".spnFDomainName").html(data[0].DomainId);
                $(".spnFAppName").html(data[0].AppointmentName);
            }
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
//function GetForwardHHierarchy(ArmyNo, StepCounter, spnRequestId) {
   
//    var userdata =
//    {
//        "StepId": StepCounter,
//        "RequestId": spnRequestId,

//    };
//    $.ajax({
//        url: '/UserProfile/GetDataForFwd',
//        contentType: 'application/x-www-form-urlencoded',
//        data: userdata,
//        type: 'POST',

//        success: function (response) {
//            if (response != "null" && response != null) {

//                if (response == InternalServerError) {
//                    Swal.fire({
//                        text: errormsg
//                    });
//                }
//                else if (response == 0) {

//                }

//                else {
//                    $(".HProfileDetails").removeClass("d-none");
//                    $("#ForwardDetails").html("");
//                    $("#btnForward").removeClass("d-none");
//                    $("#spnCurrentspnRequestId").html(response.RequestId);
//                    //if (StepCounter == 1) {
//                    //    $(".spnFtoarmyno").html(response.IOArmyNo);
//                    //    $(".spnFtoname").html(response.IOName);


//                    //    $("#spnFrom").html(response.UserId);
//                    //    $("#spnForwardTo").html(response.IOUserId);
//                    //    $("#spnFwssusno").html(0);
//                    //} else if (StepCounter == 2) {
//                    //    $(".spnFtoarmyno").html(response.GSOArmyNo);
//                    //    $(".spnFtoname").html(response.GSOName);

//                    //    $("#spnFrom").html(response.IOUserId);
//                    //    $("#spnForwardTo").html(response.GSOUserId);
//                    //    $("#spnFwssusno").html(0);
//                    //}
//                    //else if (StepCounter == 3) {

//                    //    $(".HProfileDetails").addClass("d-none");
//                    //    $("#spnFrom").html(response.GSOUserId);
//                    //    $("#spnFwssusno").html(101);

//                    //}
//                    //else if (StepCounter == 4) {

//                    //    $(".HProfileDetails").addClass("d-none");
//                    //    $("#spnFrom").html(101);
//                    //    $("#spnForwardTo").html(29);
//                    //    $("#spnFwssusno").html(0);

//                    //}
//                }
//            }
//            else {
//                $(".HProfileDetails").addClass("d-none");
//                $("#btnForward").addClass("d-none");
//                $("#ForwardDetails").html("Please Add Self Profile");

//                $(".spnFtoarmyno").html("");
//                $(".spnFtoname").html("");
//                $("#spnForwardTo").html(0);
//                $("#spnCurrentspnRequestId").html(0);
               

//            }
//        },
//        error: function (result) {
//            Swal.fire({
//                text: errormsg002
//            });
//        }
//    });
//}
function ForwardTo(RequestId, HType) {
    
    var remarks = ""+$("#ddlRemarks").val()+"";
    var userdata =
    {
        "TrnFwdId": 0,
        "RequestId": RequestId,
        "ToAspNetUsersId": $("#spnFwdToAspNetUsersId").html(),
        "ToUserId": $("#spnFwdToUsersId").html(),
        /*"FromUserId": $("#spnFrom").html(),*/
       // "ToUserId": $("#spnForwardTo").html(),
        /* "SusNo": $("#spnFwssusno").html(),*/
        "Remark": $("#txtFRemarks").val(),
        "Status": true,
        "TypeId": HType,
        "IsComplete": false,
        "RemarksIds": remarks,
    };
    $.ajax({
        url: '/BasicDetail/IcardFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
              
                if ($("#txtspnTokenArmyNo").val() != "") {
                    if (HType == 2 || HType == 3 || HType == 4 || HType == 5) {
                        var lsts = new Array();
                        var ids = $("#spnCurrentspnRequestId").html();
                        lsts.push(ids);
                        if (isToken == true) {
                            DataSignDigitaly(lsts, "tokenmsgforfwd", response.TrnFwdId);
                            DownloadPdf(RequestId);
                        }
                        else {
                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        }
                       
                       
                    }
                    else {
                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                }
                
                else {
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
            }
        }

    });
}
function RejecteTo(RequestId, HType) {
    var remarks = "" + $("#ddlRRemarks").val() + "";
    var userdata =
    {
        "TrnFwdId": 0,
        "RequestId": RequestId,
        "ToAspNetUsersId": $("#spnFwdToAspNetUsersId").html(),
        "ToUserId": $("#spnFwdToUsersId").html(),
        /*"FromUserId": $("#spnFrom").html(),*/
        // "ToUserId": $("#spnForwardTo").html(),
        /* "SusNo": $("#spnFwssusno").html(),*/
        "Remark": $("#txtFrejectedRemarks").val(),
        "Status": false,
        "TypeId": HType,
        "IsComplete": false,
        "RemarksIds": remarks
    };
    $.ajax({
        url: '/BasicDetail/IcardRejecte',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                setTimeout(function () {
                    location.reload();
                }, 2000);
            }
        }

    });
}
function UpdateStepCounter(stepId, spnRequestId, Counter,Flag) {
    var userdata =
    {
        "Id": stepId,
        "RequestId": spnRequestId,
        "StepId": Counter

    };
    $.ajax({
        url: '/BasicDetail/UpdateStepCounter',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                $("#FwdRecord").modal('hide');
                var HType = 0;
                if (Counter == 3) {
                    HType = 1;
                } else if (Counter == 4) {
                    HType = 2;
                } 
                if (Flag == "R") {
                    RejecteTo(spnRequestId, Counter);
                } else {
                    ForwardTo(spnRequestId, Counter);
                }
                if (applyfor == 1) {
                    SaveNotification(1, Counter, $("#spnFwdToAspNetUsersId").html(), spnRequestId)
                }
                else {
                    SaveNotification(1, (parseInt(Counter)+10), $("#spnFwdToAspNetUsersId").html(), spnRequestId)
                }
            }
        }

    });
}

function GetRequestHistory(spnRequestId) {
    var userdata =
    {
     
        "RequestId": spnRequestId,
      

    };
    var listItem = "";
    $.ajax({
        url: '/BasicDetail/GetRequestHistory',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                if (response.length > 0) {
                    for (var i = 0; i < response.length; i++) {
                        if (i == 0) {
                            listItem += '<div class="timeline-item">';
                            listItem += '<div class="timeline-item-marker">';
                            listItem += '<div class="timeline-item-marker-text "><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';
                            listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                            listItem += '</div>';
                            listItem += '<div class="timeline-item-content">';
                            listItem += 'I-Card Submit By -' + response[i].FromDomain + '(' + response[i].FromRank + ' ' + response[i].FromProfile + ')';

                            listItem += '</div>';
                            listItem += '</div>';
                        }
                        listItem += '<div class="timeline-item">';
                        listItem += '<div class="timeline-item-marker">';

                        if (response[i].Status == "Approved")
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';
                        else
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-danger">' + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + '</span></div>';


                        listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                        listItem += '</div>';
                        listItem += '<div class="timeline-item-content">';


                        listItem += '' + response[i].FromDomain + '(' + response[i].FromRank + ' ' + response[i].FromProfile + ')';
                        if (response[i].Status == "Approved")
                            listItem += '<br><span class="badge bg-success">' + response[i].Status + ' And Sent To</span>';
                        else
                            listItem += '<br><span class="badge bg-danger">' + response[i].Status + ' And Sent To</span>';

                        listItem += '<br> <strong class="text-center">Remark</strong> <br>' + response[i].Remark + '';

                        if (response[i].Remarks2 != null) {
                            var rem = response[i].Remarks2.split('#');
                            if (rem.length > 0) {
                               
                                listItem += '<ul>';
                                for (var j = 0; j < rem.length; j++) {
                                    listItem += '<li>' + rem[j] + '</li>';
                                }
                                listItem += '</ul>';
                            }
                        }

                      
                        listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'

                        if (response[i].IsComplete == 0) {
                            listItem += '<br><span class="badge bg-warning ">Pending from </span>';
                        }
                        listItem += '<br>' + response[i].ToDomain + '(' + response[i].ToRank + ' ' + response[i].ToProfile + ')';


                        if (response[i].Reason != null) {
                            listItem += '<br> <strong class="text-center text-danger">' + response[i].Reason + '</strong> <br> Unit Name :-' + response[i].UnitName + '';
                        }



                        listItem += '</div>';
                        listItem += '</div>';
                    }
                }
                else {
                    listItem += '<div class="timeline-item">';
                    listItem += '<div class="timeline-item-marker">';
                
                   
                    listItem += '</div>';
                    listItem += '<div class="timeline-item-content">';
                    listItem += 'I-Card Submitted Succesfully';

                    listItem += '</div>';
                    listItem += '</div>';

                    $("#RequestHistory").html(listItem);
                }
               
                $("#RequestHistory").html(listItem);
            } else {
                
            }
        }

    });
}

function DataExport(Data) {
    var userdata =
    {
        "Ids": Data,


    };
    $.ajax({
        url: '/BasicDetail/DataExport',
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

                else
                {
                    //var blob = new Blob([response], {
                    //    type: 'application/json'
                    //});
                    //var link = document.createElement('a');
                    //link.href = 'data:text/plain;charset=utf-8,' + encodeURIComponent(blob);
                    //link.download = "export.json";
                    //link.click();

                    var blob = new Blob([JSON.stringify(response, null, "\t")], { type: "application/json" });
                   
                    // Create a temporary anchor element
                    var link = document.createElement("a");
                    link.href = window.URL.createObjectURL(blob);

                   
                   
                  
                   // GetTokenSignXml(blob);
                    // Set the file name
                    link.download = "data.json";

                    // Append the anchor to the body
                    document.body.appendChild(link);

                    // Trigger the click event
                    link.click();

                    // Remove the anchor from the body
                    document.body.removeChild(link);


                    setTimeout(function () {
                        location.reload();
                    }, 1000);
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

function DataSignDigitaly(Data, msgid, TrnFwdId) {
    var userdata =
    {
        "Ids": Data,


    };
    $.ajax({
        url: '/BasicDetail/DataDigitalXmlSign',
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

                else {
                    //var blob = new Blob([response], {
                    //    type: 'application/json'
                    //});
                    //var link = document.createElement('a');
                    //link.href = 'data:text/plain;charset=utf-8,' + encodeURIComponent(blob);
                    //link.download = "export.json";
                    //link.click();
                  

                    var xmlString = jsonToXml(response);

                   
                    GetTokenSignXml(xmlString, msgid, TrnFwdId)
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
function GetTokenSignXml(xml, msgid, TrnFwdId) {


    /*    IcNo = "7f33df8ac6540b5cf7ccfd041d8c837641226444d9f1a4aa30a01924c0610996";*/
    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/SignXml',
        type: "POST",
        contentType: 'application/xml', // Set content type to XML
        data: xml, // Set the XML data
        success: function (response) {
            if (response) {
               
                var xmlContent = new XMLSerializer().serializeToString(response);
                
              
               // No Token Found
                if (xmlContent.indexOf("<Root>No Token Found</Root>") == -1) {

                    $("#" + msgid).html('<div class="mt-4 alert alert-success alert-dismissible fade show "><i class="fa fa-check " aria-hidden="true"></i><span class="m-lg-2">Token Detected  </span></div>');

                    SignXmlSendTOdatabase(xmlContent, TrnFwdId);
                    // Create a Blob from the XML string
                    //var blob = new Blob([xmlContent], { type: 'application/xml' });

                    //// Create a download link
                    //var downloadLink = document.createElement('a');
                    //downloadLink.href = window.URL.createObjectURL(blob);
                    //downloadLink.download = 'document.xml';

                    //// Trigger a click on the link to start the download
                    //document.body.appendChild(downloadLink);
                    //downloadLink.click();

                    //// Clean up: remove the download link
                    //document.body.removeChild(downloadLink);
                }
                else {
                    $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2"> No Token Found</span>.</div>');
                   
                }
            }

        },
        error: function (result) {

            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');


        }
    });


}
function SignXmlSendTOdatabase(XmlFile, TrnFwdId) {
    var userdata =
    {
        "TrnFwdId": TrnFwdId,
        "XmlFiles": XmlFile,


    };
    $.ajax({
        url: '/Log/XmlFileDigitalSign',
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

                else {
                   
                toastr.success('Xml Digital Sign Sucess');

                  
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
function jsonToXml(json) {
    var xml = '';
   
   
   
    for (var key in json) {
        i = 1;
        if (json.hasOwnProperty(key)) {

          
            xml += '<' + key + '>';

            if (typeof json[key] === 'object') {
                xml += jsonToXml(json[key]);
            } else {
                xml += json[key];
            }

         
                xml += '</' + key + '>';
        }
    }
   

    return xml;
}

function DownloadPdf(RequestId) {
    var userdata =
    {
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
                }

                else {
                    
                  
                  //  window.open('/DigitallysignaturePdf/' + response, '_blank');
                    if ($("#aspntokenarmyno").html() == $("#txtspnTokenArmyNo").val()) {
                        var url = "https://" + window.location.host + '/DigitallysignaturePdf/' + response;
                        digitalpdfsignature($("#txtspnTokenthumbprint").val(), url,'40','65');
                    }
                    //var blob = new Blob([JSON.stringify(response, null, "\t")], { type: "application/json" });

                    //// Create a temporary anchor element
                    //var link = document.createElement("a");
                    //link.href = window.URL.createObjectURL(blob);




                    //// GetTokenSignXml(blob);
                    //// Set the file name
                    //link.download = "data.json";

                    //// Append the anchor to the body
                    //document.body.appendChild(link);

                    //// Trigger the click event
                    //link.click();

                    //// Remove the anchor from the body
                    //document.body.removeChild(link);
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
function digitalpdfsignature(Thumbprint, pdfpath, XCoordinate, YCoordinate) {
    $("#loadingToken").show();
    $.ajax({
        url: 'http://localhost/Temporary_Listen_Addresses/ByteDigitalSignAsync',
        type: "Post",
        contentType: 'application/json; charset=utf-8',
        'data': JSON.stringify([{
            "Thumbprint": Thumbprint,
            "pdfpath": pdfpath,
            "XCoordinate": XCoordinate,
            "YCoordinate": YCoordinate,

        }]),
        success: function (response) {
            if (response) {

                $("#loadingToken").hide();

                // No Token Found
                if (response.Valid) {

                    base64toPDF(response.Message);
                    toastr.success('Pdf Digital Sign Sucess');
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
                else {
                    alert(response.Message)

                }
            }

        },
        error: function (result) {

            $("#" + msgid).html('<div class="mt-4 alert alert-danger alert-dismissible fade show "><i class="fa fa-times" aria-hidden="true"></i><span class="m-lg-2">DGIS Application Not Running</span>.</div>');


        }
    });

}
function base64toPDF(data) {
    var datat = data;
    var bufferArray = base64ToArrayBuffer(data);
    var blobStore = new Blob([bufferArray], { type: "application/pdf" });
    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobStore);
        return;
    }
    var data = window.URL.createObjectURL(blobStore);
    var link = document.createElement('a');
    document.body.appendChild(link);
    link.href = data;
    link.download = "digitalsignature.pdf";
    link.click();

    const fileWindow = window.open();
    const url = 'data:application/pdf;base64,' + btoa(
        new Uint8Array(bufferArray)
            .reduce((data1, byte) => data1 + String.fromCharCode(byte), '')
    );
    fileWindow.document.write(
        '<title>Digital Signature Pdf</title>' +
        '<body style="overflow: hidden; margin: 0">' +
        '<object width="100%" width="-webkit-fill-available" height="100%" height="-webkit-fill-available" type="application/pdf" data="' + encodeURI(url) + '"></object>' +
        '</body>'
    );

    window.URL.revokeObjectURL(data);
    link.remove();
}

function base64ToArrayBuffer(data) {
    var bString = window.atob(data);
    var bLength = bString.length;
    var bytes = new Uint8Array(bLength);
    for (var i = 0; i < bLength; i++) {
        var ascii = bString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
};

function GetByArmyNoIsToken(ArmyNo) {
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
                    isToken = response.IsToken;
                    if (response.IsToken == false)
                        $("#btntokenTofwd").addClass("d-none");
                    else
                        $("#btntokenTofwd").removeClass("d-none");
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