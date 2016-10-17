$(function () {
    var showModal = function () {
        var $input = $(this);
        var imgAlt = $input.attr("alt");
        $("#theModal h5.modal-title").html(imgAlt);
        var img = this;
        var theImage = new Image();
        theImage.src = $input.attr("src");
        var imgHgt = theImage.height;
        var imgWdt = theImage.width;
        //alert('w:' + imgWdt + ' h:' + imgHgt);
        var picSrc = $input.attr("src");
        $("#theModal img").attr('src', picSrc);
        $("div.modal-dialog").css("width", imgWdt);
        $("#theModal img").width(imgWdt);
        $("#theModal img").height(imgHgt);
        $("#theModal").modal("show");
    };
    var MyHtml = '<div id="theModal" class="modal fade">' +
            ' <div class="modal-dialog ">' +
                '<div class="modal-content">' +
                    ' <div class="modal-header">' +
                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                     '</div>' +
                    '<div class="modal-body">' +
                        '<img not-to-enlarge="true" class="img-responsive" + src=""alt="...">' +
                    '</div>' +
                 '</div>' +
             '</div>' +
         '</div>';
    $("div.body-content").append(MyHtml);
    $("img[not-to-enlarge!=true]").click(showModal);
    $("img[not-to-enlarge!=true]").css("cursor", "pointer");
});