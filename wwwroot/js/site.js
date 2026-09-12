// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    displayToast();
    initCurrencyInputs();
    initCountInputs();
});

function displayToast() {
    var toastEl = document.getElementById("liveToast");
    if (toastEl) {
        var toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
}

// Builds and shows a toast the same way _ToastNotification.cshtml does, for
// results that come back from a fetch() call instead of a full page load.
function showAjaxToast(title, message, success) {
    var container = document.querySelector(".toast-container");
    if (!container) {
        container = document.createElement("div");
        container.className = "toast-container position-fixed top-0 end-0 p-3";
        container.style.zIndex = 1100;
        document.body.appendChild(container);
    }

    var toastEl = document.createElement("div");
    toastEl.className = "toast";
    toastEl.setAttribute("role", "alert");
    toastEl.setAttribute("aria-live", "assertive");
    toastEl.setAttribute("aria-atomic", "true");
    toastEl.innerHTML =
        '<div class="toast-header ' + (success ? "bg-success text-white" : "bg-danger text-white") + '">' +
        '<strong class="me-auto"></strong>' +
        '<button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>' +
        "</div>" +
        '<div class="toast-body text-dark"></div>';
    toastEl.querySelector("strong").textContent = title;
    toastEl.querySelector(".toast-body").textContent = message;
    container.appendChild(toastEl);

    var toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 4000 });
    toastEl.addEventListener("hidden.bs.toast", function () {
        toastEl.remove();
    });
    toast.show();
}

// Posts a form via fetch and branches on the response's content-type: a JSON
// response (success, or a failure that only needs a toast) goes to onJson; an
// HTML response (a re-rendered partial showing validation errors) goes to onHtml.
function postFormViaFetch(form, onJson, onHtml) {
    var formData = new FormData(form);
    return fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
    }).then(function (r) {
        var contentType = r.headers.get('content-type') || '';
        if (contentType.indexOf('application/json') !== -1) {
            return r.json().then(onJson);
        }
        return r.text().then(onHtml);
    });
}

function formatCurrencyInput(input) {
    const val = parseFloat(input.value);
    input.value = isNaN(val) ? '0.00' : val.toFixed(2);
}

function initCurrencyInputs(container) {
    (container || document).querySelectorAll('input[inputmode="decimal"]')
        .forEach(formatCurrencyInput);
}

function formatCountInput(input) {
    const val = parseInt(input.value, 10);
    input.value = isNaN(val) ? '0' : String(val);
}

function initCountInputs(container) {
    (container || document).querySelectorAll('input[type="number"]')
        .forEach(formatCountInput);
}

// Re-format on blur via event delegation — covers dynamically added inputs too.
document.addEventListener('focusout', function (e) {
    if (e.target.matches('input[inputmode="decimal"]')) {
        formatCurrencyInput(e.target);
    }
    if (e.target.matches('input[type="number"]')) {
        formatCountInput(e.target);
    }
});

// Invite notification bell — load pending invites on dropdown open
(function () {
    var bell = document.getElementById('inviteBellToggle');
    if (!bell) return;

    // Invite emails link here with ?openInvites=1 so the recipient lands
    // straight on their pending invites instead of a camp they can't open yet.
    if (new URLSearchParams(window.location.search).get('openInvites') === '1') {
        bootstrap.Dropdown.getOrCreateInstance(bell).show();
    }

    bell.addEventListener('show.bs.dropdown', function () {
        var url = bell.getAttribute('data-invite-url');
        var container = document.getElementById('inviteListContainer');
        var badge = document.getElementById('inviteBadge');

        fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { return r.json(); })
            .then(function (data) {
                if (data.count > 0) {
                    badge.textContent = data.count;
                    badge.classList.remove('d-none');
                } else {
                    badge.textContent = '';
                    badge.classList.add('d-none');
                }

                if (data.invites.length === 0) {
                    container.innerHTML = '<span class="dropdown-item text-muted">Keine ausstehenden Einladungen.</span>';
                    return;
                }

                var token = '';
                var meta = document.querySelector('meta[name="RequestVerificationToken"]');
                if (meta) token = meta.getAttribute('content');

                var html = '';
                data.invites.forEach(function (inv) {
                    html += '<li class="px-3 py-2 border-bottom">';
                    html += '<div class="small fw-semibold">' + escapeHtml(inv.invitedBy) + ' lädt dich ein</div>';
                    html += '<div class="small text-muted mb-2">Lager: ' + escapeHtml(inv.budgetName) + '</div>';
                    html += '<form method="post" action="/Invite/Accept" class="d-inline">';
                    html += '<input type="hidden" name="__RequestVerificationToken" value="' + escapeHtml(token) + '">';
                    html += '<input type="hidden" name="id" value="' + inv.id + '">';
                    html += '<button type="submit" class="btn btn-sm btn-success me-1">Annehmen</button>';
                    html += '</form>';
                    html += '<form method="post" action="/Invite/Decline" class="d-inline">';
                    html += '<input type="hidden" name="__RequestVerificationToken" value="' + escapeHtml(token) + '">';
                    html += '<input type="hidden" name="id" value="' + inv.id + '">';
                    html += '<button type="submit" class="btn btn-sm btn-outline-secondary">Ablehnen</button>';
                    html += '</form>';
                    html += '</li>';
                });
                container.innerHTML = html;
            })
            .catch(function () {
                container.innerHTML = '<span class="dropdown-item text-danger small">Fehler beim Laden.</span>';
            });
    });

    function escapeHtml(text) {
        if (text == null) return '';
        var d = document.createElement('div');
        d.appendChild(document.createTextNode(text));
        return d.innerHTML;
    }
}());

// Create-transaction modal — load the form async into a modal (from Budget
// Index or Budget Detail), submit it via fetch so a server-side validation
// failure re-shows the modal with the entered values instead of navigating
// to a bare, unstyled fragment. On success, do a real navigation to the
// returned redirect URL so the toast shows the same way it does everywhere
// else (TempData survives the fetch response and is read on that load).
function openCreateTransactionModal(budgetId) {
    var container = document.getElementById('createTransactionModalContainer');
    fetch('/Transaction/Create?budgetId=' + budgetId)
        .then(function (r) { return r.text(); })
        .then(function (html) {
            container.innerHTML = html;
            wireCreateTransactionForm(container);
            new bootstrap.Modal(container.querySelector('.modal')).show();
        });
}

// Re-rendering a modal after a failed submit by wiping the container and
// creating a new bootstrap.Modal leaves the already-shown instance's backdrop
// orphaned in the DOM (a new one is added on top each time, and none of the
// earlier ones are ever removed, even once the modal is finally closed). Swap
// just the .modal-content instead, so the original instance/backdrop stays put.
function swapModalContent(container, html, rewire) {
    var temp = document.createElement('div');
    temp.innerHTML = html;
    var newContent = temp.querySelector('.modal-content');
    var currentContent = container.querySelector('.modal-content');
    if (newContent && currentContent) {
        currentContent.replaceWith(newContent);
        rewire(container);
    } else {
        container.innerHTML = html;
        rewire(container);
        new bootstrap.Modal(container.querySelector('.modal')).show();
    }
}

function wireCreateTransactionForm(container) {
    var form = container.querySelector('form');
    if (!form) return;

    var returnUrlInput = form.querySelector('[name="ReturnUrl"]');
    if (returnUrlInput) returnUrlInput.value = window.location.href;

    initCurrencyInputs(container);
    initCountInputs(container);
    if (window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive) {
        window.jQuery.validator.unobtrusive.parse(form);
    }

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        postFormViaFetch(
            form,
            function (data) {
                if (data.success) window.location.href = data.redirectUrl;
            },
            function (html) {
                swapModalContent(container, html, wireCreateTransactionForm);
            }
        ).catch(function () {
            showAjaxToast('Fehler', 'Ein Fehler ist aufgetreten.', false);
        });
    });
}

// Edit-transaction modal — same async-load-into-modal / fetch-submit approach
// as the create modal, plus its per-document delete forms (also fetch-based,
// so deleting a document doesn't navigate away from the modal either).
function openEditTransactionModal(id) {
    var container = document.getElementById('editTransactionModalContainer');
    fetch('/Transaction/Edit?id=' + id)
        .then(function (r) { return r.text(); })
        .then(function (html) {
            container.innerHTML = html;
            wireEditTransactionForm(container);
            new bootstrap.Modal(container.querySelector('.modal')).show();
        });
}

function wireEditTransactionForm(container) {
    var form = container.querySelector('#editTransactionForm');
    if (form) {
        var returnUrlInput = form.querySelector('[name="ReturnUrl"]');
        if (returnUrlInput) returnUrlInput.value = window.location.href;

        initCurrencyInputs(container);
        initCountInputs(container);
        if (window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive) {
            window.jQuery.validator.unobtrusive.parse(form);
        }

        form.addEventListener('submit', function (e) {
            e.preventDefault();
            postFormViaFetch(
                form,
                function (data) {
                    if (data.success) window.location.href = data.redirectUrl;
                },
                function (html) {
                    swapModalContent(container, html, wireEditTransactionForm);
                }
            ).catch(function () {
                showAjaxToast('Fehler', 'Ein Fehler ist aufgetreten.', false);
            });
        });
    }

    container.querySelectorAll('.delete-document-form').forEach(function (delForm) {
        delForm.addEventListener('submit', function (e) {
            e.preventDefault();
            if (!confirm('Beleg löschen?')) return;
            postFormViaFetch(
                delForm,
                function (data) {
                    showAjaxToast(data.title, data.message, data.success);
                    if (data.success) delForm.closest('li').remove();
                },
                function () {
                    showAjaxToast('Fehler', 'Ein Fehler ist aufgetreten.', false);
                }
            ).catch(function () {
                showAjaxToast('Fehler', 'Ein Fehler ist aufgetreten.', false);
            });
        });
    });
}

// Profile sidebar — load the edit form async when it's opened, submit its
// forms via fetch so saving doesn't navigate away from the sidebar.
(function () {
    var navLink = document.getElementById('profileNavLink');
    var offcanvasEl = document.getElementById('profileOffcanvas');
    if (!navLink || !offcanvasEl) return;

    var body = document.getElementById('profileOffcanvasBody');
    var url = navLink.getAttribute('href');

    offcanvasEl.addEventListener('show.bs.offcanvas', loadProfileForm);

    function loadProfileForm() {
        body.innerHTML = '<div class="text-muted">Wird geladen…</div>';
        fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { return r.text(); })
            .then(function (html) {
                body.innerHTML = html;
                bindProfileForms();
            })
            .catch(function () {
                body.innerHTML = '<div class="text-danger small">Fehler beim Laden.</div>';
            });
    }

    function bindProfileForms() {
        var editForm = body.querySelector('#profileEditForm');
        if (editForm) {
            editForm.addEventListener('submit', function (e) {
                e.preventDefault();
                submitViaFetch(editForm);
            });
        }

        var resendForm = body.querySelector('#resendConfirmationForm');
        if (resendForm) {
            resendForm.addEventListener('submit', function (e) {
                e.preventDefault();
                submitViaFetch(resendForm);
            });
        }

        initCurrencyInputs(body);
        initCountInputs(body);
    }

    // Posts a form via fetch. The server returns JSON on success/failure (toast
    // only, form stays as-is) or the re-rendered partial HTML when validation
    // failed (form needs to show the errors).
    function submitViaFetch(form) {
        postFormViaFetch(form, handleJsonResult, function (html) {
            body.innerHTML = html;
            bindProfileForms();
        }).catch(function () {
            showAjaxToast('Fehler', 'Ein Fehler ist aufgetreten.', false);
        });
    }

    function handleJsonResult(data) {
        showAjaxToast(data.title, data.message, data.success);
        if (data.success && data.displayName) {
            var nameSpan = document.getElementById('profileDisplayName');
            if (nameSpan) nameSpan.textContent = data.displayName;
        }
    }
}());