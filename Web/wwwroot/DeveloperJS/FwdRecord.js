var photo = "";
var sing = "";
var StepCounter = 0;
var applyfor = 0;
$(document).ready(function () {

    sessionStorage.removeItem('ArmyNo');

    var spnStepId = 0;
    $('.select2').select2({
        dropdownParent: $('#BasicDetails'),
        closeOnSelect: false
    });
    $('.select3').select2({
        dropdownParent: $('#FwdRecord'),
        closeOnSelect: false
    });
    
    GetAllOffsByUnitId("ddlfwdoffrs",0,0);
    $(".historyRequest").click(function () {
        $("#exampleModal").modal('show'); 
        GetRequestHistory($(this).closest("tr").find(".spnRequestId").html());
    });
    $('#ddlfwdoffrs').on('change', function () {
        $("#spnFwdToAspNetUsersId").html(0);
        $("#spnFwdToUsersId").html(0);
        $(".spnFArmyNo").html("");
        $(".spnFtoname").html("");
        $(".spnFDomainName").html("");
        FwdData($('#ddlfwdoffrs').val());
    });
    $('#ddlPhotos').on('change', function () {
        photo= $('#ddlPhotos').val();
    });
    $('#ddlsignature').on('change', function () {
        sing=$('#ddlsignature').val();
    });
    //$("#btnRejected").click(function () {

    //    $("#txtFrejectedRemarks").val($("#txtFrejectedRemarks").val() + "" + photo + "" + sing);
    //});
    $("#btnShowForward").click(function () {

        $("#BasicDetails").modal('hide');
        /*if (applyfor==1)*/
        $("#FwdRecord").modal('show');
    });
    $("input[name='Intoffrs']").change(function () {
        $(".serchfwd").removeClass("d-none");

        $("#spnFwdToAspNetUsersId").html(0);
        $("#spnFwdToUsersId").html(0);
        $(".spnFArmyNo").html("");
        $(".spnFtoname").html("");
        $(".spnFDomainName").html("");
    });

    $(".fwdrecord").click(function () {
        // ResetMapUnit();
        //alert($(this).closest("tr").find(".spnRequestId").html())

       
       
       // $("#FwdRecord").modal('show');
        $("#BasicDetails").modal('show');
        $(".spnFname").html($(this).closest("tr").find(".PersName").html());
        $(".spnFarmyno").html($(this).closest("tr").find(".ServiceNo").html());
        $("#spnStepCounter").html($(this).closest("tr").find(".spnStepCounterId").html());
        var spnRequestId = $(this).closest("tr").find(".spnRequestId").html();
        $("#spnCurrentspnRequestId").html(spnRequestId);
            spnStepId = $(this).closest("tr").find(".spnStepId").html();
           
        StepCounter = $(this).closest("tr").find(".spnStepCounterId").html();
        applyfor = $(this).closest("tr").find(".spnApplyFor").html();
        
            if (StepCounter == 1) {
                $(".recectopt").addClass("d-none");
                $("#btnRejected").addClass("d-none");
            }
            GetDataFromBasicDetails($(this).closest("tr").find(".spnBasicDetailId").html());
        if (StepCounter == 1) {
            $(".gsoio").html("IO");
            $("#btnForward").html("Forward To IO");

        }
        else if (StepCounter == 2) {
            $(".gsoio").html("GSO");
            $("#btnForward").html("Forward To GSO");
        }
        else if (StepCounter == 3) {
            $(".chkforserach").addClass("d-none");
            $(".gsoio").html("MI 11");
            $("#btnForward").html("Forward To MI 11");
            GetAllOffsByUnitId("ddlfwdoffrs", 0, 13);
        }
        else if (StepCounter == 4) {
            $(".chkforserach").addClass("d-none");
            $(".gsoio").html("HQ 54");
            $("#btnForward").html("Forward To HQ 54");
            GetAllOffsByUnitId("ddlfwdoffrs", 0, 11);
            }
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
            var param = { "Name": request.term, "TypeId": TypeId, "StepId": 1, "UnitId": 0 };
           
            $("#spnFwdToAspNetUsersId").html(0);
            $.ajax({
                url: '/UserProfile/GetDataForFwd',
                contentType: 'application/x-www-form-urlencoded',
                data: param,
                type: 'POST',
                success: function (data) {
                    console.log(data);

                    response($.map(data, function (item) {

                        $("#loading").addClass("d-none");
                        return { label: item.ArmyNo, value: item.AspNetUsersId };

                    }))
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
    $("#btnForward").click(function () {
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
                var Counter = parseInt($("#spnStepCounter").html()) + 1;

               
                UpdateStepCounter(spnStepId, spnRequestId, Counter,"A");

               
            }
        })  
    });

    $("#btnRejected").click(function () {

        $("#txtFrejectedRemarks").val($("#txtFrejectedRemarks").val() + "" + photo + "" + sing);
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
                var Counter = 1;

                if ($("#txtFrejectedRemarks").val()!="")

                    UpdateStepCounter(spnStepId, spnRequestId, Counter, "R");
                else
                    toastr.error('Please Enter Remarks To Reject');

            }
        })
    });
});
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
                $("#basicphotos").attr('src' ,response.PhotoImagePath);
                $("#Basicsing").attr('src', response.SignatureImagePath);
                $("#lblfdName").html(response.Name);
                $("#lblfdRank").html(response.RankName);
                $("#lblLfdarm").html(response.ArmedType);
                $("#lblfdArmyNo").html(response.ServiceNo);
                $("#lblfdMarks").html(response.IdentityMark);
                $("#lblfddob").html(DateFormateMMMM_dd_yyyy(response.DOB));
                $("#lblfdheight").html(response.Height);
                $("#lblfdadhar").html(response.AadhaarNo);
                $("#lblfdBloodGroup").html(response.BloodGroup);
                $("#lblfdpoi").html(response.PlaceOfIssue);
                $("#lblfddoi").html(DateFormateMMMM_dd_yyyy(response.DateOfIssue));
                $("#lblfdissuA").html(response.IssuingAuth);
                $("#lblfddateo").html(DateFormateMMMM_dd_yyyy(response.DateOfCommissioning));
                $("#lblfdaddress").html(response.PermanentAddress);
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
    var param = "";
    if (StepCounter==3)
        var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 13 };
   else if (StepCounter == 4)
        var param = { "Name": AspNetUsersId, "TypeId": 0,"UnitId":11 };
    else
        var param = { "Name": AspNetUsersId, "TypeId": 0, "UnitId": 0 };
    $.ajax({
        url: '/UserProfile/GetDataForFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: param,
        type: 'POST',
        success: function (data) {
            if (data != null) {
                $(".spnFArmyNo").html(data[0].ArmyNo);
                $(".spnFtoname").html(data[0].Name);
                $(".spnFDomainName").html(data[0].DomainId);
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
    };
    $.ajax({
        url: '/BasicDetail/IcardFwd',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                location.reload();
            }
        }

    });
}
function RejecteTo(RequestId, HType) {

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
    };
    $.ajax({
        url: '/BasicDetail/IcardRejecte',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {
                location.reload();
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
                            listItem += '<div class="timeline-item-marker-text "><span class="badge bg-success">' + DateFormateMM_dd_yyyy(response[i].UpdatedOn) + '</span></div>';
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
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-success">' + DateFormateMM_dd_yyyy(response[i].UpdatedOn) + '</span></div>';
                        else
                            listItem += '<div class="timeline-item-marker-text"><span class="badge bg-danger">' + DateFormateMM_dd_yyyy(response[i].UpdatedOn) + '</span></div>';


                        listItem += '<div class="timeline-item-marker-indicator bg-primary"></div>';
                        listItem += '</div>';
                        listItem += '<div class="timeline-item-content">';


                        listItem += '' + response[i].FromDomain + '(' + response[i].FromRank + ' ' + response[i].FromProfile + ')';
                        if (response[i].Status == "Approved")
                            listItem += '<br><span class="badge bg-success">' + response[i].Status + ' And Sent To</span>';
                        else
                            listItem += '<br><span class="badge bg-danger">' + response[i].Status + ' And Sent To</span>';

                        listItem += '<br> <strong class="text-center">Remark</strong> <br>' + response[i].Remark + '';
                        listItem += '<br><button type="button" class="btn btn-icon btn-round btn-light mr-1"><i class="fas fa-arrow-down"></i></button>'

                        if (response[i].IsComplete == 0) {
                            listItem += '<br><span class="badge bg-warning ">Pending from </span>';
                        }
                        listItem += '<br>' + response[i].ToDomain + '(' + response[i].ToRank + ' ' + response[i].ToProfile + ')';



                        listItem += '</div>';
                        listItem += '</div>';
                    }
                }
                else {
                    listItem += '<div class="timeline-item">';
                    listItem += '<div class="timeline-item-marker">';
                
                   
                    listItem += '</div>';
                    listItem += '<div class="timeline-item-content">';
                    listItem += 'I-Card Not Submited';

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