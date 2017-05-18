/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS Textbox
*
* @description
*   creates textbox control
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

ComponentJS.TextboxProxy = function () {
    ComponentJS.Input.call(this);
    this[ComponentJS.Constants.Elements].type = ComponentJS.Constants.HtmlElement.InputType.Text;
    return this;
};

ComponentJS.Textbox = function () {
    return new ComponentJS.TextboxProxy();
}
