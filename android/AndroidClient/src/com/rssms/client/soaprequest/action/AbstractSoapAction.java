package com.rssms.client.soaprequest.action;

import android.app.Activity;

public abstract class AbstractSoapAction {
	protected Activity m_context;

	public AbstractSoapAction(Activity context) {
		m_context = context;
	}

	public final void execute(SoapActionResultCallback actionCallback) {
		actionCallback.onStart(this, null);

		try {
			callAction(actionCallback);
		} catch (Exception ex) {
			actionCallback.onException(this, ex);
		}
	}

	public abstract void callAction(SoapActionResultCallback actionResultCallback) throws Exception;
}
