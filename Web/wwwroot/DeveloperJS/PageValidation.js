$(document).ready(function () {


    document.addEventListener('DOMContentLoaded', function () {
        // Push a new state to the browser history
        history.pushState(null, null, location.href);

        // Listen for the popstate event
        window.addEventListener('popstate', function (event) {
            // Prevent the back button action by pushing the state again
            history.pushState(null, null, location.href);
            alert('Back navigation is disabled!');
        });
    });


});
