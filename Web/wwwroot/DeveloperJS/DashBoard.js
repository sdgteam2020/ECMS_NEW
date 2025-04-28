$(function () {
    GetDashboardCount();

    //$("#btnMisprintedCard").on("click", function () {
    //    location.href = '/BasicDetail/FaultyCard';
    //});
    //$("#btnAdd").on("click", function () {
    //    $("#armynosearchAllName").html("");
    //    $("#txtarmynosearchAll").val("");
    //    $("#armynosearchAllpic").attr("src", "");
    //    $("#unitoffrsModal").modal("show");
    //    $("#armynosearchTypeId").val(FaultyCardRequest);
    //});
    document.getElementById("btnMisprintedCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/FaultyCard';
    });

    document.getElementById("btnHotlistCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/HotlistCard';
    });

    document.getElementById("btnLostCard").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent anchor default behavior
        location.href = '/BasicDetail/LostCard';
    });

    //document.getElementsByClassName("btnAdd").addEventListener("click", function (event) {
    //    event.stopPropagation(); // Prevent click from bubbling to the <a>
        
    //});

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
        url: '/Home/GetDashboardCount',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == InternalServerError) {
                    Swal.fire({
                        text: errormsg
                    });
                }
                else if (response == 0) {

                }

                else {
                   
                    $("#TotReq").html(response.TotReq);
                    $("#TotInaccurateData").html(response.TotInaccurateData);
                    $("#TotObservationRaised").html(response.TotObservationRaised);
                    $("#TotLostCards").html(response.TotLostCards);
                    $("#TotHotlistCards").html(response.TotHotlistCards);
                      
                   
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