package com.rssms.client.soaprequest;

import com.rssms.client.soaprequest.action.AbstractSoapAction;
import com.rssms.client.soaprequest.action.SoapActionResultCallback;

public class SoapRequestInvoker {
	public static void invokeSoapAction(final AbstractSoapAction soapAction, final SoapActionResultCallback actionCallback){
		new Thread(new Runnable() {
			public void run() {
				soapAction.execute(actionCallback);
			}
		}).start();
	}
}
