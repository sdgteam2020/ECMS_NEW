$(document).ready(function () {


  


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
            $("#txtUnit").val(i.item.label);
            //alert(i.item.value)
            var param1 = { "UnitMapId": i.item.value };
            $.ajax({
                url: '/Master/GetALLByUnitMapId',
                contentType: 'application/x-www-form-urlencoded',
                data: param1,
                type: 'POST',
                success: function (data) {


                    $("#UnitId").val(data.UnitId);
                    //$("#lblProComd").html(data.ComdName);
                    //$("#lblProCorps").html(data.CorpsName);
                    //$("#lblProDiv").html(data.DivName);
                    //$("#lblPrBde").html(data.BdeName);
                    //$("#lblProSusno").html(data.Sus_no + '' + data.Suffix);

                }
            });
        },
        appendTo: '#suggesstion-box'
    });

});