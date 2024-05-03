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
                            listItem += "<td class='align-middle'>" + i + "</td>";
                            listItem += "<td class='align-middle'><span id='comdName'>" + response[i].ComdName + "</span></td>";
                            listItem += "<td class='align-middle'><span id='comdAbbreviation'>" + response[i].ComdAbbreviation + "</span></td>";
                            

                            if (response[i].Orderby != response.length)
                                listItem += "<td class='align-middle'><span id=''><button type='button' class='cls-btnorder btn btn-icon btn-round btn-info mr-1'><i class='fas fa-arrow-down'></i></button></span></td>";
                            else
                                listItem += "<td></td>";
                              

                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>";
                            listItem += "<button type='button' class='cls-btntreeview btn btn-primary  mr-1'>Hierarchy Chart</button></td>";

                            /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                            listItem += "</tr>";
                        }
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length-1);
                  
                    memberTable = $('#tblcommnd').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        "order": [[1, "asc"]],
                        buttons: [{
                            extend: 'copy',
                            exportOptions: {
                                columns: "thead th:not(.noExport)",
                            }
                        }, {
                            extend: 'excel',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            }
                        }, {
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
                    $("body").on("click", ".cls-btntreeview", function () {
                        $("#treeview").modal('show'); 
                        GetBinaryTree($(this).closest("tr").find("#ScomdId").html())
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
        data: { "ComdName": $("#txtComandName").val().trim(), "ComdId": $("#spncomdId").html(), "ComdAbbreviation": $("#txtAbbreviation").val().trim(), "Orderby": $("#spnSOrderby").html() }, //get the search string
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