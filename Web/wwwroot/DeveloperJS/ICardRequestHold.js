var table; // Declare table variable outside the function to preserve the instance
$(function () {
    let cvalue = $("#spnFlagICardAppl").html();
    BindData(cvalue, function () {
    });
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
function BindData(cvalue, callback) {
    if ($.fn.DataTable.isDataTable("#tbldata")) {
        $("#tbldata").DataTable().destroy();
        $("#tbldata").empty(); // Clear old thead/tbody
    }
    const columns = getColumnsData(cvalue);
    table = $("#tbldata").DataTable({
        autoWidth: false, // Let us handle width via CSS
        responsive: true, // Responsive breaks layout for width control
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
                sortDirection: data.order.length > 0 ? data.order[0].dir : '' // Add a check for data.order
            };
            try {
                let response = await fetch("/BasicDetail/GetAllICardRequestHold", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams(requestData).toString()
                });

                if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

                let result = await response.json();
                callback(result); // Sends data to DataTables


            } catch (error) {
                console.error("Error fetching data:", error);
            }
        },
        columns: columns,
        language: {
            search: "", // Remove the default "Search:" label
            searchPlaceholder: "Search" // Add custom placeholder
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
                title: 'E-IASC_ApplicationHold',
                exportOptions: {
                    columns: "thead th:not(.noExport)"
                },
                customize: function (doc) {
                    WaterMarkOnPdf(doc)
                }
            }],
        initComplete: function () {
        },
        drawCallback: function (settings) {

            $("#tbldata tbody").off("click", ".cls-btnedit").on("click", ".cls-btnedit", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData.ICardHoldId != null) {

                    Reset();
                    ResetErrorMessage();
                    $("#gpUnHoldReason").removeClass("d-none");
                    $("#txtArmyNo").prop('readonly', true);
                    $("#txtHoldReason").prop('readonly', true);
                    $("#spnICardHoldId").html(rowData.ICardHoldId);
                    $("#spnRequestId").html(rowData.RequestId);
                    $("#txtArmyNo").val(rowData.ServiceNo);
                    $("#lblRank").html(rowData.RankName);
                    $("#lblName").html(`${rowData.FName || ""} ${rowData.LName || ""}`.trim());
                    $("#lblUnitName").html(rowData.UnitName);
                    $("#txtHoldReason").val(rowData.HoldReason);
                    $("#txtUnHoldReason").val(rowData.UnHoldReason != null ? rowData.UnHoldReason :"");

                    if (rowData.IsHold == true) {
                        $("#IsHoldYes").prop("checked", true);
                    }
                    else {
                        $("#IsHoldNo").prop("checked", true);
                    }

                    $("#btnAddICardRequestHold").val("Update");
                    $("#AddICardRequestHold").modal('show');
                }
                else {
                    $("#spnDispatchCardId").html(0);
                }
            });

            $("#tbldata tbody").off("click", ".cls-HoldReason").on("click", ".cls-HoldReason", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    $("#MessageDialogLabel").html('Reason');
                    $("#MessageDialogBody").html(rowData.HoldReason);
                    $("#MessageDialog").modal('show');
                }
            });
            $("#tbldata tbody").off("click", ".cls-historyRequest").on("click", ".cls-historyRequest", function () {
                var rowData = table.row($(this).closest("tr")).data();
                if (rowData != null) {
                    GetRequestHistory(rowData.RequestId);
                }
            });
        }
    });
    $('#filterApplyFor').on('keypress', function (e) {
        if (e.which === 13) {
            table.ajax.reload();
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
function getColumnsData(choice) {
    let columns = [];
    switch (choice) {
        case 'Flag ICard Appl':
            columns = [
                // Serial number column
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
                },
                {
                    title: "Rank & Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Unit",
                    data: "UnitName",
                    name: "UnitName",
                    orderable: false,
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor",
                },
                {
                    title: "Held By",
                    data: "DomainId",
                    name: "DomainId",
                },
                {
                    title: "Reason for Held",
                    data: "HoldReason",
                    name: "HoldReason",
                    render: function (data, type, row) {
                        let words = data.split(" ");
                        let truncatedSentence = words.length > 4 ? words.slice(0, 4).join(" ") + "..." : data;
                        return `<span class='cls-HoldReason'>${truncatedSentence}</span>`;
                    }
                },
                {
                    title: "Hold",
                    data: "IsHold",
                    name: "IsHold",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    }
                },
                {
                    title: "Updated On",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "History",
                    data: null,
                    name: "History",
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left"><i class="fa fa-history" aria-hidden="true"></i></button>`
                    }
                },
                {
                    title: "Print / Edit",
                    data: null,
                    name: "Action",
                    orderable: false,
                    render: function (data, type, row) {
                        let Action = `<button type='button' class='cls-btnedit btn btn-icon btn-round btn-warning mr-1'><i class='fas fa-edit'></i></button>`;
                        return Action;
                    }
                }
            ];
            break;
        default:
            columns = [
                // Serial number column
                {
                    title: "S No",
                    data: null,
                    name: "SerialNumber",
                    orderable: false, // Disable sorting for this column
                    render: function (data, type, row, meta) {
                        // Calculate serial number based on row index
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    title: "Army No",
                    data: "ServiceNo",
                    name: "ServiceNo",
                    render: function (data, type, row) {
                        // Check if first two characters are alphabets
                        if (/^[A-Za-z]{2}/.test(data)) {
                            // Insert space after first two characters
                            return data.slice(0, 2) + ' ' + data.slice(2);
                        } else {
                            // No space needed
                            return data;
                        }
                    }
                },
                {
                    title: "Rank & Name",
                    data: null,
                    name: null,
                    orderable: false,
                    render: function (data, type, row) {
                        let fullName = `${row.RankName || ""} ${row.FName || ""} ${row.LName || ""}`.trim();
                        return (fullName);
                    }
                },
                {
                    title: "Unit",
                    data: "UnitName",
                    name: "UnitName",
                    orderable: false,
                },
                {
                    title: "Type",
                    data: "ApplyFor",
                    name: "ApplyFor",
                },
                {
                    title: "Held By",
                    data: "DomainId",
                    name: "DomainId",
                },

                {
                    title: "Reason for Held",
                    data: "HoldReason",
                    name: "HoldReason",
                },
                {
                    title: "Hold",
                    data: "IsHold",
                    name: "IsHold",
                    render: function (data, type, row) {
                        // Convert boolean to "Yes" or "No"
                        return data ? "<span class='badge badge-pill badge-success'>YES</span>" : "<span class='badge badge-pill badge-danger'>No</span>";
                    }
                },
                {
                    title: "Updated On",
                    data: "UpdatedOn",
                    name: "UpdatedOn",
                    render: function (data, type, row) {
                        return DateFormateddMMyyyyhhmmss(data);
                    }
                },
                {
                    title: "History",
                    data: null,
                    name: "History",
                    render: function (data, type, row) {
                        return `<button class="btn btn-icon btn-round btn-primary mr-1 cls-historyRequest" data-toggle="tooltip" data-placement="left" title="${row.Remark}"><i class="fa fa-history" aria-hidden="true"></i></button>`
                    }
                }
            ];
    }
    return columns;
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