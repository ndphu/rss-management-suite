package com.rssms.client.view;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;

import com.rssms.client.R;
import com.rssms.client.applicationdata.ApplicationManager;

public class ConfigActivity extends Activity {
	private EditText ed_host;
	private EditText ed_port;
	private EditText ed_service;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.config_view);

		ed_host = (EditText) findViewById(R.id.host);
		ed_port = (EditText) findViewById(R.id.port);
		ed_service = (EditText) findViewById(R.id.service);

		ed_host.setText(ApplicationManager.getApplicationData().getHost());
		ed_port.setText(String.valueOf(ApplicationManager.getApplicationData().getPort()));
		ed_service.setText(ApplicationManager.getApplicationData().getServiceName().replace(".asmx", ""));
	}

	public void onSaveClick(View view) {
		ApplicationManager.getApplicationData().setHost(ed_host.getText().toString());
		ApplicationManager.getApplicationData().setPort(Integer.parseInt(ed_port.getText().toString()));
		ApplicationManager.getApplicationData().setServiceName(ed_service.getText().toString() + ".asmx");

		finish();
	}

	public void onBackClick(View view) {
		finish();
	}

	@Override
	public void onBackPressed() {
		// Do nothing
	}
}
