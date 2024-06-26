﻿$(document).ready(function () {
    BindData();
    AccountCount();
    BindRoles();
    $('.select2').select2({
        dropdownParent: $('#AddNewDomain'),
        closeOnSelect: false
    });

    $("#AddNewDomain input[name='txtapproval']").click(function () {
        $("#txtapproval-error").html("");
    });
    $("#AddNewDomain input[name='txtactive']").click(function () {
        $("#txtactive-error").html("");
    });
    $("#AddNewDomain input[name='IntOffr']").click(function () {
        $("#IntOffr-error").html("");
    });

    $("#AddNewDomain input[name='InitatingOffr']").click(function () {
        $("#InitatingOffr-error").html("");
    });
    $("#AddNewDomain input[name='CommandingOffr']").click(function () {
        $("#CommandingOffr-error").html("");
    });
    $("#AddNewDomain input[name='IsRO']").click(function () {
        $("#IsRO-error").html("");
    });
    $("#AddNewDomain input[name='IsORO']").click(function () {
        $("#IsORO-error").html("");
    });

    $("#txtAppointmentName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                $("#spnUnitAppointmentId").html('');
                var param = { "AppointmentName": request.term };
                $("#spnUnitAppointmentId").html(0);
                $.ajax({
                    url: '/Master/GetALLByAppointmentName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {

                                $("#loading").addClass("d-none");
                                return { label: item.AppointmentName, value: item.ApptId };

                            }))
                        }
                        else {
                            $("#txtAppointmentName").val(""); 
                            $("#spnUnitAppointmentId").html("");
                            alert("Appointment not found.")
                        }
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtAppointmentName").val(i.item.label);
            $("#spnUnitAppointmentId").html(i.item.value);
            //alert(i.item.value)
        },
        appendTo: '#suggesstion-box'
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            $("#lblSusno").html('');
            $("#lblPso").html('');
            $("#lblDG").html('');
            $("#lblComd").html('');
            $("#lblCorps").html('');
            $("#lblDiv").html('');
            $("#lblBde").html('');
            $("#lblFmn").html('');
            if (request.term.length > 2) {
                $("#spnUnitMapId").html('');
                var param = { "UnitName": request.term };
                $("#spnUnitMapId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.Sus_no + item.Suffix +' '+ item.UnitName , value: item.UnitMapId };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            $("#spnUnitMapId").html("");
                            $("#spnTDMUnitType").html("");
                            alert("Unit not found.")
                        }
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {
                    $("#spnTDMUnitType").html(data.UnitType);
                    $("#spnUnitMapId").html(data.UnitMapId);
                    $("#lblSusno").html(data.Sus_no + '' + data.Suffix);

                    if (data.UnitType == 1) {
                        $("#lblComd").html(data.ComdName);
                        $("#lblCorps").html(data.CorpsName);
                        $("#lblDiv").html(data.DivName);
                        $("#lblBde").html(data.BdeName);
                        $("#lbl1").addClass("d-none");
                        $("#lbl2").addClass("d-none");
                        $("#lbl3").removeClass("d-none");
                        $("#lbl4").removeClass("d-none");
                        $("#lbl5").removeClass("d-none");
                        $("#lbl6").removeClass("d-none");
                        $("#lbl7").addClass("d-none");
                    }
                    else if (data.UnitType == 2) {
                        $("#lblComd").html(data.ComdName);
                        $("#lblCorps").html(data.CorpsName);
                        $("#lblDiv").html(data.DivName);
                        $("#lblBde").html(data.BdeName);
                        $("#lblFmn").html(data.BranchName);
                        $("#lbl1").addClass("d-none");
                        $("#lbl2").addClass("d-none");
                        $("#lbl3").removeClass("d-none");
                        $("#lbl4").removeClass("d-none");
                        $("#lbl5").removeClass("d-none");
                        $("#lbl6").removeClass("d-none");
                        $("#lbl7").removeClass("d-none");
                    }
                    else if (data.UnitType == 3) {
                        $("#lblPso").html(data.PSOName);
                        $("#lblDG").html(data.SubDteName);
                        $("#lbl1").removeClass("d-none");
                        $("#lbl2").removeClass("d-none");
                        $("#lbl3").addClass("d-none");
                        $("#lbl4").addClass("d-none");
                        $("#lbl5").addClass("d-none");
                        $("#lbl6").addClass("d-none");
                        $("#lbl7").addClass("d-none");
                    }

                    

                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $('#txtUnitName').keyup(function (e) {
        if (e.keyCode == 46) {
            $("#spnUnitMapId").html('0');
            $("#txtUnitName").val("");
            $("#spnTDMUnitType").html("");

            $("#lblSusno").html('');
            $("#lblPso").html('');
            $("#lblDG").html('');
            $("#lblComd").html('');
            $("#lblCorps").html('');
            $("#lblDiv").html('');
            $("#lblBde").html('');
            $("#lblFmn").html('');
        }
    });
    $("#btnDomainAdd").click(function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewDomain").modal('show');
    });

    $("#btnDomainAddReset").click(function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtSearch").keyup(function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            if ($("input[type='radio'][name=choice]:checked").val() == "Id") {
                var num_val = parseInt(eThis.val());
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData()
                }
            }
            else {
                BindData()
            }
        }
        else {
            alert("Select Choice");
        }
    });

    $("#btnUser").click(function () {

        if ($("#lblUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Users');
            $("#DataTableDialog").modal('show');
            BindDialog("User");
        }
    });
    $("#btnMappedUser").click(function () {

        if ($("#lblMappedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Mapped Users');
            $("#DataTableDialog").modal('show');
            BindDialog("MappedUser");
        }
    });
    $("#btnUnMappedUser").click(function () {
        if ($("#lblUnMappedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total UnMapped Users');
            $("#DataTableDialog").modal('show');
            BindDialog("UnMappedUser");
        }
    });
    $("#btnActiveUser").click(function () {
        if ($("#lblActiveUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Active Users');
            $("#DataTableDialog").modal('show');
            BindDialog("ActiveUser");
        }
    });
    $("#btnInActiveUser").click(function () {
        if ($("#lblInActiveUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total InActive Users');
            $("#DataTableDialog").modal('show');
            BindDialog("InActiveUser");
        }
    });
    $("#btnVerified").click(function () {
        if ($("#lblVerifiedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("Verified");
        }
    });
    $("#btnNotVerifiedUser").click(function () {
        if ($("#lblNotVerifiedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("NotVerifiedUser");
        }
    });
    $("#btnIO").click(function () {
        if ($("#lblIO").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("IO");
        }
        else {
            BindData("IO");
        }
    });
    $("#btnApprover").click(function () {
        if ($("#lblApprover").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("CO");
        }
    });
    $("#btnRO").click(function () {
        if ($("#lblRO").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("RO");
        }
    });
    $("#btnORO").click(function () {
        if ($("#lblORO").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("ORO");
        }
    });
});

function BindDialog(Choice) {
    $("#tbldatadialog").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        ajax: {
            url: "/Account/GetDataForDataTable",
            contentType: 'application/x-www-form-urlencoded',
            type: "POST",
            data: function (d) {
                d.draw = d.draw;
                d.start = d.start;
                d.length = d.length;
                d.searchValue = d.search.value;
                d.sortColumn = d.columns[d.order[0].column].data;
                d.sortDirection = d.order[0].dir;
                d.Choice = Choice;
            },
        },
        columns: [
            { data: "Id", name: "Id" },
            { data: "DomainId", name: "DomainId" },
            { data: "ArmyNo", name: "ArmyNo" },
            { data: "RoleNames", name: "RoleNames" },
            { data: "UpdatedOn", name: "UpdatedOn" },
            { data: "Mapped", name: "Mapped" },
            { data: "Active", name: "Active" },
            { data: "AdminFlag", name: "AdminFlag" },
            { data: "IsIO", name: "IsIO" },
            { data: "IsCO", name: "IsCO" },
            { data: "IsRO", name: "IsRO" },
            { data: "IsORO", name: "IsORO" }
        ]
    });
}
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveDomain';
    ValidateInput();
    $.validator.unobtrusive.parse($(formId));

    if ($(formId).valid()) {
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
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please fill required field.',

        })
        toastr.error('Please fill required field.');
        return false;
    }
}
function ValidateInput() {
    if ($("input[type='radio'][name=txtapproval]:checked").length == 0) {
        $("#txtapproval-error").html("Approval is required.");
    }
    else {
        $("#txtapproval-error").html("");
    }

    if ($("input[type='radio'][name=txtactive]:checked").length == 0) {
        $("#txtactive-error").html("Active is required.");
    }
    else {
        $("#txtactive-error").html("");
    }

    if ($("input[type='radio'][name=InitatingOffr]:checked").length == 0) {
        $("#InitatingOffr-error").html("Initating Offr is required.");
    }
    else {
        $("#InitatingOffr-error").html("");
    }

    if ($("input[type='radio'][name=CommandingOffr]:checked").length == 0) {
        $("#CommandingOffr-error").html("Commanding Offr is required.");
    }
    else {
        $("#CommandingOffr-error").html("");
    }

    if ($("input[type='radio'][name=IsRO]:checked").length == 0) {
        $("#IsRO-error").html("Record Office is required.");
    }
    else {
        $("#IsRO-error").html("");
    }

    if ($("input[type='radio'][name=IsORO]:checked").length == 0) {
        $("#IsORO-error").html("Officer Record Office is required.");
    }
    else {
        $("#IsORO-error").html("");
    }

    var AppointmentId = $("#spnUnitAppointmentId").html();
    
    if ((AppointmentId == 0 || AppointmentId == '') && $("#txtAppointmentName").val().length > 0) {
        $("#txtAppointmentName").val('');
        $("#txtAppointmentName-error").html("Appointment name is invalid.");
        toastr.error('Appointment name is invalid.');
    }

    var UnitMapId = $("#spnUnitMapId").html();
    if ((UnitMapId == 0 || UnitMapId == '') && $("#txtUnitName").val().length > 0 ) {
        $("#txtUnitName").val('');
        $("#txtAppointmentName-error").html("Unit name is invalid.");
        toastr.error('Unit name is invalid.');
    }
}

function BindData() {
    var listItem = "";
    var userdata =
    {
        "Search": $("#txtSearch").val(),
        "Choice": $("input[type='radio'][name=choice]:checked").val()
    };
    $.ajax({
        url: '/Account/GetAllDomainRegn',
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
                else if (response.length == 0) {
                    $("#tbldata").DataTable().destroy();

                    $("#DetailBody").html(listItem);
                    memberTable = $('#tbldata').DataTable({
                        "language": {
                            "emptyTable": "No data available"
                        }
                    });


                }

                else {
                    $("#tbldata").DataTable().destroy();

                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='regId'>" + response[i].Id + "</span><span id='regTrnDomainMappingId'>" + response[i].TrnDomainMappingId + "</span><span id='regTrnDomainMappingApptId'>" + response[i].TrnDomainMappingApptId + "</span><span id='regTrnDomainMappingUnitId'>" + response[i].TrnDomainMappingUnitId + "</span><span id='regUserId'>" + response[i].UserId + "</span><span id='roleIds'>" + response[i].RoleIds + "</span><span id='domain_approval'>" + response[i].AdminFlag + "</span><span id='extension'>" + response[i].Extension + "</span><span id='dialingCode'>" + response[i].DialingCode + "</span></td>"; 
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='reg_no'>" + response[i].Id + "</span></td>";
                        listItem += "<td class='align-middle'><span id='domainId'>" + response[i].DomainId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='roleName'>" + response[i].RoleNames + "</span></td>";
                        listItem += "<td class='align-middle'><span id='updatedOn'>" + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + "</span></td>";
                        if (response[i].Mapped == true)
                            listItem += "<td class='align-middle'><span id='domain_mapping'><span class='badge badge-pill badge-success'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span id='domain_mapping'><span class='badge badge-pill badge-danger'>No</span></span></td>";

                        if (response[i].Active == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='domain_active'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_active'>No</span></span></td>";

                        if (response[i].IsIO == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isIO'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isIO'>No</span></span></td>";

                        if (response[i].IsCO == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isCO'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isCO'>No</span></span></td>";

                        if (response[i].IsRO == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isRO'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isRO'>No</span></span></td>";

                        if (response[i].IsORO == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='isORO'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='isORO'>No</span></span></td>";

                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span></td>";


                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";

                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length - 1);

                    memberTable = $('#tbldata').DataTable({
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
                            orientation: 'portrait',
                            pageSize: 'A4',
                            title: 'E-IASC_Domain_Regn',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            },
                            customize: function (doc) {
                                WaterMarkOnPdf(doc)
                            }
                        }]
                    });

                    memberTable.buttons().container().appendTo('#tbldata_wrapper .col-md-6:eq(0)');

                    var rows;

                    $("body").on("click", ".cls-btnedit", function () {
                        Reset();
                        ResetErrorMessage();
                        $("#txtDomainId").val($(this).closest("tr").find("#domainId").html());
                        $("#txtRole").val($(this).closest("tr").find("#roleName").html());
                        $("#txtDialingCode").val($(this).closest("tr").find("#dialingCode").html());
                        $("#txtExtension").val($(this).closest("tr").find("#extension").html());
                        $("#spnDomainRegId").html($(this).closest("tr").find("#regId").html());
                        //alert($(this).closest("tr").find("#domain_approval").html())
                        if ($(this).closest("tr").find("#domain_approval").html() == 'true') {
                            $("#txtapprovalyes").prop("checked", true);
                        }
                        else {
                            $("#txtapprovalno").prop("checked", true);

                        }

                        if ($(this).closest("tr").find("#domain_active").html() == 'Yes') {
                            $("#txtactiveyes").prop("checked", true);
                        }
                        else {
                            $("#txtactiveno").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#isIO").html() == 'Yes') {
                            $("#initatingOffryes").prop("checked", true);
                        }
                        else {
                            $("#initatingOffrno").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#isCO").html() == 'Yes') {
                            $("#commandingOffryes").prop("checked", true);
                        }
                        else {
                            $("#commandingOffrno").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#isRO").html() == 'Yes') {
                            $("#isroyes").prop("checked", true);
                        }
                        else {
                            $("#isrono").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#isORO").html() == 'Yes') {
                            $("#isoroyes").prop("checked", true);
                        }
                        else {
                            $("#isorono").prop("checked", true);
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingId").html() > 0) {
                            $("#spnTrnDomainMappingId").html($(this).closest("tr").find("#regTrnDomainMappingId").html()); 
                            GetALLByUnitById($(this).closest("tr").find("#regTrnDomainMappingUnitId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingApptId").html() > 0) {
                            GetNameByApptId($(this).closest("tr").find("#regTrnDomainMappingApptId").html());
                        }
                        //$("#ddlRoles").val([1, 2]);
                        //$("#ddlRoles").trigger("change");
                        let arr2 = $(this).closest("tr").find("#roleIds").html().split(',');
                        $("#ddlRoles").val(arr2);
                        $("#ddlRoles").trigger("change");

                        $("#btnDomainAdd").val("Update");
                        $("#AddNewDomain").modal('show');
                    });
                }
            }
            else {
                $("#tbldata").DataTable().destroy();

                $("#DetailBody").html(listItem);
                memberTable = $('#tbldata').DataTable({
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
function Save() {
    $.ajax({
        url: '/Account/SaveDomainRegn',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegId").html(),
            "DomainId": $("#txtDomainId").val(),
            "RoleIds": $('#ddlRoles').val(),
            "DialingCode": $("#txtDialingCode").val(),
            "Extension": $("#txtExtension").val(),
            "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
            "Active": $('input:radio[name=txtactive]:checked').val(),
            "IsIO": $('input:radio[name=InitatingOffr]:checked').val(),
            "IsRO": $('input:radio[name=IsRO]:checked').val(),
            "IsCO": $('input:radio[name=CommandingOffr]:checked').val(),
            "IsORO": $('input:radio[name=IsORO]:checked').val(),
            "TDMId": $("#spnTrnDomainMappingId").html(),
            "ApptId": $("#spnUnitAppointmentId").html(),
            "UnitMappId": $("#spnUnitMapId").html(),
        }, //get the search string
        success: function (result) {
            if (result == DataSave) {
                toastr.success('Domain Id has been saved');

                $("#AddNewDomain").modal('hide');
                AccountCount();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Domain Id has been Updated');

                $("#AddNewDomain").modal('hide');
                AccountCount();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {

                toastr.error('Domain Id Exits!');

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
                        toastr.error(result[i][0].Message)
                    }


                }


            }
        }
    });
}

function Reset()     {
    $("#txtSearch").val("");

    $("#spnDomainRegId").html("0");
    $("#txtDomainId").val("");
    //$("#ddlRoles").select2('data', null);
    $("#txtDialingCode").val("");
    $("#txtExtension").val("");

    $('#ddlRoles').val(null).trigger('change');
   

    $("#spnTrnDomainMappingId").html("0");
    $("#spnUnitMapId").html("0");
    $("#txtUnitName").val("");
    $("#lblSusno").html("");
    $("#lblPso").html("");
    $("#lblDG").html("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblFmn").html("");


    $("#spnUnitAppointmentId").html("0");
    $("#txtAppointmentName").val("");

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);

    $("#isroyes").prop("checked", false);
    $("#isrono").prop("checked", false);

    $("#initatingOffryes").prop("checked", false);
    $("#initatingOffrno").prop("checked", false);

    $("#commandingOffryes").prop("checked", false);
    $("#commandingOffrno").prop("checked", false);

    $("#isoroyes").prop("checked", false);
    $("#isorono").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtDomainId-error").html("");
    $("#ddlRoles-error").html("");
    $("#txtDialingCode-error").html("");
    $("#txtExtension-error").html("");
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
    $("#IsRO-error").html("");
    $("#InitatingOffr-error").html("");
    $("#CommandingOffr-error").html("");
    $("#IsORO-error").html("");
    $("#txtAppointmentName-error").html("");
    $("#txtUnitName-error").html("");
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUnitMapId").html(data.UnitMapId);
            $("#lblSusno").html(data.Sus_no + '' + data.Suffix);
            /*$("#txtUnitName").val(data.UnitName);*/
            $("#txtUnitName").val(data.UnitName + ' '+ data.Sus_no + data.Suffix);

            if (data.UnitType == 1) {
                $("#lblComd").html(data.ComdName);
                $("#lblCorps").html(data.CorpsName);
                $("#lblDiv").html(data.DivName);
                $("#lblBde").html(data.BdeName);
                $("#lbl1").addClass("d-none");
                $("#lbl2").addClass("d-none");
                $("#lbl3").removeClass("d-none");
                $("#lbl4").removeClass("d-none");
                $("#lbl5").removeClass("d-none");
                $("#lbl6").removeClass("d-none");
                $("#lbl7").addClass("d-none");
            }
            else if (data.UnitType == 2) {
                $("#lblComd").html(data.ComdName);
                $("#lblCorps").html(data.CorpsName);
                $("#lblDiv").html(data.DivName);
                $("#lblBde").html(data.BdeName);
                $("#lblFmn").html(data.BranchName);
                $("#lbl1").addClass("d-none");
                $("#lbl2").addClass("d-none");
                $("#lbl3").removeClass("d-none");
                $("#lbl4").removeClass("d-none");
                $("#lbl5").removeClass("d-none");
                $("#lbl6").removeClass("d-none");
                $("#lbl7").removeClass("d-none");
            }
            else if (data.UnitType == 3) {
                $("#lblPso").html(data.PSOName);
                $("#lblDG").html(data.SubDteName);
                $("#lbl1").removeClass("d-none");
                $("#lbl2").removeClass("d-none");
                $("#lbl3").addClass("d-none");
                $("#lbl4").addClass("d-none");
                $("#lbl5").addClass("d-none");
                $("#lbl6").addClass("d-none");
                $("#lbl7").addClass("d-none");
            }
        }
    });
}
function GetNameByApptId(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUnitAppointmentId").html(data.ApptId);
            $("#txtAppointmentName").val(data.AppointmentName);
        }
    });
}
function BindRoles() {
        $.ajax({
        url: "/Account/GetAllRole",
        type: "POST",
        success: function (response, status) {
            var list = "";
            for (var i = 0; i < response.length; i++) {
                list += '<option value="' + response[i].Id + '">' + response[i].Name + '</option>';
            }
            $('#ddlRoles').html(list)
        }
        });
}
function AccountCount() {
    $.ajax({
        url: "/Account/AccountCount",
        type: "POST",
        success: function (response, status) {
            $("#lblUser").html(response.User);
            $("#lblActiveUser").html(response.ActiveUser);
            $("#lblInActiveUser").html(response.InActiveUser);
            $("#lblMappedUser").html(response.MappedUser);
            $("#lblUnMappedUser").html(response.UnMappedUser);
            $("#lblVerifiedUser").html(response.VerifiedUser);
            $("#lblNotVerifiedUser").html(response.NotVerifiedUser);
            $("#lblIO").html(response.IO);
            $("#lblApprover").html(response.CO);
            $("#lblRO").html(response.RO);
            $("#lblORO").html(response.ORO);
        }
    });
}
