$(function () {
    $("#submit").click(function () {
        $("form").submit();
    });
});
var Sales;
(function (Sales) {
    var Confirmation;
    (function (Confirmation) {
        var ViewModel = (function () {
            function ViewModel(cartId, links) {
                this.cartId = cartId;
                this.links = links;
                this.products = ko.observableArray([]);
                this.minimum = ko.observable(0);
            }
            ViewModel.prototype.adjustedTotal = function () {
                var ot = this.products().reduce(function (a, b) { return a + b.subtotal; }, 0);
                return ot > this.minimum() ? ot : this.minimum();
            };
            ViewModel.prototype.total = function () {
                return this.products().reduce(function (a, b) { return a + b.subtotal; }, 0);
            };
            ViewModel.prototype.load = function () {
                var _this = this;
                $.ajax({
                    type: "GET",
                    url: this.links.dataUrl,
                    data: { cartId: this.cartId },
                    success: function (data) {
                        $.each(data.Products, function (i, v) {
                            _this.products.push(new Product(v.Product, v.Description, v.Price, v.EstimatedMatches));
                        });
                        _this.minimum(data.OrderMinimum);
                    }
                });
                $.get(this.links.walletUrl, function (data) {
                    if (!data.Primary)
                        $("#paymentForm").show();
                });
            };
            return ViewModel;
        }());
        Confirmation.ViewModel = ViewModel;
        var Product = (function () {
            function Product(product, description, price, estimatedMatches) {
                this.product = product;
                this.description = description;
                this.price = price;
                this.estimatedMatches = estimatedMatches;
                this.subtotal = this.estimatedMatches * this.price;
            }
            return Product;
        }());
        Confirmation.Product = Product;
    })(Confirmation = Sales.Confirmation || (Sales.Confirmation = {}));
})(Sales || (Sales = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQ29uZmlybWF0aW9uLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiQ29uZmlybWF0aW9uLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUdBLENBQUMsQ0FBQztJQUNBLENBQUMsQ0FBQyxTQUFTLENBQUMsQ0FBQyxLQUFLLENBQUM7UUFDakIsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDO0lBQ3JCLENBQUMsQ0FBQyxDQUFDO0FBQ0wsQ0FBQyxDQUFDLENBQUM7QUFFSCxJQUFPLEtBQUssQ0EyRFg7QUEzREQsV0FBTyxLQUFLO0lBQUMsSUFBQSxZQUFZLENBMkR4QjtJQTNEWSxXQUFBLFlBQVk7UUFFdkI7WUFRRSxtQkFBWSxNQUFXLEVBQUUsS0FBVTtnQkFDakMsSUFBSSxDQUFDLE1BQU0sR0FBRyxNQUFNLENBQUM7Z0JBQ3JCLElBQUksQ0FBQyxLQUFLLEdBQUcsS0FBSyxDQUFDO2dCQUNuQixJQUFJLENBQUMsUUFBUSxHQUFHLEVBQUUsQ0FBQyxlQUFlLENBQUMsRUFBRSxDQUFDLENBQUM7Z0JBQ3ZDLElBQUksQ0FBQyxPQUFPLEdBQUcsRUFBRSxDQUFDLFVBQVUsQ0FBQyxDQUFDLENBQUMsQ0FBQztZQUNsQyxDQUFDO1lBRUQsaUNBQWEsR0FBYjtnQkFDRSxJQUFNLEVBQUUsR0FBRyxJQUFJLENBQUMsUUFBUSxFQUFFLENBQUMsTUFBTSxDQUFDLFVBQUMsQ0FBQyxFQUFFLENBQUMsSUFBSyxPQUFBLENBQUMsR0FBRyxDQUFDLENBQUMsUUFBUSxFQUFkLENBQWMsRUFBRSxDQUFDLENBQUMsQ0FBQztnQkFDL0QsT0FBTyxFQUFFLEdBQUcsSUFBSSxDQUFDLE9BQU8sRUFBRSxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxPQUFPLEVBQUUsQ0FBQztZQUNuRCxDQUFDO1lBRUQseUJBQUssR0FBTDtnQkFDRSxPQUFPLElBQUksQ0FBQyxRQUFRLEVBQUUsQ0FBQyxNQUFNLENBQUMsVUFBQyxDQUFDLEVBQUUsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxHQUFHLENBQUMsQ0FBQyxRQUFRLEVBQWQsQ0FBYyxFQUFFLENBQUMsQ0FBQyxDQUFDO1lBQzdELENBQUM7WUFFRCx3QkFBSSxHQUFKO2dCQUFBLGlCQW1CQztnQkFsQkMsQ0FBQyxDQUFDLElBQUksQ0FDSjtvQkFDRSxJQUFJLEVBQUUsS0FBSztvQkFDWCxHQUFHLEVBQUUsSUFBSSxDQUFDLEtBQUssQ0FBQyxPQUFPO29CQUN2QixJQUFJLEVBQUUsRUFBRSxNQUFNLEVBQUUsSUFBSSxDQUFDLE1BQU0sRUFBRTtvQkFDN0IsT0FBTyxFQUFFLFVBQUEsSUFBSTt3QkFDWCxDQUFDLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxRQUFRLEVBQ2xCLFVBQUMsQ0FBQyxFQUFFLENBQUM7NEJBQ0gsS0FBSSxDQUFDLFFBQVEsQ0FBQyxJQUFJLENBQUMsSUFBSSxPQUFPLENBQUMsQ0FBQyxDQUFDLE9BQU8sRUFBRSxDQUFDLENBQUMsV0FBVyxFQUFFLENBQUMsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxDQUFDLGdCQUFnQixDQUFDLENBQUMsQ0FBQzt3QkFDekYsQ0FBQyxDQUFDLENBQUM7d0JBQ0wsS0FBSSxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsWUFBWSxDQUFDLENBQUM7b0JBQ2xDLENBQUM7aUJBQ0YsQ0FBQyxDQUFDO2dCQUVMLENBQUMsQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxTQUFTLEVBQ3hCLFVBQUEsSUFBSTtvQkFDRixJQUFJLENBQUMsSUFBSSxDQUFDLE9BQU87d0JBQUUsQ0FBQyxDQUFDLGNBQWMsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO2dCQUM5QyxDQUFDLENBQUMsQ0FBQztZQUNQLENBQUM7WUFFSCxnQkFBQztRQUFELENBQUMsQUE3Q0QsSUE2Q0M7UUE3Q1ksc0JBQVMsWUE2Q3JCLENBQUE7UUFFRDtZQUdFLGlCQUFtQixPQUFPLEVBQ2pCLFdBQW1CLEVBQ25CLEtBQWEsRUFDYixnQkFBd0I7Z0JBSGQsWUFBTyxHQUFQLE9BQU8sQ0FBQTtnQkFDakIsZ0JBQVcsR0FBWCxXQUFXLENBQVE7Z0JBQ25CLFVBQUssR0FBTCxLQUFLLENBQVE7Z0JBQ2IscUJBQWdCLEdBQWhCLGdCQUFnQixDQUFRO2dCQUMvQixJQUFJLENBQUMsUUFBUSxHQUFHLElBQUksQ0FBQyxnQkFBZ0IsR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDO1lBQ3JELENBQUM7WUFDSCxjQUFDO1FBQUQsQ0FBQyxBQVRELElBU0M7UUFUWSxvQkFBTyxVQVNuQixDQUFBO0lBQ0gsQ0FBQyxFQTNEWSxZQUFZLEdBQVosa0JBQVksS0FBWixrQkFBWSxRQTJEeEI7QUFBRCxDQUFDLEVBM0RNLEtBQUssS0FBTCxLQUFLLFFBMkRYIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL1NjcmlwdHMvdHlwaW5ncy9rZW5kby11aS9rZW5kby11aS5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rbm9ja291dC9rbm9ja291dC5kLnRzXCIgLz5cclxuXHJcbiQoKCkgPT4ge1xyXG4gICQoXCIjc3VibWl0XCIpLmNsaWNrKCgpID0+IHtcclxuICAgICQoXCJmb3JtXCIpLnN1Ym1pdCgpO1xyXG4gIH0pO1xyXG59KTtcclxuXHJcbm1vZHVsZSBTYWxlcy5Db25maXJtYXRpb24ge1xyXG5cclxuICBleHBvcnQgY2xhc3MgVmlld01vZGVsIHtcclxuXHJcbiAgICBwcm9kdWN0czogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8UHJvZHVjdD47XHJcbiAgICBtaW5pbXVtOiBLbm9ja291dE9ic2VydmFibGU8bnVtYmVyPjtcclxuICAgIGNhcnRJZDogYW55O1xyXG4gICAgbGlua3M6IGFueTtcclxuXHJcblxyXG4gICAgY29uc3RydWN0b3IoY2FydElkOiBhbnksIGxpbmtzOiBhbnkpIHtcclxuICAgICAgdGhpcy5jYXJ0SWQgPSBjYXJ0SWQ7XHJcbiAgICAgIHRoaXMubGlua3MgPSBsaW5rcztcclxuICAgICAgdGhpcy5wcm9kdWN0cyA9IGtvLm9ic2VydmFibGVBcnJheShbXSk7XHJcbiAgICAgIHRoaXMubWluaW11bSA9IGtvLm9ic2VydmFibGUoMCk7XHJcbiAgICB9XHJcblxyXG4gICAgYWRqdXN0ZWRUb3RhbCgpOiBhbnkge1xyXG4gICAgICBjb25zdCBvdCA9IHRoaXMucHJvZHVjdHMoKS5yZWR1Y2UoKGEsIGIpID0+IGEgKyBiLnN1YnRvdGFsLCAwKTtcclxuICAgICAgcmV0dXJuIG90ID4gdGhpcy5taW5pbXVtKCkgPyBvdCA6IHRoaXMubWluaW11bSgpO1xyXG4gICAgfVxyXG5cclxuICAgIHRvdGFsKCk6IGFueSB7XHJcbiAgICAgIHJldHVybiB0aGlzLnByb2R1Y3RzKCkucmVkdWNlKChhLCBiKSA9PiBhICsgYi5zdWJ0b3RhbCwgMCk7XHJcbiAgICB9XHJcblxyXG4gICAgbG9hZCgpIHtcclxuICAgICAgJC5hamF4KFxyXG4gICAgICAgIHtcclxuICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICB1cmw6IHRoaXMubGlua3MuZGF0YVVybCxcclxuICAgICAgICAgIGRhdGE6IHsgY2FydElkOiB0aGlzLmNhcnRJZCB9LFxyXG4gICAgICAgICAgc3VjY2VzczogZGF0YSA9PiB7XHJcbiAgICAgICAgICAgICQuZWFjaChkYXRhLlByb2R1Y3RzLFxyXG4gICAgICAgICAgICAgIChpLCB2KSA9PiB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLnByb2R1Y3RzLnB1c2gobmV3IFByb2R1Y3Qodi5Qcm9kdWN0LCB2LkRlc2NyaXB0aW9uLCB2LlByaWNlLCB2LkVzdGltYXRlZE1hdGNoZXMpKTtcclxuICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgdGhpcy5taW5pbXVtKGRhdGEuT3JkZXJNaW5pbXVtKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgICAgLy8gZGlzcGxheSBwYXltZW50IGZvcm0gaWYgbm8gcHJpbWFyeSBmb3JtIGlzIHByZXNlbnQuIFxyXG4gICAgICAkLmdldCh0aGlzLmxpbmtzLndhbGxldFVybCxcclxuICAgICAgICBkYXRhID0+IHtcclxuICAgICAgICAgIGlmICghZGF0YS5QcmltYXJ5KSAkKFwiI3BheW1lbnRGb3JtXCIpLnNob3coKTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgfVxyXG5cclxuICBleHBvcnQgY2xhc3MgUHJvZHVjdCB7XHJcbiAgICBzdWJ0b3RhbDogbnVtYmVyO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHB1YmxpYyBwcm9kdWN0LFxyXG4gICAgICBwdWJsaWMgZGVzY3JpcHRpb246IHN0cmluZyxcclxuICAgICAgcHVibGljIHByaWNlOiBudW1iZXIsXHJcbiAgICAgIHB1YmxpYyBlc3RpbWF0ZWRNYXRjaGVzOiBudW1iZXIpIHtcclxuICAgICAgdGhpcy5zdWJ0b3RhbCA9IHRoaXMuZXN0aW1hdGVkTWF0Y2hlcyAqIHRoaXMucHJpY2U7XHJcbiAgICB9XHJcbiAgfVxyXG59Il19