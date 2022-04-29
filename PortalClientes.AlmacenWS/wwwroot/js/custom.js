(function ($) {
    $.fn.extend({
        size: function() {
            return $(this).length;
        }
    });
})(jQuery);

$(function () {
    new WOW().init();

    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();
});

function getObjects(objec, key, val) {
    var objects = [];
    for (var i in objec) {
        if (!objec.hasOwnProperty(i)) continue;
        if (typeof objec[i] == 'object') {
            objects = objects.concat(getObjects(objec[i], key, val));
        } else if (i == key && objec[key] == val) {
            objects.push(objec);
        }
    }

    if (objects.length == 1) {
        return $.parseJSON(JSON.stringify(objects[0]));
    } else {
        return $.parseJSON(JSON.stringify(objects));
    }
}

function objAnimated(objDOM, animatedClass, duration) {
    duration = parseInt(duration) || 700;

    $(objDOM).addClass("animated");
    $(objDOM).addClass(animatedClass);

    setTimeout(function () {
        $(objDOM).removeClass("animated");
        $(objDOM).removeClass(animatedClass);
    }, duration);
}
