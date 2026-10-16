$(function () {
    var $success1 = $('#toast-success');
    if ($success1.length) {
        var msg = $success1.data('message');
        if (msg) {
            toastr.success(msg);
        }
    }

    var $error = $('#toast-error');
    if ($error.length) {
        var msg = $error.data('message');
        if (msg) {
            toastr.error(msg);
        }
    }
});