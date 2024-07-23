module AccurateAppend.Websites.Clients.Shared {

    //message type enumeration for notification
    enum MessageType
    {
        Error,
        Info,
        Warning,
        Success
    }

    //this class allows ts scripts to embed notification element to their pages easily
    export class NotificationHelper
    {
        targetElement: JQuery;

        //TODO: a nice to have feature to make notification dismissable by users by adding a small close button
        //dismisable: boolean;

        //initialize using the element id
        //pass element id with or without the # sign
        constructor(targetElementName: string) {
            if (targetElementName.indexOf('#') != 0)
                targetElementName = '#' + targetElementName;

            this.targetElement = $(targetElementName);
            this.targetElement.hide();
        }

        //clears the content of the notification and hides the element
        clearMessage() {
            this.targetElement.hide();
            this.targetElement.html('');
            this.targetElement.removeClass();
        }

        //displays the message styled as the message type
        showMessage(message: string, messageType = MessageType.Info)
        {
            this.targetElement.removeClass();
            this.targetElement.addClass('alert');
            this.targetElement.html(message);
            this.targetElement.addClass(this.getMessageCss(messageType));
            this.targetElement.show();
        }

        showError(message: string) {
            this.showMessage(message, MessageType.Error);
        }

        showInfo(message: string) {
            this.showMessage(message, MessageType.Info);
        }

        showWarning(message: string) {
            this.showMessage(message, MessageType.Warning);
        }

        showSuccess(message: string) {
            this.showMessage(message, MessageType.Success);
        }

        getMessageCss(messageType: MessageType) {
            switch (messageType) {
                case MessageType.Error:
                    return 'alert-danger';
                case MessageType.Info:
                    return 'alert-info';
                case MessageType.Success:
                    return 'alert-success';
                case MessageType.Warning:
                    return 'alert-warning';
                default:
                    return 'alert-info';
            }
        }
    }
}