define(['modules/forms/module', 'summernote'], function (module) {

    'use strict';

    module.registerDirective('smartSummernoteEditor', function () {
        return {
            restrict: 'A',
            compile: function (tElement, tAttributes) {
                tElement.removeAttr('smart-summernote-editor data-smart-summernote-editor');

                var options = {
                    focus : true,
                    tabsize : 2
                };

                if(tAttributes.height){
                    options.height = tAttributes.height;
                }

                tElement.summernote(options);
            }
        }
    });
});
