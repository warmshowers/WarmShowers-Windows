For the Windows Phone toolkit make sure that you have
marked the icons in the "Toolkit.Content" folder as content.  That way they 
can be used as the icons for the ApplicationBar control.


//
//  TrackMyTourRequest.m
//  TrackMyTour
//
//  Created by Christopher Meyer on 1/7/10.
//  Copyright 2010 Red House Consulting GmbH. All rights reserved.
//


#import "WSRequests.h"
#import "WSAppDelegate.h"
#import "Host.h"
#import "MKMapView+Utils.h"
#import "Feedback.h"
#import "NSString+truncate.h"
#import "WSHTTPClient.h"
// #import "AFXMLRequestOperation.h"
#import "AFJSONRequestOperation.h"


@implementation WSRequests


+(void)requestWithMapView:(MKMapView *)mapView {
    
    if ([[WSAppDelegate sharedInstance] isLoggedIn] == NO) {
        return;
    }
    
	bounds b = [mapView fetchBounds];


	NSString *path = @"/services/rest/hosts/by_location";


	[[WSHTTPClient sharedHTTPClient] cancelAllHTTPOperationsWithMethod:@"POST" path:path];


	NSDictionary *params = [NSDictionary dictionaryWithObjectsAndKeys:
							[NSNumber numberWithDouble:b.minLatitude], @"minlat",
							[NSNumber numberWithDouble:b.maxLatitude], @"maxlat",
							[NSNumber numberWithDouble:b.minLongitude], @"minlon",
							[NSNumber numberWithDouble:b.maxLongitude], @"maxlon",
							[NSNumber numberWithDouble:b.centerLatitude], @"centerlat",
							[NSNumber numberWithDouble:b.centerLongitude], @"centerlon",
							[NSNumber numberWithInteger:kMaxResults], @"limit",
							nil];


    NSURLRequest *nsurlrequest = [[WSHTTPClient sharedHTTPClient] requestWithMethod:@"POST" path:path parameters:params];
    
    AFJSONRequestOperation *operation = [AFJSONRequestOperation
                                         JSONRequestOperationWithRequest:nsurlrequest
                                         success:^(NSURLRequest *request, NSHTTPURLResponse *response, id JSON) {
                                             
                                             NSArray *hosts = [JSON objectForKey:@"accounts"];
                                             
                                             for (NSDictionary *dict in hosts) {
                                                 
                                                 NSString *hostidstring = [dict objectForKey:@"uid"];
                                                 NSNumber *hostid = [NSNumber numberWithInteger:[hostidstring integerValue]];
                                                 
                                                 Host *host = [Host hostWithID:hostid];
                                                 
                                                 host.name = [dict objectForKey:@"name"];
                                                 host.street = [dict objectForKey:@"street"];
                                                 host.city = [dict objectForKey:@"city"];
                                                 host.province = [dict objectForKey:@"province"];
                                                 host.postal_code = [dict objectForKey:@"postal_code"];
                                                 host.country = [dict objectForKey:@"country"];
                                                 
                                                 host.last_updated = [NSDate date];
                                                 host.notcurrentlyavailable = [NSNumber numberWithInt:0];
                                                 
                                                 NSString *latitude = [dict objectForKey:@"latitude"];
                                                 NSString *longitude = [dict objectForKey:@"longitude"];
                                                 
                                                 host.latitude = [NSNumber numberWithDouble:[latitude doubleValue]];
                                                 host.longitude = [NSNumber numberWithDouble:[longitude doubleValue]];
                                             }
                                             
                                             [Host commit];
                                             
                                         } failure:^(NSURLRequest *request, NSHTTPURLResponse *response, NSError *error, id JSON) {
                                             NSLog(@"%@", [error localizedDescription]);
                                             if ([response statusCode] == 401) {
                                                 [[WSAppDelegate sharedInstance] loginWithoutPrompt];
                                                 // [[WSAppDelegate sharedInstance] logout];
                                             }
                                         }];
    
    [[WSHTTPClient sharedHTTPClient] enqueueHTTPRequestOperation:operation];
}




+(void)hostDetailsWithHost:(Host *)host {
    
    if ([[WSAppDelegate sharedInstance] isLoggedIn] == NO) {
        return;
    }


	NSString *path = [NSString stringWithFormat:@"/user/%i/json", [host.hostid intValue]];


	[[WSHTTPClient sharedHTTPClient] cancelAllHTTPOperationsWithMethod:@"GET" path:path];


	NSURLRequest *nsurlrequest = [[WSHTTPClient sharedHTTPClient] requestWithMethod:@"GET" path:path parameters:nil];


	AFJSONRequestOperation *operation = [AFJSONRequestOperation
                                         JSONRequestOperationWithRequest:nsurlrequest
                                         success:^(NSURLRequest *request, NSHTTPURLResponse *response, id JSON) {
                                             
                                             for (NSDictionary *users in [JSON objectForKey:@"users"]) {
                                                 
                                                 NSDictionary *user = [users objectForKey:@"user"];
                                                 NSNumber *hostid = [user objectForKey:@"uid"];
                                                 
                                                 Host *host = [Host hostWithID:hostid];
                                                 
                                                 host.bed = [NSNumber numberWithInt:[[user objectForKey:@"bed"] intValue]];
                                                 host.bikeshop = [user objectForKey:@"bikeshop"];
                                                 host.campground = [user objectForKey:@"campground"];
                                                 host.city = [user objectForKey:@"city"];
                                                 
                                                 NSString *comments = [user objectForKey:@"comments"];
                                                 host.comments = [comments stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
                                                 
                                                 host.country = [user objectForKey:@"country"];
                                                 host.food = [NSNumber numberWithInt:[[user objectForKey:@"food"] intValue]];
                                                 host.fullname = [user objectForKey:@"fullname"];
                                                 host.homephone = [user objectForKey:@"homephone"];
                                                 host.kitchenuse = [NSNumber numberWithInt:[[user objectForKey:@"kitchenuse"] intValue]];
                                                 host.laundry = [NSNumber numberWithInt:[[user objectForKey:@"laundry"] intValue]];
                                                 host.lawnspace = [NSNumber numberWithInt:[[user objectForKey:@"lawnspace"] intValue]];
                                                 host.maxcyclists = [NSNumber numberWithInt:[[user objectForKey:@"maxcyclists"] intValue]];
                                                 host.motel = [user objectForKey:@"motel"];
                                                 host.name = [user objectForKey:@"name"];
                                                 host.notcurrentlyavailable = [NSNumber numberWithInteger:[[user objectForKey:@"notcurrentlyavailable"] integerValue]];
                                                 host.postal_code = [user objectForKey:@"postal_code"];
                                                 host.province = [user objectForKey:@"province"];
                                                 host.sag = [NSNumber numberWithInt:[[user objectForKey:@"sag"] intValue]];
                                                 host.shower = [NSNumber numberWithInt:[[user objectForKey:@"shower"] intValue]];
                                                 host.storage = [NSNumber numberWithInt:[[user objectForKey:@"storage"] intValue]];
                                                 host.street = [user objectForKey:@"street"];
                                                 host.preferred_notice = [user objectForKey:@"preferred_notice"];
                                                 
                                                 NSTimeInterval last_login_int = [[user objectForKey:@"login"] doubleValue];
                                                 host.last_login = [NSDate dateWithTimeIntervalSince1970:last_login_int];
                                                 
                                                 NSTimeInterval member_since = [[user objectForKey:@"created"] doubleValue];
                                                 host.member_since = [NSDate dateWithTimeIntervalSince1970:member_since];
                                                 
                                                 host.last_updated_details = [NSDate date];
                                             }
                                             
                                             [Host commit];
                                             
                                         } failure:^(NSURLRequest *request, NSHTTPURLResponse *response, NSError *error, id JSON) {
                                             
                                             if ( [response statusCode] == 404 ) {
                                                 [host setNotcurrentlyavailable:[NSNumber numberWithBool:YES]];
                                                 [Host commit];
                                                 
                                                 /*
                                                  if ([host.notcurrentlyavailable boolValue]) {
                                                  UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Host no longer available"
                                                  message:@"This host is no longer available and will no longer be displayed on the map or in the host list."
                                                  delegate:nil
                                                  cancelButtonTitle:nil
                                                  otherButtonTitles:@"OK", nil];
                                                  [alert show];
                                                  [alert release];
                                                  }
                                                  */
                                             }
                                             
                                         }];


	[[WSHTTPClient sharedHTTPClient] enqueueHTTPRequestOperation:operation];


}




+(void)hostFeedbackWithHost:(Host *)host {
    
    if ([[WSAppDelegate sharedInstance] isLoggedIn] == NO) {
        return;
    }
    
	NSString *path = [NSString stringWithFormat:@"/user/%i/json_recommendations", [host.hostid intValue]];


	NSURLRequest *URLRequest = [[WSHTTPClient sharedHTTPClient] requestWithMethod:@"GET" path:path parameters:nil];


	AFJSONRequestOperation *operation = [AFJSONRequestOperation
                                         JSONRequestOperationWithRequest:URLRequest
                                         success:^(NSURLRequest *request, NSHTTPURLResponse *response, id JSON) {
                                             
											 // We don't know when stuff gets deleted,
											 // so we purge the feedback before updating.
											 [host purgeFeedback];
                                             
                                             NSArray *recommendations = [JSON objectForKey:@"recommendations"];
                                             
                                             for (NSDictionary *feedback in recommendations) {
                                                 
                                                 NSDictionary *dict = [feedback objectForKey:@"recommendation"];
                                                 
                                                 NSString *snid = [dict objectForKey:@"nid"];
                                                 NSString *recommender = [dict objectForKey:@"fullname"];
                                                 NSString *body = [[dict objectForKey:@"body"] trim];
                                                 NSString *hostOrGuest = [dict objectForKey:@"field_guest_or_host_value"];
                                                 NSNumber *recommendationDate = [dict objectForKey:@"field_hosting_date_value"];
                                                 
                                                 NSNumber *nid = [NSNumber numberWithInteger:[snid integerValue]];
                                                 NSDate *rDate = [NSDate dateWithTimeIntervalSince1970:[recommendationDate doubleValue]];
                                                 
                                                 Feedback *feedback = [Feedback feedbackWithID:nid];
                                                 [feedback setBody:body];
                                                 [feedback setFullname:recommender];
                                                 [feedback setHostOrGuest:hostOrGuest];
                                                 [feedback setDate:rDate];
                                                 
                                                 [host addFeedbackObject:feedback];
                                             }
                                             
                                             [Feedback commit];
                                             
                                         } failure:^(NSURLRequest *request, NSHTTPURLResponse *response, NSError *error, id JSON) {
                                             
                                         }];


	[[WSHTTPClient sharedHTTPClient] enqueueHTTPRequestOperation:operation];
}

This is an easy way to make a JSON POST request to a remote service using C#:
HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); 
request.Method = "POST"; 
request.ContentType = "application/json;
 charset=utf-8"; 
 DataContractJsonSerializer ser = new DataContractJsonSerializer(data.GetType()); 
 MemoryStream ms = new MemoryStream(); 
 ser.WriteObject(ms, data); 
 String json = Encoding.UTF8.GetString(ms.ToArray()); 
 StreamWriter writer = new StreamWriter(request.GetRequestStream()); 
 writer.Write(json); 
 writer.Close();



 Below code works
string json = @" {""GetDataRESTResult"":[{""Key1"":100.0000,""Key2"":1,""Key3"":""Min""},{""Key1"":100.0000,""Key2"":2,""Key3"":""Max""}]}";

dynamic dynObj = JsonConvert.DeserializeObject(json);
foreach (var item in dynObj.GetDataRESTResult)
{
    Console.WriteLine("{0} {1} {2}", item.Key1, item.Key3, item.Key3);
}

You can also use Linq
var jObj = (JObject)JsonConvert.DeserializeObject(json);
var result = jObj["GetDataRESTResult"]
                .Select(item => new
                {
                    Key1 = (double)item["Key1"],
                    Key2 = (int)item["Key2"],
                    Key3 = (string)item["Key3"],
                })
                .ToList();











				private void MakeRequests()
{
	HttpWebResponse response;

	if (Request_www_warmshowers_org(out response))
	{
		response.Close();
	}
}



				private bool Request_www_warmshowers_org(out HttpWebResponse response)
{
	response = null;

	try
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.warmshowers.org/user/klzig/json");

		request.Accept = "text/html, application/xhtml+xml, */*";
		request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
		request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; ASU2JS)";
		request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
		request.IfModifiedSince = DateTime.Parse("Thu, 13 Dec 2012 21:27:06 GMT");
		request.Headers.Add("DNT", @"1");
		request.Headers.Set(HttpRequestHeader.Cookie, @"SESSca3ec806b9aee9140beb6c03142b4638=b67eb05fbda39b3974571c0a857468ea; __utmc=234995806; __utma=234995806.1231041202.1354770333.1354950932.1355425536.3; __utmz=234995806.1354770333.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); __utmv=234995806.anonymous%20user|1=User%20roles=anonymous%20user=1; PERSISTENT_LOGIN_ca3ec806b9aee9140beb6c03142b4638=14861%3Af00dd494fdb55b31488d9f1d9cebe214%3A1a2faf5c5ce0c8cceb471f2a5522ac4b; has_js=1; mapStatus=%7B%22latitude%22%3A42.45588764197166%2C%22longitude%22%3A-96.35009765625%2C%22zoom%22%3A8%7D");

		response = (HttpWebResponse)request.GetResponse();
	}
	catch (WebException e)
	{
		if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
		else return false;
	}
	catch (Exception)
	{
		if(response != null) response.Close();
		return false;
	}

	return true;
}


private void MakeRequests()
{
	HttpWebResponse response;

	if (Request_www_warmshowers_org(out response))
	{
		response.Close();
	}
}


login request:

private bool Request_www_warmshowers_org(out HttpWebResponse response)
{
	response = null;

	try
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.warmshowers.org/user");

		request.Accept = "text/html, application/xhtml+xml, */*";
		request.Referer = "http://www.warmshowers.org/user";
		request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
		request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; ASU2JS)";
		request.ContentType = "application/x-www-form-urlencoded";
		request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
		request.Headers.Add("DNT", @"1");
		request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
		request.Headers.Set(HttpRequestHeader.Cookie, @"SESSca3ec806b9aee9140beb6c03142b4638=b67eb05fbda39b3974571c0a857468ea; __utmc=234995806; __utma=234995806.1231041202.1354770333.1355425536.1355478756.4; __utmz=234995806.1354770333.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); __utmv=234995806.anonymous%20user|1=User%20roles=anonymous%20user=1; __utmb=234995806.4.10.1355478756; has_js=1; mapStatus=%7B%22latitude%22%3A42.45588764197166%2C%22longitude%22%3A-96.35009765625%2C%22zoom%22%3A8%7D");

		request.Method = "POST";
		request.ServicePoint.Expect100Continue = false;

		string body = @"name=klzig&pass=Mooney19&form_build_id=form-c251137ea08f607b7b54bea6acd99fef&form_id=user_login&op=Log+in";
		byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
		request.ContentLength = postBytes.Length;
		Stream stream = request.GetRequestStream();
		stream.Write(postBytes, 0, postBytes.Length);
		stream.Close();

		response = (HttpWebResponse)request.GetResponse();
	}
	catch (WebException e)
	{
		if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
		else return false;
	}
	catch (Exception)
	{
		if(response != null) response.Close();
		return false;
	}

	return true;
}


Login query captured from iPod:
private void MakeRequests()
{
	HttpWebResponse response;

	if (Request_www_warmshowers_org(out response))
	{
		response.Close();
	}
}

private bool Request_www_warmshowers_org(out HttpWebResponse response)
{
	response = null;

	try
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.warmshowers.org/services/rest/user/login");

	request.KeepAlive = true;
	request.Accept = "*/*";
	request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
	request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
	request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en, fr, de, ja, nl, it, es, pt, pt-PT, da, fi, nb, sv, ko, zh-Hans, zh-Hant, ru, pl, tr, uk, ar, hr, cs, el, he, ro, sk, th, id, ms, en-GB, ca, hu, vi, en-us;q=0.8");
	request.KeepAlive = true;
	request.UserAgent = "WS/342 (iPod touch; iOS 6.0.1; Scale/2.00)";

	request.Method = "POST";
	request.ServicePoint.Expect100Continue = false;

	string body = @"username=klzig%40live%2Ecom&password=Mooney19";
	byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
	request.ContentLength = postBytes.Length;
	Stream stream = request.GetRequestStream();
	stream.Write(postBytes, 0, postBytes.Length);
	stream.Close();

	response = (HttpWebResponse)request.GetResponse();
	}
	catch (WebException e)
	{
		if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
		else return false;
	}
	catch (Exception)
	{
		if(response != null) response.Close();
		return false;
	}

	return true;
}


Hosts query captured from iPod:
private void MakeRequests()
{
	HttpWebResponse response;

	if (Request_www_warmshowers_org(out response))
	{
		response.Close();
	}
}

private bool Request_www_warmshowers_org(out HttpWebResponse response)
{
	response = null;

	try
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.warmshowers.org/services/rest/hosts/by_location");

	request.KeepAlive = true;
	request.Accept = "*/*";
	request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
	request.Headers.Set(HttpRequestHeader.Cookie, @"SESSca3ec806b9aee9140beb6c03142b4638=56cdeb1f40e2b1ceeaba25dc82724d7a");
	request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
	request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en, fr, de, ja, nl, it, es, pt, pt-PT, da, fi, nb, sv, ko, zh-Hans, zh-Hant, ru, pl, tr, uk, ar, hr, cs, el, he, ro, sk, th, id, ms, en-GB, ca, hu, vi, en-us;q=0.8");
	request.KeepAlive = true;
	request.UserAgent = "WS/342 (iPod touch; iOS 6.0.1; Scale/2.00)";

	request.Method = "POST";
	request.ServicePoint.Expect100Continue = false;

	string body = @"centerlon=-116%2E0363824724936&centerlat=43%2E63407935605796&limit=50&minlat=43%2E26319826254485&minlon=-116%2E4758355814866&maxlat=44%2E00268557026779&maxlon=-115%2E5969293635005";
	byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
	request.ContentLength = postBytes.Length;
	Stream stream = request.GetRequestStream();
	stream.Write(postBytes, 0, postBytes.Length);
	stream.Close();

	response = (HttpWebResponse)request.GetResponse();
	}
	catch (WebException e)
	{
		if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
		else return false;
	}
	catch (Exception)
	{
		if(response != null) response.Close();
		return false;
	}

	return true;
}


// code for get user request
private void MakeRequests()
{
	HttpWebResponse response;

	if (Request_www_warmshowers_org(out response))
	{
		response.Close();
	}
}

private bool Request_www_warmshowers_org(out HttpWebResponse response)
{
	response = null;

	try
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.warmshowers.org/user/14861/json");

	request.KeepAlive = true;
	request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
	request.Accept = "*/*";
	request.Headers.Set(HttpRequestHeader.Cookie, @"SESSca3ec806b9aee9140beb6c03142b4638=56cdeb1f40e2b1ceeaba25dc82724d7a");
	request.KeepAlive = true;
	request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en, fr, de, ja, nl, it, es, pt, pt-PT, da, fi, nb, sv, ko, zh-Hans, zh-Hant, ru, pl, tr, uk, ar, hr, cs, el, he, ro, sk, th, id, ms, en-GB, ca, hu, vi, en-us;q=0.8");
	request.UserAgent = "WS/342 (iPod touch; iOS 6.0.1; Scale/2.00)";

	response = (HttpWebResponse)request.GetResponse();
	}
	catch (WebException e)
	{
		if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
		else return false;
	}
	catch (Exception)
	{
		if(response != null) response.Close();
		return false;
	}

	return true;
}


<phone:PhoneApplicationPage.Resources>
    <ControlTemplate x:Key="customPushpin" TargetType="my:Pushpin">
        <Image Height="39" Source="Resources/Icons/Pushpins/pinGreen.png" Stretch="Fill" Width="32"/>
    </ControlTemplate>
</phone:PhoneApplicationPage.Resources>

<Grid x:Name="LayoutRoot" Background="Transparent">
    <my:Map Height="Auto" HorizontalAlignment="Stretch" Margin="0" x:Name="Map"
            VerticalAlignment="Stretch" Width="Auto" CredentialsProvider="{Binding CredentialsProvider}" 
            CopyrightVisibility="Collapsed" LogoVisibility="Collapsed" Center="{Binding Mode=TwoWay, Path=Center}" 
            ZoomBarVisibility="Visible" 
            ZoomLevel="{Binding Zoom, Mode=TwoWay}">
        <my:MapItemsControl ItemsSource="{Binding Pushpins}">
            <my:MapItemsControl.ItemTemplate>
                <DataTemplate>
                    <my:Pushpin MouseLeftButtonUp="Pushpin_MouseLeftButtonUp" 
                                Location="{Binding Location}" 
                                Template="{StaticResource customPushpin}">                           
                    </my:Pushpin>
                </DataTemplate>
            </my:MapItemsControl.ItemTemplate>
        </my:MapItemsControl>
    </my:Map>        
</Grid>
