$(document).ready(function () {

    $("#btnAddUser").click(function () {
        $("#AddUserM").modal('show');
       
    });
    $("#btnsave").click(function () {
        CreateUser();

    });


});
function CreateUser() {

    /*  alert($('#bdaymonth').val());*/

    $.ajax({
        url: '/Account/Add',
        type: 'POST',
        data: { "Username": $("#txtUserName").val(), "FullName": $("#txtUserName").val(), "Password": $("#txtPassword").val(), "RankId": $("#ddlRank").val(), "Unit_ID": $("#txtUnitId").val(), "UserTypeId": $("#UserTypeId").val(), "ASCON": "13","Active":"1" }, //get the search string
        success: function (result) {


            if (result == 1) {


              

            }
            else if (result == 4) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })
            }


        }
    });
}