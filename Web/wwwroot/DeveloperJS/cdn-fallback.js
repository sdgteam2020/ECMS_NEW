(function () {
    console.log("cdn-fallback loaded");

    window.addEventListener("error", function (event) {
        var target = event.target;
        if (!target) return;

        // CSS fallback
        if (
            target.tagName === "LINK" &&
            target.rel === "stylesheet" &&
            target.getAttribute("data-local")
        ) {
            var localCss = target.getAttribute("data-local");

            if (target.getAttribute("data-fallback-done") === "true") return;

            target.setAttribute("data-fallback-done", "true");
            target.removeAttribute("data-local");

            target.href = localCss;

            console.warn("CSS fallback loaded:", localCss);
        }

        // JS fallback
        if (
            target.tagName === "SCRIPT" &&
            target.getAttribute("data-local")
        ) {
            var localScript = target.getAttribute("data-local");

            if (target.getAttribute("data-fallback-done") === "true") return;

            target.setAttribute("data-fallback-done", "true");

            var script = document.createElement("script");
            script.src = localScript;
            document.body.appendChild(script);

            console.warn("JS fallback loaded:", localScript);
        }

        // IMAGE fallback
        if (
            target.tagName === "IMG" &&
            target.getAttribute("data-local")
        ) {
            var localImg = target.getAttribute("data-local");

            if (target.getAttribute("data-fallback-done") === "true") return;

            target.setAttribute("data-fallback-done", "true");
            target.removeAttribute("data-local");

            target.src = localImg;

            console.warn("Image fallback loaded:", localImg);
        }

        // FAVICON fallback
        if (
            target.tagName === "LINK" &&
            target.rel &&
            target.rel.toLowerCase().includes("icon") &&
            target.getAttribute("data-local")
        ) {
            var localIcon = target.getAttribute("data-local");

            if (target.getAttribute("data-fallback-done") === "true") return;

            target.setAttribute("data-fallback-done", "true");
            target.removeAttribute("data-local");

            target.href = localIcon;

            console.warn("Icon fallback loaded:", localIcon);
        }

    }, true);
})();