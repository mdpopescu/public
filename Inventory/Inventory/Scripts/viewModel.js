var ViewModel = (function () {
    function ViewModel(url, rootNode) {
        this.errors = ko.validation.group(this, { deep: true, observable: false });
        this.url = url;
        this.rootNode = rootNode;

        this.load(this.initialize);
    }
    ViewModel.prototype.load = function (callback) {
        $.getJSON(this.url, callback);
    };

    ViewModel.prototype.loadWithData = function (data, callback) {
        $.getJSON(this.url, data, callback);
    };

    ViewModel.prototype.initialize = function (data) {
        this.model = ko.mapping.fromJS({ data: data });
        ko.applyBindings(this.model, this.rootNode);
    };

    ViewModel.prototype.update = function () {
        this.load(this.__update);
    };

    ViewModel.prototype.__update = function (data) {
        ko.mapping.fromJS({ data: data }, this.model);
    };

    ViewModel.prototype.updateForId = function (id) {
        this.loadWithData({ id: id }, this.__updateItems);
    };

    ViewModel.prototype.__updateItems = function (data) {
        ko.mapping.fromJS({ items: data }, this.model);
    };

    ViewModel.prototype.submit = function () {
        if (this.errors().length == 0)
            return true;

        this.errors.showAllMessages();
        return false;
    };
    return ViewModel;
})();
//@ sourceMappingURL=viewModel.js.map
