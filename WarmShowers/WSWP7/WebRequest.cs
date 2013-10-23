using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WSApp.WebServices
{
    // This gets called twice:
    //    once with valid stream for processing the results
    //    once with null stream to signal request is complete 
    public delegate void ResultsCallback(WebResponse  response);

    public enum LoginState
    {
        notLoggedIn,
        Pending,
        Failed,
        loggedIn
    }

    public enum Method
    {
        Get,
        Post
    }

    public sealed class WebService
    {
        // Singleton object
        static readonly WebService _instance = new WebService();
        public static WebService Instance
        {
            get
            { 
                return _instance;
            }
        }

        private static CookieContainer cookieJar;
        public static bool requestInProgress;
        static private string requestParameters;
        static private ResultsCallback requestCallback;

        // Constructor
        public WebService()
        {
            cookieJar = new CookieContainer();
            requestInProgress = false;  // Todo:  clear this with a timer to avoid lockout

        }

        public static bool Request(Method method, string uri, string parameters, ResultsCallback callback)   
        {
            requestParameters = parameters;     // Used in post callback
            requestCallback = callback;         // Used in response callback

            try
            {
                if (!App.networkService.IsNetworkAvailable()) return false;
//                if (requestInProgress) return false;
//                requestInProgress = true;

                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                if (Method.Post == method)
                {
                    httpReq.Method = "POST";

                    // Asynchronously get stream for post data
                    httpReq.BeginGetRequestStream(new AsyncCallback(PostCallback), httpReq);
                }
                else
                {
                    httpReq.Method = "GET"; // Todo: Do we need to set if it's a GET?

                }
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                requestInProgress = false; 
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                requestInProgress = false;
                return false;
            }
            System.Diagnostics.Debug.WriteLine("Request completed: " + callback.ToString());
            return true;
        }

        private static void PostCallback(IAsyncResult result)
        {
            HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;

            // End the operation.
            Stream postStream = httpReq.EndGetRequestStream(result);

            try
            {
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(requestParameters);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(ResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                if (postStream != null) postStream.Close();
                requestInProgress = false;
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                if (postStream != null) postStream.Close();
                requestInProgress = false;
                throw;
            }
            System.Diagnostics.Debug.WriteLine("Post callback completed: " + requestCallback.ToString());
        }

        private static void ResponseCallback(IAsyncResult result)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;

            try
            {
                WebResponse response = httpRequest.EndGetResponse(result);
   
//                Stream stream = response.GetResponseStream();

                //Grab the cookie we just got back for this specifc page (the login URI)
                string cookie = cookieJar.GetCookieHeader(httpRequest.RequestUri);

                //put it back in the cookie container for the whole server (the root URI)
                cookieJar.SetCookies(new Uri("http://www.warmshowers.org/"), cookie);

                // Call back to UI thread to process results
//                Deployment.Current.Dispatcher.BeginInvoke(() => { requestCallback(response); });
                requestCallback(response);
                response.Close();
  //              requestCallback(stream);               
  //              stream.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                requestInProgress = false;
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                requestInProgress = false;
                return;
            }
            requestInProgress = false;
            System.Diagnostics.Debug.WriteLine("Response callback completed.");

            // Call back to signal request is complete
            Deployment.Current.Dispatcher.BeginInvoke(() => { requestCallback(null); });
  //          requestCallback(null);
            System.Diagnostics.Debug.WriteLine("Response callback completed: " + requestCallback.ToString());
        }
    }
}
