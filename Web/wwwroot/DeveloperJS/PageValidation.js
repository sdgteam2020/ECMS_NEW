$(document).ready(function () {



    (function (window) {
        function preventBack() {
            window.history.forward();
        }

        preventBack();
        window.onload = preventBack;
        window.onpageshow = function (evt) {
            if (evt.persisted) preventBack();
        };

        window.onunload = function () {
            return null;
        };
    })(window);



});
