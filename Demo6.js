(function () {
    $ = function (selector) {
        //debugger;
        var elements = document.querySelectorAll(selector);
        var length = elements.length;
        for (var index = 0; index < elements.length; index++) {
            var element = elements[index];
            this[index] = element;
        }
        this.length = length;
    };
    //Fn to extend properties
    $.extend = function (target, object) {
        for (var key in object) {
            if (object.hasOwnProperty(key)) {
                target[key] = object[key];
            }
        }
        return target;
    }

    $.extend($, {
        each: function (object, cb) {
            if (typeof object.length === "number") {
                for (var index = 0; index < object.length; index++) {
                    var element = object[index];
                    cb.call(element, index, element);
                }
            }
            else {
                for (var key in object) {
                    if (object.hasOwnProperty(key)) {
                        var element = object[key];
                        cb.call(element, key, element);
                    }
                }
            }
        }
    });
    $.extend($.prototype, {
        val: function (newVal) {
            //debugger;
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