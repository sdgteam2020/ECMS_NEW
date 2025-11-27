$(function () {
    var $success = $('#toast-success');
    if ($success.length) {
        var msg = $success.data('message');
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