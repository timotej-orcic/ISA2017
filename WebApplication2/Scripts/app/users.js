var app = angular.module('myApp', ['ui.bootstrap', 'ngRoute']);

app.controller("UsersController", function ($scope, $http, $window) {

    $scope.usersList = [];

    $scope.loadUsers = function () {
        var url = '/App/Users';

        $http({
            method: "POST",
            url: url
        })
            .success(function (data, status, headers, config) {
                if (data && data.Success == true) {

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