var ViewModel = (function () {
    function ViewModel(url, rootNode) {
        var _this = this;
        if (!this.initialize) {
            this.initialize = function () {
                _this.__load(function (data) {
                    _this.model = ko.mapping.fromJS({ data: data });
                    ko.applyBindings(_this.model, _this.rootNode);
                });
            };
        }

        this.submit = function () {
            if (_this.errors().length == 0)
                return true;

            _this.errors.showAllMessages();
            return false;
        };

        if (!this.update) {
            this.update = function () {
                _this.__load(_this.__update);
            };
        }

        if (!this.__load) {
            this.__load = function (callback) {
                $.getJSON(_this.url, callback);
            };
        }

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
