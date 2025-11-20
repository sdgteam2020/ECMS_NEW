var table;
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
});

function BindData() {
    $("#tbldata").DataTable().destroy();
    table = $("#tbldata").DataTable({
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
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                defaultContent: '',
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.ServiceNo}</br>${row.Rank || ""} ${row.FName || ""} ${row.LName || ""}`;
                }
            },
            { data: "Reason", name: "Reason", defaultContent: '' },
            {
                data: "SOSDate",
                name: "SOSDate",
                defaultContent: '',
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.FromUnitName}</br>${row.FromDomainId}`;
                }
            },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.ToUnitName}</br>${row.ToDomainId}`;
                }
            },
            { data: "Authority", name: "Authority" },
            {
                data: null,
                name: null,
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
                title: 'E-IASC_PostingOut',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
            $("body").on("click", ".cls-btneyedispatchdetails", function () {
                var rowData = table.row($(this).closest("tr")).data();
                let htmlBody = `${GetHtmlLabel('Dispatched Dt', DateFormateddMMyyyyhhmmss(rowData.DispatchedOn))}
                                ${GetHtmlLabel('Ref No Regd SDS', rowData.RefNo) }
                                ${GetHtmlLabel('Updated On', DateFormateddMMyyyyhhmmss(rowData.DispatchUpdatedOn)) }
                                ${GetHtmlLabel('Updated By', rowData.DispatchUpdatedBy) }
                            `;
                $('#MessageDialog .modal-dialog').removeClass('modal-sm');
                $("#MessageDialogLabel").html('Dispatch Details');
                $("#MessageDialogBody").html(htmlBody);
                $("#MessageDialog").modal('show');
            });

            $("body").on("click", ".cls-btnAddDispatchDetails", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    Reset();
                    //ResetErrorMessage();

                    $("#spnPostingOutId").html(rowData.Id);
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
    $("#spnPostingOutId").html("0");
    $("#txtDispatchDate").val("");
    $("#txtRefNo").val("");
}

function Save() {
    $.ajax({
        url: '/Posting/SavePostingOutDispatchDetails',
        type: 'POST',
        data: {
            "encId": $("#spnPostingOutId").html(),
            "DispatchedOn": formatDateToSqlString($("#txtDispatchDate").val()),
            "RefNo": $("#txtRefNo").val(),
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            if (data.Result) {
               $("#AddDispatchDetails").modal('hide');
               Swal.fire({
                   title: "Success!",
                   text: data.Message,
                   icon: "success",
                   confirmButtonText: "OK"
               });
           }
           else {
               Swal.fire({
                   title: "OOPs!",
                   text: data.Message,
                   icon: "error",
                   confirmButtonText: "Ok"
               });
           }
           BindData();
        }
    });
}
