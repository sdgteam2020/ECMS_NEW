var skey = "";
$(function () {
    sessionStorage.clear();
    skey = $('#spnhdns').html();
    globalThis.RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
});
$("body").on("click", ".cls-btnRetry", function () {
    Swal.fire({
        title: "Are you sure?",
        text: "You want to Retry!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Retry it!"
    }).then((result) => {
        if (result.isConfirmed) {

            //var encryptedArmyNo = encryptData($(this).closest("td").find("#ArmyNo").html(), skey);
            //var encryptedOffType = encryptData($(this).closest("td").find("#OffType").html(), skey);
            //var encryptedRegistrationApplyFor = encryptData($(this).closest("td").find("#RegistrationApplyFor").html(), skey);
            //var encryptedlCardType = encryptData($(this).closest("td").find("#lCardType").html(), skey);


            //sessionStorage.setItem("OffType", encryptedOffType);
            //sessionStorage.setItem("RegistrationApplyFor", encryptedRegistrationApplyFor);
            //sessionStorage.setItem("lCardType", encryptedlCardType);
            //sessionStorage.setItem("ArmyNo", encryptedArmyNo);
            //window.location.href = "/BasicDetail/Registration?Id=MQ==";
        }
    });
});