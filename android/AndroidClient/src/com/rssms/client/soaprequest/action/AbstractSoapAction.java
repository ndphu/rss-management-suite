package com.rssms.client.soaprequest.action;

import android.content.Context;

public abstract class AbstractSoapAction {
	protected Context m_context;

	public AbstractSoapAction(Context context) {
		m_context = context;
	}

	public abstract Object execute();
}
