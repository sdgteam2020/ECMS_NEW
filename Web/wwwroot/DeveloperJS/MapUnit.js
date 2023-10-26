$(document).ready(function () {
    mMsater(0, "ddlCommand", 1, "");
    BindDataMapUnit()
    $("#btnMapUnitAdd").click(function () {
        ResetMapUnit();
        $("#AddNewUnitmap").modal('show');

    });
    $('#ddlCommand').on('change', function () {
       
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
        BindDataMapUnit();
    });

    $('#ddlCorps').on('change', function () {

        mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0);///ComdId,CorpsId,DivId,BdeId
        BindDataMapUnit();
    });
    $('#ddlDiv').on('change', function () {

        mMsaterByParent(0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0);///ComdId,CorpsId,DivId,BdeId
        BindDataMapUnit();

    });
    $('#ddlBde').on('change', function () {
        BindDataMapUnit();
    });
    $("#btnMapUnitReset").click(function () {
        ResetMapUnit();
    });



    $("#btnUnitMapsave").click(function () {
        if ($("#SaveFormMapUnit")[0].checkValidity()) {

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

                    if ($("#SpnUnitMapUnitId").html() == 0) {
                        UnitSave();
                    } else {
                      Save();
                    }
                }
            })

        } else {
            $("#SaveFormMapUnit")[0].reportValidity();
        }



        // 

    });
  

    $('#btnMapUnitMultiDelete').click(function () {
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
                   
                    DeleteMapUnitMultiple(lst);

                }
            });
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 data to Delete."
            });
        }
    });
   

    $('#txtSusno').on('input', function () {
        $("#txtUnit").val("");
        $("#SpnUnitMapId").html(0);
        $('#txtUnit').attr('readonly', false);
        if ($(this).val().length > 7) {
            GetUnitDetails($(this).val());
    }
    });
});
function GetUnitDetails(val) {
    var userdata =
    {
        "Sus_no": val,
      
    };
    $.ajax({
        url: '/Master/GetBySusNO',
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
                    $("#txtUnit").val(""); $("#SpnUnitMapId").html(0); $('#txtUnit').attr('readonly', false);
                }

                else {

                    $("#txtUnit").val(response.unit_desc);
                    $('#txtUnit').attr('readonly', true);
                    $("#SpnUnitMapId").html(response.unitId);
                    


                }
            }
            else {
                $("#txtUnit").val("");
                $("#SpnUnitMapId").html(0);
                $('#txtUnit').attr('readonly', false);
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function BindDataMapUnit() {
    var listItem = "";
    var userdata =
    {
        "ComdId": $('#ddlCommand').val(),
        "CorpsId": $('#ddlCorps').val(),
        "DivId": $('#ddlDiv').val(),
        "BdeId": $('#ddlBde').val(),

    };
    $.ajax({
        url: '/Master/GetAllMapUnit',
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
                    listItem += "<tr><td class='text-center' colspan=9>No Record Found</td></tr>";
                    //$("#tbldata").DataTable().destroy();
                    $("#DetailBodyMapUnit").html(listItem);
                    $("#lblMapUnitTotal").html(0);
                }
                
                else {

                  
                   
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnMUnitId'>" + response[i].unitId + "</span><span id='spnMbdeId'>" + response[i].bdeId + "</span><span id='spnMDivId'>" + response[i].divId + "</span><span id='spnMcorpsId'>" + response[i].corpsId + "</span><span id='spncomdId'>" + response[i].comdId + "</span></td>";
                        listItem += "<td>";
                        listItem += "<div class='custom-control custom-checkbox small'>";
                        listItem += "<input type='checkbox' class='custom-control-input' id='" + response[i].unitId + "'>";
                        listItem += "<label class='custom-control-label' for='" + response[i].unitId + "'></label>";
                        listItem += "</div>";
                        listItem += "</td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                       
                        listItem += "<td class='align-middle'><span id='comdName'>" + response[i].comdName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='corpsName'>" + response[i].corpsName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].divName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='bdeName'>" + response[i].bdeName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='unitName'>" + response[i].unitName + "</span></td>";
                       
                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";

                       
                         /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                    }

                    $("#DetailBodyMapUnit").html(listItem);
                    $("#lblTotal").html(response.length);
                  
                    memberTable = $('#tblMapUnitdata').DataTable({
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

                    memberTable.buttons().container().appendTo('#tblMapUnitdata_wrapper .col-md-6:eq(0)');

                    var rows;
                    $("#tblMapUnitdata #chkAll").click(function () {
                        if ($(this).is(':checked')) {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                        else {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                    });
                    $('#DetailBodyMapUnit').on('change', 'input[type="checkbox"]', function () {
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
                        mMsaterByParent($(this).closest("tr").find("#spnMDivId").html(), "ddlDiv", 3, $('#ddlCommand').val(), $(this).closest("tr").find("#spnMcorpsId").html(), 0, 0);///ComdId,CorpsId,DivId,BdeId
                        mMsaterByParent($(this).closest("tr").find("#spnMbdeId").html(), "ddlBde", 4, $('#ddlCommand').val(), $(this).closest("tr").find("#spnMcorpsId").html(), $(this).closest("tr").find("#spnMDivId").html(), 0);///ComdId,CorpsId,DivId,BdeId


                        $(".spnUnitMapUnitId").html($(this).closest("tr").find("#spnMUnitId").html());

                        $("#txtUnit").val($(this).closest("tr").find("#unitName").html());
                        $("#btnMapUnitsave").val("Update");
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
                                //alert($(this).closest("tr").find("#spnMbdeId").html());
                                DeleteMapUnit($(this).closest("tr").find("#spnMUnitId").html());

                            }
                        });
                    });


                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=9>No Record Found</td></tr>";
               // $("#tbldata").DataTable().destroy();
                $("#DetailBodyMapUnit").html(listItem);
                $("#lblMapUnitTotal").html(0);
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
        url: '/Master/SaveMapUnit',
        type: 'POST',
        data: { "UnitName": $("#txtUnit").val(), "ComdId": $("#ddlCommand").val(), "CorpsId": $("#ddlCorps").val(), "DivId": $("#ddlDiv").val(), "BdeId": $("#ddlBde").val(), "UnitId": $(".spnUnitMapUnitId").html() }, //get the search string
        success: function (result) {


            if (result == DataSave) {


                toastr.success('Unit has been saved');
                ResetMapUnit();
                BindDataMapUnit();

            }
            else if (result == DataUpdate) {


                toastr.success('Unit has been Updated');
                ResetMapUnit();
                BindDataMapUnit();

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

function ResetMapUnit() {
    $("#ddlCommand").val("");
    $("#ddlCorps").val("");
    $("#ddlDiv").val("");
    $("#ddlBde").val("");
    $(".spnBdeId").html("0");
    $("#btnsave").val("Save");
    $("#txtUnit").val("");
}

function DeleteMapUnit(Id) {
    var userdata =
    {
        "UnitId": Id,

    };
    $.ajax({
        url: '/Master/DeleteMapUnit',
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
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }

                else if (response == Success) {
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

function DeleteMapUnitMultiple(Id) {
   
    var userdata =
    {
        "ints": Id,

    };
    $.ajax({
        url: '/Master/DeleteMapUnitMultiple',
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
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }

                else if (response == Success) {
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