$(function () {
    var handCount = 0;
    $("#addHand").click(function () {
        if (handCount < 10) {
            handCount++;
            var html =  "<div class='form-group handInput'>"
                    + "<label for='SetHands' class='control-label col-md-3'>Hand #" + handCount + ":  </label>"
                    + "<input type='number' class='col-md-4 cardInputBox text-box single-line form-control' name='SetHands' />&nbsp"
                    + "<span class='col-md-1'></span>"
                    + "<input type='number' class='col-md-4 cardInputBox text-box single-line form-control' name='SetHands' /></div>";
            $("#handsForm").append(html);
        }
    });
    $("#addHand").trigger("click");

    $("#removeHand").click(function () {
        if (handCount > 1) {
            handCount--;
            $(".handInput:last").remove();
        }
    });
});