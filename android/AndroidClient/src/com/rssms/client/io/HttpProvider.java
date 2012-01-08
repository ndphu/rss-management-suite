package com.rssms.client.io;

import java.io.IOException;

import org.apache.http.Header;
import org.apache.http.HttpEntity;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicHttpResponse;
import org.apache.http.protocol.HTTP;

import android.util.Log;

import com.rssms.client.applicationdata.ApplicationManager;

public class HttpProvider {
	private static final String TAG = "RSSManagementSuiteClient";

	public static String request(String soapEnvelop, String action) throws ClientProtocolException, IOException {
		String result = null;

		HttpPost post = new HttpPost(ApplicationManager.getApplicationData().getRequestURL());
		post.setHeader("Content-Type", "text/xml; charset=utf-8");
		post.setHeader("SOAPAction", "\"http://tempuri.org/" + action + "\"");
		String precookie = ApplicationManager.getApplicationData().getCookie();
		if (precookie != null) {
			post.setHeader("Cookie", precookie);
		}
		post.setEntity(new StringEntity(soapEnvelop, HTTP.UTF_8));

		HttpClient client = new DefaultHttpClient();
		BasicHttpResponse httpResponse = (BasicHttpResponse) client.execute(post);

		if (httpResponse.getStatusLine().getStatusCode() != 200)
			throw new RuntimeException("Error: Server Reply = " + httpResponse.getStatusLine());

		Header[] header = httpResponse.getHeaders("Set-Cookie");
		String cookie = header.length != 0 ? header[0].getValue() : null;
		if (cookie != null) {
			ApplicationManager.getApplicationData().setCookie(cookie);
			Log.i(TAG, "Set Cookie = " + cookie);
		}
		HttpEntity entity = httpResponse.getEntity();
		result = IOUtility.getStringFromStream(entity.getContent());

		return result;
	}

}
