package com.rssms.client.view;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.EditText;

import com.rssms.client.R;
import com.rssms.client.applicationdata.ApplicationManager;
import com.rssms.client.soaprequest.SoapRequestInvoker;
import com.rssms.client.soaprequest.action.AbstractSoapAction;
import com.rssms.client.soaprequest.action.LoginSoapAction;
import com.rssms.client.soaprequest.action.SoapActionResultCallback;

public class LoginActivity extends Activity {
	private static final int REGISTER_CODE = 1;
	private EditText m_ed_username = null;
	private EditText m_ed_password = null;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.login_view);

		m_ed_username = (EditText) findViewById(R.id.username);
		m_ed_password = (EditText) findViewById(R.id.password);

		ApplicationManager.getApplicationData().setHost(getResources().getString(R.string.host_name));
		ApplicationManager.getApplicationData().setPort(Integer.parseInt(getResources().getString(R.string.host_port)));
		ApplicationManager.getApplicationData().setServiceName(getResources().getString(R.string.service_name));
		ApplicationManager.getApplicationData().setUsername("");
	}

	public void onLoginClick(View view) {
		String username = m_ed_username.getText().toString();
		String password = m_ed_password.getText().toString();

		LoginSoapAction loginSoapAction = new LoginSoapAction(LoginActivity.this);
		loginSoapAction.setUsername(username);
		loginSoapAction.setPassword(password);

		SoapRequestInvoker.invokeSoapAction(loginSoapAction, new SoapActionResultCallback(LoginActivity.this) {
			@Override
			public void onSuccess(AbstractSoapAction action, Object result) {
				super.onSuccess(action, result);
				Boolean b = (Boolean) result;
				if (b == true) {
					ApplicationManager.getApplicationData().setUsername(m_ed_username.getText().toString());
					Intent intent = new Intent(getApplicationContext(), TabsListActivity.class);
					LoginActivity.this.startActivity(intent);
				} else {
					AlertDialog.Builder builder = new AlertDialog.Builder(LoginActivity.this);
					builder.setTitle("Login Fail").setMessage("Invalid username or password.").setCancelable(false).setPositiveButton("OK", null);
					builder.create().show();
				}
			}
		});
	}

	public void onRegisterClick(View view) {
		openRegisterActivity();
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == REGISTER_CODE) {
			if (resultCode == 1) {
				if (data != null) {
					String username = data.getStringExtra("USERNAME");
					String password = data.getStringExtra("PASSWORD");
					m_ed_username.setText(username);
					m_ed_password.setText(password);
				}
			}
		}
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.login_menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case R.id.config:
			openConfigActivity();
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	private void openConfigActivity() {
		Intent intent = new Intent(getApplicationContext(), ConfigActivity.class);
		startActivity(intent);
	}

	private void openRegisterActivity() {
		Intent intent = new Intent(getApplicationContext(), RegisterActivitiy.class);
		startActivityForResult(intent, REGISTER_CODE);
	}

	public void onExitClick(View view) {
		AlertDialog.Builder builder = new AlertDialog.Builder(LoginActivity.this);
		builder.setTitle("Confirm Exit").setMessage("Do you want to exit?").setCancelable(false).setPositiveButton("OK", new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int which) {
				finish();
			}
		}).setNegativeButton("CANCEL", null);
		builder.create().show();
	}

	@Override
	public void onBackPressed() {
		// Do nothing
	}
}
