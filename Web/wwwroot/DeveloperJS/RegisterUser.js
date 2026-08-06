$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldata');

    BindData();
});

function BindData() {
    var listItem = "";

    $.ajax({
        url: '/Home/GetAllRegisterUser',
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
                else if (response.length == 0) {
                    destroyRegisterUserDataTable();

                    $("#DetailBody").html(listItem);

                    var memberTable = $('#tbldata').DataTable({
                        autoWidth: false,
                        fixedHeader: false,
                        lengthChange: false,
                        searching: false,
                        order: [[0, "asc"]],
                        dom: "rt<'row ecms-dt-footer'<'col-sm-12 col-md-6 dt-info-col'i><'col-sm-12 col-md-6 dt-page-col'p>>",
                        language: {
                            emptyTable: "No data available"
                        },
                        initComplete: function () {
                            applyRegisterUserDataTableTheme(this.api());
                        },
                        drawCallback: function () {
                            applyRegisterUserDataTableTheme(this.api());
                        }
                    });

                    moveRegisterUserExportButtons(memberTable);
                }
                else {
                    destroyRegisterUserDataTable();

                    for (var i = 0; i < response.length; i++) {
                        listItem += "<tr>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'>" + response[i].DomainId + "</td>";

                        if (response[i].ArmyNo != null && response[i].ArmyNo != "null")
                            listItem += "<td class='align-middle'>" + response[i].ArmyNo + "</td>";
                        else
                            listItem += "<td class='align-middle'><span class='badge badge-pill badge-danger ecms-status-no' id='domain_approval'>IC No Not Mapped</span></td>";

                        if (response[i].Rank != null && response[i].Rank != "null")
                            listItem += "<td class='align-middle'>" + response[i].Rank + "</td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger ecms-status-no' id='domain_approval'>NA</span></span></td>";

                        if (response[i].Name != null && response[i].Name != "null")
                            listItem += "<td class='align-middle'>" + response[i].Name + "</td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger ecms-status-no' id='domain_approval'>NA</span></span></td>";

                        listItem += "<td class='align-middle'>" + response[i].AppointmentName + "</td>";
                        listItem += "</tr>";
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length - 1);

                    var memberTable = $('#tbldata').DataTable({
                        autoWidth: false,
                        fixedHeader: false,
                        retrieve: true,
                        lengthChange: false,
                        searching: false,
                        order: [[0, "asc"]],
                        dom: "Brt<'row ecms-dt-footer'<'col-sm-12 col-md-6 dt-info-col'i><'col-sm-12 col-md-6 dt-page-col'p>>",
                        buttons: [
                            //{
                            //    extend: 'copy',
                            //    exportOptions: {
                            //        columns: "thead th:not(.noExport)"
                            //    }
                            //},
                            {
                                extend: 'excel',
                                text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i><span>Excel</span>',
                                exportOptions: {
                                    columns: "thead th:not(.noExport)"
                                }
                            }, {
                                extend: 'pdfHtml5',
                                text: '<i class="fa fa-file-pdf-o" aria-hidden="true"></i><span>PDF</span>',
                                orientation: 'portrait',
                                pageSize: 'A4',
                                title: 'E-IASC_User_Regn',
                                exportOptions: {
                                    columns: "thead th:not(.noExport)"
                                },
                                customize: function (doc) {
                                    WaterMarkOnPdf(doc);
                                }
                            }
                        ],
                        initComplete: function () {
                            applyRegisterUserDataTableTheme(this.api());
                        },
                        drawCallback: function () {
                            applyRegisterUserDataTableTheme(this.api());
                        }
                    });

                    moveRegisterUserExportButtons(memberTable);
                    memberTable.columns.adjust();
                }
            }
            else {
                destroyRegisterUserDataTable();

                $("#DetailBody").html(listItem);

                var memberTable = $('#tbldata').DataTable({
                    autoWidth: false,
                    fixedHeader: false,
                    lengthChange: false,
                    searching: false,
                    order: [[0, "asc"]],
                    dom: "rt<'row ecms-dt-footer'<'col-sm-12 col-md-6 dt-info-col'i><'col-sm-12 col-md-6 dt-page-col'p>>",
                    language: {
                        emptyTable: "No data available"
                    },
                    initComplete: function () {
                        applyRegisterUserDataTableTheme(this.api());
                    },
                    drawCallback: function () {
                        applyRegisterUserDataTableTheme(this.api());
                    }
                });

                moveRegisterUserExportButtons(memberTable);
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}

/* UI-only helper: safely destroys the current DataTable before rebinding. */
function destroyRegisterUserDataTable() {
    if ($.fn.DataTable.isDataTable('#tbldata')) {
        $('#tbldata').DataTable().destroy();
    }

    $('#registerUserTableActions').empty();
}

/* UI-only helper: places the existing export buttons in the page toolbar. */
function moveRegisterUserExportButtons(memberTable) {
    if (!memberTable || typeof memberTable.buttons !== 'function') {
        return;
    }

    var $buttonContainer = memberTable.buttons().container();
    var $pageToolbar = $('#registerUserTableActions');

    $buttonContainer.find('button, a').addClass('btn btn-primary btn-sm');

    if ($pageToolbar.length) {
        $pageToolbar.empty().append($buttonContainer);
    }
    else {
        $buttonContainer.appendTo('#tbldata_wrapper .col-md-6:eq(0)');
    }
}

/* UI-only helper: maps generated DataTable regions to shared theme classes. */
function applyRegisterUserDataTableTheme(memberTable) {
    if (!memberTable) {
        return;
    }

    var $wrapper = $(memberTable.table().container());
    var $footer = $wrapper.find('.ecms-dt-footer');

    $footer.find('.dt-info-col').addClass('d-flex align-items-center');
    $footer.find('.dt-page-col').addClass('d-flex align-items-center justify-content-md-end');

    memberTable.columns.adjust();
}

/* UI-only resize correction for responsive column alignment. */
$(window).off('resize.registerUserTable').on('resize.registerUserTable', function () {
    if ($.fn.DataTable.isDataTable('#tbldata')) {
        $('#tbldata').DataTable().columns.adjust();
    }
});
