

var TileLayer = ol.layer.Tile;
var OSM = ol.source.OSM;
var VectorSource = ol.source.Vector;
var VectorLayer = ol.layer.Vector;
var Style = ol.style.Style;
var Fill = ol.style.Fill;
var Stroke = ol.style.Stroke;
var CircleStyle = ol.style.Circle;
var MultiPoint = ol.geom.MultiPoint;
var Map = ol.Map;
var View = ol.View;
var Modify = ol.interaction.Modify;
var Draw = ol.interaction.Draw;
var Snap = ol.interaction.Snap;
var Polygon = ol.geom.Polygon;
var LineString = ol.geom.LineString;
var Point = ol.geom.Point;
var Overlay = ol.Overlay;
var getArea = ol.sphere.getArea;
var getLength = ol.sphere.getLength;
var unByKey = ol.Observable.unByKey;

const popupContainer = document.getElementById('popup');
const popupContent = document.getElementById('popup-content');
const popupContentParquet = document.getElementById('popup-content-parquet');
const popupContentPrison = document.getElementById('popup-content-prison');
const closer = document.getElementById('popup-closer');

$(document).ready(function () {

    $('#map-layer-control-btn').on("click", function () {
        $('#control-option-box').slideToggle(50);
    });

    closer.onclick = function () {
        overlay.setPosition(undefined);
        closer.blur();
        return false;
    };

    var lat = 18.971187;
    var lon = -72.285215;
    var zoom = 8.5;

    const styles = [
        new Style({
            stroke: new Stroke({
                color: 'blue',
                width: 3,
            }),
            fill: new Fill({
                color: 'rgba(0, 0, 255, 0.4)',
            }),
            text: new ol.style.Text({
                font: '18px Calibri,sans-serif',
                textAlign: 'end',
                fill: new ol.style.Fill({ color: '#000' }),
                stroke: new ol.style.Stroke({
                    color: '#fff', width: 2
                }),
                text: ''
            })
        }),
    ];

    var icon_source = $('#tribunal-icon').data('url') != null && typeof ($('#tribunal-icon').data('url')) !== "undefined " ? $('#tribunal-icon').data('url') : $('#parquet-icon').data('url') != null && typeof ($('#parquet-icon').data('url')) !== "undefined " ? $('#parquet-icon').data('url') : $('#prison-icon').data(url);
  
    var icon = new ol.style.Icon({
        color: 'white',
        scale: 0.2,
        crossOrigin: 'anonymous',
        src: '' + icon_source
        //$('#tribunal-icon').data('url')
    });


    var style = new ol.style.Style({
        image: icon,
        text: new ol.style.Text({
            font: '10px Calibri,sans-serif',
            textAlign: 'end',
            fill: new ol.style.Fill({ color: '#000' }),
            stroke: new ol.style.Stroke({
                color: '#fff', width: 2
            }),
            text: ''
        })
    });

    var tribunalVectorSource = new ol.source.Vector({
    });
    var parquetVectorSource = new ol.source.Vector({
    });
    var prisonVectorSource = new ol.source.Vector({
    });

    var tribunalLayer = new ol.layer.Vector({
        source: tribunalVectorSource,
        style: function (feature) {

            styles[0].setImage(new ol.style.Icon({
                anchor: [0.5, 0.5],
                color: 'white',
                scale: 0.05,
                crossOrigin: 'anonymous',
                src: '' + feature.get('iconSrc')
            }));

            styles[0].getText().setText(feature.get('name') + '    ');
            styles[0].getText().getFill().setColor('#324a5e');

            return styles;
        }
    });

    var parquetLayer = new ol.layer.Vector({
        source: parquetVectorSource,
        style: function (feature) {

            styles[0].setImage(new ol.style.Icon({
                anchor: [0.5, 0.5],
                color: 'white',
                scale: 0.05,
                crossOrigin: 'anonymous',
                src: '' + feature.get('iconSrc')
            }));

            styles[0].getText().setText(feature.get('name') + '    ');
            styles[0].getText().getFill().setColor('#324a5e');

            return styles;
        }
    });

    var prisonLayer = new ol.layer.Vector({
        source: prisonVectorSource,
        style: function (feature) {

            styles[0].setImage(new ol.style.Icon({
                anchor: [0.5, 0.5],
                color: 'white',
                scale: 0.05,
                crossOrigin: 'anonymous',
                src: '' + feature.get('iconSrc')
            }));

            styles[0].getText().setText(feature.get('name') + '    ');
            styles[0].getText().getFill().setColor('#324a5e');

            return styles;
        }
    });

    var mapDatas = $('#map-datas').data('json');
    mapDatas.forEach(function (feature) {
        if (popupContent != null) {
            var f = new ol.Feature({
                geometry: new Point(ol.proj.transform([feature.Coords.Longitude, feature.Coords.Latitude], 'EPSG:4326', 'EPSG:3857')),
                name: feature.Name,
                departement: feature.Departement,
                tribunalType: feature.TribunalType,
                juridiction: feature.Juridiction,
                featureAdded: true,
                iconSrc: $("#tribunal-icon").data('url')
            });
            tribunalVectorSource.addFeature(f);
        }
        else if (popupContentParquet != null) {
            var parquet = new ol.Feature({
                geometry: new Point(ol.proj.transform([feature.Coords.Longitude, feature.Coords.Latitude], 'EPSG:4326', 'EPSG:3857')),
                name: feature.Name,
                departement: feature.Departement,
                juridiction: feature.Juridiction,
                featureAdded: true,
                iconSrc: $("#parquet-icon").data('url')
            });
            parquetVectorSource.addFeature(parquet);
        }
        else if (popupContentPrison != null) {

            var prison = new ol.Feature({
                geometry: new Point(ol.proj.transform([feature.Coords.Longitude, feature.Coords.Latitude], 'EPSG:4326', 'EPSG:3857')),
                name: feature.Name,
                departement: feature.Departement,
                typePrison: feature.TypePrison,
                capacite: feature.Capacite,
                juridiction: feature.Juridiction,
                featureAdded: true,
                iconSrc: $("#prison-icon").data('url')
            });
            prisonVectorSource.addFeature(prison);
        }


    });

    $('#country-map').html('');

    var baseMap = new TileLayer({
        source: new ol.source.XYZ({
            url: 'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
            maxZoom: 19
        })
    });

    const overlay = new Overlay({
        element: popupContainer,
        autoPan: {
            animation: {
                duration: 250,
            },
        },
    });

    const drawFeatureSource = new VectorSource({ wrapX: false });

    const drawFeatureVector = new VectorLayer({
        source: drawFeatureSource,
    });

    const map = new Map({
        interactions: ol.interaction.defaults().extend([new ol.interaction.DragRotateAndZoom()]),
        layers: [
            baseMap,
            tribunalLayer,
            prisonLayer,
            parquetLayer,
            drawFeatureVector
        ],
        overlays: [overlay],
        target: 'country-map',
        view: new View({
            center: [lat, lon],
            zoom: zoom,
        }),
    });

    $('#pv-overlay-loading').fadeIn();

    map.getView().setCenter(ol.proj.transform([lon, lat], 'EPSG:4326', 'EPSG:3857'));

    map.on('singleclick', function (evt) {
        const coordinate = evt.coordinate;

        const feature = map.forEachFeatureAtPixel(evt.pixel, function (feature) {
            return feature;
        });

        popupContainer.hidden = true;

        if (feature && feature.get('featureAdded')) {

            overlay.setPosition(coordinate);
            //map.getView().setCenter(coordinate);
          
            if (popupContent != null) {
                popupContent.innerHTML = '<p style="font-weight: bold; color: #324a5e; margin: 0; margin-top: 15px; border-bottom: 1px solid lightgrey; padding-bottom: 8px;">' + feature.get('name') + '</p>' +
                    '<p> Département : ' + feature.get('departement') + '</p>' +
                    '<p> Juridiction : ' + feature.get('juridiction') + '</p>' +
                    '<p> Type : ' + feature.get('tribunalType') + ' </p>';


            } else if (popupContentParquet != null) {
                //parquet
                popupContentParquet.innerHTML = '<p style="font-weight: bold; color: #324a5e; margin: 0; margin-top: 15px; border-bottom: 1px solid lightgrey; padding-bottom: 8px;">' + feature.get('name') + '</p>' +
                    '<p> Département : ' + feature.get('departement') + '</p>' +
                    '<p> Juridiction : ' + feature.get('juridiction') + '</p>';

            } else if (popupContentPrison != null) {
                //prison
                popupContentPrison.innerHTML = '<p style="font-weight: bold; color: #324a5e; margin: 0; margin-top: 15px; border-bottom: 1px solid lightgrey; padding-bottom: 8px;">' + feature.get('name') + '</p>' +
                    '<p> Département : ' + feature.get('departement') + '</p>' +
                    '<p> Juridiction : ' + feature.get('juridiction') + '</p>' +
                    '<p> Type Prison: ' + feature.get('typePrison') + ' </p>' +
                    '<p> Capacité: ' + feature.get('capacite') + ' </p>';
                    ;
            }

            popupContainer.hidden = false;
        }
        else {

            if (($('#draw-feature-option-val').val() + '').toLowerCase() === 'none') {
                let currentCoords = ol.proj.transform([evt.coordinate[0], evt.coordinate[1]], 'EPSG:3857', 'EPSG:4326');

                let lat = currentCoords[1];
                let lon = currentCoords[0];

                overlay.setPosition(coordinate);
                if (popupContent != null && popupContent.length) {
                    popupContent.innerHTML = '<p> Lat : ' + lat + '</p>' +
                        '<p> Lon : ' + lon + '</p>';
                    popupContainer.hidden = false;
                } else if (popupContentParquet != null && popupContentParquet.length) {
                    popupContentParquet.innerHTML = '<p> Lat : ' + lat + '</p>' +
                        '<p> Lon : ' + lon + '</p>';
                    popupContainer.hidden = false;
                }
                else if (popupContentPrison != null && popupContentPrison.length) {

                    popupContentPrison.innerHTML = '<p> Lat : ' + lat + '</p>' +
                        '<p> Lon : ' + lon + '</p>';
                    popupContainer.hidden = false;
                }


            }

        }

    });


    $(document).on("click", ".map-layer-control-option", function () {

        $('.map-layer-control-option').removeClass('active');
        $(this).addClass('active');

        if ($(this).data('code') == "sat") {
            baseMap.setSource(new ol.source.XYZ({
                url: 'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
                maxZoom: 19
            }));
        }
        else {
            baseMap.setSource(new OSM());
        }

    });

    let draw;
    let sketch;
    let helpTooltipElement;
    let helpTooltip;
    let measureTooltipElement;
    let measureTooltip;
    const continuePolygonMsg = 'Cliquez pour continuer à dessiner le polygone';
    const continueLineMsg = 'Cliquez pour continuer à dessiner la ligne';

    const pointerMoveHandler = function (evt) {

        let currentCoords = ol.proj.transform([evt.coordinate[0], evt.coordinate[1]], 'EPSG:3857', 'EPSG:4326');

        let lat = currentCoords[1];
        let lon = currentCoords[0];

        $('#lat-cursor-current-pos').text(lat);
        $('#lon-cursor-current-pos').text(lon);

        if (evt.dragging) {
            return;
        }

        let helpMsg = 'Cliquez pour commencer le dessin';

        if (sketch) {
            const geom = sketch.getGeometry();
            if (geom instanceof Polygon) {
                helpMsg = continuePolygonMsg;
            } else if (geom instanceof LineString) {
                helpMsg = continueLineMsg;
            }
        }

        if (helpTooltipElement) {
            helpTooltipElement.innerHTML = helpMsg;
            helpTooltipElement.classList.remove('hidden');
        }

        if (helpTooltip) {
            helpTooltip.setPosition(evt.coordinate);
        }

    };

    map.on('pointermove', pointerMoveHandler);

    map.getViewport().addEventListener('mouseout', function () {
        if (helpTooltipElement) {
            helpTooltipElement.classList.add('hidden');
        }
    });

    const formatLength = function (line) {
        const length = getLength(line);
        let output;
        if (length > 100) {
            output = Math.round((length / 1000) * 100) / 100 + ' ' + 'km';
        } else {
            output = Math.round(length * 100) / 100 + ' ' + 'm';
        }
        return output;
    };


    const formatArea = function (polygon) {
        const area = getArea(polygon);
        let output;
        if (area > 10000) {
            output = Math.round((area / 1000000) * 100) / 100 + ' ' + 'km<sup>2</sup>';
        } else {
            output = Math.round(area * 100) / 100 + ' ' + 'm<sup>2</sup>';
        }
        return output;
    };


    function addInteraction() {

        const value = $('#draw-feature-option-val').val();

        if (value !== 'None') {
            draw = new Draw({
                source: drawFeatureSource,
                type: value,
                style: new Style({
                    fill: new Fill({
                        color: 'rgba(255, 255, 255, 0.2)',
                    }),
                    stroke: new Stroke({
                        color: 'rgba(0, 0, 0, 0.5)',
                        lineDash: [10, 10],
                        width: 2,
                    }),
                    image: new CircleStyle({
                        radius: 5,
                        stroke: new Stroke({
                            color: 'rgba(0, 0, 0, 0.7)',
                        }),
                        fill: new Fill({
                            color: 'rgba(255, 255, 255, 0.2)',
                        }),
                    }),
                }),
            });
            map.addInteraction(draw);

            createMeasureTooltip();
            createHelpTooltip();

            let listener;
            draw.on('drawstart', function (evt) {

                sketch = evt.feature;

                let tooltipCoord = evt.coordinate;

                listener = sketch.getGeometry().on('change', function (evt) {
                    const geom = evt.target;
                    let output;
                    if (geom instanceof Polygon) {
                        output = formatArea(geom);
                        tooltipCoord = geom.getInteriorPoint().getCoordinates();
                    } else if (geom instanceof LineString) {
                        output = formatLength(geom);
                        tooltipCoord = geom.getLastCoordinate();
                    }
                    measureTooltipElement.innerHTML = output;
                    measureTooltip.setPosition(tooltipCoord);
                });
            });

            draw.on('drawend', function () {
                measureTooltipElement.className = 'ol-tooltip ol-tooltip-static';
                measureTooltip.setOffset([0, -7]);
                sketch = null;
                measureTooltipElement = null;
                createMeasureTooltip();
                unByKey(listener);
            });

        }

    }


    function createHelpTooltip() {
        if (helpTooltipElement) {
            helpTooltipElement.parentNode.removeChild(helpTooltipElement);
        }
        helpTooltipElement = document.createElement('div');
        helpTooltipElement.className = 'ol-tooltip hidden';
        helpTooltip = new Overlay({
            element: helpTooltipElement,
            offset: [15, 0],
            positioning: 'center-left',
        });
        map.addOverlay(helpTooltip);
    }

    //Creates a new measure tooltip
    function createMeasureTooltip() {
        if (measureTooltipElement) {
            measureTooltipElement.parentNode.removeChild(measureTooltipElement);
        }
        measureTooltipElement = document.createElement('div');
        measureTooltipElement.className = 'ol-tooltip ol-tooltip-measure';
        measureTooltip = new Overlay({
            element: measureTooltipElement,
            offset: [0, -15],
            positioning: 'bottom-center',
            stopEvent: false,
            insertFirst: false,
        });
        map.addOverlay(measureTooltip);
    }


    $(document).on("click", ".draw-feature-option", function () {

        $('.draw-feature-option').removeClass('active');

        if ($(this).data('val') != $('#draw-feature-option-val').val()) {
            $('#draw-feature-option-val').val($(this).data('val'));
            $(this).addClass('active');
        }
        else {
            $('#draw-feature-option-val').val('None');
            map.removeOverlay(helpTooltip);
        }

        map.removeInteraction(draw);

        if (helpTooltipElement) {
            helpTooltipElement.innerHTML = '';
            helpTooltipElement.classList.add('hidden');
        }

        addInteraction();

    });


    $(document).on("click", "#reset-draw-feature", function () {

        drawFeatureSource.clear();

        map.getOverlays().clear();
        map.addOverlay(overlay);

        map.removeInteraction(draw);
        addInteraction();

    });


});

