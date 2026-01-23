var table;
let dataExportType = 1;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    BindData()
    $("#btnAdd").on("click",function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(LostCardRequest);
    });


    $('#btnDataExports').on("click", function () {
        if (globalThis.selectedIds.length == 0) {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
        else {
            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Export",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Export it!'
            }).then((result) => {
                if (result.value) {
                    dataExportType = 2;
                    DataExport();
                }
            });
        }
    });

    $('#btnDataExportsEncry').on("click", function () {
        if (globalThis.selectedIds.length == 0) {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
        else {
            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Export",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Export it!'
            }).then((result) => {
                if (result.value) {
                    dataExportType = 1;
                    DataExport();
                }
            });
        }
    });

});
function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        var table = "";
        globalThis.selectedIds = [];
        resetSelectedFields();

        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
    }
    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        scroller: true,           // ✅ Enable virtual scrolling for better performance
        deferScroll: true,        // ✅ Improve scrolling performance
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,

        autoWidth: false,  //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        order: [[8, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let searchStatus = getSearchStatusForBindDialog(data.search.value);

            // Clear old selectedIds on search change, but keep globalAllChecked state
            if (searchStatus.searchChanged) {
                globalThis.selectedIds = [];

                // Mark for re-fetch if needed
                if (globalThis.globalAllChecked) {
                    globalThis.isFirstSelectAll = true;
                }
            }

            // ✅ Determine if a fetch is needed
            const shouldFetchSelectedIds = globalThis.globalAllChecked && (globalThis.isFirstSelectAll || searchStatus.searchChanged) || (!globalThis.globalAllChecked && searchStatus.searchChanged && globalThis.isFirstSelectAll);

            // If fetch is needed, manually set searchChanged to true
            if (shouldFetchSelectedIds) {
                searchStatus.searchChanged = true; // Manually set to true to ensure data fetch
            }
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                searchTextChanged: searchStatus.searchChanged,
                AllChecked: shouldFetchSelectedIds ? true : globalThis.globalAllChecked
            };
            try {
                let response = await fetch("/BasicDetail/GetAllLost", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) {
                    let userMessage = "Something went wrong while connecting to the server. Please try again later.";

                    // Optionally append technical info for logs or developers
                    console.error(`HTTP error! Status: ${response.status}`);

                    throw new Error(userMessage);
                }

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    globalThis.selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }

                // Only update selectedIds if server returns new ones
                if (shouldFetchSelectedIds) {
                    if (result.selectedIds != null && result.selectedIds.length > 0) {
                        //selectedIds = result.selectedIds;
                        globalThis.selectedIds = result.selectedIds.map(x => x.toString());
                        console.log("Fetched selectedIds from server:", globalThis.selectedIds);
                        // If user hadn’t checked Select All, now we just load into selectedIds silently
                        if (globalThis.globalAllChecked) globalThis.isFirstSelectAll = false;
                    }
                    else {
                        //selectedIds = [];
                        if (globalThis.globalAllChecked) {
                            globalThis.globalAllChecked = false;
                            $('#chkAll_LostCard').prop('checked', false);
                        }
                        console.warn("⚠️ No valid Pending IDs found.");
                    }
                }

                $("#lblTotal").html(result.recordsTotal);
                callback(result); // Sends data to DataTables

            } catch (error) {
                Swal.fire({
                    icon: "error",
                    title: "Data Fetch Failed",
                    text: "We couldn’t load the data right now. Please try again later.",
                    confirmButtonText: "OK"
                });
            }
        },
        columns: [
            {
                title: `<div class="wd-30-f"><div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll_LostCard">
                    <label class="custom-control-label" for="chkAll_LostCard"></label>
                    </div></div>`,
                data: "RequestId",
                targets: 0,
                orderable: false,
                className: "text-center",
                width: "40px",
                searchable: false,
                render: function (data, type, row) {
                    if ($("#chkAll_LostCard").prop('checked')) {
                        return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                    } else {

                        return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                    }
                }
            },
            {
                title: "S No",
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                className: "text-center col-sno",
                width: "60px",
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            //{ data: "RequestId", name: "RequestId" },
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
                title: "Rk & Name",
                data: null,
                name: null,
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    if (!fullName) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                }
            },
            {
                title: "Unit",
                data: "UnitAbbreviation",
                name: "UnitAbbreviation",
                className: "nowrap",
                width: "150px",
                orderable: false,
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Date Of Lost",
                data: "LostOn",
                name: "LostOn",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "FIR Logged",
                data: "IsFIRLogged",
                name: "IsFIRLogged",
                className: "",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            {
                title: "Supported Document",
                data: "SupportDocName",
                name: "SupportDocName",
                className: "",
                width: "150px",
                orderable: false,
                render: function (data, type, row, meta) {
                    return data ? `
                    <button class="cls-uploadedDoc btn btn-sm btn-success download-btn" title="Download">
                        <i class="fa fa-download"></i>
                    </button>` : "";
                }
            },
            {
                title: "Date & Time",
                data: "UpdatedOn",
                name: "UpdatedOn",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "Remark",
                data: "Remark",
                name: "Remark",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
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
            searchPlaceholder: "Search Army No" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
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
                title: 'E-IASC_LostCard',
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
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table.columns.adjust().responsive.recalc();
                }, 100);
            });
        },
        drawCallback: function (settings) {
            // Recalculate widths on each data load
            this.api().columns.adjust().responsive.recalc();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });


            $(".cls-uploadedDoc").on("click", function () {
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
            updateUICheckboxes('#tbldata', 'chkRequestId', '#chkAll_LostCard');
        }
    });

    $(document).on('change', '.chkRequestId', async function () {
        await updateSelectedIds('#tbldata', 'chkRequestId');
        updateUICheckboxes('#tbldata', 'chkRequestId', '#chkAll_LostCard'); // Sync master checkbox state
    });
    $('#chkAll_LostCard').on('change', function () {
        globalThis.selectedIds = [];
        globalThis.globalAllChecked = $(this).prop('checked');
        if (globalThis.globalAllChecked) {
            globalThis.isFirstSelectAll = true; // Force fresh fetch
        }
        table.ajax.reload();
    });
}

function DataExport() {
    var userdata = {
        "Ids": globalThis.selectedIds,  //Passing the JavaScript array directly
        "DataExportType": dataExportType
    };

    fetch('/BasicDetail/LostDataExport', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: JSON.stringify(userdata),
        credentials: 'same-origin'
    })
    .then(response => response.json())
        .then(data => {
            if (data.Result) {
                const baseUrl = window.location.origin;
                const downloadUrl = `${baseUrl}/BasicDetail/DownloadCsv?fileName=${data.Message}&fileStoreName=LostCard`;
                const link = document.createElement("a");
                link.href = downloadUrl;
                link.download = data.file;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
            else {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: data.Message
                });
            }
    });
}
function resetSelectedFields() {
    // Reset global variables as explained
    globalThis.selectedIds = [];
    globalThis.previousSearchText = "";
    globalThis.isFirstSelectAll = true;
    globalThis.searchChanged = false;
    globalThis.globalAllChecked = false;

    // Uncheck all checkboxes
    $('#tbldata tbody input[type="checkbox"].chkRequestId').prop('checked', false);

    // Reset "Select All" checkbox
    $('#chkAll_LostCard').prop('checked', false);

    console.log("Reset selectedIds and checkboxes.");
}
