var table; // Declare table variable outside the function to preserve the instance
var table2;
$(function () {
    BindData();
    $("#btnAdd").on("click", function () {
        location.href = '/BasicDetail/DispatchOut';
    });
    $("#btnSubmit").on('click', async function (e) {
        let formId = '#SaveDispatchCardIn';
        $.validator.unobtrusive.parse($(formId));
        let DispatchCardId = parseInt($("#spnDispatchCardId").html());
        if (DispatchCardId == 0 || DispatchCardId < 0) {
            toastr.error('Invalid Dispatch Card Id.');
            return false; 
        }

        // Check Form Validation
        if ($(formId).valid()) {
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
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Please fill required field.',

            })
            toastr.error('Please fill required field.');
            return false;
        }
    });
});
async function Save() {
    try {
        var token = $('input[name="__RequestVerificationToken"]').val();
        let formData = new FormData();
        let DispatchCardId = parseInt($("#spnDispatchCardId").html());

        formData.append('DispatchCardId', DispatchCardId);
        formData.append('ToRemark', $("#txtToRemark").val());

        // Append the CSRF token if needed (depends on your backend configuration)
        formData.append('__RequestVerificationToken', token);

        const response = await fetch('/BasicDetail/DispatchCardIn', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            body: formData
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            $("#DispatchInDialog").modal('hide');
            Swal.fire({
                title: "Success!",
                text: result.Message,
                icon: "success",
                confirmButtonText: "OK"
            }).then(() => {
                // Wait for the SweetAlert to close before reloading the page
                setTimeout(() => {
                    location.reload();
                }, 1500); // 1500 milliseconds delay
            });

        } else {

            if (result.Message.length > 0) {

                let messages = result.Message.split(';');
                messages.forEach(msg => {
                    toastr.error(msg);
                });
            }
        }
    }
    catch (error) {
        alert("Error: " + error.message);
    }
}
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
                data: "ApplyFor",
                name: "Categery",
            },
            {
                data: "LotNo",
                name: "LotNo",
            },
            {
                data: "ToUnit",
                name: "ToUnit",
                orderable: false,
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
                name: "Sender Remark",
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
                    let ClaimValue = parseInt($("#spnClaimValue").html());
                    let Action = `<div class='d-flex'><button type='button' class='cls-btnDialog btn btn-icon btn-round btn-primary mr-1'><i class='fa fa-eye'></i></button>`;
                    if (data == false && row.Step == 1 && (ClaimValue == 2 || ClaimValue == 3)) {
                        return Action += `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></div>`;
                    }
                    else if (data == false && row.Step == 2 && ClaimValue == 0) {
                        return Action += `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></div>`;
                    }
                    else {
                        return Action += `NA</div>`;
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
                if (rowData.DispatchCardId != null) {
                    $("#spnDispatchCardId").html(rowData.DispatchCardId);
                    $("#DispatchInDialog").modal('show');
                }
                else {
                    $("#spnDispatchCardId").html(0);
                }
            });
            $("#tbldata tbody").off("click", ".cls-btnDialog").on("click", ".cls-btnDialog", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.DispatchCardId != null) {
                    $("#tbldatadialog").DataTable().destroy();
                    $("#lblModelTitle").html('Dispatch Card Lot details');
                    $("#DataTableDialog").modal('show');
                    BindDialog(rowData.DispatchCardId, rowData.ApplyForId);
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
                    $("#MessageDialogLabel").html('Remark');
                    $("#MessageDialogBody").html(rowData.ToRemark);
                    $("#MessageDialog").modal('show');
                }
            });

        }
    });
}

function BindDialog(DispatchCardId, ApplyForId) {
    table2 = $("#tbldatadialog").DataTable({
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
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                DispatchCardId: DispatchCardId
            };
            try {
                let response = await fetch("/BasicDetail/GetDispatchCardDataForDialog", {
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
                title: "ORO",
                data: "RecordOfficeName",
                name: "RecordOfficeName"
            },
            {
                title: "Regt",
                data: "RegimentalName",
                name: "RegimentalName"
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
                title: "Rank & Name",
                data: null,
                name: null,
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    return (fullName);
                }
            },
            {
                title: "Card Serial No",
                data: "CardSerialNo",
                name: "CardSerialNo"
            },
            {
                title: "Chip No",
                data: "ChipNo",
                name: "ChipNo"
            },
        ],
        initComplete: function () {
            // Hide or show column (e.g. salary column at index 3)
            if (parseInt(ApplyForId) == 1) {
                table2.column(4).visible(true); 
                table2.column(5).visible(false); 
            } else {
                table2.column(4).visible(false); 
                table2.column(5).visible(true)
            }
        },
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Service No" // Add custom placeholder
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
                title: 'E-IASC_DispathCard',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }]
    });
}