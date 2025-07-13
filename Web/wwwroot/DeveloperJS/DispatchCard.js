var table; // Declare table variable outside the function to preserve the instance
$(function () {
    BindData();
    $("#btnAdd").on("click", function () {

    });
});
function BindData() {
    $("#tbldata").DataTable().destroy();
    table = $("#tbldata").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
            };
            try {
                let response = await fetch("/BasicDetail/GetAllDispatchCard", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            // Serial number column
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
                data: "LotNo",
                name: "LotNo",
            },
            {
                data: "ToUnit",
                name: "ToUnit",
            },
            {
                data: null,
                name: "Regt / ORO",
                orderable: false,
                render: function (data, type, row) {
                    let Name = row.RegimentalName == null ? row.RecordOfficeName : row.RegimentalName;
                    return (Name);
                }
            },
            {
                data: "NameOfCourierIncharge",
                name: "Name Of Courier Incharge"
            },
            {
                data: "ToServiceNo",
                name: "Army No",
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
                name: "Dispatch To",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.ToDID} (${row.ToRankName} ${row.ToName} )`.trim();
                    return (fullName);
                }
            },

            {
                data: "DispatchDate",
                name: "Dispatch On",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                data: "FromRemark",
                name: "AFSAC Remark",
                render: function (data, type, row) {
                    if (data != null) {
                        let sentence = data;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                        return `<span class='cls-FromRemark'>${truncatedSentence}</span>`;
                    } else {
                        return `NA`;
                    }

                }
            },
            {
                data: "ReceiptDate",
                name: "Dispatch In",
                render: function (data, type, row) {
                    return data != null ? DateFormateddMMyyyyhhmmss(data): "NA";
                }
            },
            {
                data: "ToRemark",
                name: "Remark",
                render: function (data, type, row) {
                    if (data != null) {
                        let sentence = data;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;
                        return `<span class='cls-ToRemark'>${truncatedSentence}</span>`;
                    } else {
                        return `NA`;
                    }

                }
            },
            // Additional column for Edit action
            {
                data: "IsComplete",
                name: "Action",
                orderable: false,
                render: function (data, type, row) {
                    if (data == false && parseInt($("#spnClaimValue").html()) != 1) {
                        return `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button>`;
                    } else {
                        return `NA`;
                    }
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
                title: 'E-IASC_DispatchCard',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.EncryptedId != null) {
                    sessionStorage.setItem("ArmyNo", rowData.ServiceNo);
                    window.location.href = '/BasicDetail/FaultyCardRequest?Id=' + encodeURIComponent(rowData.EncryptedId);
                }
            });

            $("#tbldata tbody").off("click", ".cls-FromRemark").on("click", ".cls-FromRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('Remark');
                    $("#MessageDialogBody").html(rowData.FromRemark);
                    $("#MessageDialog").modal('show');
                }
            });

            $("#tbldata tbody").off("click", ".cls-ToRemark").on("click", ".cls-ToRemark", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('AFSAC Remark');
                    $("#MessageDialogBody").html(rowData.ToRemark);
                    $("#MessageDialog").modal('show');
                }
            });

        }
    });
}