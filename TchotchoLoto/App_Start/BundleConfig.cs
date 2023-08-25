using System.Web;
using System.Web.Optimization;

namespace TchotchoLoto
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862


        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-multiselect").Include(
                      "~/Scripts/bootstrap-multiselect.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart-js").Include(
                      "~/Scripts/moment.js",
                      "~/Content/Chart-js.2.9.3/Chart.min.js",
                      "~/Scripts/cj-plug-gantt.min.js",
                      //"~/Scripts/mapcontrol.js",
                      //"~/Scripts/modernizr-2.8.3.js",
                      "~/Content/Chart-js.2.9.3/chartjs-plugin-labels.js"));




            bundles.Add(new ScriptBundle("~/bundles/shp2geojson").Include(
                     "~/Scripts/shp2geojson/lib/proj4.js",
                     "~/Scripts/shp2geojson/lib/jszip.js",
                     "~/Scripts/shp2geojson/lib/jszip-utils.js",
                     "~/Scripts/shp2geojson/preprocess.js",
                     "~/Scripts/shp2geojson/preview.js"));


            bundles.Add(new ScriptBundle("~/bundles/OpenLayers").Include(
                      "~/Content/OpenLayers-6.9.0/ol.js"));

            bundles.Add(new ScriptBundle("~/bundles/mapcontrol").Include(
                      "~/Scripts/mapcontrol.js"));


            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Scripts/Site1.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content//leaflet/leaflet.css",
                      "~/Content/Buttons/css/buttons.bootstrap.min.css",
                      "~/Content/Data-Tables/css/dataTables.bootstrap.min.css",
                      "~/Content/Select-1.3.1/css/select.bootstrap.min.css",
                      "~/Content/awesome-markers/leaflet.awesome-markers.css",
                      "~/Content/Leaflet.Dialog-master/Leaflet.Dialog.css",
                      "~/Content/Site.css",
                      "~/Content/bootstrap-multiselect.css"));

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                      "~/Content/Data-Tables/js/jquery.dataTables.min.js",
                      "~/Content/Data-Tables/js/dataTables.bootstrap.min.js",
                      "~/Content/Select-1.3.1/js/dataTables.select.min.js",
                      "~/Content/Select-1.3.1/js/select.bootstrap.min.js",
                      "~/Content/Buttons/js/dataTables.buttons.min.js",
                      "~/Content/Buttons/js/buttons.bootstrap.min.js",
                      "~/Content/Buttons/js/buttons.html5.min.js",
                      "~/Content/Buttons/js/buttons.colVis.min.js",
                      "~/Content/Buttons/js/buttons.print.min.js",
                      "~/Content/PDF-Make/pdfmake.min.js",
                      "~/Content/PDF-Make/vfs_fonts.js",
                      "~/Content/JSZip/jszip.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet").Include(
                      "~/Content/leaflet/leaflet.js",
                      "~/Content/leaflet/plugins/leaflet.ajax.min.js",
                      "~/Content/awesome-markers/leaflet.awesome-markers.js",
                      "~/Content/Leaflet.Dialog-master/Leaflet.Dialog.js",
            "~/Content/Leaflet.Control.Custom-master/Leaflet.Control.Custom.js"));
        }
    }
}
