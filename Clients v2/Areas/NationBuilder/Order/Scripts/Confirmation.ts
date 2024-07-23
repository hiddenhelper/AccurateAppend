var orderModel: any;

$(document).ready(() => {

  $("#submit")
    .click(() => {
      $("<input>").attr({
        type: "hidden",
        name: "orderModel",
        value: ko.toJSON(orderModel.order)
      }).appendTo("form").first();

      $("form").first().submit();
    });
});