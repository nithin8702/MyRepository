/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS Form Elements
*
* @description
*   creates form elements
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

ComponentJS.FormElements = function () {
    ComponentJS.CSF.call(this);
    this.Render = function (p_arr) {
        if (typeof p_arr !== 'undefined' && p_arr && p_arr.length > 0) {
            var length = p_arr.length;
            for (var index = 0; index < length; index++) {
                var objElement = p_arr[index];
                if (objElement) {
                    var _currElement = null;
                    switch (objElement.Element) {
                        case ComponentJS.Constants.HtmlElement.Label:
                            _currElement = ComponentJS.Label().For(objElement.For);
                            break;
                        case ComponentJS.Constants.HtmlElement.Textbox:
                            _currElement = ComponentJS.Textbox();
                            break;
                        case ComponentJS.Constants.HtmlElement.TextArea:
                            _currElement = ComponentJS.TextArea().Title(objElement.Title).IsRequired(objElement.IsRequired).MaxLength(objElement.MaxLength);
                            break;
                        case ComponentJS.Constants.HtmlElement.Button:
                            _currElement = ComponentJS.Button();
                            break;
                        case ComponentJS.Constants.HtmlElement.DeleteButton:
                            _currElement = ComponentJS.DeleteButton();
                            break;
                        case ComponentJS.Constants.HtmlElement.EditButton:
                            _currElement = ComponentJS.EditButton();
                            break;
                        case ComponentJS.Constants.HtmlElement.ClearButton:
                            _currElement = ComponentJS.ClearButton().OnClick(objElement.OnClick);
                            break;
                        case ComponentJS.Constants.HtmlElement.AddButton:
                            _currElement = ComponentJS.AddButton().OnClick(objElement.OnClick);
                            break;
                        case ComponentJS.Constants.HtmlElement.DraftButton:
                            _currElement = ComponentJS.DraftButton().OnClick(objElement.OnClick).FormElementId(this._id);
                            break;
                        case ComponentJS.Constants.HtmlElement.NextButton:
                            _currElement = ComponentJS.NextButton();
                            break;
                        case ComponentJS.Constants.HtmlElement.SearchButton:
                            _currElement = ComponentJS.SearchButton().GridId(objElement.GridId).OnClick(objElement.OnClick);
                            break;
                        case ComponentJS.Constants.HtmlElement.SubmitButton:
                            _currElement = ComponentJS.SubmitButton();
                            break;
                        case ComponentJS.Constants.HtmlElement.Checkbox:
                            _currElement = ComponentJS.Checkbox();
                            break;
                        case ComponentJS.Constants.HtmlElement.Dropdown:
                            _currElement = ComponentJS.Dropdown().SelectedValue(objElement.selectedValue);
                            break;
                        case ComponentJS.Constants.HtmlElement.Calendar:
                            _currElement = ComponentJS.Calendar().SetDate(objElement.Date);
                            break;
                        case ComponentJS.Constants.HtmlElement.Repeater:
                            _currElement = ComponentJS.Repeater();
                            break;
                        default:
                            break;
                    }
                    if (_currElement) {
                        if (objElement.Element === ComponentJS.Constants.HtmlElement.Repeater)
                            _currElement.Id(objElement.Id).Key(objElement.Key).Columns(objElement.Columns).Url(objElement.Url, objElement.Filter).FilterId(objElement.FilterId).FilterConditionId(objElement.FilterConditionId).Template(objElement.Template).Rows(objElement.Rows).PageNavCount(objElement.PageNavCount).Container(objElement.Container).CallbackEvents(objElement.CallbackEvents).Render();
                        else
                            _currElement.Id(objElement.Id).Text(objElement.Text).Container(objElement.Container).Render();
                    }
                }
            }
        }
        return this;
    }
    return this;
};