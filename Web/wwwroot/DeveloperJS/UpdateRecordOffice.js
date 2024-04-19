$(document).ready(function () {
    debugger;
    GetDDMappedForRecord($("#spnUnitMapId").html(), $("#spnTrnDomainMappingId").html());
    GetUpdateRecordOffice();
});
function GetDDMappedForRecord(UnitMapId, TDMId) {
    var userdata =
    {
        "UnitMapId": UnitMapId,
    };
    $.ajax({
        url: '/Master/GetDDMappedForRecord',
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

                else {

                    var listItemddl = "";

                    listItemddl += '<option value="0">Please Select</option>';

                    for (var i = 0; i < response.length; i++) {
                        listItemddl += '<option value="' + response[i].TDMId + '">' + response[i].DomainId + ' ' + response[i].RankAbbreviation + ' ' + response[i].Name + ' ' + response[i].ArmyNo + '</option>';
                    }
                    $("#ddlDomainId").html(listItemddl);
                    if (TDMId != '') {
                        $("#ddlDomainId").val(TDMId);

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
function GetUpdateRecordOffice() {
    $.ajax({
        url: '/Master/GetUpdateRecordOffice',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        success: function (data) {
            $("#spnRecordOfficeId").html(data.RecordOfficeId);
            $("#spnTrnDomainMappingId").html(data.TDMId);
            $("#spnUnitMapId").html(data.UnitMapId);
            $("#lblROName").html(data.RecordOfficeName);
            $("#lblROAbbreviation").html(data.Abbreviation);
            $("#lblArms").html(data.ArmedName);
            $("#txtmappedbySearch").val(data.DomainId + ' ' + data.RankAbbreviation + ' ' + data.Name + ' ' + data.ArmyNo);
        }
    });
}

function Proceed() {
    ResetErrorMessage();

    let formId = '#UpdateRecordOffice';
    ValidateInput();
    $.validator.unobtrusive.parse($(formId));

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
                        listItem += "<td class='d-none'><span id='spnMRecordOfficeId'>" + response[i].RecordOfficeId + "</span><span id='spnArmedId'>" + response[i].ArmedId + "</span><span id='spnTDMId'>" + response[i].TDMId + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='Name'>" + response[i].Name + "</span></td>";
                        listItem += "<td class='align-middle'><span id='abbreviation'>" + response[i].Abbreviation + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ArmedName'>" + response[i].ArmedName + "</span></td>";

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
                        $("#txtName").val($(this).closest("tr").find("#Name").html());
                        $("#txtAbbreviation").val($(this).closest("tr").find("#abbreviation").html());
                        $("#spnRecordOfficeId").html($(this).closest("tr").find("#spnMRecordOfficeId").html());
                        $("#ddlArmType").val($(this).closest("tr").find("#spnArmedId").html());
                        if ($(this).closest("tr").find("#spnTDMId").html() != null && $(this).closest("tr").find("#spnTDMId").html() != "null") {
                            $("#spnTrnDomainMappingId").html($(this).closest("tr").find("#spnTDMId").html());
                            $("#txtmappedbyDID").prop("checked", true);
                            GetDomainIdByTDMId($(this).closest("tr").find("#spnTDMId").html());
                            $("#DivSearchField").removeClass("d-none");
                        }
                        else {
                            $("#spnTrnDomainMappingId").html(0);
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
        data: { "Name": $("#txtName").val().trim(), "Abbreviation": $("#txtAbbreviation").val().trim(), "ArmedId": $("#ddlArmType").val(), "RecordOfficeId": $("#spnRecordOfficeId").html(), "TDMId": $("#spnTrnDomainMappingId").html() }, //get the search string
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
            else if (result == 3) {
                toastr.error('Armed Id Exits!');
            }
            else if (result == 4) {
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
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }


            }
        }
    });
}
function Reset() {
    $("#DivSearchField").addClass("d-none");
    $("#txtmappedbySearch").val("");

    $("#spnRecordOfficeId").html("0");
    $("#spnTrnDomainMappingId").html("0");

    $("#txtmappedbyDID").prop("checked", false);
    $("#txtmappedbyArmyNo").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtmappedbySearch-error").html("");
}
function ValidateInput() {
    if ($("input[type='radio'][name=txtmappedby]:checked").length == 0) {
        $("#txtmappedby-error").html("Domain ID / Army No is required.");
    }
    else {
        $("#txtmappedby-error").html("");
    }

    var TDMId = $("#spnTrnDomainMappingId").html();

    if ((TDMId == 0 || TDMId == '') && $("#txtmappedbySearch").val().length > 0) {
        $("#txtmappedbySearch").val('');
        $("#txtmappedbySearch-error").html("Search name is invalid.");
        toastr.error('Search name is invalid.');
    }
}
function GetDomainIdByTDMId(TDMId) {
    $.ajax({
        url: '/Master/GetDomainIdByTDMId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "TDMId": TDMId },
        type: 'POST',
        success: function (data) {
            $("#txtmappedbySearch").val(data.DomainId + ' ' + data.RankAbbreviation + ' ' + data.Name + ' ' + data.ArmyNo);
        }
    });
}