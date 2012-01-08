package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.data.data.parser.TabParser;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;

public class GetAllTabsSoapAction extends AbstractSoapAction {

	private static final String TAG = "RSSManagementSuite";

	public GetAllTabsSoapAction(Activity context) {
		super(context);
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call GetAllTabsSoapAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.login);
		String result = HttpProvider.request(soapEnvelop, "GetAllTabs");
		actionResultCallback.onSuccess(this, TabParser.getTabList(result));
	}

}
