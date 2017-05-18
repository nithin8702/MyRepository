/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS Input
*
* @description
*   creates input control
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

//Constructor for Input Elements - TextBox, CheckBox, RadioButton, Hidden
ComponentJS.Input = function () {
    ComponentJS.CSF.call(this, ComponentJS.Constants.HtmlElement.Input);
    //Gets the updated value of textbox
    this.GetText = function () {
        return this[ComponentJS.Constants.Elements].value;
    }
    //Gets the initial value of textbox
    this.GetDefaultText = function () {
        return this._text;
    }
    return this;
};
