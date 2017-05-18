/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS
*
* @description
*   base for ComponentJS
*
* @author
*   Nithin Rajan
*
* @since
*   10 April 2017
*
* @version
*   1.0.0.0
*
********************************************************************************/

//Initiating ComponentJS
ComponentJS.Elements = {};
//to get elements
ComponentJS.GetElementById = function (p_id) {
    if (p_id && p_id !== '')
        return ComponentJS.Elements[p_id]
    else
        return ComponentJS.Elements;
}
ComponentJS.CSF = function (ele) {
    this[ComponentJS.Constants.Id] = null;
    this[ComponentJS.Constants.Elements] = null;
    this[ComponentJS.Constants.Container] = null;
    this[ComponentJS.Constants.Dictionary] = {};
    this._text = null;
    if (ele)
        this[ComponentJS.Constants.Elements] = document.createElement(ele);
    //Setting Id to the element
    this.Id = function (p_id) {
        this[ComponentJS.Constants.Id] = p_id;
        this[ComponentJS.Constants.Elements].id = p_id;
        this[ComponentJS.Constants.Elements].name = p_id;
        this.AddToHolder();
        return this;
    }
    //Setting Container for the element
    this.Container = function (p_containerId) {
        this[ComponentJS.Constants.Container] = document.getElementById(p_containerId);
        return this;
    }
    this.AddToHolder = function () {
        ComponentJS.Elements[this[ComponentJS.Constants.Id]] = this;
        return this;
    }
    //assign text property
    this.Text = function (p_text) {
        this._text = p_text;
        return this;
    }
    //Logging messages if the logging is enabled
    this.Log = function (p_value) {
        if (this.isLog)
            console.log(p_value);
        return this;
    }
    this.SetAttribute = function (p_attr, p_value) {
        this[ComponentJS.Constants.Elements].setAttribute(p_attr, p_value)
        return this;
    }
    this.SortByKey = function (array, key, isdesc) {
        if (typeof isdesc !== 'undefined' && isdesc)
            return array.sort(function (a, b) {
                var x = a[key]; var y = b[key];
                return ((x > y) ? -1 : ((x < y) ? 1 : 0));
            });
        else
            return array.sort(function (a, b) {
                var x = a[key]; var y = b[key];
                return ((x < y) ? -1 : ((x > y) ? 1 : 0));
            });
        return this;
    }
    this.Render = function () {
        this.Log('Rendering');
        //this[ComponentJS.Constants.Elements].type = this[ComponentJS.Constants.HtmlElement.Type];
        if (this[ComponentJS.Constants.Elements].type === ComponentJS.Constants.HtmlElement.InputType.Text)
            this[ComponentJS.Constants.Elements].value = this._text;
        else
            this[ComponentJS.Constants.Elements].innerHTML = this._text;
        this[ComponentJS.Constants.Container].appendChild(this[ComponentJS.Constants.Elements]);
        return this;
    }
};