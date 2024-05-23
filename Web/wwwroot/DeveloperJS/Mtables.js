function mMsater(sectid = '', ddl, TableId, ParentId) {
    

    var userdata =
    {
        "id": TableId,
        "ParentId": ParentId,

    };
    $.ajax({
        url: '/Master/GetAllMMaster',
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

                    var listItemddl = "";
                    if (parseInt(TableId) == 7) {
                        listItemddl += '<option value="">Select Rank</option>';
                    } else
                    {
                        listItemddl += '<option value="">Please Select</option>';
                    }
                   
                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
                    }
                    $("#" + ddl +"").html(listItemddl);

                    //if (TableId == 5 || TableId == 7 || TableId == 8) {
                       
                    //    if (sectid != '') {
                    //        $("#" + ddl + " option").filter(function () {
                    //            return this.text == sectid;
                    //        }).attr('selected', true);

                    //    }
                    //}
                    //else
                    //{
                        if (sectid != '') {
                            $("#" + ddl + "").val(sectid);

                        }

                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

function mMsaterByParent(sectid = '', ddl, TableId, ComdId,CorpsId,DivId,BdeId) {


    var userdata =
    {
        "TableId": TableId,
        "ComdId": ComdId,
        "CorpsId": CorpsId,
        "DivId": DivId,
        "BdeId": BdeId,

    };
    $.ajax({
        url: '/Master/GetAllMMasterByParent',
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

                    var listItemddl = "";

                    listItemddl += '<option value="">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
                    }
                    $("#" + ddl + "").html(listItemddl);

                    //if (TableId == 5 || TableId == 7 || TableId == 8) {

                    //    if (sectid != '') {
                    //        $("#" + ddl + " option").filter(function () {
                    //            return this.text == sectid;
                    //        }).attr('selected', true);

                    //    }
                    //}
                    //else
                    //{
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function yeardata(sectid = '', ddl) {
    var listItemddl = "";
   
    listItemddl += '<option value="">select One</option>';


    for (var i = new Date().getFullYear(); i >= 1950; i--) {
        listItemddl += '<option value="' + i + '">' + i + '</option>';
    }
    $("#" + ddl + "").html(listItemddl);


    if (sectid != '') {
        $("#" + ddl + "").val(sectid);

    }
}

function GetAllOffsByUnitId(ddl, sectid, UnitId, ISIO, IsCO, IsRO, IsORO,BasicDetailsId) {
    var userdata =
    {
        "id": 0,
        "UnitId": UnitId,
        "ISIO": ISIO,
        "ISCO": IsCO,
        "IsRO": IsRO,
        "IsORO": IsORO,
        "BasicDetailsId": BasicDetailsId
    };
    $.ajax({
        url: '/UserProfile/GetOffrsByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    
                }

                else {
                    if (response[0].IsError == true) {
                        $("#ErrMess1").removeClass("d-none");
                        $("#ErrMess1").html(response[0].ErrorMessage);
                        $("#btnForward").prop("disabled", true);

                    }
                    else {
                        $("#ErrMess1").addClass("d-none");
                        $("#ErrMess1").html("");

                        $("#btnForward").prop("disabled", false);
                    }



                    var listItemddl = "";

                    listItemddl += '<option value="">Select Offr</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].AspNetUsersId + '">' + response[i].ArmyNo + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + '</option>';
                        
                    }
                    $("#" + ddl + "").html(listItemddl);

                   
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    if (IsORO == 1 && response.length == 1) {
                        //$("#spndefaultAspNetUsersId").html(0);
                        //$("#spndefaultAspNetUsersId").html(response[0].AspNetUsersId);

                        $("#" + ddl + "").val(response[0].AspNetUsersId)

                        $("#spnFwdToAspNetUsersId").html(0);
                        $("#spnFwdToUsersId").html(0);
                        $(".spnFArmyNo").html("");
                        $(".spnFtoname").html("");
                        $(".spnFDomainName").html("");
                        $(".spnFAppName").html("");

                        $("#intoffsArmyNo").prop("checked", false);
                        $("#intoffDomainId").prop("checked", false);
                        $(".serchfwd").addClass("d-none");
                        FwdData(response[0].AspNetUsersId);
                    }
                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}


function GetRemarks(ddl, sectid, RemarkTypeId) {
    var userdata =
    {
        "RemarkTypeId": RemarkTypeId,
        

    };
    $.ajax({
        url: '/BasicDetail/GetRemarks',
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

                    var listItemddl = "";

                    /* listItemddl += '<option value="">Please Offers</option>';*/
                    var RemarkTypeId = 0;
                    var count = 1;
                    for (var i = 0; i < response.length; i++) {

                        if (response[i].RemarkTypeId != RemarkTypeId) {
                            if (RemarkTypeId != 0)
                                listItemddl += '</optgroup>';
                            count = 1;
                            listItemddl += '<optgroup label="' + response[i].RemarksType +'">';
                        }

                        listItemddl += '<option value="' + response[i].RemarksId + '">' +count+'. ' + response[i].Remarks + '</option>';


                        RemarkTypeId = response[i].RemarkTypeId;


                        count++;
                    }
                    listItemddl += '</optgroup>';
                    $("#" + ddl + "").html(listItemddl);


                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}