$(document).ready(function () {

    $("#AppointmentName").autocomplete({
        source: function (request, response) {
            if (request.term.length > 1) {
                $("#spnApptIdMap").html('');
                var param = { "AppointmentName": request.term };
                $("#spnApptIdMap").html(0);
                $.ajax({
                    url: '/Master/GetALLByAppointmentName',
                    contentType: 'application/x-www-form-urlencoded',
                    data: param,
                    type: 'POST',
                    success: function (data) {
                        console.log(data);
                        response($.map(data, function (item) {

                            $("#loading").addClass("d-none");
                            return { label: item.AppointmentName, value: item.ApptId };

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
            $("#AppointmentName").val(i.item.label);
            $("#spnApptIdMap").html(i.item.value);
            $("#ApptId").val(i.item.value);
            //alert(i.item.value)
         },
        appendTo: '#suggesstion-box'
    });

    $("#UnitName").autocomplete({


        source: function (request, response) {
            if (request.term.length > 2) {
                $("#spnUnitIdMap").html('');
                $("#lblProComd").html('');
                $("#lblProCorps").html('');
                $("#lblProDiv").html('');
                $("#lblPrBde").html('');
                $("#lblProSusno").html('');
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
                    $("#UnitId").val(data.UnitMapId);
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