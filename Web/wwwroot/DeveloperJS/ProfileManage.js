$(document).ready(function () {
    Reset();
    BindData()
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
                        listItem += "<td class='d-none'><span id='regId'>" + response[i].Id + "</span></td>";
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
                        $("#txtDomainId").val($(this).closest("tr").find("#domainId").html());
                        $("#txtRole").val($(this).closest("tr").find("#roleName").html());
                        $("#txtArmyNo").val($(this).closest("tr").find("#armyNo").html());
                        $("#spnDomainRegId").html($(this).closest("tr").find("#regId").html());
                        alert($(this).closest("tr").find("#domain_approval").html())
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

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Master/SaveUnit',
        type: 'POST',
        data: { "Sus_no": $("#txtSusno").val(), "UnitId": $("#spnUnitId").html(), "Suffix": $("#txtSuffix").val(), "UnitName": $("#txtUnitDesc").val(), "Abbreviation": $("#txtAbbreviation").val(), "IsVerify": true }, //get the search string
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
}