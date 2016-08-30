var ctrl = (function () {
    var element = null;

    function CreateElement(tag) {
        ele = document.createElement(tag);
        return this;
    }
    function Id(val) {
        ele.id = val;
    }
    function Text(text) {
        ele.text = text;
    }
    function Container(containerId) {
        var container = document.getElementById(containerId);
        container.appendChild(ele)
    }
    return{
        Create:CreateElement,
        Container:Container
    }

    // return {
    //     createElement:function (tag) {
    //         ele = document.createElement(tag);
    //         return this;
    //     },
    //     Container:function (containerId) {
    //         var container = document.getElementById(containerId);
    //         container.appendChild(ele)
    //     }
    // }

})();
debugger;
ctrl.Create("input").Container("div1");