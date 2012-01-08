package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.util.Log;

import com.rssms.client.R;
import com.rssms.client.io.FileIO;
import com.rssms.client.io.HttpProvider;
import com.rssms.client.io.XMLParser;

public class RenameTabSoapAction extends AbstractSoapAction {

	private static final String TAG = "RSSManagementSuite";

	private int m_tabID;
	private String m_newName;

	public RenameTabSoapAction(Activity context) {
		super(context);
	}

	@Override
	public void callAction(SoapActionResultCallback actionResultCallback) throws Exception {
		Log.i(TAG, "Call RenameTabSoapAction");
		String soapEnvelop = FileIO.getSoapEnvelop(m_context.getResources(), R.raw.renametab);
		soapEnvelop = soapEnvelop.replace("{id}", String.valueOf(m_tabID)).replace("{newname}", m_newName);
		String result = HttpProvider.request(soapEnvelop, "RenameTab");
		String response = XMLParser.getElementValue(result, "RenameTabResult");
		actionResultCallback.onSuccess(this, Integer.parseInt(response));
	}

	public int getTabID() {
		return m_tabID;
	}

	public void setTabID(int tabID) {
		this.m_tabID = tabID;
	}

	public String getNewName() {
		return m_newName;
	}

	public void setNewName(String newName) {
		this.m_newName = newName;
	}

}
