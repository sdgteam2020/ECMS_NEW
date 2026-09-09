var table_Fwd; // Declare table variable outside the function to preserve the instance
$(function () {
    //sessionStorage.clear();
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    let UserType = $("#spnUserType").html();
    let ApplyForId = parseInt($("#spnApplyForId").html());

    BindData(UserType, ApplyForId, function () {
    });

    $(window).on('resize', function () {
        // Check if element exists AND is a DataTable
        if ($('#tbldatatabledata_Completed').length && $.fn.DataTable.isDataTable('#tbldatatabledata_Completed')) {
            $('#tbldatatabledata_Completed').DataTable().columns.adjust();
        }
    });
    $("#BasicDetailCompletedHistory")
        .off("click", ".cls-btndownloadpdf")
        .on("click", ".cls-btndownloadpdf", function (e) {

            e.preventDefault();
            e.stopPropagation();

            const requestId = parseInt($(this).attr("data-request-id"));

            if (!isNaN(requestId) && requestId > 0) {
                GetCompletedHistoryPdf(requestId);
            } else {
                alert("Invalid request.");
            }
        });
});
function BindData(UserType, ApplyForId) {
    if ($.fn.DataTable.isDataTable("#tbldatatabledata_Completed")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_Completed").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_Completed thead").empty(); // Clear old thead
        $("#tbldatatabledata_Completed tbody").empty(); // Clear old tbody
    }

    table_Fwd = $("#tbldatatabledata_Completed").DataTable({
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

        autoWidth: false, //Set autoWidth to true (let DataTables decide)
        responsive: false, // Columns can hide on small screens
        deferRender: true,// ✅ Handle zoom changes
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                UserType: UserType,
                ApplyForId: ApplyForId,
                CValue: 0
            };
            let encryptedPayload = "";
            if (requestData) {
                const jsonData = JSON.stringify(requestData);
                encryptedPayload = encryptPayloadData(jsonData);

            }
            try {
                let response = await fetch("/BasicDetail/GetAllCompletedHistory", {
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
            // Serial number column
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
            {
                title: "Appl ID",
                data: "RequestId",
                name: "RequestId",
                className: "nowrap",
                width: "100px",
            },
            {
                title: "ServiceNo",
                data: "ServiceNo",
                name: "ServiceNo",
                className: "nowrap",
                width: "120px",
                render: function (data, type, row) {
                    let displayText = data;

                    // If first two chars are alphabets, insert space
                    if (/^[A-Za-z]{2}/.test(data)) {
                        displayText = data.slice(0, 2) + ' ' + data.slice(2);
                    }

                    return `${displayText}`;
                }
            },
            {
                title: "Rank & Name",
                data: null,
                name: "Name",
                className: "nowrap",
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.Name || ""}`.trim();
                    if (!fullName) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                }
            },
            {
                title: "Date Of Completed",
                data: "CompletedOn",
                name: "CompletedOn",
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
                width: "100px",
                orderable: false,
                render: function (data, type, row) {
                    return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>
                            <button class="cls-btndownloadpdfsignature" data-toggle="tooltip" data-placement="top" title="Download Details"><img src="/Images/digitalsign.png" width="40" /></button>`
                }
            }
        ],
        columnDefs: [
            { targets: 0, width: "60px", },
            { targets: 1, width: "100px" },
            { targets: 2, width: "120px" },
            { targets: 3, width: "180px" },
            { targets: 4, width: "150px" },
            { targets: 5, width: "100px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
        },
        dom: "<'dt-top ecms-dt-toolbar d-flex justify-content-between align-items-center flex-wrap'lBf>rt<'ecms-dt-footer row no-gutters'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>", // Shared ModernCSS DataTable toolbar/footer
        buttons: [
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: ':visible:not(.noExport)'
            //    }
            //},
            {
                extend: 'excel',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'portrait',
                pageSize: 'A4', //A3 , A5 , A6 , legal , letter
                title: 'E-IASC_Appl',
                exportOptions: {
                    columns: ':visible:not(.noExport)'
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
                    table_Fwd.columns.adjust().responsive.recalc();
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

            $("#tbldatatabledata_Completed tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetCompletedHistoryByRequestId(rowData.RequestId);
                    SetCompletedHistoryHeader(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_Completed tbody").off("click", ".cls-btndownloadpdfsignature").on("click", ".cls-btndownloadpdfsignature", function () {
                var rowData = table_Fwd.row($(this).closest("tr")).data();
                if (rowData != null) {
                    DownloadPdf(rowData.RequestId);
                }
            });
        }
    });
}

function DownloadPdf(RequestId) {
    try {
        const encryptedRequest = encryptPayloadData(RequestId);

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Log/CreatePdf';
        form.target = '_blank';
        form.style.display = 'none';

        const requestInput = document.createElement('input');
        requestInput.type = 'hidden';
        requestInput.name = 'Request';
        requestInput.value = encryptedRequest;
        form.appendChild(requestInput);

        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = globalThis.RequestVerificationToken;
        form.appendChild(tokenInput);

        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    } catch (e) {
        Swal.fire({
            text: errormsg002
        });
    }
}