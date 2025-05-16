$(function () {
    mMsater(0, "ddlArmType", 9, "");
    BindData()
    $("#btnAddRegimental").on("click",function () {
        Reset();
        ResetErrorMessage();
        $("#AddNewRegimental").modal('show');
    });

    $("#btnResetRegimental").on("click",function () {
        Reset();
        ResetErrorMessage();
    });

    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                $("#spnUnitMapId").html('');
                const param = new URLSearchParams({ UnitName: request.term });

                $("#spnUnitMapId").html(0);

                fetch('/Master/GetALLByUnitName', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: param
                })
                    .then(res => {
                        if (!res.ok) {
                            throw new Error('Network response was not ok');
                        }
                        return res.json();
                    })
                    .then(data => {
                        if (data.length !== 0) {
                            response(data.map(item => {
                                $("#loading").addClass("d-none");
                                return {
                                    label: item.Sus_no + item.Suffix + ' ' + item.UnitName,
                                    value: item.UnitMapId
                                };
                            }));
                        } else {
                            $("#txtUnitName").val("");
                            $("#spnUnitMapId").html("");
                            alert("Unit not found.");
                        }
                    })
                    .catch(error => {
                        alert(error.message);
                    });
            }
        },
        select: function (e, i) {
            e.preventDefault();
            $("#txtUnitName").val(i.item.label);
            $("#spnUnitMapId").html(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

    $('#txtUnitName').on('keyup',function (e) {
        if (e.key === 'Delete') {
            $("#txtUnitName").val("");
            $("#spnUnitMapId").html("");
            $("#ddlTDMId").find("option").not(":first").remove();
            $("#ddlTDMId").val("0");
        }
    });
   
    $("#btnSaveRegimental").on("click",function () {
        Proceed();
    });
});
function Proceed() {
    ResetErrorMessage();

    let formId = '#SaveRegimental';
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
        url: '/Master/GetAllRegimental',
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
                            listItem += "<td class='d-none'><span id='spnMRegId'>" + response[i].RegId + "</span><span id='spnArmedId'>" + response[i].ArmedId + "</span><span id='spnUnitId'>" + response[i].UnitId + "</span><span id='spnSus_no'>" + response[i].Sus_no + "</span><span id='spnSuffix'>" + response[i].Suffix + "</span><span id='spnUnitName'>" + response[i].UnitName + "</span></td>";
                            listItem += "<td class='align-middle'>" + (i+1) + "</td>";
                            listItem += "<td class='align-middle'><span id='Name'>" + response[i].Name + "</span></td>";
                            listItem += "<td class='align-middle'><span id='abbreviation'>" + response[i].Abbreviation + "</span></td>";
                            listItem += "<td class='align-middle'><span id='Location'>" + response[i].Location + "</span></td>";
                            listItem += "<td class='align-middle'><span id='ArmedName'>" + response[i].ArmedName + "</span></td>";
                            if (response[i].UnitId !=null)
                                listItem += "<td class='align-middle'><span id='ArmedName'>" + response[i].UnitAbbreviation + "</span></td>";
                            else
                                listItem += "<td class='align-middle'></td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";


                            /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                            listItem += "</tr>";
                       
                    }

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(response.length);
                  
                    memberTable = $('#tblData').DataTable({
                        retrieve: true,
                        lengthChange: true,
                        stateSave: true,
                        "order": [[1, "asc"]],
                        dom: 'lBfrtip', // Add buttons to the DOM
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
                        Reset();
                        ResetErrorMessage();
                        $("#txtName").val($(this).closest("tr").find("#Name").html());
                        $("#txtAbbreviation").val($(this).closest("tr").find("#abbreviation").html().toUpperCase());
                        $("#txtLocation").val($(this).closest("tr").find("#Location").html());
                       
                        $("#spnRegId").html($(this).closest("tr").find("#spnMRegId").html());

                        $("#ddlArmType").val($(this).closest("tr").find("#spnArmedId").html());

                        if ($(this).closest("tr").find("#spnUnitId").html() != null && $(this).closest("tr").find("#spnUnitId").html() != "null") {
                            $("#spnUnitMapId").html($(this).closest("tr").find("#spnUnitId").html());
                            $("#txtUnitName").val($(this).closest("tr").find("#spnSus_no").html() + $(this).closest("tr").find("#spnSuffix").html() + " " + $(this).closest("tr").find("#spnUnitName").html());
                        }
                        else {
                            $("#spnUnitMapId").html("0");
                            $("#txtUnitName").val("");
                        }
                        $("#btnSaveRegimental").val("Update");
                        $("#AddNewRegimental").modal('show');
                        
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
                                
                                Delete($(this).closest("tr").find("#spnMRegId").html());

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
    const payload = {
        Name: $("#txtName").val().trim(),
        RegId: $("#spnRegId").html(),
        Abbreviation: $("#txtAbbreviation").val().trim(),
        ArmedId: $("#ddlArmType").val(),
        Location: $("#txtLocation").val().trim(),
        UnitId: (() => {
            let val = $("#spnUnitMapId").html().trim();
            return val === "0" || val === "" ? null : parseInt(val, 10);
        })()
    };
    fetch('/Master/SaveRegimental', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json' // change to JSON
        },
        body: JSON.stringify(payload) // send proper JSON
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // or use response.text() depending on server response type
        })
        .then(result => {
            if (result === DataSave) {
                toastr.success('Regimental has been saved');
                $("#AddNewRegimental").modal('hide');
                BindData();
                Reset();
            } else if (result === DataUpdate) {
                toastr.success('Regimental has been updated');
                $("#AddNewRegimental").modal('hide');
                BindData();
                Reset();
            } else if (result === DataExists) {
                toastr.error('Regimental / Abbreviation Name exists!');
            } else if (result === InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or invalid entry!',
                });
            } else {
                if (Array.isArray(result) && result.length > 0) {
                    for (let i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage);
                    }
                }
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
            toastr.error('Failed to save data. Please try again.');
        });
}

function Reset() {
    $("#txtName").val("");
    $("#txtAbbreviation").val("");
    $("#txtLocation").val("");
    $("#txtUnitName").val("");
    $("#ddlArmType").val("0");

    $("#spnUnitMapId").html("0");
    $("#spnRegId").html("0");
}
function ResetErrorMessage() {
    $("#txtName-error").html("");
    $("#txtAbbreviation-error").html("");
    $("#txtLocation-error").html("");
    $("#txtUnitName-error").html("");
    $("#ddlArmType-error").html("");
}
function Delete(Id) {
    const userdata = new URLSearchParams({ RegId: Id });

    fetch('/Master/DeleteRegimental', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: userdata
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // Important: parse JSON instead of text
        })
        .then(response => {
            if (response !== null) {
                if (response === InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                } else if (response === Success) {
                    toastr.success('Deleted Selected');
                    BindData();
                }
            } else {
                Swal.fire({
                    text: errormsg001
                });
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
            Swal.fire({
                text: errormsg002
            });
        });
}

function DeleteMultiple(ids) {
   
    var userdata =
    {
        "ints": ids,

    };
    $.ajax({
        url: '/Master/DeleteRegimentalMultiple',
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
                     toastr.error('Deleted Selected');
                    BindData();
                }

                //}
            }
           
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}