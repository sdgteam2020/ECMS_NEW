$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    GetDashboardCount();

    document.getElementById("btnMisprintedCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/FaultyCard';
    });

    document.getElementById("btnHotlistCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/HotlistCard';
    });

    document.getElementById("btnDistCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/DistributeCard';
    });

    document.getElementById("btnDestCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/DestructionCard';
    });

    document.getElementById("btnUnitChangeRequest").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/Master/MapUnitChange';
    });
    document.getElementById("btnAddUnitChangeRequest").addEventListener("click", function (event) {
        event.stopPropagation();
        location.href = '/Master/MapUnitChangeRequest';
    });
    document.getElementById("btnDispatchCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/DispatchCard';
    });

    document.getElementById("btnLostCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/LostCard';
    });
    var btnAdd = document.getElementById('btnAddDispatchCard');
    if (btnAdd) {
        document.getElementById("btnAddDispatchCard").addEventListener("click", function (event) {
            event.stopPropagation();
            location.href = '/BasicDetail/DispatchOut';
        });
    }

    $('.btnAdd').on('click', function (event) {
        const type = $(this).data('type');
        event.stopPropagation();
        $("#armynosearchAllName").html("");
        $("#txtarmynosearchAll").val("");
        $("#armynosearchAllpic").attr("src", "");
        $("#unitoffrsModal").modal("show");
        $("#armynosearchTypeId").val(type);
    });

})
function GetDashboardCount() {
    var userdata =
    {
        "Id": 0,

    };
    $.ajax({
        url: '/Home/GetTaskBoardCount',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {TotDispatchCards
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {

                }

                else {

                    $("#TotUnitChangeRequest").html(response.TotUnitChangeRequest);
                    $("#TotMisprintedCard").html(response.TotMisprintedCard);
                    $("#TotHotlistCards").html(response.TotHotlistCards);
                    $("#TotDestCards").html(response.TotDestCards);
                    $("#TotDistCards").html(response.TotDistCards);
                    $("#TotDispatchCards").html(response.TotDispatchCards);
                    $("#TotLostCards").html(response.TotLostCards);

                    $('.counter-value').each(function () {
                        $(this).prop('Counter', 0).animate({
                            Counter: $(this).text()
                        }, {
                            duration: 200,
                            easing: 'swing',
                            step: function (now) {
                                $(this).text(Math.ceil(now));
                            }
                        });
                    });
                }
            }
            else {

            }
        },
        error: function (result) {
            Swal.fire({
                text: errormsg002
            });
        }
    });
}