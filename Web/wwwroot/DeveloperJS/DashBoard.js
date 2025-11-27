$(function () {
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
    GetDashboardCount();

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
    $.ajax({
        url: '/Home/GetDashboardCount',
        contentType: 'application/x-www-form-urlencoded',
        type: 'POST',
        headers: { 'RequestVerificationToken': globalThis.RequestVerificationToken },

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