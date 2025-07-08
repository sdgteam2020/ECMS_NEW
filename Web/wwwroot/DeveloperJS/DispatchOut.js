$(async function () {
    mMsater(0, "ddlDispatch", DispatchMode, "");
    let ClaimValue = $("#spnClaimValue").html();
    if (ClaimValue == 1) {
        $("#DispatchOut_Categery").removeClass("d-none");

        $("#txtUnitName").attr('readonly', true);
        
        $('#ddlCategery').on('change', async function () {
            let CategeryId = $(this).val(); // Get the selected value
            await GetddlRecordRegiment(CategeryId);
        });
        $('#ddlRecordRegiment').on('change', async function () {
            let RecordRegimentId = parseInt($(this).val()); // Get the selected value

            if (RecordRegimentId == 0) {
                $("#spnUnitMapId").html(0);
                $("#spnUserId").html(0);

                $("#txtUnitName").val(``);
                $("#txtArmyNo").val(``);
                $("#txtRkName").val(``);

                $("#ddlDID").find("option").not(":first").remove();
                $("#ddlDID").val("0");
            }
            if (RecordRegimentId > 0) {
                await GetDispatchToData($('#ddlCategery').val(), RecordRegimentId);
            }
        });
    }
    else if (ClaimValue == 2) {
        $("#DispatchOut_Categery").addClass("d-none");

        if ($('#txtUnitName').attr('readonly') !== undefined) {
            $("#txtUnitName").removeAttr('readonly');
        }

        $("#ddlCategery").val(1); // Set default value for Categery
        await GetddlRecordRegiment(1);
    }
    else if (ClaimValue == 3) {
        $("#DispatchOut_Categery").addClass("d-none");

        if ($('#txtUnitName').attr('readonly') !== undefined) {
            $("#txtUnitName").removeAttr('readonly');
        }
        $("#ddlCategery").val(2); // Set default value for Categery
        await GetddlRecordRegiment(2);
    }
    $("#txtUnitName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                $("#spnUnitMapId").html('');
                var param = { "UnitName": request.term };
                $("#spnUnitMapId").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        if (data.length != 0) {
                            response($.map(data, function (item) {
                                $("#loading").addClass("d-none");
                                return {
                                    label: `${item.Sus_no}${item.Suffix} ${item.UnitName}`,
                                    value: `${item.UnitMapId}`
                                };

                            }))
                        }
                        else {
                            $("#txtUnitName").val("");
                            $("#spnUnitMapId").html("");
                            $("#ddlDID").find("option").not(":first").remove();
                            $("#ddlDID").val("0");

                            $("#spnUserId").html(0);
                            $("#txtRkName").val(``);
                            $("#txtArmyNo").val(``);

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
            $("#spnUnitMapId").html(i.item.value);
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetDDMappedForRecord',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',

                success: function (response) {
                    if (response != "null" && response != null) {
                        if (response == InternalServerError) {
                            Swal.fire({
                                text: errormsg
                            });
                        }

                        else {

                            var listItemddl = "";

                            listItemddl += '<option value="0">Please Select</option>';

                            for (var i = 0; i < response.length; i++) {
                                listItemddl += `<option value="${response[i].AspNetUsersId}">${response[i].DomainId} ${response[i].RankAbbreviation} ${response[i].Name} ${response[i].ArmyNo}</option>`;
                            }
                            $("#ddlDID").html(listItemddl);
                        }
                    }
                    else {
                        //Swal.fire({
                        //    text: "No data found Offrs"
                        //});
                    }
                },
                error: function (result) {
                    Swal.fire({
                        text: errormsg002
                    });
                }
            });
        },
        appendTo: '#suggesstion-box'
    });

    $("#ddlDID").on('change', async function () {
        let AspNetUsersId = $(this).val(); // Get the selected value
        await GetUserIdWithName(AspNetUsersId);
    });
});
async function GetUserIdWithName(AspNetUsersId) {
    let param = new URLSearchParams({
        "AspNetUsersId": AspNetUsersId
    });

    try {
        const response = await fetch('/BasicDetail/GetUserIdWithName', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            $("#txtArmyNo").val(`${result.Value.ArmyNo}`);
            $("#spnUserId").html(result.Value.UserId);

            $("#txtRkName").val(`${result.Value.RankAbbreviation} ${result.Value.Name}`);

        } else {
            $("#spnUserId").html(0);
            $("#txtRkName").val(``);
            $("#txtArmyNo").val(``);
            toastr.error(`${result.Message}`);
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}
async function GetDispatchToData(CategeryId, RecordRegimentId) {
    let param = new URLSearchParams({
        "CategeryId": CategeryId,
        "RecordRegimentId": RecordRegimentId
        });

    try {
        const response = await fetch('/BasicDetail/GetDispatchToData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            $("#txtUnitName").val(`${result.Value.Sus_no} ${result.Value.UnitAbbreviation}`);
            $("#spnUnitMapId").html(result.Value.UnitId);


            let listItemddl = "";
            listItemddl += '<option value="0">Select Domain Id</option>';
            listItemddl += `<option value="${result.Value.AspNetUsersId}">${result.Value.DomainId}</option>`;

            $("#ddlDID").html(listItemddl);
            $("#ddlDID").val(`${result.Value.AspNetUsersId}`);

            $("#txtArmyNo").val(`${result.Value.ArmyNo}`);
            $("#spnUserId").html(result.Value.UserId);

            $("#txtRkName").val(`${result.Value.RankAbbreviation} ${result.Value.Name}`);

        } else {
            Reset();
            toastr.error(`${result.Message}`);
        }

    } catch (error) {
        Reset();
        alert("Error: " + error.message);
    }
}
async function GetddlRecordRegiment(CategeryId) {
    let param = new URLSearchParams({ CategeryId: CategeryId });

    try {
        const response = await fetch('/BasicDetail/GetddlRecordRegiment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: param
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();

        if (Boolean(result.Result)) {
            let listItemddl = "";
            listItemddl += '<option value="0">Select Record Office / Regiment</option>';

            for (let i = 0; i < result.Value.length; i++) {
                listItemddl += `<option value="${result.Value[i].Id}">${result.Value[i].Name}</option>`;
            }

            document.getElementById("ddlRecordRegiment").innerHTML = listItemddl;

            $("#spnUnitMapId").html(0);
            $("#spnUserId").html(0);

            $("#txtUnitName").val(``);
            $("#txtArmyNo").val(``);
            $("#txtRkName").val(``);

            $('#ddlDID').empty();

        } else {
            toastr.error(`${result.Message}`);
            Reset();
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}
function Reset() {
    $("#spnUnitMapId").html(0);
    $("#spnUserId").html(0);

    $("#txtUnitName").val(``);
    $("#txtArmyNo").val(``);
    $("#txtRkName").val(``);
    //$('#ddlDID').empty();

    //$("#ddlRecordRegiment").find("option").not(":first").remove();
    //$("#ddlRecordRegiment").val("0");

    $("#ddlDID").find("option").not(":first").remove();
    $("#ddlDID").val("0");
}