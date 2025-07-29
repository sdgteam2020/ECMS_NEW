var table; // Declare table variable outside the function to preserve the instance
var table2;
var selectedIds = [];
var lastSearchValue = "";
var unchedRequestId = []; // label array
var checkedRequestId = []; // total array
$(function () {
    BindData();
    if ($('#btnAdd').length) {
        $("#btnAdd").on("click", function () {
            location.href = '/BasicDetail/DispatchOut';
        });
    }
    $("#export").on("click", function () {
        if (checkedRequestId.length == 0 && !$("#chkAll").prop('checked')) {
            toastr.error('Please Select at least one row.');
            return;
        }

        let chkAllstatus = false;
        if ($("#chkAll").prop('checked'))
        {
            chkAllstatus = true;
        }
      
        ExportCsvFile(chkAllstatus, checkedRequestId, unchedRequestId)
    });


        $("#btnDispatchStatus").on("click", function () {
        $("#lblModelTitle").html('Dispatch Card Status details');

        $("#AdvSearch").removeClass("d-none");
        // Show the modal first
        $("#DataTableDialog").modal("show");

        // Then initialize the table
        DispatchCardStatusListBindDialog(function () {
            // Callback to show modal after DataTable is ready
        });
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

    $('#searchField').on('change', function () {
        const selectedField = $(this).val();
        const $container = $('#searchInputContainer');

        if (selectedField === 'regimentalname' || selectedField === 'recordofficename') {
            // Replace input with a select dropdown
            let newSelect = $('<select id="searchText" class="form-control form-control-sm"></select>')
                .append('<option value="">Select...</option>');

            $container.html(newSelect); // Replace input with dropdown

            // Fetch data from server
            let url = (selectedField === 'regimentalname')
                ? mMsater(0, "searchText", AllRegimental, "")
                : mMsater(0, "searchText", ORO, "");

            //fetch(url)
            //    .then(res => res.json())
            //    .then(data => {
            //        data.forEach(item => {
            //            newSelect.append(`<option value="${item.value}">${item.label}</option>`);
            //        });
            //    });

        } else {
            // Revert to text input for 'susno' or blank
            let newInput = $('<input type="text" id="searchText" class="form-control-AllowedKey form-control-sm" placeholder="Search..." />');
            $container.html(newInput);
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
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
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
                filterApplyFor: $('#filterApplyFor').val(),
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
            searchPlaceholder: "Search Cat/LotNo/CIC/Army No" // Add custom placeholder
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
        initComplete: function () {
            // Add tooltip to the search input box
            let searchBox = $('div.dataTables_filter input');
            searchBox.attr('title', 'Search Cat/LotNo/CIC/Army No');
        },
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
                    $("#lblModelTitle").html('Dispatch Card Lot details');
                    BindDialog(rowData.DispatchCardId, rowData.ApplyForId, function () {
                        $("#DataTableDialog").modal("show");
                    })
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
    $('#filterApplyFor').on('keypress', function (e) {
        if (e.which === 13) {
            table.ajax.reload();
        }
    });
}
function BindDialog(DispatchCardId, ApplyForId, callback) {
    if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
        $("#tbldatadialog").DataTable().destroy();
        $("#tbldatadialog").empty(); // Clear old thead/tbody
    }
    const columns = getColumnsByChoice(ApplyForId);
    table2 = $("#tbldatadialog").DataTable({
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
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
        columns: columns,
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search ReqId/Arm/SUSNo/" // Add custom placeholder
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
            }],
        // 👇 Show modal only after table (header + data) is fully rendered
        initComplete: function () {
            if (typeof callback === "function") {
                callback(); // show modal now
            }
            // Add tooltip to the search input box
            let searchBox = $('div.dataTables_filter input');
            searchBox.attr('title', 'Search ReqId/Arm/SUSNo/Army No/Chip No/Card Serial No');
        }
    });
}
function getColumnsByChoice(choice) {
    let columns = [];

    switch (choice) {
        case 1:
            columns = [
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
                    orderable: false
                },
                {
                    title: "SUS No",
                    data: "SUSNo",
                    name: "SUSNo"
                },
                {
                    title: "ORO",
                    data: "RecordOfficeName",
                    name: "RecordOfficeName",
                    render: function (data, type, row) {
                        return (data ?? "");
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
            ];
            break;

        case 2:
            columns = [
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
                    orderable: false
                },
                {
                    title: "SUS No",
                    data: "SUSNo",
                    name: "SUSNo"
                },
                {
                    title: "Regt",
                    data: "RegimentalName",
                    name: "RegimentalName",
                    render: function (data, type, row) {
                        return (data ?? "");
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
            ];
            break;

        default:
            columns = [
                { title: "S No", data: null, orderable: false, render: (data, type, row, meta) => meta.row + meta.settings._iDisplayStart + 1 },
                { title: "ID", data: 'Id' },
                { title: "Description", data: 'Description' }
            ];
    }

    return columns;
}
function DispatchCardStatusListBindDialog(callback) {
    var table2;
    checkedRequestId = [];
    unchedRequestId = [];
    if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatadialog").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatadialog thead").empty(); // Clear old thead
        $("#tbldatadialog tbody").empty(); // Clear old tbody
    }
    let columns = [
        {
            title: `<div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll">
                    <label class="custom-control-label" for="chkAll"></label>
                    </div>`,
            data: null,
            name: "Id",
            orderable: false, // Disable sorting for this column
            render: function (data, type, row, meta) {
                if (row.Status == `Pending`) {
                    if ($("#chkAll").prop('checked')) {
                        return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                    } else {
                        //let checkedRequestId = ['7', '10000', '9999', '9998']
                    
                                
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                       
                       
                    }
                }
                else {
                    return `<div></div>`;
                }

            }
        },
        {
            title: 'S No',
            data: null,
            name: "SerialNumber",
            orderable: false, // Disable sorting for this column
            render: function (data, type, row, meta) {
                // Calculate serial number based on row index

                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        {
            title: "Categery",
            data: "ApplyForId",
            name: "Categery",
            render: function (data, type, row) {
                return (row.ApplyFor);
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
            orderable: false
        },
        {
            title: "SUS No",
            data: "SUSNo",
            name: "SUSNo"
        },
        {
            title: "ORO",
            data: "RecordOfficeName",
            name: "RecordOfficeName",
            render: function (data, type, row) {
                return (row.ApplyForId == 1 ? data : "");
            }
        },
        {
            title: "Regt",
            data: "RegimentalName",
            name: "RegimentalName",
            render: function (data, type, row) {
                return (row.ApplyForId == 2 ? data : "");
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
        {
            title: "Status",
            data: "StepId",
            name: "StepId",
            render: function (data, type, row) {
                let color;
                if (row.Status == `Pending`) {
                    color = 'danger';
                }
                else {
                    color = 'success';
                }
                return `<span class='badge badge-${color} mr-1' >${row.Status}</span></span>`;
            }
        },
    ];
    
    table2 = $("#tbldatadialog").DataTable({
        autoWidth: false, // Let us handle width via CSS
        searching: false,
        responsive: true, // Responsive breaks layout for width control
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            const currentSearchValue = searchText || '';

            // Reset selection if search term has changed
            if (currentSearchValue !== lastSearchValue) {
                checkedRequestId = [];
                lastSearchValue = currentSearchValue;
            }
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                //searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                searchField: $('#searchField').val(), // Field-based search
                searchText: $('#searchText').val(),
                AllChecked: $('#chkAll').is(':checked')
            };
            try {
                let response = await fetch("/BasicDetail/GetDispatchCardStatusListForDialog", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" }, // Change Content-Type to JSON
                    body: JSON.stringify(requestData) // Send data as JSON
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                callback(result); // Sends data to DataTables
                setTimeout(function () {
                    checkedRequestId.forEach(function (id) {
                        
                        $('#' + id).prop('checked', true);  // If checkbox IDs match
                    });
                }, 500); // wait 500ms before checking
                setTimeout(function () {
                    unchedRequestId.forEach(function (id) {

                        $('#' + id).prop('checked', false);  // If checkbox IDs match
                    });
                }, 500); // wait 500ms before checking

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search ReqId/Arm/SUSNo/ORO/Regt" // Add custom placeholder
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
            }],
        // 👇 Show modal only after table (header + data) is fully rendered
        initComplete: function () {
            if (typeof callback === "function") {
                callback(); // Show modal after DataTable is initialized
            }
            // Add tooltip to the search input box
            let searchBox = $('div.dataTables_filter input');
            searchBox.attr('title', 'Search ReqId/Arm/SUSNo/ORO/Regt/Army No/Chip No/Card Serial No');
        },
        drawCallback: function (settings) {
           // updateUICheckboxes();
        }
    });

    $('#btnSearch').on('click', function () {
        table2.ajax.reload();
    });
    $('#btnClear').on('click', function () {
        $('#searchText').val('');
        $('#searchField').val([]).trigger('change');
        table2.ajax.reload();
    });
    // Restrict typing
    $('#searchText').on('keypress', function (e) {
        let field = $('#searchField').val();
        let char = String.fromCharCode(e.which);

        if (e.which === 13) {
            table2.ajax.reload();
        }

        // Allow navigation keys
        if (e.ctrlKey || e.metaKey || e.altKey || e.which < 32) return;

        //if (field === "requestid") {
        //    // Allow only digits
        //    if (!/[0-9]/.test(char)) {
        //        e.preventDefault();
        //    }
        //} else if (["categery", "serviceno", "susno", "regimentalname", "recordofficename", "chipno", "cardserialno", "status"].includes(field)) {
        //    // Allow alphanumeric and space only, block special characters
        //    if (!/^[a-zA-Z0-9_/ ]$/.test(char)) {
        //        e.preventDefault();
        //    }
        //}
        if (["susno", "regimentalname", "recordofficename"].includes(field)) {
            // Allow alphanumeric and space only, block special characters
            if (!/^[a-zA-Z0-9_/ ]$/.test(char)) {
                e.preventDefault();
            }
        }
    });
    // Sanitize pasted value
    $('#searchText').on('input', function () {
        let field = $('#searchField').val();
        let currentVal = $(this).val();

        //if (field === "requestid") {
        //    // Allow digits only
        //    let cleaned = currentVal.replace(/[^0-9]/g, '');
        //    if (cleaned !== currentVal) {
        //        $(this).val(cleaned);
        //    }
        //} else if (["categery", "serviceno", "susno", "regimentalname", "recordofficename", "chipno", "cardserialno", "status"].includes(field)) {
        //    // Allow only letters, numbers, and space
        //    let cleaned = currentVal.replace(/[^a-zA-Z0-9_/ ]/g, '');
        //    if (cleaned !== currentVal) {
        //        $(this).val(cleaned);
        //    }
        //}
        if (["susno", "regimentalname", "recordofficename"].includes(field)) {
            // Allow only letters, numbers, and space
            let cleaned = currentVal.replace(/[^a-zA-Z0-9_/ ]/g, '');
            if (cleaned !== currentVal) {
                $(this).val(cleaned);
            }
        }
    });
    $(document).on('change', '.chkRequestId', function () {
        if ($(this).prop('checked')) {
            if (!$("#chkAll").prop('checked')) {
                checkedRequestId.push($(this).val())
            } else {
                checkedRequestId = [];
                unchedRequestId = [];
            }
             unchedRequestId.pop($(this).val())
        } else {

            checkedRequestId.pop($(this).val())
           // if ($("#chkAll").prop('checked'))
            unchedRequestId.push($(this).val())
        }
       
       
    });
    $('#chkAll').on('change', function () {
        checkedRequestId = [];
        unchedRequestId = [];
        if (!$("#chkAll").prop('checked')) {
            $(".chkRequestId").prop('checked', false)
        } else {
            $(".chkRequestId").prop('checked', true)
        }

        
    });
}
function ExportCsvFile(Allstatus, checkedRequestId, unchedRequestId) {
   // alert(Allstatus)
   // alert("checked---" + checkedRequestId)
   // alert("Unchecked---" + unchedRequestId)
    var userdata = {
        Allstatus: Allstatus,
        checkedRequestId: checkedRequestId,
        unchedRequestId: unchedRequestId
    };

    $.ajax({
        url: '/BasicDetail/ExportCsvForDispatch',
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: userdata,
        success: function (response) {
            // handle success
          // alert("WriteReadData/Dispatchexports/" + response)
            window.location.href = "/" + "WriteReadData/Dispatchexports/" + response;
            console.log("Export successful", response);
        },
        error: function (xhr, status, error) {
            console.error("Export failed:", error);
        }
    });

    
}