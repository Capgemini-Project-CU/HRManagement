(function () {
    'use strict';

    document.addEventListener('show.bs.modal', function (e) {
        e.target.querySelectorAll('.js-edit-error').forEach(function (el) {
            el.remove();
        });
    });

    document.addEventListener('submit', async function (e) {
        var form = e.target;
        if (!form.classList.contains('js-edit-form')) return;

        e.preventDefault();

        // Remove any prior inline error banner in this form
        form.querySelectorAll('.js-edit-error').forEach(function (el) { el.remove(); });

        // Show loading state on the submit button
        var btn = form.querySelector('[type="submit"]');
        var originalLabel = btn.textContent;
        btn.disabled = true;
        btn.textContent = 'Updating…';

        var errorMessage = null;

        try {
            var response = await fetch(form.action, {
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                body: new FormData(form)
            });

            var data = {};
            try { data = await response.json(); } catch (_) { /* plain-text body */ }

            if (response.ok && data.redirectUrl) {
                // Success — navigate to the list page (button stays disabled)
                window.location.href = data.redirectUrl;
                return;
            }

            errorMessage = data.error || 'Update failed. Please try again.';

        } catch (_) {
            errorMessage = 'Network error. Please check your connection and try again.';
        }

        // Inject the error banner into the first .modal-body (the error container)
        var banner = document.createElement('div');
        banner.className = 'alert alert-danger border-0 mb-0 js-edit-error';
        banner.textContent = errorMessage;
        form.querySelector('.modal-body').prepend(banner);

        // Restore button so user can fix and retry
        btn.disabled = false;
        btn.textContent = originalLabel;
    });

}());
