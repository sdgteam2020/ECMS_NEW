var memberTable = null;

$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    prepareLoginLogModal();

    const today = new Date();
    const oneYearAgo = new Date();
    oneYearAgo.setFullYear(today.getFullYear() - 1);

    $("#FmDate").datepicker({
        dateFormat: "yy-mm-dd",
        minDate: oneYearAgo,
        maxDate: today,
        onSelect: function (dateText) {
            $("#ToDate").datepicker("option", "minDate", dateText);
        }
    });

    $("#ToDate").datepicker({
        dateFormat: "yy-mm-dd",
        minDate: oneYearAgo,
        maxDate: today
    });

    $("body")
        .off("click.loginLog", ".cls-btnhistory")
        .on("click.loginLog", ".cls-btnhistory", function () {
            const $row = $(this).closest("tr");
            const domainId = $.trim($row.find(".DomainID").text());
            const armyNo = $.trim($row.find(".ArmyNo").text());
            const rankName = $.trim($row.find(".RankName").text());
            const userName = $.trim($row.find(".Name").text());
            const aspNetUsersId = $.trim($row.find("#AspNetUsersId").text());
            const fromDateText = $("#FmDate").val();
            const toDateText = $("#ToDate").val();

            $(".loginlodetails").text(
                domainId + " (" + [armyNo, rankName, userName].filter(Boolean).join(" ") + ")"
            );

            if (fromDateText === "" && toDateText === "") {
                showLoginLogModal();
                GetLog(aspNetUsersId);
                return;
            }

            if (fromDateText === "" || toDateText === "") {
                toastr.error("Please Select Both From and To Date");
                return;
            }

            const fromDate = new Date(fromDateText);
            const toDate = new Date(toDateText);

            if (Number.isNaN(fromDate.getTime()) || Number.isNaN(toDate.getTime()) || fromDate > toDate) {
                toastr.error("Please Select Valid date");
                return;
            }

            showLoginLogModal();
            GetLog(aspNetUsersId, fromDateText, toDateText);
        });
});

/* UI-only helper: move the modal out of fixed/transformed page wrappers so
   Bootstrap's backdrop can never sit above the dialog. */
function prepareLoginLogModal() {
    const $modal = $("#modalLoginLog");

    if (!$modal.length) {
        return;
    }

    if (!$modal.parent().is("body")) {
        $modal.detach().appendTo(document.body);
    }

    $modal
        .off("show.bs.modal.loginLog shown.bs.modal.loginLog hidden.bs.modal.loginLog")
        .on("show.bs.modal.loginLog", function () {
            if ($(".modal.show").not(this).length === 0) {
                $("body > .modal-backdrop").remove();
            }

            if (!$(this).parent().is("body")) {
                $(this).detach().appendTo(document.body);
            }

            $("body").addClass("ecms-loginlog-modal-open");
        })
        .on("shown.bs.modal.loginLog", function () {
            const $currentModal = $(this);
            const $backdrop = $("body > .modal-backdrop").last();

            if ($backdrop.length) {
                $backdrop.insertBefore($currentModal).css("z-index", 1050);
            }

            $currentModal.css({
                zIndex: 1060,
                opacity: 1,
                filter: "none",
                pointerEvents: "auto"
            });

            $currentModal.find(".modal-dialog, .modal-content").css({
                opacity: 1,
                filter: "none",
                pointerEvents: "auto"
            });
        })
        .on("hidden.bs.modal.loginLog", function () {
            if ($(".modal.show").length === 0) {
                $("body > .modal-backdrop").remove();
                $("body")
                    .removeClass("modal-open ecms-loginlog-modal-open")
                    .css("padding-right", "");
            }
        });
}

/* UI-only helper: supports the Bootstrap jQuery plugin and Bootstrap 5 API. */
function showLoginLogModal() {
    const $modal = $("#modalLoginLog");

    if (!$modal.length) {
        return;
    }

    if (!$modal.parent().is("body")) {
        $modal.detach().appendTo(document.body);
    }

    if (typeof $modal.modal === "function") {
        $modal.modal("show");
        return;
    }

    if (window.bootstrap && bootstrap.Modal) {
        bootstrap.Modal.getOrCreateInstance($modal[0]).show();
    }
}

function setLoginLogLoadingState() {
    destroyLoginLogExportTable();
    $("#DataBoady").empty();
    $("#loginLogExportActions").empty();
    $("#timelineData").html(
        '<span class="timeline-label"><span class="label">Loading Login History...</span></span>'
    );
}

function renderNoLoginLog() {
    destroyLoginLogExportTable();
    $("#DataBoady").empty();
    $("#loginLogExportActions").empty();
    $("#timelineData").html(
        '<span class="timeline-label"><span class="label">No Login Log</span></span>'
    );
}

function destroyLoginLogExportTable() {
    if ($.fn.DataTable && $.fn.DataTable.isDataTable("#tbldata")) {
        $("#tbldata").DataTable().clear().destroy();
    }

    memberTable = null;
}

function moveLoginLogExportButtons(tableInstance) {
    const $target = $("#loginLogExportActions");

    if (!$target.length) {
        return;
    }

    $target.empty();

    if (!tableInstance || typeof tableInstance.buttons !== "function") {
        return;
    }

    tableInstance.buttons().container().appendTo($target);
}

function initialiseLoginLogExportTable() {
    destroyLoginLogExportTable();

    memberTable = $("#tbldata").DataTable({
        autoWidth: false,
        fixedHeader: false,
        lengthChange: false,
        searching: false,
        paging: false,
        info: false,
        ordering: false,
        destroy: true,
        dom: "Bt",
        buttons: [
            {
                extend: "excel",
                text: '<i class="fa fa-file-excel mr-1"></i> Excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            }
        ]
    });

    moveLoginLogExportButtons(memberTable);
}

function GetLog(AspNetUsersId, FmDate, ToDate) {
    const userdata = {
        "AspNetUsersId": AspNetUsersId,
        "FmDate": FmDate,
        "ToDate": ToDate
    };

    setLoginLogLoadingState();

    $.ajax({
        url: "/Log/LoginLogByAspNetUsersId",
        contentType: "application/x-www-form-urlencoded",
        data: userdata,
        type: "POST",
        headers: { "RequestVerificationToken": globalThis.RequestVerificationToken },
        success: function (response) {
            if (response === "null" || response === null || response === -1 || response === 0 || !Array.isArray(response) || response.length === 0) {
                renderNoLoginLog();
                return;
            }

            let timelineHtml = "";
            let exportRowsHtml = "";
            let currentYear = "";
            let lastLoginDate = null;

            for (let i = 0; i < response.length; i++) {
                const item = response[i];
                const loginDate = new Date(item.UpdatedOn);
                const loginYear = loginDate.getFullYear();

                exportRowsHtml += "<tr>";
                exportRowsHtml += "<td>" + (item.RoleName || "") + "</td>";
                exportRowsHtml += "<td>" + (item.DomainID || "") + "</td>";
                exportRowsHtml += "<td>" + (item.ArmyNo || "") + "</td>";
                exportRowsHtml += "<td>" + (item.RankName || "") + "</td>";
                exportRowsHtml += "<td>" + (item.Name || "") + "</td>";
                exportRowsHtml += "<td>" + DateFormateddMMyyyyhhmmss(item.UpdatedOn) + "</td>";
                exportRowsHtml += "<td>" + (item.IP || "") + "</td>";
                exportRowsHtml += "</tr>";

                if (currentYear !== loginYear) {
                    timelineHtml += '<span class="timeline-label"><span class="label">' + loginYear + "</span></span>";
                    currentYear = loginYear;
                }

                timelineHtml += '<div class="timeline-item">';
                timelineHtml += '<div class="timeline-point"></div>';
                timelineHtml += '<div class="timeline-event">';
                timelineHtml += '<div class="widget has-shadow">';
                timelineHtml += '<div class="widget-header d-flex align-items-center">';
                timelineHtml += '<div class="d-flex flex-column mr-auto">';
                timelineHtml += '<div class="title">';
                timelineHtml += '<span class="badge badge-danger">' + DateFormatehhmmss(item.UpdatedOn) + "</span>";
                timelineHtml += "</div></div>";
                timelineHtml += '<div class="widget-options"><div class="dropdown">';
                timelineHtml += '<span class="badge badge-orange">' + DateFormateMMMM_dd_yyyy(item.UpdatedOn) + "</span>";
                timelineHtml += "</div></div></div>";
                timelineHtml += '<div class="widget-body">';
                timelineHtml += '<p class="text-blue mb-0">IP Address : <span class="badge badge-purple">' + (item.IP || "") + "</span></p>";
                timelineHtml += "</div></div>";
                timelineHtml += '<div class="time-right">' + DateCalculateago(item.UpdatedOn) + "</div>";
                timelineHtml += "</div></div>";

                lastLoginDate = loginDate;
            }

            if (lastLoginDate && !Number.isNaN(lastLoginDate.getTime())) {
                timelineHtml += '<span class="timeline-label"><span class="label bg-primary">End ' + lastLoginDate.getFullYear() + "</span></span>";
            }

            $("#timelineData").html(timelineHtml);
            $("#DataBoady").html(exportRowsHtml);
            initialiseLoginLogExportTable();
        },
        error: function () {
            renderNoLoginLog();
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
