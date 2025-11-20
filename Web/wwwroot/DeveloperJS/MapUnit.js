var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    mMsater(0, "ddlCommand", 1, "");
    BindDataMapUnit();

    $("#txtSusno").autocomplete({
        source: function (request, response) {
            $("#lblUnit").html('');
            if (request.term.length > 2) {
                $("#spnUnitId").html('');
                var param = { "SUSNo": request.term };
                $("#spnUnitId").html(0);
                $.ajax({
                    url: '/Master/GetTopBySUSNo',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.Sus_no, value: item.UnitId };
                            }))
                        }
                        else {
                            $("#txtSusno").val("");
                            $("#lblUnit").html("");
                            $("#spnUnitId").html("");
                            alert("SUS No not found.")
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

            $("#spnUnitId").html(i.item.value);
            $("#txtSusno").val(i.item.label);
            var param1 = { "UnitId": i.item.value };
            $.ajax({
                url: '/Master/GetUnitByUnitId',
                method: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype: 'json',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                success: function (data) {
                    $("#lblUnit").html(data.UnitName);
                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    document.getElementById('txtSusno').addEventListener('keyup', function (e) {
        if (e.key === 'Delete') { // Use `e.key` instead of `e.keyCode`
            document.getElementById('spnUnitId').innerHTML = '0';
            document.getElementById('txtSusno').value = '';
            document.getElementById('lblUnit').innerHTML = '';
        }
    });

    $('input[name="UnitTyperdi"]').on("click",function () {
        var lst = '<option value="1">Please Select</option>';
        var val = $("input[type='radio'][name=UnitTyperdi]:checked").val();
        if (val == "1") {
            $(".unittype").removeClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").addClass("d-none");

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();

            mMsater(0, "ddlCommand", 1, "");

            $("#ddlFmnBranch").html(lst);
            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

        }
        else if (val == "2") {

            $('#ddlCommand option').remove();
            $('#ddlCorps option').remove();
            $('#ddlBde option').remove();
            $('#ddlDiv option').remove();
            $('#ddlFmnBranch option').remove();

            mMsater(0, "ddlCommand", 1, "");
            mMsater(0, "ddlFmnBranch", FmnBranches, "");

            $("#ddlPSODte").html(lst);
            $("#ddlDgSubDte").html(lst);

            $(".unittype").removeClass("d-none");
            $(".FmnBranch").removeClass("d-none");
            $(".DteBranch").addClass("d-none");
        }
        else if (val == "3") {
            $(".unittype").addClass("d-none");
            $(".FmnBranch").addClass("d-none");
            $(".DteBranch").removeClass("d-none");

            $('#ddlPSODte option').remove();
            $('#ddlDgSubDte option').remove();

            $("#ddlCommand").html(lst);
            $("#ddlCorps").html(lst);
            $("#ddlBde").html(lst);
            $("#ddlDiv").html(lst);
            $("#ddlFmnBranch").html(lst);

            mMsater(0, "ddlPSODte", PSO, "");
            mMsater(0, "ddlDgSubDte", SubDte, "");

        }
    });
    $("#btnMapUnitAdd").on("click",function () {
        ResetMapUnit();
        $("#AddNewUnitmap").modal('show');
    });
    $('#ddlCommand').on('change', function () {
        mMsater(0, "ddlCorps", 2, $('#ddlCommand').val());
    });

    $('#ddlCorps').on('change', function () {
        mMsaterByParent(0, "ddlDiv", 3, $('#ddlCommand').val(), $('#ddlCorps').val(), 0, 0); ///ComdId,CorpsId,DivId,BdeId
    });
    $('#ddlDiv').on('change', function () {
        mMsaterByParent(0, "ddlBde", 4, $('#ddlCommand').val(), $('#ddlCorps').val(), $('#ddlDiv').val(), 0); ///ComdId,CorpsId,DivId,BdeId
    });
    //$('#ddlBde').on('change', function () {
    //    //BindDataMapUnit();
    //});
    $("#btnUnitMapReset").on("click",function () {
        ResetMapUnit();
    });

    $("#btnUnitMapsave").on("click", function () {
        try {
            if ($("#SaveFormMapUnit")[0].checkValidity()) {

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
                        if (isNumeric($("#txtSusno").val().substring(0, 7)) == true && isNumeric($("#txtSusno").val().substring(8, 7)) == false) {
                            SaveUnitWithMapping();

                        }
                        else {
                            toastr.error('SUSNO Should be first 7 digit Numeric and last digit alphaBat!');
                        }
                    }
                });

            } else {
                $("#SaveFormMapUnit")[0].reportValidity();
            }
        }
        catch (error) {
        console.error("Error updating row:", error);
        }
    });


    $('#btnMapUnitMultiDelete').on("click",function () {
        var lst = new Array();

        if (memberTable.$('input[type="checkbox"]:checked').length > 0) {

            memberTable.$('input[type="checkbox"]:checked').each(function () {


                var id = $(this).attr("Id");
                lst.push(id);
                console.log(id);

            });

            Swal.fire({
                title: 'Are you sure?',
                text: "You want to Delete",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#072697',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Delete it!'
            }).then((result) => {
                if (result.value) {

                    DeleteMapUnitMultiple(lst);

                }
            });
        }
        else {
            Swal.fire({
                text: "Please select atleast 1 data to Delete."
            });
        }
    });


    //$('#txtSusno').on('input', function () {
    //    $("#txtUnit").val("");
    //    $("#SpnUnitMapId").html(0); spnUnitMapId
    //    $('#txtUnit').attr('readonly', false);
    //    if ($(this).val().length > 7) {
    //        GetUnitDetails($(this).val(),1);
    //}
    //});
});
async function GetUnitDetails(val, flag) {
    const userdata = {
        Sus_no: val,
    };

    try {
        const response = await fetch('/Master/GetBySusNO', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: new URLSearchParams(userdata).toString(), // Convert object to URL-encoded string
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json(); // Parse JSON response

        if (data != null && data !== "null") {
            if (data === InternalServerError) {
                Swal.fire({
                    text: errormsg,
                });
            } else if (data === 0) {
                document.getElementById('txtUnit').value = '';
                document.getElementById('SpnUnitMapId').innerHTML = '0';
                document.getElementById('txtUnit').readOnly = false;
            } else {
                document.getElementById('txtUnit').value = data.UnitName;
                document.getElementById('txtUnit').readOnly = true;
                document.getElementById('SpnUnitMapId').innerHTML = data.UnitId;

                if (flag == 2) {
                    await SaveUnitMap();
                }
            }
        } else {
            document.getElementById('txtUnit').value = '';
            document.getElementById('SpnUnitMapId').innerHTML = '0';
            document.getElementById('txtUnit').readOnly = false;
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            text: errormsg002,
        });
    }
}
function BindDataMapUnit() {
    $("#tbldata").DataTable().destroy();
    //if ($.fn.DataTable.isDataTable("#tbldata")) {
    //    $("#tbldata").DataTable().destroy();
    //}

    table = $("#tbldata").DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        order: [[0, 'desc']], // Default sorting on the first column
        ajax: async function (data, callback, settings) {
            let requestData = {
                draw: data.draw,
                start: data.start,
                length: data.length,
                searchValue: data.search.value,
                sortColumn: data.order.length > 0 ? data.columns[data.order[0].column].data : '',  // Add a check for data.order
                sortDirection: data.order.length > 0 ? data.order[0].dir : '', // Add a check for data.order
            };
            try {
                let response = await fetch("/Master/GetAllMapUnit", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/x-www-form-urlencoded",
                        'RequestVerificationToken': globalThis.RequestVerificationToken
                    },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                callback(result); // Sends data to DataTables
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: [
            { data: "UnitMapId", name: "UnitMapId", visible: false },
            // Serial number column
            {
                data: null,
                name: "SerialNumber",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row, meta) {
                    // Calculate serial number based on row index
                    return meta.row + (meta.settings?._iDisplayStart || 0) + 1;
                }
            },
            { data: "Sus_no", name: "Sus_no" },
            { data: "UnitName", name: "UnitName", orderable: false },
            // Display user-friendly value for UnitType
            {
                data: "UnitType",
                name: "UnitType",
                render: function (data) {
                    let types = { 1: "Unit", 2: "Fmn HQ", 3: "Dte/Br" };
                    return `<span class='badge bg-primary'>${types[data] || ""}</span>`;
                }
            },
            { data: "BdeName", name: "BdeName", orderable: false },
            { data: "DivName", name: "DivName" },
            { data: "CorpsName", name: "CorpsName" },
            { data: "ComdName", name: "ComdName" },
            { data: "BranchName", name: "BranchName" },
            { data: "SubDteName", name: "SubDteName" },
            { data: "PSOName", name: "PSOName" },
            // Display user-friendly value for IsVerify
            {
                data: "IsVerify",
                name: "IsVerify",
                render: function (data) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-success'>Verifed</span>" : "<span class='badge badge-danger'>Not Verify</span>";
                }
            },
            // Additional column for Edit action
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    return "<span id='btnedit'><button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button></span><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button>";
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "UNIT SUS No" // Add custom placeholder
        },
        dom: 'lBfrtip', // Add buttons to the DOM
        buttons: [
            {
                extend: 'copy',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                }
            },
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL',
                title: 'E-IASC_Unit',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        drawCallback: function (settings) {
            // Re-bind the click event after each draw
            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();

                if (rowData != null) {
                    $("#spnUnitMapId").html(rowData.UnitMapId);
                    $("#spnUnitId").html(rowData.UnitId);
                    $("#lblUnit").html(rowData.UnitName);
                    $("#txtSusno").val(rowData.Sus_no);

                    var lst = '<option value="1">Please Select</option>';

                    if (rowData.UnitType == 1) {
                        $("#UnitType1").prop("checked", true);

                        mMsater(rowData.ComdId, "ddlCommand", 1, "");
                        mMsater(rowData.CorpsId, "ddlCorps", 2, rowData.ComdId);
                        mMsaterByParent(rowData.DivId, "ddlDiv", 3, rowData.ComdId, rowData.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                        mMsaterByParent(rowData.BdeId, "ddlBde", 4, rowData.ComdId, rowData.CorpsId, rowData.DivId, 0);///ComdId,CorpsId,DivId,BdeId

                        $(".unittype").removeClass("d-none");
                        $(".FmnBranch").addClass("d-none");
                        $(".DteBranch").addClass("d-none");

                        $("#ddlFmnBranch").html(lst);
                        $("#ddlPSODte").html(lst);
                        $("#ddlDgSubDte").html(lst);
                    }
                    else if (rowData.UnitType == 2) {
                        $("#UnitType2").prop("checked", true);

                        mMsater(rowData.ComdId, "ddlCommand", 1, "");
                        mMsater(rowData.CorpsId, "ddlCorps", 2, rowData.ComdId);
                        mMsaterByParent(rowData.DivId, "ddlDiv", 3, rowData.ComdId, rowData.CorpsId, 0, 0);///ComdId,CorpsId,DivId,BdeId
                        mMsaterByParent(rowData.BdeId, "ddlBde", 4, rowData.ComdId, rowData.CorpsId, rowData.DivId, 0);///ComdId,CorpsId,DivId,BdeId
                        mMsater(rowData.FmnBranchID, "ddlFmnBranch", FmnBranches, "");

                        $("#ddlPSODte").html(lst);
                        $("#ddlDgSubDte").html(lst);

                        $(".unittype").removeClass("d-none");
                        $(".FmnBranch").removeClass("d-none");
                        $(".DteBranch").addClass("d-none");

                    }
                    else if (rowData.UnitType == 3) {
                        $("#UnitType3").prop("checked", true);

                        mMsater(rowData.PsoId, "ddlPSODte", PSO, "");
                        mMsater(rowData.SubDteId, "ddlDgSubDte", SubDte, "");

                        $(".unittype").addClass("d-none");
                        $(".FmnBranch").addClass("d-none");
                        $(".DteBranch").removeClass("d-none");

                        $("#ddlFmnBranch").html(lst);
                        $("#ddlCommand").html(lst);
                        $("#ddlCorps").html(lst);
                        $("#ddlCorps").html(lst);
                        $("#ddlBde").html(lst);
                        $("#ddlDiv").html(lst);
                    }
                    if (rowData.IsVerify === true) {
                        $("#isverifyyes").prop("checked", true);
                    }
                    else {
                        $("#isverifyno").prop("checked", true);
                    }

                    $("#AddNewUnitmap").modal('show');
                    $("#btnMapUnitsave").val("Update");

                }
            });
            $("#tbldata tbody").off("click", ".cls-btnDelete").on("click", ".cls-btnDelete", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
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
                            DeleteMapUnit(rowData.UnitMapId);
                        }
                    });
                }
            });
        }
    });
}

 function SaveUnitWithMapping() {
    const data = {
        UnitId: document.getElementById('spnUnitId').innerHTML,
        UnitMapId: document.getElementById('spnUnitMapId').innerHTML,
        Sus_no: document.getElementById('txtSusno').value.trim().substring(0, 7),
        Suffix: document.getElementById('txtSusno').value.trim().substring(8, 7),
        IsVerify: document.querySelector('input[type="radio"][name="IsVerify"]:checked')?.value,
        UnitType: document.querySelector('input[type="radio"][name="UnitTyperdi"]:checked')?.value,
        ComdId: document.getElementById('ddlCommand').value,
        CorpsId: document.getElementById('ddlCorps').value,
        DivId: document.getElementById('ddlDiv').value,
        BdeId: document.getElementById('ddlBde').value,
        PsoId: document.getElementById('ddlPSODte').value,
        FmnBranchID: document.getElementById('ddlFmnBranch').value,
        SubDteId: document.getElementById('ddlDgSubDte').value,
    };

    try {
        fetch('/Master/SaveUnitWithMapping', {
            method: 'POST',
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: new URLSearchParams(data).toString()
        }).then(response => {
            // Handle the response
            return response.json();
        }).then(result => {
            if (result === DataSave) {
                Swal.fire({
                    icon: 'info',
                    title: 'Unit',
                    html: 'Unit has been successfully mapped',
                });
                $('#AddNewUnitmap').modal('hide'); // Hide modal
                BindDataMapUnit();
                ResetMapUnit();
            } else if (result === DataUpdate) {
                Swal.fire({
                    icon: 'info',
                    title: 'Unit',
                    html: 'Unit has been successfully updated',
                });
                $('#AddNewUnitmap').modal('hide'); // Hide modal
                BindDataMapUnit();
                ResetMapUnit();
            } else if (result === DataExists) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Unit Already Exist!',
                });
            } else if (result === InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',
                });
            } else if (Array.isArray(result) && result.length > 0) {
                result.forEach((error) => {
                    toastr.error(error[0].ErrorMessage); // Display error messages
                });
            }
        });
  
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'An error occurred while saving the unit.',
        });
    }
}

async function UnitSave() {
    const data = {
        Sus_no: document.getElementById('txtSusno').value.substring(0, 7),
        UnitId: 0,
        Suffix: document.getElementById('txtSusno').value.substring(8, 7),
        UnitName: document.getElementById('txtUnit').value,
        IsVerify: false,
    };

    try {
        const response = await fetch('/Master/SaveUnit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Use JSON for modern APIs
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: JSON.stringify(data), // Convert data to JSON
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const result = await response.json(); // Parse JSON response

        if (result === DataSave) {
            toastr.success('Unit has been saved');
            await GetUnitDetails(document.getElementById('txtSusno').value, 2); // Call GetUnitDetails with await
        } else if (result === DataUpdate) {
            toastr.success('Unit has been Updated');
        } else if (result === DataExists) {
            toastr.error('Unit Name Exits!');
        } else if (result === InternalServerError) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Something went wrong or Invalid Entry!',
            });
        } else if (Array.isArray(result) && result.length > 0) {
            result.forEach((error) => {
                toastr.error(error[0].ErrorMessage); // Display error messages
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'An error occurred while saving the unit.',
        });
    }
}

async function SaveUnitMap() {
    const data = {
        UnitName: document.getElementById('txtUnit').value,
        ComdId: document.getElementById('ddlCommand').value,
        CorpsId: document.getElementById('ddlCorps').value,
        DivId: document.getElementById('ddlDiv').value,
        BdeId: document.getElementById('ddlBde').value,
        UnitMapId: document.getElementById('spnUnitMapUnitId').innerHTML,
        UnitId: document.getElementById('SpnUnitMapId').innerHTML,
        UnitType: document.querySelector('input[type="radio"]:checked')?.value,
        PsoId: document.getElementById('ddlPSODte').value,
        FmnBranchID: document.getElementById('ddlFmnBranch').value,
        SubDteId: document.getElementById('ddlDgSubDte').value,
    };

    try {
        const response = await fetch('/Master/SaveMapUnit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Use JSON for modern APIs
                'RequestVerificationToken': globalThis.RequestVerificationToken
            },
            body: JSON.stringify(data), // Convert data to JSON
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const result = await response.json(); // Parse JSON response

        if (result === DataSave) {
            toastr.success('Unit has been saved');
            ResetMapUnit();
            BindDataMapUnit();
            $('#AddNewUnitmap').modal('hide'); // Hide modal
        } else if (result === DataUpdate) {
            toastr.success('Unit has been Updated');
            ResetMapUnit();
            BindDataMapUnit();
            $('#AddNewUnitmap').modal('hide'); // Hide modal
        } else if (result === DataExists) {
            toastr.error('Unit Name Exits!');
        } else if (result === InternalServerError) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Something went wrong or Invalid Entry!',
            });
        } else if (Array.isArray(result) && result.length > 0) {
            result.forEach((error) => {
                toastr.error(error[0].ErrorMessage); // Display error messages
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'An error occurred while saving the unit mapping.',
        });
    }
}

function ResetMapUnit() {
    $("#ddlCommand").val("");
    $("#ddlCorps").val("");
    $("#ddlDiv").val("");
    $("#ddlBde").val("");
    $("#txtSusno").val("");
    $("#lblUnit").html("");

    $("#spnUnitMapUnitId").html("0");
    $("#SpnUnitMapId").html("0");
    $("#btnsave").val("Save");
    $("#txtUnit").val("");

    $("ddlPSODte").val("");
    $("ddlDgSubDte").val("");
    $("ddlFmnBranch").val("");
}

function DeleteMapUnit(Id) {
    var userdata =
    {
        "UnitMapId": Id,

    };
    $.ajax({
        url: '/Master/DeleteMapUnit',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: async function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }
                else if (response == "5") {
                    toastr.error('UnitMapId is used in child table.');
                }

                else if (response == Success) {
                    //lol++;
                    //if (lol == Tot) {

                    toastr.success('Deleted Selected!');

                    BindDataMapUnit();
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

function DeleteMapUnitMultiple(Id) {

    var userdata =
    {
        "ints": Id,

    };
    $.ajax({
        url: '/Master/DeleteMapUnitMultiple',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: async function (response) {
            if (response != "null") {
                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {
                    Swal.fire({
                        text: "No found."
                    });
                }

                else if (response == Success) {
                    //lol++;
                    //if (lol == Tot) {

                    toastr.success('Deleted Selected!');

                    BindDataMapUnit();
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