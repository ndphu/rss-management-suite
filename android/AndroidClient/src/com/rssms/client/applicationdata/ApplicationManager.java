package com.rssms.client.applicationdata;

public class ApplicationManager {
	private static ApplicationData APP_DATA;

	public static ApplicationData getApplicationData() {
		if (APP_DATA == null)
			APP_DATA = new ApplicationData();

		return APP_DATA;
	}
}
