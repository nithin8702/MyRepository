"use strict";
var router_1 = require('@angular/router');
var add_component_1 = require('./employee/add.component');
var edit_component_1 = require('./employee/edit.component');
var appRoutes = [
    { path: 'add', component: add_component_1.AddEmployeeComponent },
    { path: 'edit', component: edit_component_1.EditEmployeeComponent }
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map