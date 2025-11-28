var lst = '<option value="">All</option>';
var comid = 0; var corId = 0; var divId = 0; var bdeId = 0; var FmnBranchId = 0; var PsoId = 0; var SubDteId = 0;
var table; // Declare table variable outside the function to preserve the instance
var tableView; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    $("#btnprintreport").click(function () {
        //    window.scrollTo(0, 0);
        //var datef2 = new Date();
        //    $(".watermark").html($("#IpaddresGloble").html() + ' ' + DateFormateddMMyyyyhhmmss(datef2))
        //    /*$(".section-to-print-popup").focus();*/

        //    setTimeout(function () {
        //        window.print();
        //    }, 300); // 300 milliseconds delay

        PrintData("section_to_report_toPrint");
    });


    mMsater(0, "ddlCommand", 1, "");
    var val = 1;
    GetLoginUnitMappingDetails();
    $('#ddlCommand').on('change', function () {
        comid=$(this).val();
        mMsater(false,0, "ddlCorps", 2, $('#ddlCommand').val());
        $("#ddlDiv").html(lst);
        $("#ddlBde").html(lst);
        $("#ddlFmnBranch").html(lst);
        $("#ddlPSODte").html(lst);
        $("#ddlDgSubDte").html(lst);
        $("#ddlUnit").html(lst);
    });
    $('#ddlCorps').on('change', function () {
        corId = $(this).val();
        mMsaterByParent(false,0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
            $("#ddlBde").html(lst);
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst); 
    });
    $('#ddlDiv').on('change', function () {
        divId = $(this).val();
        mMsaterByParent(false,0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId     
        $("#ddlFmnBranch").html(lst);
        $("#ddlPSODte").html(lst);
        $("#ddlDgSubDte").html(lst);
        $("#ddlUnit").html(lst);
    });
    $('#ddlBde').on('change', function () {
        bdeId = $(this).val();
        mMsater(false, 0, "ddlFmnBranch", FmnBranches, "");
        GetUnitByHierarchy(false, "ddlUnit", 0, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), $('#ddlBde').val(), 1, 1, 1);

    });
    $('#ddlPSODte').on('change', function () {
        SubDteId = $(this).val();
        
    });
    $('#ddlDgSubDte').on('change', function () {
        SubDteId = $(this).val();
       
    });
   
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
        if ($("#spnclaimId").html() != "Army Level Reports")
            $(".SearchTab").addClass("d-none");

        $("#btnprintreport").removeClass("d-none");


        GetCount();
    });
});
function GetCount() {
    var Itemlist = "";
    var ItemlistR = "";
    var ItemlistA = "";
    var userdata =
    {
        "ComdId": $('#ddlCommand').val(),
        "CorpsId": $('#ddlCorps').val(),
        "DivId": $('#ddlDiv').val(),
        "BdeId": $('#ddlBde').val(),
        "FmnBranchID": $('#ddlFmnBranch').val(),
        "PsoId": $('#ddlPSODte').val(),
        "SubDteId": $('#ddlDgSubDte').val(),
        "UnitMapId": $('#ddlUnit').val(),

    };
    $.ajax({
        url: '/Home/GetReportReturnCount',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else  {
                    var dTOReportReturnCountOffs = response.dTOReportReturnCountOffs;
                    var GroupId = 0;
                    var totalaproved = 0;
                  
                    Itemlist += '<div class="seven">';
                    Itemlist += '<h1>Offrs</h1>';
                    Itemlist += '</div>';
                    for (var i = 0; i < dTOReportReturnCountOffs.length; i++) {

                        if (dTOReportReturnCountOffs[i].TypeId != GroupId) {

                            if (dTOReportReturnCountOffs[i].TypeId == 2) {

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId +'</span>';
                                Itemlist += '<div class="wrap">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl fwd to Approver';
                                Itemlist += '</h4>';
                                Total1apro = dTOReportReturnCountOffs[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountOffs[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            if (dTOReportReturnCountOffs[i].TypeId == 3) {
                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId + '</span>';
                                Itemlist += '<div class="wrap">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Approved Appl (Approver Level)';
                                Itemlist += '</h4>';

                                Total1apro = dTOReportReturnCountOffs[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountOffs[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            if (dTOReportReturnCountOffs[i].TypeId == 4) {
                               
                                var RecordOff = response.RecordOff;
                                var RecordoffCount = response.RecordoffCount;
                          
                                for (var j = 0; j < RecordOff.length; j++) {
                                    Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6">';
                                    Itemlist += '<a href="#"><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >99</span><span class="d-none spnRecordOfficeId" >' + RecordOff[j].RecordOfficeId + '</span>';
                                    Itemlist += '<div class="wrap">';
                                    Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                    Itemlist += 'Approved / Rejected / Pending';
                                    Itemlist += '</h4>';
                                    Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                    Itemlist += '' + RecordOff[j].Name + '';
                                    Itemlist += '</h4>';
                                    var counttot = 0;
                                    var Approved = 0;
                                    var Rejected = 0;
                                    var Pending = 0;
                                    for (var x = 0; x < RecordoffCount.length; x++) {

                                        if (RecordOff[j].RecordOfficeId == RecordoffCount[x].RecordOfficeId) {

                                            //Itemlist += '<span class="hind-font caption-12 c-dashboardInfo__count">';
                                            if (RecordoffCount[x].Name == "Approved")
                                                Approved = RecordoffCount[x].Total;
                                            else if (RecordoffCount[x].Name == "Rejected")
                                                Rejected = RecordoffCount[x].Total;
                                            else if (RecordoffCount[x].Name == "Pending")
                                                Pending = RecordoffCount[x].Total;

                                            //Itemlist += '</span>';
                                            counttot = 1;
                                        }
                                    }
                                    if (counttot == 0) {

                                    }
                                    Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">';
                                    Itemlist += '' + Approved + '/' + Rejected + '/' + Pending + '';
                                    Itemlist += '</span>';

                                    Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                    Itemlist += '</div>';
                                    Itemlist += '</div>';

                                }

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId + '</span>';

                                Itemlist += '<div class="wrap">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl Verified & Fwd to ADC';
                                Itemlist += '</h4>';
                               
                                Total1apro = dTOReportReturnCountOffs[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountOffs[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            Itemlist += '</div>';
                            Itemlist += '<hr>';
                            Itemlist += '<div class="row align-items-stretch">';

                            
                        }

                        if (dTOReportReturnCountOffs[i].IsApprove == 0) {
                            Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">0</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId + '</span>';
                            Itemlist += '<div class="wrap">';
                            Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                            Itemlist += '' + dTOReportReturnCountOffs[i].Name + '';
                            Itemlist += '</h4>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + dTOReportReturnCountOffs[i].Total + '</span>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                            Itemlist += '</div>';
                            Itemlist += '</a></div>';
                        }
                        
                        GroupId = dTOReportReturnCountOffs[i].TypeId;
                    }
                   
                    Itemlist += '<div class="seven">';
                    Itemlist += '<h1>JCOs/OR</h1>';
                    Itemlist += '</div>';
                   let dTOReportReturnCountJco = response.dTOReportReturnCountJco;
                   
                    for (var i = 0; i < dTOReportReturnCountJco.length; i++) {
                        var Total1apro = 0
                        if (dTOReportReturnCountJco[i].TypeId != GroupId) {

                            if (dTOReportReturnCountJco[i].TypeId == 2) {
                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                                Itemlist += '<div class="wrap">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl fwd to Approver';
                                Itemlist += '</h4>';
                                Total1apro = dTOReportReturnCountJco[i+1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountJco[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                           else if (dTOReportReturnCountJco[i].TypeId == 3 ) {

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                                Itemlist += '<div class="wrap">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Approved Appl (Approver Level)';
                                Itemlist += '</h4>';
                                Total1apro = dTOReportReturnCountJco[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountJco[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            if (dTOReportReturnCountJco[i].TypeId == 4 ) {

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                                Itemlist += '<div class="wrap">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl Verified & Fwd to ADC';
                                Itemlist += '</h4>';
                               
                                Total1apro = dTOReportReturnCountJco[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountJco[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            Itemlist += '</div>';
                            Itemlist += '<hr>';
                            Itemlist += '<div class="row align-items-stretch">';


                        }
                        if (dTOReportReturnCountJco[i].IsApprove == 0) {
                            Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">0</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                            Itemlist += '<div class="wrap">';
                            Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                            Itemlist += '' + dTOReportReturnCountJco[i].Name + '';
                            Itemlist += '</h4>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + dTOReportReturnCountJco[i].Total + '</span>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                            Itemlist += '</div>';
                            Itemlist += '</a></div>';
                        }
                        GroupId = dTOReportReturnCountJco[i].TypeId;
                    }

                    Itemlist += '</div>';


                    ///////////////////////////Add////////////////////////
                   
                    ItemlistR += '<div class="seven">';
                    ItemlistR += '<h1>Record Office Pending</h1>';
                    ItemlistR += '</div>';
                    ItemlistR += '<div class="row align-items-stretch">';
                    var RecordJcoPending = response.RecordJcoPending;
                    var RecordJco = response.RecordJco;
                    var countpending = 0;
                    for (var j = 0; j < RecordJco.length; j++) {
                        countpending = 0;
                        ItemlistR += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none applyTypeId">' + RecordJco[j].RecordOfficeId +'</span>';
                        ItemlistR += '<div class="wrap">';
                        ItemlistR += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                        ItemlistR += '' + RecordJco[j].Name + '';
                        ItemlistR += '</h4>';
                        for (var Z = 0; Z < RecordJcoPending.length; Z++) {
                            if (RecordJcoPending[Z].RecordOfficeId == RecordJco[j].RecordOfficeId) {
                                ItemlistR += ' <span class="d-none spnStepId">100</span><span class="hind-font caption-12 c-dashboardInfo__count">' + RecordJcoPending[Z].Total + '</span>';
                                countpending=1
                            }
                        }
                        if (countpending == 0) {
                            ItemlistR += ' <span class="d-none spnStepId" >0</span><span class="hind-font caption-12 c-dashboardInfo__count">0</span>';

                        }
                        ItemlistR += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                        ItemlistR += '</div>';
                        ItemlistR += '</a></div>';

                    }
                    ItemlistR += '</div>';
                  
                    

                    //ItemlistA += '<div class="seven">';
                    //ItemlistA += '<h1>Record Office Reject</h1>';
                    //ItemlistA += '</div>';
                    //ItemlistA += '<div class="row align-items-stretch">';
                    //var RecordJcoCountApproved = response.RecordJcoCountApproved;
                    //for (var j = 0; j < RecordJcoCountApproved.length; j++) {
                    //    ItemlistA += '<div class="c-dashboardInfo col-lg-1 col-sm-6">';
                    //    ItemlistA += '<div class="wrap">';
                    //    ItemlistA += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                    //    ItemlistA += '' + RecordJcoCountApproved[j].Name + '';
                    //    ItemlistA += '</h4>';
                    //    ItemlistA += ' <span class="hind-font caption-12 c-dashboardInfo__count">' + RecordJcoCountApproved[j].Total + '</span>';
                    //    ItemlistA += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                    //    ItemlistA += '</div>';
                    //    ItemlistA += '</div>';
                       

                    //}
                    ItemlistA += '</div>';
                   

                    $("#countlistreport").html(Itemlist);
                    $("#RecordOfficeCountPendding").html(ItemlistR);
                    $("#RecordOfficeCountApprove").html(ItemlistA);

                    $("body").on("click", ".c-dashboardInfo", function () {
                        
                       // alert($(this).closest("div").find(".spnStepId").html())
                        if ($(this).closest("div").find(".IsApproveId").html() == "0" && $(this).closest("div").find(".spnStepId").html() == "3" && $(this).closest("div").find(".applyTypeId").html()=="2") {

                            $("#RecordOfficeCountPendding").removeClass("d-none");
                            $(".RecordCount").addClass("d-none");
                        }
                        //else if ($(this).closest("div").find(".spnStepId").html() == "8" && $(this).closest("div").find(".applyTypeId").html() == "2") {

                        //    $("#RecordOfficeCountApprove").removeClass("d-none");
                        //    $(".RecordCount").addClass("d-none");
                        //}
                        //else
                       else if ($(this).closest("div").find(".spnStepId").html() == "99") {
                          //  alert($(this).closest("div").find(".spnRecordOfficeId").html())
                            $("#lblRepotReturnHistory").html($(this).closest("div").find(".c-dashboardInfo__title").html().replace("<br>", ""));

                            $("#RepotReturnHistory").modal("show");
                            GetReportReturnHistory($(this).closest("div").find(".spnStepId").html(), $(this).closest("div").find(".spnRecordOfficeId").html(), $(this).closest("div").find(".IsApproveId").html());

                        }
                        else {
                          //  alert($(this).closest("div").find(".spnStepId").html()+'-' + $(this).closest("div").find(".IsApproveId").html());

                          
                            $("#lblRepotReturnHistory").html($(this).closest("div").find(".c-dashboardInfo__title").html().replace("<br>", ""));
                            $("#RepotReturnHistory").modal("show");
                            
                            GetReportReturnHistory($(this).closest("div").find(".spnStepId").html(), $(this).closest("div").find(".applyTypeId").html(), $(this).closest("div").find(".IsApproveId").html());
                        }
                    });
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

function GetReportReturnHistory(spnStepId, applyTypeId, IsApproveId) {
    if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
        $("#tbldatadialog").DataTable().destroy();
    }
    var userdata =
    {
        "TableId": 0,
        "ComdId": $('#ddlCommand').val(),
        "CorpsId": $('#ddlCorps').val(),
        "DivId": $('#ddlDiv').val(),
        "BdeId": $('#ddlBde').val(),
        "FmnBranchID": $('#ddlFmnBranch').val(),
        "PsoId": $('#ddlPSODte').val(),
        "SubDteId": $('#ddlDgSubDte').val(),
        "UnitMapId": $('#ddlUnit').val()
    };
    tableView = $("#tbldatadialog").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[1, 'desc']], // Default sorting on the first column
        responsive: true,
        autoWidth: false,
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                applyForId: applyTypeId,
                stepId: spnStepId,
                isApproveId: IsApproveId,
                data: {
                    //tableId: 0,
                    //comdId: $('#ddlCommand').val(),
                    //corpsId: $('#ddlCorps').val(),
                    //divId: $('#ddlDiv').val(),
                    //bdeId: $('#ddlBde').val(),
                    //fmnBranchID: $('#ddlFmnBranch').val(),
                    //psoId: $('#ddlPSODte').val(),
                    //subDteId: $('#ddlDgSubDte').val(),
                    //unitMapId: $('#ddlUnit').val()
                    userdata
                }
            };
            try {
                let response = await fetch("/Home/GetRecordHistory", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify(requestData)
                });
                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                $("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            // Serial number column
            //{ data: "RequestId", name: "RequestId", visible: false },
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "ServiceNo", name: "ServiceNo" },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.RankName} ${row.FName} ${row.LName != null ? row.LName : ""}`;
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return row.RankFrom != null ? `<span id='divName'>${row.RankFrom} ${row.NameFrom} (${row.ArmyNoFrom}) (${row.DomainIdFrom})</span>` : "<span id='divName'></span>";
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return row.RankTo != null ? `<span id='divName'>${row.RankTo} ${row.NameTo} (${row.ArmyNoTo}) (${row.DomainIdTo})</span>` : "<span id='divName'></span>";
                }
            },
            {
                data: 'RequestId',
                name: 'RequestId',
                render: function (data, type, row) {
                    return data ? `<span id='comdName'>${data}</span>` : '';
                }
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return `<span id='comdName'>${DateFormateddMMyyyyhhmmss(data)}</span>`;
                }
            },
            {
                data: "StatusName",
                name: "StatusName",
                render: function (data, type, row) {
                    let color = 'primary';
                    if (data == "Pending") {
                        color = 'warning';
                    }
                    else if (data == "Rejected") {
                        color = 'danger';
                    }
                    else {
                        color = 'success'
                    }
                    return data ? `<span id='comdName'><span class='badge badge-${color} mr-1' >${row.StatusName}</span></span>` : `<span id='comdName'><span class='badge badge-primary mr-1' >Action Pending</span></span>`;
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_Claim',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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

               
                   var val = response.UnitType;
                    var lst = '<option value="1">Please Select</option>';

                    comid = response.ComdId;
                    corId = response.CorpsId;
                    divId = response.DivId;
                    bdeId = response.BdeId;
                    FmnBranchId = response.FmnBranchID;
                    PsoId = response.PsoId;
                    SubDteId = response.SubDteId;
                   

                    

                    if (parseInt(response.UnitType) == 1 && $("#spnclaimId").html() != "Army Level Reports") {
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
                    else if (parseInt(response.UnitType) == 2 || $("#spnclaimId").html() =="Army Level Reports") {
                        $("#UnitType2").prop("checked", true);

                        if ($("#spnclaimId").html() == "Army Level Reports") {
                            mMsater(false, '', "ddlCommand", 1, "");
                        }
                        else {
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

                            if (response.BdeId == 1)
                                mMsaterByParent(false, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId
                            else
                                mMsaterByParent(true, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId


                            mMsater(true, response.FmnBranchID, "ddlFmnBranch", FmnBranches, "");


                            GetUnitByHierarchy(false, "ddlUnit", response.UnitId, response.ComdId, response.CorpsId, response.DivId, response.BdeId, response.FmnBranchID, 1, 1);

                        }
                            $("#ddlPSODte").html(lst);
                            $("#ddlDgSubDte").html(lst);

                            $(".unittype").removeClass("d-none");
                            //$(".FmnBranch").removeClass("d-none");
                            //$(".DteBranch").addClass("d-none");

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
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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

                  

                    listItem += '<option value="">All</option>';
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

    const payload = {
        tableName: "",
        id: TableId,
        parentId: ParentId ? Number(ParentId) : null   // ⭐ THIS IS IMPORTANT
    };

    $.ajax({
        url: '/Master/GetAllMMaster',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(payload),
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
                        listItemddl += '<option value="">All</option>';
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
    const payload = {
        TableId: TableId ? Number(TableId) : null,
        ComdId: ComdId ? Number(ComdId) : null,
        CorpsId: CorpsId ? Number(CorpsId) : null,
        DivId: DivId ? Number(DivId) : null,
        BdeId: BdeId ? Number(BdeId) : null
    };
    $.ajax({
        url: '/Master/GetAllMMasterByParent',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(payload),
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
                         listItemddl += '<option value="">All</option>';
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