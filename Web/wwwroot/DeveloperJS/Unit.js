$(document).ready(function () {

    

    
    Reset();
    BindData()
    //$("#btnAdd").click(function () {
    //    Reset();
    //    $("#AddNewM").modal('show');

    //});
   
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
        url: '/Master/GetAllUnit',
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
                    listItem += "<tr><td class='text-center' colspan=6>No Record Found</td></tr>";
                    $("#tblcommnd").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }
               
                else {

                    /*$("#tblcommnd").DataTable().destroy();*/
                   
                    for (var i = 0; i < response.length; i++) {
                     
                            listItem += "<tr>";
                            listItem += "<td class='d-none'><span id='SunitId'>" + response[i].UnitId + "</span></td>";
                            listItem += "<td>";
                            listItem += "<div class='custom-control custom-checkbox small'>";
                            listItem += "<input type='checkbox' class='custom-control-input' id='" + response[i].UnitId + "'>";
                            listItem += "<label class='custom-control-label' for='" + response[i].UnitId + "'></label>";
                            listItem += "</div>";
                            listItem += "</td>";
                            listItem += "<td class='align-middle'>" + (i+1) + "</td>";
                            listItem += "<td class='align-middle'><span id='sus_no'>" + response[i].Sus_no + "</span></td>";
                            listItem += "<td class='align-middle'><span id='suffix'>" + response[i].Suffix + "</span></td>";
                            listItem += "<td class='align-middle'><span id='unit_desc'>" + response[i].Unit_desc + "</span></td>";


                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";


                            /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                            listItem += "</tr>";
                        
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length-1);
                  
                    memberTable = $('#tbldata').DataTable({
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

                    memberTable.buttons().container().appendTo('#tbldata_wrapper .col-md-6:eq(0)');

                    var rows;
                    $("#tbldata #chkAll").click(function () {
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

                    $("body").on("click", ".cls-btnedit", function () {
                      /*  $("#AddNewM").modal('show');*/
                        $("#txtSusno").val($(this).closest("tr").find("#sus_no").html());
                        $("#txtSuffix").val($(this).closest("tr").find("#suffix").html());
                        $("#txtUnitDesc").val($(this).closest("tr").find("#unit_desc").html());

                       // alert($(this).closest("tr").find("#SunitId").html())
                        $("#spnUnitId").html($(this).closest("tr").find("#SunitId").html());
                        
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
                                
                                Delete($(this).closest("tr").find("#SunitId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=6>No Record Found</td></tr>";
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
        url: '/Master/SaveUnit',
        type: 'POST',
        data: { "Sus_no": $("#txtSusno").val(), "UnitId": $("#spnUnitId").html(), "Suffix": $("#txtSuffix").val(), "Unit_desc": $("#txtUnitDesc").val(), "IsVerify": true }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Unit has been saved');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
            }
            else if (result == DataUpdate) {
                toastr.success('Unit has been Updated');

                /*  $("#AddNewM").modal('hide');*/
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
                        toastr.error(result[i][0].errorMessage)
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