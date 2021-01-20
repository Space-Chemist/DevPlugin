using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Torch;

namespace KothPlugin
{
    internal static class WebService
    {
        private const string PageHtml =
            "<!DOCTYPE html>\r\n<html lang=\"en\" ng-app=\"demoApp\">\r\n<head>\r\n\r\n    <meta charset=\"utf-8\">\r\n    <meta content=\"IE=edge\" http-equiv=\"X-UA-Compatible\">\r\n    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\r\n    <title>KOTH score Tracker</title>\r\n    <link href=\"https://ajax.googleapis.com/ajax/libs/angular_material/1.0.0/angular-material.min.css\" rel=\"stylesheet\">\r\n    <link href=\"https://fonts.googleapis.com/icon?family=Material+Icons\" rel=\"stylesheet\">\r\n    <link href=\"https://rawgit.com/daniel-nagy/md-data-table/master/dist/md-data-table.css\" rel=\"stylesheet\">\r\n\r\n</head>\r\n\r\n<body layout=\"column\">\r\n\r\n<md-toolbar class=\"md-whiteframe-1dp\">\r\n    <div class=\"md-toolbar-tools\">\r\n        <div class=\"md-title\">KOTH score Tracker</div>\r\n    </div>\r\n</md-toolbar>\r\n\r\n<md-content flex laout=\"column\" ng-controller=\"scoreController\">\r\n\r\n    <md-card>\r\n\r\n        <md-toolbar class=\"md-table-toolbar md-default\" ng-hide=\"options.rowSelection && selected.length\">\r\n            <div class=\"md-toolbar-tools\">\r\n                <span>KOTH</span>\r\n                <div flex></div>\r\n                <md-button class=\"md-icon-button\" ng-click=\"loadStuff()\">\r\n                    <md-icon>refresh</md-icon>\r\n                </md-button>\r\n            </div>\r\n        </md-toolbar>\r\n\r\n        <md-toolbar class=\"md-table-toolbar alternate\" ng-show=\"options.rowSelection && selected.length\">\r\n            <div class=\"md-toolbar-tools\">\r\n                <span>{{ selected.length }} {{ selected.length > 1 ? 'items' : 'item'}} selected</span>\r\n            </div>\r\n        </md-toolbar>\r\n\r\n        <md-table-container>\r\n            <table md-progress=\"promise\" md-row-select=\"options.rowSelection\" md-table\r\n                   multiple=\"{{ options.multiSelect }}\" ng-model=\"selected\">\r\n                <thead md-head md-on-reorder=\"logOrder\" md-order=\"query.order\" ng-if=\"!options.decapitate\">\r\n                <tr md-row>\r\n                    <th md-column md-order-by=\"Planet\"><span>Planet</span></th>\r\n                    <th md-column md-desc md-numeric md-order-by=\"FactionName\"><span>Faction</span></th>\r\n                    <th md-column md-numeric md-order-by=\"FactionTag\"><span>Tag</span></th>\r\n                    <th md-column md-numeric md-order-by=\"Points\"><span>Points</span></th>\r\n                </tr>\r\n                </thead>\r\n                <tbody md-body>\r\n                <tr md-auto-select=\"options.autoSelect\" md-on-select=\"logItem\" md-row md-select=\"score\"\r\n                    ng-disabled=\"score.FactionName > 400\"\r\n                    ng-repeat=\"score in scores.data | filter: filter.search | orderBy: query.order | limitTo: query.limit : (query.page -1) * query.limit\">\r\n                    <td md-cell>\r\n                        {{ score.PlanetId }}\r\n                    </td>\r\n                    <td md-cell>{{ score.FactionName }}</td>\r\n                    <td md-cell>{{ score.FactionTag }}</td>\r\n                    <td md-cell>{{ score.Points }}</td>\r\n                </tr>\r\n                </tbody>\r\n            </table>\r\n        </md-table-container>\r\n\r\n        <md-table-pagination md-boundary-links=\"options.boundaryLinks\" md-limit=\"query.limit\"\r\n                             md-limit-options=\"limitOptions\" md-on-paginate=\"logPagination\" md-page=\"query.page\"\r\n                             md-page-select=\"options.pageSelect\" md-total=\"{{ scores.count }}\"></md-table-pagination>\r\n    </md-card>\r\n</md-content>\r\n\r\n<script src=\"https://ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular.min.js\"></script>\r\n<script src=\"https://ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular-animate.min.js\"></script>\r\n<script src=\"https://ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular-aria.min.js\"></script>\r\n<script src=\"https://ajax.googleapis.com/ajax/libs/angular_material/1.0.0/angular-material.min.js\"></script>\r\n<script src=\"https://rawgit.com/daniel-nagy/md-data-table/master/dist/md-data-table.js\"></script>\r\n<script>\r\n    angular.module('demoApp', ['ngMaterial', 'md.data.table'])\r\n\r\n        .config(['$mdThemingProvider', function ($mdThemingProvider) {\r\n            'use strict';\r\n\r\n            $mdThemingProvider.theme('default')\r\n                .primaryPalette('deep-purple');\r\n        }])\r\n\r\n        .controller('scoreController', ['$mdEditDialog', '$q', '$scope', '$http', '$timeout', function ($mdEditDialog, $q, $scope, $http, $timeout) {\r\n            'use strict';\r\n\r\n            $scope.yes = REPLACEMEYES\r\n\r\n            $scope.data = []\r\n            $scope.yes.WorldScores.map(m => m.WorldDescription).forEach(function (e) {\r\n                    e.map(d => d.Scores).forEach(function(l){\r\n                        $scope.data.push(l.map(dd=> dd.ScoreDescription))\r\n                    })\r\n                }\r\n            )\r\n            $scope.data = $scope.data.map(m=>m[0])\r\n            $scope.scores = {\r\n                \"count\": $scope.data[0].length,\r\n                \"data\": $scope.data[0]\r\n            };\r\n\r\n            console.log($scope.data)\r\n            $scope.selected = [];\r\n            $scope.limitOptions = [5, 10, 15];\r\n\r\n            $scope.options = {\r\n                rowSelection: false,\r\n                multiSelect: false,\r\n                autoSelect: false,\r\n                decapitate: false,\r\n                largeEditDialog: false,\r\n                boundaryLinks: false,\r\n                limitSelect: true,\r\n                pageSelect: true\r\n            };\r\n\r\n            $scope.query = {\r\n                order: 'score',\r\n                limit: 5,\r\n                page: 1\r\n            };\r\n\r\n        }]);\r\n</script>\r\n</body>\r\n</html>\r\n";

        private static readonly HttpListener Listener = new HttpListener {Prefixes = {$"http://{Koth.Instance.Config.Host}:{Koth.Instance.Config.Port}/"}};

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static bool _keepGoing = true;

        private static Task _mainLoop;

        public static void StartWebServer()
        {
            if (_mainLoop != null && !_mainLoop.IsCompleted) return;
            _mainLoop = ServerLoop();
        }

        public static void StopWebServer()
        {
            _keepGoing = false;
            lock (Listener)
            {
                Listener.Stop();
            }

            try
            {
                _mainLoop.Wait();
            }
            catch (Exception error)
            {
                Log.Error(error, "Server error");
            }
        }

        private static async Task ServerLoop()
        {
            Listener.Start();
            while (_keepGoing)
            {
                try
                {
                    var context = await Listener.GetContextAsync();
                    lock (Listener)
                    {
                        if (_keepGoing) ProcessRequest(context);
                    }
                }
                catch (Exception e)
                {
                    if (e is HttpListenerException) return;
                    Log.Error(e, "Server error");
                }
            }
        }


        private static void ProcessRequest(HttpListenerContext context)
        {
            using (var response = context.Response)
            {
                try
                {
                    var handled = false;
                    switch (context.Request.Url.AbsolutePath)
                    {
                        case "/json":
                            switch (context.Request.HttpMethod)
                            {
                                case "GET":

                                    if (Koth.Instance.Config.ApiEnabled)
                                    {
                                        response.ContentType = "application/json";
                                        var data = Koth.ScoresFromStorage();
                                        var responseBody = JsonConvert.SerializeObject(data);
                                        var buffer = Encoding.UTF8.GetBytes(responseBody);
                                        response.ContentLength64 = buffer.Length;
                                        response.OutputStream.Write(buffer, 0, buffer.Length);
                                        handled = true;
                                        break;
                                    }

                                    else
                                    {
                                        break;
                                    }
                            }

                            break;


                        case "/koth":
                            switch (context.Request.HttpMethod)
                            {
                                case "GET":
                                    if (Koth.Instance.Config.WebServerEnabled)
                                    {
                                        var kothdata = Koth.ScoresFromStorage();
                                        var data = Encoding.UTF8.GetBytes(PageHtml.Replace("REPLACEMEYES",
                                            JsonConvert.SerializeObject(kothdata)));
                                        response.ContentType = "text/html";
                                        response.ContentEncoding = Encoding.UTF8;
                                        response.ContentLength64 = data.LongLength;
                                        response.OutputStream.Write(data, 0, data.Length);
                                        handled = true;
                                        break;
                                    }

                                    else
                                    {
                                        break;
                                    }
                            }

                            break;
                    }

                    if (!handled)
                    {
                        response.StatusCode = 404;
                    }
                }
                catch (Exception e)
                {
                    response.StatusCode = 500;
                    response.ContentType = "application/json";
                    var buffer = Encoding.UTF8.GetBytes("{\"msg\": 500}");
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    Log.Error(e, "Server error");
                }
            }
        }
    }
}