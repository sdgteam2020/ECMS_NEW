var table; // Declare table variable outside the function to preserve the instance
$(function () {
    GetReportDashboardCount();
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

    if ($('#ddlCommand').length > 0) {
        mMsater(0, "ddlCommand", 1, "");
    }
    //$("#btnRequisition").on("click", function (event) {
    //    event.preventDefault(); // Prevent anchor default behavior
    //    GetReportReturnHistory('Requisition');
    //})
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
    var userdata =
    {
        "TableId": 0,
        "ComdId": $('#ddlCommand').length > 0 ? $('#ddlCommand').val() : 0,
        "CorpsId": $('#ddlCorps').length > 0 ? $('#ddlCorps').val() : 0,
        "DivId": $('#ddlDiv').length > 0 ? $('#ddlDiv').val() : 0,
        "BdeId": $('#ddlBde').length > 0 ? $('#ddlBde').val() : 0,
        "FmnBranchID": $('#ddlFmnBranch').length > 0 ? $('#ddlFmnBranch').val() : 0,
        "PsoId": $('#ddlPSODte').length > 0 ? $('#ddlPSODte').val() : 0,
        "SubDteId": $('#ddlDgSubDte').length > 0 ? $('#ddlDgSubDte').val() : 0,
        "UnitMapId": $('#ddlUnit').length > 0 ? $('#ddlUnit').val() : 0,
        "MonthYear": $('#txtMonthYear').length > 0 ? $('#txtMonthYear').val() : null,
        "Choice": Choice,
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
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
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
