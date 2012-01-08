package com.rssms.client.soaprequest.action;

import android.app.Activity;
import android.app.AlertDialog;

import com.rssms.client.view.ProgressDialogManager;

public abstract class SoapActionResultCallback {
		private Activity m_context;
		public SoapActionResultCallback(Activity context) {
			m_context = context;
		}
		public void onStart(AbstractSoapAction action, Object startInfo) {
			m_context.runOnUiThread(new Runnable() {

				public void run() {
					ProgressDialogManager.showProgressDialog("Connect to server", "Connecting", m_context);
				}
			});
		}

		public void onSuccess(AbstractSoapAction action, Object result) {
			m_context.runOnUiThread(new Runnable() {
				public void run() {
					ProgressDialogManager.dissmissProgressDialog();
				}
			});
		}

		public void onException(AbstractSoapAction action, final Exception result) {
			m_context.runOnUiThread(new Runnable() {
				public void run() {
					ProgressDialogManager.dissmissProgressDialog();
					AlertDialog.Builder builder = new AlertDialog.Builder(m_context);
					builder.setTitle("Error").setMessage(result.getMessage()).setCancelable(false).setPositiveButton("OK", null);
					builder.create().show();
				}
			});
		}
	}