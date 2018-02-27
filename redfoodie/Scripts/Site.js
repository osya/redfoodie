App = {
    placeAlert: function (c, a, e, d) {
        c = c || "default message";
        e = e || "alert";
        var b = {
            text: c,
            type: e,
            dismissQueue: true
        };
        b.timeout = e === "error" ? 5000 : 2000;
        if (typeof d !== "undefined" && d === true) {
            b.timeout = false;
        }
        if (a && a.length > 0) {
            b = {
                text: c,
                type: e,
                dismissQueue: true,
                closeWith: ["click"],
                template: '<div class="noty_message"><div class="noty_text"></div><div class="noty_close"></div></div>'
            };
            a.noty(b);
        } else {
            if (typeof noty !== "undefined") {
                noty(b);
            }
        }
    },
    logging: true,
    log: function (a) {
        if (typeof console !== "undefined" && utils.logging) {
            console.log(a);
        }
    },
    postLoadFormatData: function () {
        $(".added_on_timestamp").timeago();
        $(".sc-markdown").markify();
    },
    page: {},
    config: {},
    starRating: function (b) {
        var c = "";
        for (var a = 1; a <= 5; a++) {
            if (a <= b) {
                c += '<span class="star-01 rated"></span>';
            } else {
                c += '<span class="star-01 "></span>';
            }
        }
        return c;
    },
    convertToSlug: function (a) {
        return a.toLowerCase().replace(/[^\w ]+/g, "").replace(/ +/g, "-");
    }
};
//const a = jQuery;
//(function (a) {
jQuery.fn.loadingIcon = function (c) {
    var b = this;
    c = c === "attach" ? "attach" : "detach";
    b.each(function () {
        if (c === "attach") {
            a("body").css("cursor", "progress");
            var e = a(this).width();
            var d = a(this).height();
            a(this).children().hide();
            a(this).append('<span class="icon-loading2"></span>');
            a("span.icon-loading2", this).css("width", e + "px");
            a("span.icon-loading2", this).css("height", d + "px");
        } else {
            a("body").css("cursor", "default");
            a(this).find("span.icon-loading2").remove();
            a(this).children().show();
        }
    });
    return b;
};
//})(jQuery);
$(document).ready(function () {
    $(document).on("click", "[data-modal-link=1]", function (f) {
        f.preventDefault();
        var $el = $(f.currentTarget).closest("[data-modal-link=1]");
        var c = $(".pop_wrapper"),
            b;
        if ("none" === c.css("display")) {
            c.css("top", "200px").show().css("opacity", 0);
            $("#ispg_appbundle_messagestype_sentTo").chosen();
            $(".pop_outer_wrapper").show();
            c.css("top", "150px");
            c.css("opacity", 1);
            c.css("z-index", 10000);
            b = $(".pop_wrapper");
        } else {
            b = $(".pop_wrapper");
        }
        $("div[data-modal-content-container=1]", b).css("display", "none");
        var d = $el.data("target");
        if (!d) {
            d = $el.attr("href");
        }
        var a = $(d, b);
        $("form .text-danger", a).empty();
        a.triggerHandler("popup.show", $el);
        a.attr("style", "display: block !important");
        a.css("display", "block !important");
        a.triggerHandler("popup.shown", $el);
        $("#pp_desc", b).removeClass().addClass(a.attr("data-modal-wrapper-class"));
        return false;
    });
    $(document)
        .on("click",
            ".modal-popup-close,.pop_outer_wrapper",
            function () {
                $(".pop_wrapper").hide().css("opacity", 1);
                $(".pop_outer_wrapper").hide();
            });
});
jQuery.fn.highlight = function (b) {
    function a(e, j) {
        var l = 0;
        if (e.nodeType === 3) {
            var k = e.data.toUpperCase().indexOf(j);
            if (k >= 0) {
                var h = document.createElement("span");
                h.className = "highlight";
                var f = e.splitText(k);
                var d = f.cloneNode(true);
                h.appendChild(d);
                f.parentNode.replaceChild(h, f);
                l = 1;
            }
        } else {
            if (e.nodeType === 1 && e.childNodes && !/(script|style)/i.test(e.tagName)) {
                for (var g = 0; g < e.childNodes.length; ++g) {
                    g += a(e.childNodes[g], j);
                }
            }
        }
        return l;
    }

    return this.length && b && b.length ?
        this.each(function () {
            a(this, b.toUpperCase());
        }) :
        this;
};