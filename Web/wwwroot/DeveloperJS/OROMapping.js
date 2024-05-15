$(document).ready(function () {
    mMsater(0, "ddlArmType", 9, "");
    GetArmsList("ddlArmedIdList", 0);
    BindData()
    $("#btnAdd").click(function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewRecordOffice").modal('show');
    });
    $("#btnRecordOfficeReset").click(function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
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
                                return { label: item.Sus_no + item.Suffix + ' ' + item.UnitName, value: item.UnitMapId };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            $("#spnUnitMapId").html("");
                            $("#ddlTDMId").find("option").not(":first").remove();
                            $("#ddlTDMId").val("0");
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
            $("#spnUnitMapId").html(i.item.value);
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetDDMappedForRecord',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',

                success: function (response) {
                    if (response != "null" && response != null) {
                        if (response == InternalServerError) {
                            Swal.fire({
                                text: errormsg
                            });
                        }

                        else {

                            var listItemddl = "";

                            listItemddl += '<option value="0">Please Select</option>';

                            for (var i = 0; i < response.length; i++) {
                                listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                            }
                            $("#ddlTDMId").html(listItemddl);
                        }
                    }
                    else {
                        //Swal.fire({
                        //    text: "No data found Offrs"
                        //});
                    }
                },
                error: function (result) {
                    Swal.fire({
                        text: errormsg002
                    });
                }
            });
        },
        appendTo: '#suggesstion-box'
    });
});

function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveRecordOffice';
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
function BindData() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Master/GetAllRecordOffice',
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
                    $("#tblData").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {

                    $("#tblData").DataTable().destroy();

                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnMRecordOfficeId'>" + response[i].RecordOfficeId + "</span><span id='spnArmedId'>" + response[i].ArmedId + "</span><span id='spnTDMId'>" + response[i].TDMId + "</span><span id='spnMessage'>" + response[i].Message + "</span><span id='spnUnitId'>" + response[i].UnitId + "</span><span id='spnSus_no'>" + response[i].Sus_no + "</span><span id='spnSuffix'>" + response[i].Suffix + "</span><span id='spnUnitName'>" + response[i].UnitName + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='RecordOfficeName'>" + response[i].RecordOfficeName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='abbreviation'>" + response[i].Abbreviation.toUpperCase() + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ArmedName'>" + response[i].ArmedName + "</span></td>";
                        if (response[i].TDMId != null) {
                            listItem += "<td class='align-middle'><span id='DID'>" + response[i].DomainId + ' & ' + response[i].ArmyNo + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + "</span></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'></td>";
                        }

                        listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";


                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";

                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length);

                    memberTable = $('#tblData').DataTable({
                        retrieve: true,
                        lengthChange: false,
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
                            title: 'E-IASC_Regimental',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            },
                            customize: function (doc) {
                                WaterMarkOnPdf(doc)
                            }
                        }]
                    });

                    memberTable.buttons().container().appendTo('#tblData_wrapper .col-md-6:eq(0)');

                    $("body").on("click", ".cls-btnedit", function () {
                        Reset();
                        ResetErrorMessage();
                        $("#txtName").val($(this).closest("tr").find("#RecordOfficeName").html());
                        $("#txtAbbreviation").val($(this).closest("tr").find("#abbreviation").html());
                        $("#spnRecordOfficeId").html($(this).closest("tr").find("#spnMRecordOfficeId").html());
                        $("#ddlArmType").val($(this).closest("tr").find("#spnArmedId").html());
                        if ($(this).closest("tr").find("#spnMessage").html() != null && $(this).closest("tr").find("#spnMessage").html() != "null") {
                            $("#txtMessage").val($(this).closest("tr").find("#spnMessage").html());
                        }
                        else {
                            $("#txtMessage").val("");
                        }
                        if ($(this).closest("tr").find("#spnUnitId").html() != null && $(this).closest("tr").find("#spnUnitId").html() != "null") {
                            $("#spnUnitMapId").html($(this).closest("tr").find("#spnUnitId").html());
                            $("#txtUnitName").val($(this).closest("tr").find("#spnSus_no").html() + $(this).closest("tr").find("#spnSuffix").html() + " " + $(this).closest("tr").find("#spnUnitName").html());
                        }
                        else {
                            $("#spnUnitMapId").html("0");
                            $("#txtUnitName").val("");
                        }
                        if ($(this).closest("tr").find("#spnTDMId").html() != null && $(this).closest("tr").find("#spnTDMId").html() != "null") {
                            GetDDMappedForRecord($(this).closest("tr").find("#spnUnitId").html(), $(this).closest("tr").find("#spnTDMId").html());
                        }
                        else {
                            $("#ddlTDMId").val("0");
                        }
                        $("#btnRecordOfficeAdd").val("Update");
                        $("#AddNewRecordOffice").modal('show');
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

                                Delete($(this).closest("tr").find("#spnMRecordOfficeId").html());

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

    $.ajax({
        url: '/Master/SaveRecordOffice',
        type: 'POST',
        data: {
            "Name": $("#txtName").val().trim(),
            "Abbreviation": $("#txtAbbreviation").val().trim(),
            "ArmedId": $("#ddlArmType").val(),
            "RecordOfficeId": $("#spnRecordOfficeId").html(),
            "UnitId": $("#spnUnitMapId").html() == "0" ? null : $("#spnUnitMapId").html(),
            "TDMId": $("#ddlTDMId").val() == 0 ? null : $("#ddlTDMId").val(),
            "Message": $("#txtMessage").val().length > 0 ? $("#txtMessage").val() : null,
        },
        success: function (result) {


            if (result == 5) {
                toastr.success('Record Office has been saved');

                $("#AddNewRecordOffice").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == 6) {
                toastr.success('Record Office has been Updated');

                $("#AddNewRecordOffice").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == 2) {
                toastr.error('Record Office / Abbreviation Name Exits!');
            }
            //else if (result == 3) {
            //    toastr.error('Armed Id Exits!');
            //}
            //else if (result == 4) {
            //    toastr.error('Domain Id Exits!');
            //}
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
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#ddlArmType").val("0");
    $("#txtMessage").val("");
    $("#txtUnitName").val("");
    $("#ddlTDMId").val("0");

    $("#spnRecordOfficeId").html("0");
    $("#spnUnitMapId").html("0");
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#ddlArmType-error").html("");
    $("#txtMessage-error").html("");
    $("#txtUnitName-error").html("");
    $("#ddlTDMId-error").html("");
}

function GetDDMappedForRecord(UnitId, TDMId) {
    var param1 = { "UnitMapId": UnitId };
    $.ajax({
        url: '/Master/GetDDMappedForRecord',
        contentType: 'application/x-www-form-urlencoded',
        data: param1,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="0">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                    }
                    $("#ddlTDMId").html(listItemddl);
                    if (TDMId != '') {
                        $("#ddlTDMId").val(TDMId);
                    }
                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}
function Delete(Id) {
    var userdata =
    {
        "RecordOfficeId": Id,

    };
    $.ajax({
        url: '/Master/DeleteRecordOffice',
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
function GetArmsList(ddl, sectid) {
    $.ajax({
        url: '/Master/GetArmsList',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }

                else {

                    var listItemddl = "";

                    var count = 1;
                    for (var i = 0; i < response.length; i++) {

                        listItemddl += '<option value="' + response[i].ArmedId + '">' + count + '. ' + response[i].ArmedName + '</option>';
                        count++;
                    }
                    $("#" + ddl + "").html(listItemddl);


                    if (sectid != '') {
                        $("#" + ddl + "").val(sectid);

                    }

                    //}


                }
            }
            else {
                //Swal.fire({
                //    text: "No data found Offrs"
                //});
            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}