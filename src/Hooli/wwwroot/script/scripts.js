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
    
    $(".search-filters button").click(function() {
        $(this).siblings().not(".btn-option").toggleClass("btn-option");
        $(this).toggleClass("btn-option");
        var filter = $(this).attr("name");

        if(filter === "s_all") {
            $(".search-results").show(100);
        } else {
            $(".search-results").not('.' + filter).hide(100);            
            $('.' + filter).show(100);
        }
    })

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    })
    
    $(".create-group-button").click(function () {
        $(".edit-info-container").toggleClass('open');
    });


    // Search results filter
    $(".search-filters button").click(function () {
        $(this).siblings().not(".btn-option").toggleClass("btn-option");
        $(this).toggleClass("btn-option");
        var filter = $(this).attr("name");

        if (filter === "s_all") {
            $(".search-results").show(100);
        } else {
            $(".search-results").not('.' + filter).hide(100);
            $('.' + filter).show(100);
        }
    })

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
    
    $('.image-link').magnificPopup({type:'image'});
    
    $('.popup-link').magnificPopup({ 
        type: 'image'
        // other options
    });


    $("body").on('click', ".btn-follow", function () {
        var currBtn = $(this);
        var btnText = $("span", this);
        var btnType = btnText.text();
        var uri = currBtn.attr('data-url');
        var id = currBtn.attr('data-id');
        console.log(uri + id);
        $.ajax({
            async: false,
            type: "POST",
            url: uri,
            data: {
                'Id': id
            }
        }).success(function (result) {
            var uri = currBtn.attr("data-url");
            if (btnType === "Follow") {
                btnText.text("Unfollow");
                var newAttr = (uri.slice(0, 8) + "Unf" + uri.slice(9));
                currBtn.attr("data-url", newAttr);
            } else if (btnType === "Unfollow") {
                btnText.text("Follow");
                var newAttr = (uri.slice(0, 8) + "F" + uri.slice(11));
                currBtn.attr("data-url", newAttr);
            }
            currBtn.toggleClass("btn-option");
        }).fail(function (error) {
            alert("There was an error posting the data to the server: " + error.responseText);
        });
        return false;
    });

    $(".file-upload").change(function(){
        var FileName = $(this).val().slice(12);
        var FileNameLength = FileName.length;
        if (FileNameLength > 27) {
            $(".file-upload-name").val(FileName.slice(0,16) + "..." + FileName.slice(FileNameLength - 8));
        } else {
            $(".file-upload-name").val(FileName);
        }
    })


    $("body").on('click', ".vote", function () {
        var currBtn = $(this);
        var uri = currBtn.parent().attr('data-url');
        var id = currBtn.parent().attr('data-postId');
        var upOrDown = currBtn.attr('data-type');
        var count = currBtn.siblings(".post-rating-count");
        $.ajax({
            async: true,
            type: "POST",
            url: uri,
            data: { 'upDown': upOrDown, 'postId': id },
            success: function (result) {
                var value = parseInt(count.text(), 10);
                    if (upOrDown == "up") {
                        value += 1;
                        count.text(value);
                    } else if (upOrDown == "down") {
                        value -= 1;
                        count.text(value);
            }
            }
        });
    });
    $(function () {
        $('#personButton').click(function () {
            var currBtn = $(this);
            var data = {
                PostId: currBtn.attr('data-PostId'),
                Address: currBtn.attr('data-PostId'),
                Text: currBtn.attr('data-PostId'),
                UserName: currBtn.attr('data-PostId'),
                UserId: currBtn.attr('data-PostId'),
                DateCreate: $.now(),
                ParentId: currBtn.attr('data-PostId')
                };

            $.post('@Url.Action("IsValidPerson", "FormUrlEncoded")', MVC.stringify(data))
                .done(function (result) {
                    if (result) {
                        $("#validPerson").html("The submitted data belongs to a valid person.");
                    }
                    else {
                        $("#validPerson").html("The submitted data is invalid.");
                    }
                });
        });
    });
    
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-left",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    
});