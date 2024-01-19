$(document).ready(function () {
    mMsater(0, "ddlCommand", 1, "");
    BindDataMapUnit()


    $(".form-check-input").click(function () {
        var val = $("input[type='radio']:checked").val();
        if (val == "1") {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");
            mMsater(0, "ddlCommand", 1, "");
            $("#ddlFmnBranch").val(1);
            $("#ddlPSODte").val(1);
            $("#ddlDgSubDte").val(1);
        }
        else if (val == "2") {
            mMsater(0, "ddlCommand", 1, "");
            mMsater(0, "ddlFmnBranch", FmnBranches, "");
            $("#ddlPSODte").val(1);
            $("#ddlDgSubDte").val(1);
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
            $(".DteBranch").addClass("d-none");
        }
        else if (val == "3") {
            $(".unittype").addClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").removeClass("d-none");
            $("#ddlFmnBranch").val(1);

            var lst = '<option value="1">Please Select</option>';

            $("#ddlCommand").html(lst);
            $("#ddlCorps").html(lst);
            $("#ddlCorps").html(lst);
            $("#ddlBde").html(lst);
            $("#ddlDiv").html(lst);

            mMsater(0, "ddlPSODte", PSO, "");
            mMsater(0, "ddlDgSubDte", SubDte, "");
            
        }
    });
    $("#btnMapUnitAdd").click(function () {
        ResetMapUnit();
        $("#AddNewUnitmap").modal('show');
    });
    $("#txtSerachunit").keyup(function () {
        BindDataMapUnit()
    });
    $('#ddlCommand').on('change', function () {
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
        BindDataMapUnit();
    });

    $('#ddlCorps').on('change', function () {

        mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0); ///ComdId,CorpsId,DivId,BdeId
        BindDataMapUnit();
    });
    $('#ddlDiv').on('change', function () {

        mMsaterByParent(0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0); ///ComdId,CorpsId,DivId,BdeId
        BindDataMapUnit();

    });
    $('#ddlBde').on('change', function () {
        BindDataMapUnit();
    });
    $("#btnUnitMapReset").click(function () {
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
                  
                    if ($("#SpnUnitMapId").html() == 0) {

                        //$("#txtSusno").val().substring(0, 7), "UnitId": 0, "Suffix": $("#txtSusno").val().substring(8, 7),
                        //alert($("#txtSusno").val().substring(8, 7))
                        //alert(isNumeric($("#txtSusno").val().substring(8, 7)))

                        if (isNumeric($("#txtSusno").val().substring(0, 7)) == true && isNumeric($("#txtSusno").val().substring(8, 7)) == false) {
                            UnitSave();
                        }
                        else {
                            toastr.error('SUSNO Should be first 7 digit Numeric and last digit alphaBat!');
                        }
                        // 
                       
                    } else {
                        SaveUnitMap();
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
            GetUnitDetails($(this).val(),1);
    }
    });
});
function GetUnitDetails(val,flag) {
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

                    $("#txtUnit").val(response.UnitName);
                    $('#txtUnit').attr('readonly', true);
                    $("#SpnUnitMapId").html(response.UnitId);
                    
                    if (flag == 2) {
                        SaveUnitMap();
                    }

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
        "Unit": $('#txtSerachunit').val(),
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
                    $("#tblMapUnitdata").DataTable().destroy();

                    $("#DetailBodyMapUnit").html(listItem);
                    memberTable = $('#tblMapUnitdata').DataTable({
                        "language": {
                            "emptyTable": "No data available"
                        }
                    });


                }
                
                else {

                  
                   
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnMapUnitId'>" + response[i].UnitMapId + "</span><span id='spnMUnitId'>" + response[i].UnitId + "</span><span id='spnMbdeId'>" + response[i].BdeId + "</span><span id='spnMDivId'>" + response[i].DivId + "</span>";
                        listItem += "<span id='spnMcorpsId'>" + response[i].CorpsId + "</span><span id='spncomdId'>" + response[i].ComdId + "</span><span id='spnUnitType'>" + response[i].UnitType + "</span><span id='spnPsoId'>" + response[i].PsoId + "</span><span id='spnFmnBranchID'>" + response[i].FmnBranchID + "</span><span id='spnSubDteId'>" + response[i].SubDteId + "</span></td>";
                        listItem += "<td>";
                        listItem += "<div class='custom-control custom-checkbox small'>";
                        listItem += "<input type='checkbox' class='custom-control-input' id='" + response[i].UnitMapId + "'>";
                        listItem += "<label class='custom-control-label' for='" + response[i].UnitMapId + "'></label>";
                        listItem += "</div>";
                        listItem += "</td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                       
                        listItem += "<td class='align-middle'><span id='comdName'>" + response[i].ComdName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='corpsName'>" + response[i].CorpsName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].DivName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='bdeName'>" + response[i].BdeName + "</span></td>";
                        if (parseInt(response[i].UnitType) == 1) {
                            listItem += "<td class='align-middle'><span class='badge bg-danger'></span></td>";
                        }
                        else if (parseInt(response[i].UnitType) == 2) {
                            listItem += "<td class='align-middle'><span class='badge bg-primary'>Unit is Fmn HQ</span></td>";

                        } else if (parseInt(response[i].UnitType) == 3) {
                            listItem += "<td class='align-middle'><span class='badge bg-primary'>Unit is Dte/Branch</span></td>";

                        }
                        listItem += "<td class='align-middle'><span id='BranchName'>" + response[i].BranchName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='PSOName'>" + response[i].PSOName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='SubDteName'>" + response[i].SubDteName + "</span></td>";

                        listItem += "<td class='align-middle'><span id='unitName'>" + response[i].UnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='Sus_no'>" + response[i].Sus_no + response[i].Suffix + "</span></td>";
                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";

                       
                         /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                    }

                    $("#DetailBodyMapUnit").html(listItem);
                    $("#lblTotal").html(response.length);
                  
                    memberTable = $('#tblMapUnitdata').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        searching: false,
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
                      
                        mMsater($(this).closest("tr").find("#spnPsoId").html(), "ddlPSODte", PSO, "");
                        mMsater($(this).closest("tr").find("#spnSubDteId").html(), "ddlDgSubDte", SubDte,"" );
                        mMsater($(this).closest("tr").find("#spnFmnBranchID").html(), "ddlFmnBranch", FmnBranches, "");
                        $("#spnUnitMapUnitId").html($(this).closest("tr").find("#spnMapUnitId").html());

                        $("#SpnUnitMapId").html($(this).closest("tr").find("#spnMUnitId").html());

                        $("#txtUnit").val($(this).closest("tr").find("#unitName").html());
                        $("#txtSusno").val($(this).closest("tr").find("#Sus_no").html());

                        if (parseInt($(this).closest("tr").find("#spnUnitType").html()) == 1) {
                            $("#UnitType1").prop("checked", true);

                            $(".unittype").removeClass("d-none");
                            $(".FmnBranch").addClass("d-none");
                            $(".DteBranch").addClass("d-none");
                           
                        } else if (parseInt($(this).closest("tr").find("#spnUnitType").html()) == 2) {
                            $("#UnitType2").prop("checked", true);
                            $("#ddlPSODte").val(1);
                            $("#ddlDgSubDte").val(1);
                            $(".unittype").removeClass("d-none");
                            $(".FmnBranch").removeClass("d-none");
                            $(".DteBranch").addClass("d-none");

                        } else if (parseInt($(this).closest("tr").find("#spnUnitType").html()) == 3) {
                            $("#UnitType3").prop("checked", true);
                            $(".unittype").addClass("d-none");
                            $(".FmnBranch").addClass("d-none");
                            $(".DteBranch").removeClass("d-none");
                            $("#ddlFmnBranch").val(1);
                            var lst = '<option value="1">Please Select</option>';

                            $("#ddlCommand").html(lst);
                            $("#ddlCorps").html(lst);
                            $("#ddlCorps").html(lst);
                            $("#ddlBde").html(lst);
                            $("#ddlDiv").html(lst);
                        }
                       
                        $("#AddNewUnitmap").modal('show');
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
                $("#tblMapUnitdata").DataTable().destroy();

                $("#DetailBodyMapUnit").html(listItem);
                memberTable = $('#tblMapUnitdata').DataTable({
                    "language": {
                        "emptyTable": "No data available"
                    }
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

function UnitSave() {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Master/SaveUnit',
        type: 'POST',
        data: { "Sus_no": $("#txtSusno").val().substring(0, 7), "UnitId": 0, "Suffix": $("#txtSusno").val().substring(8, 7), "UnitName": $("#txtUnit").val(), "IsVerify": false }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Unit has been saved');
                GetUnitDetails($("#txtSusno").val(),2);
                /*  $("#AddNewM").modal('hide');*/
               
            }
            else if (result == DataUpdate) {
                toastr.success('Unit has been Updated');

                /*  $("#AddNewM").modal('hide');*/
              
              
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
function SaveUnitMap() {

    /*  alert($('#bdaymonth').val());*/
   
    $.ajax({
        url: '/Master/SaveMapUnit',
        type: 'POST',
        data: { "UnitName": $("#txtUnit").val(), "ComdId": $("#ddlCommand").val(), "CorpsId": $("#ddlCorps").val(), "DivId": $("#ddlDiv").val(), "BdeId": $("#ddlBde").val(), "UnitMapId": $("#spnUnitMapUnitId").html(), "UnitId": $("#SpnUnitMapId").html(), "UnitType": $("input[type='radio']:checked").val(), "PsoId": $("#ddlPSODte").val(), "FmnBranchID": $("#ddlFmnBranch").val(), "SubDteId": $("#ddlDgSubDte").val() }, //get the search string
        success: function (result) {


            if (result == DataSave) {


                toastr.success('Unit has been saved');
                ResetMapUnit();
                BindDataMapUnit();
                $("#AddNewUnitmap").modal('hide');
            }
            else if (result == DataUpdate) {


                toastr.success('Unit has been Updated');
                ResetMapUnit();
                BindDataMapUnit();
                $("#AddNewUnitmap").modal('hide');

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

function ResetMapUnit() {
    $("#ddlCommand").val("");
    $("#ddlCorps").val("");
    $("#ddlDiv").val("");
    $("#ddlBde").val("");
    $("#txtSusno").val("");
    $("#txtUnit").val("");

    $("#spnUnitMapUnitId").html("0");
    $("#SpnUnitMapId").html("0");
    $("#btnsave").val("Save");
    $("#txtUnit").val("");

    $("ddlPSODte").val("");
    $("ddlDgSubDte").val("");
    $("ddlFmnBranch").val("");
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