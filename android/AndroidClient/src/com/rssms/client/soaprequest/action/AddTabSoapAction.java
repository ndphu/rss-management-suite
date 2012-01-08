package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class AddTabSoapAction extends AbstractSoapAction {

	private static final String TAG = "RSSManagementSuite";
	private String m_tabName;

	public AddTabSoapAction(Activity context) {
		super(context);
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call AddTabSoapAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.addtab);
		soapEnvelop = soapEnvelop.replace("{tabname}", getTabName());
		String result = HttpProvider.request(soapEnvelop, "AddTab");
		String response = XMLParser.getElementValue(result, "AddTabResult");
		actionResultCallback.onSuccess(this, Integer.parseInt(response));
	}

	public String getTabName() {
		return m_tabName;
	}

	public void setTabName(String tabName) {
		this.m_tabName = tabName;
	}

}
