var lst = '<option value="null">All</option>';
var comid = 0; var corId = 0; var divId = 0; var bdeId = 0; var FmnBranchId = 0; var PsoId = 0; var SubDteId = 0;
var UnitType = 1;
var table; // Declare table variable outside the function to preserve the instance
$(function () {
    GetReportDashboardCount();

    if ($('#ddlCommand').length > 0) {
        $('#ddlCommand').on('change', function () {
            comid = $(this).val();
            mMsater(false, 0, "ddlCorps", 2, $('#ddlCommand').val());
            $("#ddlDiv").html(lst);
            $("#ddlBde").html(lst);
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst);
        });
        // Manually trigger change if only one option exists
        if ($('#ddlCommand option').length === 1) {
            $('#ddlCommand').trigger('change');
        }
    }
    if ($('#ddlCorps').length > 0) {
        $('#ddlCorps').on('change', function () {
            corId = $(this).val();
            mMsaterByParent(false, 0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
            $("#ddlBde").html(lst);
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst);
        });
    }
    if ($('#ddlDiv').length > 0) {
        $('#ddlDiv').on('change', function () {
            divId = $(this).val();
            mMsaterByParent(false, 0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId     
            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);
            $("#ddlUnit").html(lst);
        });
    }
    if ($('#ddlBde').length > 0) {
        $('#ddlBde').on('change', function () {
            bdeId = $(this).val();
            mMsater(false, 0, "ddlFmnBranch", FmnBranches, "");
            $("#ddlUnit").html(lst);
            GetUnitByHierarchy(false, "ddlUnit", 0, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), $('#ddlBde').val(), 1, 1, 1);

        });
    }
    if ($('#ddlFmnBranch').length > 0) {
        $('#ddlFmnBranch').on('change', function () {
            FmnBranchId = $(this).val();
            $("#ddlUnit").html(lst);
            GetUnitByHierarchy(false,"ddlUnit", 0, $("#ddlCommand").val(), $("#ddlCorps").val(), 1, 1, $("#ddlFmnBranch").val(), 1, 1);

        });
    }
    if ($('#ddlDgSubDte').length > 0) {
        $('#ddlDgSubDte').on('change', function () {
            SubDteId = $(this).val();
            GetUnitByHierarchy(false,"ddlUnit", 0, 1, 1, 1, 1, 1, PsoId, $("#ddlDgSubDte").val());
        });
    }
    if ($('#ddlPSODte').length > 0) {
        $('#ddlPSODte').on('change', function () {
            PsoId = $(this).val();
            GetUnitByHierarchy(false,"ddlUnit", 0, 1, 1, 1, 1, 1, $("#ddlPSODte").val(), SubDteId);
        });
    }
    $('input[name="UnitTyperdi"]').on("click",function () {

        UnitType = $("input[type='radio'][name=UnitTyperdi]:checked").val();

        if (UnitType == "1") {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();


            if ($("#spnclaimId").html() == "Army Level Reports") {
                mMsater(false, '', "ddlCommand", 1, "");

                $("#ddlCorps").html(lst);
                $("#ddlDiv").html(lst);
                $("#ddlBde").html(lst);
            }
            else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                mMsater(true, comid, "ddlCommand", 1, "");
            }
            else {
                mMsater(true, comid, "ddlCommand", 1, "");

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
                mMsater(false, '', "ddlCommand", 1, "");
                mMsater(false, '', "ddlFmnBranch", FmnBranches, "");

                $("#ddlCorps").html(lst);
                $("#ddlDiv").html(lst);
                $("#ddlBde").html(lst);
            }
            else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                mMsater(true, comid, "ddlCommand", 1, "");
                mMsater(true, FmnBranchId, "ddlFmnBranch", FmnBranches, "");
            }
            else {
                mMsater(true, comid, "ddlCommand", 1, "");
                mMsater(true, FmnBranchId, "ddlFmnBranch", FmnBranches, "");

            }

            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
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
                mMsater(false, '', "ddlPSODte", PSO, "");
                mMsater(false, '', "ddlDgSubDte", SubDte, "");
            }
            else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                mMsater(true, PsoId, "ddlPSODte", PSO, "");
                mMsater(true, SubDteId, "ddlDgSubDte", SubDte, "");
            }
            else {
                mMsater(true, PsoId, "ddlPSODte", PSO, "");
                mMsater(true, SubDteId, "ddlDgSubDte", SubDte, "");

            }
        }
    });

    if ($('#spnclaimId').length > 0) {
        if ($('#spnclaimId').html() === 'Army Level Reports' || $('#spnclaimId').html() === 'Fmn Level Reports') {
            GetLoginUnitMappingDetails();
        }
    }


    $('#txtMonthYear').datepicker({
        dateFormat: 'mm/yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        yearRange: function () {
            const today = new Date();
            return (today.getFullYear() - 2) + ":" + today.getFullYear();
        }(),

        beforeShow: function (input, inst) {
            setTimeout(function () {
                const today = new Date();
                const currentYear = today.getFullYear();
                const currentMonth = today.getMonth();

                function restrictUI() {
                    $(".ui-datepicker-calendar").hide();

                    const selectedYear = parseInt($(".ui-datepicker-year").val(), 10);
                    const selectedMonth = parseInt($(".ui-datepicker-month").val(), 10);

                    // Disable future months
                    $(".ui-datepicker-month option").each(function (index) {
                        if (selectedYear === currentYear && index > currentMonth) {
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
                    const isMinMonth = selectedYear === (currentYear - 2) && selectedMonth === 0;

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
            const month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
            const year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
            if (month !== null && year !== null) {
                const formattedMonth = ("0" + (parseInt(month) + 1)).slice(-2);
                $(this).val(formattedMonth + "/" + year); // enforce MM/YYYY format
            }
        }
    });

    $("#txtMonthYear").attr("readonly", true).on("keydown paste input", function (e) {
        e.preventDefault();
    });


    $("#btnRequisition").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('New Requisition');
        $("#CardReport").modal("show");
        $('#CardReport').one('shown.bs.modal', function () {
            GetReportReturnHistory('Requisition');
        });
    });
    $("#btnNonFunctionalCard").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('Non Functional Card');
        $("#CardReport").modal("show");
        $('#CardReport').one('shown.bs.modal', function () {
            GetReportReturnHistory('NonFunctional');
        });
    });
    $("#btnLostCase").on("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        $("#CardReport_lblModelTitle").html('Lost Case');
        $("#CardReport").modal("show");
        $('#CardReport').one('shown.bs.modal', function () {
            GetReportReturnHistory('LostCase');
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
        $("#CardReport").modal("show");

        $('#CardReport').one('shown.bs.modal', function () {
            GetReportReturnHistory('MonthlyProcessed');
        });
    });
});
function GetReportReturnHistory(Choice) {
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
        processing: true,
        serverSide: true,
        filter: true,
        order: [[1, 'desc']], // Default sorting on the first column
        responsive: true,
        autoWidth: false,
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
            console.log(JSON.stringify(requestData));
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
                    title: "Tracking Id",
                    data: 'TrackingId',
                    name: 'TrackingId',
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
                    title: "Rank,Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Status",
                    data: "StepId",
                    name: "StepId",
                    render: function (data, type, row) {
                        let color;
                        if (data == 1 || data == 2 || data == 3 || data == 4 || data == 5 || data == 6) {
                            color = 'warning';
                        }
                        else {
                            color = 'dangers';
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
                    title: "Rank,Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
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
                        return data;
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
                    title: "Rank,Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
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
                    title: "Tracking Id",
                    data: 'TrackingId',
                    name: 'TrackingId',
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
                    title: "Rank,Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor"
                },
                {
                    title: "Status",
                    data: "StepId",
                    name: "StepId",
                    render: function (data, type, row) {
                        let color;
                        if (data == 1 || data == 2 || data == 3 || data == 4 || data == 5 || data == 6) {
                            color = 'warning';
                        }
                        else {
                            color = 'dangers';
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
function GetReportDashboardCount() {
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Home/GetReportDashboardCount',
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

                    $("#TotRequisition").html(response.TotRequisition);
                    $("#TotLostCases").html(response.TotLostCases);
                    $("#TotMonthlyProcessed").html(response.TotMonthlyProcessed);
                    $("#TotNonFunctionalCard").html(response.TotNonFunctionalCard);

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


                    UnitType = response.UnitType;
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

                        if ($("#spnclaimId").html() == "Army Level Reports") {
                            mMsater(false, '', "ddlCommand", 1, "");
                        }
                        else if ($("#spnclaimId").html() == "Fmn Level Reports") {
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

                            GetUnitByHierarchy(false, "ddlUnit", response.UnitId, response.ComdId, response.CorpsId, response.DivId, response.BdeId,1, 1, 1);
                        }
                        else {
                            mMsater(true, response.ComdId, "ddlCommand", 1, "");
                            mMsater(true, response.CorpsId, "ddlCorps", 2, response.ComdId);
                            mMsaterByParent(true, response.DivId, "ddlDiv", 3, response.ComdId, response.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                            mMsaterByParent(true, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId

                            GetUnitByHierarchy(true, "ddlUnit", response.UnitId, response.ComdId, response.CorpsId, response.DivId, response.BdeId, 1, 1, 1);
                        }

                        $(".unittype").removeClass("d-none");
                        $(".FmnBranch").addClass("d-none");
                        $(".DteBranch").addClass("d-none");

                        $("#ddlFmnBranch").html(lst);
                        $("#ddlPSODte").html(lst);
                        $("#ddlDgSubDte").html(lst);

                    }
                    else if (parseInt(response.UnitType) == 2) {
                        $("#UnitType2").prop("checked", true);

                        if ($("#spnclaimId").html() == "Army Level Reports") {
                            mMsater(false, '', "ddlCommand", 1, "");
                            mMsater(true, response.FmnBranchID, "ddlFmnBranch", FmnBranches, "");
                        }
                        else if ($("#spnclaimId").html() == "Fmn Level Reports") {

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
                        else {
                            mMsater(true, response.ComdId, "ddlCommand", 1, "");
                            mMsater(true, response.CorpsId, "ddlCorps", 2, response.ComdId);
                            mMsaterByParent(true, response.DivId, "ddlDiv", 3, response.ComdId, response.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                            mMsaterByParent(true, response.BdeId, "ddlBde", 4, response.ComdId, response.CorpsId, response.DivId, 0);///ComdId,CorpsId,DivId,BdeId
                            mMsater(true, response.FmnBranchID, "ddlFmnBranch", FmnBranches, "");

                            GetUnitByHierarchy(true, "ddlUnit", response.UnitId, response.ComdId, response.CorpsId, response.DivId, response.BdeId, response.FmnBranchID, 1, 1);
                        }
                        $("#ddlPSODte").html(lst);
                        $("#ddlDgSubDte").html(lst);

                        $(".unittype").removeClass("d-none");
                        $(".FmnBranch").removeClass("d-none");
                        $(".DteBranch").addClass("d-none");

                    }
                    else if (parseInt(response.UnitType) == 3) {
                        $("#UnitType3").prop("checked", true);

                        if ($("#spnclaimId").html() == "Army Level Reports") {
                            mMsater(false, '', "ddlPSODte", PSO, "");
                            mMsater(false, '', "ddlDgSubDte", SubDte, "");
                        }
                        else if ($("#spnclaimId").html() == "Fmn Level Reports") {
                            mMsater(true, response.PsoId, "ddlPSODte", PSO, "");
                            mMsater(true, response.SubDteId, "ddlDgSubDte", SubDte, response.PsoId);

                            GetUnitByHierarchy(false,"ddlUnit", response.UnitId, 1, 1, 1, 1, 1, response.PsoId, response.SubDteId);
                        }
                        else {
                            mMsater(true,response.PsoId, "ddlPSODte", PSO, "");
                            mMsater(true,response.SubDteId, "ddlDgSubDte", SubDte, "");
                            GetUnitByHierarchy(true,"ddlUnit", response.UnitId, 1, 1, 1, 1, 1, response.PsoId, response.SubDteId);
                        }

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

function GetUnitByHierarchy(IsOnly, ddl, sectid, ComdId, CorpsId, DivId, BdeId, FmnBranchID, PsoId, SubDteId) {
    var listItem = "";
    var userdata =
    {
        "TableId": 0,
        "UnitType":UnitType,
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



                    listItem += '<option value="null">All</option>';
                    for (var i = 0; i < response.length; i++) {
                        if (IsOnly == true && response[i].UnitId == sectid) {

                            listItem += '<option value="' + response[i].UnitId + '">' + response[i].UnitName + '</option>';
                        } else if
                            (IsOnly == false) {
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

function mMsater(IsOnly, sectid = '', ddl, TableId, ParentId) {


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
                        listItemddl += '<option value="null">All</option>';
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

function mMsaterByParent(IsOnly, sectid = '', ddl, TableId, ComdId, CorpsId, DivId, BdeId) {


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
                        listItemddl += '<option value="null">All</option>';
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