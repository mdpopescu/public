var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DetailViewModel = (function (_super) {
    __extends(DetailViewModel, _super);
    function DetailViewModel(url, rootNode) {
        var _this = this;
        _super.call(this, url, rootNode);

        this.initialize = function () {
        };

        this.update = function (id) {
            _this.__load({ id: id }, _this.__updateItems);
        };

        this.__load = function (data, callback) {
            $.getJSON(_this.url, data, callback);
        };

        this.__updateItems = function (data) {
            if (_this.model) {
                ko.mapping.fromJS({ items: data }, _this.model);
            } else {
                _this.model = ko.mapping.fromJS({ items: data });
                ko.applyBindings(_this.model, _this.rootNode);
            }
        };
    }
    return DetailViewModel;
})(ViewModel);
//@ sourceMappingURL=detailViewModel.js.map
