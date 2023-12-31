$(document).ready(function () {
  
   
    BindData()
    $("#btnReset").click(function () {
        Reset()();
       

    });
   
    $("#btnsave").click(function () {
        if ($("#SaveForm")[0].checkValidity()) {

            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be Save!",
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

    $('#btnMultiDelete').click(function () {
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
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Master/GetAllCommand',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });

                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=10>No Record Found</td></tr>";
                    $("#tblcommnd").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }
               
                else {

                    $("#tblcommnd").DataTable().destroy();
                   
                    for (var i = 0; i < response.length; i++) {
                        if (response[i].ComdId != 1) {
                            listItem += "<tr>";
                            listItem += "<td class='d-none'><span id='ScomdId'>" + response[i].ComdId + "</span><span id='SOrderby'>" + response[i].Orderby + "</span></td>";
                            listItem += "<td>";
                            listItem += "<div class='custom-control custom-checkbox small'>";
                            listItem += "<input type='checkbox' class='custom-control-input' id='" + response[i].ComdId + "'>";
                            listItem += "<label class='custom-control-label' for='" + response[i].ComdId + "'></label>";
                            listItem += "</div>";
                            listItem += "</td>";
                            listItem += "<td class='align-middle'>" + i + "</td>";
                            listItem += "<td class='align-middle'><span id='comdName'>" + response[i].ComdName + "</span></td>";
                            listItem += "<td class='align-middle'><span id='comdAbbreviation'>" + response[i].ComdAbbreviation + "</span></td>";
                            

                            if (response[i].Orderby != response.length-1)
                                listItem += "<td class='align-middle'><span id=''><button type='button' class='cls-btnorder btn btn-icon btn-round btn-info mr-1'><i class='fas fa-arrow-down'></i></button></span></td>";
                            else
                                listItem += "<td></td>";
                              

                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";


                            /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                            listItem += "</tr>";
                        }
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length-1);
                  
                    memberTable = $('#tblcommnd').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        "order": [[2, "asc"]],
                        buttons: [{
                            extend: 'copy',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }, {
                            extend: 'excel',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }, {
                            extend: 'pdfHtml5',
                            orientation: 'landscape',
                            pageSize: 'LEGAL',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }]
                    });

                    memberTable.buttons().container().appendTo('#tblcommnd_wrapper .col-md-6:eq(0)');

                    var rows;
                    $("#tblcommnd #chkAll").click(function () {
                        if ($(this).is(':checked')) {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                        else {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                    });
                    $('#DetailBody').on('change', 'input[type="checkbox"]', function () {
                        if (!this.checked) {
                            var el = $('#chkAll').get(0);
                            if (el && el.checked && ('indeterminate' in el)) {
                                el.indeterminate = true;
                            }
                        }
                    });

                    $("body").off("click").on("click", ".cls-btnorder", function () {

                     
                        OrderByChange($(this).closest("tr").find("#ScomdId").html() ,$(this).closest("tr").find("#SOrderby").html());
                        
                    });
                    $("body").on("click", ".cls-btnedit", function () {
                      /*  $("#AddNewM").modal('show');*/
                        $("#txtComandName").val($(this).closest("tr").find("#comdName").html());
                        $("#txtAbbreviation").val($(this).closest("tr").find("#comdAbbreviation").html());
                       
                        $("#spncomdId").html($(this).closest("tr").find("#ScomdId").html());
                        $("#spnSOrderby").html($(this).closest("tr").find("#SOrderby").html());
                        
                    });


                    $("body").on("click", ".cls-btnDelete", function () {

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
                                
                                Delete($(this).closest("tr").find("#ScomdId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=10>No Record Found</td></tr>";
                $("#tblcommnd").DataTable().destroy();
                $("#DetailBody").html(listItem);
                $("#lblTotal").html(0);
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });

}
function Save() {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Master/SaveCommand',
        type: 'POST',
        data: { "ComdName": $("#txtComandName").val(), "ComdId": $("#spncomdId").html(), "ComdAbbreviation": $("#txtAbbreviation").val(), "Orderby": $("#spnSOrderby").html() }, //get the search string
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

function DeleteMultiple(ComdId) {
   
    var userdata =
    {
        "ints": ComdId,

    };
    $.ajax({
        url: '/Master/DeleteCommandMultiple',
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