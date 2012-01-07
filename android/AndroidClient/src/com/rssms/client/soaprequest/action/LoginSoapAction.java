package com.rssms.client.soaprequest.action;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import android.content.Context;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.applicationdata.ApplicationManager;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.SocketIO;

public class LoginSoapAction extends AbstractSoapAction {

	private static final String TAG = LoginSoapAction.class.getName();
	private String m_username;
	private String m_password;

	public LoginSoapAction(Context context) {
		super(context);
	}

	@Override
	public Object execute() {
		String result = null;
		
		
		return result;
//		Map<String, List<String>> map = FileIO.getRequestFromResource(m_context.getResources(), R.raw.login);
//
//		StringBuilder sb = new StringBuilder();
//
//		List<String> body = new ArrayList<String>();
//		for (String str : map.get("BODY")) {
//			if (str.contains("username")) {
//				str = str.replace("{username}", ApplicationManager.getApplicationData().getUsername());
//			}
//			if (str.contains("{password}")) {
//				str = str.replace("{password}", ApplicationManager.getApplicationData().getPassword());
//			}
//			sb.append(str);
//			sb.append("\r\n");
//			body.add(str);
//		}
//
//		List<String> header = new ArrayList<String>();
//
//		for (String str : map.get("HEADER")) {
//			if (str.contains("{host}")) {
//				str = str.replace("{host}",
//						ApplicationManager.getApplicationData().getHost() + ":" + String.valueOf(ApplicationManager.getApplicationData().getPort()));
//			}
//
//			if (str.contains("{length}")) {
//				str = str.replace("{length}", String.valueOf(sb.toString().getBytes().length));
//			}
//			header.add(str);
//			Log.e(TAG, "[REQUEST HEADER] = " + str);
//		}
//
//		Map<String, List<String>> request = new HashMap<String, List<String>>();
//
//		request.put("HEADER", header);
//		request.put("BODY", body);
//
//		Map<String, List<String>> response = SocketIO.sendRequest(request);
//
//		for (String str : response.get("HEADER")) {
//			Log.e(TAG, "[HEADER] " + str);
//		}
//
//		for (String str : response.get("BODY")) {
//			Log.e(TAG, "[BODY] " + str);
//		}

		
	}

	public String getPassword() {
		return m_password;
	}

	public void setPassword(String password) {
		this.m_password = password;
	}

	public String getUsername() {
		return m_username;
	}

	public void setUsername(String username) {
		this.m_username = username;
	}

}
