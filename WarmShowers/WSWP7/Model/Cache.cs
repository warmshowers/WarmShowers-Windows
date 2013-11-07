using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using WSApp.Utility;


namespace WSApp.DataModel
{
    public class ViewportCache
    {
        public int resultLimit = 100;
        LocationRect lastViewport = new LocationRect();
        GeoCoordinate lastCenter = null;
        int lastResults = 0;
//        bool firstTime = true;
        public bool isMapInitialized { get; private set; }

        // MapPolyline line2 = new MapPolyline();
        // MapPolyline line = new MapPolyline();

        enum Zoomed
        {
            Out,
            In,
            panned,
            noChange
        }

        // Constructor
        public ViewportCache()
        {
            isMapInitialized = false;
        }

        private Zoomed checkNorth(double North, Zoomed zoom)
        {
            Zoomed newZoom = Zoomed.noChange;

            if (North != lastViewport.North)
            {
                if (North > lastViewport.North)
                    newZoom = Zoomed.Out;
                else
                    newZoom = Zoomed.In;
            }

            if ((zoom != Zoomed.noChange) && (newZoom != zoom))
            {
                newZoom = Zoomed.panned;
            }

            return newZoom;
        }

        private Zoomed checkSouth(double South, Zoomed zoom)
        {
            Zoomed newZoom = Zoomed.noChange;

            if (South != lastViewport.South)
            {
                if (South < lastViewport.South)
                    newZoom = Zoomed.Out;
                else
                    newZoom = Zoomed.In;
            }

            if ((zoom != Zoomed.noChange) && (newZoom != zoom))
            {
                newZoom = Zoomed.panned;
            }

            return newZoom;
        }

        private Zoomed checkEast(double East, Zoomed zoom)
        {
            Zoomed newZoom = Zoomed.noChange;

            if (East != lastViewport.East)
            {
                if (East > lastViewport.East)
                    newZoom = Zoomed.Out;
                else
                    newZoom = Zoomed.In;
            }

            if ((zoom != Zoomed.noChange) && (newZoom != zoom))
            {
                newZoom = Zoomed.panned;
            }

            return newZoom;
        }

        private Zoomed checkWest(double West, Zoomed zoom)
        {
            Zoomed newZoom = Zoomed.noChange;

            if (West != lastViewport.West)
            {
                if (West < lastViewport.West)
                    newZoom = Zoomed.Out;
                else
                    newZoom = Zoomed.In;
            }

            if ((zoom != Zoomed.noChange) && (newZoom != zoom))
            {
                newZoom = Zoomed.panned;
            }

            return newZoom;
        }

        /// <summary>
        /// Bypass cache to force a hosts request
        /// </summary>
        public bool getHostsForce(GeoCoordinate newCenter)
        {
            bool callSuccessful = WebService.GetHosts();
            if (callSuccessful)
            {
                lastResults = 0;
                if (null != newCenter)
                {
                    lastCenter = newCenter;
                    lastViewport.North = lastCenter.Latitude;
                    lastViewport.South = lastCenter.Latitude;
                    lastViewport.East = lastCenter.Longitude;
                    lastViewport.West = lastCenter.Longitude;
                }
                //                System.Diagnostics.Debug.WriteLine("N" + lastViewport.North + " S" + lastViewport.South + " E" + lastViewport.East + " W" + lastViewport.West);
            }
            return callSuccessful;
        }

        public bool isInside(GeoCoordinate center, LocationRect bounds)
        {
            if ((null == center) || (null == bounds)) return false;
            if (center.Latitude < bounds.South) return false;
            if (center.Latitude > bounds.North) return false;
            if (center.Longitude < bounds.West) return false;
            if (center.Longitude > bounds.East) return false;
            isMapInitialized = true;
            return true;
        }

        /// <summary>
        /// Normal entry point for hosts request, with viewport caching
        /// </summary>
        public void getHosts()
        {
            if (App.ViewModelMain.debug)
            {   // Debug: Bypass viewport cache. 
                getHostsForce(App.nearby.mapCenter);
                return;
            }

            Zoomed zoom = Zoomed.noChange;
            LocationRect newViewport = App.nearby.locationRect;
            GreatCircle gc = new GreatCircle();

            // This sanity check avoids some map initialization problems in WP8
            if (!isInside(App.nearby.mapCenter, newViewport)) return;

//            if (mapInit)
//            {   // Skip first query after map initialization
//                mapInit = false;
//                return;
//            }

            /*
                       Deployment.Current.Dispatcher.BeginInvoke(() =>
                         {
            //App.nearby.myMap.Children.Remove(line);
                             line = new MapPolyline();
            line.Locations = new LocationCollection();
            line.Stroke = new SolidColorBrush(Colors.Blue);
            line.StrokeThickness = 5;
            line.Opacity = 0.7;
            LocationRect loc = newViewport;
            line.Locations.Add(loc.Northwest);
            line.Locations.Add(loc.Northeast);
            line.Locations.Add(loc.Southeast);
            line.Locations.Add(loc.Southwest);
            line.Locations.Add(loc.Northwest);
            App.nearby.myMap.Children.Add(line);
                         });
             */

/*            if (firstTime)
            {   // First request after startup
                if (getHosts2(App.ViewModelMain.mapLocation))
                {
                    firstTime = false;
                }
                return;
            }
*/
            GeoCoordinate newCenter = App.ViewModelMain.mapLocation;

            zoom = checkNorth(newViewport.North, zoom);
            if (Zoomed.panned != zoom)
            {
                zoom = checkSouth(newViewport.South, zoom);
                if (Zoomed.panned != zoom)
                {
                    zoom = checkEast(newViewport.East, zoom);
                    if (Zoomed.panned != zoom)
                    {
                        zoom = checkWest(newViewport.West, zoom);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Count: " + lastResults);

            switch (zoom)
            {
                case Zoomed.Out:
                    if (!gc.isEqual(newCenter, lastCenter))
                    {   // If zoomed out, keep pushpins in the middle
                        getHostsForce(newCenter);
                        zoom = Zoomed.panned;
                    }
                    else if (lastResults < resultLimit)
                    {   // Otherwise query only if we haven't hit the limit yet
                        getHostsForce(newCenter);
                    }
                    break;
                case Zoomed.panned:
                    getHostsForce(newCenter);

                    // This was causing map intitialization problems with WP8 device, putting it back in...
                    if (!gc.isEqual(newCenter, lastCenter))
                    {
                        getHostsForce(newCenter);
                    }
                    else
                    {   // This isn't a pan, but a zoom in that straddles an asymmetric lastViewport
                        zoom = Zoomed.In;
                    }
                     
                    break;
                case Zoomed.In:         // Fall-thru
                case Zoomed.noChange:   // Fall-thru
                default:
                    break;
            }
            System.Diagnostics.Debug.WriteLine("Zoomed: " + zoom);
        }

        public void hostLocation(GeoCoordinate loc)
        {
            double lat = loc.Latitude;
            double lon = loc.Longitude;

            if (lat > lastViewport.North) lastViewport.North = lat;
            else if (lat < lastViewport.South) lastViewport.South = lat;

            if (lon > lastViewport.East) lastViewport.East = lon;
            else if (lon < lastViewport.West) lastViewport.West = lon;

            lastResults++;
            /*
             System.Diagnostics.Debug.WriteLine("Results " + lastResults + " lat " + lat + " lon " + lon + " N" + lastViewport.North + " S" + lastViewport.South + " E" + lastViewport.East + " W" + lastViewport.West);
             Deployment.Current.Dispatcher.BeginInvoke(() =>
             {
                 App.nearby.myMap.Children.Remove(line2);

                 line2.Locations = new LocationCollection();
                 line2.Stroke = new SolidColorBrush(Colors.Red);
                 line2.StrokeThickness = 5;
                 line2.Opacity = 0.7;
                 LocationRect l = lastViewport;
                 line2.Locations.Add(new GeoCoordinate(l.North, l.West));
                 line2.Locations.Add(new GeoCoordinate(l.North, l.East));
                 line2.Locations.Add(new GeoCoordinate(l.South, l.East));
                 line2.Locations.Add(new GeoCoordinate(l.South, l.West));
                 line2.Locations.Add(new GeoCoordinate(l.North, l.West));
                 App.nearby.myMap.Children.Add(line2);
             });
             */
        }

//        public void mapInitialized()
//        {
//            isMapInitialized = true;
//        }
    }
}