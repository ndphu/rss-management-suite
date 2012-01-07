package com.rssms.client;

import java.util.ArrayList;

import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicHttpResponse;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.protocol.HTTP;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

import com.rssms.client.applicationdata.ApplicationManager;

public class AndroidClientActivity extends Activity {
	private static final String TAG = AndroidClientActivity.class.getName();
	TextView tv = null;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);

		ApplicationManager.getApplicationData().setHost(getResources().getString(R.string.host_name));
		ApplicationManager.getApplicationData().setPort(Integer.parseInt(getResources().getString(R.string.host_port)));
		ApplicationManager.getApplicationData().setUsername("ndphu");
		ApplicationManager.getApplicationData().setPassword("123");

		// SoapRequestInvoker.invokeSoapAction(new LoginSoapAction(getApplicationContext()));
		try {
			String r = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
					+ "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
					+ "<soap:Body>" + "<Login xmlns=\"http://tempuri.org/\">" + "<username>{username}</username>" + "<password>{password}</password>"
					+ "</Login>" + "</soap:Body>" + "</soap:Envelope>";
			HttpPost post = new HttpPost("http://" + getResources().getString(R.string.host_name) + ":" + getResources().getString(R.string.host_port)
					+ "/CoreServiceImpl.asmx");
			post.setHeader("Content-Type", "text/xml; charset=utf-8");
			post.setHeader("SOAPAction", "\"http://tempuri.org/Login\"");

			post.setEntity(new StringEntity(r, HTTP.UTF_8));

			HttpClient client = new DefaultHttpClient();

			BasicHttpResponse httpResponse = (BasicHttpResponse) client.execute(post);
			tv = (TextView) findViewById(R.id.tv_result);
			tv.setText(httpResponse.getStatusLine().getReasonPhrase());

			// message = httpResponse.getStatusLine().getReasonPhrase();
			//
			// HttpEntity entity = httpResponse.getEntity();
			//
			// if (entity != null) {
			//
			// InputStream instream = entity.getContent();
			// response = convertStreamToString(instream);
			//
			// // Closing the input stream will trigger connection release
			// instream.close();
			//
			//
			//
			// post.getRe
		} catch (Exception ex) {
			ex.printStackTrace();
		}

	}
}