var table; // DataTable API instance
var RegId = 0;
var UnitMapId = 0;
var regimentalResizeTimer;

function setRegimentalModalMode(isEdit) {
    $("#exampleModalLabel").text(isEdit ? "Update Regimental Details" : "Add Regimental Details");
    $("#regimentalModalSubtitle").text(
        isEdit
            ? "Review and update the selected regimental centre details"
            : "Fill regimental centre, arms/service and unit mapping details"
    );
    $("#btnSaveRegimental").val(isEdit ? "Update" : "Save");
}

function moveRegimentalModalToBody() {
    const modalElement = document.getElementById("AddNewRegimental");
    if (!modalElement) return null;

    /*
       Keep the modal as a direct child of <body>. This removes the layout
       stacking context that previously allowed the backdrop to cover it.
       It is done before opening—not during Bootstrap's show event.
    */
    if (modalElement.parentElement !== document.body) {
        document.body.appendChild(modalElement);
    }

    return modalElement;
}

function openRegimentalModalFallback(modalElement) {
    if (!modalElement || modalElement.classList.contains("show")) return;

    document.querySelectorAll("body > .modal-backdrop.ecms-regimental-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    const backdrop = document.createElement("div");
    backdrop.className = "modal-backdrop fade show ecms-regimental-manual-backdrop";
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

function closeRegimentalModalFallback(modalElement) {
    if (!modalElement) return;

    modalElement.classList.remove("show");
    modalElement.style.display = "none";
    modalElement.setAttribute("aria-hidden", "true");
    modalElement.removeAttribute("aria-modal");

    document.querySelectorAll("body > .modal-backdrop.ecms-regimental-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    if (!document.querySelector("body > .modal.show")) {
        document.body.classList.remove("modal-open");
        document.body.style.removeProperty("padding-right");
        document.body.style.removeProperty("overflow");
    }
}

function showRegimentalModal() {
    const modalElement = moveRegimentalModalToBody();
    if (!modalElement) return;

    /* Remove only an old page fallback backdrop. Bootstrap manages its own backdrop. */
    document.querySelectorAll("body > .modal-backdrop.ecms-regimental-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    /* The application originally used the Bootstrap jQuery modal plug-in. */
    if (window.jQuery && $.fn && typeof $.fn.modal === "function") {
        try {
            $(modalElement).modal("show");

            /* Safety fallback when a conflicting modal plug-in fails silently. */
            window.setTimeout(function () {
                if (!modalElement.classList.contains("show")) {
                    openRegimentalModalFallback(modalElement);
                }
            }, 150);
            return;
        } catch (error) {
            console.warn("Bootstrap jQuery modal could not open Regimental modal:", error);
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
            console.warn("Bootstrap modal could not open Regimental modal:", error);
        }
    }

    openRegimentalModalFallback(modalElement);
}

function hideRegimentalModal() {
    const modalElement = document.getElementById("AddNewRegimental");
    if (!modalElement) return;

    if (window.jQuery && $.fn && typeof $.fn.modal === "function") {
        try {
            $(modalElement).modal("hide");
            window.setTimeout(function () {
                if (modalElement.classList.contains("show")) {
                    closeRegimentalModalFallback(modalElement);
                } else {
                    document.querySelectorAll("body > .modal-backdrop.ecms-regimental-manual-backdrop")
                        .forEach(function (backdrop) { backdrop.remove(); });
                }
            }, 200);
            return;
        } catch (error) {
            console.warn("Bootstrap jQuery modal could not close Regimental modal:", error);
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
            console.warn("Bootstrap modal could not close Regimental modal:", error);
        }
    }

    closeRegimentalModalFallback(modalElement);
}

function adjustRegimentalTable() {
    if (!table || typeof table.columns !== "function") return;

    table.columns.adjust();

    if (table.responsive && typeof table.responsive.recalc === "function") {
        table.responsive.recalc();
    }
}

function initializeRegimentalTooltips() {
    if (!(window.bootstrap && bootstrap.Tooltip)) return;

    document.querySelectorAll('#tbldata [data-bs-toggle="tooltip"]').forEach(function (element) {
        bootstrap.Tooltip.getOrCreateInstance(element);
    });
}

$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    /*
       Initialise and bind the modal before DataTable startup. Therefore a table
       error cannot prevent the Add button from receiving its click handler.
    */
    moveRegimentalModalToBody();

    $(document)
        .off("click.ecmsRegimentalAdd", "#btnAddRegimental")
        .on("click.ecmsRegimentalAdd", "#btnAddRegimental", function (e) {
            e.preventDefault();
            Reset();
            ResetErrorMessage();
            setRegimentalModalMode(false);
            showRegimentalModal();
        })
        .off("click.ecmsRegimentalClose", "#AddNewRegimental [data-dismiss='modal'], #AddNewRegimental [data-bs-dismiss='modal']")
        .on("click.ecmsRegimentalClose", "#AddNewRegimental [data-dismiss='modal'], #AddNewRegimental [data-bs-dismiss='modal']", function (e) {
            e.preventDefault();
            hideRegimentalModal();
        });

    $(document)
        .off("keydown.ecmsRegimentalModal")
        .on("keydown.ecmsRegimentalModal", function (e) {
            if (e.key === "Escape" && document.getElementById("AddNewRegimental")?.classList.contains("show")) {
                hideRegimentalModal();
            }
        });

    mMsater(0, "ddlArmType", 9, "");

    if (typeof applyDataTableSearchValidation === "function") { applyDataTableSearchValidation('#tbldata'); }

    try {
        BindData();
    } catch (error) {
        console.error("Regimental DataTable initialisation failed:", error);
    }
    $('input.js-uppercase').on('input', function () {
        this.value = this.value.toUpperCase();
    });

    $("#btnResetRegimental").on("click", function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                UnitMapId = 0;
                const param = new URLSearchParams({ UnitName: request.term });

                fetch('/Master/GetALLByUnitName', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: param
                })
                    .then(res => {
                        if (!res.ok) {
                            throw new Error('Network response was not ok');
                        }
                        return res.json();
                    })
                    .then(data => {
                        if (data.length !== 0) {
                            response(data.map(item => {
                                $("#loading").addClass("d-none");
                                return {
                                    label: item.Sus_no + item.Suffix + ' ' + item.UnitName,
                                    value: item.UnitMapId
                                };
                            }));
                        } else {
                            $("#txtUnitName").val("");
                            UnitMapId = 0;
                            alert("Unit not found.");
                        }
                    })
                    .catch(error => {
                        alert(error.message);
                    });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            UnitMapId = i.item.value;
        },

    });

    $('#txtUnitName').on('keyup', function (e) {
        if (e.key === 'Delete') {
            $("#txtUnitName").val("");
            UnitMapId = 0;
            $("#ddlTDMId").find("option").not(":first").remove();
            $("#ddlTDMId").val("0");
        }
    });

    $("#btnSaveRegimental").on("click", function () {
        Proceed();
    });
});
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveRegimental';
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
    const columns = getColumnsForRegimental();
    table = $("#tbldata").DataTable({
        scrollY: 'var(--ecms-regimental-scroll-height)',
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
                let response = await fetch("/Master/GetAllRegimental_Pagination", {
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
            { targets: 2, width: "20%" },
            { targets: 3, width: "14%" },
            { targets: 4, width: "14%" },
            { targets: 5, width: "22%" },
            { targets: 6, width: "14%" },
            { targets: 7, width: "10%" },
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
                title: 'E-IASC_Regimental',
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
                title: 'Search Regimental Centre, Abbreviation or Location',
                'aria-label': 'Search regimental centre records'
            });

            adjustRegimentalTable();

            $(window)
                .off('resize.ecmsRegimentalTable')
                .on('resize.ecmsRegimentalTable', function () {
                    clearTimeout(regimentalResizeTimer);
                    regimentalResizeTimer = setTimeout(adjustRegimentalTable, 120);
                });
        },
        drawCallback: function () {
            adjustRegimentalTable();
            initializeRegimentalTooltips();

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RegId != null) {
                    Reset();
                    ResetErrorMessage();
                    $("#txtName").val(rowData.Name);
                    $("#txtAbbreviation").val(rowData.Abbreviation.toUpperCase());
                    $("#txtLocation").val(rowData.Location);
                    RegId = rowData.RegId;
                    $("#ddlArmType").val(rowData.ArmedId);

                    if (rowData.UnitId != null) {
                        UnitMapId = rowData.UnitId;
                        $("#txtUnitName").val(`${rowData.Sus_no}${rowData.Suffix} ${rowData.UnitName}`);
                    }
                    else {
                        UnitMapId = 0;
                        $("#txtUnitName").val("");
                    }
                    setRegimentalModalMode(true);
                    showRegimentalModal();
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RegId != null) {
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
                            Delete(rowData.RegId);
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
        Name: $("#txtName").val().trim(),
        RegId: RegId,
        Abbreviation: $("#txtAbbreviation").val().trim(),
        ArmedId: $("#ddlArmType").val(),
        Location: $("#txtLocation").val().trim(),
        UnitId: (() => {
            let val = UnitMapId;

            if (!val || val === "0") return null;

            const parsed = parseInt(val, 10);
            return isNaN(parsed) ? null : parsed;
        })()
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    fetch('/Master/SaveRegimental', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json', // change to JSON
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: JSON.stringify({ data: encrypted })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // or use response.text() depending on server response type
        })
        .then(result => {

            if (result.Result == true) {
                toastr.success(result.Message);
                hideRegimentalModal();
                BindData();
                Reset();
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
        })
        .catch(error => {
            console.error('Fetch error:', error);
            toastr.error('Failed to save data. Please try again.');
        });
}

function Reset() {
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#txtLocation").val("");
    $("#txtUnitName").val("");
    $("#ddlArmType").val("0");
    UnitMapId = 0;
    RegId = 0;
    setRegimentalModalMode(false);
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#txtLocation-error").html("");
    $("#txtUnitName-error").html("");
    $("#ddlArmType-error").html("");
}
function Delete(Id) {
    const userdata = new URLSearchParams({ RegId: Id });

    fetch('/Master/DeleteRegimental', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': globalThis.RequestVerificationToken
        },
        body: userdata
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // Important: parse JSON instead of text
        })
        .then(response => {
            if (response !== null) {
                if (response === InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else if (response === Success) {
                    toastr.success('Deleted Selected');
                    BindData();
                }
            } else {
                Swal.fire({
                    text: errormsg001
                });
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
            Swal.fire({
                text: errormsg002
            });
        });
}

function DeleteMultiple(ids) {

    var userdata =
    {
        "ints": ids,

    };
    $.ajax({
        url: '/Master/DeleteRegimentalMultiple',
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
                    toastr.error('Deleted Selected');
                    BindData();
                }

                //}
            }

        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function getColumnsForRegimental() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "RegId",
            name: "RegId",
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
            title: "Regimental Centre",
            data: "Name",
            name: "Name",
            className: "nowrap",
            width: "20%",
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
            width: "14%",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Location",
            data: "Location",
            name: "Location",
            className: "nowrap",
            width: "14%",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Arms / Service",
            data: "ArmedName",
            name: "ArmedName",
            className: "nowrap",
            width: "22%",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Unit",
            data: "UnitAbbreviation",
            name: "UnitAbbreviation",
            className: "nowrap",
            width: "14%",
            orderable: false,
            render: function (data, type, row, meta) {
                if (row.UnitId != null)
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                else
                    return ``;
            }
        },
        // Additional column for Edit action
        {
            title: "Action",
            data: null,
            name: "Action",
            orderable: false,
            searchable: false,
            className: "noExport text-center col-action",
            width: "10%",
            render: function () {
                return `<button type="button"
                                class="cls-btnedit btn btn-warning ecms-action-btn"
                                title="Edit"
                                aria-label="Edit regimental centre"
                                data-bs-toggle="tooltip"
                                data-bs-placement="top">
                            <i class="fas fa-edit" aria-hidden="true"></i>
                        </button>
                        <button type="button"
                                class="cls-btnDelete btn btn-danger ecms-action-btn"
                                title="Delete"
                                aria-label="Delete regimental centre"
                                data-bs-toggle="tooltip"
                                data-bs-placement="top">
                            <i class="fas fa-trash-alt" aria-hidden="true"></i>
                        </button>`;
            }
        }
    ];
    return columns;
}