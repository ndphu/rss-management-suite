package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class CheckValidUsernameAction extends AbstractSoapAction {
	private static final String TAG = "RSSManagementSuiteClient";
	private String m_username;

	public CheckValidUsernameAction(Activity context) {
		super(context);
	}

	public String getUsername() {
		return m_username;
	}

	public void setUsername(String username) {
		this.m_username = username;
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call CheckValidUsernameAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.checkvalidusername);
		soapEnvelop = soapEnvelop.replace("{username}", m_username);
		String result = null;
		result = HttpProvider.request(soapEnvelop, "CheckValidUsername");
		actionResultCallback.onSuccess(this, XMLParser.getElementValue(result, "CheckValidUsernameResult"));
	}

}
