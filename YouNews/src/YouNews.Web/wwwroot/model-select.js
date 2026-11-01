(function () {
    var select = document.querySelector('[data-model-select]');
    if (!select) {
        return;
    }

    var button = select.form.querySelector('button[type=submit]');

    function update() {
        button.disabled = select.value === select.dataset.currentModel;
    }

    select.addEventListener('change', update);
    update();
})();