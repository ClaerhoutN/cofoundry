﻿angular.module("cms.dashboard", ["ngRoute", "cms.shared"]).constant("_", window._).constant("dashboard.modulePath", "/admin/modules/dashboard/js/"); angular.module("cms.dashboard").config(["$routeProvider", "shared.routingUtilities", "dashboard.modulePath", function (n, t, i) { n.otherwise(t.mapOptions(i, "Dashboard")) }]); angular.module("cms.dashboard").controller("DashboardController", ["_", "shared.modalDialogService", "shared.urlLibrary", function (n, t, i) { function u() { r.urlLibrary = i } var r = this; u() }]); angular.module("cms.dashboard").factory("dashboard.dashboardService", ["$http", "_", "shared.serviceBase", function (n, t, i) { function u(n) { var i = f; return n && (i = t.extend({}, f, n)), { params: i } } var r = {}, f = { pageSize: 5 }; return r.getPages = function () { return n.get(i + "pages", u()) }, r.getDraftPages = function () { return n.get(i + "pages", u({ workFlowStatus: "Draft" })) }, r.getUsers = function () { return n.get(i + "users", u({ userAreaCode: "COF" })) }, r.getPageTemplates = function () { return n.get(i + "page-templates", u()) }, r }]); angular.module("cms.dashboard").directive("cmsDashboardComponent", ["dashboard.modulePath", function (n) { function t() { } return { restrict: "E", templateUrl: n + "uicomponents/DashboardComponent.html", scope: { heading: "@cmsHeading", listUrl: "@cmsListUrl", createUrl: "@cmsCreateUrl", entityName: "@cmsEntityName", entityNamePlural: "@cmsEntityNamePlural", numItems: "=cmsNumItems", loader: "=cmsLoader" }, replace: !0, controller: t, controllerAs: "vm", bindToController: !0, transclude: !0 } }])