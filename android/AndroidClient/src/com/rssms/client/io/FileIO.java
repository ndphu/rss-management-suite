package com.rssms.client.io;

import android.content.res.Resources;


public class FileIO {

	public static String getSoapEnvelop(Resources resource, int fileID){
		return IOUtility.getStringFromStream(resource.openRawResource(fileID));
	}
}
