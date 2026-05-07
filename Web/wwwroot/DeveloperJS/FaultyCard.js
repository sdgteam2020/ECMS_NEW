var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldata');

    BindData();
    $("#btnAdd").on("click",function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(FaultyCardRequest);
    });
});
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
                let response = await fetch("/BasicDetail/GetAllFaulty", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken,
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
                orderable: false, 
                className: "text-center col-sno",
                width: "60px",
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                title: "Army No",
                data: "ServiceNo",
                name: "ServiceNo",
                className: "nowrap",
                width: "120px",
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
                title: "Rk & Name",
                data: null,
                name: "FromName",
                className: "nowrap",
                width: "180px",
                orderable: false,
                render: function (data, type, row) {
                    let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                    if (!fullName) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${fullName}">${fullName}</span>`;
                }
            },
            {
                title: "Unit",
                data: "UnitAbbreviation",
                name: "UnitAbbreviation",
                className: "nowrap",
                width: "150px",
                orderable: false,
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "Date & Time",
                data: "UpdatedOn",
                name: "UpdatedOn",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    return DateFormateddMMyyyyhhmmss(data);
                }
            },
            {
                title: "Reason",
                data: "RemarksIds",
                name: "RemarksIds",
                className: "",
                width: "100px",
                orderable: false,
                render: function (data, type, row) {
                    if (data != null) {
                        return `<button type='button' class='cls-remarks btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i></button>`;
                    }
                    else {
                        return `NA`;
                    }
                    return data;
                }
            },
            {
                title: "Remark",
                data: "FromRemark",
                name: "FromRemark",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            {
                title: "AFSAC Remark",
                data: "ToRemark",
                name: "ToRemark",
                className: "",
                width: "150px",
                render: function (data, type, row) {
                    if (!data) return '';
                    return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
                }
            },
            // Additional column for Edit action
            {
                title: "Action",
                data: "IsEditAction",
                name: "Action",
                className: "noExport nowrap",
                width: "150px",
                orderable: false,
                render: function (data, type, row) {
                    if (data == false) {
                        return `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button>`;
                    } else {
                        return `NA`;
                    }
                }
            }
        ],
        columnDefs: [
            {
                targets: '_all',  // Apply to all visible columns
                orderSequence: ["asc", "desc"]  // ⬅️ ONLY 2 states!
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Army No" // Add custom placeholder
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
                title: 'E-IASC_FaultyCard',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        // 👇 Show modal only after table (header + data) is fully rendered
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
                if (rowData.EncryptedId != null) {
                    sessionStorage.setItem("ArmyNo", rowData.ServiceNo);
                    window.location.href = '/BasicDetail/FaultyCardRequest?Id=' + encodeURIComponent(rowData.EncryptedId);
                }
            });

            $("#tbldata tbody").off("click", ".cls-remarks").on("click", ".cls-remarks", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RemarksIds != null) {
                    GetRemarksData(rowData.RemarksIds);
                }
            });
        }
    });
    if ($("#spnCValue").html().toLowerCase() === "true") {
        table.column(8).visible(true);
    }
    else {
        table.column(8).visible(false);
    }
}

async function GetRemarksData(remarksRemarksIds) {

    let param = new URLSearchParams({ RemarksIds: remarksRemarksIds });

    try {
        const response = await fetch('/BasicDetail/GetRemarksData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (result != null) {
            var remarksArray = result.split('#');
            if (remarksArray != null) {
                var listItem="";
                listItem += "<ul>";
                for (var j = 0; j < remarksArray.length; j++) {
                    listItem += "<li>" + remarksArray[j] + "</li>";
                }
                listItem += "</ul>";
                $("#MessageDialogLabel").html('Reason');
                $("#MessageDialogBody").html(listItem);
                $("#MessageDialog").modal('show');
            }

        } else {
            toastr.error('Invalid Input.');
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}