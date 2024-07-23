declare let CheckUrl: string;
declare let NextUrl: string;

var timer;
$(() => {
  console.log("Rendering wait for @this.Model.OrderId");

  timer = window.setInterval("check()", 1000);
});

function check() {
    $.ajax(
        {
            type: "GET",
            url: CheckUrl,
            success(result) {
              if (result.Ready) {
                if (timer) {
                  window.clearInterval(timer);
                }
                window.location.href = NextUrl;
              }
            }
        });
}