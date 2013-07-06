/// <reference path="./knockout.d.ts" />

class ViewModel {
  private errors: any;
  private url: string;
  private model: any;

  constructor(url) {
    this.errors = ko.validation.group(this, { deep: true, observable: false });
    this.url = url;

    this.load(this.initialize);
  }

  load(callback): void {
    $.getJSON(this.url, callback);
  }

  initialize(data: any) {
    this.model = ko.mapping.fromJS({ data: data });
    ko.applyBindings(this.model);
  }

  update() {
    this.load(this.__update);
  }

  __update(data: any) {
    ko.mapping.fromJS({ data: data }, this.model);
  }

  submit(): bool {
    if (this.errors().length == 0)
      return true;

    this.errors.showAllMessages();
    return false;
  }
}