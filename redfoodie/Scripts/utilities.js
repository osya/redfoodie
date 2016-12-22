utils = {
    logging: true,
    gMapMarkers: new Array(),        
    directionsDisplay: null,  
    log: function(message) {
        if (typeof console != "undefined" && utils.logging) {
            console.log(message);
        }
    },
    geocode: function(address, callback, id, extraData) { // accepts an address and calls call back using the co-ordinate object as argument
        geocoder = new google.maps.Geocoder();
        var response = {};
        geocoder.geocode({'address': address}, function(results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                try {
                    utils.log(results);
                    response.status = 'OK';
                    response.latitude = results[0].geometry.location.lat();
                    response.longitude = results[0].geometry.location.lng();
                } catch (e) {
                    response.status = 'ERROR';
                    response.message = 'Error parsing the response';
                }
            }
            else {
                //error 
                response.status = 'ERROR';
                response.message = 'ERROR';
            }
            utils.log(response);
            callback.call(window, response, id, address,extraData);
        })
    },
    plotMap: function(latitude, longitude, canvas_id, zoom) {
        var map;
        if(typeof zoom == 'undefined'){
            zoom = 8;
        }
        var mapOptions = {
            zoom: zoom,
            scrollwheel: false,
            panControl: false,
            streetViewControl: false,
            center: new google.maps.LatLng(latitude, longitude),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById(canvas_id),
                mapOptions);
        return map;
    },
    staticMapImage: function(latitude, longitude, canvas_id, zoom){
        var map;
        if(typeof zoom == 'undefined'){
            zoom = 8;
        }
        var mapOptions = {
            zoom: zoom,
            scrollwheel: false,
            panControl: false,
            streetViewControl: false,
            center: new google.maps.LatLng(latitude, longitude),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById(canvas_id),
            mapOptions);
        return map;
    },
    addMapMarker: function(latitude, longitude, map, title, html, icon_url, draggable) {
        if(typeof draggable == 'undefined'){
            draggable = false;
        }
        var markerParams = {
            map: map,
            animation: google.maps.Animation.DROP,
            title: title,
            position: new google.maps.LatLng(latitude, longitude),
            html: html,
            draggable : draggable
        };
        if(typeof icon_url != 'undefined'){
            markerParams.icon = icon_url;
        }
        marker = new google.maps.Marker(markerParams);
        utils.gMapMarkers.push(marker);
    },
    getYQLData: function(yql, callback) {
        // Construct your query:
        var query = yql;
        // Instantiate with the query:
        var firstFeedItem = new YQLQuery(query, callback);
        // If you're ready then go:
        firstFeedItem.fetch(); // Go!!
    },
    setAllMap: function(map) {
        markers = utils.gMapMarkers;
        
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(map);
        }
    },
    clearMarkers: function() {
        utils.setAllMap(null);
    },
    clearMap: function() {
        if (utils.directionsDisplay) {
            utils.directionsDisplay.setMap(null);
        }
    },
    renderGMapDirections: function(start, end, gMap,  panelElId, gMapIcons) {
        if (google.maps) {
            document.getElementById(panelElId).innerHTML = '';
            utils.clearMarkers();
            utils.clearMap();
            var directionsService = new google.maps.DirectionsService();
            var directionsDisplay = new google.maps.DirectionsRenderer(); 
            var request = {
                origin: start,
                destination: end,
                travelMode: google.maps.DirectionsTravelMode.DRIVING
            };
            directionsDisplay.setMap(gMap);
            directionsDisplay.setOptions( { suppressMarkers: true } );
            directionsDisplay.setPanel(document.getElementById(panelElId));
            directionsService.route(request, function(response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                    gMapRoute = response.routes[0].legs[0];
                    var markerParams = {
                            map: gMap,
                            animation: google.maps.Animation.DROP,
                            position: gMapRoute.steps[0].start_location,
                            draggable : false
                        };
                    if (typeof gMapIcons != 'undefined' && typeof gMapIcons.start != 'undefined') {
                        markerParams.icon = gMapIcons.start;
                    }
                    marker = new google.maps.Marker(markerParams);
                    utils.gMapMarkers.push(marker);
                    markerParams.position = gMapRoute.steps[gMapRoute.steps.length - 1].start_location;
                    if (typeof gMapIcons != 'undefined' && typeof gMapIcons.end != 'undefined') {
                        markerParams.icon = gMapIcons.end;
                    }
                    marker = new google.maps.Marker(markerParams);
                    utils.gMapMarkers.push(marker);
                }
            });
            utils.directionsDisplay = directionsDisplay;
        }
    }
};