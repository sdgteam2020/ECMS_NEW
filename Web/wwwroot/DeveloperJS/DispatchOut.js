$(async function () {
    mMsater(0, "ddlDispatch", DispatchMode, "");
    let ClaimValue = $("#spnClaimValue").html();
    if (ClaimValue == 1) {
        $("#DispatchOut_Categery").removeClass("d-none");
        $('#ddlCategery').on('change', async function () {
            let CategeryId = $(this).val(); // Get the selected value
            await GetddlRecordRegiment(CategeryId);
        });
        $('#ddlRecordRegiment').on('change', async function () {
            let RecordRegimentId = $(this).val(); // Get the selected value
            await GetDispatchToData($('#ddlCategery').val(),RecordRegimentId);
        });
    }
    else if (ClaimValue == 2) {
        $("#DispatchOut_Categery").addClass("d-none");
        $("#ddlCategery").val(1); // Set default value for Categery
        await GetddlRecordRegiment(1);
    }
    else if (ClaimValue == 3) {
        $("#DispatchOut_Categery").addClass("d-none");
        $("#ddlCategery").val(2); // Set default value for Categery
        await GetddlRecordRegiment(2);
    }

});
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

        if (result.Result = true) {
            console.log(result.Value.Sus_no);
            $("#txtUnitName").val(`${result.Value.Sus_no} ${result.Value.UnitAbbreviation}`);
            $("#spnUnitMapId").html(result.Value.UnitId);

            let listItemddl = "";
            listItemddl += '<option value="">Please Select</option>';
            listItemddl += `<option value="${result.Value[i].Id}">${result.Value[i].Name}</option>`;

        } else {
            toastr.error(`${result.Message}`);
        }

    } catch (error) {
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

        if (result.Result = true) {
            let listItemddl = "";
            listItemddl += '<option value="">Please Select</option>';

            for (let i = 0; i < result.Value.length; i++) {
                listItemddl += `<option value="${result.Value[i].Id}">${result.Value[i].Name}</option>`;
            }

            document.getElementById("ddlRecordRegiment").innerHTML = listItemddl;

        } else {
            toastr.error('Invalid Input.');
        }

    } catch (error) {
        alert("Error: " + error.message);
    }
}