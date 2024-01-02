$(document).ready(function () {
    mMsater(0, "ddlRank", Rank, "");
    BindData()

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
                $("#spnUnitIdMap").html('');
                $("#lblComd").html('');
                $("#lblCorps").html('');
                $("#lblDiv").html('');
                $("#lblBde").html('');
                $("#lblSusno").html('');
                var param = { "UnitName": request.term };
                $("#spnUnitIdMap").html(0);
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
                    $("#spnUnitIdMap").html(data.UnitMapId);
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
    $("#btnDomainAdd").click(function () {
        Reset();
        $("#AddNewDomain").modal('show');
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
 

    $("#btnDomainsave").click(function () {
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
    });
});

function BindData() {
    var listItem = "";
    var userdata =
    {
        "Search": $("#txtSearch").val(),
        "Choice": $("input[type='radio'][name=choice]:checked").val()
    };
    $.ajax({
        url: '/Account/GetAllProfileManage',
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
                        $("#txtDomainId").val($(this).closest("tr").find("#domainId").html());
                        $("#txtRole").val($(this).closest("tr").find("#roleName").html());
                        $("#spnDomainRegId").html($(this).closest("tr").find("#regId").html());
                        //alert($(this).closest("tr").find("#domain_approval").html())
                        if ($(this).closest("tr").find("#domain_approval").html() == 'Verifed' ) {
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

                        if ($(this).closest("tr").find("#regTrnDomainMappingUnitId").html() > 0) {
                            GetALLByUnitById($(this).closest("tr").find("#regTrnDomainMappingUnitId").html());
                        }

                        if ($(this).closest("tr").find("#regTrnDomainMappingApptId").html() > 0) {
                            GetNameByApptId($(this).closest("tr").find("#regTrnDomainMappingApptId").html());
                        }
                        

                        $("#AddNewDomain").modal('show');
                        $("#btnDomainsave").val("Update");

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
    debugger;
    /*  alert($('#bdaymonth').val());*/
    alert($("txtapproval").val());
    $.ajax({
        url: '/Account/SaveDomain',
        type: 'POST',
        data: { "DomainId": $("#txtDomainId").val(), "Id": $("#spnDomainRegId").html(), "RoleName": $("#txtRole").val(), "IsVerify": $("txtapproval").val() }, //get the search string
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
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }

                        
            }
        }
    });
}

function Reset() {
    $("#txtSearch").val("");

    $("#spnDomainRegId").html("");
    $("#txtDomainId").val("");
    $("#txtRole").val("");

    $("#spnUnitIdMap").html("");
    $("#txtUnitName").val("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblSusno").html("");

    $("#spnUserProfileId").html("");
    $("#txtArmyNo").val("");
    $("#ddlRank").val("");
    $("#txtName").val("");

    $("#spnUnitAppointmentId").html("");
    $("#txtAppointmentName").val("");

    $("#intoffsyes").prop("checked", false);
    $("#intoffsno").prop("checked", false);

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        success: function (data) {
            $("#spnUnitIdMap").html(data.UnitMapId);
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
            $("#ddlRank").val(data.RankId);
            $("#txtName").val(data.Name);
            if (data.IntOffr == true) {
                $("#intoffsyes").prop("checked", true);
            }
            else {
                $("#intoffsno").prop("checked", true);
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