$(function () {
    BindData();
    $('.select2').select2({
        dropdownParent: $('#AddICardRequestHold'),
        closeOnSelect: false
    });
    $("#btnRequestHoldAdd").on("click", function (){
        Reset();
        ResetErrorMessage();
        $("#gpUnHoldReason").addClass("d-none");
        $("#txtArmyNo").prop('readonly', false);
        $("#txtHoldReason").prop('readonly', false);
        $("#AddICardRequestHold").modal('show');
    });

    $("#btnAddICardRequestHold").on("click", function () {
        ResetErrorMessage();

        let formId = '#SaveICardRequestHold';
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
    });

    $("#txtArmyNo").autocomplete({
        source: function (request, response) {
            $("#lblName").html('');
            $("#lblRank").html('');
            $("#lblUnitName").html('');
            if (request.term.length > 2) {
                $("#spnRequestId").html('');
                var param = { "ArmyNo": request.term };
                $("#spnRequestId").html(0);
                $.ajax({
                    url: '/BasicDetail/GetTopArmyNoFromICardRequest',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.ServiceNo, value: item.RequestId };
                            }))
                        }
                        else {
                            $("#txtArmyNo").val("");
                            $("#spnRequestId").html("");
                            alert("Army No not found.")
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
            $("#spnRequestId").html(i.item.value);
            $("#txtArmyNo").val(i.item.label);
            var param1 = { "RequestId": i.item.value };
            $.ajax({
                url: '/BasicDetail/GetBDetailByRequestId',
                method: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype: 'json',
                success: function (data) {
                    $("#lblName").html(data.LName == null ? data.FName : data.FName + ' ' + data.LName);
                    $("#lblRank").html(data.RankName);
                    $("#lblUnitName").html(data.UnitName);
                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $('#txtArmyNo').on("keyup", function (e) {
        if (e.which == 46) {
            $("#spnRequestId").html('0');
            $("#txtArmyNo").val('');
            $("#lblName").html('');
            $("#lblRank").html('');
            $("#lblUnitName").html('');
        }
    });

});
function BindData() {
    var listItem = "";

    $.ajax({
        url: '/BasicDetail/GetAllICardRequestHold',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });

                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=12>No Record Found</td></tr>";
                    $("#tblData").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                }
                else {
                    $("#tblData").DataTable().destroy();

                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='iCardHoldId'>" + response[i].ICardHoldId + "</span><span id='requestId'>" + response[i].RequestId + "</span><span id='rankName'>" + response[i].RankName + "</span><span id='name'>" + (response[i].LName == null ? response[i].FName : response[i].FName + ' ' + response[i].LName) + "</span><span id='unHoldReason'>" + response[i].UnHoldReason + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><a href='#' class='BasicDetailView'><span id='serviceNo'>" + response[i].ServiceNo + "</span></a></td>";
                        if (response[i].LName == null)
                            listItem += "<td class='align-middle'><span id='nameWithRank'>" + response[i].RankName + ' ' + response[i].FName + "</span></td>";
                        else
                            listItem += "<td class='align-middle'><span id='nameWithRank'>" + response[i].RankName + ' ' + response[i].FName + ' ' + response[i].LName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='unitName'>" + response[i].UnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='applyFor'>" + response[i].ApplyFor + "</span></td>";
                        listItem += "<td class='align-middle'><span id='domainId'>" + response[i].DomainId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='holdReason'>" + response[i].HoldReason + "</span></td>";
                        if (response[i].IsHold == true)
                            listItem += "<td class='align-middle'><span class='badge badge-pill badge-success' id='isHold'>Yes</span></td>";
                        else
                            listItem += "<td class='align-middle'><span class='badge badge-pill badge-danger' id='isHold'>No</span></td>";
                        listItem += "<td class='align-middle'><span id='updatedOn'>" + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + "</span></td>";
                        listItem += "<td class='noExport'><button class='historyRequest btn btn-icon btn-round btn-primary mr-1' data-toggle='tooltip' data-placement='left' title=''><i class='fa fa-history' aria-hidden='true'></i></button></td>";
                        if ($("#spnFlagICardAppl").html() == 'Flag ICard Appl') {
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span></td>";
                        }
                        
                        listItem += "</tr>";

                    }

                    $("#DetailBody").html(listItem);

                    memberTable = $('#tblData').DataTable({
                        retrieve: true,
                        lengthChange: false,
                        stateSave: true,
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
                            title: 'E-IASC_ICardRequestHold',
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
                        $("#gpUnHoldReason").removeClass("d-none");
                        $("#txtArmyNo").prop('readonly', true);
                        $("#txtHoldReason").prop('readonly', true);
                        $("#spnICardHoldId").html($(this).closest("tr").find("#iCardHoldId").html());
                        $("#spnRequestId").html($(this).closest("tr").find("#requestId").html());
                        $("#txtArmyNo").val($(this).closest("tr").find("#serviceNo").html());
                        $("#lblRank").html($(this).closest("tr").find("#rankName").html());
                        $("#lblName").html($(this).closest("tr").find("#name").html());
                        $("#lblUnitName").html($(this).closest("tr").find("#unitName").html());
                        $("#txtHoldReason").val($(this).closest("tr").find("#holdReason").html());
                        $("#txtUnHoldReason").val($(this).closest("tr").find("#unHoldReason").html() != 'null' ? $(this).closest("tr").find("#unHoldReason").html() : "");

                        if ($(this).closest("tr").find("#isHold").html() == 'Yes') {
                            $("#IsHoldYes").prop("checked", true);
                        }
                        else {
                            $("#IsHoldNo").prop("checked", true);
                        }

                        $("#btnAddICardRequestHold").val("Update");
                        $("#AddICardRequestHold").modal('show');
                    });
                    $("body").on("click", ".BasicDetailView", function () {
                        GetBasicDetailByRequestId($(this).closest("tr").find("#requestId").html());
                    });
                    $("body").on("click", ".historyRequest", function () {
                        $("#exampleModal").modal('show');
                        GetRequestHistory($(this).closest("tr").find("#requestId").html());
                    });
                }
            }
            else {
                $("#tblData").DataTable().destroy();

                $("#DetailBody").html(listItem);
                memberTable = $('#tblData').DataTable({
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
        url: '/BasicDetail/SaveICardRequestHold',
        type: 'POST',
        data: {
            "ICardHoldId": $("#spnICardHoldId").html(),
            "RequestId": $('#spnRequestId').html(),
            "IsHold": $('input:radio[name=IsHold]:checked').val(),
            "HoldReason": $("#txtHoldReason").val(),
            "UnHoldReason": $("#txtUnHoldReason").val().length > 0 ?$("#txtUnHoldReason").val() : null,
        }, 
        success: function (result) {
            if (result == DataSave) {
                toastr.success('ICard Request Hold has been saved');

                $("#AddICardRequestHold").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('ICard Request Hold has been Updated');

                $("#AddICardRequestHold").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {
                toastr.error('Request Id Exits!');
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

function Reset() {
    $("#spnICardHoldId").html("0");
    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#lblRank").html("");
    $("#lblName").html("");
    $("#lblUnitName").html("");
    $("#txtHoldReason").val("");
    $("#IsHoldYes").prop("checked", false);
    $("#IsHoldNo").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtArmyNo-error").html("");
    $("#txtHoldReason-error").html("");
    $("#IsHold-error").html("");
}