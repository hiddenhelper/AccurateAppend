/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../../scripts/typings/knockout/knockout.d.ts" />

$(() => {
  $("#submit").click(() => {
    $("form").submit();
  });
});

module Sales.Confirmation {

  export class ViewModel {

    products: KnockoutObservableArray<Product>;
    minimum: KnockoutObservable<number>;
    cartId: any;
    links: any;


    constructor(cartId: any, links: any) {
      this.cartId = cartId;
      this.links = links;
      this.products = ko.observableArray([]);
      this.minimum = ko.observable(0);
    }

    adjustedTotal(): any {
      const ot = this.products().reduce((a, b) => a + b.subtotal, 0);
      return ot > this.minimum() ? ot : this.minimum();
    }

    total(): any {
      return this.products().reduce((a, b) => a + b.subtotal, 0);
    }

    load() {
      $.ajax(
        {
          type: "GET",
          url: this.links.dataUrl,
          data: { cartId: this.cartId },
          success: data => {
            $.each(data.Products,
              (i, v) => {
                this.products.push(new Product(v.Product, v.Description, v.Price, v.EstimatedMatches));
              });
            this.minimum(data.OrderMinimum);
          }
        });
      // display payment form if no primary form is present. 
      $.get(this.links.walletUrl,
        data => {
          if (!data.Primary) $("#paymentForm").show();
        });
    }

  }

  export class Product {
    subtotal: number;

    constructor(public product,
      public description: string,
      public price: number,
      public estimatedMatches: number) {
      this.subtotal = this.estimatedMatches * this.price;
    }
  }
}