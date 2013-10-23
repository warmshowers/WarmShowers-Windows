using System;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;

namespace WSApp.Utility
{
    public class GreatCircle
    {
        const double R = 6371;                  // Radius of the earth
        const double MiPerKm = 0.621371192;     // Miles per km
        const double RadPerDeg = 0.0174532925;
        const double North = 0;
        const double East = 90 * RadPerDeg;
        const double South = 180 * RadPerDeg;
        const double West = 270 * RadPerDeg;

        /// <summary>
        /// Determine if two GeoLocations are close enough to be considered equal
        /// </summary>
        public bool isEqual(GeoCoordinate x, GeoCoordinate y)
        {
            double d = dDistance(x, y); /// (App.nearby.locationRect.Width + App.nearby.locationRect.Height);

            if (d < .01)
            {
                return true;
            }
            else
            {  
                return false;
            }
        }
 
        /// <summary>
        /// Calculate great circle distance from current location
        /// </summary>
        public string Distance(GeoCoordinate hostCenter)
        {
            string d;

            d = dDistance(hostCenter, App.nearby.meCenter).ToString("F1") + " ";

            if (App.settings.isMetric)
            {
                d += WebResources.km;
            }
            else
            {
                d += WebResources.mi;
            }
            return d;
        }

        /// <summary>
        /// Calculate great circle distance from current location
        /// </summary>
        public double dDistance(GeoCoordinate hostCenter)
        {
             return dDistance(hostCenter, App.nearby.meCenter);
        }
 
        /// <summary>
        /// Calculate great circle distance between two GeoCoordinates in mi or km.
        /// </summary>
        public string Distance(GeoCoordinate x, GeoCoordinate y)
        {
            string d;

            d = dDistance(x, y).ToString("F1") + " ";

            if (App.settings.isMetric)
            {
                d += WebResources.km;
            }
            else
            {
                d += WebResources.mi;
            }
            return d;
        }
 
        /// <summary>
        /// Calculate great circle distance between two GeoCoordinates in mi or km.
        /// </summary>
        public double dDistance(GeoCoordinate x, GeoCoordinate y)
        {   // Coordinates in degrees
            if (null == x) return 0.0;
            if (null == y) return 0.0;

            double lat1 = x.Latitude * RadPerDeg;
            double lon1 = x.Longitude * RadPerDeg;
            double lat2 = y.Latitude * RadPerDeg;
            double lon2 = y.Longitude * RadPerDeg;
            double d;

            d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * 
                Math.Cos(lat2) * Math.Cos(lon2 - lon1)) * R;

            if (System.Double.IsNaN(d))
            {
                d = 0.0;
            }
            
            if (!App.settings.isMetric) d *= MiPerKm;           
            return (d);   
        }

        /// <summary>
        /// Calculate rectanble bounding a location.
        /// </summary>
        public LocationRect BoundingRectangle(GeoCoordinate center, double d)
        {
            LocationRect lr = new LocationRect();
            double lat1 = center.Latitude * RadPerDeg;
            double lon1 = center.Longitude * RadPerDeg;
            double dOverR = d / R;

  //          var lat2 = Math.asin(Math.sin(lat1) * Math.cos(d / R) +    Math.cos(lat1) * Math.sin(d / R) * Math.cos(brng));

            lr.North = Math.Asin(Math.Sin(lat1) * Math.Cos(dOverR) + Math.Cos(lat1) * Math.Sin(dOverR) * Math.Cos(North));
            lr.South = Math.Asin(Math.Sin(lat1) * Math.Cos(dOverR) + Math.Cos(lat1) * Math.Sin(dOverR) * Math.Cos(South));
            lr.East = lon1 + Math.Atan2(Math.Sin(East) * Math.Sin(dOverR) * Math.Cos(lat1), Math.Cos(dOverR) - Math.Sin(lat1) * Math.Sin(lat1));
            lr.West = lon1 + Math.Atan2(Math.Sin(West) * Math.Sin(dOverR) * Math.Cos(lat1), Math.Cos(dOverR) - Math.Sin(lat1) * Math.Sin(lat1));

            lr.North /= RadPerDeg;
            lr.South /= RadPerDeg;
            lr.East /= RadPerDeg;
            lr.West /= RadPerDeg;

            return lr;
        }

        /// <summary>
        /// Convert km to mi.
        /// </summary>
        public double mi(double km)
        {
            return (km * MiPerKm);
        }
 
        /// <summary>
        /// Convert mi to km.
        /// </summary>
        public double km(double mi)
        {
            return (mi / MiPerKm);
        }

        /// <summary>
        /// Return string in fromat 'Jan 2013' from epoch time
        /// </summary>
        public string date(long epoch)
        {
            // Epoch (Unix) time is number of seconds since Jan 1 1970
            // DateTime time is 100 nS intervals since 1/1/1.

            System.DateTime e = new DateTime(1970,1,1);
            e = e.AddSeconds(epoch);

            string d = "";
            int month = e.Month;

            d += MonthIndexToString(month);
            d += " " + e.Year.ToString();

            return d;
         }

        public string MonthIndexToString(int month)
        {
            switch (month)
            {
                case 1:
                    return WebResources.Jan;
                case 2:
                    return WebResources.Feb;             
                case 3:
                    return WebResources.Mar;
                case 4:
                    return WebResources.Apr;
                case 5:
                    return WebResources.May;
                  case 6:
                    return WebResources.Jun;
                case 7:
                    return WebResources.Jul;
                case 8:
                    return WebResources.Aug;
                 case 9:
                    return WebResources.Sep;
                case 10:
                    return WebResources.Oct;
                case 11:
                    return WebResources.Nov;
                case 12:
                    return WebResources.Dec;
                default:
                    return "";
            }
        }

        /// <summary>
        /// Return string in fromat 'Jan 13 2013' from epoch time
        /// </summary>
        public string date_mmmddyyyy(long epoch)
        {
            // Epoch (Unix) time is number of seconds since Jan 1 1970
            // DateTime time is 100 nS intervals since 1/1/1.

            System.DateTime e = new DateTime(1970, 1, 1);
            e = e.AddSeconds(epoch);

            string d = "";

            switch (e.Month)
            {
                case 1:
                    d += WebResources.Jan;
                    break;
                case 2:
                    d += WebResources.Feb;
                    break;
                case 3:
                    d += WebResources.Mar;
                    break;
                case 4:
                    d += WebResources.Apr;
                    break;
                case 5:
                    d += WebResources.May;
                    break;
                case 6:
                    d += WebResources.Jun;
                    break;
                case 7:
                    d += WebResources.Jul;
                    break;
                case 8:
                    d += WebResources.Aug;
                    break;
                case 9:
                    d += WebResources.Sep;
                    break;
                case 10:
                    d += WebResources.Oct;
                    break;
                case 11:
                    d += WebResources.Nov;
                    break;
                case 12:
                    d += WebResources.Dec;
                    break;
                default:
                    break;
            }

            d += " " + e.Day.ToString();

            d += " " + e.Year.ToString();

            return d;
        }

        /// <summary>
        /// Return elapsed time
        /// </summary>
        public string TimeSince(long epoch)
        {
            DateTime targetTime = new DateTime(1970, 1, 1);
            targetTime = targetTime.AddSeconds(epoch);

            TimeSpan span = DateTime.Now.ToUniversalTime() - targetTime;
            bool big = false;

            string timeSince = "";
            int days = span.Days;
            if (days > 364)
            {
                big = true;
                int years = days / 365;
                if (years > 1)
                {
                    timeSince += years; 
                    timeSince += " " + WebResources.years + " ";
                }
                else if (years > 0)
                {
                    timeSince += years; 
                    timeSince += " " + WebResources.year + " ";
                }
                days -= years * 365;
            }
            if (days > 6)
            {
                big = true;
                int weeks = days/7;
                if (weeks > 1)
                {
                    timeSince += weeks; 
                    timeSince += " " + WebResources.weeks + " ";
                }
                else if (weeks > 0)
                {
                    timeSince += weeks; 
                    timeSince += " " + WebResources.week + " ";
                }
 
                days -= weeks*7;
            }

            if (days > 0)
            {
                if (days > 1)
                {
                    timeSince += days; 
                    timeSince += " " + WebResources.days + " ";
                }
                else if (days > 0)
                {
                    timeSince += days; 
                    timeSince += " " + WebResources.day + " ";
                }
            }

            if (!big)
            {
                int hours = span.Hours;
                if (hours > 1)
                {
                    timeSince += hours; 
                    timeSince += " " + WebResources.hours + " ";
                }
                else if (hours > 0)
                {
                    timeSince += hours; 
                    timeSince += " " + WebResources.hour + " ";
                }
            }

            if (!big)
            {
                int minutes = span.Minutes; 
                if (minutes > 1)
                {
                     timeSince += minutes + " " + WebResources.minutes + " ";
                }
                else if (minutes > 0)
                {
                    timeSince += minutes + " " + WebResources.minute + " ";
                }
                else
                {
                    int seconds = span.Seconds; 
                    if (seconds > 1 || seconds < 1)
                    {
                        timeSince += seconds + " " + WebResources.seconds + " ";
                    }
                    else
                    {
                        timeSince += seconds + " " + WebResources.second + " ";
                    }
                }
            }
            return timeSince;
        }


        /// <summary>
        /// Return current time as epoch (Unix) time
        /// </summary>
        public long Now()
        {
            System.DateTime e = new DateTime(1970, 1, 1);

            long time = (DateTime.Now.ToUniversalTime() - e).Ticks;
            time /= 10000000;
            return time;
        }

        /// <summary>
        /// Return number of words in supplied string
        /// </summary>
        public int GetWordCount(string s)
        {
            return s.Split(new char[] { ' ', '.', ',', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}