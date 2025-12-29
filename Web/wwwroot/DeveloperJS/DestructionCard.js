var table;
let checkedDataIds = [];
let dataExportType = 1;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData();
    $("#btnAdd").on("click",function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(DestructionCardRequest);
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
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY
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
                let response = await fetch("/BasicDetail/GetAllDestruction", {
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
                    return `<span id='spnTrnFaultyCardId'> ${row.DestructionCardId}</span><span id='spnEncryptedId'>${row.EncryptedId}</span><span id='spnRequestId'>${row.RequestId}</span><span id='spnServiceNo'>${row.ServiceNo}</span>`;
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
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    return `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`;
                }
            },
            { data: "UnitAbbreviation", name: "UnitAbbreviation", orderable: false },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: "DestructedOn",
                name: "DestructedOn",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: 'RemarksNameList',
                name: 'RemarksNameList',
                orderable: false,
                render: function (data, type, row) {
                    return "<button type='button' class='cls-remarks btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i><span id='spnRemarks' class='d-none'></span></button>";
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
                title: 'E-IASC_HotlistCard',
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

            $("body").on("click", ".cls-remarks", function () {
                var rowData = table.row($(this).closest("tr")).data();
                let Label = `Request Id :- ${rowData.RequestId}`;
                var remarksArray = rowData.RemarksNameList.split('#');
                let listItem = "";
                if (remarksArray != null) {
                    listItem += "<ul>";
                    for (var j = 0; j < remarksArray.length; j++) {
                        listItem += "<li>" + remarksArray[j] + "</li>";
                    }
                    listItem += "</ul>";
                }
                $("#MessageDialogLabel").html(Label);
                $("#MessageDialogBody").html(listItem);
                $("#MessageDialog").modal('show');
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

            $('#DetailBody .customcheckbox').on('change', function () {
                const id = $(this).attr('id');
                const isChecked = this.checked;
                if (isChecked) {
                    if (!checkedDataIds.includes(id)) checkedDataIds.push(id);
                } else {
                    checkedDataIds = checkedDataIds.filter(x => x !== id);
                }
            });
        }
    });
}

function DataExport() {
    var userdata = {
        "Ids": checkedDataIds,
        "DataExportType": dataExportType
    };

    fetch('/BasicDetail/DestructionDataExport', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: JSON.stringify(userdata)
    })
    .then(response => response.json())
        .then(data => {
            if (data.Result) {
                const baseUrl = window.location.origin;
                const downloadUrl = `${baseUrl}/BasicDetail/DownloadCsv?fileName=${data.Message}&fileStoreName=DestructionCard`;
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
