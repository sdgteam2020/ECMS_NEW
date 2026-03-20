var table; // Declare table variable outside the function to preserve the instance
var tabledialog; // Declare tabledialog variable outside the function to preserve the instance
var DomainRegId = 0;
var TrnDomainMappingIdForMapping = 0;
var DomainRegIdForMapping = 0;
var UserProfileId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData("");
    AccountCount();
    $("#btnDomainFlag").on("click", function () {
        Proceed();
    });
    $("#btnAddMapping").on("click", function () {
        ProceedForMapping();
    });

    $("#AddNewDomain input[name='txtapproval']").on("click",function () {
        $("#txtapproval-error").html("");
    });
    $("#AddNewDomain input[name='txtactive']").on("click",function () {
        $("#txtactive-error").html("");
    });

    $("#txtArmyNo").autocomplete({
        source: function (request, response) {
            $("#lblName").html('');
            $("#lblRank").html('');
            if (request.term.length > 2) {
                UserProfileId = 0;
                var param = { "ArmyNo": request.term };
                $.ajax({
                    url: '/UserProfile/GetTopByArmyNo',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.ArmyNo, value: item.UserId };
                            }))
                        }
                        else {
                            $("#txtArmyNo").val("");
                            UserProfileId = 0;
                            alert("Offrs Army No not found.")
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
            UserProfileId = i.item.value;
            $("#txtArmyNo").val(i.item.label);
            var param1 = { "UserId": i.item.value };
            $.ajax({
                url: '/UserProfile/GetProfileByUserId',
                method: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype: 'json',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                success: function (data) {
                    $("#lblName").html(data.Name);
                    $("#lblRank").html(data.RankName);
                }
            });
        },
        
    });

    $('#txtArmyNo').on("keyup",function (e) {
        if (e.key === 'Delete' || e.key === 'Del' || e.key === 'Backspace') {
            UserProfileId = 0;
            $("#txtArmyNo").val('');
            $("#lblName").html('');
            $("#lblRank").html('');
        }
    });

    $("#txtSearch").on("keyup",function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            var ChoiceValue = $("input[type='radio'][name=choice]:checked").val();
            if (ChoiceValue == "Id") {
                var num_val = parseInt(eThis.val());
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData(ChoiceValue);
                }
            }
            else {
                BindData(ChoiceValue);
            }
        }
        else {
            alert("Select Choice");
        }
    });

    $("#btnUser").on("click",function () {

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
    //$("#btnRO").click(function () {
    //    if ($("#lblRO").html() > 0) {
    //        $("#tbldatadialog").DataTable().destroy();
    //        $("#lblModelTitle").html('Total Not Verified Users');
    //        $("#DataTableDialog").modal('show');
    //        BindDialog("RO");
    //    }
    //});
    //$("#btnORO").click(function () {
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
                    width: "100px",
                },
                {
                    title: "Domain Id",
                    data: "DomainId",
                    name: "DomainId",
                    className: "nowrap",
                    width: "150px",
                    render: function (data, type, row, meta) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Army No",
                    data: "ArmyNo",
                    name: "ArmyNo",
                    className: "nowrap",
                    width: "120px",
                    render: function (data, type, row) {
                        return data ? data : "<span class='badge badge-pill badge-danger'>IC No Not Mapped</span>";
                    }
                },
                {
                    title: "Role",
                    data: "RoleNames",
                    name: "RoleNames",
                    orderable: false, // Disable sorting for this column
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        return data ? data.join(', ') : '';  // Convert array to string
                    }
                },
                {
                    title: "Request Generated On (Dt)",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    className: "text-wrap requested-generated-col",
                    width: "150px",
                    render: function (data, type, row) {
                        return data ? DateFormateddMMyyyyhhmmss(data) : "NA";
                    },
                },
                // Display user-friendly value for Mapped
                {
                    title: "Mapping",
                    data: "Mapped",
                    name: "Mapped",
                    className: "noExport nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                // Display user-friendly value for Active
                {
                    title: "IsActive",
                    data: "Active",
                    name: "Active",
                    className: "nowrap",
                    width: "100px",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    },
                },
                // Display user-friendly value for AdminFlag
                {
                    title: "Status",
                    data: "AdminFlag",
                    name: "AdminFlag",
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
            ],
            columnDefs: [
                { targets: 0, width: "60px", },
                { targets: 1, width: "100px" },
                { targets: 2, width: "150px" },
                { targets: 3, width: "120px" },
                { targets: 4, width: "100px" },
                { targets: 5, width: "150px" },
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
                let response = await fetch("/Account/GetAllUserRegn", {
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
                width: "150px",
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Army No",
                data: "ArmyNo",
                name: "ArmyNo",
                className: "nowrap",
                width: "120px",
                render: function (data, type, row) {
                    return data ? data : "<span class='badge badge-pill badge-danger'>IC No Not Mapped</span>";
                }
            },
            {
                title: "Role",
                data: "RoleNames",
                name: "RoleNames",
                orderable: false, // Disable sorting for this column
                className: "nowrap",
                width: "120px",
                render: function (data, type, row) {
                    return data ? data.join(', ') : '';  // Convert array to string
                }
            },
            {
                title: "Request Generated On (Dt)",
                data: "UpdatedOn",
                name: "UpdatedOn",
                className: "text-wrap requested-generated-col",
                width: "150px",
                render: function (data, type, row) {
                    return data ? DateFormateddMMyyyyhhmmss(data) : "NA";
                },
            },
            // Display user-friendly value for Mapped
            {
                title: "Mapping",
                data: "Mapped",
                name: "Mapped",
                className: "noExport nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<button type='button' class='cls-btneditMapping btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-link'></i></button>" : "<button type='button' class='cls-btneditMapping btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-unlink'></i></button>";
                },
            },
            // Display user-friendly value for Active
            {
                title: "Active Yes/No",
                data: "Active",
                name: "Active",
                className: "nowrap",
                width: "100px",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for AdminFlag
            {
                title: "Status",
                data: "AdminFlag",
                name: "AdminFlag",
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
                name: "Id",
                className: "nowrap",
                width: "120px",
                orderable: false,
                className: "noExport text-center col-action",
                render: function (data, type, row) {
                    return data ? "<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>" : "NA";
                }
            }
        ],
        /* ===== FORCE WIDTHS (IMPORTANT) ===== */
        columnDefs: [
            { targets: 0, width: "60px", },
            { targets: 1, width: "120px" },
            { targets: 2, width: "150px" },
            { targets: 3, width: "120px" },
            { targets: 4, width: "120px" },
            { targets: 5, width: "150px" },
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
                title: 'E-IASC_UserRegn',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
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

            this.api().columns.adjust();

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
                    Reset();
                    ResetErrorMessage();
                    $("#lblDomainId").html(rowData.DomainId);
                    $("#lblRole").html(rowData.RoleNames.join(', ')); //data ? data.join(', ') : ''
                    DomainRegId = rowData.Id;
                    if (rowData.AdminMsg != null) {
                        $("#txtadminmessage").val(rowData.AdminMsg);
                    }
                    else {
                        $("#txtadminmessage").val("");
                    }

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

                    if (rowData.TrnDomainMappingId > 0) {
                        GetALLByUnitById(rowData.TrnDomainMappingUnitId);
                    }

                    if (rowData.TrnDomainMappingApptId > 0) {
                        GetNameByApptId(rowData.TrnDomainMappingApptId);
                    }

                    $("#btnDomainFlag").val("Update");
                    $("#AddDomainFlag").modal('show');



                    //$("#spnUnitMapId").html(rowData.UnitMapId);
                    //$("#spnUnitId").html(rowData.UnitId);
                    $("#lblUnit").html(rowData.UnitName);
                    $("#txtSusno").val(rowData.Sus_no);

                    $("#AddNewUnitmap").modal('show');
                    $("#btnMapUnitsave").val("Update");
                }
            });
            $("#tbldata tbody").off("click", ".cls-btneditMapping").on("click", ".cls-btneditMapping", function () {
                var rowData = table.row($(this).closest("tr")).data();

                if (rowData != null) {
                    ResetForMapping();
                    ResetErrorMessageForMapping();
                    $("#lblDomainIdForMapping").html(rowData.DomainId);
                    $("#lblRoleForMapping").html(rowData.RoleNames);
                    DomainRegIdForMapping = rowData.Id;

                    if (rowData.AdminFlag == true) {
                        $("#txtapprovalyesForMapping").prop("checked", true);
                    }
                    else {
                        $("#txtapprovalnoForMapping").prop("checked", true);
                    }

                    if (rowData.Active == true) {
                        $("#txtactiveyesForMapping").prop("checked", true);
                    }
                    else {
                        $("#txtactivenoForMapping").prop("checked", true);
                    }
                    if (rowData.UserId > 0) {
                        GetProfileByUserId(rowData.UserId);
                    }

                    if (rowData.TrnDomainMappingId > 0) {
                        TrnDomainMappingIdForMapping = rowData.TrnDomainMappingId;
                        GetALLByUnitByIdForMapping(rowData.TrnDomainMappingUnitId);
                    }

                    if (rowData.TrnDomainMappingApptId > 0) {
                        GetNameByApptIdForMapping(rowData.TrnDomainMappingApptId);
                    }

                    $("#btnAddMapping").val("Update");
                    $("#AddMapping").modal('show');

                }
            });
        }
    });
}
function ProceedForMapping() {
    ResetErrorMessageForMapping();
    ValidateMappingInput();
    let formId = '#SaveMapping';
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
                SaveMapping();
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
function SaveMapping() {
    //alert($("#spnDomainRegId").html());
    $.ajax({
        url: '/Account/SaveMapping',
        type: 'POST',
        data: {
            "Id": DomainRegIdForMapping,
            "TDMId": TrnDomainMappingIdForMapping,
            "UserId": UserProfileId,
            "ArmyNo": $("#txtArmyNo").val(),

        }, //get the search string
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            var obj = jQuery.parseJSON(response);
            if (obj.Result == true) {
                toastr.success(obj.Message);

                $("#AddMapping").modal('hide');
                AccountCount();
                BindData("");
                ResetForMapping();
                ResetErrorMessageForMapping();
            }
            else if (obj.Result == false) {
                toastr.error(obj.Message);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: obj.Message,

                })
            }
            else if (obj.Result == false && obj.Message.length > 1) {
                for (var i = 0; i < obj.Message.length; i++) {
                    toastr.error(result[i][0].Message)
                }
            }
        }
    });
}
function ValidateMappingInput() {

    if ((UserProfileId == 0) && $("#txtArmyNo").val().length > 0) {
        $("#txtArmyNo").val('');
        $("#txtArmyNo-error").html("ArmyNo is invalid.");
        toastr.error('ArmyNo is invalid.');
    }
}
function GetProfileByUserId(param1) {
    $.ajax({
        url: '/UserProfile/GetProfileByUserId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UserId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            UserProfileId = data.UserId;
            $("#txtArmyNo").val(data.ArmyNo);
            $("#lblRank").html(data.RankName);
            $("#lblName").html(data.Name);
        }
    });
}
function GetALLByUnitByIdForMapping(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": encryptPayloadData(param1) },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblUnitNameForMapping").html(data.UnitName);
            $("#lblSusnoForMapping").html(data.Sus_no + '' + data.Suffix);


            if (data.UnitType == 1) {
                $("#lblComdForMapping").html(data.ComdName);
                $("#lblCorpsForMapping").html(data.CorpsName);
                $("#lblDivForMapping").html(data.DivName);
                $("#lblBdeForMapping").html(data.BdeName);
                $("#lblM1").addClass("d-none");
                $("#lblM2").addClass("d-none");
                $("#lblM3").removeClass("d-none");
                $("#lblM4").removeClass("d-none");
                $("#lblM5").removeClass("d-none");
                $("#lblM6").removeClass("d-none");
                $("#lblM7").addClass("d-none");
            }
            else if (data.UnitType == 2) {
                $("#lblComdForMapping").html(data.ComdName);
                $("#lblCorpsForMapping").html(data.CorpsName);
                $("#lblDivForMapping").html(data.DivName);
                $("#lblBdeForMapping").html(data.BdeName);
                $("#lblFmnForMapping").html(data.BranchName);
                $("#lblM1").addClass("d-none");
                $("#lblM2").addClass("d-none");
                $("#lblM3").removeClass("d-none");
                $("#lblM4").removeClass("d-none");
                $("#lblM5").removeClass("d-none");
                $("#lblM6").removeClass("d-none");
                $("#lblM7").removeClass("d-none");
            }
            else if (data.UnitType == 3) {
                $("#lblPsoForMapping").html(data.PSOName);
                $("#lblDGForMapping").html(data.SubDteName);
                $("#lblM1").removeClass("d-none");
                $("#lblM2").removeClass("d-none");
                $("#lblM3").addClass("d-none");
                $("#lblM4").addClass("d-none");
                $("#lblM5").addClass("d-none");
                $("#lblM6").addClass("d-none");
                $("#lblM7").addClass("d-none");
            }


        }
    });
}
function GetNameByApptIdForMapping(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblAppointmentNameForMapping").html(data.AppointmentName);
        }
    });
}
function ResetForMapping() {
    $("#txtSearch").val("");
    DomainRegIdForMapping = 0;
    $("#lblDomainIdForMapping").html("");
    $("#lblRoleForMapping").html("");
    TrnDomainMappingIdForMapping = 0;
    $("#lblUnitNameForMapping").html("");
    $("#lblSusnoForMapping").html("");
    $("#lblPsoForMapping").html("");
    $("#lblDGForMapping").html("");
    $("#lblComdForMapping").html("");
    $("#lblCorpsForMapping").html("");
    $("#lblDivForMapping").html("");
    $("#lblBdeForMapping").html("");
    $("#lblFmnForMapping").html("");

    UserProfileId = 0;
    $("#txtArmyNo").val("");
    $("#lblRank").html("");
    $("#lblName").html("");

    $("#lblAppointmentNameForMapping").html("");

    $("#txtapprovalyesForMapping").prop("checked", false);
    $("#txtapprovalnoForMapping").prop("checked", false);

    $("#txtactiveyesForMapping").prop("checked", false);
    $("#txtactivenoForMapping").prop("checked", false);
}
function ResetErrorMessageForMapping() {
    $("#txtArmyNo-error").html("");
    $("#txtadminmessage-error").html("");
}

function Proceed() {
    ResetErrorMessageForMapping();
    let formId = '#UpdateDomainFlag';
    $.validator.unobtrusive.parse($(formId));

    ValidateRadioButton();

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
                UpdateDomainFlag();
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
function UpdateDomainFlag() {
    $.ajax({
        url: '/Account/UpdateDomainFlag',
        type: 'POST',
        data: {
            "Id": DomainRegId,
            "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
            "Active": $('input:radio[name=txtactive]:checked').val(),
            "AdminMsg": $('#txtadminmessage').val().length > 0 ? $('#txtadminmessage').val() : null,
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {
            if (result == DataUpdate) {
                toastr.success('Domain Flag has been Updated');

                $("#AddDomainFlag").modal('hide');
                AccountCount();
                BindData("");
                Reset();
                ResetErrorMessage();
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
                        toastr.error(result[i][0].ErrorMessage)
                    }
                }
            }
        }
    });
}
function ValidateRadioButton() {
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
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": encryptPayloadData(param1) },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblUnitName").html(data.UnitName);
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
}
function GetNameByApptId(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblAppointmentName").html(data.AppointmentName);
        }
    });
}
function Reset() {
    $("#txtSearch").val("");
    DomainRegId = 0;
    $("#txtadminmessage").val("");
    $("#lblDomainId").html("");
    $("#lblRole").html("");

    $("#lblUnitName").html("");
    $("#lblSusno").html("");
    $("#lblPso").html("");
    $("#lblDG").html("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblFmn").html("");


    $("#lblAppointmentName").html("");

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
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
            $("#lblRO").html(response.RO);
            $("#lblORO").html(response.ORO);
        }
    });
}