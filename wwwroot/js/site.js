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
                    html += '<div class="small text-muted mb-2"><a href="' + escapeHtml(inv.campUrl) + '">Lager anzeigen</a></div>';
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