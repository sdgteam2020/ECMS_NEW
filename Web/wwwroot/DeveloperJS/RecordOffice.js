var table; // Declare table variable outside the function to preserve the instance
var RecordOfficeId = 0;
var UnitMapId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    mMsater(0, "ddlArmType", ArmyType, "");
    BindData()
    $("#btnAdd").on("click",function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewRecordOffice").modal('show');
    });
    $("#btnRecordOfficeReset").on("click", function () {
        Reset();
        ResetErrorMessage();
    });
    $("#btnRecordOfficeAdd").on("click", function () {
        Proceed()
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                UnitMapId = 0;
                var param = { "UnitName": request.term };
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.Sus_no + item.Suffix + ' ' + item.UnitName, value: item.UnitMapId };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            UnitMapId = 0;
                            $("#ddlTDMId").find("option").not(":first").remove();
                            $("#ddlTDMId").val("0");
                            alert("Unit not found.")
                        }
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            UnitMapId = i.item.value;
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetDDMappedForRecord',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

                success: function (response) {
                    if (response != "null" && response != null) {
                        if (response == InternalServerError) {
                            Swal.fire({
                                text: errormsg
                            });
                        }

                        else {

                            var listItemddl = "";

                            listItemddl += '<option value="0">Please Select</option>';

                            for (var i = 0; i < response.length; i++) {
                                listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                            }
                            $("#ddlTDMId").html(listItemddl);
                        }
                    }
                    else {
                        //Swal.fire({
                        //    text: "No data found Offrs"
                        //});
                    }
                },
                error: function (result) {
                    Swal.fire({
                        text: errormsg002
                    });
                }
            });
        },
        
    });

    $('#txtUnitName').on('keyup', function (e) {
        if (e.key === 'Delete' || e.key === 'Backspace') {
            $(this).val('');
            UnitMapId = 0;
            $('#ddlTDMId').find('option:not(:first)').remove();
            $('#ddlTDMId').val('0');
        }
    });
});

function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveRecordOffice';
    $.validator.unobtrusive.parse($(formId));

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
function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
    }
    const columns = getColumnsForRecordOffice();
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
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {

            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search?.value || '',  // ✅ Safe access
                sortColumn: data.order?.[0]?.column >= 0 && data.columns?.[data.order[0].column]?.data || '',
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
            };
            try {
                let response = await fetch("/Master/GetAllRecordOffice_Pagination", {
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
        /* ===== FORCE WIDTHS (IMPORTANT) ===== */
        columnDefs: [
            {
                targets: 0,
                visible: false,
                width: "0px",
                searchable: false
            },
            { targets: 1, width: "60px" },
            { targets: 2, width: "200px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "200px" },
            { targets: 5, width: "200px" },
            { targets: 6, width: "120px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip",
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
                orientation: 'portrait',
                pageSize: 'A4',
                title: 'E-IASC_Regimental',
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
            searchBox.attr('title', 'Search Comd/Abbreviation');

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

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RecordOfficeId != null) {
                    Reset();
                    ResetErrorMessage();
                    $("#txtName").val(rowData.RecordOfficeName);
                    $("#txtAbbreviation").val(rowData.Abbreviation);
                    RecordOfficeId = rowData.RecordOfficeId;
                    $("#ddlArmType").val(rowData.ArmedId);
                    if (rowData.Message != null) {
                        $("#txtMessage").val(rowData.Message);
                    }
                    else {
                        $("#txtMessage").val("");
                    }
                    if (rowData.UnitId != null) {
                        UnitMapId = rowData.UnitId;
                        $("#txtUnitName").val(`${rowData.Sus_no}${rowData.Suffix} ${rowData.UnitName}`);
                    }
                    else {
                        UnitMapId = 0;
                        $("#txtUnitName").val("");
                    }
                    if (rowData.TDMId != null) {
                        GetDDMappedForRecord(rowData.UnitId, rowData.TDMId);
                    }
                    else {
                        $("#ddlTDMId").val("0");
                    }
                    $("#btnRecordOfficeAdd").val("Update");
                    $("#AddNewRecordOffice").modal('show');
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RecordOfficeId != null) {
                    Swal.fire({
                        title: 'Are you sure?',
                        text: "You want to Delete ",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#072697',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Yes, Delete It!'
                    }).then((result) => {
                        if (result.value) {
                            Delete(rowData.RecordOfficeId);
                        }
                    });
                }
                else {
                    //Invalid Data
                }
            });


        }
    });

    // Force hide the column
    table.column(0).visible(false);
}
function Save() {
    $.ajax({
        url: '/Master/SaveRecordOffice',
        type: 'POST',
        data: {
            "Name": $("#txtName").val().trim(),
            "Abbreviation": $("#txtAbbreviation").val().trim(),
            "ArmedId": $("#ddlArmType").val(),
            "RecordOfficeId": RecordOfficeId,
            "UnitId": UnitMapId == 0 ? null : UnitMapId,
            "TDMId": $("#ddlTDMId").val() == 0 ? null : $("#ddlTDMId").val(),
            "Message": $("#txtMessage").val().length > 0 ? $("#txtMessage").val():null,
        }, 
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {


            if (result == 5) {
                toastr.success('Record Office has been saved');

                $("#AddNewRecordOffice").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == 6) {
                toastr.success('Record Office has been Updated');

                $("#AddNewRecordOffice").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == 2) {
                toastr.error('Record Office / Abbreviation Name Exits!');
            }
            //else if (result == 3) {
            //    toastr.error('Armed Id Exits!');
            //}
            //else if (result == 4) {
            //    toastr.error('Domain Id Exits!');
            //}
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }


            }
        }
    });
}

function Reset() {
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#ddlArmType").val("0");
    $("#txtMessage").val("");
    $("#txtUnitName").val("");
    $("#ddlTDMId").val("0");
    RecordOfficeId = 0;
    UnitMapId = 0;
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#ddlArmType-error").html("");
    $("#txtMessage-error").html("");
    $("#txtUnitName-error").html("");
    $("#ddlTDMId-error").html("");
}

function GetDDMappedForRecord(UnitId,TDMId) {
    var param1 = { "UnitMapId": UnitId };
    $.ajax({
        url: '/Master/GetDDMappedForRecord',
        contentType: 'application/x-www-form-urlencoded',
        data: param1,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="0">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                    }
                    $("#ddlTDMId").html(listItemddl);
                    if (TDMId != '') {
                        $("#ddlTDMId").val(TDMId);
                    }
                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function Delete(Id) {
    var userdata =
    {
        "RecordOfficeId": Id,

    };
    $.ajax({
        url: '/Master/DeleteRecordOffice',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }

                else if (response == Success) {
                    //lol++;
                    //if (lol == Tot) {

                    toastr.success('Deleted Selected');
                    BindData();
                }

                //}
            }
            else {
                Swal.fire({
                    text: errormsg001
                });
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function getColumnsForRecordOffice() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "RecordOfficeId",
            name: "RecordOfficeId",
            visible: false,        // hidden
            searchable: false,
            width: "0px",
        },
        // Serial number column
        {
            title: "S No",
            data: null,
            name: "SerialNumber",
            orderable: false, // Disable sorting for this column
            className: "text-center col-sno",
            width: "60px",
            render: function (data, type, row, meta) {
                // Calculate serial number based on row index
                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        {
            title: "Record Office",
            data: "RecordOfficeName",
            name: "RecordOfficeName",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Abbreviation",
            data: "Abbreviation",
            name: "Abbreviation",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data.toUpperCase()}">${data.toUpperCase()}</span>`;
            }
        },
        {
            title: "Arms / Service",
            data: "ArmedName",
            name: "ArmedName",
            className: "text-center nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Linked Domain ID & Pers Details",
            data: "Name",
            name: "Name",
            className: "nowrap",
            width: "200px",
            orderable: false,
            render: function (data, type, row, meta) {
                if (row.TDMId != null) {
                    let name = `${row.DomainId} & ${row.ArmyNo} ${row.RankAbbreviation} ${row.Name}`;
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${name}">${name}</span>`;
                }
                else {
                    return ``;
                }
                
            }
        },
        // Additional column for Edit action
        {
            title: "Action",
            data: null,
            className: "noExport",
            name: "Action",
            orderable: false,
            className: "noExport text-center col-action",
            width: "200px",
            render: function (data, type, row) {
                if (row.ArmedId != $("#ArmedIdForORO").html()) {
                    let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>`;
                    return Action;
                }
                else {
                    return `<span class='badge rounded-pill bg-success'></span>`;
                }

            }
        }
    ];
    return columns;
}