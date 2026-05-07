$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    applyDataTableSearchValidation('#tbldata');

    BindData();
});
function BindData() {
    var listItem = "";
    $.ajax({
        url: '/Home/GetAllRegisterUser',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
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
                    var memberTable = $('#tbldata').DataTable({
                        "language": {
                            "emptyTable": "No data available"
                        }
                    });


                }

                else {

                    $("#tbldata").DataTable().destroy();

                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'>" + response[i].DomainId;

                        if (response[i].ArmyNo != null && response[i].ArmyNo != "null")
                            listItem += "<td class='align-middle'>" + response[i].ArmyNo + "</td>";
                        else
                            listItem += "<td class='align-middle'><span class='badge badge-pill badge-danger' id='domain_approval'>IC No Not Mapped</span></td>";

                        if (response[i].Rank != null && response[i].Rank != "null")
                            listItem += "<td class='align-middle'>" + response[i].Rank + "</td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_approval'>NA</span></span></td>";

                        if (response[i].Name != null && response[i].Name != "null")
                            listItem += "<td class='align-middle'>" + response[i].Name + "</td>";
                        else
                            listItem += "<td class='align-middle'><span><span class='badge badge-pill badge-danger' id='domain_approval'>NA</span></span></td>";

                        listItem += "<td class='align-middle'>" + response[i].AppointmentName;
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length - 1);

                    var memberTable = $('#tbldata').DataTable({
                        scrollY: '65vh',          // ✅ vertical scroll
                        scrollX: true,            // ✅ horizontal scroll
                        scrollCollapse: true,
                        fixedHeader: false,       // ❌ disable when using scrollY
                        retrieve: true,
                        lengthChange: false,
                        searching: false,
                        "order": [[0, "asc"]],
                        buttons: [
                            //{
                            //    extend: 'copy',
                            //    exportOptions: {
                            //        columns: "thead th:not(.noExport)"
                            //    }
                            //},
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: "thead th:not(.noExport)"
                                }
                            }, {
                                extend: 'pdfHtml5',
                                orientation: 'portrait',
                                pageSize: 'A4',
                                title: 'E-IASC_User_Regn',
                                exportOptions: {
                                    columns: "thead th:not(.noExport)"
                                },
                                customize: function (doc) {
                                    WaterMarkOnPdf(doc)
                                }
                            }]
                    });

                    memberTable.buttons().container().appendTo('#tbldata_wrapper .col-md-6:eq(0)');
                }
            }
            else {
                $("#tbldata").DataTable().destroy();

                $("#DetailBody").html(listItem);
                var memberTable = $('#tbldata').DataTable({
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