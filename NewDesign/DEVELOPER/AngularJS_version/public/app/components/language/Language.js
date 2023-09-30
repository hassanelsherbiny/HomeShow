define(['app'], function(app){
    "use strict";

    return app.factory('Language', function($http){

		function getLanguage(key, callback) {

			$http.get('api/langs/' + key + '.json').success(function(data){

				callback(data);
				
			}).error(function(){

				$log.log('Error');
				callback([]);

			});

		}

		function getLanguages(callback) {

			$http.get('api/languages.json').success(function(data){

				callback(data);
				
			}).error(function(){

				$log.log('Error');
				callback([]);

			});

		}

		return {
			getLang: function(type, callback) {
				getLanguage(type, callback);
			},
			getLanguages:function(callback){
				getLanguages(callback);
			}
		}

    })
})
