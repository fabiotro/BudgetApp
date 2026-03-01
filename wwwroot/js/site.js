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