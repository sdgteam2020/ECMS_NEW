var table; // Declare table variable outside the function to preserve the instance
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
        $("#tbldata").DataTable().destroy();
        $("#tbldata").empty(); // Clear old thead/tbody
    }
    const columns = getColumnsForCommand();
    table = $("#tbldata").DataTable({
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        responsive: true,
        order: [[2, 'asc']], // Default sorting on the first column
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
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
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
                title: 'E-IASC_DispatchCard',
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
        },
        drawCallback: function (settings) {

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ComdId != null) {
                    $("#txtComandName").val(rowData.ComdName);
                    $("#txtAbbreviation").val(rowData.ComdAbbreviation);

                    $("#spncomdId").html(rowData.ComdId);
                    $("#spnSOrderby").html(rowData.Orderby);

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
}
function Save() {
    $.ajax({
        url: '/Master/SaveCommand',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        data: { "ComdName": $("#txtComandName").val().trim(), "ComdId": $("#spncomdId").html(), "ComdAbbreviation": $("#txtAbbreviation").val().trim().toUpperCase(), "Orderby": $("#spnSOrderby").html() }, //get the search string
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
    $("#spncomdId").html("0");
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
            title: "Comd / PSO",
            data: "ComdName",
            name: "ComdName",
        },
        {
            title: "Abbreviation",
            data: "ComdAbbreviation",
            name: "ComdAbbreviation",
        },
        {
            title: `Order`,
            data: "Orderby",
            className: "noExport",
            name: "Orderby",
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