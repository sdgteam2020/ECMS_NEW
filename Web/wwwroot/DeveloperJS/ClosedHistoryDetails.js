let table_ClosedHistory;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldatatabledata_ClosedHistory');

    let UserType = ($("#spnType").html());
    let ApplyForId = $("#spnJCOOR").html();
    BindData(UserType, ApplyForId, function () {
        // Reset global variables as explained
        globalThis.selectedIds = [];
        globalThis.previousSearchText = "";
        globalThis.isFirstSelectAll = true;
        globalThis.searchChanged = false;
        globalThis.globalAllChecked = false;
    });
    $(window).on('resize', function () {
        // Check if element exists AND is a DataTable
        if ($('#tbldatatabledata_ClosedHistory').length && $.fn.DataTable.isDataTable('#tbldatatabledata_ClosedHistory')) {
            $('#tbldatatabledata_ClosedHistory').DataTable().columns.adjust();
        }
    });
});
$("#BasicDetailClosedHistory")
    .off("click", ".cls-btndownloadclosedhistorypdf")
    .on("click", ".cls-btndownloadclosedhistorypdf", function (e) {

        e.preventDefault();
        e.stopPropagation();

        const requestId = parseInt($(this).attr("data-request-id"));

        if (!isNaN(requestId) && requestId > 0) {
            GenerateClosedHistoryPDF(requestId);
        } else {
            alert("Invalid request.");
        }
    });

function BindData(UserType, ApplyForId) {
    globalThis.selectedIds = [];

    if ($.fn.DataTable.isDataTable("#tbldatatabledata_ClosedHistory")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatatabledata_ClosedHistory").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatatabledata_ClosedHistory thead").empty(); // Clear old thead
        $("#tbldatatabledata_ClosedHistory tbody").empty(); // Clear old tbody
    }


    const columns = getColumnsForClosedHistory(UserType, ApplyForId);
    table_ClosedHistory = $("#tbldatatabledata_ClosedHistory").DataTable({
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
        order: [[2, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let searchStatus = getSearchStatusForBindData(data.search.value);

            // Clear old selectedIds on search change, but keep globalAllChecked state
            if (searchStatus.searchChanged) {
                globalThis.selectedIds = [];

                // Mark for re-fetch if needed
                if (globalThis.globalAllChecked) {
                    globalThis.isFirstSelectAll = true;
                }
            }

            // ✅ Determine if a fetch is needed
            const shouldFetchSelectedIds =
                globalThis.globalAllChecked && (globalThis.isFirstSelectAll || searchStatus.searchChanged) ||
                (!globalThis.globalAllChecked && searchStatus.searchChanged && globalThis.isFirstSelectAll);

            // If fetch is needed, manually set searchChanged to true
            if (shouldFetchSelectedIds) {
                searchStatus.searchChanged = true; // Manually set to true to ensure data fetch
            }


            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: searchStatus.currentSearchText,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order               
                CValue: 0,
                UserType: UserType,
                ApplyForId: ApplyForId,
                searchTextChanged: searchStatus.searchChanged
            };
            //alert(UserType)
            //alert(ApplyForId)
            let encryptedPayload = "";
            if (requestData) {
                const jsonData = JSON.stringify(requestData);
                encryptedPayload = encryptPayloadData(jsonData);

            }
            try {
                let response = await fetch("/BasicDetail/GetAllClosedHistory", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: JSON.stringify({ data: encryptedPayload })
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    globalThis.selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }

                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        columnDefs: [
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]  // Only global settings
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No / Appl ID" // Add custom placeholder
        },
        dom: "<'dt-top ecms-dt-toolbar d-flex justify-content-between align-items-center flex-wrap'lBf>rt<'ecms-dt-footer row no-gutters'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>", // Shared ModernCSS DataTable toolbar/footer

        // 👇 Show modal only after table (header + data) is fully rendered
        initComplete: function () {
            if (typeof callback === "function") {
                callback(); // show modal now
            }
            // Force DataTables to calculate optimal widths
            this.api().columns.adjust();

            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    table_ClosedHistory.columns.adjust().responsive.recalc();
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
            $("#tbldatatabledata_ClosedHistory tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table_ClosedHistory.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetClosedHistoryByRequestId(rowData.RequestId);
                    SetClosedHistoryHeader(rowData.RequestId);
                }
            });
            $("#tbldatatabledata_ClosedHistory tbody").off("click", ".cls-btndownloadpdfphotosignature").on("click", ".cls-btndownloadpdfphotosignature", function () {
                var rowData = table_ClosedHistory.row($(this).closest("tr")).data();
                if (rowData != null) {
                    DownloadPdf(rowData.RequestId);
                }
            });
        }
    });
    table_ClosedHistory.button('.buttons-copy').nodes().hide();
    table_ClosedHistory.button('.buttons-csv').nodes().hide();
    table_ClosedHistory.button('.buttons-print').nodes().hide();

}

function getColumnsForClosedHistory(UserType, ApplyForId) {
    let columns = [];
    switch (ApplyForId) {
        case "0":
            columns = [

                // Serial number column
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    className: "text-center col-sno",
                    width: "30px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl Id",
                    data: "RequestId",
                    name: "ApplId",
                    className: "nowrap",
                    width: "60px",
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
                    orderable: false,
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
                    title: "Reason",
                    data: "Reason",
                    name: "Reason",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Closed On",
                    data: "ClosedOn",
                    name: "ClosedOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Remark",
                    data: "Remarks",
                    name: "Remarks",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Auth",
                    data: "Authority",
                    name: "Authority",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: `<div>History</div>`,
                    className: "noExport",
                    width: "100px",
                    data: null,
                    name: "History",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                            // Add a download button next to the history button
                            `<button class="cls-btndownloadpdfphotosignature" data-toggle="tooltip" data-placement="top" title="Download Details">
                                <img src="/Images/digitalsign.png" width="40" />
                                </button>`;
                    }
                },
            ];
            break;
        default:
            columns = [

                // Serial number column
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    className: "text-center col-sno",
                    width: "30px",
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Appl Id",
                    data: "RequestId",
                    name: "ApplId",
                    className: "nowrap",
                    width: "60px",
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
                    orderable: false,
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
                    title: "Reason",
                    data: "Reason",
                    name: "Reason",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Closed On",
                    data: "ClosedOn",
                    name: "ClosedOn",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Remark",
                    data: "Remarks",
                    name: "Remarks",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Auth",
                    data: "Authority",
                    name: "Authority",
                    className: "",
                    width: "150px",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },

                {
                    title: `<div>History</div>`,
                    className: "noExport",
                    width: "100px",
                    data: null,
                    name: "History",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                            // Add a download button next to the history button
                            + `<button class="cls-btndownloadpdfphotosignature" data-toggle="tooltip" data-placement="top" title="Download Closed Details">
                                <img src="/Images/digitalsign.png" width="40" />
                                </button>`;
                    }
                },

            ];
    }
    return columns;
}
function getSearchStatusForBindData(search) {
    const currentSearchText = search.trim();

    // Ensure searchChanged is only true when the actual search field or text changes.
    globalThis.searchChanged = (
        (currentSearchText !== globalThis.previousSearchText)
    );

    // Update previous values after comparison
    globalThis.previousSearchText = currentSearchText;

    return {
        searchChanged: globalThis.searchChanged,
        currentSearchText
    };
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