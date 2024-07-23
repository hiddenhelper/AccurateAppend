let newClientRegistrationView: AccurateAppend.Websites.Clients.Public.NewClientRegistrationView;

module AccurateAppend.Websites.Clients.Public {
    export interface INewClientRegistrationViewModel {
        Model: any,
        Canada: string,
        UnitedStates: string
    }

    export class NewClientRegistrationView {
        ViewModel: INewClientRegistrationViewModel;

        initialize(viewModel: INewClientRegistrationViewModel) {
            this.ViewModel = viewModel;

            $('#ckUseBillingAddress').change(function () {
                if ($(this).is(":checked")) {
                    $("#CardHolderFirstName").val($("#FirstName").val());
                    $("#CardHolderLastName").val($("#LastName").val());
                    $("#CardAddress").val($("#Address").val());
                    $("#CardCity").val($("#City").val());
                    $("#CardStateAbbreviation").val($("#StateAbbreviation").val());
                    $("#CardStatePlainText").val($("#StatePlainText").val());
                    $("#CardCountry").val($("#Country").val());
                    $("#CardPostalCode").val($("#PostalCode").val());
                    $("#CardHolderPhone").val($("#Phone").val());
                    $("#CardNumber").focus();
                } else {
                    $("#CardHolderFirstName").val("");
                    $("#CardHolderLastName").val("");
                    $("#CardAddress").val("");
                    $("#CardCity").val("");
                    $("#CardPostalCode").val("");
                    $("#CardStateAbbreviation").val("");
                    $("#CardStatePlainText").val("");
                    $("#CardCountry").val("");
                }
            });

            $("#lnkTerms").click(function () {
                $("#tos-modal").modal('show');
            });

            //card country toggling based on north america or not

            if (viewModel.Model.CardCountry == viewModel.UnitedStates || viewModel.Model.CardCountry == viewModel.Canada || viewModel.Model.CardCountry == null) {
                $("input[name=CardStatePlainText]").hide();
                $("select[name=CardStateAbbreviation]").next().show();
            }
            else {
                $("input[name=CardStatePlainText]").show();
                $("select[name=CardStateAbbreviation]").next().hide();
            }

            $("select[name=CardCountry]").change(function () {
                if ($(this).val() == viewModel.UnitedStates || $(this).val() == viewModel.Canada) {
                    $("input[name=CardStatePlainText]").hide();
                    $("select[name=CardStateAbbreviation]").val('');
                    $("select[name=CardStateAbbreviation]").next().show();
                } else {
                    $("input[name=CardStatePlainText]").show();
                    $("select[name=CardStateAbbreviation]").next().hide();
                }
            });


            //personal country toggling based on north america or not

            if (viewModel.Model.Country == viewModel.UnitedStates || viewModel.Model.Country == viewModel.Canada || viewModel.Model.Country == null) {
                $("input[name=StatePlainText]").hide();
                $("select[name=StateAbbreviation]").next().show();
            }
            else {
                $("input[name=StatePlainText]").show();
                $("select[name=StateAbbreviation]").next().hide();
            }

            $("select[name=Country]").change(function () {
                if ($(this).val() == viewModel.UnitedStates || $(this).val() == viewModel.Canada) {
                    $("input[name=StatePlainText]").hide();
                    $("select[name=StateAbbreviation]").val('');
                    $("select[name=StateAbbreviation]").next().show();
                } else {
                    $("input[name=StatePlainText]").show();
                    $("select[name=StateAbbreviation]").next().hide();
                }
            });
        }
    }
}




$(document).ready(function () {
    (new AccurateAppend.Websites.Clients.Public.NewClientRegistrationView()).initialize(viewModel);
});




