package com.rssms.client.view;

import android.app.Activity;
import android.app.ProgressDialog;

public class ProgressDialogManager {
	private static ProgressDialog PROGRESS_DIALOG = null;

	public static void showProgressDialog(String title, String message, Activity context) {
		if (context == null)
			return;
		PROGRESS_DIALOG = ProgressDialog.show(context, title, message, true, false);
		// PROGRESS_DIALOG = new ProgressDialog(context);
		// PROGRESS_DIALOG.dismiss();
		// PROGRESS_DIALOG.setCancelable(false);
		// PROGRESS_DIALOG.setTitle(title);
		// PROGRESS_DIALOG.setMessage(message);
		//
		// PROGRESS_DIALOG.show();
	}

	public static void dissmissProgressDialog() {
		PROGRESS_DIALOG.dismiss();
	}
}
