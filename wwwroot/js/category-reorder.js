// Drag-and-drop reordering for the Category index page (SortableJS).
document.addEventListener("DOMContentLoaded", function () {
    var categoriesContainer = document.getElementById("categories-container");
    if (!categoriesContainer) {
        return;
    }

    Sortable.create(categoriesContainer, {
        handle: ".drag-handle",
        animation: 150,
        onEnd: persistCategoryOrder,
    });

    document.querySelectorAll(".subcategory-sortable").forEach(function (tbody) {
        Sortable.create(tbody, {
            handle: ".drag-handle",
            animation: 150,
            onEnd: persistSubCategoryOrder,
        });
    });
});

function persistCategoryOrder() {
    var categoriesContainer = document.getElementById("categories-container");
    var items = [...categoriesContainer.children].map(function (el, index) {
        return { id: el.dataset.categoryId, sortIndex: index + 1 };
    });
    postReorder("/Category/ReorderCategories", buildItemsFormData(items));
}

function persistSubCategoryOrder(evt) {
    var tbody = evt.target;
    var items = [...tbody.children].map(function (el, index) {
        return { id: el.dataset.subcategoryId, sortIndex: index + 1 };
    });
    var formData = buildItemsFormData(items);
    formData.append("categoryId", tbody.dataset.categoryId);
    postReorder("/Category/ReorderSubCategories", formData);
}

function buildItemsFormData(items) {
    var formData = new FormData();
    items.forEach(function (item, index) {
        formData.append("items[" + index + "].Id", item.id);
        formData.append("items[" + index + "].SortIndex", item.sortIndex);
    });
    return formData;
}

function getAntiforgeryToken() {
    var tokenInput = document.querySelector(
        '#antiforgeryTokenForm input[name="__RequestVerificationToken"]'
    );
    return tokenInput ? tokenInput.value : "";
}

function postReorder(url, formData) {
    formData.append("__RequestVerificationToken", getAntiforgeryToken());

    fetch(url, { method: "POST", body: formData })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Reorder request failed with status " + response.status);
            }
        })
        .catch(function (err) {
            console.error(err);
            location.reload();
        });
}
