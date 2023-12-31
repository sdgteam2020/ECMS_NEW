﻿$(document).ready(function () {
    BindData()

    $("#AddNewDomain input[name='txtapproval']").click(function () {
        $("#txtapproval-error").html("");
    });
    $("#AddNewDomain input[name='txtactive']").click(function () {
        $("#txtactive-error").html("");
    });
    $("#AddNewDomain input[name='IntOffr']").click(function () {
        $("#IntOffr-error").html("");
    });


    $("#txtAppointmentName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                var param = { "AppointmentName": request.term };
                $("#spnUnitAppointmentId").html(0);
                $.ajax({
                    url: '/Master/GetALLByAppointmentName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.AppointmentName, value: item.ApptId };

                        }))
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
            if (request.term.length > 2) {
                $("#lblComd").html('');
                $("#lblCorps").html('');
                $("#lblDiv").html('');
                $("#lblBde").html('');
                $("#lblSusno").html('');
                var param = { "UnitName": request.term };
                $("#spnUnitMapId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.UnitName, value: item.UnitMapId };

                        }))
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
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {
                    $("#spnUnitMapId").html(data.UnitMapId);
                    $("#lblComd").html(data.ComdName);
                    $("#lblCorps").html(data.CorpsName);
                    $("#lblDiv").html(data.DivName);
                    $("#lblBde").html(data.BdeName);
                    $("#lblSusno").html(data.Sus_no + '' + data.Suffix);

                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $("#txtArmyNo").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                $("#spnUserProfileId").html('');
                $("#lblName").html('');
                $("#lblRank").html('');
                var param = { "ArmyNo": request.term };
                $("#spnUserProfileId").html(0);
                $.ajax({
                    url: '/UserProfile/GetTopByArmyNo',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {
                            $("#loading").addClass("d-none");
                            return { label: item.ArmyNo, value: item.UserId };

                        }))
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

            $("#spnUserProfileId").html(i.item.value);
            $("#txtArmyNo").val(i.item.label);
            var param1 = { "UserId": i.item.value };
            $.ajax({
                url: '/UserProfile/GetProfileByUserId',
                method:'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype:'json',
                success: function (data) {
                    $("#lblName").html(data.Name);
                    $("#lblRank").html(data.RankName);
                }
            });
        },
        appendTo: '#suggesstion-box'
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
});

function Proceed() {
    ResetErrorMessage();
    let formId = '#SaveDomain';
    $.validator.unobtrusive.parse($(formId));

    ValidateRadioButton();

    if ($(formId).valid()) {
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
function ValidateRadioButton() {
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

    if ($("input[type='radio'][name=IntOffr]:checked").length == 0) {
        $("#IntOffr-error").html("IntOffr is required.");
    }
    else {
        $("#IntOffr-error").html("");
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
        url: '/Account/GetAllUserRegn',
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
                        listItem += "<td class='d-none'><span id='regId'>" + response[i].Id + "</span><span id='regTrnDomainMappingId'>" + response[i].TrnDomainMappingId + "</span><span id='regTrnDomainMappingApptId'>" + response[i].TrnDomainMappingApptId + "</span><span id='regTrnDomainMappingUnitId'>" + response[i].TrnDomainMappingUnitId + "</span><span id='regUserId'>" + response[i].UserId + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='reg_no'>" + response[i].Id + "</span></td>";
                        listItem += "<td class='align-middle'><span id='domainId'>" + response[i].DomainId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='armyNo'>" + response[i].ArmyNo + "</span></td>";
                        listItem += "<td class='align-middle'><span id='roleName'>" + response[i].RoleName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='updatedOn'>" + response[i].UpdatedOn + "</span></td>";
                        if (response[i].Mapped == true)
                            listItem += "<td class='align-middle'><span id='domain_mapping'><span class='badge badge-pill badge-success'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span id='domain_mapping'><span class='badge badge-pill badge-danger'>No</span></span></td>";

                        if (response[i].Active == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='domain_active'>Yes</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_active'>No</span></span></td>";

                        if (response[i].AdminFlag == true)
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-success' id='domain_approval'>Verifed</span></span></td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_approval'>Not Verify</span></span></td>";

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

                    $("body").on("click", ".cls-btnedit", function () {
                        Reset();
                        ResetErrorMessage();
                        $("#txtDomainId").val($(this).closest("tr").find("#domainId").html());
                        $("#txtRole").val($(this).closest("tr").find("#roleName").html());
                        $("#spnDomainRegId").html($(this).closest("tr").find("#regId").html());
                        //alert($(this).closest("tr").find("#domain_approval").html())
                        if ($(this).closest("tr").find("#domain_approval").html() == 'Verifed') {
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
                        if ($(this).closest("tr").find("#regUserId").html() > 0) {
                            GetProfileByUserId($(this).closest("tr").find("#regUserId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingId").html() > 0) {
                            $("spnTrnDomainMappingId").val($(this).closest("tr").find("#regTrnDomainMappingId").html());
                            GetALLByUnitById($(this).closest("tr").find("#regTrnDomainMappingUnitId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingApptId").html() > 0) {
                            GetNameByApptId($(this).closest("tr").find("#regTrnDomainMappingApptId").html());
                        }

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
    //alert($("#spnDomainRegId").html());
    $.ajax({
        url: '/Account/SaveDomain',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegId").html(),
            "DomainId": $("#txtDomainId").val(),
            "RoleName": $("#txtRole").val(),
            "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
            "Active": $('input:radio[name=txtactive]:checked').val(),
            "TDMId": $("#spnTrnDomainMappingId").html(),
            "ApptId": $("#spnUnitAppointmentId").html(),
            "UnitMappId": $("#spnUnitMapId").html(),
            "UserId": $("#spnUserProfileId").html(),
            "ArmyNo": $("#txtArmyNo").val(),

        }, //get the search string
        success: function (response) {
            var obj = jQuery.parseJSON(response);
            if (obj.Result == true) {
                toastr.success(obj.Message);

                $("#AddNewDomain").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else {
                toastr.error(obj.Message);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: obj.Message,

                })
            }
            //if (result == DataSave) {
            //    toastr.success('Domain Id has been saved');

            //    $("#AddNewDomain").modal('hide');
            //    BindData();
            //    Reset();
            //    ResetErrorMessage();
            //}
            //else if (result == DataUpdate) {
            //    toastr.success('Domain Id has been Updated');

            //    $("#AddNewDomain").modal('hide');
            //    BindData();
            //    Reset();
            //    ResetErrorMessage();
            //}
            //else if (result == DataExists) {

            //    toastr.error('Domain Id Exits!');

            //}
            //else if (result == InternalServerError) {
            //    Swal.fire({
            //        icon: 'error',
            //        title: 'Oops...',
            //        text: 'Something went wrong or Invalid Entry!',

            //    })

            //} else {
            //    if (result.length > 0) {
            //        for (var i = 0; i < result.length; i++) {
            //            toastr.error(result[i][0].Message)
            //        }


            //    }


            //}
        }
    });
}

function Reset() {
    $("#txtSearch").val("");

    $("#spnDomainRegId").html("0");
    $("#txtDomainId").val("");
    $("#txtRole").val("");

    $("#spnTrnDomainMappingId").html("0");
    $("#spnUnitMapId").html("0");
    $("#txtUnitName").val("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblSusno").html("");

    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#lblRank").val("");
    $("#lblName").val("");

    $("#spnUnitAppointmentId").html("0");
    $("#txtAppointmentName").val("");

    $("#intoffsyes").prop("checked", false);
    $("#intoffsno").prop("checked", false);

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtDomainId-error").html("");
    $("#txtRole-error").html("");
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
    $("#IntOffr-error").html("");
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
            $("#txtUnitName").val(data.UnitName);
            $("#lblComd").html(data.ComdName);
            $("#lblCorps").html(data.CorpsName);
            $("#lblDiv").html(data.DivName);
            $("#lblBde").html(data.BdeName);
            $("#lblSusno").html(data.Sus_no + '' + data.Suffix);

        }
    });
}
function GetProfileByUserId(param1) {
    $.ajax({
        url: '/UserProfile/GetProfileByUserId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UserId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUserProfileId").html(data.UserId);
            $("#txtArmyNo").val(data.ArmyNo);
            $("#lblRank").html(data.RankName);
            $("#lblName").html(data.Name);
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