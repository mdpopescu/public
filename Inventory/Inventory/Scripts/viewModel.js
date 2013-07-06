var ViewModel = (function () {
    function ViewModel(url, rootNode) {
        var _this = this;
        this.initialize = function () {
            _this.__load(function (data) {
                this.model = ko.mapping.fromJS({ data: data });
                ko.applyBindings(this.model, this.rootNode);
            });
        };

        this.submit = function () {
            if (_this.errors().length == 0)
                return true;

            _this.errors.showAllMessages();
            return false;
        };

        this.update = function () {
            _this.__load(_this.__update);
        };

        this.__load = function (callback) {
            $.getJSON(_this.url, callback);
        };

        this.__update = function (data) {
            ko.mapping.fromJS({ data: data }, _this.model);
        };

        this.errors = ko.validation.group(this, { deep: true, observable: false });
        this.url = url;
        this.rootNode = rootNode;

        this.initialize();
    }
    return ViewModel;
})();
//@ sourceMappingURL=viewModel.js.map
