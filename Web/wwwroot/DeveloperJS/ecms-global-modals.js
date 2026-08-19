/*
 * ECMS Global Modal Stack Manager
 * Supports Bootstrap stacked modals.
 */
(function () {
    'use strict';

    const MODAL_BASE_Z = 1060;
    const BACKDROP_BASE_Z = 1050;
    const STACK_STEP = 20;

    let modalStack = [];

    function normalizeModal(modal) {

        if (!(modal instanceof HTMLElement) ||
            !modal.classList.contains('modal')) {
            return;
        }

        modal.classList.add('ecms-global-modal-hosted');

        // Move modal directly under body.
        if (modal.parentElement !== document.body) {
            document.body.appendChild(modal);
        }
    }


    function addToStack(modal) {

        if (!modal) {
            return;
        }

        normalizeModal(modal);

        // Remove modal if already present.
        modalStack = modalStack.filter(function (item) {
            return item !== modal;
        });

        // Latest opened modal always goes last.
        modalStack.push(modal);

        syncModalStack();
    }


    function removeFromStack(modal) {

        modalStack = modalStack.filter(function (item) {
            return item !== modal;
        });

        syncModalStack();
    }


    function cleanModalStack() {

        modalStack = modalStack.filter(function (modal) {

            if (!document.body.contains(modal)) {
                return false;
            }

            return modal.classList.contains('show') ||
                modal.classList.contains('in');
        });
    }


    function syncModalStack() {

        cleanModalStack();

        const backdrops = Array.from(
            document.querySelectorAll(
                'body > .modal-backdrop'
            )
        );


        // ==========================================
        // MODALS
        // ==========================================

        modalStack.forEach(function (modal, index) {

            const modalZ =
                MODAL_BASE_Z + (index * STACK_STEP);

            modal.classList.add(
                'ecms-modal-stack-active'
            );

            modal.style.setProperty(
                'z-index',
                String(modalZ),
                'important'
            );
        });


        // ==========================================
        // BACKDROPS
        // ==========================================

        backdrops.forEach(function (backdrop, index) {

            const backdropZ =
                BACKDROP_BASE_Z +
                (index * STACK_STEP);

            backdrop.style.setProperty(
                'z-index',
                String(backdropZ),
                'important'
            );
        });


        // ==========================================
        // BODY
        // ==========================================

        if (modalStack.length > 0) {

            document.body.classList.add(
                'modal-open'
            );

        } else {

            document.body.classList.remove(
                'modal-open'
            );

            window.setTimeout(function () {

                if (modalStack.length === 0) {

                    document
                        .querySelectorAll(
                            'body > .modal-backdrop'
                        )
                        .forEach(function (backdrop) {
                            backdrop.remove();
                        });
                }

            }, 200);
        }
    }


    // ==============================================
    // BOOTSTRAP EVENTS
    // ==============================================

    document.addEventListener(
        'show.bs.modal',
        function (event) {

            const modal =
                event.target.closest('.modal');

            if (modal) {
                addToStack(modal);
            }

        },
        true
    );


    document.addEventListener(
        'shown.bs.modal',
        function (event) {

            const modal =
                event.target.closest('.modal');

            if (modal) {
                addToStack(modal);
            }

            window.setTimeout(
                syncModalStack,
                0
            );

        },
        true
    );


    document.addEventListener(
        'hidden.bs.modal',
        function (event) {

            const modal =
                event.target.closest('.modal');

            if (modal) {

                modal.style.removeProperty(
                    'z-index'
                );

                modal.classList.remove(
                    'ecms-modal-stack-active'
                );

                removeFromStack(modal);
            }


            // Important:
            // Bootstrap removes modal-open when second
            // modal closes. Add it again when first
            // modal is still open.

            if (modalStack.length > 0) {

                document.body.classList.add(
                    'modal-open'
                );
            }

        },
        true
    );


    // ==============================================
    // DYNAMIC MODALS
    // ==============================================

    const observer =
        new MutationObserver(function (mutations) {

            let needSync = false;

            mutations.forEach(function (mutation) {

                mutation.addedNodes.forEach(
                    function (node) {

                        if (!(node instanceof HTMLElement)) {
                            return;
                        }

                        if (node.classList.contains('modal')) {
                            normalizeModal(node);
                        }

                        if (
                            node.classList.contains(
                                'modal-backdrop'
                            )
                        ) {
                            needSync = true;
                        }
                    }
                );
            });

            if (needSync) {

                window.requestAnimationFrame(
                    syncModalStack
                );
            }
        });


    function initialize() {

        document
            .querySelectorAll('.modal')
            .forEach(function (modal) {
                normalizeModal(modal);
            });

        observer.observe(
            document.body,
            {
                childList: true,
                subtree: true
            }
        );
    }


    if (document.readyState === 'loading') {

        document.addEventListener(
            'DOMContentLoaded',
            initialize,
            {
                once: true
            }
        );

    } else {

        initialize();
    }

})();
