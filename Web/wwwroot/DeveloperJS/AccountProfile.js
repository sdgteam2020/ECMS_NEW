$(document).ready(function () {
    mMsater(0, "FormationId", Formation, "");
    mMsater(0, "RankId", Rank, "");

    $('#FormationId').on('change', function () {
        mMsater(0, "ApptId", Appt, $('#FormationId').val());

    });

    //$("#btnConfigsave").click(function () {
    //    if ($("#txtArmyNo").val() != "" && $("#spnUnitIdMap").html() != "0")
    //    {
    //       SaveMapping();
    //    }
    //    else
    //        toastr.error('Offrs Army/Unit  No Not Blank!'); 
    //});
    $("#UnitName").autocomplete({


        source: function (request, response) {
            if (request.term.length > 2) {
                var param = { "UnitName": request.term };
                $("#spnUnitIdMap").html(0);
                $.ajax({
                    url: '/Master/GetALLByUnitName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.UnitName, value: item.UnitMapId };

                        }))
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
            $("#UnitName").val(i.item.label);
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {


                    $("#spnUnitIdMap").html(data.UnitMapId);
                    $("#lblProComd").html(data.ComdName);
                    $("#lblProCorps").html(data.CorpsName);
                    $("#lblProDiv").html(data.DivName);
                    $("#lblPrBde").html(data.BdeName);
                    $("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

                }
            });
        },
        appendTo: '#suggesstion-box'
    });
});




