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
    if ($("#btnToggleSearchCard").length > 0) {

        $("#btnToggleSearchCard")
            .off("click.ecmsFilterToggle")
            .on("click.ecmsFilterToggle", function () {

                const $button = $(this);
                const $searchCard = $("#searchUnitCard");
                const isVisible = $searchCard.is(":visible");

                if (isVisible) {

                    $searchCard.stop(true, true).slideUp(200);

                    $button.attr("aria-expanded", "false");

                    $button
                        .find(".ecms-filter-toggle-text")
                        .text("Show Unit Search");

                    $button
                        .find(".ecms-filter-toggle-icon")
                        .removeClass("fa-chevron-up")
                        .addClass("fa-chevron-down");
                }
                else {

                    $searchCard.stop(true, true).slideDown(200);

                    $button.attr("aria-expanded", "true");

                    $button
                        .find(".ecms-filter-toggle-text")
                        .text("Hide Unit Search");

                    $button
                        .find(".ecms-filter-toggle-icon")
                        .removeClass("fa-chevron-down")
                        .addClass("fa-chevron-up");
                }
            });
    }

});
function parseVal(val) {
    if (val === "null" || val === undefined || val === "") {
        return null;
    }
    return val;
}
function buildReportCard(
    title,
    total,
    applyTypeId,
    stepId,
    isApproveId,
    recordOfficeId = null,
    subTitle = ""
) {
    const hiddenFields = [
        `<span class="d-none applyTypeId">${applyTypeId}</span>`,
        `<span class="d-none spnStepId">${stepId}</span>`
    ];

    if (isApproveId !== null && isApproveId !== undefined) {
        hiddenFields.push(
            `<span class="d-none IsApproveId">${isApproveId}</span>`
        );
    }

    if (recordOfficeId !== null && recordOfficeId !== undefined) {
        hiddenFields.push(
            `<span class="d-none spnRecordOfficeId">${recordOfficeId}</span>`
        );
    }

    const subTitleHtml = subTitle
        ? `<h4 class="c-dashboardInfo__title_OROName">${subTitle}</h4>`
        : "";

    return `
        <div class="c-dashboardInfo">
            <a href="#">
                ${hiddenFields.join("")}

                <div class="wrap ecms-tile">
                    <h4 class="c-dashboardInfo__title">
                        ${title ?? ""}
                    </h4>

                    ${subTitleHtml}

                    <span class="c-dashboardInfo__count count">
                        ${total ?? 0}
                    </span>

                    <span class="c-dashboardInfo__subInfo"></span>
                </div>
            </a>
        </div>
    `;
}

function buildReportSections(
    reportData,
    applyTypeId,
    recordOff = [],
    recordoffCount = []
) {
    const initiatorCards = [];
    const approverCards = [];
    const verifierCards = [];
    const afterVerifierCards = [];

    let currentSection = initiatorCards;
    let groupId = 0;

    for (let i = 0; i < reportData.length; i++) {
        const item = reportData[i];

        if (item.TypeId != groupId) {

            // Initiator -> application forwarded to Approver.
            if (item.TypeId == 2) {
                const total =
                    Number(item.Total ?? 0) +
                    Number(reportData[i + 1]?.Total ?? 0);

                initiatorCards.push(
                    buildReportCard(
                        "Appl fwd to Approver",
                        total,
                        applyTypeId,
                        item.StepId,
                        1
                    )
                );

                currentSection = approverCards;
            }

            // Approver -> approved application.
            else if (item.TypeId == 3) {
                const total =
                    Number(item.Total ?? 0) +
                    Number(reportData[i + 1]?.Total ?? 0);

                approverCards.push(
                    buildReportCard(
                        "Approved Appl (Approver Level)",
                        total,
                        applyTypeId,
                        item.StepId,
                        1
                    )
                );

                currentSection = verifierCards;
            }

            // Verifier -> Record Office status + forward to ADC.
            else if (item.TypeId == 4) {

                // Original code shows these Record Office cards only for Officers.
                if (applyTypeId == 1) {
                    for (const recordOffice of recordOff) {
                        const officeCounts = recordoffCount.filter(
                            count => count.RecordOfficeId == recordOffice.RecordOfficeId
                        );

                        const approved =
                            officeCounts.find(count => count.Name == "Approved")?.Total ?? 0;

                        const rejected =
                            officeCounts.find(count => count.Name == "Rejected")?.Total ?? 0;

                        const pending =
                            officeCounts.find(count => count.Name == "Pending")?.Total ?? 0;

                        verifierCards.push(
                            buildReportCard(
                                "Approved / Rejected / Pending",
                                `${approved}/${rejected}/${pending}`,
                                1,
                                99,
                                null,
                                recordOffice.RecordOfficeId,
                                recordOffice.Name
                            )
                        );
                    }
                }

                const total =
                    Number(item.Total ?? 0) +
                    Number(reportData[i + 1]?.Total ?? 0);

                verifierCards.push(
                    buildReportCard(
                        "Appl Verified & Fwd to ADC",
                        total,
                        applyTypeId,
                        item.StepId,
                        1
                    )
                );

                // TypeId 4 normal status cards remain below Verifier,
                // matching the existing page flow.
                currentSection = afterVerifierCards;
            }
        }

        // Drafted / Pending / Rejected and other normal cards.
        if (item.IsApprove == 0) {
            currentSection.push(
                buildReportCard(
                    item.Name,
                    item.Total,
                    applyTypeId,
                    item.StepId,
                    0
                )
            );
        }

        groupId = item.TypeId;
    }

    const sections = [
        { title: "Initiator", cards: initiatorCards },
        { title: "Approver", cards: approverCards },
        { title: "Verifier", cards: verifierCards }
    ]
        .filter(section => section.cards.length > 0)
        .map(section => `
            <div class="ecms-report-level">
                <div class="ecms-report-level-title">
                    ${section.title}
                </div>

                <div class="ecms-card-row">
                    ${section.cards.join("")}
                </div>
            </div>
        `);

    if (afterVerifierCards.length > 0) {
        sections.push(`
            <div class="ecms-report-level ecms-report-level-last">
                <div class="ecms-card-row">
                    ${afterVerifierCards.join("")}
                </div>
            </div>
        `);
    }

    return sections.join("");
}

function GetCount() {
    const requestData = {
        "TableId": 0,
        "UnitType": $("input[type='radio'][name=UnitTyperdi]").length > 0
            ? parseVal($("input[type='radio'][name=UnitTyperdi]:checked").val())
            : null,
        "ComdId": $('#ddlCommand').length > 0
            ? parseVal($('#ddlCommand').val())
            : null,
        "CorpsId": $('#ddlCorps').length > 0
            ? parseVal($('#ddlCorps').val())
            : null,
        "DivId": $('#ddlDiv').length > 0
            ? parseVal($('#ddlDiv').val())
            : null,
        "BdeId": $('#ddlBde').length > 0
            ? parseVal($('#ddlBde').val())
            : null,
        "FmnBranchID": $('#ddlFmnBranch').length > 0
            ? parseVal($('#ddlFmnBranch').val())
            : null,
        "PsoId": $('#ddlPSODte').length > 0
            ? parseVal($('#ddlPSODte').val())
            : null,
        "SubDteId": $('#ddlDgSubDte').length > 0
            ? parseVal($('#ddlDgSubDte').val())
            : null,
        "UnitMapId": $('#ddlUnit').length > 0
            ? parseVal($('#ddlUnit').val())
            : null
    };

    const encrypted = encryptPayloadData(
        JSON.stringify(requestData)
    );

    $.ajax({
        url: '/Home/GetReportReturnCount',
        contentType: 'application/x-www-form-urlencoded',
        data: { "request": encrypted },
        type: 'POST',
        headers: {
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },

        success: function (response) {
            if (response == null || response == "null") {
                return;
            }

            if (response == InternalServerError) {
                Swal.fire({
                    text: errormsg
                });
                return;
            }

            const officerData =
                response.dTOReportReturnCountOffs || [];

            const jcoData =
                response.dTOReportReturnCountJco || [];

            const recordOff =
                response.RecordOff || [];

            const recordoffCount =
                response.RecordoffCount || [];

            const officerHtml = buildReportSections(
                officerData,
                1,
                recordOff,
                recordoffCount
            );

            const jcoHtml = buildReportSections(
                jcoData,
                2
            );

            const Itemlist = `
                <div class="ecms-personnel-card">
                    <div class="ecms-personnel-card-title">
                        Officers
                    </div>

                    ${officerHtml}
                </div>

                <div class="ecms-personnel-card">
                    <div class="ecms-personnel-card-title">
                        JCOs / OR
                    </div>

                    ${jcoHtml}
                </div>
            `;

            const recordJcoPending =
                response.RecordJcoPending || [];

            const recordJco =
                response.RecordJco || [];

            const recordPendingCards = recordJco.map(function (recordOffice) {
                const pendingRecord = recordJcoPending.find(
                    item => item.RecordOfficeId == recordOffice.RecordOfficeId
                );

                return buildReportCard(
                    recordOffice.Name,
                    pendingRecord?.Total ?? 0,
                    recordOffice.RecordOfficeId,
                    pendingRecord ? 100 : 0,
                    null
                );
            });

            const ItemlistR = `
                <div class="ecms-personnel-card">
                    <div class="ecms-personnel-card-title">
                        Record Office Pending
                    </div>

                    <div class="ecms-report-level ecms-report-level-last">
                        <div class="ecms-card-row">
                            ${recordPendingCards.join("")}
                        </div>
                    </div>
                </div>
            `;

            const ItemlistA = "";

            $("#countlistreport").html(Itemlist);
            $("#RecordOfficeCountPendding").html(ItemlistR);
            $("#RecordOfficeCountApprove").html(ItemlistA);

            // Delegated event is required because report cards are generated dynamically.
            $("body")
                .off("click.ecmsReportCards", ".c-dashboardInfo")
                .on("click.ecmsReportCards", ".c-dashboardInfo", function (event) {
                    event.preventDefault();

                    const $card = $(this);

                    const stepId =
                        $card.find(".spnStepId").first().text().trim();

                    const applyTypeId =
                        $card.find(".applyTypeId").first().text().trim();

                    const isApproveElement =
                        $card.find(".IsApproveId").first();

                    const isApproveId = isApproveElement.length > 0
                        ? isApproveElement.text().trim()
                        : undefined;

                    const recordOfficeElement =
                        $card.find(".spnRecordOfficeId").first();

                    const recordOfficeId = recordOfficeElement.length > 0
                        ? recordOfficeElement.text().trim()
                        : undefined;

                    // JCO/OR - Pending at Verifier Level:
                    // show Record Office Pending cards.
                    if (
                        isApproveId == "0" &&
                        stepId == "3" &&
                        applyTypeId == "2"
                    ) {
                        $("#RecordOfficeCountPendding").removeClass("d-none");
                        $(".RecordCount").addClass("d-none");
                        return;
                    }

                    const reportTitle = $card
                        .find(".c-dashboardInfo__title")
                        .first()
                        .text()
                        .trim();

                    $("#lblRepotReturnHistory")
                        .text(reportTitle || "Application History");

                    // Record Office Approved / Rejected / Pending.
                    if (stepId == "99") {
                        GetReportReturnHistory(
                            stepId,
                            recordOfficeId,
                            isApproveId
                        );
                        return;
                    }

                    // Normal report card.
                    GetReportReturnHistory(
                        stepId,
                        applyTypeId,
                        isApproveId
                    );
                });
        },

        error: function () {
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
                    let encryptedPayload = "";
                    if (requestData) {
                        const jsonData = JSON.stringify(requestData);
                        encryptedPayload = encryptPayloadData(jsonData);

                    }
                    try {
                        let response = await fetch("/Home/GetRecordHistory", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                                'RequestVerificationToken': globalThis.RequestVerificationToken
                            },
                            body: JSON.stringify({ data: encryptedPayload })
                        });
                        if (!response.ok) throw new Error(`HTTP error! Status: ${response.Message}`);

                        let result = await response.json();

                        if (result.Result == false) {
                            toastr.error("Failed to Fetch Date: " + response.Message);
                        }

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

        const unitOptions = IsOnly
            ? result
                .filter(item => item.UnitId == sectid)
                .map(item => `<option value="${item.UnitId}">${item.UnitName}</option>`)
            : [
                `<option value=${null}>All</option>`,
                ...result.map(
                    item => `<option value="${item.UnitId}">${item.UnitName}</option>`
                )
            ];

        $("#" + ddl).html(unitOptions.join(""));
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

        const masterOptions = IsOnly
            ? data
                .filter(item => item.Id == sectid)
                .map(item => `<option value="${item.Id}">${item.Name}</option>`)
            : [
                `<option value=${null}>All</option>`,
                ...data.map(
                    item => `<option value="${item.Id}">${item.Name}</option>`
                )
            ];

        $("#" + ddl).html(masterOptions.join(""));
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

        const masterOptions = IsOnly
            ? data
                .filter(item => item.Id == sectid)
                .map(item => `<option value="${item.Id}">${item.Name}</option>`)
            : [
                `<option value=${null}>All</option>`,
                ...data.map(
                    item => `<option value="${item.Id}">${item.Name}</option>`
                )
            ];

        $("#" + ddl).html(masterOptions.join(""));
        if (sectid !== '') {
            $("#" + ddl).val(sectid);
        }
    } catch (error) {
        Swal.fire({ text: errormsg002 });
        console.error("mMsaterByParent error:", error);
    }
}