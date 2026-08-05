let lst = `<option value=${null}>All</option>`;
var comid = 0; var corId = 0; var divId = 0; var bdeId = 0; var FmnBranchId = 0; var PsoId = 0; var SubDteId = 0;
var table; // Declare table variable outside the function to preserve the instance
var tableView; // Declare table variable outside the function to preserve the instance
let UnitType = 0;
$(async function () {
    document.documentElement.classList.add('ecms-report-scroll-lock');
    document.body.classList.add('ecms-lock-page-scroll', 'ecms-report-scroll-lock');

    // Keep the modal outside the layout stacking context so its backdrop
    // always remains behind the dialog.
    const reportModal = document.getElementById('RepotReturnHistory');
    if (reportModal && reportModal.parentElement !== document.body) {
        document.body.appendChild(reportModal);
    }

    $("#RepotReturnHistory")
        .off('hidden.bs.modal.ecmsReportHistory')
        .on('hidden.bs.modal.ecmsReportHistory', function () {
            $(window).off('resize.ecmsReportHistory');
        });

    $(window)
        .off('pagehide.ecmsReport')
        .on('pagehide.ecmsReport', function () {
            document.documentElement.classList.remove('ecms-report-scroll-lock');
            document.body.classList.remove('ecms-lock-page-scroll', 'ecms-report-scroll-lock');
            $(window).off('resize.ecmsReportHistory');
        });

    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    $("#btnprintreport").on("click", function () {
        PrintData("section_to_report_toPrint");
    });
    if ($('#spnclaimId').length > 0) {
        if ($('#spnclaimId').html() === 'Army Level Reports' || $('#spnclaimId').html() === 'Fmn Level Reports') {
            await GetLoginUnitMappingDetails();
        }
        else {
            GetCount();
        }
    }
    else {
        GetCount();
    }

    if ($('#ddlCommand').length > 0) {
        let lastVal = $('#ddlCommand').val();

        $('#ddlCommand').on('change', async function () {
            const newVal = $(this).val();
            if (newVal !== lastVal || $('#ddlCommand option').length === 1) {
                lastVal = newVal;

                if (newVal == null || newVal === "null") {
                    $("#ddlCorps").html(lst);
                } else {
                    await mMsater(false, 0, "ddlCorps", 2, newVal);
                }

                $("#ddlDiv").html(lst);
                $("#ddlBde").html(lst);
                $("#ddlFmnBranch").html(lst);
                $("#ddlPSODte").html(lst);
                $("#ddlDgSubDte").html(lst);
                $("#ddlUnit").html(lst);
            }
        });

        $('#ddlCommand').on('click', async function () {
            const val = $(this).val();

            // If only one option and user clicks it (again), manually trigger
            if ($('#ddlCommand option').length === 1) {
                $('#ddlCommand').trigger('change');
            }
        });
    }

    if ($('#ddlCorps').length > 0) {
        $('#ddlCorps').on('change', async function () {
            corId = $(this).val();
            if ($('#ddlCorps').val() == null || $('#ddlCorps').val() == "null") {
                $("#ddlDiv").html(lst);
            }
            else {
                await mMsaterByParent(false, 0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
            }
            $("#ddlBde").html(lst);
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst);
        });
    }

    if ($('#ddlDiv').length > 0) {
        $('#ddlDiv').on('change', async function () {
            divId = $(this).val();
            if ($('#ddlDiv').val() == null || $('#ddlDiv').val() == "null") {
                $("#ddlBde").html(lst);
            }
            else {
                await mMsaterByParent(false, 0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId   
            }
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst);
        });
    }

    if ($('#ddlBde').length > 0) {
        $('#ddlBde').on('change', async function () {
            bdeId = $(this).val();
            if (UnitType == "2") {
                if ($("#spnclaimId").html() == "Army Level Reports") {
                    await mMsater(false, 0, "ddlFmnBranch", FmnBranches, "");

                }
                else {
                    await mMsater(true, FmnBranchId, "ddlFmnBranch", FmnBranches, "");
                }
            }

            await GetUnitByHierarchy(false, "ddlUnit", 0, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), $('#ddlBde').val(), 1, 1, 1);

        });
    }

    if ($('#ddlFmnBranch').length > 0) {
        $('#ddlFmnBranch').on('change', async function () {
            FmnBranchId = $(this).val();
            await GetUnitByHierarchy(false, "ddlUnit", 0, $("#ddlCommand").val(), $("#ddlCorps").val(), 1, 1, $("#ddlFmnBranch").val(), 1, 1);

        });
    }

    if ($('#ddlDgSubDte').length > 0) {
        $('#ddlDgSubDte').on('change', async function () {
            SubDteId = $(this).val();
            await GetUnitByHierarchy(false, "ddlUnit", 0, 1, 1, 1, 1, 1, PsoId, $("#ddlDgSubDte").val());
        });
    }

    if ($('#ddlPSODte').length > 0) {
        $('#ddlPSODte').on('change', async function () {
            PsoId = $(this).val();
            await GetUnitByHierarchy(false, "ddlUnit", 0, 1, 1, 1, 1, 1, $("#ddlPSODte").val(), SubDteId);
        });
    }

    $('input[name="UnitTyperdi"]').on("click", async function () {

        UnitType = Number($("input[type='radio'][name='UnitTyperdi']:checked").val() || 0);

        if (UnitType == 1) {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();


            if ($("#spnclaimId").html() == "Army Level Reports") {
                await mMsater(false, '', "ddlCommand", 1, "");

                $("#ddlCorps").html(lst);
                $("#ddlDiv").html(lst);
                $("#ddlBde").html(lst);
            }
            else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                await mMsater(true, comid, "ddlCommand", 1, "");
            }
            else {
                await mMsater(true, comid, "ddlCommand", 1, "");

            }
            if ($('#ddlCommand option').length === 1) {
                $('#ddlCommand').trigger('change');
            }

            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
        }
        else if (UnitType == 2) {

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();
            $('#ddlFmnBranch option').remove();
            $("#ddlUnit").html(lst);

            if ($("#spnclaimId").html() == "Army Level Reports") {
                $(".FmnBranch").removeClass("d-none");

                await mMsater(false, '', "ddlCommand", 1, "");
                await mMsater(false, '', "ddlFmnBranch", FmnBranches, "");

                $("#ddlCorps").html(lst);
                $("#ddlDiv").html(lst);
                $("#ddlBde").html(lst);
            }
            else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                $(".FmnBranch").addClass("d-none");

                await mMsater(true, comid, "ddlCommand", 1, "");
                await mMsater(true, FmnBranchId, "ddlFmnBranch", FmnBranches, "");
            }
            else {
                $(".FmnBranch").addClass("d-none");

                await mMsater(true, comid, "ddlCommand", 1, "");
                await mMsater(true, FmnBranchId, "ddlFmnBranch", FmnBranches, "");

            }
            if ($('#ddlCommand option').length === 1) {
                $('#ddlCommand').trigger('change');
            }

            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

            $(".unittype").removeClass("d-none");
            $(".DteBranch").addClass("d-none");
        }
        else if (UnitType == 3) {
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

            if ($("#spnclaimId").html() == "Army Level Reports") {
                await mMsater(false, '', "ddlPSODte", PSO, "");
                await mMsater(false, '', "ddlDgSubDte", SubDte, "");
            }
            else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                await mMsater(true, PsoId, "ddlPSODte", PSO, "");
                await mMsater(true, SubDteId, "ddlDgSubDte", SubDte, "");
            }
            else {
                await mMsater(true, PsoId, "ddlPSODte", PSO, "");
                await mMsater(true, SubDteId, "ddlDgSubDte", SubDte, "");

            }
        }
    });
    if ($("#btnSearch").length > 0) {
        $("#btnSearch").on("click", function () {
            $("#btnprintreport").removeClass("d-none");
            GetCount();
        });
    }

});
function parseVal(val) {
    if (val === "null" || val === undefined || val === "") {
        return null;
    }
    return val;
}
function GetCount() {
    var Itemlist = "";
    var ItemlistR = "";
    var ItemlistA = "";

    var requestData =
    {
        "TableId": 0,
        "UnitType": $("input[type='radio'][name=UnitTyperdi]").length > 0 ? parseVal($("input[type='radio'][name=UnitTyperdi]:checked").val()) : null,
        "ComdId": $('#ddlCommand').length > 0 ? parseVal($('#ddlCommand').val()) : null,
        "CorpsId": $('#ddlCorps').length > 0 ? parseVal($('#ddlCorps').val()) : null,
        "DivId": $('#ddlDiv').length > 0 ? parseVal($('#ddlDiv').val()) : null,
        "BdeId": $('#ddlBde').length > 0 ? parseVal($('#ddlBde').val()) : null,
        "FmnBranchID": $('#ddlFmnBranch').length > 0 ? parseVal($('#ddlFmnBranch').val()) : null,
        "PsoId": $('#ddlPSODte').length > 0 ? parseVal($('#ddlPSODte').val()) : null,
        "SubDteId": $('#ddlDgSubDte').length > 0 ? parseVal($('#ddlDgSubDte').val()) : null,
        "UnitMapId": $('#ddlUnit').length > 0 ? parseVal($('#ddlUnit').val()) : null

    };
    let jsonData = JSON.stringify(requestData);

    let encrypted = encryptPayloadData(jsonData);


    $.ajax({
        url: '/Home/GetReportReturnCount',
        contentType: 'application/x-www-form-urlencoded',
        data: { "request": encrypted },
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
                    var dTOReportReturnCountOffs = response.dTOReportReturnCountOffs;
                    var GroupId = 0;
                    var totalaproved = 0;

                    Itemlist += '<div class="seven ecms-section">';
                    Itemlist += '<h1 class="ecms-section-title">Offrs</h1>';
                    Itemlist += '</div>';
                    for (var i = 0; i < dTOReportReturnCountOffs.length; i++) {

                        if (dTOReportReturnCountOffs[i].TypeId != GroupId) {

                            if (dTOReportReturnCountOffs[i].TypeId == 2) {

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId + '</span>';
                                Itemlist += '<div class="wrap ecms-tile">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl fwd to Approver';
                                Itemlist += '</h4>';
                                Total1apro = dTOReportReturnCountOffs[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountOffs[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            if (dTOReportReturnCountOffs[i].TypeId == 3) {
                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId + '</span>';
                                Itemlist += '<div class="wrap ecms-tile">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Approved Appl (Approver Level)';
                                Itemlist += '</h4>';

                                Total1apro = dTOReportReturnCountOffs[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountOffs[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + Total1apro + '</span>';
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
                                    Itemlist += '<div class="wrap ecms-tile">';
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

                                            //Itemlist += '<span class="hind-font caption-12 c-dashboardInfo__count count">';
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
                                    Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">';
                                    Itemlist += '' + Approved + '/' + Rejected + '/' + Pending + '';
                                    Itemlist += '</span>';

                                    Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                    Itemlist += '</div>';
                                    Itemlist += '</div>';

                                }

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">1</span><span class="d-none spnStepId" >' + dTOReportReturnCountOffs[i].StepId + '</span>';

                                Itemlist += '<div class="wrap ecms-tile">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl Verified & Fwd to ADC';
                                Itemlist += '</h4>';

                                Total1apro = dTOReportReturnCountOffs[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountOffs[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + Total1apro + '</span>';
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
                            Itemlist += '<div class="wrap ecms-tile">';
                            Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                            Itemlist += '' + dTOReportReturnCountOffs[i].Name + '';
                            Itemlist += '</h4>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + dTOReportReturnCountOffs[i].Total + '</span>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                            Itemlist += '</div>';
                            Itemlist += '</a></div>';
                        }

                        GroupId = dTOReportReturnCountOffs[i].TypeId;
                    }

                    Itemlist += '<div class="seven ecms-section">';
                    Itemlist += '<h1 class="ecms-section-title">JCOs/OR</h1>';
                    Itemlist += '</div>';
                    let dTOReportReturnCountJco = response.dTOReportReturnCountJco;

                    for (var i = 0; i < dTOReportReturnCountJco.length; i++) {
                        var Total1apro = 0
                        if (dTOReportReturnCountJco[i].TypeId != GroupId) {

                            if (dTOReportReturnCountJco[i].TypeId == 2) {
                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                                Itemlist += '<div class="wrap ecms-tile">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl fwd to Approver';
                                Itemlist += '</h4>';
                                Total1apro = dTOReportReturnCountJco[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountJco[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            else if (dTOReportReturnCountJco[i].TypeId == 3) {

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                                Itemlist += '<div class="wrap ecms-tile">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Approved Appl (Approver Level)';
                                Itemlist += '</h4>';
                                Total1apro = dTOReportReturnCountJco[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountJco[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + Total1apro + '</span>';
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                                Itemlist += '</div>';
                                Itemlist += '</a></div>';
                            }
                            if (dTOReportReturnCountJco[i].TypeId == 4) {

                                Itemlist += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none IsApproveId">1</span><span class="d-none applyTypeId">2</span><span class="d-none spnStepId" >' + dTOReportReturnCountJco[i].StepId + '</span>';
                                Itemlist += '<div class="wrap ecms-tile">';
                                Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                                Itemlist += 'Appl Verified & Fwd to ADC';
                                Itemlist += '</h4>';

                                Total1apro = dTOReportReturnCountJco[i + 1].Total;
                                Total1apro = Total1apro + dTOReportReturnCountJco[i].Total;
                                Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + Total1apro + '</span>';
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
                            Itemlist += '<div class="wrap ecms-tile">';
                            Itemlist += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                            Itemlist += '' + dTOReportReturnCountJco[i].Name + '';
                            Itemlist += '</h4>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + dTOReportReturnCountJco[i].Total + '</span>';

                            Itemlist += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                            Itemlist += '</div>';
                            Itemlist += '</a></div>';
                        }
                        GroupId = dTOReportReturnCountJco[i].TypeId;
                    }

                    Itemlist += '</div>';


                    ///////////////////////////Add////////////////////////

                    ItemlistR += '<div class="seven ecms-section">';
                    ItemlistR += '<h1 class="ecms-section-title">Record Office Pending</h1>';
                    ItemlistR += '</div>';
                    ItemlistR += '<div class="row align-items-stretch">';
                    var RecordJcoPending = response.RecordJcoPending;
                    var RecordJco = response.RecordJco;
                    var countpending = 0;
                    for (var j = 0; j < RecordJco.length; j++) {
                        countpending = 0;
                        ItemlistR += '<div class="c-dashboardInfo col-lg-1 col-sm-6"><a href="#"><span class="d-none applyTypeId">' + RecordJco[j].RecordOfficeId + '</span>';
                        ItemlistR += '<div class="wrap ecms-tile">';
                        ItemlistR += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                        ItemlistR += '' + RecordJco[j].Name + '';
                        ItemlistR += '</h4>';
                        for (var Z = 0; Z < RecordJcoPending.length; Z++) {
                            if (RecordJcoPending[Z].RecordOfficeId == RecordJco[j].RecordOfficeId) {
                                ItemlistR += ' <span class="d-none spnStepId">100</span><span class="hind-font caption-12 c-dashboardInfo__count count">' + RecordJcoPending[Z].Total + '</span>';
                                countpending = 1
                            }
                        }
                        if (countpending == 0) {
                            ItemlistR += ' <span class="d-none spnStepId" >0</span><span class="hind-font caption-12 c-dashboardInfo__count count">0</span>';

                        }
                        ItemlistR += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                        ItemlistR += '</div>';
                        ItemlistR += '</a></div>';

                    }
                    ItemlistR += '</div>';



                    //ItemlistA += '<div class="seven ecms-section">';
                    //ItemlistA += '<h1>Record Office Reject</h1>';
                    //ItemlistA += '</div>';
                    //ItemlistA += '<div class="row align-items-stretch">';
                    //var RecordJcoCountApproved = response.RecordJcoCountApproved;
                    //for (var j = 0; j < RecordJcoCountApproved.length; j++) {
                    //    ItemlistA += '<div class="c-dashboardInfo col-lg-1 col-sm-6">';
                    //    ItemlistA += '<div class="wrap ecms-tile">';
                    //    ItemlistA += '<h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">';
                    //    ItemlistA += '' + RecordJcoCountApproved[j].Name + '';
                    //    ItemlistA += '</h4>';
                    //    ItemlistA += ' <span class="hind-font caption-12 c-dashboardInfo__count count">' + RecordJcoCountApproved[j].Total + '</span>';
                    //    ItemlistA += ' <span class="hind-font caption-12 c-dashboardInfo__subInfo"></span>';
                    //    ItemlistA += '</div>';
                    //    ItemlistA += '</div>';


                    //}
                    ItemlistA += '</div>';


                    $("#countlistreport").html(Itemlist);
                    $("#RecordOfficeCountPendding").html(ItemlistR);
                    $("#RecordOfficeCountApprove").html(ItemlistA);

                    $("body")
                        .off("click.ecmsReportCards", ".c-dashboardInfo")
                        .on("click.ecmsReportCards", ".c-dashboardInfo", function (event) {
                            event.preventDefault();

                            // alert($(this).closest("div").find(".spnStepId").html())
                            if ($(this).closest("div").find(".IsApproveId").html() == "0" && $(this).closest("div").find(".spnStepId").html() == "3" && $(this).closest("div").find(".applyTypeId").html() == "2") {

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
                                const reportTitle = $(this).find(".c-dashboardInfo__title").first().text().trim();
                                $("#lblRepotReturnHistory").text(reportTitle || "Application History");
                                GetReportReturnHistory($(this).closest("div").find(".spnStepId").html(), $(this).closest("div").find(".spnRecordOfficeId").html(), $(this).closest("div").find(".IsApproveId").html());

                            }
                            else {
                                //  alert($(this).closest("div").find(".spnStepId").html()+'-' + $(this).closest("div").find(".IsApproveId").html());


                                const reportTitle = $(this).find(".c-dashboardInfo__title").first().text().trim();
                                $("#lblRepotReturnHistory").text(reportTitle || "Application History");

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
    $("#RepotReturnHistory")
        .off('shown.bs.modal.ecmsReportHistory')
        .one('shown.bs.modal.ecmsReportHistory', function () {
            if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
                // Destroy the DataTable and clear the table content
                $("#tbldatadialog").DataTable().clear().destroy(); // Clear and destroy DataTable properly
                $("#tbldatadialog thead").empty(); // Clear old thead
                $("#tbldatadialog tbody").empty(); // Clear old tbody
            }
            var userdata =
            {
                "TableId": 0,
                "UnitType": $("input[type='radio'][name=UnitTyperdi]").length > 0 ? parseVal($("input[type='radio'][name=UnitTyperdi]:checked").val()) : null,
                "ComdId": $('#ddlCommand').length > 0 ? parseVal($('#ddlCommand').val()) : null,
                "CorpsId": $('#ddlCorps').length > 0 ? parseVal($('#ddlCorps').val()) : null,
                "DivId": $('#ddlDiv').length > 0 ? parseVal($('#ddlDiv').val()) : null,
                "BdeId": $('#ddlBde').length > 0 ? parseVal($('#ddlBde').val()) : null,
                "FmnBranchID": $('#ddlFmnBranch').length > 0 ? parseVal($('#ddlFmnBranch').val()) : null,
                "PsoId": $('#ddlPSODte').length > 0 ? parseVal($('#ddlPSODte').val()) : null,
                "SubDteId": $('#ddlDgSubDte').length > 0 ? parseVal($('#ddlDgSubDte').val()) : null,
                "UnitMapId": $('#ddlUnit').length > 0 ? parseVal($('#ddlUnit').val()) : null

            };
            tableView = $("#tbldatadialog").DataTable({
                scrollY: 'calc(100vh - 360px)',
                scrollX: true,
                scrollCollapse: false,
                fixedHeader: false,

                processing: true,
                serverSide: true,
                filter: true,
                stateSave: false,

                autoWidth: false,
                responsive: false,
                deferRender: true,
                order: [[1, 'desc']],
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
                            ...userdata
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
                    {
                        title: "S No",
                        data: null,
                        name: "SerialNumber",
                        className: "text-center col-sno",
                        width: "60px",
                        orderable: false, // Disable sorting for this column
                        render: function (data, type, row, meta) {
                            // Calculate serial number based on row index
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    },
                    {
                        title: "Army No",
                        data: "ServiceNo",
                        name: "ServiceNo",
                        className: "nowrap",
                        width: "120px",
                    },
                    {
                        title: "Rank & Name",
                        data: null,
                        name: null,
                        className: "nowrap",
                        width: "180px",
                        orderable: false,
                        render: function (data, type, row) {
                            let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                            if (!fullName) return '';
                            return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                        }
                    },
                    {
                        title: "From",
                        data: null,
                        name: null,
                        className: "nowrap",
                        width: "180px",
                        orderable: false,
                        render: function (data, type, row) {
                            let From = `${row.RankFrom} ${row.NameFrom} (${row.ArmyNoFrom}) (${row.DomainIdFrom})`.trim();
                            if (row.RankFrom != null) {
                                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${From}">${From}</span>`;
                            }
                            else {
                                return '';
                            }
                        }
                    },
                    {
                        title: "Sent To",
                        data: null,
                        name: null,
                        className: "nowrap",
                        width: "180px",
                        orderable: false,
                        render: function (data, type, row) {
                            let SentTo = `${row.RankTo} ${row.NameTo} (${row.ArmyNoTo}) (${row.DomainIdTo})`.trim();
                            if (row.RankTo != null) {
                                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${SentTo}">${SentTo}</span>`;
                            }
                            else {
                                return '';
                            }
                        }
                    },
                    {
                        title: "Appl Id",
                        data: 'RequestId',
                        name: 'RequestId',
                        className: "nowrap",
                        width: "100px",
                    },
                    {
                        title: "Action On",
                        data: "UpdatedOn",
                        name: "UpdatedOn",
                        className: "",
                        width: "150px",
                        render: function (data, type, row) {
                            return data ? DateFormateddMMyyyyhhmmss(data) : "";
                        }
                    },
                    {
                        title: "Status",
                        data: "StatusName",
                        name: "StatusName",
                        className: "",
                        width: "150px",
                        render: function (data, type, row) {
                            let badgeClass = 'bg-primary';
                            if (data == "Pending") {
                                badgeClass = 'bg-warning text-dark';
                            }
                            else if (data == "Rejected") {
                                badgeClass = 'ecms-status-no';
                            }
                            else {
                                badgeClass = 'ecms-status-yes';
                            }
                            return data
                                ? `<span class="badge ${badgeClass}">${row.StatusName}</span>`
                                : '<span class="badge bg-primary">Action Pending</span>';
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: '_all',  // Apply to all visible columns
                        orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
                    },
                ],
                language: {
                    search: "", // Remove the default "Search:" label
                    searchPlaceholder: "Search Army No",
                    emptyTable: "No application history records found"
                },
                dom:
                    "<'dt-top d-flex flex-column flex-md-row align-items-stretch align-items-md-center gap-2'lB<'ms-md-auto'f>>rt" +
                    "<'ecms-dt-footer row g-2'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>",
                buttons: [
                    //{
                    //    extend: 'copy',
                    //    exportOptions: {
                    //        columns: "thead th:not(.noExport)"
                    //    }
                    //},
                    {
                        extend: 'excel',
                        text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Excel',
                        className: 'btn btn-success btn-sm',
                        titleAttr: 'Export application history to Excel',
                        exportOptions: {
                            columns: "thead th:not(.noExport)"
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        text: '<i class="fa fa-file-pdf-o" aria-hidden="true"></i> PDF',
                        className: 'btn btn-danger btn-sm',
                        titleAttr: 'Export application history to PDF',
                        orientation: 'landscape',
                        pageSize: 'LEGAL',
                        title: 'E-IASC_Report',
                        exportOptions: {
                            columns: "thead th:not(.noExport)"
                        },
                        customize: function (doc) {
                            WaterMarkOnPdf(doc)
                        }
                    }],
                // 👇 Show modal only after table (header + data) is fully rendered
                initComplete: function () {
                    // Force DataTables to calculate optimal widths
                    this.api().columns.adjust();

                    // Handle zoom/resize
                    var resizeTimer;
                    $(window)
                        .off('resize.ecmsReportHistory')
                        .on('resize.ecmsReportHistory', function () {
                            clearTimeout(resizeTimer);
                            resizeTimer = setTimeout(function () {
                                if (tableView) {
                                    tableView.columns.adjust();
                                }
                            }, 100);
                        });
                },
                drawCallback: function (settings) {
                    // Recalculate widths on each data load
                    this.api().columns.adjust();

                    const tooltipTriggerList = [].slice.call(
                        document.querySelectorAll('[data-bs-toggle="tooltip"]')
                    );
                    tooltipTriggerList.forEach(el => {
                        new bootstrap.Tooltip(el);
                    });
                }
            });
        });

    // Show once after the handler is registered.
    $("#RepotReturnHistory").modal("show");

}


async function GetLoginUnitMappingDetails() {
    try {
        const response = await fetch('/Master/GetALLByUnitMapWonUnit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
        });

        const result = await response.json();

        if (!result || result === "null") return;
        if (result === InternalServerError) {
            Swal.fire({ text: errormsg });
            return;
        }

        const lst = `<option value=${1}>Please Select</option>`;
        UnitType = result.UnitType;
        comid = result.ComdId;
        corId = result.CorpsId;
        divId = result.DivId;
        bdeId = result.BdeId;
        FmnBranchId = result.FmnBranchID;
        PsoId = result.PsoId;
        SubDteId = result.SubDteId;

        if (parseInt(result.UnitType) === 1) {
            $("#UnitType1").prop("checked", true);

            if ($("#spnclaimId").html() === "Army Level Reports") {
                await mMsater(false, '', "ddlCommand", 1, '');
            } else if ($("#spnclaimId").html() === "Fmn Level Reports") {
                await mMsater(true, result.ComdId, "ddlCommand", 1, '');
                await mMsater(result.CorpsId == 1 ? false : true, result.CorpsId, "ddlCorps", 2, result.ComdId);
                await mMsaterByParent(result.DivId == 1 ? false : true, result.DivId, "ddlDiv", 3, result.ComdId, result.CorpsId, 0, 0);
                await mMsaterByParent(result.BdeId == 1 ? false : true, result.BdeId, "ddlBde", 4, result.ComdId, result.CorpsId, result.DivId, 0);
                await GetUnitByHierarchy(false, "ddlUnit", result.UnitId, result.ComdId, result.CorpsId, result.DivId, result.BdeId, 1, 1, 1);
            } else {
                await mMsater(true, result.ComdId, "ddlCommand", 1, '');
                await mMsater(true, result.CorpsId, "ddlCorps", 2, result.ComdId);
                await mMsaterByParent(true, result.DivId, "ddlDiv", 3, result.ComdId, result.CorpsId, 0, 0);
                await mMsaterByParent(true, result.BdeId, "ddlBde", 4, result.ComdId, result.CorpsId, result.DivId, 0);
                await GetUnitByHierarchy(true, "ddlUnit", result.UnitId, result.ComdId, result.CorpsId, result.DivId, result.BdeId, 1, 1, 1);
            }

            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");
            $("#ddlFmnBranch, #ddlPSODte, #ddlDgSubDte").html(lst);

        } else if (parseInt(result.UnitType) === 2) {
            $("#UnitType2").prop("checked", true);

            if ($("#spnclaimId").html() === "Army Level Reports") {
                await mMsater(false, '', "ddlCommand", 1, '');
                await mMsater(true, result.FmnBranchID, "ddlFmnBranch", FmnBranches, '');
            } else if ($("#spnclaimId").html() === "Fmn Level Reports") {
                await mMsater(true, result.ComdId, "ddlCommand", 1, '');
                await mMsater(result.CorpsId == 1 ? false : true, result.CorpsId, "ddlCorps", 2, result.ComdId);
                await mMsaterByParent(result.DivId == 1 ? false : true, result.DivId, "ddlDiv", 3, result.ComdId, result.CorpsId, 0, 0);
                await mMsaterByParent(result.BdeId == 1 ? false : true, result.BdeId, "ddlBde", 4, result.ComdId, result.CorpsId, result.DivId, 0);
                await mMsater(true, result.FmnBranchID, "ddlFmnBranch", FmnBranches, '');
                await GetUnitByHierarchy(false, "ddlUnit", result.UnitId, result.ComdId, result.CorpsId, result.DivId, result.BdeId, result.FmnBranchID, 1, 1);
            } else {
                await mMsater(true, result.ComdId, "ddlCommand", 1, '');
                await mMsater(true, result.CorpsId, "ddlCorps", 2, result.ComdId);
                await mMsaterByParent(true, result.DivId, "ddlDiv", 3, result.ComdId, result.CorpsId, 0, 0);
                await mMsaterByParent(true, result.BdeId, "ddlBde", 4, result.ComdId, result.CorpsId, result.DivId, 0);
                await mMsater(true, result.FmnBranchID, "ddlFmnBranch", FmnBranches, '');
                await GetUnitByHierarchy(true, "ddlUnit", result.UnitId, result.ComdId, result.CorpsId, result.DivId, result.BdeId, result.FmnBranchID, 1, 1);
            }

            $("#ddlPSODte, #ddlDgSubDte").html(lst);
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
            $(".DteBranch").addClass("d-none");

        } else if (parseInt(result.UnitType) === 3) {
            $("#UnitType3").prop("checked", true);

            if ($("#spnclaimId").html() === "Army Level Reports") {
                await mMsater(false, '', "ddlPSODte", PSO, '');
                await mMsater(false, '', "ddlDgSubDte", SubDte, '');
            } else if ($("#spnclaimId").html() === "Fmn Level Reports") {
                await mMsater(true, result.PsoId, "ddlPSODte", PSO, '');
                await mMsater(true, result.SubDteId, "ddlDgSubDte", SubDte, result.PsoId);
                await GetUnitByHierarchy(false, "ddlUnit", result.UnitId, 1, 1, 1, 1, 1, result.PsoId, result.SubDteId);
            } else {
                await mMsater(true, result.PsoId, "ddlPSODte", PSO, '');
                await mMsater(true, result.SubDteId, "ddlDgSubDte", SubDte, '');
                await GetUnitByHierarchy(true, "ddlUnit", result.UnitId, 1, 1, 1, 1, 1, result.PsoId, result.SubDteId);
            }

            $(".unittype").addClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").removeClass("d-none");

            $("#ddlFmnBranch, #ddlCommand, #ddlCorps, #ddlBde, #ddlDiv").html(lst);
        }
    } catch (error) {
        Swal.fire({ text: errormsg002 });
        console.error("GetLoginUnitMappingDetails error:", error);
    }
}

async function GetUnitByHierarchy(IsOnly, ddl, sectid, ComdId, CorpsId, DivId, BdeId, FmnBranchID, PsoId, SubDteId) {
    try {
        const normalize = (val) => (val === "null" || val === "" || val === undefined ? null : val);

        const payload = {
            TableId: 0,
            UnitType: UnitType,
            ComdId: normalize(ComdId),
            CorpsId: normalize(CorpsId),
            DivId: normalize(DivId),
            BdeId: normalize(BdeId),
            FmnBranchID: normalize(FmnBranchID),
            PsoId: normalize(PsoId),
            SubDteId: normalize(SubDteId)
        };
        let encryptedPayload = "";

        if (payload) {
            const jsonData = JSON.stringify(payload);
            encryptedPayload = encryptPayloadData(jsonData);
        }

        const response = await fetch('/Master/GetUnitByHierarchy', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: JSON.stringify({ data: encryptedPayload })
        });

        const result = await response.json();

        if (!result || result === "null") return;

        if (result === InternalServerError) {
            Swal.fire({ text: errormsg });
            return;
        }

        let listItem = `<option value=${null}>All</option>`;

        for (const item of result) {
            if (IsOnly && item.UnitId == sectid) {
                listItem += `<option value="${item.UnitId}">${item.UnitName}</option>`;
            } else if (!IsOnly) {
                listItem += `<option value="${item.UnitId}">${item.UnitName}</option>`;
            }
        }

        $("#" + ddl).html(listItem);
        if (sectid !== '') {
            $("#" + ddl).val(sectid);
        }
    } catch (error) {
        Swal.fire({ text: errormsg002 });
        console.error("GetUnitByHierarchy error:", error);
    }
}

async function mMsater(IsOnly, sectid = '', ddl, TableId, ParentId) {
    const payload = {
        tableName: "",
        id: TableId,
        parentId: ParentId ? Number(ParentId) : null   // ⭐ THIS IS IMPORTANT
    };
    let encryptedPayload = "";
    if (payload) {
        const jsonData = JSON.stringify(payload);
        encryptedPayload = encryptPayloadData(jsonData);

    }
    try {
        const response = await fetch('/Master/GetAllMMaster', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            credentials: 'include',          // <--- IMPORTANT ensures the browser sends .AspNetCore.Session cookie with the request. when using fetch API
            // body: JSON.stringify(payload)
            body: JSON.stringify({ data: encryptedPayload })
        });

        const data = await response.json();

        if (!data || data === "null") return;
        if (data === InternalServerError) {
            Swal.fire({ text: errormsg });
            return;
        }

        let listItemddl = IsOnly ? '' : `<option value=${null}>All</option>`;

        for (let item of data) {
            if (IsOnly && item.Id == sectid) {
                listItemddl += `<option value="${item.Id}">${item.Name}</option>`;
            } else if (!IsOnly) {
                listItemddl += `<option value="${item.Id}">${item.Name}</option>`;
            }
        }

        $("#" + ddl).html(listItemddl);
        if (sectid !== '') {
            $("#" + ddl).val(sectid);
        }
    } catch (error) {
        Swal.fire({ text: errormsg002 });
        console.error("mMsater error:", error);
    }
}

async function mMsaterByParent(IsOnly, sectid = '', ddl, TableId, ComdId, CorpsId, DivId, BdeId) {
    const payload = {
        TableId: TableId ? Number(TableId) : null,
        ComdId: ComdId ? Number(ComdId) : null,
        CorpsId: CorpsId ? Number(CorpsId) : null,
        DivId: DivId ? Number(DivId) : null,
        BdeId: BdeId ? Number(BdeId) : null
    };
    let encryptedPayload = "";
    if (payload) {
        const jsonData = JSON.stringify(payload);
        encryptedPayload = encryptPayloadData(jsonData);

    }
    try {
        const response = await fetch('/Master/GetAllMMasterByParent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            credentials: 'include',          // <--- IMPORTANT ensures the browser sends .AspNetCore.Session cookie with the request. when using fetch API
            body: JSON.stringify({ data: encryptedPayload })
        });

        const data = await response.json();

        if (!data || data === "null") return;
        if (data === InternalServerError) {
            Swal.fire({ text: errormsg });
            return;
        }

        let listItemddl = IsOnly ? '' : `<option value=${null}>All</option>`;

        for (let item of data) {
            if (IsOnly && item.Id == sectid) {
                listItemddl += `<option value="${item.Id}">${item.Name}</option>`;
            } else if (!IsOnly) {
                listItemddl += `<option value="${item.Id}">${item.Name}</option>`;
            }
        }

        $("#" + ddl).html(listItemddl);
        if (sectid !== '') {
            $("#" + ddl).val(sectid);
        }
    } catch (error) {
        Swal.fire({ text: errormsg002 });
        console.error("mMsaterByParent error:", error);
    }
}