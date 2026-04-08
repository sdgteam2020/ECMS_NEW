var table; // Declare table variable outside the function to preserve the instance
var tabledialog; // Declare tabledialog variable outside the function to preserve the instance
let UnitMapId = 0;
let UnitType = 0;
let ApptId = 0;
let TDMId = 0;
let DomainRegId = 0;

$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData();
    AccountCount();
    BindClaims();
    BindRoles();

    $('#ddlRoles').select2({
        placeholder: "Select Role",
        width: '100%',
        dropdownParent: $('#AddNewDomain'),
        closeOnSelect: false
    });
    $('#ddClaims').select2({
        placeholder: "Select Claims",
        width: '100%',
        dropdownParent: $('#AddNewDomain'),
        closeOnSelect: false
    });


    $("#btnDomainAddDialog").on("click", function () {
        Proceed();
    });

    $("#AddNewDomain input[name='txtapproval']").on("click", function () {
        $("#txtapproval-error").html("");
    });
    $("#AddNewDomain input[name='txtactive']").on("click", function () {
        $("#txtactive-error").html("");
    });
    $("#AddNewDomain input[name='IntOffr']").on("click", function () {
        $("#IntOffr-error").html("");
    });

    $("#AddNewDomain input[name='InitatingOffr']").on("click", function () {
        $("#InitatingOffr-error").html("");
    });
    $("#AddNewDomain input[name='CommandingOffr']").on("click", function () {
        $("#CommandingOffr-error").html("");
    });
    //$("#AddNewDomain input[name='IsRO']").on("click", function () {
    //    $("#IsRO-error").html("");
    //});
    //$("#AddNewDomain input[name='IsORO']").on("click", function () {
    //    $("#IsORO-error").html("");
    //});

    $("#txtAppointmentName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                ApptId = 0;
                var param = { "AppointmentName": request.term };
                $.ajax({
                    url: '/Master/GetALLByAppointmentName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.AppointmentName, value: item.ApptId };

                            }))
                        }
                        else {
                            $("#txtAppointmentName").val("");
                            ApptId = 0;
                            alert("Appointment not found.")
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
            $("#txtAppointmentName").val(i.item.label);
            ApptId = i.item.value;
            return false;
        }
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            $("#lblSusno").html('');
            $("#lblPso").html('');
            $("#lblDG").html('');
            $("#lblComd").html('');
            $("#lblCorps").html('');
            $("#lblDiv").html('');
            $("#lblBde").html('');
            $("#lblFmn").html('');
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
                            UnitType = 0;
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
            var param1 = { "UnitMapId": encryptPayloadData(i.item.value) };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                success: function (data) {
                    UnitType = data.UnitType;
                    UnitMapId = data.UnitMapId;
                    $("#lblSusno").html(data.Sus_no + '' + data.Suffix);

                    if (data.UnitType == 1) {
                        $("#lblComd").html(data.ComdName);
                        $("#lblCorps").html(data.CorpsName);
                        $("#lblDiv").html(data.DivName);
                        $("#lblBde").html(data.BdeName);
                        $("#lbl1").addClass("d-none");
                        $("#lbl2").addClass("d-none");
                        $("#lbl3").removeClass("d-none");
                        $("#lbl4").removeClass("d-none");
                        $("#lbl5").removeClass("d-none");
                        $("#lbl6").removeClass("d-none");
                        $("#lbl7").addClass("d-none");
                    }
                    else if (data.UnitType == 2) {
                        $("#lblComd").html(data.ComdName);
                        $("#lblCorps").html(data.CorpsName);
                        $("#lblDiv").html(data.DivName);
                        $("#lblBde").html(data.BdeName);
                        $("#lblFmn").html(data.BranchName);
                        $("#lbl1").addClass("d-none");
                        $("#lbl2").addClass("d-none");
                        $("#lbl3").removeClass("d-none");
                        $("#lbl4").removeClass("d-none");
                        $("#lbl5").removeClass("d-none");
                        $("#lbl6").removeClass("d-none");
                        $("#lbl7").removeClass("d-none");
                    }
                    else if (data.UnitType == 3) {
                        $("#lblPso").html(data.PSOName);
                        $("#lblDG").html(data.SubDteName);
                        $("#lbl1").removeClass("d-none");
                        $("#lbl2").removeClass("d-none");
                        $("#lbl3").addClass("d-none");
                        $("#lbl4").addClass("d-none");
                        $("#lbl5").addClass("d-none");
                        $("#lbl6").addClass("d-none");
                        $("#lbl7").addClass("d-none");
                    }



                }
            });
        },
        
    });

    $('#txtUnitName').on("keyup", function (e) {
        if (e.which == 46) {
            UnitMapId = 0;
            $("#txtUnitName").val("");
            UnitType = 0;

            $("#lblSusno").html('');
            $("#lblPso").html('');
            $("#lblDG").html('');
            $("#lblComd").html('');
            $("#lblCorps").html('');
            $("#lblDiv").html('');
            $("#lblBde").html('');
            $("#lblFmn").html('');
        }
    });

    $("#btnDomainAdd").on("click", function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewDomain").modal('show');
    });

    $("#btnDomainAddReset").on("click", function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtSearch").on("keyup", function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            if ($("input[type='radio'][name=choice]:checked").val() == "Id") {
                var num_val = parseInt(eThis.val());
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData()
                }
            }
            else {
                BindData()
            }
        }
        else {
            alert("Select Choice");
        }
    });

    $("#btnUser").on("click", function () {

        if ($("#lblUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Users');
            $("#DataTableDialog").modal('show');
            BindDialog("User");
        }
    });

    $("#btnMappedUser").on("click", function () {

        if ($("#lblMappedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Mapped Users');
            $("#DataTableDialog").modal('show');
            BindDialog("MappedUser");
        }
    });

    $("#btnUnMappedUser").on("click", function () {
        if ($("#lblUnMappedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total UnMapped Users');
            $("#DataTableDialog").modal('show');
            BindDialog("UnMappedUser");
        }
    });

    $("#btnActiveUser").on("click", function () {
        if ($("#lblActiveUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Active Users');
            $("#DataTableDialog").modal('show');
            BindDialog("ActiveUser");
        }
    });

    $("#btnInActiveUser").on("click", function () {
        if ($("#lblInActiveUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total InActive Users');
            $("#DataTableDialog").modal('show');
            BindDialog("InActiveUser");
        }
    });

    $("#btnVerified").on("click", function () {
        if ($("#lblVerifiedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("Verified");
        }
    });

    $("#btnNotVerifiedUser").on("click", function () {
        if ($("#lblNotVerifiedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("NotVerifiedUser");
        }
    });

    $("#btnIO").on("click", function () {
        if ($("#lblIO").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("IO");
        }
        else {
            BindData("IO");
        }
    });

    $("#btnApprover").on("click", function () {
        if ($("#lblApprover").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("CO");

        }
    });

    //$("#btnRO").on("click", function () {
    //    if ($("#lblRO").html() > 0) {
    //        $("#tbldatadialog").DataTable().destroy();
    //        $("#lblModelTitle").html('Total Not Verified Users');
    //        $("#DataTableDialog").modal('show');
    //        BindDialog("RO");
    //    }
    //});
    //$("#btnORO").on("click", function () {
    //    if ($("#lblORO").html() > 0) {
    //        $("#tbldatadialog").DataTable().destroy();
    //        $("#lblModelTitle").html('Total Not Verified Users');
    //        $("#DataTableDialog").modal('show');
    //        BindDialog("ORO");
    //    }
    //});
});

function BindDialog(Choice) {
    // STEP 1: Move ALL DataTable code into shown.bs.modal
    $("#DataTableDialog").one('shown.bs.modal', function () {
        if ($.fn.DataTable.isDataTable("#tbldatadialog")) {
            // Destroy the DataTable and clear the table content
            $("#tbldatadialog").DataTable().clear().destroy(); // Clear and destroy DataTable properly
            $("#tbldatadialog thead").empty(); // Clear old thead
            $("#tbldatadialog tbody").empty(); // Clear old tbody
        }

        tabledialog = $("#tbldatadialog").DataTable({
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
            ajax: {
                url: "/Account/GetDataForDataTable",
                contentType: 'application/x-www-form-urlencoded',
                type: "POST",
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                data: function (d) {
                    d.draw = d.draw;
                    d.start = d.start;
                    d.length = d.length;
                    d.searchValue = d.search.value;
                    d.sortColumn = d.columns[d.order[0].column].data;
                    d.sortDirection = d.order[0].dir;
                    d.Choice = Choice;
                },
            },
            columns: [
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
                    title: "Reg Id",
                    data: "Id",
                    name: "Id",
                    className: "nowrap",
                    width: "120px",
                },
                {
                    title: "Domain Id",
                    data: "DomainId",
                    name: "DomainId",
                    className: "nowrap",
                    width: "180px",
                    render: function (data, type, row, meta) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "IC No",
                    data: "ArmyNo",
                    name: "ArmyNo",
                    className: "nowrap",
                    width: "120px",
                },
                {
                    title: "Role",
                    data: "RoleNames",
                    name: "RoleNames",
                    orderable: false, // Disable sorting for this column
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row) {
                        return data ? data.join(', ') : '';  // Convert array to string
                    }
                },
                {
                    title: "Requested Genr On (DT)",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    className: "text-wrap requested-generated-col",
                    width: "150px",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    },
                },
                // Display user-friendly value for Mapped
                {
                    title: "Mapping",
                    data: "Mapped",
                    name: "Mapped",
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                // Display user-friendly value for AdminFlag
                {
                    title: "Approval",
                    data: "AdminFlag",
                    name: "AdminFlag",
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                // Display user-friendly value for Active
                {
                    title: "Active",
                    data: "Active",
                    name: "Active",
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                // Display user-friendly value for IsIO
                {
                    title: "IO",
                    data: "IsIO",
                    name: "IsIO",
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                // Display user-friendly value for IsCO
                {
                    title: "Approver",
                    data: "IsCO",
                    name: "IsCO",
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                //{ data: "IsRO", name: "IsRO" },
                //{ data: "IsORO", name: "IsORO" }
            ],
            /* ===== FORCE WIDTHS (IMPORTANT) ===== */
            columnDefs: [
                { targets: 0, width: "60px", },
                { targets: 1, width: "120px" },
                { targets: 2, width: "180px" },
                { targets: 3, width: "120px" },
                { targets: 4, width: "220px" },
                { targets: 5, width: "100px" },
                { targets: 6, width: "100px" },
                { targets: 7, width: "100px" },
                { targets: 8, width: "100px" },
                { targets: 9, width: "100px" },
                { targets: 10, width: "100px" },
                {
                    targets: '_all',  // Apply to all visible columns
                    orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
                },
            ],
            language: {
                search: "", // Remove the default "Search:" label
                searchPlaceholder: "Search Domain ID" // Add custom placeholder
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
                    title: 'E-IASC_' + $("#lblModelTitle").html(),
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
                searchBox.attr('title', 'Search Domain ID');

                // Force DataTables to calculate optimal widths
                this.api().columns.adjust();

                // Handle zoom/resize
                var resizeTimer;
                $(window).on('resize', function () {
                    clearTimeout(resizeTimer);
                    resizeTimer = setTimeout(function () {
                        tabledialog.columns.adjust().responsive.recalc();
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
            }
        });
    });

    // STEP 2: Show modal (this triggers the above)
    $("#DataTableDialog").modal("show");
}
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveDomain';
    ValidateInput();
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
function ValidateInput() {
    if ($("input[type='radio'][name=txtapproval]:checked").length == 0) {
        $("#txtapproval-error").html("Approval is required.");
    }
    else {
        $("#txtapproval-error").html("");
    }

    if ($("input[type='radio'][name=txtactive]:checked").length == 0) {
        $("#txtactive-error").html("Active is required.");
    }
    else {
        $("#txtactive-error").html("");
    }

    if ($("input[type='radio'][name=InitatingOffr]:checked").length == 0) {
        $("#InitatingOffr-error").html("Initating Offr is required.");
    }
    else {
        $("#InitatingOffr-error").html("");
    }

    if ($("input[type='radio'][name=CommandingOffr]:checked").length == 0) {
        $("#CommandingOffr-error").html("Commanding Offr is required.");
    }
    else {
        $("#CommandingOffr-error").html("");
    }

    //if ($("input[type='radio'][name=IsRO]:checked").length == 0) {
    //    $("#IsRO-error").html("Record Office is required.");
    //}
    //else {
    //    $("#IsRO-error").html("");
    //}

    //if ($("input[type='radio'][name=IsORO]:checked").length == 0) {
    //    $("#IsORO-error").html("Officer Record Office is required.");
    //}
    //else {
    //    $("#IsORO-error").html("");
    //}

    if ((ApptId == 0 ) && $("#txtAppointmentName").val().length > 0) {
        $("#txtAppointmentName").val('');
        $("#txtAppointmentName-error").html("Appointment name is invalid.");
        toastr.error('Appointment name is invalid.');
    }

    if ((UnitMapId == 0 ) && $("#txtUnitName").val().length > 0) {
        $("#txtUnitName").val('');
        $("#txtAppointmentName-error").html("Unit name is invalid.");
        toastr.error('Unit name is invalid.');
    }
}

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
                let response = await fetch("/Account/GetAllDomainRegn", {
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
        columns: [
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
                title: "Registered ID",
                data: "Id",
                name: "Id",
                className: "nowrap",
                width: "120px",
            },
            {
                title: "Domain ID",
                data: "DomainId",
                name: "DomainId",
                className: "nowrap",
                width: "180px",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Role",
                data: "RoleNames",
                name: "RoleNames",
                orderable: false, // Disable sorting for this column
                className: "nowrap",
                width: "150px",
                render: function (data, type, row) {
                    return data ? data.join(', ') : '';  // Convert array to string
                }
            },
            // Display user-friendly value for ClaimTypes
            {
                title: "Claims",
                data: "ClaimTypes",
                name: "ClaimTypes",
                orderable: false, // Disable sorting for this column
                className: "nowrap",
                width: "100px",
                render: function (data, type, rowata) {
                    if (Array.isArray(data) && data.length > 0) {
                        return `<button type='button' class='cls-claimtypes btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i></button>`;
                    }
                    return ''; // Return empty if no claims
                }
            },
            {
                title: "Requested Generated On (Dt)",
                data: "UpdatedOn",
                name: "UpdatedOn",
                className: "text-wrap requested-generated-col",
                width: "220px",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                },
            },
            // Display user-friendly value for Mapped
            {
                title: "Mapping",
                data: "Mapped",
                name: "Mapped",
                className: "nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for AdminFlag
            {
                title: "Approval",
                data: "AdminFlag",
                name: "AdminFlag",
                className: "nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for Active
            {
                title: "Active",
                data: "Active",
                name: "Active",
                className: "nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for IsIO
            {
                title: "IO",
                data: "IsIO",
                name: "IsIO",
                className: "nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for IsCO
            {
                title: "Approver",
                data: "IsCO",
                name: "IsCO",
                className: "nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Additional column for Edit action
            {
                title: "Action",
                data: null,
                orderable: false,
                className: "noExport text-center col-action",
                width: "100px",
                render: function (data, type, row) {
                    return "<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>";
                }
            }
        ],
        /* ===== FORCE WIDTHS (IMPORTANT) ===== */
        columnDefs: [
            { targets: 0,width: "60px",  },
            { targets: 1, width: "120px" },
            { targets: 2, width: "180px" },
            { targets: 3, width: "150px" },
            { targets: 4, width: "220px" },
            { targets: 5, width: "100px" },
            { targets: 6, width: "100px" },
            { targets: 7, width: "100px" },
            { targets: 8, width: "100px" },
            { targets: 9, width: "100px" },
            { targets: 10, width: "100px" },
            { targets: 11, width: "100px" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Domain ID" // Add custom placeholder
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
                title: 'E-IASC_DomainRegn',
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

            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();

                if (rowData != null) {
                    UnitMapId = rowData.UnitMapId;
                    $("#lblUnit").html(rowData.UnitName);
                    $("#txtSusno").val(rowData.Sus_no);

                    $("#AddNewUnitmap").modal('show');
                    $("#btnMapUnitsave").val("Update");

                    Reset();
                    ResetErrorMessage();
                    $("#txtDomainId").val(rowData.DomainId);
                    $("#txtRole").val($(this).closest("tr").find("#roleName").html());
                    DomainRegId = rowData.Id;
                    if (rowData.AdminFlag == true) {
                        $("#txtapprovalyes").prop("checked", true);
                    }
                    else {
                        $("#txtapprovalno").prop("checked", true);

                    }

                    if (rowData.Active == true) {
                        $("#txtactiveyes").prop("checked", true);
                    }
                    else {
                        $("#txtactiveno").prop("checked", true);
                    }

                    if (rowData.IsIO == true) {
                        $("#initatingOffryes").prop("checked", true);
                    }
                    else {
                        $("#initatingOffrno").prop("checked", true);
                    }

                    if (rowData.IsCO == true) {
                        $("#commandingOffryes").prop("checked", true);
                    }
                    else {
                        $("#commandingOffrno").prop("checked", true);
                    }


                    if (rowData.TrnDomainMappingId > 0) {
                        TDMId = rowData.TrnDomainMappingId;
                        GetALLByUnitById(rowData.TrnDomainMappingUnitId);
                    }

                    if (rowData.TrnDomainMappingApptId > 0) {
                        GetNameByApptId(rowData.TrnDomainMappingApptId);
                    }
                    //$("#ddlRoles").val([1, 2]);
                    //$("#ddlRoles").trigger("change");
                    //let arr2 = $(this).closest("tr").find("#roleIds").html().split(',');
                    $("#ddlRoles").val(rowData.RoleIds);
                    $("#ddlRoles").trigger("change");

                    //let arr3 = $(this).closest("tr").find("#claimValues").html().split(',');
                    $("#ddClaims").val(rowData.ClaimValues);
                    $("#ddClaims").trigger("change");

                    $("#btnDomainAddDialog").val("Update");
                    $("#AddNewDomain").modal('show');

                }
            });
            $("#tbldata tbody").off("click", ".cls-claimtypes").on("click", ".cls-claimtypes", function () {
                var rowData = table.row($(this).closest("tr")).data();

                if (rowData != null) {
                    $("#claimDomainId").html(rowData.DomainId);
                    $("#ClaimsShowBody").html(`<ul>` + rowData.ClaimTypes.map(ct => `<li>${ct}</li>`).join('') + `</ul>`);
                    $("#ClaimsShow").modal('show');

                }
            });
        }
    });
}
function Save() {
    let param = {
        "Id": DomainRegId,
        "DomainId": $("#txtDomainId").val(),
        "RoleIds": $('#ddlRoles').val(),
        "ClaimValues": $('#ddClaims').val(),
        "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
        "Active": $('input:radio[name=txtactive]:checked').val(),
        "IsIO": $('input:radio[name=InitatingOffr]:checked').val(),
        "IsCO": $('input:radio[name=CommandingOffr]:checked').val(),
        //"IsRO": $('input:radio[name=IsRO]:checked').val(),
        //"IsORO": $('input:radio[name=IsORO]:checked').val(),
        "TDMId": TDMId,
        "ApptId": ApptId,
        "UnitMappId": UnitMapId,
    }
    $.ajax({
        url: '/Account/SaveDomainRegn',
        type: 'POST',
        //data: param, //get the search string
        data: { "request": encryptPayloadData(JSON.stringify(param)) },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {
            if (result == DataSave) {
                toastr.success('Domain Id has been saved');

                $("#AddNewDomain").modal('hide');
                AccountCount();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Domain Id has been Updated');

                $("#AddNewDomain").modal('hide');
                AccountCount();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {

                toastr.error('Domain Id Exits!');

            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].Message)
                    }


                }


            }
        }
    });
}

function Reset() {
    $("#btnDomainAddDialog").val("Save");
    $("#txtSearch").val("");
    DomainRegId = 0;
    $("#txtDomainId").val("");
    //$("#ddlRoles").select2('data', null);

    $('#ddlRoles').val(null).trigger('change');
    $('#ddClaims').val(null).trigger('change');

    TDMId = 0;
    UnitMapId = 0;
    $("#txtUnitName").val("");
    $("#lblSusno").html("");
    $("#lblPso").html("");
    $("#lblDG").html("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblFmn").html("");


    ApptId = 0;
    $("#txtAppointmentName").val("");

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);

    //$("#isroyes").prop("checked", false);
    //$("#isrono").prop("checked", false);

    $("#initatingOffryes").prop("checked", false);
    $("#initatingOffrno").prop("checked", false);

    $("#commandingOffryes").prop("checked", false);
    $("#commandingOffrno").prop("checked", false);

    //$("#isoroyes").prop("checked", false);
    //$("#isorono").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtDomainId-error").html("");
    $("#ddlRoles-error").html("");
    $("#ddClaims-error").html("");
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
    //$("#IsRO-error").html("");
    $("#InitatingOffr-error").html("");
    $("#CommandingOffr-error").html("");
    //$("#IsORO-error").html("");
    $("#txtAppointmentName-error").html("");
    $("#txtUnitName-error").html("");
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": encryptPayloadData(param1) },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            UnitMapId = data.UnitMapId;
            $("#lblSusno").html(data.Sus_no + '' + data.Suffix);
            /*$("#txtUnitName").val(data.UnitName);*/
            $("#txtUnitName").val(data.UnitName + ' ' + data.Sus_no + data.Suffix);

            if (data.UnitType == 1) {
                $("#lblComd").html(data.ComdName);
                $("#lblCorps").html(data.CorpsName);
                $("#lblDiv").html(data.DivName);
                $("#lblBde").html(data.BdeName);
                $("#lbl1").addClass("d-none");
                $("#lbl2").addClass("d-none");
                $("#lbl3").removeClass("d-none");
                $("#lbl4").removeClass("d-none");
                $("#lbl5").removeClass("d-none");
                $("#lbl6").removeClass("d-none");
                $("#lbl7").addClass("d-none");
            }
            else if (data.UnitType == 2) {
                $("#lblComd").html(data.ComdName);
                $("#lblCorps").html(data.CorpsName);
                $("#lblDiv").html(data.DivName);
                $("#lblBde").html(data.BdeName);
                $("#lblFmn").html(data.BranchName);
                $("#lbl1").addClass("d-none");
                $("#lbl2").addClass("d-none");
                $("#lbl3").removeClass("d-none");
                $("#lbl4").removeClass("d-none");
                $("#lbl5").removeClass("d-none");
                $("#lbl6").removeClass("d-none");
                $("#lbl7").removeClass("d-none");
            }
            else if (data.UnitType == 3) {
                $("#lblPso").html(data.PSOName);
                $("#lblDG").html(data.SubDteName);
                $("#lbl1").removeClass("d-none");
                $("#lbl2").removeClass("d-none");
                $("#lbl3").addClass("d-none");
                $("#lbl4").addClass("d-none");
                $("#lbl5").addClass("d-none");
                $("#lbl6").addClass("d-none");
                $("#lbl7").addClass("d-none");
            }
        }
    });
}
function GetNameByApptId(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            ApptId = data.ApptId;
            $("#txtAppointmentName").val(data.AppointmentName);
        }
    });
}
function BindRoles() {
    $.ajax({
        url: "/Account/GetAllRole",
        type: "POST",
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            var list = "";
            for (var i = 0; i < response.length; i++) {
                list += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
            }
            $('#ddlRoles').html(list)
        }
    });
}
function BindClaims() {
    $.ajax({
        url: "/Account/GetAllClaimsForDD",
        method: "POST",
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            var list = "";
            for (var i = 0; i < response.length; i++) {
                list += '<option value="' + response[i].ClaimValue + '">' + response[i].ClaimType + '</option>';
            }
            $('#ddClaims').html(list)
        }
    });
}
function AccountCount() {
    $.ajax({
        url: "/Account/AccountCount",
        type: "POST",
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            $("#lblUser").html(response.User);
            $("#lblActiveUser").html(response.ActiveUser);
            $("#lblInActiveUser").html(response.InActiveUser);
            $("#lblMappedUser").html(response.MappedUser);
            $("#lblUnMappedUser").html(response.UnMappedUser);
            $("#lblVerifiedUser").html(response.VerifiedUser);
            $("#lblNotVerifiedUser").html(response.NotVerifiedUser);
            $("#lblIO").html(response.IO);
            $("#lblApprover").html(response.CO);
            //$("#lblRO").html(response.RO);
            //$("#lblORO").html(response.ORO);
        }
    });
}
