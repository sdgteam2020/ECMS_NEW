var lst = '<option value="1">Please Select</option>';
var comid = 0; var corId = 0; var divId = 0; var bdeId = 0; var FmnBranchId = 0; var PsoId = 0; var SubDteId = 0;
$(document).ready(function () {
    mMsater(0, "ddlCommand", 1, "");
    var val = 1;

    $('#ddlCommand').on('change', function () {
        comid=$(this).val();
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
        $("#ddlDiv").html(lst);
        $("#ddlBde").html(lst);
        $("#ddlFmnBranch").html(lst);
        $("#ddlPSODte").html(lst);
        $("#ddlDgSubDte").html(lst);
        $("#ddlUnit").html(lst);
    });
    $('#ddlCorps').on('change', function () {
        corId = $(this).val();
       mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
            $("#ddlBde").html(lst);
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst); 
    });
    $('#ddlDiv').on('change', function () {
        divId = $(this).val();
        mMsaterByParent(0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId     
        $("#ddlFmnBranch").html(lst);
        $("#ddlPSODte").html(lst);
        $("#ddlDgSubDte").html(lst);
        $("#ddlUnit").html(lst);
    });
    $('#ddlBde').on('change', function () {
        bdeId = $(this).val();
        mMsater(0, "ddlFmnBranch", FmnBranches, "");
    });
    $('#ddlPSODte').on('change', function () {
        SubDteId = $(this).val();
        
    });
    $('#ddlDgSubDte').on('change', function () {
        SubDteId = $(this).val();
       
    });
    GetLoginUnitMappingDetails();
    $('input[name="UnitTyperdi"]').click(function () {

        val = $("input[type='radio'][name=UnitTyperdi]:checked").val();
      
    
   
    if (val == "1") {
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
        $('#ddlBde').on('change', function () {
            $("#ddlUnit").html(lst);
            GetUnitByHierarchy("ddlUnit", 0, $("#ddlCommand").val(), $("#ddlCorps").val(), $("#ddlDiv").val(), $("#ddlBde").val(), 1, 1, 1);

        });
    }
    else if (val == "2") {

        $('#ddlCommand option').remove();
        $('#ddlCorps option').remove();
        $('#ddlBde option').remove();
        $('#ddlDiv option').remove();
        $('#ddlFmnBranch option').remove();
        $("#ddlUnit").html(lst);
        mMsater(0, "ddlCommand", 1, "");
        mMsater(0, "ddlFmnBranch", FmnBranches, "");

        $("#ddlPSODte").html(lst);
        $("#ddlDgSubDte").html(lst);
        $('#ddlFmnBranch').on('change', function () {
            $("#ddlUnit").html(lst);
            GetUnitByHierarchy("ddlUnit", 0, $("#ddlCommand").val(), $("#ddlCorps").val(), 1, 1, $("#ddlFmnBranch").val(), 1, 1);

        });
        $(".unittype").removeClass("d-none");
        $(".FmnBranch").removeClass("d-none");
        $(".DteBranch").addClass("d-none");
    }
    else if (val == "3") {
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
        $("#ddlUnit").html(lst);
        mMsater(0, "ddlPSODte", PSO, "");
        mMsater(0, "ddlDgSubDte", SubDte, "");
        $('#ddlDgSubDte').on('change', function () {
            $("#ddlUnit").html(lst);
            GetUnitByHierarchy("ddlUnit", 0, 1, 1, 1, 1, 1, $("#ddlPSODte").val(), $("#ddlDgSubDte").val());

        });
    }
    });


    $("#btnSearch").click(function () {
        GetCount();
    });
});
function GetCount() {
    var Itemlist = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Home/GetReportReturnCount',
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
                else  {
                    var dTOReportReturnCount = response.dTOReportReturnCount;
                    for (var i = 0; i < dTOReportReturnCount.length; i++) {
                        Itemlist += '<div class="c-dashboardInfo col-lg-1 col-md-6">';
                        Itemlist += '<div class="wrap">';
                        Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                        Itemlist += '' + dTOReportReturnCount[i].Name + '';
                        Itemlist += '</h4>';
                        Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + dTOReportReturnCount[i].Total+'</span>';
                        Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                        Itemlist += '</div>';
                        Itemlist += '</div>';
                        
                    }
                    var RecordOff = response.RecordOff;
                    for (var i = 0; i < RecordOff.length; i++) {
                        Itemlist += '<div class="c-dashboardInfo col-lg-1 col-md-6">';
                        Itemlist += '<div class="wrap">';
                        Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                        Itemlist += 'Approved/Reject/Pending';
                        Itemlist += '</h4>';
                        Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                        Itemlist += '' + RecordOff[i].Name + '';
                        Itemlist += '</h4>';
                      
                        Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + RecordOff[i].Total + '</span>';
                        Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                        Itemlist += '</div>';
                        Itemlist += '</div>';

                    }

                    $("#countlistreport").html(Itemlist);
                }

               
            }
            else {

            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function GetLoginUnitMappingDetails() {
   
    var listItem = "";
    var userdata =
    {
        "UnitMapId": 0
       
    };
    $.ajax({
        url: '/Master/GetALLByUnitMapWonUnit',
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

               
                    val = response.UnitType;
                    var lst = '<option value="1">Please Select</option>';

                    comid = response.ComdId;
                    corId = response.CorpsId;
                    divId = response.DivId;
                    bdeId = response.BdeId;
                    FmnBranchId = response.FmnBranchID;
                    PsoId = response.PsoId;
                    SubDteId = response.SubDteId;
                   



                    if (parseInt(response.UnitType) == 1) {
                            $("#UnitType1").prop("checked", true);

                        mMsater(true, response.ComdId, "ddlCommand", 1, "");
                        mMsater(true, response.CorpsId, "ddlCorps", 2, response.ComdId);
                        mMsaterByParent(true, response.DivId, "ddlDiv", 3, response.ComdId, response.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                        mMsaterByParent(true, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId

                       
                            GetUnitByHierarchy(true, "ddlUnit", response.UnitId, response.ComdId, response.CorpsId, response.DivId, response.BdeId, 1, 1, 1);

                          
                            $(".unittype").removeClass("d-none");
                            $(".FmnBranch").addClass("d-none");
                            $(".DteBranch").addClass("d-none");

                            $("#ddlFmnBranch").html(lst);
                            $("#ddlPSODte").html(lst);
                            $("#ddlDgSubDte").html(lst);

                        }
                     else if (parseInt(response.UnitType) == 2) {
                            $("#UnitType2").prop("checked", true);
                        if (response.ComdId == 1)
                            mMsater(false, response.ComdId, "ddlCommand", 1, "");
                        else
                            mMsater(true, response.ComdId, "ddlCommand", 1, "");


                        if (response.CorpsId == 1)
                            mMsater(false, response.CorpsId, "ddlCorps", 2, response.ComdId);
                        else
                            mMsater(true, response.CorpsId, "ddlCorps", 2, response.ComdId);


                        if (response.DivId == 1)
                            mMsaterByParent(false, response.DivId, "ddlDiv", 3, response.ComdId, response.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                        else
                            mMsaterByParent(true, response.DivId, "ddlDiv", 3, response.ComdId, response.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId

                        if (response.BdeId==1)
                            mMsaterByParent(false, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId
                        else
                            mMsaterByParent(true, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId


                        mMsater(true, response.FmnBranchID, "ddlFmnBranch", FmnBranches, "");


                        GetUnitByHierarchy(false, "ddlUnit", response.UnitId, response.ComdId, response.CorpsId, response.DivId, response.BdeId, response.FmnBranchID, 1, 1);

                            $("#ddlPSODte").html(lst);
                            $("#ddlDgSubDte").html(lst);

                            $(".unittype").removeClass("d-none");
                            $(".FmnBranch").removeClass("d-none");
                            $(".DteBranch").addClass("d-none");

                        }
                    else if (parseInt(response.UnitType) == 3) {
                            $("#UnitType3").prop("checked", true);

                        mMsater(response.PsoId, "ddlPSODte", PSO, "");
                        mMsater(response.SubDteId, "ddlDgSubDte", SubDte, "");
                        GetUnitByHierarchy("ddlUnit", response.UnitId, 1, 1, 1, 1, 1, response.PsoId, response.SubDteId);
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
                    
                }
            }
            else {
               
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });

}

function GetUnitByHierarchy(IsOnly,ddl, sectid, ComdId, CorpsId, DivId, BdeId, FmnBranchID, PsoId, SubDteId) {
    var listItem = "";
    var userdata =
    {
        "TableId": 0,
        "ComdId": ComdId,
        "CorpsId": CorpsId,
        "DivId": DivId,
        "BdeId": BdeId,
        "FmnBranchID": FmnBranchID,
        "PsoId": PsoId,
        "SubDteId": SubDteId,

    };
    $.ajax({
        url: '/Master/GetUnitByHierarchy',
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
                else if (response.length == 0) {
                   

                }

                else {

                  


                    for (var i = 0; i < response.length; i++) {
                        if (IsOnly == true && response[i].UnitId == sectid) {

                            listItem += '<option value="' + response[i].UnitId + '">' + response[i].UnitName + '</option>';
                        } else if
                            (IsOnly == false)
                        {
                            listItem += '<option value="' + response[i].UnitId + '">' + response[i].UnitName + '</option>';
                        }
                      
                        
                    }
                    $("#" + ddl + "").html(listItem);
                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }
                }
            }
            else {
                
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });

}

function mMsater(IsOnly,sectid = '', ddl, TableId, ParentId) {


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
                    if (IsOnly == false) {
                        listItemddl += '<option value="">Please Select</option>';
                    }


                    for (var i = 0; i < response.length; i++) {
                        if (IsOnly == true && response[i].Id == sectid) {
                            listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
                        }
                        else if (IsOnly == false) {
                            listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';

                        }
                    }
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

function mMsaterByParent(IsOnly,sectid = '', ddl, TableId, ComdId, CorpsId, DivId, BdeId) {


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
                     if (IsOnly == false) {
                        listItemddl += '<option value="">Please Select</option>';
                    }

                    for (var i = 0; i < response.length; i++) {
                        if (IsOnly == true && response[i].Id == sectid) {
                            listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
                        }
                        else if (IsOnly == false) {
                            listItemddl += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';

                        }
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