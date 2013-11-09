using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using System.Reflection;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using System.Timers;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI;
using System.Collections.Generic;
using OsmSharp.Math;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.TSP.Genetic;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Animations.Navigation;
using OsmSharp.Routing.Navigation;
using OsmSharp.Routing.Instructions;
using System.Threading.Tasks;

namespace OsmSharp.iOS.UI.Sample
{
	public partial class OsmSharp_iOS_UI_SampleViewController : UIViewController
	{
		/// <summary>
		/// Holds the router.
		/// </summary>
		private Router _router;

		private MapView _mapView;

		/// <summary>
		/// Holds the route layer.
		/// </summary>
		private LayerRoute _routeLayer;

		public OsmSharp_iOS_UI_SampleViewController () : base ("OsmSharp_iOS_UI_SampleViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void LoadView ()
		{
			OsmSharp.Logging.Log.Enable ();
//            OsmSharp.Logging.Log.RegisterListener(
//                new OsmSharp.Android.UI.Log.LogTraceListener());
			base.LoadView ();

			// initialize a test-map.
			var map = new Map ();
			map.AddLayer (new LayerScene (Scene2DLayered.Deserialize (
				Assembly.GetExecutingAssembly ().GetManifestResourceStream (
                    "OsmSharp.iOS.UI.Sample.kempen-big.osm.pbf.scene.layered"), 
					    true)));

			// Perform any additional setup after loading the view, typically from a nib.
			MapView mapView = new MapView ();
			_mapView = mapView;
			//mapViewAnimator = new MapViewAnimator (mapView);
			mapView.Map = map;
            mapView.MapCenter = new GeoCoordinate(51.26371, 4.78601); // wechel
//			mapView.MapTapEvent+= delegate(GeoCoordinate geoCoordinate) {
//				mapView.AddMarker(geoCoordinate).TouchDown  += MapMarkerClicked;
//			};

			mapView.MapMaxZoomLevel = 18;
			mapView.MapMinZoomLevel = 12;
			mapView.MapZoom = 16;
			mapView.MapTilt = 30;

			var routingSerializer = new OsmSharp.Routing.CH.Serialization.Sorted.v2.CHEdgeDataDataSourceSerializer(false);
			var graphDeserialized = routingSerializer.Deserialize(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.iOS.UI.Sample.kempen-big.osm.pbf.routing"), true);

			_router = Router.CreateCHFrom(
				graphDeserialized, new CHRouter(),
				new OsmRoutingInterpreter());

            var from = new GeoCoordinate(51.261203, 4.780760);
            var to = new GeoCoordinate(51.267797, 4.801362);

			RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, from);
			RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, to);
			Route route1 = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
			_enumerator = route1.GetRouteEnumerable(10).GetEnumerator();

			_routeLayer = new LayerRoute(map.Projection);
			_routeLayer.AddRoute (route1);
			map.AddLayer(_routeLayer);

			View = mapView;

			mapView.AddMarker(from);
			mapView.AddMarker(to);
			
			RouteTracker routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());
			_routeTrackerAnimator = new RouteTrackerAnimator(mapView, routeTracker, 5);

            Timer timer = new Timer (250);
            timer.Elapsed += new ElapsedEventHandler (TimerHandler);
            timer.Start ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			
			//_mapView.ZoomToMarkers();
			//
			(_mapView as IMapView).Invalidate ();
		}

		private RouteTrackerAnimator _routeTrackerAnimator;

		private IEnumerator<GeoCoordinate> _enumerator;

		private void TimerHandler(object sender, ElapsedEventArgs e)
		{
			this.InvokeOnMainThread (MoveNext);
		}

		private void MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				GeoCoordinate other = _enumerator.Current.OffsetRandom(10);
				_routeTrackerAnimator.Track(other);

				if (_routeTrackerAnimator.NextInstruction != null) {
					OsmSharp.Logging.Log.TraceEvent ("SampleView", System.Diagnostics.TraceEventType.Information,
					                                _routeTrackerAnimator.NextInstruction.Text);
				}
			}
		}

		private void IncreaseMapTilt()
		{
			//(this.View as MapView).MapTilt = (this.View as MapView).MapTilt + 5;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return true;
		}
	}
}