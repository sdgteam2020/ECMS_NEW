var table; // DataTable API instance
var RecordOfficeId = 0;
var UnitMapId = 0;
var recordOfficeResizeTimer;

function setRecordOfficeModalMode(isEdit) {
    $("#exampleModalLabel").text(isEdit ? "Update Record Office Details" : "Add Record Office Details");
    $("#recordOfficeModalSubtitle").text(
        isEdit
            ? "Review and update the selected record office and mapping details"
            : "Fill record office, arms/service, message and linked domain mapping details"
    );
    $("#btnRecordOfficeAdd").val(isEdit ? "Update" : "Save");
}

function moveRecordOfficeModalToBody() {
    const modalElement = document.getElementById("AddNewRecordOffice");
    if (!modalElement) return null;

    /*
       Keep the modal as a direct child of body before it opens. This prevents
       a layout stacking context from placing the backdrop over the dialog.
    */
    if (modalElement.parentElement !== document.body) {
        document.body.appendChild(modalElement);
    }

    return modalElement;
}

function openRecordOfficeModalFallback(modalElement) {
    if (!modalElement || modalElement.classList.contains("show")) return;

    document.querySelectorAll("body > .modal-backdrop.ecms-recordoffice-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    const backdrop = document.createElement("div");
    backdrop.className = "modal-backdrop fade show ecms-recordoffice-manual-backdrop";
    backdrop.addEventListener("click", hideRecordOfficeModal);
    document.body.appendChild(backdrop);

    modalElement.style.display = "block";
    modalElement.removeAttribute("aria-hidden");
    modalElement.setAttribute("aria-modal", "true");
    modalElement.setAttribute("role", "dialog");
    modalElement.classList.add("show");
    document.body.classList.add("modal-open");

    const firstInput = modalElement.querySelector("input:not([type='hidden']), select, textarea, button");
    if (firstInput) {
        window.setTimeout(function () { firstInput.focus(); }, 0);
    }
}

function closeRecordOfficeModalFallback(modalElement) {
    if (!modalElement) return;

    modalElement.classList.remove("show");
    modalElement.style.display = "none";
    modalElement.setAttribute("aria-hidden", "true");
    modalElement.removeAttribute("aria-modal");

    document.querySelectorAll("body > .modal-backdrop.ecms-recordoffice-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    if (!document.querySelector("body > .modal.show")) {
        document.body.classList.remove("modal-open");
        document.body.style.removeProperty("padding-right");
        document.body.style.removeProperty("overflow");
    }
}

function showRecordOfficeModal() {
    const modalElement = moveRecordOfficeModalToBody();
    if (!modalElement) return;

    document.querySelectorAll("body > .modal-backdrop.ecms-recordoffice-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    /* Bootstrap 4 / jQuery modal is used first because the existing project uses it. */
    if (window.jQuery && $.fn && typeof $.fn.modal === "function") {
        try {
            $(modalElement).modal("show");

            window.setTimeout(function () {
                if (!modalElement.classList.contains("show")) {
                    openRecordOfficeModalFallback(modalElement);
                }
            }, 150);
            return;
        } catch (error) {
            console.warn("Bootstrap jQuery modal could not open Record Office modal:", error);
        }
    }

    if (window.bootstrap && window.bootstrap.Modal) {
        try {
            const instance = typeof window.bootstrap.Modal.getOrCreateInstance === "function"
                ? window.bootstrap.Modal.getOrCreateInstance(modalElement, { backdrop: true, keyboard: true })
                : new window.bootstrap.Modal(modalElement, { backdrop: true, keyboard: true });
            instance.show();
            return;
        } catch (error) {
            console.warn("Bootstrap modal could not open Record Office modal:", error);
        }
    }

    openRecordOfficeModalFallback(modalElement);
}

function hideRecordOfficeModal() {
    const modalElement = document.getElementById("AddNewRecordOffice");
    if (!modalElement) return;

    if (window.jQuery && $.fn && typeof $.fn.modal === "function") {
        try {
            $(modalElement).modal("hide");
            window.setTimeout(function () {
                if (modalElement.classList.contains("show")) {
                    closeRecordOfficeModalFallback(modalElement);
                } else {
                    document.querySelectorAll("body > .modal-backdrop.ecms-recordoffice-manual-backdrop")
                        .forEach(function (backdrop) { backdrop.remove(); });
                }
            }, 200);
            return;
        } catch (error) {
            console.warn("Bootstrap jQuery modal could not close Record Office modal:", error);
        }
    }

    if (window.bootstrap && window.bootstrap.Modal) {
        try {
            const instance = typeof window.bootstrap.Modal.getInstance === "function"
                ? window.bootstrap.Modal.getInstance(modalElement)
                : null;
            if (instance) {
                instance.hide();
                return;
            }
        } catch (error) {
            console.warn("Bootstrap modal could not close Record Office modal:", error);
        }
    }

    closeRecordOfficeModalFallback(modalElement);
}

function adjustRecordOfficeTable() {
    if (!table || typeof table.columns !== "function") return;

    table.columns.adjust();

    if (table.responsive && typeof table.responsive.recalc === "function") {
        table.responsive.recalc();
    }
}

function initializeRecordOfficeTooltips() {
    if (!(window.bootstrap && bootstrap.Tooltip)) return;

    document.querySelectorAll('#tbldata [data-bs-toggle="tooltip"]').forEach(function (element) {
        if (typeof bootstrap.Tooltip.getOrCreateInstance === "function") {
            bootstrap.Tooltip.getOrCreateInstance(element);
        } else {
            new bootstrap.Tooltip(element);
        }
    });
}

$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    /* Bind modal actions before DataTable startup so a table error cannot block the Add button. */
    moveRecordOfficeModalToBody();

    $(document)
        .off("click.ecmsRecordOfficeAdd", "#btnAdd")
        .on("click.ecmsRecordOfficeAdd", "#btnAdd", function (e) {
            e.preventDefault();
            Reset();
            ResetErrorMessage();
            setRecordOfficeModalMode(false);
            showRecordOfficeModal();
        })
        .off("click.ecmsRecordOfficeClose", "#AddNewRecordOffice [data-dismiss='modal'], #AddNewRecordOffice [data-bs-dismiss='modal']")
        .on("click.ecmsRecordOfficeClose", "#AddNewRecordOffice [data-dismiss='modal'], #AddNewRecordOffice [data-bs-dismiss='modal']", function (e) {
            e.preventDefault();
            hideRecordOfficeModal();
        });

    $(document)
        .off("keydown.ecmsRecordOfficeModal")
        .on("keydown.ecmsRecordOfficeModal", function (e) {
            const modalElement = document.getElementById("AddNewRecordOffice");
            if (e.key === "Escape" && modalElement && modalElement.classList.contains("show")) {
                hideRecordOfficeModal();
            }
        });

    $("#AddNewRecordOffice")
        .off("hidden.bs.modal.ecmsRecordOffice")
        .on("hidden.bs.modal.ecmsRecordOffice", function () {
            document.querySelectorAll("body > .modal-backdrop.ecms-recordoffice-manual-backdrop")
                .forEach(function (backdrop) { backdrop.remove(); });
        });

    mMsater(0, "ddlArmType", ArmyType, "");

    if (typeof applyDataTableSearchValidation === "function") {
        applyDataTableSearchValidation('#tbldata');
    }

    try {
        BindData();
    } catch (error) {
        console.error("Record Office DataTable initialisation failed:", error);
    }

    $("#btnRecordOfficeReset")
        .off("click.ecmsRecordOfficeReset")
        .on("click.ecmsRecordOfficeReset", function () {
            Reset();
            ResetErrorMessage();
        });

    $("#btnRecordOfficeAdd")
        .off("click.ecmsRecordOfficeSave")
        .on("click.ecmsRecordOfficeSave", function () {
            Proceed();
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
        });
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        });
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
        scrollY: 'var(--ecms-recordoffice-scroll-height)',
        scrollX: true,
        scrollCollapse: false,
        fixedHeader: false,

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
            { targets: 1, width: "6%" },
            { targets: 2, width: "22%" },
            { targets: 3, width: "16%" },
            { targets: 4, width: "22%" },
            { targets: 5, width: "24%" },
            { targets: 6, width: "10%" },
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rt<'ecms-dt-footer row g-2'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>",
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
                orientation: 'portrait',
                pageSize: 'A4',
                title: 'E-IASC_RecordOffice',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
            const searchBox = $('#tbldata_wrapper .dataTables_filter input, #tbldata_wrapper .dt-search input');
            searchBox.attr({
                title: 'Search Record Office, Abbreviation or Arms / Service',
                'aria-label': 'Search record office records'
            });

            adjustRecordOfficeTable();

            $(window)
                .off('resize.ecmsRecordOfficeTable')
                .on('resize.ecmsRecordOfficeTable', function () {
                    clearTimeout(recordOfficeResizeTimer);
                    recordOfficeResizeTimer = setTimeout(adjustRecordOfficeTable, 120);
                });
        },
        drawCallback: function () {
            adjustRecordOfficeTable();
            initializeRecordOfficeTooltips();

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
                    setRecordOfficeModalMode(true);
                    showRecordOfficeModal();
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
    const payload = {
        "Name": $("#txtName").val().trim(),
        "Abbreviation": $("#txtAbbreviation").val().trim(),
        "ArmedId": $("#ddlArmType").val(),
        "RecordOfficeId": RecordOfficeId,
        "UnitId": UnitMapId == 0 ? null : UnitMapId,
        "TDMId": $("#ddlTDMId").val() == 0 ? null : $("#ddlTDMId").val(),
        "Message": $("#txtMessage").val().length > 0 ? $("#txtMessage").val() : null,
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    $.ajax({
        url: '/Master/SaveRecordOffice',
        type: 'POST',
        data: { Request: encrypted },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                toastr.success(result.Message);
                hideRecordOfficeModal();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else {
                const Message = result.Message || "Something went wrong.";

                const errors = Message
                    .split(";")
                    .map(x => x.trim())
                    .filter(x => x !== "");

                const list = document.createElement("ul");
                list.classList.add("error-list"); // ✅ use CSS class

                errors.forEach(function (error) {
                    const item = document.createElement("li");
                    item.textContent = error;
                    list.appendChild(item);
                });

                Swal.fire({
                    icon: "error",
                    title: "Message",
                    html: list
                });
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
    setRecordOfficeModalMode(false);
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#ddlArmType-error").html("");
    $("#txtMessage-error").html("");
    $("#txtUnitName-error").html("");
    $("#ddlTDMId-error").html("");
}

function GetDDMappedForRecord(UnitId, TDMId) {
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
            width: "6%",
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
            width: "22%",
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
            width: "16%",
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
            width: "22%",
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
            width: "24%",
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
            searchable: false,
            width: "10%",
            render: function (data, type, row) {
                if (row.ArmedId != $("#ArmedIdForORO").html()) {
                    return `<button type="button"
                                class="cls-btnedit btn btn-warning ecms-action-btn"
                                title="Edit"
                                aria-label="Edit record office"
                                data-bs-toggle="tooltip"
                                data-bs-placement="top">
                            <i class="fas fa-edit" aria-hidden="true"></i>
                        </button>
                        <button type="button"
                                class="cls-btnDelete btn btn-danger ecms-action-btn"
                                title="Delete"
                                aria-label="Delete record office"
                                data-bs-toggle="tooltip"
                                data-bs-placement="top">
                            <i class="fas fa-trash-alt" aria-hidden="true"></i>
                        </button>`;
                }
                else {
                    return `<span class='badge rounded-pill bg-success'></span>`;
                }

            }
        }
    ];
    return columns;
}