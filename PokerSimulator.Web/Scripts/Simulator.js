$(function () {
    var handCount = 0;
    $("#addHand").click(function () {
        if (handCount < 10) {
            handCount++;
            var html =  "<div class='handInput'>"
                    + "<label for='SetHands' class='control-label col-md-5'>Hand #" + handCount + ":</label>"
                    + "<input style='width:100px;' type='number' class='text-box single-line' name='SetHands' />"
                    + "<input style='width:100px;' type='number' class='text-box single-line' name='SetHands' /></div>";
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