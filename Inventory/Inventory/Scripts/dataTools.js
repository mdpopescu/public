var ViewModel = (function () {
    function ViewModel(url) {
        this.errors = ko.validation.group(this, { deep: true, observable: false });
        this.url = url;

        this.load(this.initialize);
    }
    ViewModel.prototype.load = function (callback) {
        $.getJSON(this.url, callback);
    };

    ViewModel.prototype.initialize = function (data) {
        this.model = ko.mapping.fromJS({ data: data });
        ko.applyBindings(this.model);
    };

    ViewModel.prototype.update = function () {
        this.load(this.__update);
    };

    ViewModel.prototype.__update = function (data) {
        ko.mapping.fromJS({ data: data }, this.model);
    };

    ViewModel.prototype.submit = function () {
        if (this.errors().length == 0)
            return true;

        this.errors.showAllMessages();
        return false;
    };
    return ViewModel;
})();
//@ sourceMappingURL=dataTools.js.map
