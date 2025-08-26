var table;
let checkedDataIds = [];
let dataExportType = 1;
$(function () {
    BindData()
    $("#btnAdd").on("click",function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(LostCardRequest);
    });


    $('#btnDataExports').on("click", function () {
        if (checkedDataIds.length > 0) {
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
        } else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
    });

    $('#btnDataExportsEncry').on("click", function () {
        var lst = new Array();

        if (checkedDataIds.length > 0) {
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
        } else {
            Swal.fire({
                text: "Please select atleast 1 data to Export."
            });
        }
    });

});
function BindData() {
    $("#tbldata").DataTable().destroy();
    table = $("#tbldata").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        order: [[7, 'desc']], // Default sorting on the first column
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
                let response = await fetch("/BasicDetail/GetAllLost", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams(requestData).toString()
                });
                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
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
                data: "RequestId",
                targets: 0,
                orderable: false,
                searchable: false,
                render: function (data, type, row) {
                    return `<div class="custom-control custom-checkbox small">
                                <input type="checkbox" class="custom-control-input customcheckbox" id="${data}">
                                <label class="custom-control-label" for="${data}"></label>
                            </div>`;
                }
            },
            {
                data: null,
                name: null,
                visible: false,
                render: function (data, type, row) {
                    return `<span id='spnTrnFaultyCardId'> ${row.LostCardId}</span><span id='spnEncryptedId'>${row.EncryptedId}</span><span id='spnRequestId'>${row.RequestId}</span><span id='spnServiceNo'>${row.ServiceNo}</span>`;
                }
            },
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            //{ data: "RequestId", name: "RequestId" },
            { data: "ModifiedServiceNo", name: "ModifiedServiceNo" },
            {
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`;
                }
            },
            { data: "UnitAbbreviation", name: "UnitAbbreviation", orderable: false },
            {
                data: "LostOn",
                name: "LostOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: "IsFIRLogged",
                name: "IsFIRLogged",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                }
            },
            {
                data: "SupportDocName",
                orderable: false,
                name: "SupportDocName",
                render: function (data, type, row, meta) {
                    return data ? `
                    <button class="cls-uploadedDoc btn btn-sm btn-success download-btn" title="Download">
                        <i class="fa fa-download"></i>
                    </button>` : "";
                }
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: "Remark",
                name: "Remark",
                render: function (data, type, row) {
                    let words = data.split(" ");
                    let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : data;
                    return `<span class='cls-FromRemark'>${truncatedSentence}</span>`;
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
                title: 'E-IASC_LostCard',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
            // Add 'align-middle' class to all td elements in the body
            $('#tbldata tbody tr').each(function () {
                $(this).find('td').addClass('align-middle');
            });

            $("body").on("click", ".cls-FromRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                let Label = `Request Id :- ${rowData.RequestId}`;
                $("#MessageDialogLabel").text(Label);
                $("#MessageDialogBody").text(rowData.Remark);
                $("#MessageDialog").modal('show');
            });

            
            $("#tbldata #chkAll").on("click", function () {
                checkedDataIds = [];
                const isChecked = this.checked;
                $('.customcheckbox').each(function () {
                    $(this).prop('checked', isChecked);
                    if (isChecked) {
                        const id = $(this).attr('id');
                        checkedDataIds.push(id);
                    }
                });
            });

            $('#tbldata .customcheckbox').on('change', function () {
                const id = $(this).attr('id');
                const isChecked = this.checked;
                if (isChecked) {
                    if (!checkedDataIds.includes(id)) checkedDataIds.push(id);
                } else {
                    checkedDataIds = checkedDataIds.filter(x => x !== id);
                }
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
        }
    });
}

function DataExport() {
    var userdata = {
        "Ids": checkedDataIds,
        "DataExportType": dataExportType
    };

    fetch('/BasicDetail/LostDataExport', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userdata)
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
