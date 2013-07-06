/// <reference path="./viewModel.ts" />
/// <reference path="./knockout.d.ts" />
/// <reference path="./knockout.validation.d.ts" />

class DetailViewModel extends ViewModel {
  initialize: () => void;
  update: (id: number) => void;
  __load: (data: any, callback) => void;
  __updateItems: (data: any) => void;

  constructor(url: string, rootNode: Element) {
    super(url, rootNode);

    // set up the methods

    this.initialize = () => { }

    this.update = (id: number) => {
      this.__load({ id: id }, this.__updateItems);
    }

    this.__load = (data: any, callback): void => {
      $.getJSON(this.url, data, callback);
    }

    this.__updateItems = (data: any) => {
      if (this.model) {
        ko.mapping.fromJS({ items: data }, this.model);
      }
      else {
        this.model = ko.mapping.fromJS({ items: data });
        ko.applyBindings(this.model, this.rootNode);
      }
    }
  }
}