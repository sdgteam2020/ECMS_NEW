
var table; // Declare table variable outside the function to preserve the instance
let Orderby = 0;
let RankId = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    Reset();
    BindData(function () { });
    $("#btnReset").on("click", function () {
        Reset();
    });
   
    $("#btnsave").on("click",function () {
        if ($("#SaveForm")[0].checkValidity()) {

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
           
        } else {
            $("#SaveForm")[0].reportValidity();
        }
    });

    $('#btnMultiDelete').on("click", function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {
                var id = $(this).attr("Id");
                lst.push(id);
            });
          
            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Delete",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Delete it!'
            }).then((result) => {
                if (result.value) {
                    DeleteMultiple(lst);
                }
            });
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 data to Delete."
            });
        }
    });
});

function BindData() {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
    }
    const columns = getColumnsForRank();
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
        order: [[4, 'asc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order?.[0]?.column >= 0 && data.columns?.[data.order[0].column]?.data || '',
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
            };
            try {
                let response = await fetch("/Master/GetAllRank_Pagination", {
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
            { targets: 0, width: "0px" },
            { targets: 1, width: "60px" },
            { targets: 2, width: "200px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "200px" },
            { targets: 5, width: "120px" },
            {
                targets: '_all',
                orderSequence: ["asc", "desc"]
            },
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip",
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
                title: 'E-IASC_Rank',
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

            $("#tbldata tbody").off("click", ".cls-btnorder").on("click", ".cls-btnorder", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.Orderby != null) {

                    OrderByChange(rowData.RankId, rowData.Orderby);
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RankId != null) {

                    /*  $("#AddNewM").modal('show');*/
                    $("#txtRank").val(rowData.RankName);
                    $("#txtAbbreviation").val(rowData.RankAbbreviation);
                    $("#ddlRankType").val(rowData.ApplyForId);
                    RankId = rowData.RankId;
                    Orderby = rowData.Orderby;
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.RankId != null) {
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
                            Delete(rowData.RankId);
                        }
                    });
                }
                else {
                    //Invalid Data
                }
            });


        }
    });
}
function Save() {
    $.ajax({
        url: '/Master/SaveRank',
        type: 'POST',
        data: {
            "ApplyForId": $("#ddlRankType").val(),
            "RankName": $("#txtRank").val().trim(),
            "RankId": RankId,
            "RankAbbreviation": $("#txtAbbreviation").val().trim(),
            "Orderby": Orderby
        }, //get the search string
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Rank has been saved');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
            }
            else if (result == DataUpdate) {
                toastr.success('Rank has been Updated');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
            }
            else if (result == DataExists) {

                toastr.error('Rank Name Exits!');

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

function Reset() {
    $("#txtRank").val("");
    $("#txtAbbreviation").val("");
    $("#ddlRankType").val("");
    RankId = 0;
    Orderby = 0;
}

function Delete(RankId) {
    var userdata =
    {
        "RankId": RankId,   

    };
    $.ajax({
        url: '/Master/DeleteRank',
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
                else if (response == "5") {
                    toastr.error('RankId is used in child table.');
                }
                else if (response == Success) {
                    toastr.success('Rank Selected');
                    BindData();
                }
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

function DeleteMultiple(ComdId) {
   
    var userdata =
    {
        "ints": ComdId,

    };
    $.ajax({
        url: '/Master/DeleteRankMultiple',
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

function OrderByChange(RankId, OrderBy) {
   
    var userdata =
    {
        "RankId": RankId,
        "Orderby": OrderBy,

    };
    $.ajax({
        url: '/Master/RankOrderByChange',
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
                    toastr.success('Order Changed Success');
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
function getColumnsForRank() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "RankId",
            name: "RankId",
            visible: false,
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
            title: "Rank",
            data: "RankName",
            name: "RankName",
            className: "nowrap",
            width: "200px",
            orderable: true, 
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Abbreviation",
            data: "RankAbbreviation",
            name: "RankAbbreviation",
            className: "nowrap",
            width: "200px",
            orderable: true, 
            render: function (data, type, row, meta) {
                if (!data) return '';
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Order",
            data: "Orderby",
            name: "Orderby",
            className: "noExport nowrap",
            width: "200px",
            orderable: true, 
            render: function (data, type, row, meta) {
                const api = meta.settings.oInstance.api();
                const pageInfo = api.page.info();

                const isLastRowOnPage =
                    meta.row === api.rows({ page: 'current' }).count() - 1;

                const isLastPage =
                    pageInfo.page === pageInfo.pages - 1;

                if (isLastRowOnPage && isLastPage) {
                    return `<span class="badge bg-secondary">Last</span>`;
                }

                return `<button class="cls-btnorder btn btn-info btn-sm"><i class="fas fa-arrow-down"></i></button>`;
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
            width: "120px",
            render: function (data, type, row) {
                let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>`;
                return Action;
            }
        }
    ];
    return columns;
}