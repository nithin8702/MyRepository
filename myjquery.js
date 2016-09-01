(function () {

    $ = function (selector) {
        var elements = document.querySelectorAll(selector);
        var length = elements.length;
        for (var i = 0; i < length; i++) {
            this[i] = elements[i];
        }
        this.length = length;
        //return this;
        //return $;
    };
    $.extend = function (target, object) {
        for (var prop in object) {
            target[prop] = object[prop];
        }
        return target;
    };
    var isArrayLike = function (obj) {
        if (typeof obj.length === "number") {
            if (obj.length === 0) {
                return true;
            }
            else if (obj.length > 0) {
                return (obj.length - 1) in obj;
            }
        }
        return false;
    };


    $.extend($, {
        isArray: function (obj) {
            return Object.prototype.toString.call(obj) === "[object Array]";
        },
        each: function (collection, cb) {
            if (isArrayLike(collection)) {
                for (var i = 0; i < collection.length; i++) {
                    var value = collection[i];
                    cb.call(value, i, value);
                }
            }
            else {
                for (var prop in collection) {
                    if (collection.hasOwnProperty(prop)) {
                        var value = collection[prop];
                        cb.call(value, prop, value);
                    }
                }
            }
            return collection;
        },
        proxy: function (fn, context) {
            return function () {
                fn.apply(context, arguments);
            };
        }

    });
    $.extend($.prototype, {
        html: function (newHTML) {
            if (arguments.length) {
                $.each(this, function (i, ele) {
                    ele.innerHTML = newHTML;
                });
                return this;
            }
            else {
                return this[0].innerHTML;
            }
        },
        val: function (newVal) {
            if (arguments.length) {
                $.each(this, function (i, ele) {
                    ele.value = newVal;
                });
                return this;
            }
            else {
                return this[0].value;
            }
        }
    });
})();