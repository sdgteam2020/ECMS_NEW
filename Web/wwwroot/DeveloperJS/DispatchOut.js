$(async function () {
    mMsater(0, "ddlDispatch", DispatchMode, "");

    $('#ddlCategery').on('change',async function () {
        let CategeryId = $(this).val(); // Get the selected value
        await GetddlRecordRegiment(CategeryId);
    });
});
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