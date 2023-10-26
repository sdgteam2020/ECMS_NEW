$(document).ready(function () {
    mMsater(0, "ddlCommand", 1, "");
    BindData()
    $('#ddlCommand').on('change', function () {
       
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
    });
    $("#btnReset").click(function () {
        Reset();
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
        url: '/Master/GetAllBdeCat',
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
                    listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
                    $("#tbldata").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }
                
                else {

                  
                   
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnMbdeCatId'>" + response[i].bdeCatId + "</span><span id='spnMcorpsId'>" + response[i].corpsId + "</span><span id='spncomdId'>" + response[i].comdId + "</span></td>";
                        listItem += "<td>";
                        listItem += "<div class='custom-control custom-checkbox small'>";
                        listItem += "<input type='checkbox' class='custom-control-input' id='" + response[i].bdeCatId + "'>";
                        listItem += "<label class='custom-control-label' for='" + response[i].bdeCatId + "'></label>";
                        listItem += "</div>";
                        listItem += "</td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                       
                        listItem += "<td class='align-middle'><span id='comdName'>" + response[i].comdName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='corpsName'>" + response[i].corpsName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='bdeCatName'>" + response[i].bdeCatName + "</span></td>";
                       
                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";

                       
                         /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length);
                  
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
                    
                        $("#ddlCommand").val($(this).closest("tr").find("#spncomdId").html());

                        mMsater($(this).closest("tr").find("#spnMcorpsId").html(), "ddlCorps", 2, $(this).closest("tr").find("#spncomdId").html());

                        $(".spnCorpsId").html($(this).closest("tr").find("#spnMbdeCatId").html());

                        $("#txtBdeCatName").val($(this).closest("tr").find("#bdeCatName").html());
                        $("#btnsave").val("Update");
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
                                
                                Delete($(this).closest("tr").find("#spnMbdeCatId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
                $("#tbldata").DataTable().destroy();
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
        url: '/Master/SaveBdeCat',
        type: 'POST',
        data: { "BdeCatName": $("#txtBdeCatName").val(), "ComdId": $("#ddlCommand").val(), "CorpsId": $("#ddlCorps").val(), "BdeCatId": $(".spnCorpsId").html() }, //get the search string
        success: function (result) {


            if (result == DataSave) {


                toastr.success('BdeCat has been saved');
                Reset();
                BindData();

            }
            else if (result == DataUpdate) {


                toastr.success('BdeCat has been Updated');
                Reset();
                BindData();

            }
            else if (result == DataExists) {

                toastr.error('BdeCat Name Exits!');
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
    $("#ddlCommand").val("");
    $("#ddlCorps").val("");
    $(".spnCorpsId").html("0");
    $("#btnsave").val("Save");
    $("#txtBdeCatName").val("");
}

function Delete(BdeCatId) {
    var userdata =
    {
        "BdeCatId": BdeCatId,

    };
    $.ajax({
        url: '/Master/DeleteBdeCat',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null") {
                if (response == -1) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }
                else if (response == -3) {
                    window.location.href = "/Login";//Session expired
                }
                else if (response == 2) {
                    //lol++;
                    //if (lol == Tot) {
                   
                    toastr.success('Deleted Selected!');

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

function DeleteMultiple(BdeCatId) {
   
    var userdata =
    {
        "ints": BdeCatId,

    };
    $.ajax({
        url: '/Master/DeleteBdeCatMultiple',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null") {
                if (response == -1) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }
                else if (response == -3) {
                    window.location.href = "/Login";//Session expired
                }
                else if (response == 2) {
                    //lol++;
                    //if (lol == Tot) {
                   
                    toastr.success('Deleted Selected!');

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