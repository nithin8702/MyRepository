(function () {
    $ = function (selector) {
        var elements = document.querySelectorAll(selector);
        for (var index = 0; index < elements.length; index++) {
            var element = elements[index];
            this[index] = element;
        }
        this.length = elements.length;
    }

    $.extend = function (target, object) {
        for (var key in object) {
            if (object.hasOwnProperty(key)) {
                target[key] = object[key];
            }
        }
    }

    $.extend($, {
        each: function (collections, cb) {
            if (typeof collections.length === 'number') {
                //array
                for (var index = 0; index < collections.length; index++) {
                    var element = collections[index];
                    cb.call(element, index, element);
                }
            }
            else {
                //object
                for (var key in collections) {
                    if (collections.hasOwnProperty(key)) {
                        var element = collections[key];
                        cb.call(element, key, element);
                    }
                }
            }
        }
    });

    $.extend($.prototype, {
        val: function (newval) {
            if (arguments.length) {
                $.each(this, function (i, ele) {
                    ele.value = newval;
                })
            }
            else {
                return this[0].value;
            }
        }
    });

})();