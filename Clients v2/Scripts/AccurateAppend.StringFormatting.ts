module AccurateAppend {

  export class FormatString {

    static formatCurrency(value, decimalPlaces) {
      const formatter = new Intl.NumberFormat("en-US",
        {
          style: "currency",
          currency: "USD",
          minimumFractionDigits: decimalPlaces
        });
      return formatter.format(value);
    }

    static formatNumber(value) {
      const formatter = new Intl.NumberFormat("en-US", { maximumSignificantDigits: 3 });
      return formatter.format(value);
    }

    static formatDate(value) {
      value = new Date(Date.parse(value));

      const options = {
        year: "numeric",
        month: "numeric",
        day: "numeric",
        timeZone: "America/Los_Angeles"
      };

      const formatter = new Intl.DateTimeFormat("en-US", options);
      return formatter.format(value);
    }

    static formatDateTime(date) {
      date = new Date(Date.parse(date));

      const options = {
        year: "numeric",
        month: "numeric",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
        timeZone: "America/Los_Angeles"
      };

      const formatter = new Intl.DateTimeFormat("en-US", options);
      return formatter.format(date);
    }
  }

}