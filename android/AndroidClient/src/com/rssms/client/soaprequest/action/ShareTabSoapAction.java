package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class ShareTabSoapAction extends AbstractSoapAction {

	private static final String TAG = "RSSManagementSuite";

	private int m_tabID;
	private String m_userName;

	public ShareTabSoapAction(Activity context) {
		super(context);
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call ShareTabSoapAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.sharetab);
		soapEnvelop = soapEnvelop.replace("{id}", String.valueOf(m_tabID)).replace("{username}", m_userName);
		String result = HttpProvider.request(soapEnvelop, "ShareTab");
		String response = XMLParser.getElementValue(result, "ShareTabResult");
		actionResultCallback.onSuccess(this, Integer.parseInt(response));
	}

	public int getTabID() {
		return m_tabID;
	}

	public void setTabID(int tabID) {
		this.m_tabID = tabID;
	}

	public String getUserName() {
		return m_userName;
	}

	public void setUserName(String userName) {
		this.m_userName = userName;
	}

}
