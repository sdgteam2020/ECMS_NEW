$(document).ready(function () {
    mMsater(0, "ddlCommand", 1, "");
    BindDataMapUnit()


    $('input[name="UnitTyperdi"]').click(function () {
        var lst = '<option value="1">Please Select</option>';
        var val = $("input[type='radio'][name=UnitTyperdi]:checked").val();
        if (val == "1") {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();

            mMsater(0, "ddlCommand", 1, "");

            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

        }
        else if (val == "2") {

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();
            $('#ddlFmnBranch option').remove();

            mMsater(0, "ddlCommand", 1, "");
            mMsater(0, "ddlFmnBranch", FmnBranches, "");

            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
            $(".DteBranch").addClass("d-none");
        }
        else if (val == "3") {
            $(".unittype").addClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").removeClass("d-none");

            $('#ddlPSODte option').remove();
            $('#ddlDgSubDte option').remove();

            $("#ddlCommand").html(lst);
            $("#ddlCorps").html(lst);
            $("#ddlBde").html(lst);
            $("#ddlDiv").html(lst);
            $("#ddlFmnBranch").html(lst);

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
                    if (isNumeric($("#txtSusno").val().substring(0, 7)) == true && isNumeric($("#txtSusno").val().substring(8, 7)) == false) {
                        SaveUnitWithMapping();
                    }
                    else {
                        toastr.error('SUSNO Should be first 7 digit Numeric and last digit alphaBat!');
                    }

                  
                    //if ($("#SpnUnitMapId").html() == 0) {

                    //    //$("#txtSusno").val().substring(0, 7), "UnitId": 0, "Suffix": $("#txtSusno").val().substring(8, 7),
                    //    //alert($("#txtSusno").val().substring(8, 7))
                    //    //alert(isNumeric($("#txtSusno").val().substring(8, 7)))

                    //    if (isNumeric($("#txtSusno").val().substring(0, 7)) == true && isNumeric($("#txtSusno").val().substring(8, 7)) == false) {
                    //        UnitSave();
                    //    }
                    //    else {
                    //        toastr.error('SUSNO Should be first 7 digit Numeric and last digit alphaBat!');
                    //    }
                    //    // 
                       
                    //} else {
                    //    SaveUnitMap();
                    //}
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
   

    //$('#txtSusno').on('input', function () {
    //    $("#txtUnit").val("");
    //    $("#SpnUnitMapId").html(0); spnUnitMapId
    //    $('#txtUnit').attr('readonly', false);
    //    if ($(this).val().length > 7) {
    //        GetUnitDetails($(this).val(),1);
    //}
    //});
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
                        listItem += "<span id='spnMcorpsId'>" + response[i].CorpsId + "</span><span id='spncomdId'>" + response[i].ComdId + "</span><span id='spnUnitType'>" + response[i].UnitType + "</span><span id='spnPsoId'>" + response[i].PsoId + "</span><span id='spnFmnBranchID'>" + response[i].FmnBranchID + "</span><span id='spnSubDteId'>" + response[i].SubDteId + "</span><span id='spnIsVerify'>" + response[i].IsVerify + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='Sus_no'>" + response[i].Sus_no + response[i].Suffix + "</span></td>";
                        listItem += "<td class='align-middle'><span id='unitName'>" + response[i].UnitName + "</span></td>";

                        if (parseInt(response[i].UnitType) == 1) {
                            listItem += "<td class='align-middle'><span class='badge bg-primary'>Unit</span></td>";
                        }
                        else if (parseInt(response[i].UnitType) == 2) {
                            listItem += "<td class='align-middle'><span class='badge bg-primary'>Fmn HQ</span></td>";

                        } else if (parseInt(response[i].UnitType) == 3) {
                            listItem += "<td class='align-middle'><span class='badge bg-primary'>Dte/Br</span></td>";
                        }
                        listItem += "<td class='align-middle'><span id='bdeName'>" + response[i].BdeName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].DivName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='corpsName'>" + response[i].CorpsName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='comdName'>" + response[i].ComdName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='BranchName'>" + response[i].BranchName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='SubDteName'>" + response[i].SubDteName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='PSOName'>" + response[i].PSOName + "</span></td>";

                        if (response[i].IsVerify == true)
                            listItem += "<td class='align-middle'><span id='unit_desc'><span class='badge badge-pill badge-success'>Verifed</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span id='unit_desc'><span class='badge badge-pill badge-danger'>Not Verify</span></span></td>";

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
                        "order": [[1, "asc"]],
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
                    
                        $("#spnUnitMapId").html($(this).closest("tr").find("#spnMapUnitId").html());

                        $("#spnUnitId").html($(this).closest("tr").find("#spnMUnitId").html());

                        $("#txtUnit").val($(this).closest("tr").find("#unitName").html());
                        $("#txtSusno").val($(this).closest("tr").find("#Sus_no").html());
                        var lst = '<option value="1">Please Select</option>';
                        if (parseInt($(this).closest("tr").find("#spnUnitType").html()) == 1) {
                            $("#UnitType1").prop("checked", true);

                            mMsater($(this).closest("tr").find("#spncomdId").html(), "ddlCommand", 1, "");
                            mMsater($(this).closest("tr").find("#spnMcorpsId").html(), "ddlCorps", 2, $(this).closest("tr").find("#spncomdId").html());
                            mMsaterByParent($(this).closest("tr").find("#spnMDivId").html(), "ddlDiv", 3, $(this).closest("tr").find("#spncomdId").html(), $(this).closest("tr").find("#spnMcorpsId").html(), 0, 0);///ComdId,CorpsId,DivId,BdeId
                            mMsaterByParent($(this).closest("tr").find("#spnMbdeId").html(), "ddlBde", 4, $(this).closest("tr").find("#spncomdId").html(), $(this).closest("tr").find("#spnMcorpsId").html(), $(this).closest("tr").find("#spnMDivId").html(), 0);///ComdId,CorpsId,DivId,BdeId

                            $(".unittype").removeClass("d-none");
                            $(".FmnBranch").addClass("d-none");
                            $(".DteBranch").addClass("d-none");

                            $("#ddlFmnBranch").html(lst);
                            $("#ddlPSODte").html(lst);
                            $("#ddlDgSubDte").html(lst);
                        }
                        else if (parseInt($(this).closest("tr").find("#spnUnitType").html()) == 2)
                        {
                            $("#UnitType2").prop("checked", true);

                            mMsater($(this).closest("tr").find("#spncomdId").html(), "ddlCommand", 1, "");
                            mMsater($(this).closest("tr").find("#spnMcorpsId").html(), "ddlCorps", 2, $(this).closest("tr").find("#spncomdId").html());
                            mMsaterByParent($(this).closest("tr").find("#spnMDivId").html(), "ddlDiv", 3, $(this).closest("tr").find("#spncomdId").html(), $(this).closest("tr").find("#spnMcorpsId").html(), 0, 0);///ComdId,CorpsId,DivId,BdeId
                            mMsaterByParent($(this).closest("tr").find("#spnMbdeId").html(), "ddlBde", 4, $(this).closest("tr").find("#spncomdId").html(), $(this).closest("tr").find("#spnMcorpsId").html(), $(this).closest("tr").find("#spnMDivId").html(), 0);///ComdId,CorpsId,DivId,BdeId
                            mMsater($(this).closest("tr").find("#spnFmnBranchID").html(), "ddlFmnBranch", FmnBranches, "");

                            $("#ddlPSODte").html(lst);
                            $("#ddlDgSubDte").html(lst);

                            $(".unittype").removeClass("d-none");
                            $(".FmnBranch").removeClass("d-none");
                            $(".DteBranch").addClass("d-none");

                        }
                        else if (parseInt($(this).closest("tr").find("#spnUnitType").html()) == 3)
                        {
                            $("#UnitType3").prop("checked", true);

                            mMsater($(this).closest("tr").find("#spnPsoId").html(), "ddlPSODte", PSO, "");
                            mMsater($(this).closest("tr").find("#spnSubDteId").html(), "ddlDgSubDte", SubDte,"" );

                            $(".unittype").addClass("d-none");
                            $(".FmnBranch").addClass("d-none");
                            $(".DteBranch").removeClass("d-none");

                            $("#ddlFmnBranch").html(lst);
                            $("#ddlCommand").html(lst);
                            $("#ddlCorps").html(lst);
                            $("#ddlCorps").html(lst);
                            $("#ddlBde").html(lst);
                            $("#ddlDiv").html(lst);
                        }
                        if ($(this).closest("tr").find("#spnIsVerify").html() == 'true') {
                            $("#isverifyyes").prop("checked", true);
                        }
                        else {
                            $("#isverifyno").prop("checked", true);
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
                                DeleteMapUnit($(this).closest("tr").find("#spnMapUnitId").html());

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
function SaveUnitWithMapping() {
    $.ajax({
        url: '/Master/SaveUnitWithMapping',
        type: 'POST',
        data: {
            "UnitId": $("#spnUnitId").html(),
            "UnitMapId": $("#spnUnitMapId").html(),
            "Sus_no": $("#txtSusno").val().substring(0, 7),
            "Suffix": $("#txtSusno").val().substring(8, 7),
            "UnitName": $("#txtUnit").val(),
            "IsVerify": $("input[type='radio'][name=IsVerify]:checked").val(),
            "UnitType": $("input[type='radio'][name=UnitTyperdi]:checked").val(),
            "ComdId": $("#ddlCommand").val(),
            "CorpsId": $("#ddlCorps").val(),
            "DivId": $("#ddlDiv").val(),
            "BdeId": $("#ddlBde").val(),
            "PsoId": $("#ddlPSODte").val(),
            "FmnBranchID": $("#ddlFmnBranch").val(),
            "SubDteId": $("#ddlDgSubDte").val()
        },
        success: function (result) {
            if (result == DataSave) {
                Swal.fire({
                    icon: 'info',
                    title: 'Unit',
                    html: 'Unit has been saved',
                })
                $("#AddNewUnitmap").modal('hide'); 
                ResetMapUnit();
                BindDataMapUnit();
            }
            else if (result == DataUpdate) {
                Swal.fire({
                    icon: 'info',
                    title: 'Unit',
                    html: 'Unit has been updated',
                })
                $("#AddNewUnitmap").modal('hide');
                ResetMapUnit();
                BindDataMapUnit();
            }
            else if (result == DataExists) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Unit Name Exits!',
                })

            }
            else if (result == 5) {
                Swal.fire({
                    icon: 'info',
                    title: 'Unit',
                    html: 'Unit has been updated.<br/>Mapping has been saved.',
                })

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
        "UnitMapId": Id,

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

                    BindDataMapUnit();
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

                    BindDataMapUnit();
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