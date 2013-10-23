using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Runtime.Serialization.Json;
using WSApp.DataModel;

namespace WSApp.DataModel
{
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
        public static RequestManager requestManager = new RequestManager();
        private static int threadId;
        public delegate void LoginCompleteCallback();
        private LoginCompleteCallback loginCompleteCallback;
        public delegate void LoginFailedCallback(string msg, string un, string pw);
        private LoginFailedCallback loginFailedCallback;
        public delegate void LogoutCompleteCallback(string msg, string un, string pw);
        private LogoutCompleteCallback logoutCompleteCallback;
        public delegate void SendMessageSuccessCallback();
        private SendMessageSuccessCallback sendMessageSuccessCallback;
        public delegate void SendMessageFailCallback();
        private SendMessageFailCallback sendMessageFailCallback;
        public delegate void SendFeedbackSuccessCallback();
        private SendMessageSuccessCallback sendFeedbackSuccessCallback;
        public delegate void SendFeedbackFailCallback();
        private SendMessageFailCallback sendFeedbackFailCallback;
        public delegate void WP8KickstartCallback();
        private WP8KickstartCallback wP8KickstartCallback;



        /// <summary>
        /// Request type
        /// </summary>
        public enum Request
        {
            login,
            logout,
            getHosts,
            getHost,
            getFeedback,
            sendFeedback,
            getMessages,
            sendMessage,
            replyMessage,
            getMessageCount,
            getMessageThread,
            markThreadRead
        }

        public class MessageCount
        {
            public int count = 0;
            public String name = "";
        }



        // Constructor
        public WebService()
        {
            cookieJar = new CookieContainer();
        }

        public void RegisterLoginCompleteCallback(LoginCompleteCallback func)
        {
            loginCompleteCallback += func;
        }

        public void RegisterLoginFailedCallback(LoginFailedCallback func)
        {
            loginFailedCallback += func;
        }

        public void RegisterLogoutCompleteCallback(LogoutCompleteCallback func)
        {
            logoutCompleteCallback += func;
        }

        public void RegisterSendMessageSuccessCallback(SendMessageSuccessCallback func)
        {
            sendMessageSuccessCallback += func;
        }

        public void RegisterSendMessageFailCallback(SendMessageFailCallback func)
        {
            sendMessageFailCallback += func;
        }

        public void RegisterSendFeedbackSuccessCallback(SendMessageSuccessCallback func)
        {
            sendFeedbackSuccessCallback += func;
        }

        public void RegisterSendFeedbackFailCallback(SendMessageFailCallback func)
        {
            sendFeedbackFailCallback += func;
        }

        public void RegisterWP8KickstartCallback(WP8KickstartCallback func)
        {
            wP8KickstartCallback += func;
        }

        public class RequestManager
        {
            enum RequestState
            {
                idle,
                loggingIn,
                loginDialogOpen,
                gettingHosts,
                gettingHost,
                gettingFeedback,
                gettingMessages,
                gettingMessageThread
            }

            private static Timer t = null;
            const int startTime = 5000;                   // Milliseconds until first timer callback
            const int periodTime = 5000;                // Milliseconds between timer callbacks
//            private static Timer NewMessageTimer = null;
            const int NewMessageStartTime = 10;         // Milliseconds until first timer callback
            const int NewMessagePeriodTime = 600000;    // Milliseconds between timer callbacks

            private RequestState requestState;
            public Request request;

            // Constructor
            public RequestManager()
            {
                requestState = RequestState.idle;
            }

            private void RequestTimerCallback(object state)
            {
                System.Diagnostics.Debug.WriteLine("TimerCallback:" + requestState.ToString());

                switch (requestState)
                {
                    case RequestState.idle:
                        if (null != t)
                        {
                            t.Dispose();
                            t = null;
//                            NewMessageTimer = new Timer(RequestTimerCallback, t, NewMessageStartTime, NewMessagePeriodTime);
                                            // With the 'check message' button, I don't think we need timer to check for new messages
                            GetMessages();  // Check once after initial hosts request in lieu of timer
                        }
                        
 //                       GetMessages();  // Poll for new messages
                        break;

                    case RequestState.loggingIn:
                        //                        Login();     
                        break;

                    case RequestState.gettingHosts:
                        // WP8 doesn't always get data first time, keep trying 'til we get it  
                        Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.wP8KickstartCallback(); });
                        break;

                    default:
                        break;
                }
            }

            /// <summary>
            /// State machine entry point for beginning request
            /// </summary>
            public bool RequestStart(Request request)
            {
                System.Diagnostics.Debug.WriteLine("Request Start:" + request.ToString() + " State:" + requestState.ToString());
                if (false == App.networkService.IsNetworkAvailable())
                {
                    return false;
                }
                
                switch (request)
                {
                    case Request.login:
                        if (null != t) t.Dispose();
                        t = new Timer(RequestTimerCallback, t, startTime, periodTime);
                        requestState = RequestState.loggingIn;
                        break;
                    case Request.logout:
                        break;
                    case Request.getHosts:
                        if (RequestState.idle != requestState && RequestState.gettingHosts != requestState && RequestState.gettingHost != requestState) return false;       // Todo:  Do we really need this check?
                        if (null != t) t.Dispose();
                        t = new Timer(RequestTimerCallback, t, startTime, periodTime);
                        requestState = RequestState.gettingHosts;
                        break;
                    case Request.getHost:
                        requestState = RequestState.gettingHost;
                        break;
                    case Request.getFeedback:
                        requestState = RequestState.gettingFeedback;
                        break;
                    case Request.sendFeedback:
                        break;
                    case Request.getMessages:
                        // Stay in idle state so we don't trigger RefreshHost except at end of chain
 //                       requestState = RequestState.gettingMessages;
                        break;
                    case Request.sendMessage:                    
                        break;
                    case Request.replyMessage: 
                         break;
                    case Request.getMessageCount:
                        break;
                    case Request.getMessageThread:
                        requestState = RequestState.gettingMessageThread;
                        break;
                    case Request.markThreadRead:
                        break;
                    default:
                        break;
                }
                return true;
            }

            /// <summary>
            /// State machine entry point for complted request
            /// </summary>
            public void RequestComplete(Request request)
            {
                // Reset the timer
                if (null != t)
                {
                    t.Change(periodTime, periodTime);
                }

                System.Diagnostics.Debug.WriteLine("RequestComplete:" + request.ToString() + " State:" + requestState.ToString());

                switch (request)
                {
                    case Request.login:
                        Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.loginCompleteCallback(); });                       
                        if (RequestState.loggingIn == requestState)
                        {   // Chain requests
                            requestState = RequestState.gettingHosts;
                            App.nearby.viewportCache.getHosts();   // Todo:  Fake names for screen shots
                        }
                        break;
                    case Request.logout:
                        Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.logoutCompleteCallback(WebResources.LoginNew, "", ""); });
                        break;
                    case Request.getHosts:
                        // Last in a chain of requests
                        requestState = RequestState.idle;
                        break;
                    case Request.getHost:
                        if (RequestState.gettingHost == requestState)
                        {   // Chain requests
                            requestState = RequestState.gettingFeedback;
                            GetFeedback(App.nearby.host.profile.users_Result.users[0].user.uid);
                        }
                        break;
                    case Request.getFeedback:
                        if (RequestState.gettingFeedback == requestState)
                        {   // Chain requests
                            requestState = RequestState.gettingMessages;  
                            GetMessages();
                        }
                        break;
                    case Request.sendFeedback:
                        Deployment.Current.Dispatcher.BeginInvoke(() => {
                            App.webService.sendFeedbackSuccessCallback();
                        });
                        break;
                    case Request.getMessages:
                        // Last in a chain of requests
                        if (RequestState.gettingMessages == requestState)
                        {
                            App.pinned.refreshHost();
                        }
                        requestState = RequestState.idle;
                        break;
                    case Request.sendMessage:
                        Deployment.Current.Dispatcher.BeginInvoke(() => {
                            App.webService.sendMessageSuccessCallback();
                        });
                        break;
                    case Request.replyMessage:
                        Deployment.Current.Dispatcher.BeginInvoke(() => {
                            App.webService.sendMessageSuccessCallback();
                        });
                       break;
                    case Request.getMessageCount:
                        break;
                    case Request.getMessageThread:
                        if (RequestState.gettingMessageThread == requestState)
                        {
                            markThreadRead();
                        }
                        break;
                    case Request.markThreadRead:
                        break;
                    default:
                        break;
                }
            }
        }

        #region Login
        // POST /services/rest/user/login
        // Accept: application/json
        // POST parameters: username, password
        // Response: JSON with sessid, session_name, and user json object.
        //
        // This information can then be used to authenticate all following requests by adding the header
        // Cookie: <session_name>=<sessid>

        /// <summary>
        /// Send login request
        /// </summary>
        public static bool Login()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/user/login";
            requestManager.RequestStart(Request.login);

            try
            {
                cookieJar = new CookieContainer();

                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(LoginPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void LoginPostCallback(IAsyncResult result)
        {
            string parms = "username=" + System.Net.HttpUtility.UrlEncode(App.settings.myUsername) + "&password=" + System.Net.HttpUtility.UrlEncode(App.settings.myPassword);   

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);

                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(LoginResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("LoginPostCallback");
        }

        static void LoginResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;

                // Do this once during login
                string cookie = cookieJar.GetCookieHeader(httpRequest.RequestUri);      // Grab the cookie we just got back for this specifc page (the login URI)
                cookieJar.SetCookies(new Uri(WebResources.uriPrefix + WebResources.WarmShowersUri + "/"), cookie);   // Put it back in the cookie container for the whole server (the root URI)

                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                }
                response.Close();
            }
            catch (WebException e)
            {
                // Display last attempted UN/PW
                string un = App.settings.myUsername;
                string pw = App.settings.myPassword;

                // Force prompting for new credentials 
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.settings.isAuthenticated = false; });

                // Callback to UI to prompt for credentials
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.loginFailedCallback(WebResources.LoginFailed, un, pw); });

                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.login);
        }

        #endregion

        #region Logout

        // POST /services/rest/user/logout
        // Accept: application/json
        // Response: 1 (if was logged in) or null (if was not logged in) 

        /// <summary>
        /// Send logout request
        /// </summary>
        public static bool Logout()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/user/logout";
            requestManager.RequestStart(Request.logout);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";                
                httpReq.BeginGetRequestStream(new AsyncCallback(LogoutPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void LogoutPostCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                httpReq.BeginGetResponse(new AsyncCallback(LogoutResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("LogoutPostCallback");
        }

        static void LogoutResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.logout);
        }

        #endregion

        #region GetHosts
        // POST /services/rest/hosts/by_location
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)`
        // Post parameters: minlat maxlat minlon maxlon centerlat centerlon limit 

        /// <summary>
        /// Get hosts within bounding rectangle
        /// </summary>
        public static bool GetHosts()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/hosts/by_location";
            if (false == requestManager.RequestStart(Request.getHosts)) return false;

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                // httpReq.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                // httpReq.Headers.Set(HttpRequestHeader.AcceptLanguage, "en, fr, de, ja, nl, it, es, pt, pt-PT, da, fi, nb, sv, ko, zh-Hans, zh-Hant, ru, pl, tr, uk, ar, hr, cs, el, he, ro, sk, th, id, ms, en-GB, ca, hu, vi, en-us;q=0.8");
                // httpReq.UserAgent = "WS/342 (iPod touch; iOS 6.0.1; Scale/2.00)";
                httpReq.BeginGetRequestStream(new AsyncCallback(GetHostsPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void GetHostsPostCallback(IAsyncResult result)
        {
            GeoCoordinate locCenter = App.nearby.mapCenter;
            LocationRect locRect = App.nearby.locationRect;

            if ((null == locCenter) || (null == locRect))
            {
                return;
            }

            string parms =  "centerlon=" + locCenter.Longitude + "&centerlat=" + locCenter.Latitude +
                            "&minlat=" + locRect.South + "&minlon=" + locRect.West +
                            "&maxlat=" + locRect.North + "&maxlon=" + locRect.East + "&limit=" + App.nearby.viewportCache.resultLimit;
                
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(GetHostsResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("GetHostsPostCallback");
        }

        static void GetHostsResponseCallback(IAsyncResult result)
        {
             try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
                    var serializer = new DataContractJsonSerializer(typeof(Hosts.Hosts_Result));
                    App.nearby.hosts.hosts_Result = (Hosts.Hosts_Result)serializer.ReadObject(stream);
                    stream.Close();

                    App.nearby.loadHosts();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.getHosts);
        }

        #endregion  

        #region GetHost
        // GET /user/<uid>/json
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)`

        /// <summary>
        /// Get profile information for individual host 
        /// </summary>
        public static bool GetHost(int uid)
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/user/" + uid.ToString() + "/json";
            requestManager.RequestStart(Request.getHost);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "GET";
                httpReq.BeginGetResponse(new AsyncCallback(GetHostResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void GetHostResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string junk = sr.ReadToEnd();

                    var serializer = new DataContractJsonSerializer(typeof(Profile.Users_result));
                    App.nearby.host.profile.users_Result = (Profile.Users_result)serializer.ReadObject(stream);
                    stream.Close();

                    // Pull out uId and username 
                    Profile.User2 user = App.nearby.host.profile.users_Result.users[0].user;
                    App.nearby.host.uId = user.uid;
                    App.nearby.host.name = user.fullname;

                    App.nearby.loadProfile();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.getHost);
        }

        #endregion

        #region GetFeedback
        // GET /user/<uid>/json_recommendations
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)

        /// <summary>
        /// Get feedback for individual host
        /// </summary>
        public static bool GetFeedback(int uid)
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/user/" + uid.ToString() + "/json_recommendations";
            requestManager.RequestStart(Request.getFeedback);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "GET";
                httpReq.BeginGetResponse(new AsyncCallback(GetFeedbackResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void GetFeedbackResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
//                    string junk2 = junk;
 
                    var serializer = new DataContractJsonSerializer(typeof(Feedback.Recommendations_result));
                    App.nearby.host.feedback.recommendations_Result = (Feedback.Recommendations_result)serializer.ReadObject(stream);
                    stream.Close();

                    App.nearby.loadFeedback();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.getFeedback);
        }

        #endregion

        #region SendFeedback
        // (Note: I had to do drush vset services_node_save_button_trust_referral_resource_create "Submit" to make this work) 
        // POST /services/rest/node
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)`
        //
        // Post parameters: 
        // node[type]=trust_referral
        // node[field_member_i_trust][0][uid][uid]=<username>  (NOTE not the uid, the username instead)
        // node[body]=<body of recommendation>
        // node[field_guest_or_host][value]=Guest|Host|Met Traveling|Other
        // node[field_hosting_date][0][value][year]=<year>
        // node[field_hosting_date][0][value][month]=<numeric month, 1-12>

        /// <summary>
        /// Give feedback on indidual host
        /// </summary>
        public static bool SendFeedback()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/node";
            requestManager.RequestStart(Request.sendFeedback);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(SendFeedbackPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void SendFeedbackPostCallback(IAsyncResult result)
        {
            FeedbackData feedbackData = App.settings.messageData.feedbackData;
            string parms = "node[type]=" + "trust_referral" +
               "&node[field_member_i_trust][0][uid][uid]=" + System.Net.HttpUtility.UrlEncode(feedbackData.to) +
               "&node[body]=" + System.Net.HttpUtility.UrlEncode(feedbackData.body) +
               "&node[field_guest_or_host][value]=" + System.Net.HttpUtility.UrlEncode(feedbackData.guestOrHost) +
               "&node[field_hosting_date][0][value][year]=" + feedbackData.yearMet +
               "&node[field_hosting_date][0][value][month]=" + (feedbackData.monthMet + 1).ToString() +    // Zero-based index
               "&node[field_rating][value]=" + System.Net.HttpUtility.UrlEncode(feedbackData.experience);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(SendFeedbackResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("SendFeedbackPostCallback");
        }

        static void SendFeedbackResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
                }
                response.Close();
            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.sendFeedbackFailCallback(); });  
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.sendFeedbackFailCallback(); });  
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.sendFeedback);
        }

        #endregion

        #region GetMessages
        // POST /services/rest/message/get
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)

        /// <summary>
        /// Get all private messages
        /// </summary>
        public static bool GetMessages()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/message/get";
            requestManager.RequestStart(Request.getMessages);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(GetMessagesPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void GetMessagesPostCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                httpReq.BeginGetResponse(new AsyncCallback(GetMessagesResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("GetMessagesPostCallback");
        }

        static void GetMessagesResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState; 
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
//                    string junk2 = junk;

                    var serializer = new DataContractJsonSerializer(typeof(List<Messages.Message>));
                    List<Messages.Message> messages = new List<Messages.Message>();
                    messages = (List<Messages.Message>)serializer.ReadObject(stream);
                    stream.Close();

                    App.nearby.host.messages.messages_result = new Messages.Messages_result();
                    App.nearby.host.messages.messages_result.messages = new List<Messages.Message>();
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        App.ViewModelMain.ClearNewMessageCount();
                    });

                    Dictionary<int, MessageCount> counts = new Dictionary<int, MessageCount>();
                    MessageCount messageCount;         
                        
                    foreach (var message in messages)
                    {
                        foreach (var participant in message.participants)
                        {
                            if (null != App.nearby.host.profile.users_Result)  // Profile might not be cached yet
                            {
                                if (participant.uid == App.nearby.host.profile.users_Result.users[0].user.uid)
                                {   // Filter all but threads where curent host is a participant
                                    App.nearby.host.messages.messages_result.messages.Add(message);
                                }
                            }
              
                            if (0 < message.is_new) // is_new represents the number of new messages, it's not a new message boolean!
                            {   // Aggregate message counts
                                if (counts.ContainsKey(participant.uid))
                                {
                                    counts.TryGetValue(participant.uid, out messageCount);
                                }
                                else
                                {
                                    messageCount = new MessageCount();
                                }
                                messageCount.count = messageCount.count + message.is_new;
                                messageCount.name = participant.name;
                                counts.Remove(participant.uid);
                                counts.Add(participant.uid, messageCount);
                            }
                        }
                    }
                    foreach (var item in counts)
                    {   // This host is in a new thread with us, automatically pin and show message count in UI 
                        messageCount = item.Value;      
                        App.pinned.newMessage(item.Key, messageCount.name, messageCount.count);  // Will initially display username
                    }
                  App.nearby.loadMessages();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.getMessages);
        }

        #endregion

        #region SendMessage
        // Send a privatemsg (not replying)
        // POST /services/rest/message/send
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)`

        // Post parameters: 
        // recipients=username(s) [Not UID]
        // subject=
        // body=

        /// <summary>
        /// Send private message
        /// </summary>
        public static bool SendMessage()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/message/send";
            requestManager.RequestStart(Request.sendMessage);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(SendMessagePostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void SendMessagePostCallback(IAsyncResult result)
        {
            ContactData contactData = App.settings.messageData.contactData;

            string parms = "recipients=" +  System.Net.HttpUtility.UrlEncode(contactData.to) + "&subject=" +  System.Net.HttpUtility.UrlEncode(contactData.subject) + "&body=" +  System.Net.HttpUtility.UrlEncode(contactData.body);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(SendMessageResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("SendMessagePostCallback");
        }

        static void SendMessageResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
                    stream.Close();
                }
                response.Close();
            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.sendMessageFailCallback(); });    
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.sendMessageFailCallback(); });    
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.sendMessage);
        }

        #endregion

        #region ReplyMessage
        // POST /services/rest/message/reply
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)
        // Parameters thread_id= body= 

        /// <summary>
        /// Send private message
        /// </summary>
        public static bool ReplyMessage()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/message/reply";
            requestManager.RequestStart(Request.replyMessage);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(ReplyMessagePostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void ReplyMessagePostCallback(IAsyncResult result)
        {
            ContactData contactData = App.settings.messageData.contactData;
            MessageThread.Message_result message_result = App.nearby.messageThread.message_result;

            if (null == message_result) return;

            string parms = "thread_id=" + App.nearby.messageThread.message_result.thread_id + "&body=" + System.Net.HttpUtility.UrlEncode(contactData.body); 
            
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(ReplyMessageResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("ReplyMessagePostCallback");
        }

        static void ReplyMessageResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
                    //                    StreamReader sr = new StreamReader(stream);
                    //                    string junk = sr.ReadToEnd();
                    stream.Close();
                }
                response.Close();
            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.sendMessageFailCallback(); });    
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.webService.sendMessageFailCallback(); });
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.replyMessage);
        }

        #endregion
        
        #region GetMessageCount
        // POST /services/rest/message/unreadCount
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)

        /// <summary>
        /// Get unread private message count
        /// </summary>
        public static bool GetMessageCount()
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/message/unreadCount";
            requestManager.RequestStart(Request.getMessageCount);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(GetMessageCountPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void GetMessageCountPostCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                httpReq.BeginGetResponse(new AsyncCallback(GetMessageCountResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("GetMessageCountPostCallback");
        }

        static void GetMessageCountResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.getMessageCount);
        }

        #endregion

        #region GetMessageThread
        // POST /services/rest/message/getThread
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)
        //
        // Parameters thread_id=

        /// <summary>
        /// Get specific private message thread
        /// </summary>
        public static bool GetMessageThread(int tId)
        {
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/message/getThread";
            requestManager.RequestStart(Request.getMessageThread);

            threadId = tId;

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(GetMessageThreadPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
             return true;
        }

        static void GetMessageThreadPostCallback(IAsyncResult result)
        {
            string parms = "thread_id=" + threadId.ToString();

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(GetMessageThreadResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("GetMessageThreadPostCallback");
        }

        static void GetMessageThreadResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
//                    string junk2 = junk;

                    var serializer = new DataContractJsonSerializer(typeof(MessageThread.Message_result));
                    App.nearby.messageThread.message_result = (MessageThread.Message_result)serializer.ReadObject(stream);
                    stream.Close();

                    App.nearby.loadMessage();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.getMessageThread);
        }

        #endregion

        #region markThreadRead
        // POST /services/rest/message/getThread
        // Accept: application/json
        // Cookie: <session_name>=<sessid>  (obtained from login)
        //
        // Parameters thread_id=

        /// <summary>
        /// Mark as read the thread that was just fetched by getMessageThread
        /// </summary>
        public static bool markThreadRead()
        {   // Marks read the thread specified in threadId
            string uri = WebResources.uriPrefix + WebResources.WarmShowersUri + "/services/rest/message/markThreadRead";
            requestManager.RequestStart(Request.markThreadRead);

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
                httpReq.CookieContainer = cookieJar;
                httpReq.Accept = "application/json";
                httpReq.Method = "POST";
                httpReq.BeginGetRequestStream(new AsyncCallback(markThreadReadPostCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return false;
            }
            return true;
        }

        static void markThreadReadPostCallback(IAsyncResult result)
        {
            string parms = "thread_id=" + threadId.ToString();

            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)result.AsyncState;
                Stream postStream = httpReq.EndGetRequestStream(result);
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(parms);

                // Write to the request stream
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();
                httpReq.BeginGetResponse(new AsyncCallback(markThreadReadResponseCallback), httpReq);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                throw;
            }
            System.Diagnostics.Debug.WriteLine("markThreadReadPostCallback");
        }

        static void markThreadReadResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                WebResponse response = httpRequest.EndGetResponse(result);
                if (null != response)
                {
                    Stream stream = response.GetResponseStream();
//                    StreamReader sr = new StreamReader(stream);
//                    string junk = sr.ReadToEnd();
//                    string junk2 = junk;

                    stream.Close();
                }
                response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                return;
            }
            requestManager.RequestComplete(WebService.Request.markThreadRead);
        }

        #endregion

    }
}
