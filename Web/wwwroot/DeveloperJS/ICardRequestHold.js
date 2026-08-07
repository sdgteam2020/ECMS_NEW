let spnICardHoldId = 0;
let spnRequestId = 0;
var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    let cvalue = $("#spnFlagICardAppl").html();

    applyDataTableSearchValidation('#tbldata');

    prepareICardHoldModal();

    BindData(cvalue, function () {
    });
    $('.select2').select2({
        dropdownParent: $('#AddICardRequestHold'),
        closeOnSelect: false
    });
    $("#btnRequestHoldAdd").on("click", function () {
        Reset();
        ResetErrorMessage();
        $("#gpUnHoldReason").addClass("d-none");
        $("#txtArmyNo").prop('readonly', false);
        $("#txtHoldReason").prop('readonly', false);
        showICardHoldModal();
    });

    $("#btnAddICardRequestHold").on("click", function () {
        ResetErrorMessage();

        let formId = '#SaveICardRequestHold';
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
    });

    $("#txtArmyNo").autocomplete({
        source: function (request, response) {
            $("#lblName").html('');
            $("#lblRank").html('');
            $("#lblUnitName").html('');
            if (request.term.length > 2) {
                var param = { "ArmyNo": request.term };
                spnRequestId = 0;
                $.ajax({
                    url: '/BasicDetail/GetTopArmyNoFromICardRequest',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.ServiceNo, value: item.RequestId };
                            }))
                        }
                        else {
                            $("#txtArmyNo").val("");
                            spnRequestId = 0;
                            alert("Army No not found.")
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
            spnRequestId = i.item.value;
            $("#txtArmyNo").val(i.item.label);
            var param1 = { "RequestId": i.item.value };
            $.ajax({
                url: '/BasicDetail/GetBDetailByRequestId',
                method: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype: 'json',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                success: function (data) {
                    $("#lblName").html(data.LName == null ? data.FName : data.FName + ' ' + data.LName);
                    $("#lblRank").html(data.RankName);
                    $("#lblUnitName").html(data.UnitName);
                }
            });
        },

    });

    $('#txtArmyNo').on("keyup", function (e) {
        if (e.which == 46) {
            spnRequestId = 0;
            $("#txtArmyNo").val('');
            $("#lblName").html('');
            $("#lblRank").html('');
            $("#lblUnitName").html('');
        }
    });

});

/* UI-only helper: move this modal outside fixed/stacking containers and keep
   the dim backdrop below the bright, clickable dialog. */
function prepareICardHoldModal() {
    const selector = "#AddICardRequestHold";
    const $modal = $(selector);

    if ($modal.length && !$modal.parent().is("body")) {
        $modal.appendTo(document.body);
    }

    $(document)
        .off("show.bs.modal.icardHold", selector)
        .on("show.bs.modal.icardHold", selector, function () {
            if ($(".modal.show").length === 0) {
                $(".modal-backdrop").remove();
            }

            $(this).css({ zIndex: 1055, opacity: 1, pointerEvents: "auto" });
        })
        .off("shown.bs.modal.icardHold", selector)
        .on("shown.bs.modal.icardHold", selector, function () {
            $(this).css({ zIndex: 1055, opacity: 1, pointerEvents: "auto" });
            $(this).find(".modal-dialog, .modal-content").css({
                opacity: 1,
                filter: "none",
                pointerEvents: "auto"
            });
            $(".modal-backdrop").last().css("z-index", 1040);
        })
        .off("hidden.bs.modal.icardHold", selector)
        .on("hidden.bs.modal.icardHold", selector, function () {
            if ($(".modal.show").length === 0) {
                $(".modal-backdrop").remove();
                $("body").removeClass("modal-open").css("padding-right", "");
            }
        });
}

/* UI-only helper: open the existing Bootstrap modal after ensuring its DOM
   position cannot place it below the shared page overlay. */
function showICardHoldModal() {
    const $modal = $("#AddICardRequestHold");

    if (!$modal.length) {
        return;
    }

    if (!$modal.parent().is("body")) {
        $modal.appendTo(document.body);
    }

    $modal.modal("show");
}

/* UI-only helper: adjust DataTable columns safely, whether or not the
   Responsive extension is loaded. */
function adjustICardHoldTable(api) {
    const currentTable = api || table;

    if (!currentTable || typeof currentTable.columns !== "function") {
        return;
    }

    currentTable.columns.adjust();

    if (currentTable.responsive && typeof currentTable.responsive.recalc === "function") {
        currentTable.responsive.recalc();
    }
}


$(window)
    .off("resize.icardHoldTable")
    .on("resize.icardHoldTable", function () {
        window.clearTimeout(globalThis.icardHoldResizeTimer);
        globalThis.icardHoldResizeTimer = window.setTimeout(function () {
            adjustICardHoldTable();
        }, 120);
    });

function BindData(cvalue, callback) {
    cvalue = cvalue ?? $("#spnFlagICardAppl").html();

    if ($.fn.DataTable.isDataTable("#tbldata")) {
        $("#tbldata").DataTable().destroy();
        $("#tbldata").empty(); // Clear old thead/tbody
    }
    const columns = getColumnsData(cvalue);
    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY
        autoWidth: false, // Let us handle width via CSS
        responsive: false, // Keep all columns available through the horizontal table scroll
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: false,
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
                let response = await fetch("/BasicDetail/GetAllICardRequestHold", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                if (result.Result === false) {
                    toastr.error(result.Message);
                    callback({
                        draw: data.draw,
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    });
                    return;
                }
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        language: {
            search: "",
            searchPlaceholder: "Search Army No / Name",
            emptyTable: "No I-Card hold records found"
        },
        dom: "<'row g-2 align-items-center dt-top ecms-dt-toolbar'<'col-auto'l><'col-auto'B><'col ml-auto ms-auto'f>>" +
            "rt" +
            "<'row ecms-dt-footer'<'col-md-6 dt-info-col'i><'col-md-6 dt-page-col'p>>",
        buttons: [
            //{
            //    extend: 'copy',
            //    exportOptions: {
            //        columns: "thead th:not(.noExport)"
            //    }
            //},
            {
                extend: 'excel',
                className: 'btn btn-primary btn-sm',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'pdfHtml5',
                className: 'btn btn-primary btn-sm',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_ApplicationHold',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
            adjustICardHoldTable(this.api());
        },
        drawCallback: function (settings) {
            adjustICardHoldTable(this.api());

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            tooltipTriggerList.forEach(el => {
                new bootstrap.Tooltip(el);
            });

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ICardHoldId != null) {

                    Reset();
                    ResetErrorMessage();
                    $("#gpUnHoldReason").removeClass("d-none");
                    $("#txtArmyNo").prop('readonly', true);
                    $("#txtHoldReason").prop('readonly', true);
                    spnICardHoldId = rowData.ICardHoldId;
                    spnRequestId = rowData.RequestId;
                    $("#txtArmyNo").val(rowData.ServiceNo);
                    $("#lblRank").html(rowData.RankName);
                    $("#lblName").html(`${rowData.FName || ""} ${rowData.LName || ""}`.trim());
                    $("#lblUnitName").html(rowData.UnitName);
                    $("#txtHoldReason").val(rowData.HoldReason);
                    $("#txtUnHoldReason").val(rowData.UnHoldReason != null ? rowData.UnHoldReason : "");

                    if (rowData.IsHold == true) {
                        $("#IsHoldYes").prop("checked", true);
                    }
                    else {
                        $("#IsHoldNo").prop("checked", true);
                    }

                    $("#btnAddICardRequestHold").val("Update");
                    showICardHoldModal();
                }
                else {
                    $("#spnDispatchCardId").html(0);
                }
            });

            $("#tbldata tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetRequestHistory(rowData.RequestId);
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
function Save() {
    $.ajax({
        url: '/BasicDetail/SaveICardRequestHold',
        type: 'POST',
        data: {
            "ICardHoldId": spnICardHoldId,
            "RequestId": spnRequestId,
            "IsHold": $('input:radio[name=IsHold]:checked').val(),
            "HoldReason": $("#txtHoldReason").val(),
            "UnHoldReason": $("#txtUnHoldReason").val().length > 0 ? $("#txtUnHoldReason").val() : null,
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                toastr.success(result.Message);

                $("#AddICardRequestHold").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else {
                if (result.Message.length > 0) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        html: result.Message
                    });
                }
            }
        }
    });
}
function getColumnsData(choice) {
    let columns = [];
    switch (choice) {
        case 'Flag ICard Appl':
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
                    title: "Unit",
                    data: "UnitName",
                    name: "UnitName",
                    orderable: false,
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor",
                },
                {
                    title: "Held By",
                    data: "DomainId",
                    name: "DomainId",
                },
                {
                    title: "Reason for Held",
                    data: "HoldReason",
                    name: "HoldReason",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Hold",
                    data: "IsHold",
                    name: "IsHold",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    }
                },
                {
                    title: "Updated On",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "History",
                    data: null,
                    name: "History",
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" ></i></button>`
                    }
                },
                {
                    title: "Print / Edit",
                    data: null,
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>`;
                        return Action;
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
                    title: "Unit",
                    data: "UnitName",
                    name: "UnitName",
                    orderable: false,
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor",
                },
                {
                    title: "Held By",
                    data: "DomainId",
                    name: "DomainId",
                },

                {
                    title: "Reason for Held",
                    data: "HoldReason",
                    name: "HoldReason",
                    render: function (data, type, row) {
                        if (!data) return '';
                        return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                    }
                },
                {
                    title: "Hold",
                    data: "IsHold",
                    name: "IsHold",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    }
                },
                {
                    title: "Updated On",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "History",
                    data: null,
                    name: "History",
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" ></i></button>`
                    }
                }
            ];
    }
    return columns;
}
function Reset() {
    spnICardHoldId = 0;
    spnRequestId = 0;
    $("#btnAddICardRequestHold").val("Save");
    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#lblRank").html("");
    $("#lblName").html("");
    $("#lblUnitName").html("");
    $("#txtHoldReason").val("");
    $("#txtUnHoldReason").val("");
    $("#IsHoldYes").prop("checked", false);
    $("#IsHoldNo").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtArmyNo-error").html("");
    $("#txtHoldReason-error").html("");
    $("#IsHold-error").html("");
}