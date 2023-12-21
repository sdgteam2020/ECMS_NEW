$(document).ready(function () {


    getunitbymapid($("#spnUnitIdid").val())

    if ($("#ApplyForId").val() == 1) {
        $(".OptionsRegimental").addClass("d-none");
        mMsater($("#spnrankid").val(), "RankId", Rank, "");
    } else if ($("#ApplyForId").val() == 2) {
        mMsater($("#spnrankid").val(), "RankId", RankJCo, "");
        $(".OptionsRegimental").removeClass("d-none");
    }

    if (sessionStorage.getItem("ArmyNo") != null) {
        $("#ServiceNumber").val(sessionStorage.getItem("ArmyNo"));

        if (sessionStorage.getItem("OffType") == 1) {
            $(".OptionsRegimental").addClass("d-none");
            mMsater($("#spnrankid").val(), "RankId", Rank, "");
        }
        else if (sessionStorage.getItem("OffType") == 2) {
            {
                mMsater($("#spnrankid").val(), "RankId", RankJCo, "");
                $(".OptionsRegimental").removeClass("d-none");
            }
        }
        if (sessionStorage.getItem("OffType") != "")
            $("#ApplyForId").val(sessionStorage.getItem("OffType"));
        $("#Type").val(sessionStorage.getItem("OffType"));

        if (sessionStorage.getItem("lCardType") != "")
            $("#TypeId").val(sessionStorage.getItem("lCardType"));

    }


    $("#txtUnit").autocomplete({
        source: function (request, response) {
            if (request.term.length > 2) {
                var param = { "UnitName": request.term };
                $("#UnitId").html(0);
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
           /* $("#txtUnit").val(i.item.label);*/
            //alert(i.item.value)
            getunitbymapid(i.item.value);
        },
        appendTo: '#suggesstion-box'
    });

});
function getunitbymapid(value)
{
    var param1 = { "UnitMapId": value };
    $.ajax({
        url: '/Master/GetALLByUnitMapId',
        contentType: 'application/x-www-form-urlencoded',
        data: param1,
        type: 'POST',
        success: function (data) {

            $("#txtUnit").val(data.UnitName);
            $("#UnitId").val(data.UnitId);
            //$("#lblProComd").html(data.ComdName);
            //$("#lblProCorps").html(data.CorpsName);
            //$("#lblProDiv").html(data.DivName);
            //$("#lblPrBde").html(data.BdeName);
            //$("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

        }
    });
}