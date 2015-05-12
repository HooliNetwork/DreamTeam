

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

    $(".create-post-button").click(function(){
        $(".new-post-container").toggleClass('open');
    });
    
    // Search results filter
    $(".search-filters button").click(function() {
        $(this).siblings().not(".btn-option").toggleClass("btn-option");
        $(this).toggleClass("btn-option");
        var filter = $(this).attr("name");

        if(filter === "s_all") {
            $(".search-results").show(100);
        } else {
            //$(".search-results").hide(100)
            $(".search-results").not('.' + filter).hide(100);            
            $('.' + filter).show(100);
        }
    })
    
    $(function () {
      $('[data-toggle="tooltip"]').tooltip()
    })
    
});


