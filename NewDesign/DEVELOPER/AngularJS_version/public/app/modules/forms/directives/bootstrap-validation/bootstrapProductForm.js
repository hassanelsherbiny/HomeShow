define(['modules/forms/module','bootstrap-validator'], function(module){

    "use strict";


    module.registerDirective('bootstrapProductForm', function(){

        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'app/modules/forms/directives/bootstrap-validation/bootstrap-product-form.tpl.html',
            link: function(scope, form){
                form.bootstrapValidator({
                    feedbackIcons : {
                        valid : 'glyphicon glyphicon-ok',
                        invalid : 'glyphicon glyphicon-remove',
                        validating : 'glyphicon glyphicon-refresh'
                    },
                    fields : {
                        price : {
                            validators : {
                                notEmpty : {
                                    message : 'The price is required'
                                },
                                numeric : {
                                    message : 'The price must be a number'
                                }
                            }
                        },
                        amount : {
                            validators : {
                                notEmpty : {
                                    message : 'The amount is required'
                                },
                                numeric : {
                                    message : 'The amount must be a number'
                                }
                            }
                        },
                        color : {
                            validators : {
                                notEmpty : {
                                    message : 'The color is required'
                                }
                            }
                        },
                        size : {
                            validators : {
                                notEmpty : {
                                    message : 'The size is required'
                                }
                            }
                        }
                    }
                });
;

            }

        }



    })


});