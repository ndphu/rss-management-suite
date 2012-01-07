package com.rssms.client.soaprequest;

import com.rssms.client.soaprequest.action.AbstractSoapAction;

public class SoapRequestInvoker {
	public static Object invokeSoapAction(AbstractSoapAction soapAction){
		return soapAction.execute();
	}
}
