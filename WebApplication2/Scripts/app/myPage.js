var app = angular.module('myApp', ['ui.bootstrap', 'ngRoute']);

app.controller("MyPageController", function ($scope, $http, $window) {

    $scope.alerts = [];
    $scope.currAlert = {};

    $scope.usersList = [];

    $scope.loadUsers = function () {
        var url = '/App/getUsers';

        $http({
            method: "POST",
            url: url
        })
            .success(function (data, status, headers, config) {
                if (data) {
                    $scope.usersList = data;
                } else if (data && data.Success == false) {
                    $scope.currAlert = { type: "danger", msg: data.Message };
                    $scope.alerts.push($scope.currAlert);
                } else {
                    $scope.currAlert = { type: "danger", msg: 'There was a mistake. Please try again.' };
                    $scope.alerts.push($scope.currAlert);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.currAlert = { type: 'danger', msg: 'Function call error.' };
                $scope.alerts.push($scope.currAlert);
            });
    };

    $scope.loadUsers();

});

app.config(function ($routeProvider) {
    $routeProvider
        .when("", {
            templateUrl: ""
        })
        .when("/users", {
            templateUrl: "PartialViews/Users.cshtml"
        });
});