package com.rssms.client.view.dialog;

import android.app.Dialog;
import android.content.Context;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.rssms.client.R;

public class EditTextCustomDialog extends Dialog {
	private String m_title = null;
	private EditText ed_text = null;
	private OnCustomDialogButtonClick m_okClick = null;
	private OnCustomDialogButtonClick m_cancelClick = null;

	public EditTextCustomDialog(Context context, String title, OnCustomDialogButtonClick okClick, OnCustomDialogButtonClick cancelClick) {
		super(context);
		m_title = title;
		m_okClick = okClick;
		m_cancelClick = cancelClick;
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.edittext_customdialog);
		ed_text = (EditText) findViewById(R.id.ed_customdialog);
		setTitle(m_title);

		((Button) findViewById(R.id.btn_ok)).setOnClickListener(new View.OnClickListener() {

			public void onClick(View v) {
				if (m_okClick != null)
					m_okClick.onClick(EditTextCustomDialog.this);
				dismiss();
			}
		});

		((Button) findViewById(R.id.btn_cancel)).setOnClickListener(new View.OnClickListener() {

			public void onClick(View v) {
				if (m_cancelClick != null)
					m_cancelClick.onClick(EditTextCustomDialog.this);
				dismiss();
			}
		});
	}

	public String getUserInput() {
		return ed_text != null ? ed_text.getText().toString() : null;
	}

	public interface OnCustomDialogButtonClick {
		void onClick(EditTextCustomDialog dialog);
	}
}
