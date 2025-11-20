var table; // Declare table variable outside the function to preserve the instance
//var table2;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    let cvalue = parseInt($("#spnClaimValue").html());
    BindData(cvalue, function () {
    });

    $("#searchText").autocomplete({
        source: function (request, response) {
            if (cvalue === 2 || cvalue === 3) {
                if (request.term.length > 2) {
                    $("#spnUnitMapId").html('');
                    var param = { "UnitName": request.term };
                    $("#spnUnitMapId").html(0);

                    // Use fetch instead of jQuery AJAX
                    fetch('/Master/GetALLByUnitName', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': globalThis.RequestVerificationToken
                        },
                        body: new URLSearchParams(param) // Send data in URL encoded format
                    })
                        .then(response => response.json())  // Parse the JSON response
                        .then(data => {
                            if (data.length != 0) {
                                response($.map(data, function (item) {
                                    $("#loading").addClass("d-none");
                                    return {
                                        label: `${item.Sus_no}${item.Suffix} ${item.UnitName}`,
                                        value: `${item.UnitMapId}`
                                    };
                                }));
                            }
                            else {
                                $("#searchText").val("");
                                $("#spnUnitMapId").html("");
                                alert("Unit not found.");
                            }
                        })
                        .catch(error => {
                            console.error("Error fetching data:", error);
                            alert("Error fetching data: " + error);
                        });
                }
            } 
        },
        select: function (e, i) {
            e.preventDefault();
            $("#searchText").val(i.item.label);
            $("#spnUnitMapId").html(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

    if ($('#btnAdd').length) {
        $("#btnAdd").on("click", function () {
            location.href = '/BasicDetail/DispatchOut';
        });
    }
    $("#exportLot").on("click", function () {
        if (globalThis.selectedIds.length == 0) {
            toastr.error('Please Select at least one row.');
            return;
        }
        else {
            ExportCsvFile();
        }
    });

    $("#exportRequestIds").on("click", function () {
        if (globalThis.selectedIds.length == 0) {
            toastr.error('Please Select at least one row.');
            return;
        }
        else {
            ExportCsvFile();
        }
    });

    $("#btnProceedToDispatch").on("click", function () {
        if (globalThis.selectedIds.length == 0) {
            toastr.error('Please Select at least one row.');
            return;
        }
        else {
            let searchField = $("#searchField").val();
            let searchText = (cvalue == 2 || cvalue == 3) ? $("#spnUnitMapId").html() : $('#searchText').val().trim();

            if (searchField == null) {
                toastr.error('Please Select Field.');
            }
            else if (searchText == null || searchText.trim() === "" ) {
                toastr.error('Please Valid Search Input.');
            }
            else {
                ProceedToDispatch(searchField, searchText);
            }
        }
    });

    $('#DataTableDialog').on('hidden.bs.modal', function () {
        resetSelectedFields(); // Reset selected fields when the modal is closed
    });

    $('#DataTableDialogForLot').on('hidden.bs.modal', function () {
        // Reset global variables as explained
        globalThis.selectedIds = [];
        globalThis.previousSearchText = "";
        globalThis.isFirstSelectAll = true;
        globalThis.searchChanged = false;
        globalThis.globalAllChecked = false;

        // Uncheck all checkboxes
        $('#tbldatadialogLot tbody input[type="checkbox"].chkRequestId').prop('checked', false);

        // Reset "Select All" checkbox
        $('#chkAll_BindDialog').prop('checked', false);

        console.log("Reset selectedIds and checkboxes.");
    });

    $("#btnDispatchStatus").on("click", function () {
        $("#lblModelTitle").html('Details of Card Dispatch Status');

        $("#AdvSearch").removeClass("d-none");
        // Show the modal first
        $("#DataTableDialog").modal("show");

        // Then initialize the table
        DispatchCardStatusListBindDialog(cvalue,function () {
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
        if ($("#TermsConditions").prop("checked")) {
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

        }
        else {
            toastr.error('Please accept the Terms and Conditions');
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

            selectedField === 'regimentalname'? mMsater(0, "searchText", AllRegimental, "") : mMsater(0, "searchText", ORO, "");

        }
    });
});
function resetSelectedFields() {
    // Reset global variables as explained
    globalThis.selectedIds = [];
    globalThis.previousSearchText = "";
    globalThis.previousSearchField = "";
    globalThis.isFirstSelectAll = true;
    globalThis.searchChanged = false;
    globalThis.globalAllChecked = false;

    // Uncheck all checkboxes
    $('#tbldatadialog tbody input[type="checkbox"].chkRequestId').prop('checked', false);

    // Reset "Select All" checkbox
    $('#chkAll').prop('checked', false);

    $('#searchText').val('');
    $('#searchField').val([]).trigger('change');

    console.log("Reset selectedIds and checkboxes.");
}

async function Save() {
    try {
        let formData = new FormData();
        let DispatchCardId = parseInt($("#spnDispatchCardId").html());

        formData.append('DispatchCardId', DispatchCardId);
        formData.append('ToRemark', $("#txtToRemark").val());


        const response = await fetch('/BasicDetail/DispatchCardIn', {
            method: 'POST',
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
function BindData(cvalue, callback) {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        $("#tbldata").DataTable().destroy();
        $("#tbldata").empty(); // Clear old thead/tbody
    }
    const columns = getColumnsForDispatchCard(cvalue);
    table = $("#tbldata").DataTable({
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        responsive: true,
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
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
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
            searchPlaceholder: "Search" // Add custom placeholder
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
            if (cvalue == 1) {
                searchBox.attr('title', 'Search Cat/SUS NO/Lot No/ORO/Reg');
            }
            else if (cvalue == 2 || cvalue == 3) {
                searchBox.attr('title', 'Search In/Out/LotNo/SUS NO');
            }
            else {
                searchBox.attr('title', 'Search Cat/SUS NO/Lot No/ORO/Reg');
            }
            
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
                    $("#lblModelTitleLot").html('Dispatch Card Lot details');
                    let summary = '';
                    if (cvalue == 1) {
                        summary =
                            `<strong>Unit & SUS No : </strong> ${rowData.ToUnit} ${rowData.ToSUSNo} |
                             ${rowData.ApplyForId == 1 ? `<strong>ORO : </strong> ${rowData.RecordOfficeName}` : `<strong>Regiment : </strong> ${rowData.RegimentalName}`} |
                             <strong>Name Of Courier Incharge : </strong> ${rowData.NameOfCourierIncharge} |
                             <strong>Dispatch To : </strong > ${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo)? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo} |
                             <strong>Sender Remark : </strong> ${rowData.FromRemark} |
                             <strong>Receiver Remark : </strong> ${rowData.ToRemark != null ? rowData.ToRemark : ''} 
                    `;
                    }
                    else if (cvalue == 2 || cvalue == 3) {
                        summary =
                            `<strong>Unit & SUS No : </strong> ${rowData.Step == 1 ? `${rowData.FromUnit} ${rowData.FromSUSNo }` : `${ rowData.ToUnit } ${ rowData.ToSUSNo }`} |
                             <strong>Name Of Courier Incharge : </strong> ${rowData.NameOfCourierIncharge} |
                             ${rowData.Step == 1 ?
                                `<strong>Dispatch From : </strong >${`${rowData.FromDID} (${rowData.FromRankName} ${rowData.FromName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.FromServiceNo) ? `${rowData.FromServiceNo.slice(0, 2)}  ${rowData.FromServiceNo.slice(2)}` : rowData.FromServiceNo} |`
                                :`<strong>Dispatch To : </strong >${`${rowData.ToDID} (${rowData.ToRankName} ${rowData.ToName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.ToServiceNo) ? `${rowData.ToServiceNo.slice(0, 2)}  ${rowData.ToServiceNo.slice(2)}` : rowData.ToServiceNo} |`
                              } 
                             <strong>Sender Remark : </strong> ${rowData.FromRemark} |
                             <strong>Receiver Remark : </strong> ${rowData.ToRemark != null ? rowData.ToRemark : ''} 
                    `;
                    }
                    else {
                        summary =
                            `<strong>Unit & SUS No : </strong> ${rowData.ToUnit} ${rowData.ToSUSNo} |
                             ${rowData.ApplyForId == 1 ? `<strong>ORO : </strong> ${rowData.RecordOfficeName}` : `<strong>Regiment : </strong> ${rowData.RegimentalName}`} |
                             <strong>Name Of Courier Incharge :</strong> ${rowData.NameOfCourierIncharge} |
                             <strong>Dispatch From : </strong > ${`${rowData.FromDID} (${rowData.FromRankName} ${rowData.FromName})`.trim()} ${/^[A-Za-z]{2}/.test(rowData.FromServiceNo) ? `${rowData.FromServiceNo.slice(0, 2)}  ${rowData.FromServiceNo.slice(2)}` : rowData.FromServiceNo} |
                             <strong>Sender Remark : </strong> ${rowData.FromRemark} |
                             <strong>Receiver Remark : </strong> ${rowData.ToRemark != null ? rowData.ToRemark : ''} 
                    `;
                    }
                    $("#LotDetails").html(summary);
                    BindDialog(rowData, cvalue, function () {
                        $("#DataTableDialogForLot").modal("show");
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
function BindDialog(rowData, cvalue, callback) {
    var table2 = "";
    globalThis.selectedIds = [];
    if ($.fn.DataTable.isDataTable("#tbldatadialogLot")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatadialogLot").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatadialogLot thead").empty(); // Clear old thead
        $("#tbldatadialogLot tbody").empty(); // Clear old tbody
    }

    const columns = getColumnsByChoice(cvalue);
    table2 = $("#tbldatadialogLot").DataTable({
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let searchStatus = getSearchStatusForBindDialog(data.search.value);

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
                DispatchCardId: rowData.DispatchCardId,
                searchTextChanged: searchStatus.searchChanged,
                AllChecked: shouldFetchSelectedIds ? true : globalThis.globalAllChecked
            };
            try {
                let response = await fetch("/BasicDetail/GetDispatchCardDataForDialog", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    globalThis.selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }

                // Only update selectedIds if server returns new ones
                if (shouldFetchSelectedIds) {
                    if (result.selectedIds != null && result.selectedIds.length > 0) {
                        //selectedIds = result.selectedIds;
                        globalThis.selectedIds = result.selectedIds.map(x => x.toString());
                        console.log("Fetched selectedIds from server:", globalThis.selectedIds);
                        // If user hadn’t checked Select All, now we just load into selectedIds silently
                        if (globalThis.globalAllChecked) globalThis.isFirstSelectAll = false;
                    }
                    else {
                        //selectedIds = [];
                        if (globalThis.globalAllChecked) {
                            globalThis.globalAllChecked = false;
                            $('#chkAll_BindDialog').prop('checked', false);
                        }
                        console.warn("⚠️ No valid Pending IDs found.");
                    }
                }

                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
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
            searchBox.attr('title', 'Search Appl Id/Arm/Army No/Chip No/Card Serial No');
        },
        drawCallback: function (settings) {
            updateUICheckboxes('#tbldatadialogLot', 'chkRequestId', '#chkAll_BindDialog');
        }
    });
    $(document).on('change', '.chkRequestId', async function () {
        await updateSelectedIds('#tbldatadialogLot', 'chkRequestId');
        updateUICheckboxes('#tbldatadialogLot', 'chkRequestId', '#chkAll_BindDialog'); // Sync master checkbox state
    });
    $('#chkAll_BindDialog').on('change', function () {
        globalThis.selectedIds = [];
        globalThis.globalAllChecked = $(this).prop('checked');
        if (globalThis.globalAllChecked) {
            globalThis.isFirstSelectAll = true; // Force fresh fetch
        }
        table2.ajax.reload();
    });
}
function getColumnsForDispatchCard(choice) {
    let columns = [];
    switch (choice) {
        case 1:
            columns = [
                // Serial number column
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
                    title: "Category",
                    data: "ApplyFor",
                    name: "Categery",
                },
                {
                    title: "Lot No",
                    data: "DispatchCardId",
                    name: "DispatchCardId",
                },
                {
                    title: "Unit",
                    data: "ToUnit",
                    name: "ToUnit",
                    orderable: false,
                },
                {
                    title: "SUS No",
                    data: "ToSUSNo",
                    name: "ToSUSNo",
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
                    title: "Reg",
                    data: "RegimentalName",
                    name: "RegimentalName",
                    render: function (data, type, row) {
                        return (data??"");
                    }
                },
                {
                    title: "Dispatch On",
                    data: "DispatchDate",
                    name: "Dispatch On",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Received On",
                    data: "ReceiptDate",
                    name: "Dispatch In",
                    render: function (data, type, row) {
                        return data != null ? DateFormateddMMyyyyhhmmss(data) : "";
                    }
                },
                // Additional column for Edit action
                {
                    title: "Action",
                    data: "IsComplete",
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        let Action = `<div class='d-flex'><button type='button' class='cls-btnDialog btn btn-icon btn-round btn-primary mr-1'><i class='fa fa-eye'></i></button>`;
                        return Action;
                    }
                }
            ];
            break;
        case 2:
        case 3:
            columns = [
                // Serial number column
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
                    title: "In / Out",
                    data: "Step",
                    name: "Step",
                    render: function (data, type, row) {
                        return data == 1 ? "In" : "Out";
                    }
                },
                {
                    title: "Lot No",
                    data: "DispatchCardId",
                    name: "DispatchCardId",
                },
                {
                    title: "Unit",
                    data: null,
                    name: "ToUnit",
                    orderable: false,
                    render: function (data, type, row) {
                        return row.Step == 1 ? row.FromUnit : row.ToUnit;
                    }
                },
                {
                    title: "To SUS No",
                    data: "ToSUSNo",
                    name: "ToSUSNo",
                    render: function (data, type, row) {
                        return row.Step == 2 ? data : "";
                    }
                },
                {
                    title: "From SUS No",
                    data: "FromSUSNo",
                    name: "FromSUSNo",
                    render: function (data, type, row) {
                        return row.Step == 1 ? data : "";
                    }
                },
                {
                    title: "Dispatch On",
                    data: "DispatchDate",
                    name: "Dispatch On",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Received On",
                    data: "ReceiptDate",
                    name: "Dispatch In",
                    render: function (data, type, row) {
                        return data != null ? DateFormateddMMyyyyhhmmss(data) : "";
                    }
                },
                // Additional column for Edit action
                {
                    title: "Action",
                    data: "IsComplete",
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        let Action = `<div class='d-flex'><button type='button' class='cls-btnDialog btn btn-icon btn-round btn-primary mr-1'><i class='fa fa-eye'></i></button>`;
                        if (data == false && row.Step == 1) {
                            return Action += `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></div>`;
                        }
                        else {
                            return Action += `</div>`;
                        }
                    }
                }
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
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Category",
                    data: "ApplyFor",
                    name: "Categery",
                },
                {
                    title: "Lot No",
                    data: "DispatchCardId",
                    name: "DispatchCardId",
                },
                {
                    title: "Unit",
                    data: "FromUnit",
                    name: "FromUnit",
                    orderable: false,
                },
                {
                    title: "SUS No",
                    data: "FromSUSNo",
                    name: "FromSUSNo",
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
                    title: "Reg",
                    data: "RegimentalName",
                    name: "RegimentalName",
                    render: function (data, type, row) {
                        return (data ?? "");
                    }
                },
                {
                    title: "Dispatch On",
                    data: "DispatchDate",
                    name: "Dispatch On",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "Received On",
                    data: "ReceiptDate",
                    name: "Dispatch In",
                    render: function (data, type, row) {
                        return data != null ? DateFormateddMMyyyyhhmmss(data) : "";
                    }
                },
                // Additional column for Edit action
                {
                    title: "Action",
                    data: "IsComplete",
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        let Action = `<div class='d-flex'><button type='button' class='cls-btnDialog btn btn-icon btn-round btn-primary mr-1'><i class='fa fa-eye'></i></button>`;
                        if (data == false && row.Step == 2) {
                            return Action += `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></div>`;
                        }
                        else {
                            return Action += `</div>`;
                        }
                    }
                }
            ];
    }
    return columns;
}
function getColumnsByChoice(choice) {
    let columns = [];

    switch (choice) {
        case 1:
            columns = [
                {
                    title: `<div class="noExport wd-30-f"><div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll_BindDialog">
                    <label class="custom-control-label" for="chkAll_BindDialog"></label>
                    </div></div>`,
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll_BindDialog").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        }

                    }
                },
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
                    title: "Appl Id",
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
            ];
            break;

        case 2:
        case 3:
            columns = [
                {
                    title: `<div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll_BindDialog">
                    <label class="custom-control-label" for="chkAll_BindDialog"></label>
                    </div>`,
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll_BindDialog").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        }

                    }
                },
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
                    title: "Appl Id",
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

            ];
            break;

        default:
            columns = [
                {
                    title: `<div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll_BindDialog">
                    <label class="custom-control-label" for="chkAll_BindDialog"></label>
                    </div>`,
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll_BindDialog").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        }

                    }
                },
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
                    title: "Appl Id",
                    data: 'RequestId',
                    name: 'RequestId',
                },
                {
                    title: "Arm / Service",
                    data: "ArmedAbbreviation",
                    name: "ArmedAbbreviation"
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

            ];
    }

    return columns;
}
function DispatchCardStatusListBindDialog(cvalue, callback) {
    var table2;
    globalThis.selectedIds = [];
    if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
        // Destroy the DataTable and clear the table content
        $("#tbldatadialog").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldatadialog thead").empty(); // Clear old thead
        $("#tbldatadialog tbody").empty(); // Clear old tbody
    }
    let columns = getColumnsForListBindDialog(cvalue);
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

            let searchStatus = getSearchStatus(cvalue);

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
                //searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
                searchField: searchStatus.currentSearchField,
                searchText: searchStatus.currentSearchText,
                searchTextChanged: searchStatus.searchChanged,
                AllChecked: shouldFetchSelectedIds ? true : globalThis.globalAllChecked
            };
            try {
                let response = await fetch("/BasicDetail/GetDispatchCardStatusListForDialog", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    }, // Change Content-Type to JSON
                    body: JSON.stringify(requestData) // Send data as JSON
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();

                // 🔁 If no data returned, always clear selection
                if (result.data.length === 0) {
                    globalThis.selectedIds = [];
                    console.log("No results. Cleared selectedIds.");
                }

                // Only update selectedIds if server returns new ones
                if (shouldFetchSelectedIds) {
                    if (result.selectedIds != null && result.selectedIds.length > 0) {
                        //selectedIds = result.selectedIds;
                        globalThis.selectedIds = result.selectedIds.map(x => x.toString());
                        console.log("Fetched selectedIds from server:", globalThis.selectedIds);
                        // If user hadn’t checked Select All, now we just load into selectedIds silently
                        if (globalThis.globalAllChecked) globalThis.isFirstSelectAll = false;
                    }
                    else {
                        //selectedIds = [];
                        if (globalThis.globalAllChecked) {
                            globalThis.globalAllChecked = false;
                            $('#chkAll').prop('checked', false);
                        }
                        console.warn("⚠️ No valid Pending IDs found.");
                    }
                }

                callback(result); // Sends data to DataTables
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
            updateUICheckboxes('#tbldatadialog', 'chkRequestId', '#chkAll');
        }
    });

    $('#btnSearch').on('click', function () {
        // Get search field and search text values
        const searchField = $('#searchField').val()?.trim();
        const searchText = $('#searchText').val()?.trim();

        // If no search field is selected or no search text entered, prevent the search
        if (!searchField || !searchText) {
            toastr.error('Please select a search field and enter a value.');
            return; // Exit the function and prevent table reload
        }

        // Proceed with table reload if search criteria is valid
        table2.ajax.reload();
    });
    $('#btnClear').on('click', function () {
        $('#searchText').val('');
        $('#searchField').val([]).trigger('change');

        table2.ajax.reload();
    });
    $(document).on('change', '.chkRequestId', async function () {
        await updateSelectedIds('#tbldatadialog', 'chkRequestId');
        updateUICheckboxes('#tbldatadialog', 'chkRequestId', '#chkAll'); // Sync master checkbox state
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
    $('#chkAll').on('change', function () {
        globalThis.selectedIds = [];
        globalThis.globalAllChecked = $(this).prop('checked');
        if (globalThis.globalAllChecked) {
            globalThis.isFirstSelectAll = true; // Force fresh fetch
        }
        table2.ajax.reload();
    });
}
function getColumnsForListBindDialog(choice) {
    let columns = [];

    switch (choice) {
        case 2:
        case 3:
            columns = [
                {
                    title: `<div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll">
                    <label class="custom-control-label" for="chkAll"></label>
                    </div>`,
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
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
                    title: "Category",
                    data: "ApplyForId",
                    name: "Category",
                    render: function (data, type, row) {
                        return (row.ApplyFor);
                    }
                },
                {
                    title: "Appl Id",
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
            ];
            break;
        default:
            columns = [
                {
                    title: `<div class="custom-control custom-checkbox small">
                    <input type="checkbox" class="custom-control-input" id="chkAll">
                    <label class="custom-control-label" for="chkAll"></label>
                    </div>`,
                    data: null,
                    name: "Id",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        if ($("#chkAll").prop('checked')) {
                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}" checked>
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
                        } else {

                            return `<div class="custom-control custom-checkbox small">
                                    <input type="checkbox" class="custom-control-input chkRequestId" id="${row.RequestId}" value="${row.RequestId}">
                                    <label class="custom-control-label" for="${row.RequestId}"></label>
                                </div>`;
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
                    title: "Category",
                    data: "ApplyForId",
                    name: "Category",
                    render: function (data, type, row) {
                        return (row.ApplyFor);
                    }
                },
                {
                    title: "Appl Id",
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
            ];

    }

    return columns;
}
function getSearchStatus(cvalue) {
    const currentSearchText = (cvalue == 2 || cvalue == 3) ? $("#spnUnitMapId").html() : (($('#searchText').val() ?? '').toString().trim());
    const currentSearchField = $('#searchField').val();

    // Ensure searchChanged is only true when the actual search field or text changes.
    // If currentSearchField is null (i.e., no field selected), treat it as no change.
    globalThis.searchChanged = (
        (currentSearchText !== globalThis.previousSearchText || currentSearchField !== globalThis.previousSearchField) &&
        (currentSearchField !== null)
    );

    // Update previous values after comparison
    globalThis.previousSearchText = currentSearchText;
    globalThis.previousSearchField = currentSearchField;

    return {
        searchChanged: globalThis.searchChanged,
        currentSearchText,
        currentSearchField
    };
}
function ExportCsvFile() {
    return new Promise((resolve, reject) => {
        const requestData = {
            RequestIds: globalThis.selectedIds  //Passing the JavaScript array directly
        };
        fetch('/BasicDetail/ExportCsvFileForDispatchCard', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: JSON.stringify(requestData), // Convert the request     data to JSON
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    window.location.href = "/" + "WriteReadData/DispatchExports/Temp/" + response.Value;
                    console.log("Export successful", response);
                    resolve(response);
                } else {
                    toastr.error("Export failed: " + response.Message);
                    reject(new Error(response.Message));
                }
            })
            .catch((error) => {
                console.error("Export failed:", error);
                reject(new Error("Export failed: " + error.message));
            });
    });
}
function ProceedToDispatch(searchField, searchText) {
    return new Promise((resolve, reject) => {
        const requestData = {
            RequestIds: globalThis.selectedIds,  //Passing the JavaScript array directly
            SearchField: searchField, // Pass the search field
            SearchText: searchText // Pass the search text
        };
        fetch('/BasicDetail/BeforeProceedToDispatchCheck', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Tell the server we are sending JSON
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: JSON.stringify(requestData), // Convert the request     data to JSON
        })
            .then(response => response.json())
            .then((response) => {
                if (response.Result === true) {
                    location.href = '/BasicDetail/DispatchOut';
                    resolve(response);
                } else {
                    toastr.error("Export failed: " + response.Message);
                    reject(new Error(response.Message));
                }
            })
            .catch((error) => {
                toastr.error("Proceed To Dispatch failed: " + response.Message);
                reject(new Error("Proceed To Dispatch: " + error.message));
            });
    });
}
function getSearchStatusForBindDialog(search) {
    const currentSearchText = search.trim();

    // Ensure searchChanged is only true when the actual search field or text changes.
    globalThis.searchChanged = (
        (currentSearchText !== globalThis.previousSearchText )
    );

    // Update previous values after comparison
    globalThis.previousSearchText = currentSearchText;

    return {
        searchChanged: globalThis.searchChanged,
        currentSearchText
    };
}