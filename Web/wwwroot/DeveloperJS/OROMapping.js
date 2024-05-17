$(document).ready(function () {
    mMsater(0, "ddlRO", RecordOffice, "");
    mMsater(0, "ddlRank", Rank, "");
    GetArmsList("ddlArmedIdList", 0);
    BindData();
    $('.select2').select2({
        dropdownParent: $('#AddNewOROMapping'),
        closeOnSelect: false
    });
    $("#btnAdd").on("click",function () {
        Reset();
        ResetErrorMessage();
        $("#btnOROMappingAdd").val("Save");
        $("#AddNewOROMapping").modal('show');
    });
    $("#btnOROMappingAdd").on("click", function () {
        Proceed();
    });
    $("#btnOROMappingReset").on("click", function () {
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
    if (($("#ddlRank").val() == 0 || $("#ddlRank").val() == "null") && ($('#ddlArmedIdList').val().length == 0 || $('#ddlArmedIdList').val() == "null")) {
        toastr.error('Rank / Arme any one required.');
        return false;
    }

    let formId = '#SaveOROMapping';
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
        url: '/Master/GetAllOROMapping',
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
                        listItem += "<td class='d-none'><span id='spnMOROMappingId'>" + response[i].OROMappingId + "</span><span id='spnRecordOfficeId'>" + response[i].RecordOfficeId + "</span><span id='spnRankId'>" + response[i].RankId + "</span><span id='spnArmedIds'>" + response[i].ArmedIdList + "</span><span id='spnTDMId'>" + response[i].TDMId + "</span><span id='spnUnitId'>" + response[i].UnitId + "</span><span id='spnSus_no'>" + response[i].Sus_no + "</span><span id='spnSuffix'>" + response[i].Suffix + "</span><span id='spnUnitName'>" + response[i].UnitName + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='RecordOfficeName'>" + response[i].RecordOfficeName + "</span></td>";
                       
                        if (response[i].ArmedIdList != null) {
                            var armsArray = response[i].ArmNameList.split('#');
                            if (armsArray != null) {
                                listItem += "<td class='align-middle'><ul>";
                                    for (var j = 0; j < armsArray.length; j++) {
                                        listItem += "<li>" + armsArray[j] +"</li>";
                                }
                                listItem += "</ul></td>";
                            }
                        }
                        else {
                            listItem += "<td class='align-middle'></td>";
                        }
                        if (response[i].RankId != null) {
                            listItem += "<td class='align-middle'><span id='ArmedIdList'>" + response[i].RankName + "</span></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'></td>";
                        }

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
                        $("#spnOROMappingId").html($(this).closest("tr").find("#spnMOROMappingId").html());
                        $("#ddlRO").val($(this).closest("tr").find("#spnRecordOfficeId").html());

                        if ($(this).closest("tr").find("#spnRankId").html() != null && $(this).closest("tr").find("#spnRankId").html() != "null") {
                            $("#ddlRank").val($(this).closest("tr").find("#spnRankId").html());
                        }
                        else {
                            $("#ddlRank").val("0");
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

                        let arr2 = $(this).closest("tr").find("#spnArmedIds").html().split(',');
                        $("#ddlArmedIdList").val(arr2);
                        $("#ddlArmedIdList").trigger("change");

                        $("#btnOROMappingAdd").val("Update");
                        $("#AddNewOROMapping").modal('show');
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

                                Delete($(this).closest("tr").find("#spnMOROMappingId").html());

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
    var ArmedIds = "" + $("#ddlArmedIdList").val() + "";
    $.ajax({
        url: '/Master/SaveOROMapping',
        type: 'POST',
        data: {
            "OROMappingId": $("#spnOROMappingId").html(),
            "ArmedIdList": $("#ddlArmedIdList").val().length >0 ? ArmedIds : null,
            "RecordOfficeId": $("#ddlRO").val(),
            "RankId": $("#ddlRank").val() == 0 ? null : $("#ddlRank").val(),
            "TDMId": $("#ddlTDMId").val() == 0 ? null : $("#ddlTDMId").val(),
            "UnitId": $("#spnUnitMapId").html() == "0" ? null : $("#spnUnitMapId").html(),
        },
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Officer Record Office Mapping has been saved');

                $("#AddNewOROMapping").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Officer Record Office Mapping has been Updated');

                $("#AddNewOROMapping").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == "5") {
                toastr.error('Rank / Arme any one required.');
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
    $("#spnOROMappingId").html("0"); 
    $('#ddlArmedIdList').val(null).trigger('change');
    $("#ddlRO").val("0");
    $("#ddlRank").val("0");
    $("#txtUnitName").val("");
    $("#ddlTDMId").val("0");
    $("#spnUnitMapId").html("0");
}
function ResetErrorMessage() {
    $("#ddlArmedIdList-error").html(""); 
    $("#ddlRO-error").html("");
    $("#ddlRank-error").html("");
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
        "OROMappingId": Id,

    };
    $.ajax({
        url: '/Master/DeleteOROMapping',
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