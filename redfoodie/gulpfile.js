// jshint esversion: 6
/// <binding Clean='clean' />
(function () {
    "use strict";

    let gulp = require("gulp"),
        less = require("gulp-less"),
        postcss = require("gulp-postcss"),
        autoprefixer = require("autoprefixer");

    gulp.task("less", function () {
        return gulp.src("Content/*.less")
            .pipe(less())
            .pipe(postcss([
                autoprefixer({
                    browsers: "> 1%, last 2 versions"
                })
            ]))
            .pipe(gulp.dest("Content/css"));
    });
}());