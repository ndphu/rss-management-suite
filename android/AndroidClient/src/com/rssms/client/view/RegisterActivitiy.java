package com.rssms.client.view;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import com.rssms.client.R;
import com.rssms.client.soaprequest.SoapRequestInvoker;
import com.rssms.client.soaprequest.action.AbstractSoapAction;
import com.rssms.client.soaprequest.action.CheckValidUsernameAction;
import com.rssms.client.soaprequest.action.RegisterSoapAction;
import com.rssms.client.soaprequest.action.SoapActionResultCallback;

public class RegisterActivitiy extends Activity {

	private EditText ed_username = null;
	private EditText ed_password = null;
	private EditText ed_reenterpassword = null;
	private String m_username = null;
	private String m_password = null;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.register_view);

		ed_username = (EditText) findViewById(R.id.username);
		ed_password = (EditText) findViewById(R.id.password);
		ed_reenterpassword = (EditText) findViewById(R.id.reenterpassword);
	}

	public void onSubmitClick(View view) {
		String username = ed_username.getText().toString();
		String password = ed_password.getText().toString();
		String reenterpassword = ed_reenterpassword.getText().toString();

		if (username == null || username.trim().length() == 0) {
			Toast.makeText(getApplicationContext(), "Username can't be empty", Toast.LENGTH_SHORT).show();
			return;
		}

		if (password == null || password.trim().length() == 0) {
			Toast.makeText(getApplicationContext(), "Password can't be empty", Toast.LENGTH_SHORT).show();
			return;
		}

		if (reenterpassword == null || reenterpassword.compareTo(password) != 0) {
			Toast.makeText(getApplicationContext(), "Re-enter password doen's match with password", Toast.LENGTH_SHORT).show();
			return;
		}
		m_username = username;
		m_password = password;
		CheckValidUsernameAction action = new CheckValidUsernameAction(RegisterActivitiy.this);
		action.setUsername(username);
		SoapRequestInvoker.invokeSoapAction(action, new SoapActionResultCallback(RegisterActivitiy.this) {
			@Override
			public void onSuccess(AbstractSoapAction action, Object result) {
				super.onSuccess(action, result);
				Boolean b = (Boolean) result;
				if (b == true) {
					RegisterSoapAction registerSoapAction = new RegisterSoapAction(RegisterActivitiy.this);
					registerSoapAction.setUsername(m_username);
					registerSoapAction.setPassword(m_password);
					SoapRequestInvoker.invokeSoapAction(registerSoapAction, new SoapActionResultCallback(RegisterActivitiy.this) {
						public void onSuccess(AbstractSoapAction action, final Object result) {
							super.onSuccess(action, result);
							runOnUiThread(new Runnable() {

								public void run() {
									Boolean b = (Boolean) result;
									if (b == true) {
										AlertDialog.Builder builder = new AlertDialog.Builder(RegisterActivitiy.this);
										builder.setTitle("Regist Account Result").setMessage("REGIST NEW ACCOUNT COMPLETE").setCancelable(false)
												.setPositiveButton("OK", new DialogInterface.OnClickListener() {
													public void onClick(DialogInterface dialog, int id) {
														Intent intent = new Intent();
														intent.putExtra("USERNAME", m_username);
														intent.putExtra("PASSWORD", m_password);
														RegisterActivitiy.this.setResult(1, intent);
														finish();
													}
												});
										builder.create().show();
									}
								}
							});
						};
					});
				} else {
					Toast.makeText(getApplicationContext(), "Username is existed, please chose other one", Toast.LENGTH_SHORT).show();
				}
			}
		});

	}

	public void onBackClick(View view) {
		setResult(-1);
		finish();
	}

	@Override
	public void onBackPressed() {
	}
}
