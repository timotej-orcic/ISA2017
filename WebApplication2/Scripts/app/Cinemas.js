var app = angular.module('myApp', ['ui.bootstrap', 'ngRoute']);

app.controller("CinemaController", function ($scope, $http, $window) {

    $scope.alerts = [];
    $scope.currAlert = {};

    $scope.CinemasList = [];

    $scope.loadCinemas = function () {
        var url = '/App/ShowCinemas';

        $http({
            method: "POST",
            url: url
        })
            .success(function (data, status, headers, config) {
                if (data) {
                    $scope.CinemasList = data;
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

    $scope.loadCinemas();

});
