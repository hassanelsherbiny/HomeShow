define(['modules/forms/module', 'jcrop'], function (module) {

    'use strict';

    return module.registerDirective('smartJcrop', function ($q) {
        return {
            restrict: 'A',
            scope: {
                coords: '=',
                options: '=',
                selection: '='
            },
            link: function (scope, element, attributes) {
                var jcropApi, imageWidth, imageHeight, imageLoaded = $q.defer();

                var listeners = {
                    onSelectHandlers: [],
                    onChangeHandlers: [],
                    onSelect: function (c) {
                        angular.forEach(listeners.onSelectHandlers, function (handler) {
                            handler.call(jcropApi, c)
                        })
                    },
                    onChange: function (c) {
                        angular.forEach(listeners.onChangeHandlers, function (handler) {
                            handler.call(jcropApi, c)
                        })
                    }
                };

                if (attributes.coords) {
                    var coordsUpdate = function (c) {
                        scope.$apply(function () {
                            scope.coords = c;
                        });
                    };
                    listeners.onSelectHandlers.push(coordsUpdate);
                    listeners.onChangeHandlers.push(coordsUpdate);
                }

                var $previewPane = $(attributes.smartJcropPreview),
                    $previewContainer = $previewPane.find('.preview-container'),
                    $previewImg = $previewPane.find('img');

                if ($previewPane.length && $previewImg.length) {
                    var previewUpdate = function (coords) {
                        if (parseInt(coords.w) > 0) {
                            var rx = $previewContainer.width() / coords.w;
                            var ry = $previewContainer.height() / coords.h;

                            $previewImg.css({
                                width: Math.round(rx * imageWidth) + 'px',
                                height: Math.round(ry * imageHeight) + 'px',
                                marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                                marginTop: '-' + Math.round(ry * coords.y) + 'px'
                            });
                        }
                    };
                    listeners.onSelectHandlers.push(previewUpdate);
                    listeners.onChangeHandlers.push(previewUpdate);
                }


                var options = {
                    onSelect: listeners.onSelect,
                    onChange: listeners.onChange
                };

                if ($previewContainer.length) {
                    options.aspectRatio = $previewContainer.width() / $previewContainer.height()
                }

                if (attributes.selection) {
                    scope.$watch('selection', function (newVal, oldVal) {
                        if (newVal != oldVal) {
                            var rectangle = newVal == 'release' ? [imageWidth / 2, imageHeight / 2, imageWidth / 2, imageHeight / 2] : newVal;

                            var callback = newVal == 'release' ? function () {
                                jcropApi.release();
                            } : angular.noop;

                            imageLoaded.promise.then(function () {
                                if (scope.options && scope.options.animate) {
                                    jcropApi.animateTo(rectangle, callback);
                                } else {
                                    jcropApi.setSelect(rectangle);
                                }
                            });
                        }
                    });
                }

                if (attributes.options) {

                    var optionNames = [
                        'bgOpacity', 'bgColor', 'bgFade', 'shade', 'outerImage',
                        'allowSelect', 'allowMove', 'allowResize',
                        'aspectRatio'
                    ];

                    angular.forEach(optionNames, function (name) {
                        if (scope.options[name])
                            options[name] = scope.options[name]

                        scope.$watch('options.' + name, function (newVal, oldVal) {
                            if (newVal != oldVal) {
                                imageLoaded.promise.then(function () {
                                    var update = {};
                                    update[name] = newVal;
                                    jcropApi.setOptions(update);
                                });
                            }
                        });

                    });


                    scope.$watch('options.disabled', function (newVal, oldVal) {
                        if (newVal != oldVal) {
                            if (newVal) {
                                jcropApi.disable();
                            } else {
                                jcropApi.enable();
                            }
                        }
                    });

                    scope.$watch('options.destroyed', function (newVal, oldVal) {
                        if (newVal != oldVal) {
                            if (newVal) {
                                jcropApi.destroy();
                            } else {
                                _init();
                            }
                        }
                    });

                    scope.$watch('options.src', function (newVal, oldVal) {
                        imageLoaded = $q.defer();
                        if (newVal != oldVal) {
                            jcropApi.setImage(scope.options.src, function () {
                                imageLoaded.resolve();
                            });
                        }
                    });

                    var updateSize = function(){
                        jcropApi.setOptions({
                            minSize: [scope.options.minSizeWidth, scope.options.minSizeHeight],
                            maxSize: [scope.options.maxSizeWidth, scope.options.maxSizeHeight]
                        });
                    };

                    scope.$watch('options.minSizeWidth', function (newVal, oldVal) {
                        if (newVal != oldVal) updateSize();
                    });
                    scope.$watch('options.minSizeHeight', function (newVal, oldVal) {
                        if (newVal != oldVal) updateSize();
                    });
                    scope.$watch('options.maxSizeWidth', function (newVal, oldVal) {
                        if (newVal != oldVal) updateSize();
                    });
                    scope.$watch('options.maxSizeHeight', function (newVal, oldVal) {
                        if (newVal != oldVal) updateSize();
                    });
                }

                var _init = function () {
                    element.Jcrop(options, function () {
                        jcropApi = this;
                        // Use the API to get the real image size
                        var bounds = this.getBounds();
                        imageWidth = bounds[0];
                        imageHeight = bounds[1];

                        if (attributes.selection && angular.isArray(scope.selection)) {
                            if (scope.options && scope.options.animate) {
                                jcropApi.animateTo(scope.selection);
                            } else {
                                jcropApi.setSelect(scope.selection);
                            }
                        }
                        imageLoaded.resolve();
                    });
                };

                _init()


            }
        }
    });
});
