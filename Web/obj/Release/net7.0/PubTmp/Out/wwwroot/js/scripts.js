/*!
    * Start Bootstrap - SB Admin Pro v1.2.0 (https://shop.startbootstrap.com/product/sb-admin-pro)
    * Copyright 2013-2020 Start Bootstrap
    * Licensed under SEE_LICENSE (https://github.com/StartBootstrap/sb-admin-pro/blob/master/LICENSE)
    */
(function ($) {
    "use strict";

  

    // Add active state to sidbar nav links
    var path = window.location.href; // because the 'href' property of the DOM element is the absolute path
    $("#layoutSidenav_nav .sidenav a.nav-link").each(function () {
        if (this.href === path) {
            $(this).addClass("active");
        }
    });
   
    $("#layoutPerinfo_nav ul li a.nav-link").each(function () {
        if (this.href === path) {
            $(this).addClass("active");
            $("#PersInfo").addClass("active");
        }
    });
    // Toggle the side navigation
   

    if ($(window).width() < 992) {
        $("#sidebarToggle").on("click", function (e) {
            e.preventDefault();
            $("body").toggleClass("sidenav-toggled");
            if ($("body").hasClass("rightSide-toggled")) {
                $("body").removeClass("rightSide-toggled");
            }
        });
        $("#rightSidebarToggle").click(function (e) {
            e.preventDefault();
            $("body").toggleClass("rightSide-toggled");
            if ($("body").hasClass("sidenav-toggled")) {
                $("body").removeClass("sidenav-toggled");
            }
        });
    } else {
        $("#sidebarToggle").on("click", function (e) {
            e.preventDefault();
            $("body").toggleClass("sidenav-toggled");
        });
        $("#rightSidebarToggle").click(function (e) {
            e.preventDefault();
            $("body").toggleClass("rightSide-toggled");
        });
    };

    // Activate Feather icons
    feather.replace();

   

    // Scrolls to an offset anchor when a sticky nav link is clicked
    $('.nav-sticky a.nav-link[href*="#"]:not([href="#"])').click(function () {
        if (
            location.pathname.replace(/^\//, "") ==
            this.pathname.replace(/^\//, "") &&
            location.hostname == this.hostname
        ) {
            var target = $(this.hash);
            target = target.length ? target : $("[name=" + this.hash.slice(1) + "]");
            if (target.length) {
                $("html, body").animate(
                    {
                        scrollTop: target.offset().top - 81
                    },
                    200
                );
                return false;
            }
        }
    });

    // Click to collapse responsive sidebar

    $("#layoutSidenav_content").click(function () {
        const BOOTSTRAP_LG_WIDTH = 992;
        if (window.innerWidth >= 992) {
            return;
        }
        if ($("body").hasClass("sidenav-toggled")) {
            $("body").toggleClass("sidenav-toggled");
        }
    });
    $("#layoutSidenav_content").click(function () {
        if ($("body").hasClass("rightSide-toggled")) {
            $("body").toggleClass("rightSide-toggled");
        }
    });

    // Init sidebar
    let activatedPath = window.location.pathname.match(/([\w-]+\.html)/, '$1');

    if (activatedPath) {
        activatedPath = activatedPath[0];
    }
    else {
        activatedPath = '';
    }

    let targetAnchor = $('[href="' + activatedPath + '"]');
    let collapseAncestors = targetAnchor.parents('.collapse');

    targetAnchor.addClass('active');

    collapseAncestors.each(function () {
        $(this).addClass('show');
        $('[data-target="#' + this.id + '"]').removeClass('collapsed');

    });




        ; (function (document, window, index) {
            var inputs = document.querySelectorAll('.inputfile');
            Array.prototype.forEach.call(inputs, function (input) {
                var label = input.nextElementSibling,
                    labelVal = label.innerHTML;

                input.addEventListener('change', function (e) {
                    var fileName = '';
                    if (this.files && this.files.length > 1)
                        fileName = (this.getAttribute('data-multiple-caption') || '').replace('{count}', this.files.length);
                    else
                        fileName = e.target.value.split('\\').pop();

                    if (fileName) {
                        label.querySelector('span').innerHTML = fileName;
                        label.querySelector('span').title = fileName;
                    }
                    else
                        label.innerHTML = labelVal;
                });

                // Firefox bug fix
                input.addEventListener('focus', function () { input.classList.add('has-focus'); });
                input.addEventListener('blur', function () { input.classList.remove('has-focus'); });
            });
        }(document, window, 0));

})(jQuery);
