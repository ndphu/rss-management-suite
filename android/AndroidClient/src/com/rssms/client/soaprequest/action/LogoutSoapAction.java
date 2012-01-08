package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class LogoutSoapAction extends AbstractSoapAction {

	private static final String TAG = "RSSManagementSuiteClient";

	public LogoutSoapAction(Activity context) {
		super(context);
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call LogoutAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.logout);
		String result = HttpProvider.request(soapEnvelop, "Logout");
		actionResultCallback.onSuccess(this, new Boolean(XMLParser.getElementValue(result, "LogoutResult")));
	}

}
