package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class RemoveTabSoapAction extends AbstractSoapAction {
	private static final String TAG = "RSSManagementSuite";
	private int m_tabID;

	public RemoveTabSoapAction(Activity context) {
		super(context);
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call RemoveTabSoapAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.removetab);
		soapEnvelop = soapEnvelop.replace("{id}", String.valueOf(m_tabID));
		String result = HttpProvider.request(soapEnvelop, "RemoveTab");
		String response = XMLParser.getElementValue(result, "RemoveTabResult");
		actionResultCallback.onSuccess(this, Integer.parseInt(response));
	}

	public int getTabID() {
		return m_tabID;
	}

	public void setTabID(int tabID) {
		this.m_tabID = tabID;
	}

}
