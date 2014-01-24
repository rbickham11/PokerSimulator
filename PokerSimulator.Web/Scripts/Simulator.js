$(function () {
    var handCount = 1;
    $("#addHand").click(function () {
        handCount++;
        var html = "<label for='SetHands' class='control-label col-md-5'>Hand #" + handCount + ": </label>"
            + "<input style='width:100px;' type='number' class='text-box single-line' name='SetHands' />"
            + "<input style='width:100px;' type='number' class='text-box single-line' name='SetHands' /><br />";
        $("#hands_form").append(html);
    });
});