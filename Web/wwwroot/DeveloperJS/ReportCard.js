let lst = `<option value=${null}>All</option>`;
var comid = 0; var corId = 0; var divId = 0; var bdeId = 0; var FmnBranchId = 0; var PsoId = 0; var SubDteId = 0;
var UnitType = 1;
var table; // Declare table variable outside the function to preserve the instance
$(async function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    // Keep page scroll and modal stacking local to this page.
    $("body").addClass("ecms-report-card-page");

    const $cardReportModal = $("#CardReport");

    // A modal inside a transformed/positioned page wrapper can remain below the
    // backdrop even with a large z-index. Detaching it to <body> removes that
    // stacking-context problem without changing any modal ID or functionality.
    if ($cardReportModal.length > 0 && !$cardReportModal.parent().is("body")) {
        $cardReportModal.detach().appendTo(document.body);
    }

    function placeCardReportAboveBackdrop() {
        const $backdrop = $("body > .modal-backdrop").last();

        if ($backdrop.length > 0) {
            // Keep the backdrop immediately before the modal in DOM order and
            // below it in z-index. This also handles legacy CSS with !important.
            $backdrop.insertBefore($cardReportModal);
            $backdrop.css("z-index", 2147482990);
        }

        $cardReportModal.css("z-index", 2147483000);
        $cardReportModal.find(".modal-dialog, .modal-content")
            .css({
                "z-index": 2,
                "opacity": 1,
                "filter": "none",
                "pointer-events": "auto"
            });
    }

    $cardReportModal
        .off(".reportCardUi")
        .on("show.bs.modal.reportCardUi", function () {
            // Remove only stale backdrops when no other modal is active.
            if ($(".modal.show").not(this).length === 0) {
                $("body > .modal-backdrop").remove();
            }

            if (!$(this).parent().is("body")) {
                $(this).detach().appendTo(document.body);
            }

            $("body").addClass("ecms-report-card-modal-open");
        })
        .on("shown.bs.modal.reportCardUi", function () {
            placeCardReportAboveBackdrop();

            // Run once more after Bootstrap completes its transition/backdrop work.
            window.requestAnimationFrame(placeCardReportAboveBackdrop);

            if ($.fn.DataTable.isDataTable("#CardReport_tbldatadialog")) {
                adjustReportCardTable($("#CardReport_tbldatadialog").DataTable());
            }
        })
        .on("hidden.bs.modal.reportCardUi", function () {
            $(window).off("resize.reportCardTable");

            if ($(".modal.show").length === 0) {
                $("body > .modal-backdrop").remove();
                $("body")
                    .removeClass("modal-open ecms-report-card-modal-open")
                    .css("padding-right", "");
            }
        });

    $(window)
        .off("beforeunload.reportCardUi")
        .on("beforeunload.reportCardUi", function () {
            $("body").removeClass("ecms-report-card-page ecms-report-card-modal-open");
        });

    if ($('#spnclaimId').length > 0) {
        if ($('#spnclaimId').html() === 'Army Level Reports' || $('#spnclaimId').html() === 'Fmn Level Reports') {
            await GetLoginUnitMappingDetails();
        }
    }
    await GetReportCardDashboardCount();

    if ($('#ddlUnit').length > 0) {
        let previousValue = $('#ddlUnit').val();
        let calledForSingleOption = false;

        $('#ddlUnit').on('focus', function () {
            previousValue = $(this).val();
            calledForSingleOption = false;
        });

        $('#ddlUnit').on('blur', async function () {
            const currentValue = $(this).val();

            if (currentValue !== previousValue) {
                await GetReportCardDashboardCount();
            } else if ($('#ddlUnit option').length === 1 && !calledForSingleOption) {
                await GetReportCardDashboardCount();
                calledForSingleOption = true;
            }
        });
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

                ResetCount();
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
            ResetCount();
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
            ResetCount();
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
            ResetCount();

            await GetUnitByHierarchy(false, "ddlUnit", 0, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), $('#ddlBde').val(), 1, 1, 1);

        });
    }

    if ($('#ddlFmnBranch').length > 0) {
        $('#ddlFmnBranch').on('change', async function () {
            FmnBranchId = $(this).val();
            ResetCount();
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
            ResetCount();
            await GetUnitByHierarchy(false, "ddlUnit", 0, 1, 1, 1, 1, 1, $("#ddlPSODte").val(), SubDteId);
        });
    }

    $('input[name="UnitTyperdi"]').on("click", async function () {

        UnitType = $("input[type='radio'][name=UnitTyperdi]:checked").val();
        ResetCount();
        if (UnitType == "1") {
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
        else if (UnitType == "2") {

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
        else if (UnitType == "3") {
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


    const ApplyForId_Officer = 1;
    const ApplyForId_OR = 2;

    // One UI-only action map replaces repeated modal-open code.
    // IDs, report choices and ApplyForId values remain unchanged.
    const reportCardActions = [
        { selector: "#btnExport_Officer", title: "Exported I-Card", choice: "Export", applyForId: ApplyForId_Officer },
        { selector: "#btnPrinted_Officer", title: "Printed I-Card", choice: "Printed", applyForId: ApplyForId_Officer },
        { selector: "#btnDispatchToORO", title: "Card Dispatch to Officer Record Office", choice: "DispatchToORO_Regt", applyForId: ApplyForId_Officer },
        { selector: "#btnCardInORO", title: "Card in Officer Record Office", choice: "CardInORO_Regt", applyForId: ApplyForId_Officer },
        { selector: "#btnDispatchToUnit_Officer", title: "Card Dispatch to Unit", choice: "DispatchToUnit", applyForId: ApplyForId_Officer },
        { selector: "#btnCardInUnit_Officer", title: "Card in Unit", choice: "CardInUnit", applyForId: ApplyForId_Officer },
        { selector: "#btnDistributed_Officer", title: "Card Distributed", choice: "CardDistributed", applyForId: ApplyForId_Officer },
        { selector: "#btnExport_OR", title: "Exported I-Card", choice: "Export", applyForId: ApplyForId_OR },
        { selector: "#btnPrinted_OR", title: "Printed I-Card", choice: "Printed", applyForId: ApplyForId_OR },
        { selector: "#btnDispatchToRegt", title: "Card Dispatch to Regiment", choice: "DispatchToORO_Regt", applyForId: ApplyForId_OR },
        { selector: "#btnCardInRegt", title: "Card in Regiment", choice: "CardInORO_Regt", applyForId: ApplyForId_OR },
        { selector: "#btnDispatchToUnit_OR", title: "Card Dispatch to Unit", choice: "DispatchToUnit", applyForId: ApplyForId_OR },
        { selector: "#btnCardInUnit_OR", title: "Card in Unit", choice: "CardInUnit", applyForId: ApplyForId_OR },
        { selector: "#btnDistributed_OR", title: "Card Distributed", choice: "CardDistributed", applyForId: ApplyForId_OR }
    ];

    reportCardActions.forEach(function (action) {
        $(action.selector)
            .off("click.reportCard")
            .on("click.reportCard", function (event) {
                event.preventDefault();
                $("#CardReport_lblModelTitle").text(action.title);
                GetReportReturnHistory(action.choice, action.applyForId);
            });
    });
});
function showCardReportModal($modal) {
    if (!$modal || $modal.length === 0) {
        return;
    }

    if ($.fn.modal) {
        $modal.modal("show");
        return;
    }

    if (window.bootstrap && bootstrap.Modal) {
        bootstrap.Modal.getOrCreateInstance($modal[0]).show();
    }
}

function hideCardReportModal($modal) {
    if (!$modal || $modal.length === 0) {
        return;
    }

    if ($.fn.modal) {
        $modal.modal("hide");
        return;
    }

    if (window.bootstrap && bootstrap.Modal) {
        const instance = bootstrap.Modal.getInstance($modal[0]);
        if (instance) {
            instance.hide();
        }
    }
}

function adjustReportCardTable(api) {
    if (!api) {
        return;
    }

    api.columns.adjust();

    if (api.responsive && typeof api.responsive.recalc === "function") {
        api.responsive.recalc();
    }
}

function GetReportReturnHistory(Choice, ApplyForId) {
    const $modal = $("#CardReport");
    const $reportTable = $("#CardReport_tbldatadialog");

    if ($modal.length === 0 || $reportTable.length === 0) {
        return;
    }

    if (!$modal.parent().is("body")) {
        $modal.appendTo(document.body);
    }

    $modal
        .off("shown.bs.modal.reportCardTable")
        .one("shown.bs.modal.reportCardTable", function () {
            if ($.fn.DataTable.isDataTable("#CardReport_tbldatadialog")) {
                $reportTable.DataTable().clear().destroy();
            }

            $reportTable.empty();

            function parseVal(val) {
                if (val === "null" || val === undefined || val === "") {
                    return null;
                }
                return val;
            }

            const userdata = {
                "Choice": Choice,
                "ApplyForId": ApplyForId,
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

            const columns = getColumnsByChoice(Choice);

            table = $reportTable.DataTable({
                scrollY: '55vh',
                scrollX: true,
                scrollCollapse: false,
                scroller: true,
                deferScroll: true,
                fixedHeader: false,

                processing: true,
                serverSide: true,
                filter: true,
                stateSave: false,

                autoWidth: false,
                responsive: false,
                deferRender: true,
                order: [[1, 'desc']],

                ajax: async function (data, dataTableCallback) {
                    const requestData = {
                        Draw: data.draw,
                        Start: data.start,
                        Length: data.length,
                        SearchValue: data.search.value,
                        SortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',
                        SortDirection: data.order.length > 0 ? data.order[0].dir : '',
                        ...userdata
                    };

                    let encryptedPayload = "";
                    if (requestData) {
                        encryptedPayload = encryptPayloadData(JSON.stringify(requestData));
                    }

                    try {
                        const response = await fetch("/Home/GetReportCardData", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                                'RequestVerificationToken': globalThis.RequestVerificationToken
                            },
                            body: JSON.stringify({ data: encryptedPayload })
                        });

                        if (!response.ok) {
                            hideCardReportModal($modal);
                            const error = await response.json();
                            toastr.error(error.message || `Error ${response.status}`, "Error");
                            throw new Error(error.message || `HTTP error! Status: ${response.status}`);
                        }

                        const result = await response.json();
                        dataTableCallback(result);
                    } catch (error) {
                        console.error("Error fetching data:", error);
                    }
                },

                columns: columns,
                columnDefs: [
                    {
                        targets: '_all',
                        orderSequence: ["asc", "desc"]
                    }
                ],

                language: {
                    search: "",
                    searchPlaceholder: "Search Army No",
                    emptyTable: "No card movement records found"
                },

                dom:
                    "<'dt-top row g-2 align-items-center'" +
                    "<'col-12 col-md-auto'l>" +
                    "<'col-12 col-md-auto'B>" +
                    "<'col-12 col-md ms-md-auto'f>" +
                    ">rt" +
                    "<'ecms-dt-footer row g-2'" +
                    "<'col-12 col-md-6 dt-info-col'i>" +
                    "<'col-12 col-md-6 dt-page-col'p>" +
                    ">",

                buttons: [
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
                        title: 'E-IASC_Report',
                        exportOptions: {
                            columns: "thead th:not(.noExport)"
                        },
                        customize: function (doc) {
                            WaterMarkOnPdf(doc);
                        }
                    }
                ],

                initComplete: function () {
                    const api = this.api();
                    window.setTimeout(function () {
                        adjustReportCardTable(api);
                    }, 0);

                    let resizeTimer;
                    $(window)
                        .off("resize.reportCardTable")
                        .on("resize.reportCardTable", function () {
                            window.clearTimeout(resizeTimer);
                            resizeTimer = window.setTimeout(function () {
                                if ($.fn.DataTable.isDataTable("#CardReport_tbldatadialog")) {
                                    adjustReportCardTable($reportTable.DataTable());
                                }
                            }, 120);
                        });
                },

                drawCallback: function () {
                    adjustReportCardTable(this.api());

                    const tooltipElements = document.querySelectorAll(
                        '[data-bs-toggle="tooltip"], [data-toggle="tooltip"]'
                    );

                    tooltipElements.forEach(function (element) {
                        if (window.bootstrap && bootstrap.Tooltip) {
                            const existingTooltip = bootstrap.Tooltip.getInstance
                                ? bootstrap.Tooltip.getInstance(element)
                                : null;

                            if (!existingTooltip) {
                                new bootstrap.Tooltip(element);
                            }
                        }
                    });

                    $reportTable.find("tbody")
                        .off("click", ".cls-historyRequest")
                        .on("click", ".cls-historyRequest", function () {
                            const rowData = table.row($(this).closest("tr")).data();
                            if (rowData != null) {
                                GetRequestHistory(rowData.RequestId);
                            }
                        });

                    $reportTable.find("tbody")
                        .off("click", ".cls-cardhistoryRequest")
                        .on("click", ".cls-cardhistoryRequest", function () {
                            const rowData = table.row($(this).closest("tr")).data();
                            if (rowData != null) {
                                GetMovementHistory(rowData.RequestId);
                            }
                        });
                }
            });
        });

    // Show exactly once. The table is initialized after the modal is visible,
    // so DataTables can calculate the real column widths correctly.
    showCardReportModal($modal);
}

function getColumnsByChoice(choice) {
    let columns = [];

    switch (choice) {
        case 'Export':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false,
                    className: "text-center col-sno",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    orderable: false,
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    title: "Exported On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },

            ];
            break;

        case 'Printed':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false,
                    className: "text-center col-sno",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    title: "Printed On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
            ];
            break;

        case 'DispatchToORO_Regt':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false,
                    className: "text-center col-sno",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    orderable: false,
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let From = `${`${rowData.FromDID} (${rowData.FromRankName} ${rowData.FromName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.FromServiceNo) ? `${rowData.FromServiceNo.slice(0, 2)}  ${rowData.FromServiceNo.slice(2)}` : rowData.FromServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${From}">${From}</span>`;
                    }
                },
                {
                    title: "Sent To",
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let To = `${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo) ? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${To}">${To}</span>`;
                    }
                },
                {
                    title: "Dispatch On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
            ];
            break;

        case 'CardInORO_Regt':
            columns = [
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
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    orderable: false,
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let From = `${`${rowData.FromDID} (${rowData.FromRankName} ${rowData.FromName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.FromServiceNo) ? `${rowData.FromServiceNo.slice(0, 2)}  ${rowData.FromServiceNo.slice(2)}` : rowData.FromServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${From}">${From}</span>`;
                    }
                },
                {
                    title: "Sent To",
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let To = `${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo) ? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${To}">${To}</span>`;
                    }
                },
                {
                    title: "Received On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
            ];
            break;

        case 'DispatchToUnit':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false,
                    className: "text-center col-sno",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let From = `${`${rowData.FromDID} (${rowData.FromRankName} ${rowData.FromName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.FromServiceNo) ? `${rowData.FromServiceNo.slice(0, 2)}  ${rowData.FromServiceNo.slice(2)}` : rowData.FromServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${From}">${From}</span>`;
                    }
                },
                {
                    title: "Sent To",
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let To = `${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo) ? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${To}">${To}</span>`;
                    }
                },
                {
                    title: "Dispatch On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
            ];
            break;

        case 'CardInUnit':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    className: "text-center col-sno",
                    width: "60px",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let From = `${`${rowData.FromDID} (${rowData.FromRankName} ${rowData.FromName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.FromServiceNo) ? `${rowData.FromServiceNo.slice(0, 2)}  ${rowData.FromServiceNo.slice(2)}` : rowData.FromServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${From}">${From}</span>`;
                    }
                },
                {
                    title: "Sent To",
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let To = `${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo) ? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${To}">${To}</To>`;
                    }
                },
                {
                    title: "Received On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "120px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
            ];
            break;

        case 'CardDistributed':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    className: "text-center col-sno",
                    width: "60px",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl ID",
                    data: 'RequestId',
                    name: 'RequestId',
                    className: "nowrap",
                    width: "100px",
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
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
                    title: "Distributed By",
                    data: "null",
                    name: "null",
                    className: "nowrap",
                    width: "180px",
                    orderable: false,
                    render: function (data, type, rowData) {
                        let To = `${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo) ? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo}`;
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${To}">${To}</To>`;
                    }
                },
                {
                    title: "Distributed On",
                    data: "ActionOn",
                    name: "ActionOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Application History",
                    data: null,
                    name: "Application History",
                    className: "noExport",
                    width: "110px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Card History",
                    data: null,
                    name: "Card History",
                    className: "noExport",
                    width: "110px",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-cardhistoryRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                },
            ];
            break;

        default:
            columns = [
                { title: "S No", data: null, orderable: false, render: (data, type, row, meta) => meta.row + meta.settings._iDisplayStart + 1 },
                { title: "ID", data: 'Id' },
                { title: "Description", data: 'Description' }
            ];
    }

    return columns;
}

async function GetReportCardDashboardCount() {
    try {
        function parseVal(val) {
            if (val === "null" || val === undefined || val === "") {
                return null;
            }
            return val;
        }
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
        let encryptedPayload = "";
        if (requestData) {
            const jsonData = JSON.stringify(requestData);
            encryptedPayload = encryptPayloadData(jsonData);

        }

        const response = await fetch('/Home/GetReportCardDashboardCount', {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: JSON.stringify({ data: encryptedPayload })
        });

        const data = await response.json();

        if (!data || data === "null") return;

        if (data === InternalServerError) {
            Swal.fire({ text: errormsg });
            return;
        }

        if (data === 0) {
            // Optionally handle zero count case
            return;
        }

        $("#TotExported_Officer").html(data.TotExported_Officer);
        $("#TotPrinted_Officer").html(data.TotPrinted_Officer);
        $("#TotDispatchToORO").html(data.TotDispatchToORO);
        $("#TotCardInORO").html(data.TotCardInORO);
        $("#TotDispatchToUnit_Officer").html(data.TotDispatchToUnit_Officer);
        $("#TotCardInUnit_Officer").html(data.TotCardInUnit_Officer);
        $("#TotDistributed_Officer").html(data.TotDistributed_Officer);

        $("#TotExported_OR").html(data.TotExported_OR);
        $("#TotPrinted_OR").html(data.TotPrinted_OR);
        $("#TotDispatchToRegt").html(data.TotDispatchToRegt);
        $("#TotCardInRegt").html(data.TotCardInRegt);
        $("#TotDispatchToUnit_OR").html(data.TotDispatchToUnit_OR);
        $("#TotCardInUnit_OR").html(data.TotCardInUnit_OR);
        $("#TotDistributed_OR").html(data.TotDistributed_OR);

        $('.counter-value').each(function () {
            $(this).prop('Counter', 0).animate({
                Counter: $(this).text()
            }, {
                duration: 200,
                easing: 'swing',
                step: function (now) {
                    $(this).text(Math.ceil(now));
                }
            });
        });
    } catch (error) {
        Swal.fire({ text: errormsg002 });
        console.error("GetReportDashboardCount error:", error);
    }
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

        const userdata = {
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
        if (userdata) {
            const jsonData = JSON.stringify(userdata);
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
    let encryptedPayload = "";
    const payload = {
        tableName: "",
        id: TableId,
        parentId: ParentId ? Number(ParentId) : null   // ⭐ THIS IS IMPORTANT
    };
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
function ResetCount() {
    $("#TotExported_Officer").html('0');
    $("#TotPrinted_Officer").html('0');
    $("#TotDispatchToORO").html('0');
    $("#TotCardInORO").html('0');
    $("#TotDispatchToUnit_Officer").html('0');
    $("#TotCardInUnit_Officer").html('0');
    $("#TotDistributed_Officer").html('0');

    $("#TotExported_OR").html('0');
    $("#TotPrinted_OR").html('0');
    $("#TotDispatchToRegt").html('0');
    $("#TotCardInRegt").html('0');
    $("#TotDispatchToUnit_OR").html('0');
    $("#TotCardInUnit_OR").html('0');
    $("#TotDistributed_OR").html('0');
}