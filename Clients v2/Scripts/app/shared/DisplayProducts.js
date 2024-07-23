var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var viewModel;
$(document).ready(function () {
    viewModel = new AccurateAppend.JobProcessing.ViewModel();
    viewModel.load();
    ko.applyBindings(viewModel);
});
var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var ViewModel = (function () {
            function ViewModel() {
                var self = this;
                self.products = ko.observableArray([]);
                self.total = ko.computed(function () { return Math.max(self.products().map(function (item) { return item.includeInOrder() ? item.subtotal : 0; })
                    .reduce(function (runningTotal, subtotal) { return runningTotal + subtotal; }, 0), OrderMinimum); }, self);
            }
            ViewModel.prototype.filteredProductsByCategory = function (filter) {
                return ko.utils.arrayFilter(this.products(), function (prod) { return prod.category === filter; });
            };
            ViewModel.prototype.filteredProduct = function (filter) {
                return ko.utils.arrayFirst(this.products(), function (prod) { return prod.productKey === filter; });
            };
            ViewModel.prototype.load = function () {
                var self = this;
                $.ajax({
                    url: "/" + PostBack_AreaName + "/Pricing/GetProducts?cartId=" + OrderId,
                    dataType: "json",
                    async: false,
                    type: "GET",
                    success: function (data) {
                        $.each(data, function (i, p) {
                            var product = new Product(p.Title, p.OperationName, p.Description, p.Cost, p.EstMatchRate, p.SuitableRecords, p.Category);
                            if (product.productKey === PublicProduct.PHONE_PREM)
                                product.performOverwrites = true;
                            self.products.push(product);
                        });
                    }
                });
            };
            ViewModel.prototype.reset = function () {
                this.products([]);
            };
            ViewModel.prototype.isValid = function () {
                var valid = false;
                ko.utils.arrayForEach(this.products(), function (e) {
                    if (e.includeInOrder() === true)
                        valid = true;
                });
                return valid;
            };
            ViewModel.prototype.submit = function () {
                if (this.isValid() === false) {
                    displayMessage("Please select at least one product to submit an order", "warning");
                    return false;
                }
                else {
                    $("#alert").removeClass().hide();
                }
                var selectedProducts = [];
                ko.utils.arrayForEach(this.products(), function (product) {
                    if (product.includeInOrder() === true) {
                        selectedProducts.push(product);
                    }
                });
                var orderModel = __assign({}, OrderExtensionData, { products: selectedProducts, orderId: OrderId, listName: ListName, recordCount: OrderRecordCount });
                $("<input>").attr({
                    type: "hidden",
                    name: "orderModel",
                    value: ko.toJSON(orderModel)
                }).appendTo("form").first();
                $("form").submit();
                return false;
            };
            ViewModel.prototype.emailBroadcastPolicyStatement = function (flag) {
                if (flag) {
                    var product = ko.utils.arrayFirst(this.products(), function (item) { return item.productKey === $(".modal-footer [name='productKey']").val(); });
                    product.includeInOrder(true);
                }
            };
            ViewModel.prototype.addProductToOrder = function () {
                var product = ko.utils.arrayFirst(this.products(), function (item) { return item.productKey === $("#product-details-modal [name='productKey']").val(); });
                product.includeInOrder(true);
                $("#product-details-modal").modal("hide");
            };
            ViewModel.prototype.multiMatchPhoneIncludedInOrder = function () {
                return this.filteredProduct(PublicProduct.PHONE_PREM).includeInOrder() &&
                    this.filteredProduct(PublicProduct.PHONE_MOB).includeInOrder() &&
                    this.filteredProduct(PublicProduct.PHONE_DA).includeInOrder();
            };
            ViewModel.prototype.addMultiMatchPhoneIncludedInOrder = function () {
                this.filteredProduct(PublicProduct.PHONE_PREM).add();
                this.filteredProduct(PublicProduct.PHONE_MOB).add();
                this.filteredProduct(PublicProduct.PHONE_DA).add();
            };
            ViewModel.prototype.removeMultiMatchPhoneIncludedInOrder = function () {
                this.filteredProduct(PublicProduct.PHONE_PREM).remove();
                this.filteredProduct(PublicProduct.PHONE_MOB).remove();
                this.filteredProduct(PublicProduct.PHONE_DA).remove();
            };
            return ViewModel;
        }());
        JobProcessing.ViewModel = ViewModel;
        var Product = (function () {
            function Product(title, productKey, description, cost, estMatchRate, count, category) {
                this.title = title;
                this.productKey = productKey;
                this.description = description;
                this.cost = cost;
                this.estMatchRate = estMatchRate;
                this.count = count;
                this.category = category;
                this.includeInOrder = ko.observable(false);
                this.estMatches = Math.round(OrderRecordCount * this.estMatchRate);
                this.subtotal = this.estMatches * this.cost;
            }
            Product.prototype.add = function () {
                $("#validationError").hide();
                if (this.productKey === PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION ||
                    this.productKey === PublicProduct.EMAIL_VER_DELIVERABLE) {
                    $("#broadcasting-notice").modal("show");
                    $(".modal-footer [name='productKey']").val(this.productKey);
                }
                else {
                    this.includeInOrder(true);
                }
            };
            Product.prototype.remove = function () {
                this.includeInOrder(false);
                $("#validationError").hide();
            };
            Product.prototype.displayProductInfo = function () {
                var _this = this;
                $("#product-details-modal h5").text(this.title);
                $.get("/" + PostBack_AreaName + "/Pricing/GetHelpText?product=" + this.productKey, function (data) {
                    $("#product-details-modal .modal-body").html(data.Text);
                    $("#product-details-modal").modal("show");
                    $("#product-details-modal [name='productKey']").val(_this.productKey);
                });
            };
            return Product;
        }());
        JobProcessing.Product = Product;
        var PublicProduct;
        (function (PublicProduct) {
            PublicProduct["EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION"] = "EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION";
            PublicProduct["ACCUSEND_W_INPUT_EMAIL_VERIFICATION"] = "ACCUSEND_W_INPUT_EMAIL_VERIFICATION";
            PublicProduct["EMAIL_VER_DELIVERABLE"] = "EMAIL_VER_DELIVERABLE";
            PublicProduct["PHONE_MOB"] = "PHONE_MOB";
            PublicProduct["PHONE_PREM"] = "PHONE_PREM";
            PublicProduct["PHONE_DA"] = "PHONE_DA";
            PublicProduct["DEMOGRAHICS"] = "DEMOGRAHICS";
            PublicProduct["NCOA48"] = "NCOA48";
            PublicProduct["CASS"] = "CASS";
            PublicProduct["PHONE"] = "PHONE";
            PublicProduct["UNIFIED_REV_ALL"] = "UNIFIED_REV_ALL";
            PublicProduct["PHONE_BUS_DA"] = "PHONE_BUS_DA";
            PublicProduct["SCORE_DONOR"] = "SCORE_DONOR";
            PublicProduct["SCORE_GREEN"] = "SCORE_GREEN";
            PublicProduct["SCORE_WEALTH"] = "SCORE_WEALTH";
            PublicProduct["EMAIL_BASIC_REV"] = "EMAIL_BASIC_REV";
            PublicProduct["PHONE_REV_PREM"] = "PHONE_REV_PREM";
        })(PublicProduct || (PublicProduct = {}));
        var SupportedProductCategory;
        (function (SupportedProductCategory) {
            SupportedProductCategory[SupportedProductCategory["Append"] = 0] = "Append";
            SupportedProductCategory[SupportedProductCategory["Enhance"] = 1] = "Enhance";
            SupportedProductCategory[SupportedProductCategory["Score"] = 2] = "Score";
            SupportedProductCategory[SupportedProductCategory["Verify"] = 3] = "Verify";
            SupportedProductCategory[SupportedProductCategory["Phone"] = 4] = "Phone";
            SupportedProductCategory[SupportedProductCategory["Email"] = 5] = "Email";
        })(SupportedProductCategory || (SupportedProductCategory = {}));
        function displayMessage(message, type) {
            $("#alert").removeClass().addClass("alert alert-" + type).html(message).show()
                .fadeTo(5000, 500).slideUp(500, function () { $("#alert").slideUp(500); });
        }
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiRGlzcGxheVByb2R1Y3RzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiRGlzcGxheVByb2R1Y3RzLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7O0FBRUEsSUFBSSxTQUFjLENBQUM7QUFFbkIsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxDQUFDLEtBQUssQ0FBQztJQUNoQixTQUFTLEdBQUcsSUFBSSxjQUFjLENBQUMsYUFBYSxDQUFDLFNBQVMsRUFBRSxDQUFDO0lBQ3pELFNBQVMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztJQUNqQixFQUFFLENBQUMsYUFBYSxDQUFDLFNBQVMsQ0FBQyxDQUFDO0FBQzlCLENBQUMsQ0FBQyxDQUFDO0FBRUgsSUFBTyxjQUFjLENBb09wQjtBQXBPRCxXQUFPLGNBQWM7SUFBQyxJQUFBLGFBQWEsQ0FvT2xDO0lBcE9xQixXQUFBLGFBQWE7UUFTakM7WUFLRTtnQkFDRSxJQUFJLElBQUksR0FBRyxJQUFJLENBQUM7Z0JBRWhCLElBQUksQ0FBQyxRQUFRLEdBQUcsRUFBRSxDQUFDLGVBQWUsQ0FBQyxFQUFFLENBQUMsQ0FBQztnQkFDdkMsSUFBSSxDQUFDLEtBQUssR0FBRyxFQUFFLENBQUMsUUFBUSxDQUFDLGNBQU0sT0FBQSxJQUFJLENBQUMsR0FBRyxDQUFDLElBQUksQ0FBQyxRQUFRLEVBQUUsQ0FBQyxHQUFHLENBQUMsVUFBQSxJQUFJLElBQUksT0FBQSxJQUFJLENBQUMsY0FBYyxFQUFFLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxRQUFRLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBekMsQ0FBeUMsQ0FBQztxQkFDekcsTUFBTSxDQUFDLFVBQUMsWUFBWSxFQUFFLFFBQVEsSUFBSyxPQUFBLFlBQVksR0FBRyxRQUFRLEVBQXZCLENBQXVCLEVBQUUsQ0FBQyxDQUFDLEVBQy9ELFlBQVksQ0FBQyxFQUZjLENBRWQsRUFDZixJQUFJLENBQUMsQ0FBQztZQUNWLENBQUM7WUFFRCw4Q0FBMEIsR0FBMUIsVUFB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d0JBQXdCLFFBTzVCO1FBR0QsU0FBUyxjQUFjLENBQUMsT0FBTyxFQUFFLElBQUk7WUFDakMsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxDQUFDLFdBQVcsRUFBRSxDQUFDLFFBQVEsQ0FBQyxpQkFBZSxJQUFNLENBQUMsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLENBQUMsSUFBSSxFQUFFO2lCQUN6RSxNQUFNLENBQUMsSUFBSSxFQUFFLEdBQUcsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxHQUFHLEVBQUUsY0FBUSxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUMsT0FBTyxDQUFDLEdBQUcsQ0FBQyxDQUFBLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDNUUsQ0FBQztJQUVILENBQUMsRUFwT3FCLGFBQWEsR0FBYiw0QkFBYSxLQUFiLDRCQUFhLFFBb09sQztBQUFELENBQUMsRUFwT00sY0FBYyxLQUFkLGNBQWMsUUFvT3BCIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uL3R5cGluZ3Mva25vY2tvdXQva25vY2tvdXQuZC50c1wiIC8+XHJcblxyXG52YXIgdmlld01vZGVsOiBhbnk7XHJcblxyXG4kKGRvY3VtZW50KS5yZWFkeSgoKSA9PiB7XHJcbiAgdmlld01vZGVsID0gbmV3IEFjY3VyYXRlQXBwZW5kLkpvYlByb2Nlc3NpbmcuVmlld01vZGVsKCk7XHJcbiAgdmlld01vZGVsLmxvYWQoKTtcclxuICBrby5hcHBseUJpbmRpbmdzKHZpZXdNb2RlbCk7XHJcbn0pO1xyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLkpvYlByb2Nlc3Npbmcge1xyXG5cclxuICBkZWNsYXJlIGxldCBQb3N0QmFja19BcmVhTmFtZTogc3RyaW5nO1xyXG4gIGRlY2xhcmUgbGV0IE9yZGVySWQ6IHN0cmluZztcclxuICBkZWNsYXJlIGxldCBPcmRlck1pbmltdW06IG51bWJlcjtcclxuICBkZWNsYXJlIGxldCBPcmRlclJlY29yZENvdW50OiBudW1iZXI7XHJcbiAgZGVjbGFyZSBsZXQgTGlzdE5hbWU6IHN0cmluZztcclxuICBkZWNsYXJlIGxldCBPcmRlckV4dGVuc2lvbkRhdGE6IG9iamVjdDtcclxuXHJcbiAgZXhwb3J0IGNsYXNzIFZpZXdNb2RlbCB7XHJcblxyXG4gICAgcHJvZHVjdHM6IEtub2Nrb3V0T2JzZXJ2YWJsZUFycmF5PFByb2R1Y3Q+O1xyXG4gICAgdG90YWw6IEtub2Nrb3V0T2JzZXJ2YWJsZTxudW1iZXI+O1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKCkge1xyXG4gICAgICB2YXIgc2VsZiA9IHRoaXM7XHJcblxyXG4gICAgICBzZWxmLnByb2R1Y3RzID0ga28ub2JzZXJ2YWJsZUFycmF5KFtdKTtcclxuICAgICAgc2VsZi50b3RhbCA9IGtvLmNvbXB1dGVkKCgpID0+IE1hdGgubWF4KHNlbGYucHJvZHVjdHMoKS5tYXAoaXRlbSA9PiBpdGVtLmluY2x1ZGVJbk9yZGVyKCkgPyBpdGVtLnN1YnRvdGFsIDogMClcclxuICAgICAgICAgIC5yZWR1Y2UoKHJ1bm5pbmdUb3RhbCwgc3VidG90YWwpID0+IHJ1bm5pbmdUb3RhbCArIHN1YnRvdGFsLCAwKSxcclxuICAgICAgICAgIE9yZGVyTWluaW11bSksXHJcbiAgICAgICAgc2VsZik7XHJcbiAgICB9XHJcblxyXG4gICAgZmlsdGVyZWRQcm9kdWN0c0J5Q2F0ZWdvcnkoZmlsdGVyOiBTdXBwb3J0ZWRQcm9kdWN0Q2F0ZWdvcnkpIHtcclxuICAgICAgcmV0dXJuIGtvLnV0aWxzLmFycmF5RmlsdGVyKHRoaXMucHJvZHVjdHMoKSxcclxuICAgICAgICBwcm9kID0+IHByb2QuY2F0ZWdvcnkgPT09IGZpbHRlcik7XHJcbiAgICB9XHJcblxyXG4gICAgZmlsdGVyZWRQcm9kdWN0KGZpbHRlcjogUHVibGljUHJvZHVjdCkge1xyXG4gICAgICByZXR1cm4ga28udXRpbHMuYXJyYXlGaXJzdCh0aGlzLnByb2R1Y3RzKCksXHJcbiAgICAgICAgcHJvZCA9PiBwcm9kLnByb2R1Y3RLZXkgPT09IGZpbHRlcik7XHJcbiAgICB9XHJcblxyXG4gICAgbG9hZCgpIHtcclxuICAgICAgdmFyIHNlbGYgPSB0aGlzO1xyXG4gICAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogYC8ke1Bvc3RCYWNrX0FyZWFOYW1lfS9QcmljaW5nL0dldFByb2R1Y3RzP2NhcnRJZD0ke09yZGVySWR9YCxcclxuICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgYXN5bmM6IGZhbHNlLFxyXG4gICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgc3VjY2VzcyhkYXRhKSB7XHJcbiAgICAgICAgICAkLmVhY2goZGF0YSxcclxuICAgICAgICAgICAgKGksIHApID0+IHtcclxuICAgICAgICAgICAgICBjb25zdCBwcm9kdWN0ID0gbmV3IFByb2R1Y3QocC5UaXRsZSxcclxuICAgICAgICAgICAgICAgIHAuT3BlcmF0aW9uTmFtZSxcclxuICAgICAgICAgICAgICAgIHAuRGVzY3JpcHRpb24sXHJcbiAgICAgICAgICAgICAgICBwLkNvc3QsXHJcbiAgICAgICAgICAgICAgICBwLkVzdE1hdGNoUmF0ZSxcclxuICAgICAgICAgICAgICAgIHAuU3VpdGFibGVSZWNvcmRzLFxyXG4gICAgICAgICAgICAgICAgcC5DYXRlZ29yeSk7XHJcbiAgICAgICAgICAgICAgaWYgKHByb2R1Y3QucHJvZHVjdEtleSA9PT0gUHVibGljUHJvZHVjdC5QSE9ORV9QUkVNKSBwcm9kdWN0LnBlcmZvcm1PdmVyd3JpdGVzID0gdHJ1ZTtcclxuICAgICAgICAgICAgICBzZWxmLnByb2R1Y3RzLnB1c2gocHJvZHVjdCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVzZXQoKSB7XHJcbiAgICAgIHRoaXMucHJvZHVjdHMoW10pO1xyXG4gICAgfVxyXG5cclxuICAgIGlzVmFsaWQoKSB7XHJcbiAgICAgIHZhciB2YWxpZCA9IGZhbHNlO1xyXG4gICAgICBrby51dGlscy5hcnJheUZvckVhY2godGhpcy5wcm9kdWN0cygpLFxyXG4gICAgICAgIChlOiBhbnkpID0+IHtcclxuICAgICAgICAgIGlmIChlLmluY2x1ZGVJbk9yZGVyKCkgPT09IHRydWUpXHJcbiAgICAgICAgICAgIHZhbGlkID0gdHJ1ZTtcclxuICAgICAgICB9KTtcclxuICAgICAgcmV0dXJuIHZhbGlkO1xyXG4gICAgfVxyXG5cclxuICAgIHN1Ym1pdCgpIHtcclxuXHJcbiAgICAgIC8vIHZhbGlkYXRlXHJcbiAgICAgIGlmICh0aGlzLmlzVmFsaWQoKSA9PT0gZmFsc2UpIHtcclxuICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGBQbGVhc2Ugc2VsZWN0IGF0IGxlYXN0IG9uZSBwcm9kdWN0IHRvIHN1Ym1pdCBhbiBvcmRlcmAsIFwid2FybmluZ1wiKTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgJChcIiNhbGVydFwiKS5yZW1vdmVDbGFzcygpLmhpZGUoKTtcclxuICAgICAgfVxyXG5cclxuICAgICAgdmFyIHNlbGVjdGVkUHJvZHVjdHMgPSBbXTtcclxuICAgICAga28udXRpbHMuYXJyYXlGb3JFYWNoKHRoaXMucHJvZHVjdHMoKSxcclxuICAgICAgICAocHJvZHVjdDogYW55KSA9PiB7XHJcbiAgICAgICAgICBpZiAocHJvZHVjdC5pbmNsdWRlSW5PcmRlcigpID09PSB0cnVlKSB7XHJcbiAgICAgICAgICAgIHNlbGVjdGVkUHJvZHVjdHMucHVzaChwcm9kdWN0KTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgIGNvbnN0IG9yZGVyTW9kZWwgPSB7XHJcbiAgICAgICAgLi4uT3JkZXJFeHRlbnNpb25EYXRhLFxyXG4gICAgICAgIHByb2R1Y3RzOiBzZWxlY3RlZFByb2R1Y3RzLFxyXG4gICAgICAgIG9yZGVySWQ6IE9yZGVySWQsXHJcbiAgICAgICAgbGlzdE5hbWU6IExpc3ROYW1lLFxyXG4gICAgICAgIHJlY29yZENvdW50OiBPcmRlclJlY29yZENvdW50XHJcbiAgICAgIH07XHJcblxyXG4gICAgICAvLyBhZGQgdG8gZm9ybSAmIHN1Ym1pdFxyXG4gICAgICAkKFwiPGlucHV0PlwiKS5hdHRyKHtcclxuICAgICAgICB0eXBlOiBcImhpZGRlblwiLFxyXG4gICAgICAgIG5hbWU6IFwib3JkZXJNb2RlbFwiLFxyXG4gICAgICAgIHZhbHVlOiBrby50b0pTT04ob3JkZXJNb2RlbClcclxuICAgICAgfSkuYXBwZW5kVG8oXCJmb3JtXCIpLmZpcnN0KCk7XHJcblxyXG4gICAgICAkKFwiZm9ybVwiKS5zdWJtaXQoKTtcclxuXHJcbiAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxuXHJcbiAgICBlbWFpbEJyb2FkY2FzdFBvbGljeVN0YXRlbWVudChmbGFnKSB7XHJcbiAgICAgIGlmIChmbGFnKSB7XHJcbiAgICAgICAgY29uc3QgcHJvZHVjdCA9IGtvLnV0aWxzLmFycmF5Rmlyc3QodGhpcy5wcm9kdWN0cygpLFxyXG4gICAgICAgICAgaXRlbSA9PiBpdGVtLnByb2R1Y3RLZXkgPT09ICQoXCIubW9kYWwtZm9vdGVyIFtuYW1lPSdwcm9kdWN0S2V5J11cIikudmFsKCkpO1xyXG4gICAgICAgIHByb2R1Y3QuaW5jbHVkZUluT3JkZXIodHJ1ZSk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBhZGRQcm9kdWN0VG9PcmRlcigpIHtcclxuICAgICAgY29uc3QgcHJvZHVjdCA9IGtvLnV0aWxzLmFycmF5Rmlyc3QodGhpcy5wcm9kdWN0cygpLFxyXG4gICAgICAgIGl0ZW0gPT4gaXRlbS5wcm9kdWN0S2V5ID09PSAkKFwiI3Byb2R1Y3QtZGV0YWlscy1tb2RhbCBbbmFtZT0ncHJvZHVjdEtleSddXCIpLnZhbCgpKTtcclxuICAgICAgcHJvZHVjdC5pbmNsdWRlSW5PcmRlcih0cnVlKTtcclxuICAgICAgJChcIiNwcm9kdWN0LWRldGFpbHMtbW9kYWxcIikubW9kYWwoXCJoaWRlXCIpO1xyXG4gICAgfVxyXG5cclxuICAgIG11bHRpTWF0Y2hQaG9uZUluY2x1ZGVkSW5PcmRlcigpIHtcclxuICAgICAgcmV0dXJuIHRoaXMuZmlsdGVyZWRQcm9kdWN0KFB1YmxpY1Byb2R1Y3QuUEhPTkVfUFJFTSkuaW5jbHVkZUluT3JkZXIoKSAmJlxyXG4gICAgICAgIHRoaXMuZmlsdGVyZWRQcm9kdWN0KFB1YmxpY1Byb2R1Y3QuUEhPTkVfTU9CKS5pbmNsdWRlSW5PcmRlcigpICYmXHJcbiAgICAgICAgdGhpcy5maWx0ZXJlZFByb2R1Y3QoUHVibGljUHJvZHVjdC5QSE9ORV9EQSkuaW5jbHVkZUluT3JkZXIoKTtcclxuICAgIH1cclxuXHJcbiAgICBhZGRNdWx0aU1hdGNoUGhvbmVJbmNsdWRlZEluT3JkZXIoKSB7XHJcbiAgICAgIHRoaXMuZmlsdGVyZWRQcm9kdWN0KFB1YmxpY1Byb2R1Y3QuUEhPTkVfUFJFTSkuYWRkKCk7XHJcbiAgICAgIHRoaXMuZmlsdGVyZWRQcm9kdWN0KFB1YmxpY1Byb2R1Y3QuUEhPTkVfTU9CKS5hZGQoKTtcclxuICAgICAgdGhpcy5maWx0ZXJlZFByb2R1Y3QoUHVibGljUHJvZHVjdC5QSE9ORV9EQSkuYWRkKCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVtb3ZlTXVsdGlNYXRjaFBob25lSW5jbHVkZWRJbk9yZGVyKCkge1xyXG4gICAgICB0aGlzLmZpbHRlcmVkUHJvZHVjdChQdWJsaWNQcm9kdWN0LlBIT05FX1BSRU0pLnJlbW92ZSgpO1xyXG4gICAgICB0aGlzLmZpbHRlcmVkUHJvZHVjdChQdWJsaWNQcm9kdWN0LlBIT05FX01PQikucmVtb3ZlKCk7XHJcbiAgICAgIHRoaXMuZmlsdGVyZWRQcm9kdWN0KFB1YmxpY1Byb2R1Y3QuUEhPTkVfREEpLnJlbW92ZSgpO1xyXG4gICAgfVxyXG5cclxuICB9XHJcblxyXG4gIGV4cG9ydCBjbGFzcyBQcm9kdWN0IHtcclxuXHJcbiAgICBpbmNsdWRlSW5PcmRlcjogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+O1xyXG4gICAgcGVyZm9ybU92ZXJ3cml0ZXM6IGJvb2xlYW47XHJcbiAgICBlc3RNYXRjaGVzOiBudW1iZXI7XHJcbiAgICBzdWJ0b3RhbDogbnVtYmVyO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHB1YmxpYyB0aXRsZSxcclxuICAgICAgcHVibGljIHByb2R1Y3RLZXk6IFB1YmxpY1Byb2R1Y3QsXHJcbiAgICAgIHB1YmxpYyBkZXNjcmlwdGlvbjogc3RyaW5nLFxyXG4gICAgICBwdWJsaWMgY29zdDogbnVtYmVyLFxyXG4gICAgICBwdWJsaWMgZXN0TWF0Y2hSYXRlOiBudW1iZXIsXHJcbiAgICAgIHB1YmxpYyBjb3VudDogbnVtYmVyLFxyXG4gICAgICBwdWJsaWMgY2F0ZWdvcnk6IFN1cHBvcnRlZFByb2R1Y3RDYXRlZ29yeSkge1xyXG4gICAgICB0aGlzLmluY2x1ZGVJbk9yZGVyID0ga28ub2JzZXJ2YWJsZShmYWxzZSk7XHJcbiAgICAgIHRoaXMuZXN0TWF0Y2hlcyA9IE1hdGgucm91bmQoT3JkZXJSZWNvcmRDb3VudCAqIHRoaXMuZXN0TWF0Y2hSYXRlKTtcclxuICAgICAgdGhpcy5zdWJ0b3RhbCA9IHRoaXMuZXN0TWF0Y2hlcyAqIHRoaXMuY29zdDtcclxuICAgIH1cclxuXHJcbiAgICBhZGQoKSB7XHJcbiAgICAgICQoXCIjdmFsaWRhdGlvbkVycm9yXCIpLmhpZGUoKTtcclxuICAgICAgaWYgKHRoaXMucHJvZHVjdEtleSA9PT0gUHVibGljUHJvZHVjdC5FTUFJTF9CQVNJQ19OT19TVVBSRVNTSU9OX1dfVkVSSUZJQ0FUSU9OIHx8XHJcbiAgICAgICAgdGhpcy5wcm9kdWN0S2V5ID09PSBQdWJsaWNQcm9kdWN0LkVNQUlMX1ZFUl9ERUxJVkVSQUJMRSkge1xyXG4gICAgICAgICQoXCIjYnJvYWRjYXN0aW5nLW5vdGljZVwiKS5tb2RhbChcInNob3dcIik7XHJcbiAgICAgICAgJChcIi5tb2RhbC1mb290ZXIgW25hbWU9J3Byb2R1Y3RLZXknXVwiKS52YWwodGhpcy5wcm9kdWN0S2V5KTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICB0aGlzLmluY2x1ZGVJbk9yZGVyKHRydWUpO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVtb3ZlKCkge1xyXG4gICAgICB0aGlzLmluY2x1ZGVJbk9yZGVyKGZhbHNlKTtcclxuICAgICAgJChcIiN2YWxpZGF0aW9uRXJyb3JcIikuaGlkZSgpO1xyXG4gICAgfVxyXG5cclxuICAgIGRpc3BsYXlQcm9kdWN0SW5mbygpIHtcclxuICAgICAgJChcIiNwcm9kdWN0LWRldGFpbHMtbW9kYWwgaDVcIikudGV4dCh0aGlzLnRpdGxlKTtcclxuICAgICAgJC5nZXQoYC8ke1Bvc3RCYWNrX0FyZWFOYW1lfS9QcmljaW5nL0dldEhlbHBUZXh0P3Byb2R1Y3Q9JHt0aGlzLnByb2R1Y3RLZXl9YCxcclxuICAgICAgICBkYXRhID0+IHtcclxuICAgICAgICAgICQoXCIjcHJvZHVjdC1kZXRhaWxzLW1vZGFsIC5tb2RhbC1ib2R5XCIpLmh0bWwoZGF0YS5UZXh0KTtcclxuICAgICAgICAgICQoXCIjcHJvZHVjdC1kZXRhaWxzLW1vZGFsXCIpLm1vZGFsKFwic2hvd1wiKTtcclxuICAgICAgICAgICQoXCIjcHJvZHVjdC1kZXRhaWxzLW1vZGFsIFtuYW1lPSdwcm9kdWN0S2V5J11cIikudmFsKHRoaXMucHJvZHVjdEtleSk7XHJcblxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICB9XHJcblxyXG4gIGVudW0gUHVibGljUHJvZHVjdCB7XHJcbiAgICBFTUFJTF9CQVNJQ19OT19TVVBSRVNTSU9OX1dfVkVSSUZJQ0FUSU9OID0gXCJFTUFJTF9CQVNJQ19OT19TVVBSRVNTSU9OX1dfVkVSSUZJQ0FUSU9OXCIsXHJcbiAgICBBQ0NVU0VORF9XX0lOUFVUX0VNQUlMX1ZFUklGSUNBVElPTiA9IFwiQUNDVVNFTkRfV19JTlBVVF9FTUFJTF9WRVJJRklDQVRJT05cIixcclxuICAgIEVNQUlMX1ZFUl9ERUxJVkVSQUJMRSA9IFwiRU1BSUxfVkVSX0RFTElWRVJBQkxFXCIsXHJcbiAgICBQSE9ORV9NT0IgPSBcIlBIT05FX01PQlwiLFxyXG4gICAgUEhPTkVfUFJFTSA9IFwiUEhPTkVfUFJFTVwiLFxyXG4gICAgUEhPTkVfREEgPSBcIlBIT05FX0RBXCIsXHJcbiAgICBERU1PR1JBSElDUyA9IFwiREVNT0dSQUhJQ1NcIixcclxuICAgIE5DT0E0OCA9IFwiTkNPQTQ4XCIsXHJcbiAgICBDQVNTID0gXCJDQVNTXCIsXHJcbiAgICBQSE9ORSA9IFwiUEhPTkVcIixcclxuICAgIFVOSUZJRURfUkVWX0FMTCA9IFwiVU5JRklFRF9SRVZfQUxMXCIsXHJcbiAgICBQSE9ORV9CVVNfREEgPSBcIlBIT05FX0JVU19EQVwiLFxyXG4gICAgU0NPUkVfRE9OT1IgPSBcIlNDT1JFX0RPTk9SXCIsXHJcbiAgICBTQ09SRV9HUkVFTiA9IFwiU0NPUkVfR1JFRU5cIixcclxuICAgIFNDT1JFX1dFQUxUSCA9IFwiU0NPUkVfV0VBTFRIXCIsXHJcbiAgICBFTUFJTF9CQVNJQ19SRVYgPSBcIkVNQUlMX0JBU0lDX1JFVlwiLFxyXG4gICAgUEhPTkVfUkVWX1BSRU0gPSBcIlBIT05FX1JFVl9QUkVNXCIsXHJcbiAgfVxyXG5cclxuICBlbnVtIFN1cHBvcnRlZFByb2R1Y3RDYXRlZ29yeSB7XHJcbiAgICBBcHBlbmQsXHJcbiAgICBFbmhhbmNlLFxyXG4gICAgU2NvcmUsXHJcbiAgICBWZXJpZnksXHJcbiAgICBQaG9uZSxcclxuICAgIEVtYWlsXHJcbiAgfVxyXG5cclxuICBmdW5jdGlvbiBkaXNwbGF5TWVzc2FnZShtZXNzYWdlOiBhbnksIHR5cGU6IGFueSk7XHJcbiAgZnVuY3Rpb24gZGlzcGxheU1lc3NhZ2UobWVzc2FnZSwgdHlwZSkge1xyXG4gICAgICAkKFwiI2FsZXJ0XCIpLnJlbW92ZUNsYXNzKCkuYWRkQ2xhc3MoYGFsZXJ0IGFsZXJ0LSR7dHlwZX1gKS5odG1sKG1lc3NhZ2UpLnNob3coKVxyXG4gICAgICAgICAgLmZhZGVUbyg1MDAwLCA1MDApLnNsaWRlVXAoNTAwLCAoKSA9PiB7ICQoXCIjYWxlcnRcIikuc2xpZGVVcCg1MDApIH0pO1xyXG4gIH1cclxuXHJcbn1cclxuXHJcbiJdfQ==