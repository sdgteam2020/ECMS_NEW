$(function () {
    BindData()
    $("#btnAdd").on("click",function () {
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(FaultyCardRequest);
    });
});
function BindData() {
    var listItem = "";
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/BasicDetail/GetAllFaulty',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {
                if (response == -1) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=4>No Record Found</td></tr>";
                    $("#tbldata").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }
                else if (response == InternalServerError) {
                    listItem += "<tr><td class='text-center' colspan=4>No Record Found</td></tr>";
                    $("#tbldata").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {
                    $("#tbldata").DataTable().destroy();
                    for (var i = 0; i < response.length; i++) {
                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnTrnFaultyCardId'>" + response[i].TrnFaultyCardId + "</span></td>";
                        listItem += "<td class='align-middle'>" + (i + 1) + "</td>";
                        listItem += "<td class='align-middle'><span id='spnRequestId'>" + response[i].RequestId + "</span></td>";
                        listItem += "<td class='align-middle'><span id='spnModifiedServiceNo'>" + response[i].ModifiedServiceNo + "</span></td>";
                        let fullName = `${response[i].RankName || ""} ${response[i].FName || ""} ${response[i].LName || ""}`.trim();
                        listItem += "<td class='align-middle'><span id='spnfullName'>" + fullName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='spnUnitAbbreviation'>" + response[i].UnitAbbreviation + "</span></td>";
                        listItem += "<td class='align-middle'><span id='spnUpdatedOn'>" + DateFormateddMMyyyyhhmmss(response[i].UpdatedOn) + "</span></td>";

                        if (response[i].RemarksIds != null) {
                            var remarksArray = response[i].RemarksNameList.split('#');
                            if (remarksArray != null) {
                                listItem += "<td class='align-middle'><button type='button' class='cls-remarks btn btn-icon btn-round btn-warning mr-1'><i class='fa fa-eye'></i><span id='spnRemarks' class='d-none'><ul>";
                                for (var j = 0; j < remarksArray.length; j++) {
                                    listItem += "<li>" + remarksArray[j] + "</li>";
                                }
                                listItem += "</ul></span></button></td>";
                            }
                        }
                        else {
                            listItem += "<td class='align-middle'></td>";
                        }
                        let sentence = response[i].FromRemark;
                        let words = sentence.split(" ");

                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : sentence;

                        listItem += "<td class='align-middle'><span class='cls-FromRemark'>" + truncatedSentence + "<span id='spanFromRemark' class='d-none'>" + sentence + "</span></span></td>";
                        listItem += `<td class='align-middle'><span id='spnName'> ${response[i].ToRemark ?? "NA"}</span></td>`;
                        if ($("#spnClaimValue").html().toLowerCase() === "true") {
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-primary mr-1'><i class='fas fa-edit'></i></button></span></td>";
                        }
                        

                        listItem += "</tr>";
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length);

                    memberTable = $('#tbldata').DataTable({
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
                            title: 'E-IASC_Appoinment',
                            exportOptions: {
                                columns: "thead th:not(.noExport)"
                            },
                            customize: function (doc) {
                                WaterMarkOnPdf(doc)
                            }
                        }]
                    });

                    memberTable.buttons().container().appendTo('#tbldata_wrapper .col-md-6:eq(0)');

                    var rows;
                    $("#tbldata #chkAll").on("click", function () {
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

                        $("#spnapptId").html($(this).closest("tr").find("#spnMapptId").html());
                        $("#txtAppoinment").val($(this).closest("tr").find("#appointmentName").html());
                        if ($(this).closest("tr").find("#appointmentAbbreviation").html() == "") {
                            $("#txtAbbreviation").val("");
                        }
                        else {
                            $("#txtAbbreviation").val($(this).closest("tr").find("#appointmentAbbreviation").html());
                        }
                        $("#btnsave").val("Update");
                    });


                    $("body").on("click", ".cls-remarks", function () {
                        let Label = "Request Id :- " + $(this).closest("tr").find("#spnRequestId").html();
                        $("#MessageDialogLabel").html(Label);
                        $("#MessageDialogBody").html($(this).closest("tr").find("#spnRemarks").html());
                        $("#MessageDialog").modal('show');
                    });
                    $("body").on("click", ".cls-FromRemark", function () {
                        let Label = "Request Id :- " + $(this).closest("tr").find("#spnRequestId").html();
                        $("#MessageDialogLabel").html(Label);
                        $("#MessageDialogBody").html($(this).closest("tr").find("#spanFromRemark").html());
                        $("#MessageDialog").modal('show');
                    });

                }
            }
            else {
                $("#tbldata").DataTable().destroy();
                $("#DetailBody").html(listItem);
                $("#lblTotal").html(0);
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
//function showFullSentence(fullSentence) {
//    //document.getElementById("fullSentence").textContent = fullSentence;
//    //document.getElementById("wordModal").style.display = "block";
//    $("#MessageDialogBody").html(fullSentence);
//    $("#MessageDialog").modal('show');
//}