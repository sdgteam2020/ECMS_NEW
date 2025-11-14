var table; // Declare table variable outside the function to preserve the instance
$(function () {
    Reset();
    BindData()
    $("#txtSerachunit").on("keyup",function () {
        BindData()
    });
    $("#btnReset").on("click",function () {
        Reset();
    });
    $("#btnsave").on("click", function () {
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

    $('#btnMultiDelete').on("click", function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);
                console.log(id);

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
    $("#tbldata").DataTable().destroy();    
    table = $("#tbldata").DataTable({
                processing: true,
                serverSide: true,
                filter: true,
                stateSave: true,
                order: [[0, 'desc']], // Default sorting on the first column
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
                        let response = await fetch("/Master/GetAllUnit", {
                            method: "POST",
                            headers: { "Content-Type": "application/x-www-form-urlencoded" },
                            body: new URLSearchParams(requestData).toString()
                        });

                        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                        let result = await response.json();
                        callback(result); // Sends data to DataTables

                    }catch (error) {
                        console.error("Error fetching data:", error);
                    }
                },
        columns: [
        { data: "UnitId", name: "UnitId", visible: false },
        // Serial number column
        {
            data: null,
            name: "SerialNumber",
            orderable: false, // Disable sorting for this column
            render: function (data, type, row, meta) {
                // Calculate serial number based on row index
                return meta.row + meta.settings._iDisplayStart + 1;
            }
        },
        { data: "Sus_no", name: "Sus_no" },
        { data: "Suffix", name: "Suffix" },
        { data: "UnitName", name: "UnitName", orderable: false },
        { data: "Abbreviation", name: "Abbreviation", orderable: false },
        // Display user-friendly value for IsVerify
        {
            data: "IsVerify",
            name: "IsVerify",
            render: function (data, type, row) {
                // Convert boolean to "Yes" or "No"
                return data ? "<span class='badge badge-pill badge-success'>Verifed</span>" : "<span class='badge badge-pill badge-danger'>Not Verify</span>";
            }
        },
        // Additional column for Edit action
        {
            data: null,
            orderable: false,
            render: function (data, type, row) {
                return "<span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>";
            }
        }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "UNIT SUS No" // Add custom placeholder
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
            title: 'E-IASC_Unit',
            exportOptions: {
                columns: "thead th:not(.noExport)"
            },
            customize: function (doc) {
                WaterMarkOnPdf(doc)
            }
        }],
        drawCallback: function (settings) {
            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#txtSusno").val(rowData.Sus_no);
                    $("#txtSuffix").val(rowData.Suffix);
                    $("#txtUnitDesc").val(rowData.UnitName);
                    $("#txtAbbreviation").val(rowData.Abbreviation);
                    $("#spnUnitId").html(rowData.UnitId);
                }
            });
            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
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
                            Delete(rowData.UnitId);
                        }
                    });
                }
            });
        }
    });
}

function Save() {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Master/SaveUnit',
        type: 'POST',
        data: { "Sus_no": $("#txtSusno").val().trim(), "UnitId": $("#spnUnitId").html().trim(), "Suffix": $("#txtSuffix").val().trim(), "UnitName": $("#txtUnitDesc").val().trim(), "Abbreviation": $("#txtAbbreviation").val().trim(), "IsVerify": true }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Unit has been saved');

                /*  $("#AddNewM").modal('hide');*/
               /* $("#tbldata").DataTable().destroy();*/    
                BindData();
                Reset();
            }
            else if (result == DataUpdate) {
                toastr.success('Unit has been Updated');

                /*  $("#AddNewM").modal('hide');*/
                $("#tbldata").DataTable().destroy();    
                BindData();
                Reset();
            }
            else if (result == DataExists) {

                toastr.error('Unit Name Exits!');

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
    $("#txtSusno").val("");
    $("#txtSuffix").val("");
    $("#txtUnitDesc").val("");
    $("#txtAbbreviation").val("");
    $("#spnUnitId").html("0");
}

function Delete(Id) {
    var userdata =
    {
        "UnitId": Id,

    };
    $.ajax({
        url: '/Master/DeleteUnit',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == "5") {
                    toastr.error('UnitId is used in child table.');
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

function DeleteMultiple(Id) {

    var userdata =
    {
        "ints": Id,

    };
    $.ajax({
        url: '/Master/DeleteUnitMultiple',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
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