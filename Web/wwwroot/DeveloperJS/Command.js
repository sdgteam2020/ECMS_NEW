var table; // Declare table variable outside the function to preserve the instance
let ComdId = 0;
let Orderby = 0;
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData(function () {
    });
    $("#btnReset").on("click",function () {
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

       
       
       // 

    });

});

function BindData(callback) {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        // Destroy the DataTable and clear the table content
        $("#tbldata").DataTable().clear().destroy(); // Clear and destroy DataTable properly
        $("#tbldata thead").empty(); // Clear old thead
        $("#tbldata tbody").empty(); // Clear old tbody
    }

    const columns = getColumnsForCommand();
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
        order: [[2, 'asc']], // Default sorting on the first column
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
                let response = await fetch("/Master/GetAllCommand_Pagination", {
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
            { targets: 1, width: "60px" },
            { targets: 2, width: "200px" },
            { targets: 3, width: "200px" },
            { targets: 4, width: "110px" },
            { targets: 5, width: "250px" },
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
                title: 'E-IASC_Command',
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

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ComdId != null) {
                    $("#txtComandName").val(rowData.ComdName);
                    $("#txtAbbreviation").val(rowData.ComdAbbreviation);
                    ComdId = rowData.ComdId;
                    Orderby = rowData.Orderby;
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btntreeview").on("click", ".cls-btntreeview", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ComdId != null) {
                    $("#treeview").modal('show');
                    GetBinaryTree(rowData.ComdId)
                }
                else {
                    //Invalid Data
                }
            });
            $("#tbldata tbody").off("click", ".cls-btnorder").on("click", ".cls-btnorder", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ComdId != null && rowData.Orderby != null) {
                    OrderByChange(rowData.ComdId, rowData.Orderby);
                }
                else {
                    //Invalid Data
                }
            });

            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ComdId != null) {
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

                            Delete(rowData.ComdId);

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
    $.ajax({
        url: '/Master/SaveCommand',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        data: {
            "ComdName": $("#txtComandName").val().trim(),
            "ComdId": ComdId,
            "ComdAbbreviation": $("#txtAbbreviation").val().trim().toUpperCase(),
            "Orderby": Orderby
        }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Data has been saved');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
            }
            else if (result == DataUpdate) {
                toastr.success('Data has been Updated');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
            }
            else if (result == DataExists) {

                toastr.error('Comd / PSO Name Exits!');

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
    $("#txtComandName").val("");
    $("#txtAbbreviation").val("");
    ComdId = 0;
    Orderby = 0;
}

function Delete(ComdId) {
    var userdata =
    {
        "ComdId": ComdId,

    };
    $.ajax({
        url: '/Master/DeleteCommand',
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
                else if (response == "5")
                {
                    toastr.error('ComdId is used in child table.');
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

function OrderByChange(ComdId, OrderBy) {
   
    var userdata =
    {
        "ComdId": ComdId,
        "Orderby": OrderBy,

    };
    $.ajax({
        url: '/Master/OrderByChange',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        type: 'POST',
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

function GetBinaryTree(ComdId) {
    var listitem = "";
    var userdata =
    {
        "Id": ComdId,
        

    };
    $.ajax({
        url: '/Master/GetBinaryTree',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        type: 'POST',
        success: function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else {
                    var MComd = response.MComd
                    var MCorps = response.MCorps
                    var MDiv = response.MDiv
                    var MBde = response.MBde
                    var Unit = response.Unit

              

                    listitem += ' <ul class="bullet-list-round">';
                    listitem += ' <li>';
                   
                   

                 
                    for (var i = 0; i < MComd.length; i++) {
                        listitem += '<a href="#" class="bg-danger text-white">' + MComd[i].ComdName + '</a>';
                        listitem += ' <ul class="bullet-list-round">';
                      
                      
                            for (var C = 0; C < MCorps.length; C++) {

                                listitem += '<li><a href="#" class="bg-warning text-white">' + MCorps[C].CorpsName + '</a>';
                                
                                //////////////Div in Corps
                                listitem += '<ul class="bullet-list-round">';
                                for (var C1 = 0; C1 < MDiv.length; C1++) {
                                  /*  if (C1 == 0)*/
                                       

                                    if (MCorps[C].CorpsId == MDiv[C1].CorpsId) {
                                        listitem += '<li><a href="#" class="bg-primary text-white">' + MDiv[C1].DivName + '</a>';

                                        listitem += '<ul class="bullet-list-round">';
                                      
                                    //////////////Bde direvct in Div

                                    for (var db1 = 0; db1 < MBde.length; db1++) {


                                        if (MCorps[C].CorpsId == MBde[db1].CorpsId && MDiv[C1].CorpsId == MBde[db1].CorpsId && MBde[db1].DivId == MDiv[C1].DivId) {

                                            listitem += '<li><a href="#" class="bg-info text-white">' + MBde[db1].BdeName + '</a>';
                                             //////////////unit direvct in bde
                                           
                                            var unitcount = 0;
                                            for (var unit1 = 0; unit1 < Unit.length; unit1++) {


                                                if (MCorps[C].CorpsId == Unit[unit1].CorpsId && MDiv[C1].DivId == Unit[unit1].DivId && MBde[db1].BdeId == Unit[unit1].BdeId ) {
                                                    if (parseInt(unitcount) == 0)
                                                        listitem += '<ul>';

                                                    listitem += '<li><a href="#" class="bg-success text-white">' + Unit[unit1].UnitName + '</a>';
                                                    //////////////unit direvct in bde

                                                    unitcount = 1;
                                                    //////////////end unit direvct in bde
                                                    listitem += '</li>';
                                                }

                                                if (parseInt(unit1) + 1 == Unit.length && parseInt(unitcount) == 1)
                                                    listitem += '</ul>';

                                            }
                                           

                                              //////////////end unit direvct in bde
                                            listitem += '</li>';
                                        }



                                    }  //////   end    Bde direvct in Div
                                        listitem += '</ul>';

                                        listitem += '</li>';
                                    }

                                   
                                    //listitem += '</ul>';
                                   
                                    /*if (parseInt(C1)+1 == MDiv.length)*/
                                       
                                }
                                ////////////Bde direvct in Corps

                                for (var C1 = 0; C1 < MBde.length; C1++) {


                                    if (MCorps[C].CorpsId == MBde[C1].CorpsId && MBde[C1].DivId == 1) {

                                        listitem += '<li><a href="#" class="bg-info text-dark">' + MBde[C1].BdeName + '</a></li>';

                                    }



                                }  //////   end    Bde direvct in Corps

                                ////////////Unit direvct in Corps

                                for (var C1 = 0; C1 < Unit.length; C1++) {


                                    if (MCorps[C].CorpsId == Unit[C1].CorpsId && Unit[C1].DivId == 1 && Unit[C1].BdeId == 1) {

                                        listitem += '<li><a href="#" class="bg-success text-white">' + Unit[C1].UnitName + '</a></li>';

                                    }



                                }  //////   end    Unit direvct in Corps


                                listitem += '</ul>';
                               
                                listitem += '</li>';
                        }
                        for (var C = 0; C < MDiv.length; C++) {

                            if (MDiv[C].CorpsId==1)
                                listitem += '<li><a href="#" class="bg-primary text-white">' + MDiv[C].DivName + '</a></li>';


                        }
                        for (var C = 0; C < MBde.length; C++) {

                            if (MBde[C].DivId == 1 && MBde[C].CorpsId == 1)
                                listitem += '<li><a href="#" class="bg-info text-dark">' + MBde[C].BdeName + '</a></li>';


                        }
                        for (var C = 0; C < Unit.length; C++) {

                            if (Unit[C].BdeId == 1 && Unit[C].DivId == 1 && Unit[C].CorpsId == 1)
                                listitem += '<li><a href="#" class="bg-success text-white">' + Unit[C].UnitName + '</a></li>';


                        }
                       
                        listitem += ' </ul>';
                    }
                  
                
                    listitem += ' </li>';
                    listitem += ' </ul>';

                    $("#tree").html(listitem);
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

function getColumnsForCommand() {
    let columns = [];
    columns = [
        {
            title: "",
            data: "ComdId",
            name: "ComdId",
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
            title: "Comd / PSO",
            data: "ComdName",
            name: "ComdName",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, rowData) {
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: "Abbreviation",
            data: "ComdAbbreviation",
            name: "ComdAbbreviation",
            className: "nowrap",
            width: "200px",
            orderable: true,
            render: function (data, type, rowData) {
                return `<span class="dt-ellipsis" data-bs-toggle="tooltip" data-bs-placement="top" title="${data}">${data}</span>`;
            }
        },
        {
            title: `Order`,
            data: "Orderby",
            className: "noExport",
            name: "Orderby",
            width: "110px",
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

                return `<button class="cls-btnorder btn btn-info btn-sm">
                <i class="fas fa-arrow-down"></i>
            </button>`;
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
            width: "250px",
            render: function (data, type, row) {
                let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>
                                <button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>
                                    <button type='button' class='cls-btntreeview btn btn-primary  mr-1'>Hierarchy Chart</button>`;
                return Action;
            }
        }
    ];
    return columns;
}