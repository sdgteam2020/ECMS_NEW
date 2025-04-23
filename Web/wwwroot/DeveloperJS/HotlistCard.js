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
        $("#armynosearchTypeId").val(HoltlistCardRequest);
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
                let response = await fetch("/BasicDetail/GetAllHotlist", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
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
                    return `<span id='spnTrnFaultyCardId'> ${row.HotlistCardId}</span><span id='spnEncryptedId'>${row.EncryptedId}</span><span id='spnRequestId'>${row.RequestId}</span><span id='spnServiceNo'>${row.ServiceNo}</span>`;
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
            { data: "RequestId", name: "RequestId" },
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
                data: "UpdatedOn",
                name: "UpdatedOn",
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
                $("#MessageDialogLabel").html(Label);
                $("#MessageDialogBody").html(rowData.Remark);
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
    $.ajax({
        url: '/BasicDetail/HotlistDataExport',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: "Data Not Export Internal Server Error"
                    });
                } else {
                    //var blob = new Blob([response], {
                    //    type: 'application/json'
                    //});
                    //var link = document.createElement('a');
                    //link.href = 'data:text/plain;charset=utf-8,' + encodeURIComponent(blob);
                    //link.download = "export.json";
                    //link.click();
                    if (DataExportType == 1) {
                        window.location = "/WriteReadData/ExportAFSACCell/" + response + '.zip';
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    } else {
                        window.location = "/WriteReadData/ExportAFSACCell/" + response + '.zip';
                        setTimeout(function () {
                            location.reload();
                        }, 1000);
                    }


                    // var blob = new Blob([JSON.stringify(response, null, "\t")], { type: "application/json" });

                    // // Create a temporary anchor element
                    // var link = document.createElement("a");
                    // link.href = window.URL.createObjectURL(blob);




                    //// GetTokenSignXml(blob);
                    // // Set the file name
                    // link.download = "data.json";

                    // // Append the anchor to the body
                    // document.body.appendChild(link);

                    // // Trigger the click event
                    // link.click();

                    // // Remove the anchor from the body
                    // document.body.removeChild(link);


                    // setTimeout(function () {
                    //     location.reload();
                    // }, 1000);
                }


            }




        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
