$(document).ready(function () {
    mMsater(0, "ddlArmType", 9, "");
    BindData()
    $("#txtmappedbyDID").click(function () {
        $("#txtmappedbySearch").val("");  
        $("#spnTrnDomainMappingId").html(0);
        $("#txtmappedbySearch").attr("placeholder", "Enter Domain ID");  
        $("#DivSearchField").removeClass("d-none");
    });
    $("#txtmappedbyArmyNo").click(function () {
        $("#txtmappedbySearch").val("");  
        $("#spnTrnDomainMappingId").html(0);
        $("#txtmappedbySearch").attr("placeholder", "Enter Army No");
        $("#DivSearchField").removeClass("d-none");
    });
    $("#btnAdd").click(function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewRecordOffice").modal('show');
    });
    $("#btnRecordOfficeReset").click(function () {
        Reset();
        ResetErrorMessage();
    });
    $("#txtmappedbySearch").autocomplete({
        source: function (request, response) {
            var TypeId = 1;
            if ($("#txtmappedbyDID").prop("checked")) {
                TypeId = 1;
            } else if ($("#txtmappedbyArmyNo").prop("checked")) {
                TypeId = 2;
            } 
            if (request.term.length > 2) {
                $("#spnTrnDomainMappingId").html('');
                var param = { "SearchName": request.term,"TypeId": TypeId };
                $("#spnTrnDomainMappingId").html(0);
                $.ajax({
                    url: '/Master/GetMappedForRecord',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                {
                                    $("#loading").addClass("d-none");
                                    if ($("#txtmappedbyDID").prop("checked")) {
                                        return {
                                            label: item.DomainId + ' ' + item.RankAbbreviation + ' ' + item.Name + ' ' + item.ArmyNo,
                                            value: item.TDMId
                                        }
                                    }
                                    else
                                    {
                                        return {
                                            label: item.ArmyNo + ' ' + item.RankAbbreviation + ' ' + item.Name + ' ' + item.DomainId,
                                            value: item.TDMId
                                        }
                                    }
                                    
                                };

                            }))
                        }
                        else {
                            $("#txtmappedbySearch").val("");
                            $("#spnTrnDomainMappingId").html("");
                            alert("DomainId / ArmyNo not found.")
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
            $("#txtmappedbySearch").val(i.item.label);
            $("#spnTrnDomainMappingId").html(i.item.value);
        },
        appendTo: '#suggesstion-box'

    });
});

function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveRecordOffice';
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


            if (result == DataSave) {
                toastr.success('Record Office has been saved');

                $("#AddNewRecordOffice").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Record Office has been Updated');

                $("#AddNewRecordOffice").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {

                toastr.error('Record Office / Abbreviation Name Exits!');

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
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#ddlArmType").val("");

    $("#spnRecordOfficeId").html("0");
    $("#spnTrnDomainMappingId").html("0");

    $("#txtmappedbyDID").prop("checked", false);
    $("#txtmappedbyArmyNo").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#ddlArmType-error").html("");
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