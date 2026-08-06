var table; // DataTable API instance
let UnitMapId = 0;
let OROMappingId = 0;
let oroResizeTimer;

function setOROMappingModalMode(isEdit) {
    $("#exampleModalLabel").text(
        isEdit
            ? "Update Officer Record Office Mapping Details"
            : "Add Officer Record Office Mapping Details"
    );

    $("#oroModalSubtitle").text(
        isEdit
            ? "Review and update the selected officer record office mapping details"
            : "Fill record office, arms/service, rank, SUS no and linked domain details"
    );

    $("#btnOROMappingAdd").val(isEdit ? "Update" : "Save");
}

function moveOROMappingModalToBody() {
    const modalElement = document.getElementById("AddNewOROMapping");
    if (!modalElement) return null;

    /*
       Keep the modal outside page/layout stacking contexts. Bootstrap creates
       its backdrop under body, so the modal must use the same top-level layer.
    */
    if (modalElement.parentElement !== document.body) {
        document.body.appendChild(modalElement);
    }

    return modalElement;
}

function removeStaleOROMappingBackdrops() {
    document.querySelectorAll("body > .modal-backdrop.ecms-oro-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    if (!document.querySelector("body > .modal.show")) {
        document.querySelectorAll("body > .modal-backdrop")
            .forEach(function (backdrop) { backdrop.remove(); });
    }
}

function openOROMappingModalFallback(modalElement) {
    if (!modalElement || modalElement.classList.contains("show")) return;

    removeStaleOROMappingBackdrops();

    const backdrop = document.createElement("div");
    backdrop.className = "modal-backdrop fade show ecms-oro-manual-backdrop";
    backdrop.addEventListener("click", hideOROMappingModal);
    document.body.appendChild(backdrop);

    modalElement.style.display = "block";
    modalElement.removeAttribute("aria-hidden");
    modalElement.setAttribute("aria-modal", "true");
    modalElement.setAttribute("role", "dialog");
    modalElement.classList.add("show");

    document.body.classList.add("modal-open", "ecms-oro-modal-open");

    const firstControl = modalElement.querySelector(
        "input:not([type='hidden']):not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled])"
    );

    if (firstControl) {
        window.setTimeout(function () { firstControl.focus(); }, 0);
    }
}

function closeOROMappingModalFallback(modalElement) {
    if (!modalElement) return;

    modalElement.classList.remove("show");
    modalElement.style.display = "none";
    modalElement.setAttribute("aria-hidden", "true");
    modalElement.removeAttribute("aria-modal");

    removeStaleOROMappingBackdrops();

    if (!document.querySelector("body > .modal.show")) {
        document.body.classList.remove("modal-open", "ecms-oro-modal-open");
        document.body.style.removeProperty("padding-right");
        document.body.style.removeProperty("overflow");
    }
}

function showOROMappingModal() {
    const modalElement = moveOROMappingModalToBody();
    if (!modalElement) return;

    document.body.classList.add("ecms-oro-modal-open");
    document.querySelectorAll("body > .modal-backdrop.ecms-oro-manual-backdrop")
        .forEach(function (backdrop) { backdrop.remove(); });

    /* Bootstrap 4 / jQuery is checked first because the existing project uses it. */
    if (window.jQuery && $.fn && typeof $.fn.modal === "function") {
        try {
            $(modalElement).modal({ backdrop: true, keyboard: true, show: true });

            window.setTimeout(function () {
                if (!modalElement.classList.contains("show")) {
                    openOROMappingModalFallback(modalElement);
                }
            }, 180);
            return;
        } catch (error) {
            console.warn("Bootstrap jQuery modal could not open ORO Mapping modal:", error);
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
            console.warn("Bootstrap modal could not open ORO Mapping modal:", error);
        }
    }

    openOROMappingModalFallback(modalElement);
}

function hideOROMappingModal() {
    const modalElement = document.getElementById("AddNewOROMapping");
    if (!modalElement) return;

    if (window.jQuery && $.fn && typeof $.fn.modal === "function") {
        try {
            $(modalElement).modal("hide");

            window.setTimeout(function () {
                if (modalElement.classList.contains("show")) {
                    closeOROMappingModalFallback(modalElement);
                }
            }, 220);
            return;
        } catch (error) {
            console.warn("Bootstrap jQuery modal could not close ORO Mapping modal:", error);
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
            console.warn("Bootstrap modal could not close ORO Mapping modal:", error);
        }
    }

    closeOROMappingModalFallback(modalElement);
}

function initializeOROArmsSelect() {
    if (!(window.jQuery && $.fn && typeof $.fn.select2 === "function")) return;

    const $armsSelect = $("#ddlArmedIdList");
    if (!$armsSelect.length) return;

    if ($armsSelect.hasClass("select2-hidden-accessible")) {
        $armsSelect.select2("destroy");
    }

    $armsSelect.select2({
        placeholder: "Select Arms",
        width: "100%",
        dropdownParent: $("#AddNewOROMapping"),
        closeOnSelect: false
    });
}

function adjustOROMappingTable() {
    if (!table || typeof table.columns !== "function") return;

    table.columns.adjust();

    if (table.responsive && typeof table.responsive.recalc === "function") {
        table.responsive.recalc();
    }
}

function initializeOROMappingTooltips() {
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

    /* Bind modal actions before DataTable startup so a table error cannot block Add. */
    moveOROMappingModalToBody();

    $(document)
        .off("click.ecmsOROAdd", "#btnAdd")
        .on("click.ecmsOROAdd", "#btnAdd", function (e) {
            e.preventDefault();
            Reset();
            ResetErrorMessage();
            $("#ddlRank").prop("disabled", true);
            setOROMappingModalMode(false);
            showOROMappingModal();
        })
        .off("click.ecmsOROClose", "#AddNewOROMapping [data-dismiss='modal'], #AddNewOROMapping [data-bs-dismiss='modal']")
        .on("click.ecmsOROClose", "#AddNewOROMapping [data-dismiss='modal'], #AddNewOROMapping [data-bs-dismiss='modal']", function (e) {
            e.preventDefault();
            hideOROMappingModal();
        });

    $(document)
        .off("keydown.ecmsOROModal")
        .on("keydown.ecmsOROModal", function (e) {
            const modalElement = document.getElementById("AddNewOROMapping");
            if (e.key === "Escape" && modalElement && modalElement.classList.contains("show")) {
                hideOROMappingModal();
            }
        });

    $("#AddNewOROMapping")
        .off("shown.bs.modal.ecmsORO hidden.bs.modal.ecmsORO")
        .on("shown.bs.modal.ecmsORO", function () {
            document.body.classList.add("ecms-oro-modal-open");
        })
        .on("hidden.bs.modal.ecmsORO", function () {
            document.body.classList.remove("ecms-oro-modal-open");
            removeStaleOROMappingBackdrops();
        });

    mMsater(0, "ddlRO", RecordOffice, "");
    mMsater(0, "ddlRank", Rank, "");
    GetArmsList("ddlArmedIdList", 0);
    initializeOROArmsSelect();

    if (typeof applyDataTableSearchValidation === "function") {
        applyDataTableSearchValidation('#tbldata');
    }

    $("#btnOROMappingAdd")
        .off("click.ecmsOROSave")
        .on("click.ecmsOROSave", function () {
            Proceed();
        });

    $("#btnOROMappingReset")
        .off("click.ecmsOROReset")
        .on("click.ecmsOROReset", function () {
            Reset();
            ResetErrorMessage();
            setOROMappingModalMode(false);
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
                            }));
                        }
                        else {
                            $("#txtUnitName").val("");
                            UnitMapId = 0;
                            $("#ddlTDMId").find("option").not(":first").remove();
                            $("#ddlTDMId").val("0");
                            alert("Unit not found.");
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
                            Swal.fire({ text: errormsg });
                        }
                        else {
                            var listItemddl = '<option value="0">Please Select</option>';
                            for (var i = 0; i < response.length; i++) {
                                listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                            }
                            $("#ddlTDMId").html(listItemddl);
                        }
                    }
                },
                error: function () {
                    Swal.fire({ text: errormsg002 });
                }
            });
        }
    });

    $('#txtUnitName').on('keyup', function (e) {
        if (e.key === 'Delete' || e.key === 'Backspace') {
            $(this).val('');
            UnitMapId = 0;
            $('#ddlTDMId').find('option:not(:first)').remove();
            $('#ddlTDMId').val('0');
        }
    });

    $("#ddlRank").prop('disabled', true);

    try {
        BindData();
    } catch (error) {
        console.error("ORO Mapping DataTable initialisation failed:", error);
    }
});

function Proceed() {
    ResetErrorMessage();
    const selectedArms = $('#ddlArmedIdList').val() || [];
    if (($("#ddlRank").val() == 0 || $("#ddlRank").val() == "null") && selectedArms.length === 0) {
        toastr.error('Rank / Arms: at least one is required.');
        return false;
    }

    let formId = '#SaveOROMapping';
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
        $("#tbldata").empty();       // UI FIX: remove any old DataTables cloned sizing/header markup
    }
    const columns = getColumnsForOROMapping();
    table = $("#tbldata").DataTable({
        scrollY: '100%',          // UI FIX: internal vertical scroll height; keeps last row visible
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,     // Collapse unused space while keeping variable-height rows scrollable
        scroller: false,          // UI FIX: disable Scroller because rows have variable height lists
        deferScroll: false,       // UI FIX: normal DataTable scroll prevents hidden last rows
        fixedHeader: false,       // ❌ disable when using scrollY

        processing: false,
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
                let response = await fetch("/Master/GetAllOROMapping_Pagination", {
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
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
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
                title: 'E-IASC_OROMapping',
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
            adjustOROMappingTable();


            // Handle zoom/resize
            var resizeTimer;
            $(window).on('resize', function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    adjustOROMappingTable();
                }, 100);
            });
        },
        drawCallback: function (settings) {

            // Recalculate widths on each data load
            adjustOROMappingTable();

            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            if (window.bootstrap && bootstrap.Tooltip) {
                tooltipTriggerList.forEach(el => {
                    try { new bootstrap.Tooltip(el); } catch (e) { }
                });
            }

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.OROMappingId != null) {
                    Reset();
                    ResetErrorMessage();
                    $("#ddlRank").prop('disabled', true);
                    $("#ddlRO").prop('disabled', true);
                    OROMappingId = rowData.OROMappingId;
                    $("#ddlRO").val(rowData.RecordOfficeId);

                    if (rowData.RankId != null) {
                        $("#ddlRank").val(rowData.RankId);
                    }
                    else {
                        $("#ddlRank").val("0");
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
                    let arr2 = rowData.ArmedIdList ? rowData.ArmedIdList.split(',') : [];

                    $("#ddlArmedIdList").val(arr2);
                    $("#ddlArmedIdList").trigger("change");

                    setOROMappingModalMode(true);
                    showOROMappingModal();
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.OROMappingId != null) {
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
                            Delete(rowData.OROMappingId);
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
    const selectedArms = $("#ddlArmedIdList").val() || [];
    const ArmedIds = selectedArms.join(",");

    const payload = {
        "OROMappingId": OROMappingId,
        "ArmedIdList": selectedArms.length > 0 ? ArmedIds : null,
        "RecordOfficeId": $("#ddlRO").val(),
        "RankId": $("#ddlRank").val() == 0 ? null : $("#ddlRank").val(),
        "TDMId": $("#ddlTDMId").val() == 0 ? null : $("#ddlTDMId").val(),
        "UnitId": UnitMapId == 0 ? null : UnitMapId,
    };
    let jsonData = JSON.stringify(payload);

    let encrypted = encryptPayloadData(jsonData);

    $.ajax({
        url: '/Master/SaveOROMapping',
        type: 'POST',
        data: { Request: encrypted },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {

            if (result.Result == true) {
                toastr.success(result.Message);
                hideOROMappingModal();
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
    OROMappingId = 0;
    $('#ddlArmedIdList').val(null).trigger('change');
    $("#ddlRO").val("0");
    $("#ddlRank").val("0");
    $("#txtUnitName").val("");
    $("#ddlTDMId").val("0");
    UnitMapId = 0;
    $("#ddlRO").prop('disabled', false);
    $("#ddlRank").prop('disabled', true);
    setOROMappingModalMode(false);
}
function ResetErrorMessage() {
    $("#ddlArmedIdList-error").html("");
    $("#ddlRO-error").html("");
    $("#ddlRank-error").html("");
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
        "OROMappingId": Id,

    };
    $.ajax({
        url: '/Master/DeleteOROMapping',
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
function GetArmsList(ddl, sectid) {
    $.ajax({
        url: '/Master/GetArmsList',
        contentType: 'application/x-www-form-urlencoded',
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

                    var count = 1;
                    for (var i = 0; i < response.length; i++) {

                        listItemddl += '<option value="' + response[i].ArmedId + '">' + count + '. ' + response[i].ArmedName + '</option>';
                        count++;
                    }
                    $("#" + ddl + "").html(listItemddl);


                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    //}


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
function getColumnsForOROMapping() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "OROMappingId",
            name: "OROMappingId",
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
            width: "150px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Arms / Service",
            data: "ArmNameList",
            name: "ArmNameList",
            className: "nowrap",
            width: "250px",
            orderable: false,
            searchable: false,
            render: function (data, type, row, meta) {
                let listItem = "";
                if (data != null) {
                    var armsArray = data.split('#');
                    if (armsArray != null) {
                        listItem += "<ul class='ecms-oro-arms-list'>";
                        for (var j = 0; j < armsArray.length; j++) {
                            listItem += "<li>" + armsArray[j] + "</li>";
                        }
                        listItem += "</ul>";
                    }
                    return listItem;
                }
                else {
                    return ``;
                }
            }
        },
        {
            title: "Rank",
            data: null,
            name: null,
            className: "text-center nowrap",
            width: "150px",
            orderable: true,
            render: function (data, type, row, meta) {
                if (row.RankId != null) {
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${row.RankName}">${row.RankName}</span>`;
                }
                else {
                    return ``;
                }
            }
        },
        {
            title: "Linked Domain ID & Pers Details",
            data: null,
            name: null,
            className: "nowrap",
            width: "200px",
            orderable: false,
            searchable: false,
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
            searchable: false,
            className: "noExport text-center col-action",
            width: "120px",
            render: function (data, type, row) {
                let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1' title='Edit ORO mapping' aria-label='Edit ORO mapping' data-bs-toggle='tooltip' data-bs-placement='top'><i class='fas fa-edit' aria-hidden='true'></i></button>
                                <button type='button' class='cls-btnDelete btn btn-icon btn-round btn-danger mr-1' title='Delete ORO mapping' aria-label='Delete ORO mapping' data-bs-toggle='tooltip' data-bs-placement='top'><i class='fas fa-trash-alt' aria-hidden='true'></i></button>`;
                return Action;
            }
        }
    ];
    return columns;
}
