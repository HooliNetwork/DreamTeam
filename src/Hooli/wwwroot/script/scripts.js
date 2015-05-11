

$(document).ready(function () {
    //TODO: add a check so that if a person clicks a previously selected button classes aren't added
    $(".posts-type button:nth-child(1)").click(function () {
        $(".posts-type button:nth-child(1)").removeClass("btn-option").addClass("btn-selected");
        $(".posts-type button:nth-child(2)").removeClass("btn-selected").addClass("btn-option");
    });
    $(".posts-type button:nth-child(2)").click(function () {
        $(".posts-type button:nth-child(1)").removeClass("btn-selected").addClass("btn-option");
        $(".posts-type button:nth-child(2)").removeClass("btn-option").addClass("btn-selected");
    });
    $(".posts-orderby button:nth-child(1)").click(function() {
        $(".posts-orderby button:nth-child(1)").removeClass("btn-option").addClass("btn-selected");
        $(".posts-orderby button:nth-child(2)").removeClass("btn-selected").addClass("btn-option");
    });
    $(".posts-orderby button:nth-child(2)").click(function () {
        $(".posts-orderby button:nth-child(1)").removeClass("btn-selected").addClass("btn-option");
        $(".posts-orderby button:nth-child(2)").removeClass("btn-option").addClass("btn-selected");
    });

    
});