module AccurateAppend {

  export class Util {

    static displayMessage(elementId, message, type) {
      $(`#${elementId}`).removeClass().addClass(`alert alert-${type}`).html(message).show()
        .fadeTo(5000, 500).slideUp(500, () => { $("#globalMessage").slideUp(500) });
    }

  }

}