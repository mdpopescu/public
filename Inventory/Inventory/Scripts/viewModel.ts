/// <reference path="./knockout.d.ts" />
/// <reference path="./knockout.validation.d.ts" />

class ViewModel {
  private errors: any;
  private url: string;
  private rootNode: any;
  private model: any;

  constructor(url, rootNode) {
    this.errors = ko.validation.group(this, { deep: true, observable: false });
    this.url = url;
    this.rootNode = rootNode;

    this.load(this.initialize);
  }

  load(callback): void {
    $.getJSON(this.url, callback);
  }

  loadWithData(data: any, callback): void {
    $.getJSON(this.url, data, callback);
  }

  initialize(data: any) {
    this.model = ko.mapping.fromJS({ data: data });
    ko.applyBindings(this.model, this.rootNode);
  }

  update() {
    this.load(this.__update);
  }

  __update(data: any) {
    ko.mapping.fromJS({ data: data }, this.model);
  }

  updateForId(id: int) {
    this.loadWithData({ id: id }, this.__updateItems);
  }

  __updateItems(data: any) {
    ko.mapping.fromJS({ items: data }, this.model);
  }

  submit(): bool {
    if (this.errors().length == 0)
      return true;

    this.errors.showAllMessages();
    return false;
  }
}