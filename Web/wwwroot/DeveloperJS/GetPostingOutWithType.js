var table;
let spnPostingOutId;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData();
    $("#btnAdd").on("click", function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(ApplicantPostingOut);
    });
    $("#btnDispatchDetailsAddButton").on("click", function () {
        Proceed();
    });
});

function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
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
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
            };
            try {
                let response = await fetch("/Posting/GetAllPostingOutWithType", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
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
                title: "Updated On",
                data: "UpdatedOn",
                name: "UpdatedOn",
                className: "",
                width: "150px",
                defaultContent: '',
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "Army No , Rank , Name",
                data: null,
                name: null,
                name: "Reason",
                className: "nowrap",
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.ServiceNo} ${row.Rank || ""} ${row.FName || ""} ${row.LName || ""}`;
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;

                }
            },
            {
                title: "Reason",
                data: "Reason",
                name: "Reason",
                className: "nowrap",
                width: "180px",
                defaultContent: ''
            },
            {
                title: "SOS Dt",
                data: "SOSDate",
                name: "SOSDate",
                className: "",
                width: "150px",
                defaultContent: '',
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "From Unit",
                data: null,
                name: null,
                className: "nowrap",
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let FromUnit = `${row.FromUnitName}</br>${row.FromDomainId}`;
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${row.FromUnitName} ${row.FromDomainId}">${FromUnit}</To>`;

                }
            },
            {
                title: "To Unit",
                data: null,
                name: null,
                className: "nowrap",
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let ToUnit = `${row.ToUnitName}</br>${row.ToDomainId}`;
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${row.ToUnitName} ${row.ToDomainId}">${ToUnit}</To>`;
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
                title: "Dispatch Details",
                data: null,
                name: null,
                className: "noExport",
                width: "120px",
                orderable: false,
                render: function (data, type, row) {
                    let returnStr = ``;
                    let dateObj = new Date(row.DispatchedOn);
                    if (isNaN(dateObj.getTime()) || row.DispatchedOn === '0001-01-01T00:00:00') {
                        if (row.CanAddDispatchDetail) {
                            returnStr = `<span id="btnedit">
                                                <button type="button" class="cls-btnAddDispatchDetails btn btn-icon btn-round btn-warning mr-1">
                                                    <i class="fas fa-edit"></i>
                                                </button>
                                        </span>`;
                        }
                    }
                    else
                    {
                        returnStr = `<span id="btneye">
                                        <button type="button" class="cls-btneyedispatchdetails btn btn-icon btn-round btn-warning mr-1">
                                            <i class="fas fa-eye"></i>
                                        </button>
                                    </span>`;
                    }

                    return returnStr;
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
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: "thead th:not(.noExport)"
            //    }
            //},
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
                title: 'E-IASC_PostingOut',
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
            $("#tbldata tbody").off("click", ".cls-btneyedispatchdetails").on("click", ".cls-btneyedispatchdetails", function () {
                var rowData = table.row($(this).closest("tr")).data();
                let htmlBody = `${GetHtmlLabel('Dispatched Dt', DateFormateddMMyyyyhhmmss(rowData.DispatchedOn))}
                                ${GetHtmlLabel('Ref No Regd SDS', rowData.RefNo)}
                                ${GetHtmlLabel('Updated On', DateFormateddMMyyyyhhmmss(rowData.DispatchUpdatedOn))}
                                ${GetHtmlLabel('Updated By', rowData.DispatchUpdatedBy)}
                            `;
                $('#MessageDialog .modal-dialog').removeClass('modal-sm');
                $("#MessageDialogLabel").html('Dispatch Details');
                $("#MessageDialogBody").html(htmlBody);
                $("#MessageDialog").modal('show');
            });
            $("#tbldata tbody").off("click", ".cls-btnAddDispatchDetails").on("click", ".cls-btnAddDispatchDetails", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    Reset();
                    //ResetErrorMessage();
                    spnPostingOutId = rowData.Id;
                    $("#AddDispatchDetails").modal('show');
                }
            });
        }
    });

}

function Proceed() {
    if ($("#txtDispatchDate").val() == '') {
        toastr.error('Please Select Dispatch Date & Time');
        return;
    }

    if ($("#txtRefNo").val() == '') {
        toastr.error('Please Enter Ref No Regd.');
        return;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: "",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Save it!'
    }).then((result) => {
        if (result.isConfirmed) {
            Save();
        }
    })
}

function GetHtmlLabel(label,value) {
    return `<div class="row mb-2 align-items-center">
                <div class="col-md-4 fw-semibold text-muted">${label}</div>
                <div class="col-md-8">
                    <div class="form-control-plaintext border p-2 rounded bg-light">${value}</div>
                </div>
            </div>`;
}

function Reset() {
    spnPostingOutId = 0;
    $("#txtDispatchDate").val("");
    $("#txtRefNo").val("");
}

function Save() {
    const payload = {
        "encId": spnPostingOutId,
        "DispatchedOn": formatDateToSqlString($("#txtDispatchDate").val()),
        "RefNo": $("#txtRefNo").val(),
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    $.ajax({
        url: '/Posting/SavePostingOutDispatchDetails',
        type: 'POST',
        data: { Request: encrypted },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {
            if (result.Result == true) {
                $("#AddDispatchDetails").modal('hide');
                toastr.success(result.Message);
            }
            else {
                const Message = result.Message || "Something went wrong.";

                const errors = Message
                    .split(";")
                    .map(x => x.trim())
                    .filter(x => x !== "");

                const list = document.createElement("ul");
                list.classList.add("error-list"); // ✅ use CSS class

                errors.forEach(function (error) {
                    const item = document.createElement("li");
                    item.textContent = error;
                    list.appendChild(item);
                });

                Swal.fire({
                    icon: "error",
                    title: "Message",
                    html: list
                });
            }
            BindData();
        }
    });
}
