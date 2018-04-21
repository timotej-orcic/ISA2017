/*$(window).load(function () {
    initializeMap();
});*/
var geocoder;
var map;
var myLocation;
var locLocation;

function initializeMap() {
    infoWindow = new google.maps.InfoWindow;

    // Try HTML5 geolocation.
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            myLocation = pos;

            //find all locations
            $('.adrDiv').each(function (i, obj) {
                var address = $(obj).attr("data-address");
                getMyDistance(address, obj);
            });
           
            var myOptions = {
                zoom: 16,
                center: pos,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById("map"), myOptions);

            infoWindow.setPosition(pos);
            infoWindow.setContent('My location.');
            infoWindow.open(map);
            map.setCenter(pos);
        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }
};

function getAddressCoords(address, obj) {
    geocoder = new google.maps.Geocoder();
    var lat = '';
    var lng = '';
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            lat = results[0].geometry.location.lat(); //getting the lat
            lng = results[0].geometry.location.lng(); //getting the lng

            var loc = {
                lat: lat,
                lng: lng
            };

            var locDist = getDistance(myLocation, loc);
            $(obj).find("#onLoadDist").html(locDist);


        } else {
            alert("Geocode was not successful for the following reason: " + status);
        }
    });
}

function codeAddress(address) {
    geocoder = new google.maps.Geocoder();
    var lat = '';
    var lng = ''
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            lat = results[0].geometry.location.lat(); //getting the lat
            lng = results[0].geometry.location.lng(); //getting the lng

            locLocation = {
                lat: lat,
                lng: lng
            };
            $(document.getElementById("distance")).html(getMyDistance());
            $(document.getElementById("distancePlholder")).css("visibility", "visible");


            map.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
        } else {
            alert("Geocode was not successful for the following reason: " + status);
        }
    });
    var latlng = new google.maps.LatLng(lat, lng);
    var myOptions = {
        zoom: 16,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById("map"), myOptions);

    window.scrollTo(0, 0);
};

var getMyDistance = function (address, obj) {

    if (typeof address == 'undefined') {
        if ((typeof myLocation == 'undefined') || (typeof locLocation == 'undefined')) {
            return "Undefined";
        }
        else {
            return getDistance(myLocation, locLocation);
        }
    }
    else {
        getAddressCoords(address, obj);
    }
};

var rad = function (x) {
    return x * Math.PI / 180;
};

var getDistance = function (p1, p2) {
    var R = 6378137; // Earth’s mean radius in meter
    var dLat = rad(p2.lat - p1.lat);
    var dLong = rad(p2.lng - p1.lng);
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(rad(p1.lat)) * Math.cos(rad(p2.lat)) *
      Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return Math.round(d * 100) / 100; // returns the distance in meter, 2 decimals
};