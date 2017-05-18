/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS Dictionary
*
* @description
*   creates dictionary object
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

ComponentJS.Dictionary = function () {
    ComponentJS.CSF.call(this);
    this.Add = function (p_key, p_value) {
        this[ComponentJS.Constants.Dictionary][p_key] = p_value;
        return this;
    }
    this.Find = function (p_key) {
        return this[ComponentJS.Constants.Dictionary][p_key];
    }
    this.FindAll = function () {
        return this[ComponentJS.Constants.Dictionary];
    }
};