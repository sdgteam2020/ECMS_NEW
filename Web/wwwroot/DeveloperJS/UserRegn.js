var table; // Declare table variable outside the function to preserve the instance
$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();

    BindData("");
    AccountCount();
    $("#btnDomainFlag").on("click", function () {
        Proceed();
    });
    $("#btnAddMapping").on("click", function () {
        ProceedForMapping();
    });

    $("#AddNewDomain input[name='txtapproval']").on("click",function () {
        $("#txtapproval-error").html("");
    });
    $("#AddNewDomain input[name='txtactive']").on("click",function () {
        $("#txtactive-error").html("");
    });

    $("#txtArmyNo").autocomplete({
        source: function (request, response) {
            $("#lblName").html('');
            $("#lblRank").html('');
            if (request.term.length > 2) {
                $("#spnUserProfileId").html('');
                var param = { "ArmyNo": request.term };
                $("#spnUserProfileId").html(0);
                $.ajax({
                    url: '/UserProfile/GetTopByArmyNo',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return { label: item.ArmyNo, value: item.UserId };
                            }))
                        }
                        else {
                            $("#txtArmyNo").val("");
                            $("#spnUserProfileId").html("");
                            alert("Offrs Army No not found.")
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

            $("#spnUserProfileId").html(i.item.value);
            $("#txtArmyNo").val(i.item.label);
            var param1 = { "UserId": i.item.value };
            $.ajax({
                url: '/UserProfile/GetProfileByUserId',
                method: 'POST',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                datatype: 'json',
                headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
                success: function (data) {
                    $("#lblName").html(data.Name);
                    $("#lblRank").html(data.RankName);
                }
            });
        },
        
    });

    $('#txtArmyNo').on("keyup",function (e) {
        if (e.which == 46) {
            $("#spnUserProfileId").html('0');
            $("#txtArmyNo").val('');
            $("#lblName").html('');
            $("#lblRank").html('');
        }
    });

    $("#txtSearch").on("keyup",function () {
        var eThis = $(this);
        if ($("input[type='radio'][name=choice]:checked").length > 0) {
            var ChoiceValue = $("input[type='radio'][name=choice]:checked").val();
            if (ChoiceValue == "Id") {
                var num_val = parseInt(eThis.val());
                if (isNaN(num_val)) {
                    alert("Enter only number");
                    eThis.val('')
                }
                else {
                    eThis.val(num_val)
                    BindData(ChoiceValue);
                }
            }
            else {
                BindData(ChoiceValue);
            }
        }
        else {
            alert("Select Choice");
        }
    });

    $("#btnUser").on("click",function () {

        if ($("#lblUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Users');
            $("#DataTableDialog").modal('show');
            BindDialog("User");
        }
    });
    $("#btnMappedUser").on("click", function () {

        if ($("#lblMappedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Mapped Users');
            $("#DataTableDialog").modal('show');
            BindDialog("MappedUser");
        }
    });
    $("#btnUnMappedUser").on("click", function () {
        if ($("#lblUnMappedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total UnMapped Users');
            $("#DataTableDialog").modal('show');
            BindDialog("UnMappedUser");
        }
    });
    $("#btnActiveUser").on("click", function () {
        if ($("#lblActiveUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Active Users');
            $("#DataTableDialog").modal('show');
            BindDialog("ActiveUser");
        }
    });
    $("#btnInActiveUser").on("click", function () {
        if ($("#lblInActiveUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total InActive Users');
            $("#DataTableDialog").modal('show');
            BindDialog("InActiveUser");
        }
    });
    $("#btnVerified").on("click", function () {
        if ($("#lblVerifiedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("Verified");
        }
    });
    $("#btnNotVerifiedUser").on("click", function () {
        if ($("#lblNotVerifiedUser").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("NotVerifiedUser");
        }
    });
    $("#btnIO").on("click", function () {
        if ($("#lblIO").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("IO");
        }
        else {
            BindData("IO");
        }
    });
    $("#btnApprover").on("click", function () {
        if ($("#lblApprover").html() > 0) {
            $("#tbldatadialog").DataTable().destroy();
            $("#lblModelTitle").html('Total Not Verified Users');
            $("#DataTableDialog").modal('show');
            BindDialog("CO");
        }
    });
    //$("#btnRO").click(function () {
    //    if ($("#lblRO").html() > 0) {
    //        $("#tbldatadialog").DataTable().destroy();
    //        $("#lblModelTitle").html('Total Not Verified Users');
    //        $("#DataTableDialog").modal('show');
    //        BindDialog("RO");
    //    }
    //});
    //$("#btnORO").click(function () {
    //    if ($("#lblORO").html() > 0) {
    //        $("#tbldatadialog").DataTable().destroy();
    //        $("#lblModelTitle").html('Total Not Verified Users');
    //        $("#DataTableDialog").modal('show');
    //        BindDialog("ORO");
    //    }
    //});
});
function BindDialog(Choice) {
    $("#tbldatadialog").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY
        processing: true,
        serverSide: true,
        filter: true,
        order: [[1, 'desc']], // Default sorting on the first column
        ajax: {
            url: "/Account/GetDataForDataTable",
            contentType: 'application/x-www-form-urlencoded',
            type: "POST",
            headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
            data: function (d) {
                d.draw = d.draw;
                d.start = d.start;
                d.length = d.length;
                d.searchValue = d.search.value;
                d.sortColumn = d.columns[d.order[0].column].data;
                d.sortDirection = d.order[0].dir;
                d.Choice = Choice;
            },
        },
        columns: [
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
            { data: "Id", name: "Id" },
            { data: "DomainId", name: "DomainId" },
            {
                data: "ArmyNo",
                name: "ArmyNo",
                render: function (data, type, row) {
                    return data ? data : "<span class='badge badge-pill badge-danger'>IC No Not Mapped</span>";
                }
            },
            {
                data: "RoleNames",
                name: "RoleNames",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row) {
                    return data ? data.join(', ') : '';  // Convert array to string
                }
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return data ? DateFormateddMMyyyyhhmmss(data) : "NA";
                },
            },
            // Display user-friendly value for Mapped
            {
                data: "Mapped",
                name: "Mapped",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for Active
            {
                data: "Active",
                name: "Active",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for AdminFlag
            {
                data: "AdminFlag",
                name: "AdminFlag",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for IsIO
            {
                data: "IsIO",
                name: "IsIO",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for IsCO
            {
                data: "IsCO",
                name: "IsCO",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            //{ data: "IsRO", name: "IsRO" },
            //{ data: "IsORO", name: "IsORO" }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Domain ID" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
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
                title: 'E-IASC_' + $("#lblModelTitle").html(),
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }]
    });
}
function BindData() {
    $("#tbldata").DataTable().destroy();
    //if ($.fn.DataTable.isDataTable("#tbldata")) {
    //    $("#tbldata").DataTable().destroy();
    //}

    table = $("#tbldata").DataTable({
        scrollY: '65vh',          // ✅ vertical scroll
        scrollX: true,            // ✅ horizontal scroll
        scrollCollapse: true,
        fixedHeader: false,       // ❌ disable when using scrollY
        processing: true,
        serverSide: true,
        filter: true,
        stateSave: true,
        order: [[1, 'desc']], // Default sorting on the first column
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
                let response = await fetch("/Account/GetAllUserRegn", {
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
            { data: "Id", name: "Id" },
            { data: "DomainId", name: "DomainId" },
            {
                data: "ArmyNo",
                name: "ArmyNo",
                render: function (data, type, row) {
                    return data ? data : "<span class='badge badge-pill badge-danger'>IC No Not Mapped</span>";
                }
            },
            {
                data: "RoleNames",
                name: "RoleNames",
                orderable: false, // Disable sorting for this column
                render: function (data, type, row) {
                    return data ? data.join(', ') : '';  // Convert array to string
                }
            },
            {
                data: "UpdatedOn",
                name: "UpdatedOn",
                render: function (data, type, row) {
                    return data ? DateFormateddMMyyyyhhmmss(data) : "NA";
                },
            },
            // Display user-friendly value for Mapped
            {
                data: "Mapped",
                name: "Mapped",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<button type='button' class='cls-btneditMapping btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-link'></i></button>" : "<button type='button' class='cls-btneditMapping btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-unlink'></i></button>";
                },
            },
            // Display user-friendly value for Active
            {
                data: "Active",
                name: "Active",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for AdminFlag
            {
                data: "AdminFlag",
                name: "AdminFlag",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },

            // Display user-friendly value for IsIO
            {
                data: "IsIO",
                name: "IsIO",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Display user-friendly value for IsCO
            {
                data: "IsCO",
                name: "IsCO",
                render: function (data, type, row) {
                    // Convert boolean to "Yes" or "No"
                    return data ? "<span class='badge badge-pill badge-success'>Yes</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                },
            },
            // Additional column for Edit action
            {
                data: "Id",
                name: "Id",
                orderable: false,
                render: function (data, type, row) {
                    return data ? "<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>" : "NA";
                }
            }
        ],
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search Domain ID" // Add custom placeholder
        },
        dom: "<'dt-top'lBf>rtip", // Add buttons to the DOM
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
                title: 'E-IASC_UserRegn',
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
                    Reset();
                    ResetErrorMessage();
                    $("#lblDomainId").html(rowData.DomainId);
                    $("#lblRole").html(rowData.RoleNames.join(', ')); //data ? data.join(', ') : ''
                    $("#spnDomainRegId").html(rowData.Id);
                    if (rowData.AdminMsg != null) {
                        $("#txtadminmessage").val(rowData.AdminMsg);
                    }
                    else {
                        $("#txtadminmessage").val("");
                    }

                    if (rowData.AdminFlag == true) {
                        $("#txtapprovalyes").prop("checked", true);
                    }
                    else {
                        $("#txtapprovalno").prop("checked", true);
                    }

                    if (rowData.Active == true) {
                        $("#txtactiveyes").prop("checked", true);
                    }
                    else {
                        $("#txtactiveno").prop("checked", true);
                    }

                    if (rowData.TrnDomainMappingId > 0) {
                        GetALLByUnitById(rowData.TrnDomainMappingUnitId);
                    }

                    if (rowData.TrnDomainMappingApptId > 0) {
                        GetNameByApptId(rowData.TrnDomainMappingApptId);
                    }

                    $("#btnDomainFlag").val("Update");
                    $("#AddDomainFlag").modal('show');



                    $("#spnUnitMapId").html(rowData.UnitMapId);
                    $("#spnUnitId").html(rowData.UnitId);
                    $("#lblUnit").html(rowData.UnitName);
                    $("#txtSusno").val(rowData.Sus_no);

                    $("#AddNewUnitmap").modal('show');
                    $("#btnMapUnitsave").val("Update");
                }
            });
            $("#tbldata tbody").off("click", ".cls-btneditMapping").on("click", ".cls-btneditMapping", function () {
                var rowData = table.row($(this).closest("tr")).data();

                if (rowData != null) {
                    ResetForMapping();
                    ResetErrorMessageForMapping();
                    $("#lblDomainIdForMapping").html(rowData.DomainId);
                    $("#lblRoleForMapping").html(rowData.RoleNames);
                    $("#spnDomainRegIdForMapping").html(rowData.Id);

                    if (rowData.AdminFlag == true) {
                        $("#txtapprovalyesForMapping").prop("checked", true);
                    }
                    else {
                        $("#txtapprovalnoForMapping").prop("checked", true);
                    }

                    if (rowData.Active == true) {
                        $("#txtactiveyesForMapping").prop("checked", true);
                    }
                    else {
                        $("#txtactivenoForMapping").prop("checked", true);
                    }
                    if (rowData.UserId > 0) {
                        GetProfileByUserId(rowData.UserId);
                    }

                    if (rowData.TrnDomainMappingId > 0) {
                        $("#spnTrnDomainMappingIdForMapping").html(rowData.TrnDomainMappingId);
                        GetALLByUnitByIdForMapping(rowData.TrnDomainMappingUnitId);
                    }

                    if (rowData.TrnDomainMappingApptId > 0) {
                        GetNameByApptIdForMapping(rowData.TrnDomainMappingApptId);
                    }

                    $("#btnAddMapping").val("Update");
                    $("#AddMapping").modal('show');

                }
            });
        }
    });

}
function ProceedForMapping() {
    ResetErrorMessageForMapping();
    ValidateMappingInput();
    let formId = '#SaveMapping';
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
                SaveMapping();
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
function SaveMapping() {
    //alert($("#spnDomainRegId").html());
    $.ajax({
        url: '/Account/SaveMapping',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegIdForMapping").html(),
            "TDMId": $("#spnTrnDomainMappingIdForMapping").html(),
            "UserId": $("#spnUserProfileId").html(),
            "ArmyNo": $("#txtArmyNo").val(),

        }, //get the search string
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response) {
            var obj = jQuery.parseJSON(response);
            if (obj.Result == true) {
                toastr.success(obj.Message);

                $("#AddMapping").modal('hide');
                AccountCount();
                BindData("");
                ResetForMapping();
                ResetErrorMessageForMapping();
            }
            else if (obj.Result == false) {
                toastr.error(obj.Message);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    html: obj.Message,

                })
            }
            else if (obj.Result == false && obj.Message.length > 1) {
                for (var i = 0; i < obj.Message.length; i++) {
                    toastr.error(result[i][0].Message)
                }
            }
        }
    });
}
function ValidateMappingInput() {

    var UserProfileId = $("#spnUserProfileId").html();

    if ((UserProfileId == 0 || UserProfileId == '') && $("#txtArmyNo").val().length > 0) {
        $("#txtArmyNo").val('');
        $("#txtArmyNo-error").html("ArmyNo is invalid.");
        toastr.error('ArmyNo is invalid.');
    }
}
function GetProfileByUserId(param1) {
    $.ajax({
        url: '/UserProfile/GetProfileByUserId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UserId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#spnUserProfileId").html(data.UserId);
            $("#txtArmyNo").val(data.ArmyNo);
            $("#lblRank").html(data.RankName);
            $("#lblName").html(data.Name);
        }
    });
}
function GetALLByUnitByIdForMapping(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblUnitNameForMapping").html(data.UnitName);
            $("#lblSusnoForMapping").html(data.Sus_no + '' + data.Suffix);


            if (data.UnitType == 1) {
                $("#lblComdForMapping").html(data.ComdName);
                $("#lblCorpsForMapping").html(data.CorpsName);
                $("#lblDivForMapping").html(data.DivName);
                $("#lblBdeForMapping").html(data.BdeName);
                $("#lblM1").addClass("d-none");
                $("#lblM2").addClass("d-none");
                $("#lblM3").removeClass("d-none");
                $("#lblM4").removeClass("d-none");
                $("#lblM5").removeClass("d-none");
                $("#lblM6").removeClass("d-none");
                $("#lblM7").addClass("d-none");
            }
            else if (data.UnitType == 2) {
                $("#lblComdForMapping").html(data.ComdName);
                $("#lblCorpsForMapping").html(data.CorpsName);
                $("#lblDivForMapping").html(data.DivName);
                $("#lblBdeForMapping").html(data.BdeName);
                $("#lblFmnForMapping").html(data.BranchName);
                $("#lblM1").addClass("d-none");
                $("#lblM2").addClass("d-none");
                $("#lblM3").removeClass("d-none");
                $("#lblM4").removeClass("d-none");
                $("#lblM5").removeClass("d-none");
                $("#lblM6").removeClass("d-none");
                $("#lblM7").removeClass("d-none");
            }
            else if (data.UnitType == 3) {
                $("#lblPsoForMapping").html(data.PSOName);
                $("#lblDGForMapping").html(data.SubDteName);
                $("#lblM1").removeClass("d-none");
                $("#lblM2").removeClass("d-none");
                $("#lblM3").addClass("d-none");
                $("#lblM4").addClass("d-none");
                $("#lblM5").addClass("d-none");
                $("#lblM6").addClass("d-none");
                $("#lblM7").addClass("d-none");
            }


        }
    });
}
function GetNameByApptIdForMapping(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblAppointmentNameForMapping").html(data.AppointmentName);
        }
    });
}
function ResetForMapping() {
    $("#txtSearch").val("");

    $("#spnDomainRegIdForMapping").html("0");
    $("#lblDomainIdForMapping").html("");
    $("#lblRoleForMapping").html("");

    $("#spnTrnDomainMappingIdForMapping").html("");
    $("#lblUnitNameForMapping").html("");
    $("#lblSusnoForMapping").html("");
    $("#lblPsoForMapping").html("");
    $("#lblDGForMapping").html("");
    $("#lblComdForMapping").html("");
    $("#lblCorpsForMapping").html("");
    $("#lblDivForMapping").html("");
    $("#lblBdeForMapping").html("");
    $("#lblFmnForMapping").html("");


    $("#spnUserProfileId").html("0");
    $("#txtArmyNo").val("");
    $("#lblRank").html("");
    $("#lblName").html("");

    $("#lblAppointmentNameForMapping").html("");

    $("#txtapprovalyesForMapping").prop("checked", false);
    $("#txtapprovalnoForMapping").prop("checked", false);

    $("#txtactiveyesForMapping").prop("checked", false);
    $("#txtactivenoForMapping").prop("checked", false);
}
function ResetErrorMessageForMapping() {
    $("#txtArmyNo-error").html("");
    $("#txtadminmessage-error").html("");
}

function Proceed() {
    ResetErrorMessageForMapping();
    let formId = '#UpdateDomainFlag';
    $.validator.unobtrusive.parse($(formId));

    ValidateRadioButton();

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
                UpdateDomainFlag();
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
function UpdateDomainFlag() {
    $.ajax({
        url: '/Account/UpdateDomainFlag',
        type: 'POST',
        data: {
            "Id": $("#spnDomainRegId").html(),
            "AdminFlag": $('input:radio[name=txtapproval]:checked').val(),
            "Active": $('input:radio[name=txtactive]:checked').val(),
            "AdminMsg": $('#txtadminmessage').val().length > 0 ? $('#txtadminmessage').val() : null,
        },
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (result) {
            if (result == DataUpdate) {
                toastr.success('Domain Flag has been Updated');

                $("#AddDomainFlag").modal('hide');
                AccountCount();
                BindData("");
                Reset();
                ResetErrorMessage();
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
function ValidateRadioButton() {
    if ($("input[type='radio'][name=txtapproval]:checked").length == 0) {
        $("#txtapproval-error").html("Approval is required.");
    }
    else {
        $("#txtapproval-error").html("");
    }

    if ($("input[type='radio'][name=txtactive]:checked").length == 0) {
        $("#txtactive-error").html("Active is required.");
    }
    else {
        $("#txtactive-error").html("");
    }
}
function GetALLByUnitById(param1) {
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "UnitMapId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblUnitName").html(data.UnitName);
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
}
function GetNameByApptId(param1) {
    $.ajax({
        url: '/Master/GetByApptId',
        contentType: 'application/x-www-form-urlencoded',
        data: { "ApptId": param1 },
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (data) {
            $("#lblAppointmentName").html(data.AppointmentName);
        }
    });
}
function Reset() {
    $("#txtSearch").val("");

    $("#spnDomainRegId").html("0");
    $("#txtadminmessage").val("");
    $("#lblDomainId").html("");
    $("#lblRole").html("");

    $("#lblUnitName").html("");
    $("#lblSusno").html("");
    $("#lblPso").html("");
    $("#lblDG").html("");
    $("#lblComd").html("");
    $("#lblCorps").html("");
    $("#lblDiv").html("");
    $("#lblBde").html("");
    $("#lblFmn").html("");


    $("#lblAppointmentName").html("");

    $("#txtapprovalyes").prop("checked", false);
    $("#txtapprovalno").prop("checked", false);

    $("#txtactiveyes").prop("checked", false);
    $("#txtactiveno").prop("checked", false);
}
function ResetErrorMessage() {
    $("#txtapproval-error").html("");
    $("#txtactive-error").html("");
}
function AccountCount() {
    $.ajax({
        url: "/Account/AccountCount",
        type: "POST",
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },
        success: function (response, status) {
            $("#lblUser").html(response.User);
            $("#lblActiveUser").html(response.ActiveUser);
            $("#lblInActiveUser").html(response.InActiveUser);
            $("#lblMappedUser").html(response.MappedUser);
            $("#lblUnMappedUser").html(response.UnMappedUser);
            $("#lblVerifiedUser").html(response.VerifiedUser);
            $("#lblNotVerifiedUser").html(response.NotVerifiedUser);
            $("#lblIO").html(response.IO);
            $("#lblApprover").html(response.CO);
            $("#lblRO").html(response.RO);
            $("#lblORO").html(response.ORO);
        }
    });
}