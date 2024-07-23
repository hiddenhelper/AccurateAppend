/// <reference path="../../typings/knockout/knockout.d.ts" />

var viewModel: any;

$(document).ready(() => {
  viewModel = new AccurateAppend.JobProcessing.ViewModel();
  viewModel.load();
  ko.applyBindings(viewModel);
});

module AccurateAppend.JobProcessing {

  declare let PostBack_AreaName: string;
  declare let OrderId: string;
  declare let OrderMinimum: number;
  declare let OrderRecordCount: number;
  declare let ListName: string;
  declare let OrderExtensionData: object;

  export class ViewModel {

    products: KnockoutObservableArray<Product>;
    total: KnockoutObservable<number>;

    constructor() {
      var self = this;

      self.products = ko.observableArray([]);
      self.total = ko.computed(() => Math.max(self.products().map(item => item.includeInOrder() ? item.subtotal : 0)
          .reduce((runningTotal, subtotal) => runningTotal + subtotal, 0),
          OrderMinimum),
        self);
    }

    filteredProductsByCategory(filter: SupportedProductCategory) {
      return ko.utils.arrayFilter(this.products(),
        prod => prod.category === filter);
    }

    filteredProduct(filter: PublicProduct) {
      return ko.utils.arrayFirst(this.products(),
        prod => prod.productKey === filter);
    }

    load() {
      var self = this;
      $.ajax({
        url: `/${PostBack_AreaName}/Pricing/GetProducts?cartId=${OrderId}`,
        dataType: "json",
        async: false,
        type: "GET",
        success(data) {
          $.each(data,
            (i, p) => {
              const product = new Product(p.Title,
                p.OperationName,
                p.Description,
                p.Cost,
                p.EstMatchRate,
                p.SuitableRecords,
                p.Category);
              if (product.productKey === PublicProduct.PHONE_PREM) product.performOverwrites = true;
              self.products.push(product);
            });
        }
      });
    }

    reset() {
      this.products([]);
    }

    isValid() {
      var valid = false;
      ko.utils.arrayForEach(this.products(),
        (e: any) => {
          if (e.includeInOrder() === true)
            valid = true;
        });
      return valid;
    }

    submit() {

      // validate
      if (this.isValid() === false) {
          displayMessage(`Please select at least one product to submit an order`, "warning");
        return false;
      } else {
        $("#alert").removeClass().hide();
      }

      var selectedProducts = [];
      ko.utils.arrayForEach(this.products(),
        (product: any) => {
          if (product.includeInOrder() === true) {
            selectedProducts.push(product);
          }
        });

      const orderModel = {
        ...OrderExtensionData,
        products: selectedProducts,
        orderId: OrderId,
        listName: ListName,
        recordCount: OrderRecordCount
      };

      // add to form & submit
      $("<input>").attr({
        type: "hidden",
        name: "orderModel",
        value: ko.toJSON(orderModel)
      }).appendTo("form").first();

      $("form").submit();

      return false;
    }

    emailBroadcastPolicyStatement(flag) {
      if (flag) {
        const product = ko.utils.arrayFirst(this.products(),
          item => item.productKey === $(".modal-footer [name='productKey']").val());
        product.includeInOrder(true);
      }
    }

    addProductToOrder() {
      const product = ko.utils.arrayFirst(this.products(),
        item => item.productKey === $("#product-details-modal [name='productKey']").val());
      product.includeInOrder(true);
      $("#product-details-modal").modal("hide");
    }

    multiMatchPhoneIncludedInOrder() {
      return this.filteredProduct(PublicProduct.PHONE_PREM).includeInOrder() &&
        this.filteredProduct(PublicProduct.PHONE_MOB).includeInOrder() &&
        this.filteredProduct(PublicProduct.PHONE_DA).includeInOrder();
    }

    addMultiMatchPhoneIncludedInOrder() {
      this.filteredProduct(PublicProduct.PHONE_PREM).add();
      this.filteredProduct(PublicProduct.PHONE_MOB).add();
      this.filteredProduct(PublicProduct.PHONE_DA).add();
    }

    removeMultiMatchPhoneIncludedInOrder() {
      this.filteredProduct(PublicProduct.PHONE_PREM).remove();
      this.filteredProduct(PublicProduct.PHONE_MOB).remove();
      this.filteredProduct(PublicProduct.PHONE_DA).remove();
    }

  }

  export class Product {

    includeInOrder: KnockoutObservable<boolean>;
    performOverwrites: boolean;
    estMatches: number;
    subtotal: number;

    constructor(public title,
      public productKey: PublicProduct,
      public description: string,
      public cost: number,
      public estMatchRate: number,
      public count: number,
      public category: SupportedProductCategory) {
      this.includeInOrder = ko.observable(false);
      this.estMatches = Math.round(OrderRecordCount * this.estMatchRate);
      this.subtotal = this.estMatches * this.cost;
    }

    add() {
      $("#validationError").hide();
      if (this.productKey === PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION ||
        this.productKey === PublicProduct.EMAIL_VER_DELIVERABLE) {
        $("#broadcasting-notice").modal("show");
        $(".modal-footer [name='productKey']").val(this.productKey);
      } else {
        this.includeInOrder(true);
      }
    }

    remove() {
      this.includeInOrder(false);
      $("#validationError").hide();
    }

    displayProductInfo() {
      $("#product-details-modal h5").text(this.title);
      $.get(`/${PostBack_AreaName}/Pricing/GetHelpText?product=${this.productKey}`,
        data => {
          $("#product-details-modal .modal-body").html(data.Text);
          $("#product-details-modal").modal("show");
          $("#product-details-modal [name='productKey']").val(this.productKey);

        });
    }

  }

  enum PublicProduct {
    EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION = "EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION",
    ACCUSEND_W_INPUT_EMAIL_VERIFICATION = "ACCUSEND_W_INPUT_EMAIL_VERIFICATION",
    EMAIL_VER_DELIVERABLE = "EMAIL_VER_DELIVERABLE",
    PHONE_MOB = "PHONE_MOB",
    PHONE_PREM = "PHONE_PREM",
    PHONE_DA = "PHONE_DA",
    DEMOGRAHICS = "DEMOGRAHICS",
    NCOA48 = "NCOA48",
    CASS = "CASS",
    PHONE = "PHONE",
    UNIFIED_REV_ALL = "UNIFIED_REV_ALL",
    PHONE_BUS_DA = "PHONE_BUS_DA",
    SCORE_DONOR = "SCORE_DONOR",
    SCORE_GREEN = "SCORE_GREEN",
    SCORE_WEALTH = "SCORE_WEALTH",
    EMAIL_BASIC_REV = "EMAIL_BASIC_REV",
    PHONE_REV_PREM = "PHONE_REV_PREM",
  }

  enum SupportedProductCategory {
    Append,
    Enhance,
    Score,
    Verify,
    Phone,
    Email
  }

  function displayMessage(message: any, type: any);
  function displayMessage(message, type) {
      $("#alert").removeClass().addClass(`alert alert-${type}`).html(message).show()
          .fadeTo(5000, 500).slideUp(500, () => { $("#alert").slideUp(500) });
  }

}

