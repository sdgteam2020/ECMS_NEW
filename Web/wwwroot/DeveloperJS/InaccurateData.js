var skey = "";
var inaccurateDataTable;

$(function () {
    document.documentElement.classList.add('ecms-inaccurate-scroll-lock');
    document.body.classList.add('ecms-lock-page-scroll', 'ecms-inaccurate-scroll-lock');

    sessionStorage.clear();
    skey = $('#spnhdns').text();
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    initializeInaccurateDataTable();

    $(window)
        .off('pagehide.ecmsInaccurate')
        .on('pagehide.ecmsInaccurate', function () {
            document.documentElement.classList.remove('ecms-inaccurate-scroll-lock');
            document.body.classList.remove('ecms-lock-page-scroll', 'ecms-inaccurate-scroll-lock');
            $(window).off('resize.ecmsInaccurate');
        });
});

function initializeInaccurateDataTable() {
    var $table = $('#tbldatatabledata');

    if (!$table.length || !$.fn.DataTable) {
        return;
    }

    if ($.fn.DataTable.isDataTable($table[0])) {
        $table.DataTable().destroy();
    }

    if (typeof applyDataTableSearchValidation === 'function') {
        applyDataTableSearchValidation('#tbldatatabledata');
    }

    inaccurateDataTable = $table.DataTable({
        scrollY: 'calc(100vh - 500px)',
        scrollX: true,
        scrollCollapse: false,
        fixedHeader: false,
        autoWidth: false,
        responsive: false,
        deferRender: true,
        pageLength: 10,
        lengthMenu: [10, 25, 50, 100],
        order: [],
        columnDefs: [
            {
                targets: [0, 5],
                orderable: false
            },
            {
                targets: '_all',
                orderSequence: ['asc', 'desc']
            }
        ],
        language: {
            search: "",
            searchPlaceholder: "Search Army No / Name",
            emptyTable: "No requests with incorrect details found",
            zeroRecords: "No matching requests found",
            paginate: {
                first: "\u00AB",
                previous: "\u2039",
                next: "\u203A",
                last: "\u00BB"
            }
        },
        pagingType: 'full_numbers',
        dom:
            "<'dt-top d-flex flex-column flex-md-row align-items-stretch align-items-md-center gap-2'l<'ms-md-auto'f>>rt" +
            "<'ecms-dt-footer row g-2'<'col-12 col-md-6 dt-info-col'i><'col-12 col-md-6 dt-page-col'p>>",
        initComplete: function () {
            this.api().columns.adjust();
            bindInaccurateTableResize();
        },
        drawCallback: function () {
            this.api().columns.adjust();
        }
    });
}

function bindInaccurateTableResize() {
    var resizeTimer;

    $(window)
        .off('resize.ecmsInaccurate')
        .on('resize.ecmsInaccurate', function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function () {
                if (inaccurateDataTable) {
                    inaccurateDataTable.columns.adjust();
                }
            }, 100);
        });
}

$("body").on("click", ".cls-btnRetry", function () {
    Swal.fire({
        title: "Are you sure?",
        text: "You want to Retry!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Retry it!"
    }).then((result) => {
        if (result.isConfirmed) {

            //var encryptedArmyNo = encryptData($(this).closest("td").find("#ArmyNo").html(), skey);
            //var encryptedOffType = encryptData($(this).closest("td").find("#OffType").html(), skey);
            //var encryptedRegistrationApplyFor = encryptData($(this).closest("td").find("#RegistrationApplyFor").html(), skey);
            //var encryptedlCardType = encryptData($(this).closest("td").find("#lCardType").html(), skey);


            //sessionStorage.setItem("OffType", encryptedOffType);
            //sessionStorage.setItem("RegistrationApplyFor", encryptedRegistrationApplyFor);
            //sessionStorage.setItem("lCardType", encryptedlCardType);
            //sessionStorage.setItem("ArmyNo", encryptedArmyNo);
            //window.location.href = "/BasicDetail/Registration?Id=MQ==";
        }
    });
});
