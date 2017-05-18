/******************************************************************************* 
* @copyright
*   Copyright (c) 2017 All Right Reserved, 
*   Unauthorized copying of this file, via any medium is strictly prohibited
*   Proprietary and confidential
*
* @name
*   ComponentJS Repeater
*
* @description
*   creates repeater control
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

ComponentJS.RepeaterProxy = function () {
    ComponentJS.CSF.call(this, ComponentJS.Constants.HtmlElement.Label);
    this._dataSource = null;
    this.totalRows = 0;
    this.rows = 10;
    this.lastNavigated = 1;
    this.pageNavCount = 5;
    this.pageul = null;
    this._template = '';
    this.url = '';
    this._fnFilter = null;
    this.sessionData = {};
    this._gridColumns = [];
    this._filterId = '';
    this.isInitial = 1;
    this._filterConditionId = '';
    this._take = 0;
    this._currentPage = 0;
    this._key = '';
    this._callbackEvents = [];

    this.Id = function (p_id) {
        this[ComponentJS.Constants.Id] = p_id;
        this.AddToHolder();
        return this;
    }
    this.Key = function (p_key) {
        this._key = p_key;
        return this;
    }
    this.Url = function (p_url, p_filterfn) {
        this.url = p_url;
        this._fnFilter = p_filterfn;
        return this;
    }
    this.FilterId = function (p_filterId) {
        this._filterId = p_filterId || '';
        return this;
    }
    this.FilterConditionId = function (p_filterConditionId) {
        this._filterConditionId = p_filterConditionId || '';
        return this;
    }
    this.GridDataSource = function (take, currentPage) {
        //debugger;
        this._take = take;
        this._currentPage = currentPage;
        var url = this.url;
        var filter = this._fnFilter(take, currentPage);
        var id = this[ComponentJS.Constants.Id];
        $.ajax({
            url: url,
            data: filter,
            dataType: "json",
            content: "application/json; charset=utf-8",
            success: function (data) {
                //debugger;
                var element = ComponentJS.GetElementById(id);
                if (element)
                    element.Update(data);
            }
        });
        return this;
    }
    this.CallbackEvents = function (p_callbackEvents) {
        this._callbackEvents = p_callbackEvents || [];
        return this;
    }
    this.Search = function () {
        var url = this.url;
        var filter = this._fnFilter(this._take, 0);
        var id = this[ComponentJS.Constants.Id];
        $.ajax({
            url: url,
            data: filter,
            dataType: "json",
            content: "application/json; charset=utf-8",
            success: function (data) {
                var element = ComponentJS.GetElementById(id);
                if (element) {
                    element.Update(data);
                    element.TotalRows(data.Count);
                    var ul = $('#' + id).parent().next().next().children('ul');
                    var lis = element.GenerateNavs(data.Count);
                    $(ul).empty();
                    $(ul).append(lis);
                    element.lastNavigated = 1;
                    ul.id = id + '_' + element.totalRows + '_' + element.rows + '_' + element.pageNavCount + '_' + element.lastNavigated;
                    element.pageul = ul;
                    $(element.pageul).children().click(element.ListItemClick);
                }
            }
        });
        return this;
    }
    this.Rows = function (p_rows) {
        this.rows = p_rows || this.rows;
        return this;
    }
    this.PageNavCount = function (p_pageNavCount) {
        this.pageNavCount = p_pageNavCount || this.pageNavCount;
        return this;
    }
    this.Template = function (p_template) {
        this._template = p_template;
        return this;
    }
    this.TotalRows = function (p_totalRows) {
        this.totalRows = p_totalRows;
        return this;
    }
    this.Columns = function (p_gridColumns) {
        this._gridColumns = p_gridColumns || [];
        return this;
    }
    this.Update = function (p_data) {
        //debugger;
        if (typeof p_data !== 'undefined' && p_data) {
            this._dataSource = p_data.DataSource;
            this.totalRows = p_data.Count;
            if (!this.isInitial) {
                $('#' + this[ComponentJS.Constants.Id] + ' tr').not('thead tr').remove();
                var ds = this._dataSource;
                if (ds) {
                    var table = document.getElementById(this[ComponentJS.Constants.Id]);
                    if (table) {
                        var tblBody = (table.tBodies && table.tBodies.length > 0) ? table.tBodies[0] : document.createElement("tbody");
                        var gridColumns = this._gridColumns;
                        for (var i = 0; i < ds.length; i++) {
                            var row = table.insertRow(-1);
                            var temp = this._template(ds[i], gridColumns);
                            row.innerHTML = temp;
                            tblBody.appendChild(row);
                        }
                        table.appendChild(tblBody);
                    }
                }
                var msg = "";
                var currentPageNumber = $('#' + this._containerId + ' ul li.active').children().text();
                if ($.isNumeric(currentPageNumber) && currentPageNumber <= 1) {
                    if (this.totalRows === 0)
                        msg = "Showing 0 of 0 entries.";
                    else
                        msg = "Showing 1 of " + this.totalRows + " entries.";

                }
                else {
                    msg = "Showing " + (((currentPageNumber - 1) * this.rows) + 1) + " of " + this.totalRows + " entries.";
                }
                var info = document.getElementById(this[ComponentJS.Constants.Id] + '_info');
                if (info)
                    info.innerHTML = msg;
            }
            else
                this.Create();
            this.ApplyCallbackEvents();
        }
        return this;
    }
    this.GenerateFilter = function () {
        var filterDiv1 = document.createElement('div');
        filterDiv1.id = 'filterdiv_' + this[ComponentJS.Constants.Id];

        var filterDiv2 = document.createElement('div');
        filterDiv2.className = 'btn-group pull-right';
        filterDiv1.appendChild(filterDiv2);
        var button = document.createElement('button');
        button.title = 'Columns';
        button.type = 'button';
        button.setAttribute('data-toggle', 'dropdown');
        button.className = 'btn btn-default dropdown-toggle';
        var span1 = document.createElement('span1');
        span1.className = 'glyphicon glyphicon-th';
        button.appendChild(span1);
        var span2 = document.createElement('span1');
        span2.className = 'caret';
        button.appendChild(span2);
        filterDiv2.appendChild(button);

        var ul = document.createElement('ul');
        ul.id = 'filterul1_' + this[ComponentJS.Constants.Id];
        ul.className = 'dropdown-menu dropdown-menu-right ui-sortable';
        if (this._gridColumns) {
            var gridColumns = this._gridColumns = ComponentJS.SortByKey(this._gridColumns, 'Sequence');
            var length = gridColumns.length;
            var colIndex = 0;
            var id = this[ComponentJS.Constants.Id];
            for (var i = 0; i < length; i++) {
                var gridColumn = gridColumns[i];
                if (gridColumn.IsVisible) {
                    if (gridColumn.ColumnName === this._key || gridColumn.ColumnName === ComponentJS.Constants.Buttons.Edit || gridColumn.ColumnName === ComponentJS.Constants.Buttons.Delete)
                        colIndex++;
                    else {
                        var li = document.createElement('li');
                        var anc = document.createElement('a');
                        anc.href = 'javascript:void(0);';
                        var label = document.createElement('label');
                        label.className = 'columns-label';
                        var check = document.createElement('input');
                        check.type = 'checkbox';
                        check.className = 'col-checkbox';
                        check.setAttribute('checked', '');
                        check.setAttribute('data-col-index', colIndex);
                        check.setAttribute('data-col-name', gridColumn.ColumnName);
                        label.appendChild(check);
                        $(check).change(function () {
                            var col = $(this).attr('data-col-index');
                            var colName = $(this).attr('data-col-name');
                            var isChecked = $(this).is(':checked');
                            $("#" + id + " tr").each(function () {
                                if (isChecked)
                                    $(this).find("th:eq(" + col + "),td:eq(" + col + ")").show();
                                else
                                    $(this).find("th:eq(" + col + "),td:eq(" + col + ")").hide();
                            });
                            var element = ComponentJS.GetElementById(id);
                            if (element) {
                                for (var j = 0; j < element.gridColumns.length; j++) {
                                    if (element.gridColumns[j].ColumnName === colName)
                                        element.gridColumns[j].IsVisible = isChecked;
                                }
                                element.GridDataSource(element._take, element._currentPage);
                            }
                        });
                        var text = document.createTextNode(' ' + gridColumns[i].DisplayName);
                        label.appendChild(text);
                        anc.appendChild(label);
                        li.appendChild(anc);
                        ul.appendChild(li);
                        colIndex++;
                    }
                }
            }
            filterDiv2.appendChild(ul);
        }
        var filterElement = document.getElementById(this._filterId);
        if (filterElement)
            filterElement.appendChild(filterDiv1);
        return this;
    }
    this.Create = function () {
        this.isInitial = 0;
        var table = document.createElement('table');
        table.id = this[ComponentJS.Constants.Id];
        table.width = '100%';
        table.height = '100%';
        table.className = 'col-md-12 table table-bordered table-striped table-condensed';
        if (this._dataSource && this._dataSource[0]) {
            var ds = this._dataSource;
            if (this._gridColumns) {
                var header = table.createTHead();
                var row = header.insertRow();
                var gridColumns = this._gridColumns = ComponentJS.SortByKey(this._gridColumns, 'Sequence');
                var length = gridColumns.length;
                for (var i = 0; i < length; i++) {
                    if (gridColumns[i].IsVisible) {
                        var headerCell = document.createElement('th');
                        var txt = (gridColumns[i].IsSortable) ? "<span>" + gridColumns[i].DisplayName + " <i data-id='" + gridColumns[i].ColumnName + "' data-grid='" + this[ComponentJS.Constants.Id] + "' data-status='up' data-sort=true class='fa fa-chevron-circle-up'></i></span>" : gridColumns[i].DisplayName;
                        headerCell.innerHTML = txt;
                        row.appendChild(headerCell);
                    }
                }
                length = ds.length;
                var tblBody = document.createElement("tbody");
                for (var i = 0; i < length; i++) {
                    row = table.insertRow(-1);
                    var temp = this._template(ds[i], gridColumns);
                    row.innerHTML = temp;
                    tblBody.appendChild(row);
                }
                table.appendChild(tblBody);
                var div = document.createElement('div');
                var info = document.createElement('div');
                info.id = this[ComponentJS.Constants.Id] + '_info';
                info.className = 'pagination';
                info.innerHTML = 'showing 1 of ' + this.totalRows;
                div.setAttribute('style', 'overflow:auto;height:auto;width:auto');
                div.innerHTML = "";
                div.appendChild(table);
                this._element = table;
                this[ComponentJS.Constants.Container].appendChild(div);
                this[ComponentJS.Constants.Container].appendChild(info);
                var pageDiv = document.createElement('div');
                pageDiv.className = 'btn-group pull-right'
                var ul = document.createElement('ul');
                ul.id = this[ComponentJS.Constants.Id] + '_' + this.totalRows + '_' + this.rows + '_' + this.pageNavCount + '_' + this.lastNavigated;
                ul.className = 'pagination';
                var lis = this.GenerateNavs(this.totalRows);
                pageDiv.appendChild(ul);
                $(ul).append(lis);
                this.pageul = ul;
                $(this.pageul).children().click(this.ListItemClick);
                this[ComponentJS.Constants.Container].appendChild(pageDiv);
                $('i[data-sort=true]').on('click', function () {
                    var gridId = $(this).attr('data-grid');
                    var isdesc = false;
                    if ($(this).attr('data-status') === 'up') {
                        this.className = 'fa fa-chevron-circle-down';
                        $(this).attr('data-status', 'down');
                        isdesc = true;
                    }
                    else {
                        this.className = 'fa fa-chevron-circle-up';
                        $(this).attr('data-status', 'up');
                        isdesc = false;
                    }
                    var dataId = $(this).attr('data-id');
                    var element = ComponentJS.GetElementById(gridId);
                    if (element) {
                        var Count = element.totalRows;
                        var DataSource = element._dataSource = ComponentJS.SortByKey(element._dataSource, dataId, isdesc);
                        element.Update({ DataSource: DataSource, Count: Count });
                    }
                });
            }
        }
        return this;
    }
    this.ApplyCallbackEvents = function () {
        if (this._callbackEvents.length > 0) {
            for (var c = 0; c < this._callbackEvents.length; c++) {
                var _callbackEvent = this._callbackEvents[c];
                $(_callbackEvent.Element).on(_callbackEvent.Event, {}, _callbackEvent.Callback);
            }
        }
        return this;
    }
    this.Render = function () {
        //debugger;
        console.log('repeater rendered.');
        this.GenerateFilter();
        this.AddToHolder();
        this._dataSource = this.GridDataSource(this.rows, 0);
    }
    this.GenerateNavs = function (rows, init) {
        var noofnavs = rows || 1;
        init = init || 0;
        if (rows <= this.rows)
            noofnavs = 1;
        else if (rows <= (this.rows * 2))
            noofnavs = 2;
        else if (rows <= (this.rows * 3))
            noofnavs = 3;
        else if (rows <= (this.rows * 4))
            noofnavs = 4;
        else if (rows <= (this.rows * 5))
            noofnavs = 5;
        else if (rows <= (this.rows * 6))
            noofnavs = 6;
        else if (rows <= (this.rows * 7))
            noofnavs = 7;
        else if (rows <= (this.rows * 8))
            noofnavs = 8;
        else if (rows <= (this.rows * 9))
            noofnavs = 9;
        else
            noofnavs = 10;
        if (noofnavs > this.pageNavCount)
            noofnavs = this.pageNavCount;
        var lis = "<li><a id='pagprev' href='javascript:void(0)' aria-label='Previous'><span aria-hidden='true'>&laquo;</span></a></li>";
        for (var i = 0; i < noofnavs; i++) {
            if (i == 0)
                lis += "<li class='active'><a href='javascript:void(0)'>" + (init + i + 1) + "</a></li>";
            else
                lis += "<li><a href='javascript:void(0)'>" + (init + i + 1) + "</a></li>";
        }
        lis += "<li><a id='pagnext' href='javascript:void(0)' aria-label='Next'><span aria-hidden='true'>&raquo;</span></a></li>";
        return lis;
    }
    this.ListItemClick = function () {
        var tid = $(this).children().attr('id');
        var closeul = $(this).closest('ul');
        var closeulid = closeul.attr('id');
        var last_ = closeulid.lastIndexOf("_");
        var arr = closeulid.split('_');
        var ulId = arr[0];
        var totalRows = parseInt(arr[1]);
        var rows = parseInt(arr[2]);
        var pageNavCount = parseInt(arr[3]);
        var lastNavigated = parseInt(arr[4]);

        if (tid == 'pagnext' && (lastNavigated * rows > totalRows)
            || (tid == 'pagprev' && lastNavigated == 1)) {
            return;
        }
        $('#' + closeulid + ' li').removeAttr('class');
        $(this).attr('class', 'active');
        var element = ComponentJS.GetElementById(ulId);
        element.isInitial = 0;
        if (tid == 'pagprev' || tid == 'pagnext') {
            if (tid == 'pagprev') {
                if ((parseInt(lastNavigated) - 1) % pageNavCount === 0) {
                    var lis = element.GenerateNavs((parseInt(totalRows)), (parseInt(lastNavigated) - 1 - pageNavCount));
                    $('#' + closeulid).empty();
                    $('#' + closeulid).append(lis);
                    $('#' + closeulid + ' li').click(element.ListItemClick);
                    lastNavigated = lastNavigated - 1;
                    $('#' + closeulid + ' li').removeAttr('class');
                    $('#' + closeulid + ' li').eq(pageNavCount).attr('class', 'active');
                    closeul.attr('id', closeulid.substr(0, last_ + 1) + lastNavigated);
                }
                else {
                    lastNavigated = lastNavigated - 1;
                    $('#' + closeulid + ' li').removeAttr('class');
                    var aN = (this.lastNavigated <= this.pageNavCount) ? lastNavigated : lastNavigated % pageNavCount;
                    $('#' + closeulid + ' li').eq(aN).attr('class', 'active');
                    closeul.attr('id', closeulid.substr(0, last_ + 1) + lastNavigated);
                }
            }
            else {
                if ((((lastNavigated + 1) < (parseInt(totalRows / rows))) && (lastNavigated % pageNavCount == 0)) || ((totalRows - (lastNavigated * rows) > 0) && (lastNavigated % pageNavCount == 0))) {
                    var lis = element.GenerateNavs((totalRows - (lastNavigated * rows)), lastNavigated);
                    $('#' + closeulid).empty();
                    $('#' + closeulid).append(lis);
                    $('#' + closeulid + ' li').click(element.ListItemClick);
                    lastNavigated = lastNavigated + 1;
                    $('#' + closeulid + ' li').removeAttr('class');
                    $('#' + closeulid + ' li').eq(lastNavigated % pageNavCount).attr('class', 'active');
                    closeul.attr('id', closeulid.substr(0, last_ + 1) + lastNavigated);
                }
                else {
                    lastNavigated = lastNavigated + 1;
                    closeulid = closeulid.substr(0, last_ + 1) + lastNavigated;
                    closeul.attr('id', closeulid);
                    $('#' + closeulid + ' li').removeAttr('class');
                    $('#' + closeulid + ' li a').each(function () {
                        var t = $(this).text()
                        if (t == lastNavigated) {
                            $(this).parent().attr('class', 'active');
                        }
                    })
                }
            }
        }
        else {
            lastNavigated = parseInt($(this).children().text());
            closeul.attr('id', closeulid.substr(0, last_ + 1) + lastNavigated);
        }
        element.GridDataSource(parseInt(arr[2]), parseInt(lastNavigated) - 1);
    }
    this.ShowFilter = function () {
        return this;
    }
    return this;
};

ComponentJS.Repeater = function () {
    return new ComponentJS.RepeaterProxy();
}