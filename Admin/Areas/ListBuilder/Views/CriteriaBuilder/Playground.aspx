<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Playground
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">        
    <script src="<%= Url.Content("~/scripts/knockout-3.4.2.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
        
<!-- This is a *view* - HTML markup that defines the appearance of your UI -->
<h2>Your seat reservations</h2>

<table>
    <thead><tr>
        <th>Passenger name</th><th>Meal</th><th>Surcharge</th><th></th>
    </tr></thead>
    
    <!-- Todo: Generate table body -->
    <tbody data-bind="foreach: seats">
        <tr>
            <td><input data-bind="value: name"></td>
            <td><select data-bind="options: $root.availableMeals, value: meal, optionsText: 'mealName'"></td>
            <td data-bind="text: formattedPrice"></td>
            <td><a href="#" data-bind="click: $root.removeSeat">Remove</a></td>
        </tr>  
    </tbody>    
</table>

<button data-bind="click: addSeat">Reserve another seat</button>

<h3 data-bind="visible: totalSurcharge() > 0">
    Total surcharge: $<span data-bind="text: totalSurcharge().toFixed(2)"></span>
</h3>


<script>
    // Class to represent a row in the seat reservations grid
    function SeatReservation(name, initialMeal) {
        var self = this;
        self.name = name;
        self.meal = ko.observable(initialMeal);

        self.formattedPrice = ko.computed(function () {
            var price = self.meal().price;
            return price ? "$" + price.toFixed(2) : "None";
        });
    }

    // Overall viewmodel for this screen, along with initial state
    function ReservationsViewModel() {
        var self = this;

        // Non-editable catalog data - would come from the server
        self.availableMeals = [
            { mealName: "Standard (sandwich)", price: 0 },
            { mealName: "Premium (lobster)", price: 34.95 },
            { mealName: "Ultimate (whole zebra)", price: 290 }
        ];

        // Editable data
        self.seats = ko.observableArray([
            new SeatReservation("Steve", self.availableMeals[1]),
            new SeatReservation("Bert", self.availableMeals[2]),
            new SeatReservation("Joe", self.availableMeals[2])
        ]);

        // Operations
        self.addSeat = function () {
            self.seats.push(new SeatReservation("", self.availableMeals[0]));
        }

        self.removeSeat = function (seat) {
            self.seats.remove(seat);
        }

        self.totalSurcharge = ko.computed(function () {
            var total = 0;
            for (var i = 0; i < self.seats().length; i++)
                total += self.seats()[i].meal().price;
            return total;
        });
    }

    ko.applyBindings(new ReservationsViewModel());
</script>  

</asp:Content>
