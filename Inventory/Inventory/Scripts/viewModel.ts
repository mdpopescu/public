/// <reference path="./knockout.d.ts" />
/// <reference path="./knockout.validation.d.ts" />

class ViewModel {
  private errors: any;
  private url: string;
  private rootNode: Element;
  private model: any;

  initialize: () => void;
  submit: () => bool;
  update: () => void;
  __load: (callback) => void;
  __update: (data: any) => void;

  constructor(url: string, rootNode: Element) {
    // set up the methods
    
    if (!this.initialize) {
      this.initialize = () => {
        this.__load((data: any) => {
          this.model = ko.mapping.fromJS({ data: data });
          ko.applyBindings(this.model, this.rootNode);
        });
      }
    }

    this.submit = () => {
      if (this.errors().length == 0)
        return true;

      this.errors.showAllMessages();
      return false;
    }

    if (!this.update) {
      this.update = () => {
        this.__load(this.__update);
      }
    }

    if (!this.__load) {
      this.__load = callback => {
        $.getJSON(this.url, callback);
      }
    }

    this.__update = data => {
      ko.mapping.fromJS({ data: data }, this.model);
    }

    // start the real constructor code

    this.errors = ko.validation.group(this, { deep: true, observable: false });
    this.url = url;
    this.rootNode = rootNode;

    this.initialize();
  }
}