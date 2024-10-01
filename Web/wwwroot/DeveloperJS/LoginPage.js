$(function () {
    $("#LClick").on("click", function () {
        alert("Hello");
    });
    $("#LoginSubmit").on("click", function () {
        let formId = '#login';
        $.validator.unobtrusive.parse($(formId));

        if ($(formId).valid()) {
            SubmitsEncry();
            setTimeout(function () {
                Submit();
            }, 500);
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Please fill required field.',
            })
            toastr.error('Please fill required field.');
            return false;
        }
    });
})
function Submit() {
    $.ajax({
        url: '/Account/Login',
        type: 'POST',
        data: {
            "UserName": $("#UserName").val(),
            "Password": $("#Password").val(),
            "Role": $("#Role").val(),
            "hdns": $("#hdns").val(),
        }, //get the search string
        success: function (result) {


            if (result == DataSave) {
                toastr.success('Profile has been saved');

                $("#AddNewProfile").modal('hide');
                ProfileCount();
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataUpdate) {
                toastr.success('Profile has been Updated');

                $("#AddNewProfile").modal('hide');
                BindData();
                Reset();
                ResetErrorMessage();
            }
            else if (result == DataExists) {

                toastr.error('Army No. Exits!');

            }
            else if (result == InternalServerError) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong or Invalid Entry!',

                })

            } else {
                if (result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        toastr.error(result[i][0].ErrorMessage)
                    }


                }


            }
        }
    });
}