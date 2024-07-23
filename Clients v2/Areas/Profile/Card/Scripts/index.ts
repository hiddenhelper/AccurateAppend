
interface Hub {
    id: string;
    state: any;
    start(options?: any, callback?: () => any): JQueryPromise<any>;
}

interface CallbackHub {
    client: CallbackHubClient;
}

interface CallbackHubClient {
    callbackComplete: () => void;
}

interface SignalR {
    callbackHub: CallbackHub;
    hub: Hub;
}

interface JQueryStatic {
    connection: SignalR;
}

module AccurateAppend.Websites.Clients.Profile.Card {

    export class IndexView {
        cardUrl: string;

        constructor(url: string) {
            this.cardUrl = url;
        }

        populateForm(data) {
            $("#cardHolderFirstName").val(data.Primary.BillTo.FirstName);
            $("#cardHolderLastName").val(data.Primary.BillTo.LastName);
            $("#cardHolderBusinessName").val(data.Primary.BillTo.BusinessName);
            $("#cardPostalCode").val(data.Primary.BillTo.PostalCode);
            $("#cardHolderPhone").val(data.Primary.BillTo.PhoneNumber);
            $("#cardNumber").attr("placeholder", data.DisplayValue);
            $("#cardExpirationMonth").val(data.Primary.Expiration.substring(0, 2)).change();
            $("#cardExpirationYear").val(data.Primary.Expiration.substring(3, 7)).change();
        }

        resetForm() {
            var submitButton = $("#submitBtn");
            submitButton.html("Update");
            submitButton.prop('disabled', false);
        }

        submitForm(el) {
            console.log("Submitting form");

            let self = this;
            var submitButton = $("#submitBtn");
            submitButton.prop('disabled', true);
            submitButton.html(submitButton.html() + ' <i class="fa fa-refresh fa-spin" style="font-size: 20px;"></i> ');

            $.post(
                self.cardUrl,
                $('form').serialize(),
                function (e) {
                    console.log("Form processed");;
                    if (!e.Success) {
                        var summary = $(".validation-summary-valid").find("ul");
                        summary.empty();
                        e.Errors.forEach(function (item) {
                            summary.append($("<li>").text(item.ErrorMessage));
                        });
                        self.resetForm();
                    } else {
                        $("#alert").addClass("alert alert-success").text("Your payment details have been updated.").show();
                    }
                });
            return false;
        }

        initialize(url: string) {
            let self = this;
            // Reference the auto-generated proxy for the hub.
            var channel = $.connection.callbackHub;
            // Create a function that the hub can call back to signal completion messages.
            channel.client.callbackComplete = function () {
                self.resetForm();
            };
            // Start the connection.
            $.connection.hub.start().done(function () {
                console.log("Connection established: " + $.connection.hub.id);
                $("#connectionId").val($.connection.hub.id);

                console.log("Loading data");
                $.ajax({
                    type: "GET",
                    url: url,
                    success(result) {
                        if (result.Primary != null) self.populateForm(result);
                    }
                });

                self.resetForm();
            });
        }

    }
}

let cardIndexView: AccurateAppend.Websites.Clients.Profile.Card.IndexView;

function initializeCardView(primaryCardUrl, cardUrl) {
    $(document).ready(() => {
        cardIndexView = new AccurateAppend.Websites.Clients.Profile.Card.IndexView(cardUrl);
        cardIndexView.initialize(primaryCardUrl);
    });
}