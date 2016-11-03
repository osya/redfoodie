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
        if (typeof d != "undefined" && d === true) {
            b.timeout = false;
        }
        if (a != null && a.length > 0) {
            b = {
                text: c,
                type: e,
                dismissQueue: true,
                closeWith: ["click"],
                template: '<div class="noty_message"><div class="noty_text"></div><div class="noty_close"></div></div>'
            };
            a.noty(b);
        } else {
            if (typeof noty != "undefined") {
                noty(b);
            }
        }
    },
    logging: true,
    log: function (a) {
        if (typeof console != "undefined" && utils.logging) {
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

$(document).ready(function () {
    $(document).on("click", "[data-modal-link=1]", function (f) {
        f.preventDefault();
        $el = $(f.currentTarget).closest("[data-modal-link=1]");
        var c = $(".pop_wrapper")
          , b = c.clone();
        if ("none" === c.css("display")) {
            c.css("top", "200px").show().css("opacity", 0);
            $(".pop_outer_wrapper").show();
            c.css("top", "150px");
            c.css("opacity", 1);
            c.css("z-index", 10000);
            b = $(".pop_wrapper");
        } else {
            $(".pop_wrapper:first").hide().css("opacity", 1);
            b.css("opacity", 0);
            c.after(b);
            b.css("top", "150px");
            b.css("opacity", 1);
            $(".pop_wrapper:first").remove();
        }
        $("div[data-modal-content-container=1]", b).css("display", "none");
        var d = $el.data("target");
        if (!d) {
            d = $el.attr("href");
        }
        var a = $(d, b);
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
