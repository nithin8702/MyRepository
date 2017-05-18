/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS Label
*
* @description
*   creates label control
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

ComponentJS.LabelProxy = function () {
    ComponentJS.CSF.call(this, ComponentJS.Constants.HtmlElement.Label);
    return this;
};

ComponentJS.Label = function () {
    return new ComponentJS.LabelProxy();
}
