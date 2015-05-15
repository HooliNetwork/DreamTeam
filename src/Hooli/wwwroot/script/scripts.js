$(document).ready(function () {
    $(".feed-control button").click(function(){
        var button = $(this);
        var feedControl = $(".feed-control");
        if(button.hasClass("btn-option")) {
            button.parent().children().eq(0).toggleClass("btn-option");
            button.parent().children().eq(1).toggleClass("btn-option");
            var fPeople = !$("span:contains('People')", feedControl).parent().hasClass("btn-option");
            var fGroups = !$("span:contains('Groups')", feedControl).parent().hasClass("btn-option");
            var fNew = !$("span:contains('New')", feedControl).parent().hasClass("btn-option");
            var fTop = !$("span:contains('Top')", feedControl).parent().hasClass("btn-option");
            if (fPeople && fNew) {
                console.log("New in People");
                filterFeedAjax(false,true);
            } else if (fPeople && fTop) {
                console.log("Top in People");
                filterFeedAjax(false,false);
            } else if (fGroups && fNew) {
                console.log("New in Groups");
                filterFeedAjax(true,true);
            } else {
                console.log("Top in Groups");
                filterFeedAjax(true,false);
            }
        }
    });
    
    var filterFeedAjax = function (filter1, filter2) {
        var uri = "/Home/Sort/";
        var groupId = "Front";
        $.ajax({
            async: true,
            type: "POST",
            url: uri,
            data: {'group': filter1, 'latestPosts': filter2, 'groupId': groupId},
            success: function (result) {
                filterReplaceFeed(result);
            }  
        });
    };
    
    var filterReplaceFeed = function (html) {
        console.log("Inside HTML replacer");
        $('#feed-content').empty().append(html);
    };

    $(".create-post-button").click(function(){
        $(".new-post-container").toggleClass('open');
    });
    
     $(".create-group-button").click(function(){
        $(".edit-info-container").toggleClass('open');
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
    
    $(".edit-post-button").click(function () {
        $(".edit-info-container").toggleClass('open');
        $(".post-information").toggleClass('open');
    });

    $(".comment-body button").click(function () {
        var parent = $(this).parent();
        $(".reply-to-comment", parent).toggleClass('open');
    });

    $("#button-comment-to-post").click(function () {
        $(".reply-to-post").toggleClass('open');
    });

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
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });
    
    $('.image-link').magnificPopup({type:'image'});
    
    $('body').on('submit', '#profile_change_password', function() {
       var theForm = $(this);
       $.ajax({
           type: "POST",
           url: "/Manage/ChangePassword/",
           data: theForm.serialize(),
       }).done(function (result) {
           $('.edit-info-container').toggleClass('open');
           toastr["success"]("Password updated!");
       }).fail(function (error) {
           alert("There was an error posting data to the server");
       });
       return false;
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
    }; 
});
