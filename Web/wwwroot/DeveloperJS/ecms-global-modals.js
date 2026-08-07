/*
 * ECMS global modal host/stack manager.
 * UI-only behavior: keeps Bootstrap modals directly under <body>, prevents
 * backdrops from covering dialogs, and supports stacked modals safely.
 */
(function () {
    'use strict';

    const MODAL_BASE_Z = 1060;
    const BACKDROP_BASE_Z = 1050;
    const STACK_STEP = 20;

    function normalizeModal(modal) {
        if (!(modal instanceof HTMLElement) || !modal.classList.contains('modal')) {
            return;
        }

        modal.classList.add('ecms-global-modal-hosted');

        // A modal inside a transformed/scrolled page container can appear below
        // its backdrop. Hosting it directly under body removes that stacking issue.
        if (modal.parentElement !== document.body) {
            document.body.appendChild(modal);
        }
    }

    function normalizeAllModals(root) {
        if (!root) return;

        if (root instanceof HTMLElement && root.classList.contains('modal')) {
            normalizeModal(root);
        }

        if (root.querySelectorAll) {
            root.querySelectorAll('.modal').forEach(normalizeModal);
        }
    }

    function getOpenModals() {
        return Array.from(document.querySelectorAll('body > .modal.show, body > .modal.in'))
            .filter(function (modal) {
                return getComputedStyle(modal).display !== 'none';
            });
    }

    function syncModalStack() {
        const openModals = getOpenModals();
        const backdrops = Array.from(document.querySelectorAll('body > .modal-backdrop'));

        openModals.forEach(function (modal, index) {
            modal.classList.add('ecms-modal-stack-active');
            modal.style.setProperty('z-index', String(MODAL_BASE_Z + (index * STACK_STEP)), 'important');
        });

        document.querySelectorAll('body > .modal:not(.show):not(.in)').forEach(function (modal) {
            modal.classList.remove('ecms-modal-stack-active');
            modal.style.removeProperty('z-index');
        });

        backdrops.forEach(function (backdrop, index) {
            // Match the newest backdrop to the newest modal and always keep it below.
            const modalIndex = Math.min(index, Math.max(openModals.length - 1, 0));
            backdrop.style.setProperty('z-index', String(BACKDROP_BASE_Z + (modalIndex * STACK_STEP)), 'important');
        });

        if (openModals.length > 0) {
            document.body.classList.add('modal-open');
        } else {
            document.body.classList.remove('modal-open');

            // Remove only stale backdrops after Bootstrap has completed hiding.
            window.setTimeout(function () {
                if (getOpenModals().length === 0) {
                    document.querySelectorAll('body > .modal-backdrop').forEach(function (backdrop) {
                        backdrop.remove();
                    });
                }
            }, 180);
        }
    }

    function onModalEvent(event) {
        const modal = event && event.target && event.target.closest
            ? event.target.closest('.modal')
            : null;

        if (modal) {
            normalizeModal(modal);
        }

        window.setTimeout(syncModalStack, 0);
    }

    function initialize() {
        normalizeAllModals(document);
        syncModalStack();

        // Bootstrap 5/native custom events.
        ['show.bs.modal', 'shown.bs.modal', 'hide.bs.modal', 'hidden.bs.modal']
            .forEach(function (eventName) {
                document.addEventListener(eventName, onModalEvent, true);
            });

        // Bootstrap 4/jQuery event bridge. Safe when jQuery is present and ignored otherwise.
        if (window.jQuery) {
            window.jQuery(document)
                .off('.ecmsGlobalModals')
                .on('show.bs.modal.ecmsGlobalModals shown.bs.modal.ecmsGlobalModals hide.bs.modal.ecmsGlobalModals hidden.bs.modal.ecmsGlobalModals', '.modal', function () {
                    normalizeModal(this);
                    window.setTimeout(syncModalStack, 0);
                });
        }

        // Handle modals that a page injects dynamically after initial render.
        const observer = new MutationObserver(function (mutations) {
            let stackMayHaveChanged = false;

            mutations.forEach(function (mutation) {
                mutation.addedNodes.forEach(function (node) {
                    if (!(node instanceof HTMLElement)) return;

                    if (node.classList.contains('modal') || node.querySelector('.modal')) {
                        normalizeAllModals(node);
                        stackMayHaveChanged = true;
                    }

                    if (node.classList.contains('modal-backdrop') || node.querySelector('.modal-backdrop')) {
                        stackMayHaveChanged = true;
                    }
                });
            });

            if (stackMayHaveChanged) {
                window.requestAnimationFrame(syncModalStack);
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize, { once: true });
    } else {
        initialize();
    }
})();
