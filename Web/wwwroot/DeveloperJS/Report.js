let lst = `<option value=${null}>All</option>`;
var comid = 0; var corId = 0; var divId = 0; var bdeId = 0; var FmnBranchId = 0; var PsoId = 0; var SubDteId = 0;
var UnitType = 1;
var table; // Declare table variable outside the function to preserve the instance
$(async function () {

    if ($('#spnclaimId').length > 0) {
        if ($('#spnclaimId').html() === 'Army Level Reports' || $('#spnclaimId').html() === 'Fmn Level Reports') {
            await GetLoginUnitMappingDetails();
        }
    }
    await GetReportDashboardCount();

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
                await GetReportDashboardCount();
            } else if ($('#ddlUnit option').length === 1 && !calledForSingleOption) {
                await GetReportDashboardCount();
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
            await GetUnitByHierarchy(false,"ddlUnit", 0, $("#ddlCommand").val(), $("#ddlCorps").val(), 1, 1, $("#ddlFmnBranch").val(), 1, 1);

        });
    }
    if ($('#ddlDgSubDte').length > 0) {
        $('#ddlDgSubDte').on('change', async function () {
            SubDteId = $(this).val();
            await GetUnitByHierarchy(false,"ddlUnit", 0, 1, 1, 1, 1, 1, PsoId, $("#ddlDgSubDte").val());
        });
    }
    if ($('#ddlPSODte').length > 0) {
        $('#ddlPSODte').on('change', async function () {
            PsoId = $(this).val();
            ResetCount();
            await GetUnitByHierarchy(false,"ddlUnit", 0, 1, 1, 1, 1, 1, $("#ddlPSODte").val(), SubDteId);
        });
    }
    $('input[name="UnitTyperdi"]').on("click",async function () {

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


        $('#txtMonthYear').datepicker({
            dateFormat: 'mm/yy',
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            yearRange: function () {
                const today = new Date();
                return 2025 + ":" + today.getFullYear();
            }(),

            beforeShow: function (input, inst) {
                setTimeout(function () {
                    const today = new Date();
                    const currentYear = today.getFullYear();
                    const currentMonth = today.getMonth();

                    const minYear = 2025;
                    const minMonth = 2; // March (0-based)

                    function restrictUI() {
                        $(".ui-datepicker-calendar").hide();

                        const selectedYear = parseInt($(".ui-datepicker-year").val(), 10);
                        const selectedMonth = parseInt($(".ui-datepicker-month").val(), 10);

                        // Disable future months & past months
                        $(".ui-datepicker-month option").each(function (index) {
                            if ((selectedYear === minYear && index < minMonth) || (selectedYear === currentYear && index > currentMonth)) {
                                $(this).attr("disabled", "disabled");
                            } else {
                                $(this).removeAttr("disabled");
                            }
                        });

                        // Disable "Next" button if at max month/year
                        const isMaxMonth = selectedYear === currentYear && selectedMonth >= currentMonth;
                        if (isMaxMonth) {
                            $(".ui-datepicker-next").addClass("ui-state-disabled").hide(); // disable + hide
                        } else {
                            $(".ui-datepicker-next").removeClass("ui-state-disabled").show(); // enable + show
                        }

                        // Disable "Prev" if at minimum allowed month/year
                        const isMinMonth = (selectedYear === minYear && selectedMonth <= minMonth) || (selectedYear < minYear);

                        if (isMinMonth) {
                            $(".ui-datepicker-prev").addClass("ui-state-disabled").hide();
                        } else {
                            $(".ui-datepicker-prev").removeClass("ui-state-disabled").show();
                        }
                    }

                    function observeCalendarChanges() {
                        const observer = new MutationObserver(restrictUI);
                        const dpDiv = document.querySelector("#ui-datepicker-div");
                        if (dpDiv) {
                            observer.observe(dpDiv, { childList: true, subtree: true });
                        }
                    }

                    restrictUI();
                    observeCalendarChanges();
                }, 0);
            },

            onClose: function (dateText, inst) {
                const month = parseInt($("#ui-datepicker-div .ui-datepicker-month :selected").val(), 10);
                const year = parseInt($("#ui-datepicker-div .ui-datepicker-year :selected").val(), 10);

                const minYear = 2025;
                const minMonth = 2; // July (because March is 2, so March is 3)

                if (year < minYear || (year === minYear && month < minMonth)) {
                    alert("Please select March 2025 or later.");  // or show error message your way
                    $(this).val('');  // clear the input
                    return;
                }

                // valid selection
                const formattedMonth = ("0" + (month + 1)).slice(-2);
                $(this).val(formattedMonth + "/" + year); // enforce MM/YYYY format
            }
        });

    $("#txtMonthYear").attr("readonly", true).on("keydown paste input", function (e) {
        e.preventDefault();
    });

    const $dialog = $("#CardReport .modal-dialog");

    $("#btnRequisition").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('New Requisition');

        // If modal-xl class is present, override its width
        if ($dialog.hasClass("modal-xl")) {
            $dialog.css("width", "75%");
        }

        GetReportReturnHistory('Requisition', function () {
            $("#CardReport").modal("show"); // shown only after data is fully ready
        });
    });
    $("#btnNonFunctionalCard").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('Non Functional Card');

         // If modal-xl class is present, override its width
        if ($dialog.hasClass("modal-xl")) {
            $dialog.css("width", "100%");
        }

        GetReportReturnHistory('NonFunctional', function () {
            $("#CardReport").modal("show"); // shown only after data is fully ready
        });
    });
    $("#btnLostCase").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('Lost Case');

        // If modal-xl class is present, override its width
        if ($dialog.hasClass("modal-xl")) {
            $dialog.css("width", "100%");
        }

        GetReportReturnHistory('LostCase', function () {
            $("#CardReport").modal("show"); // shown only after data is fully ready
        });
    });
    $("#btnMonthlyProcessed").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#MonthlyProcessedDialog").modal("show");
    });
    $("#btnMonthlyProcessedDialogOk").on("click", function (event) {
        event.preventDefault();

        // ALWAYS use a secure and specific selector
        const $input = $("#txtMonthYear");
        const inputVal = $input.val().trim();

        // Check if input is non-empty and exactly 7 characters long (MM/YYYY)
        if (!inputVal || inputVal.length !== 7) {
            toastr.error("Please select Month and Year in MM/YYYY format.");
            $input.focus();
            return;
        }

        // Validate MM/YYYY with 4-digit year only
        const isValidFormat = /^(0[1-9]|1[0-2])\/(19|20)\d{2}$/.test(inputVal);
        if (!isValidFormat) {
            toastr.error("Invalid format. Use MM/YYYY only.");
            $input.focus();
            return;
        }

        const [monthStr, yearStr] = inputVal.split("/");
        const inputMonth = parseInt(monthStr, 10);
        const inputYear = parseInt(yearStr, 10);

        const today = new Date();
        const currentMonth = today.getMonth() + 1;
        const currentYear = today.getFullYear();
        const minYear = currentYear - 2;

        // Validate against date range
        if (
            inputYear < minYear ||
            inputYear > currentYear ||
            (inputYear === currentYear && inputMonth > currentMonth)
        ) {
            toastr.error(`Month/Year should be within last 2 years and not beyond ${currentMonth < 10 ? '0' + currentMonth : currentMonth}/${currentYear}.`);
            $input.focus();
            return;
        }
        const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        const monthName = monthNames[inputMonth - 1];

        // ✅ All good: Proceed
        $("#MonthlyProcessedDialog").modal("hide");
        $("#CardReport_lblModelTitle").html(`Monthly Processed :- ${monthName}  ${inputYear}`);
        //$("#CardReport").modal("show");

        //$('#CardReport').one('shown.bs.modal', function () {
        //    GetReportReturnHistory('MonthlyProcessed');
        //});

        // If modal-xl class is present, override its width
        if ($dialog.hasClass("modal-xl")) {
            $dialog.css("width", "100%");
        }

        GetReportReturnHistory('MonthlyProcessed', function () {
            $("#CardReport").modal("show"); // shown only after data is fully ready
        });
    });

});
function GetReportReturnHistory(Choice, callback) {
    if ($.fn.DataTable.isDataTable("#CardReport_tbldatadialog")) {
        $("#CardReport_tbldatadialog").DataTable().destroy();
        $("#CardReport_tbldatadialog").empty(); // Clear old thead/tbody
    }
    function parseVal(val) {
        if (val === "null" || val === undefined || val === "") {
            return null;
        }
        return val;
    }
    var userdata =
    {
        "Choice": Choice,
        "TableId": 0,
        "UnitType": $("input[type='radio'][name=UnitTyperdi]").length > 0 ? parseVal($("input[type='radio'][name=UnitTyperdi]:checked").val()) : null,
        "ComdId": $('#ddlCommand').length > 0 ? parseVal($('#ddlCommand').val()) : null,
        "CorpsId": $('#ddlCorps').length > 0 ? parseVal($('#ddlCorps').val()) : null,
        "DivId": $('#ddlDiv').length > 0 ? parseVal($('#ddlDiv').val()) : null,
        "BdeId": $('#ddlBde').length > 0 ? parseVal($('#ddlBde').val()) : null,
        "FmnBranchID": $('#ddlFmnBranch').length > 0 ? parseVal($('#ddlFmnBranch').val()) : null,
        "PsoId": $('#ddlPSODte').length > 0 ? parseVal($('#ddlPSODte').val()) : null,
        "SubDteId": $('#ddlDgSubDte').length > 0 ? parseVal($('#ddlDgSubDte').val()) : null,
        "UnitMapId": $('#ddlUnit').length > 0 ? parseVal($('#ddlUnit').val()) : null,
        "MonthYear": $('#txtMonthYear').length > 0 ? $('#txtMonthYear').val() : null,
        
    };

    const columns = getColumnsByChoice(Choice);
    table = $("#CardReport_tbldatadialog").DataTable({
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
        processing: true,
        serverSide: true,
        filter: true,
        order: [[1, 'desc']], // Default sorting on the first column

        ajax: async function (data, callback, settings) {
            
            let requestData = {
                Draw: data.draw,
                Start: data.start,
                Length: data.length,
                SearchValue: data.search.value,
                SortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                SortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                ...userdata
            };
            try {
                let response = await fetch("/Home/GetReportData", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(requestData)
                });
                if (!response.ok) {
                    $("#CardReport").modal("hide");
                    const error = await response.json();
                    toastr.error(error.message || `Error ${response.status}`, "Error");
                    throw new Error(error.message || `HTTP error! Status: ${response.status}`);
                }
                    

                let result = await response.json();
                //$("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
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
            if (typeof callback === "function") {
                callback(); // show modal now
            }
        },
        drawCallback: function (settings) {


            $("#CardReport_tbldatadialog tbody").off("click", ".cls-remarks").on("click", ".cls-remarks", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RemarksNameList != null) {
                    var remarksArray = rowData.RemarksNameList.split('#');
                    if (remarksArray != null) {
                        var listItem = "";
                        listItem += "<ul>";
                        for (var j = 0; j < remarksArray.length; j++) {
                            listItem += "<li>" + remarksArray[j] + "</li>";
                        }
                        listItem += "</ul>";
                        $("#MessageDialogLabel").html('Reason');
                        $("#MessageDialogBody").html(listItem);
                        $("#MessageDialog").modal('show');
                    }
                }
            });


            $("#CardReport_tbldatadialog tbody").off("click", ".cls-Faulty-FromRemark").on("click", ".cls-Faulty-FromRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('Remark');
                    $("#MessageDialogBody").html(rowData.FromRemark);
                    $("#MessageDialog").modal('show');
                }
            });

            $("#CardReport_tbldatadialog tbody").off("click", ".cls-Faulty-ToRemark").on("click", ".cls-Faulty-ToRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('AFSAC Remark');
                    $("#MessageDialogBody").html(rowData.ToRemark);
                    $("#MessageDialog").modal('show');
                }
            });
            $("#CardReport_tbldatadialog tbody").off("click", ".cls-Lost-FromRemark").on("click", ".cls-Lost-FromRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('Remark');
                    $("#MessageDialogBody").text(rowData.FromRemark);
                    $("#MessageDialog").modal('show');
                }
            });

            $("#CardReport_tbldatadialog tbody").off("click", ".cls-Lost-uploadedDoc").on("click", ".cls-Lost-uploadedDoc", function () {
                var rowData = table.row($(this).closest("tr")).data();
                const baseUrl = window.location.origin;
                const downloadUrl = `${baseUrl}/LostCardSupportingDoc/${encodeURIComponent(rowData.SupportDocName)}`;
                const link = document.createElement('a');
                link.href = downloadUrl;
                link.download = "LostCard_SupportiveDocument.pdf";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                //window.location.href = downloadUrl;
            });

        }
    });
}
function getColumnsByChoice(choice) {
    let columns = [];

    switch (choice) {
        case 'Requisition':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Request ID",
                    data: 'RequestId',
                    name: 'RequestId',
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
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
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Status",
                    data: "StepId",
                    name: "StepId",
                    render: function (data, type, row) {
                        let color;
                        if (data == 1 || data == 2 || data == 3 || data == 4 || data == 5 || data == 6 || data == 11 || data == 12 || data == 13 || data == 14) {
                            color = 'warning';
                        }
                        else if (data == 7 || data == 8 || data == 9 || data == 10) {
                            color = 'danger';
                        }
                        else {
                            color = 'success';
                        }
                        return `<span class='badge badge-${color} mr-1' >${row.Status}</span></span>`;
                    }
                }
            ];
            break;

        case 'NonFunctional':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Request ID",
                    data: 'RequestId',
                    name: 'RequestId',
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
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
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Unit",
                    data: "UnitAbbreviation",
                    name: "UnitAbbreviation",
                    orderable: false,
                },
                {
                    title: "Date & Time",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Reason",
                    data: "RemarksNameList",
                    name: "RemarksNameList",
                    orderable: false,
                    render: function (data, type, row) {
                        if (data != null) {
                            return `<button type='button' class='cls-remarks btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i></button>`;
                        }
                        else {
                            return `NA`;
                        }
                    }
                },
                {
                    title: "Remark",
                    data: "FromRemark",
                    name: "FromRemark",
                    render: function (data, type, row) {
                        if (data != null) {
                            let sentence = data;
                            let words = sentence.split(" ");

                            let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                            return `<span class='cls-Faulty-FromRemark'>${truncatedSentence}</span>`;
                        } else {
                            return `NA`;
                        }

                    }
                },
                {
                    title: "AFSAC Remark",
                    data: "ToRemark",
                    name: "ToRemark",
                    render: function (data, type, row) {
                        if (data != null) {
                            let sentence = data;
                            let words = sentence.split(" ");

                            let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                            return `<span class='cls-Faulty-ToRemark'>${truncatedSentence}</span>`;
                        } else {
                            return `NA`;
                        }

                    }
                },
            ];
            break;

        case 'LostCase':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Request ID",
                    data: 'RequestId',
                    name: 'RequestId',
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
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
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Unit",
                    data: "UnitAbbreviation",
                    name: "UnitAbbreviation",
                    orderable: false,
                },
                {
                    title: "Date of Lost",
                    data: "LostOn",
                    name: "LostOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Fir Logged",
                    data: "IsFIRLogged",
                    name: "IsFIRLogged",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    }
                },
                {
                    title: "Support Doc",
                    data: "SupportDocName",
                    orderable: false,
                    name: "SupportDocName",
                    render: function (data, type, row, meta) {
                        return data ? `
                    <button class="cls-Lost-uploadedDoc btn btn-sm btn-success download-btn" title="Download">
                        <i class="fa fa-download"></i>
                    </button>` : "";
                    }
                },
                {
                    title: "Date & Time",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Remark",
                    data: "FromRemark",
                    name: "FromRemark",
                    render: function (data, type, row) {
                        let words = data.split(" ");
                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : data;
                        return `<span class='cls-Lost-FromRemark'>${truncatedSentence}</span>`;
                    }
                }
            ];
            break;

        case 'MonthlyProcessed':
            columns = [
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Request ID",
                    data: 'RequestId',
                    name: 'RequestId',
                },
                {
                    title: "Draft Dt. & Time",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
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
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Status",
                    data: "StepId",
                    name: "StepId",
                    render: function (data, type, row) {
                        let color;
                        if (data == 1 || data == 2 || data == 3 || data == 4 || data == 5 || data == 6 || data == 11 || data == 12 || data == 13 || data == 14) {
                            color = 'warning';
                        }
                        else if (data == 7 || data == 8 || data == 9 || data == 10) {
                            color = 'danger';
                        }
                        else {
                            color = 'success';
                        }
                        return `<span class='badge badge-${color} mr-1' >${row.Status}</span></span>`;
                    }
                }
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
async function GetReportDashboardCount() {
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
        const response = await fetch('/Home/GetReportDashboardCount', {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestData)
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

        $("#TotRequisition").html(data.TotRequisition);
        $("#TotLostCases").html(data.TotLostCases);
        $("#TotMonthlyProcessed").html(data.TotMonthlyProcessed);
        $("#TotNonFunctionalCard").html(data.TotNonFunctionalCard);

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
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ UnitMapId: 0 })
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

        const userdata = new URLSearchParams({
            TableId: 0,
            UnitType: UnitType,
            ComdId: normalize(ComdId),
            CorpsId: normalize(CorpsId),
            DivId: normalize(DivId),
            BdeId: normalize(BdeId),
            FmnBranchID: normalize(FmnBranchID),
            PsoId: normalize(PsoId),
            SubDteId: normalize(SubDteId)
        });

        const response = await fetch('/Master/GetUnitByHierarchy', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: userdata
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
    try {
        const response = await fetch('/Master/GetAllMMaster', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({
                id: TableId,
                ParentId: ParentId
            })
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
    try {
        const response = await fetch('/Master/GetAllMMasterByParent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({
                TableId,
                ComdId,
                CorpsId,
                DivId,
                BdeId
            })
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
    $("#TotRequisition").html('0');
    $("#TotLostCases").html('0');
    $("#TotMonthlyProcessed").html('0');
    $("#TotNonFunctionalCard").html('0');
}