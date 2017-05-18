/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS CheckBox
*
* @description
*   creates checkbox control
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

ComponentJS.Checkbox = function () {
    ComponentJS.Input.call(this);
    this[ComponentJS.Constants.Elements].type = ComponentJS.Constants.HtmlElement.InputType.CheckBox;
    this.message = null;
    //generates checkbox
    this.Render = function () {
        this[ComponentJS.Constants.Container].appendChild(this[ComponentJS.Constants.Elements]);
        var label = document.createElement(ComponentJS.Constants.HtmlElement.Label);
        label.setAttribute('for', this[ComponentJS.Constants.Id])
        label.innerHTML = ComponentJS.Constants.Space.OneSpace + this._text;
        this[ComponentJS.Constants.Container].appendChild(label);
    }
    return this;
};