$(document).ready(function () {
    Reset();
    mMsater(0, "ddlArmType", 9, "");
    BindData()
    $("#txtmappedbyDID").click(function () {
        $("#txtmappedbySearch").attr("placeholder", "Enter Domain ID");  
        $("#DivSearchField").removeClass("d-none");
    });
    $("#txtmappedbyArmyNo").click(function () {
        $("#txtmappedbySearch").attr("placeholder", "Enter Army No");
        $("#DivSearchField").removeClass("d-none");
    });
    $("#btnAdd").click(function () {
        //Reset();
        //ResetErrorMessage();
        $("#AddNewRecordOffice").modal('show');
    });
    $("#btnReset").click(function () {
        Reset();
    });
    $("#txtmappedbySearch").autocomplete({
        if(true) {
            source: function (request, response) {
                if (request.term.length > 2) {
                    $("#spnTrnDomainMappingId").html('');
                    var param = { "UnitName": request.term };
                    $("#spnTrnDomainMappingId").html(0);
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
        }
        else
        {

        }

    });

    $("#btnsave").click(function () {
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



        // 

    });
});

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
                        listItem += "<td class='d-none'><span id='spnMRecordOfficeId'>" + response[i].RecordOfficeId + "</span><span id='spnArmedId'>" + response[i].ArmedId + "</span></td>";
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

                    var rows;
                    $("#tblData #chkAll").click(function () {
                        if ($(this).is(':checked')) {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                        else {
                            rows = memberTable.rows({ 'search': 'applied' }).nodes();
                            $('input[type="checkbox"]', rows).prop('checked', this.checked);
                        }
                    });
                    $('#DetailBody').on('change', 'input[type="checkbox"]', function () {
                        if (!this.checked) {
                            var el = $('#chkAll').get(0);
                            if (el && el.checked && ('indeterminate' in el)) {
                                el.indeterminate = true;
                            }
                        }
                    });


                    $("body").on("click", ".cls-btnedit", function () {
                        /*  $("#AddNewM").modal('show');*/
                        $("#txtName").val($(this).closest("tr").find("#Name").html());
                        $("#txtAbbreviation").val($(this).closest("tr").find("#abbreviation").html());
                        $("#spnRecordOfficeId").html($(this).closest("tr").find("#spnMRecordOfficeId").html());
                        $("#ddlArmType").val($(this).closest("tr").find("#spnArmedId").html());

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
        data: { "Name": $("#txtName").val().trim(), "RecordOfficeId": $("#spnRecordOfficeId").html(), "Abbreviation": $("#txtAbbreviation").val().trim(), "ArmedId": $("#ddlArmType").val() }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Record Office has been saved');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
            }
            else if (result == DataUpdate) {
                toastr.success('Record Office has been Updated');

                /*  $("#AddNewM").modal('hide');*/
                BindData();
                Reset();
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
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#spnRecordOfficeId").html("0");
    $("#ddlArmType").val("0");
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