package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class LoginSoapAction extends AbstractSoapAction {

	private static final String TAG = "RSSManagementSuiteClient";
	private String m_username;
	private String m_password;

	public LoginSoapAction(Activity context) {
		super(context);
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

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call LoginSoapAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.login);
		soapEnvelop = soapEnvelop.replace("{username}", m_username).replace("{password}", m_password);
		String result = HttpProvider.request(soapEnvelop, "Login");
		actionResultCallback.onSuccess(this, new Boolean(XMLParser.getElementValue(result, "LoginResult")));
	}

}
