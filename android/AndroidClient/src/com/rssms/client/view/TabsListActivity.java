package com.rssms.client.view;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView.OnItemLongClickListener;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.rssms.client.R;
import com.rssms.client.applicationdata.ApplicationManager;
import com.rssms.client.data.Tab;
import com.rssms.client.soaprequest.SoapRequestInvoker;
import com.rssms.client.soaprequest.action.AbstractSoapAction;
import com.rssms.client.soaprequest.action.AddTabSoapAction;
import com.rssms.client.soaprequest.action.GetAllTabsSoapAction;
import com.rssms.client.soaprequest.action.LogoutSoapAction;
import com.rssms.client.soaprequest.action.RemoveTabSoapAction;
import com.rssms.client.soaprequest.action.RenameTabSoapAction;
import com.rssms.client.soaprequest.action.ShareTabSoapAction;
import com.rssms.client.soaprequest.action.SoapActionResultCallback;
import com.rssms.client.view.adapter.TabArrayAdapter;
import com.rssms.client.view.dialog.EditTextCustomDialog;
import com.rssms.client.view.dialog.EditTextCustomDialog.OnCustomDialogButtonClick;

public class TabsListActivity extends Activity {
	private ListView m_listView = null;
	private TabArrayAdapter m_adapter = null;
	private TextView m_tv_username;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.tabslist_view);

		m_tv_username = (TextView) findViewById(R.id.username);
		m_listView = (ListView) findViewById(R.id.listView);
		m_adapter = new TabArrayAdapter(this, 0, 0, new ArrayList<Tab>());
		m_listView.setAdapter(m_adapter);
		m_listView.setOnItemClickListener(onItemClickListener);
		m_listView.setOnItemLongClickListener(onItemLongClickListener);

		m_tv_username.setText(ApplicationManager.getApplicationData().getUsername());

		refreshList();
	}

	private void refreshList() {
		m_adapter.clear();
		GetAllTabsSoapAction getAllTabsSoapAction = new GetAllTabsSoapAction(TabsListActivity.this);
		SoapRequestInvoker.invokeSoapAction(getAllTabsSoapAction, new SoapActionResultCallback(TabsListActivity.this) {
			@Override
			public void onSuccess(AbstractSoapAction action, final Object result) {
				super.onSuccess(action, result);
				runOnUiThread(new Runnable() {
					public void run() {
						ProgressDialogManager.dissmissProgressDialog();
						@SuppressWarnings("unchecked")
						List<Tab> tabs = (List<Tab>) result;
						for (Tab tab : tabs) {
							m_adapter.add(tab);
							m_listView.invalidate();
						}
					}
				});
			}
		});

	}

	private OnItemClickListener onItemClickListener = new OnItemClickListener() {

		public void onItemClick(AdapterView<?> adapter, View view, int position, long arg3) {
			openTab(position);
		}
	};

	private OnItemLongClickListener onItemLongClickListener = new OnItemLongClickListener() {

		public boolean onItemLongClick(AdapterView<?> adapter, View view, final int position, long arg3) {
			final CharSequence[] items = { "Open", "Rename", "Share", "Remove" };

			new AlertDialog.Builder(TabsListActivity.this).setTitle("Select Action").setItems(items, new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int item) {
					switch (item) {
					case 0:
						openTab(position);
						break;
					case 1:
						renameTab(position);
						break;
					case 2:
						shareTab(position);
						break;
					case 3:
						removeTab(position);
						break;
					default:
						break;
					}
				}
			}).create().show();
			return true;
		}
	};

	private void openTab(int idx) {

	}

	private void renameTab(final int idx) {
		EditTextCustomDialog editDialog = new EditTextCustomDialog(TabsListActivity.this, "Enter New Name", new OnCustomDialogButtonClick() {

			public void onClick(EditTextCustomDialog dialog) {
				String newName = dialog.getUserInput();
				int tabId = m_adapter.getItem(idx).getID();
				RenameTabSoapAction renameTabSoapAction = new RenameTabSoapAction(TabsListActivity.this);
				renameTabSoapAction.setTabID(tabId);
				renameTabSoapAction.setNewName(newName);
				SoapRequestInvoker.invokeSoapAction(renameTabSoapAction, new SoapActionResultCallback(TabsListActivity.this) {
					@Override
					public void onSuccess(AbstractSoapAction action, final Object result) {
						super.onSuccess(action, result);
						runOnUiThread(new Runnable() {
							public void run() {
								switch ((Integer) result) {
								case 0:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Rename Success", Toast.LENGTH_SHORT).show();
									break;
								case 1:
									Toast.makeText(TabsListActivity.this, "You are not the owner of this tab", Toast.LENGTH_SHORT).show();
									break;
								case 2:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Tab doesn't exist", Toast.LENGTH_SHORT).show();
									break;
								case 3:
									Toast.makeText(TabsListActivity.this, "Duplicate tab's name", Toast.LENGTH_SHORT).show();
									break;
								case 4:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Internal server error", Toast.LENGTH_SHORT).show();
									break;
								default:
									break;
								}
							}
						});
					}
				});
			}
		}, null);

		editDialog.show();
	}

	private void shareTab(final int idx) {
		EditTextCustomDialog editDialog = new EditTextCustomDialog(TabsListActivity.this, "Enter Username To Share", new OnCustomDialogButtonClick() {

			public void onClick(EditTextCustomDialog dialog) {
				String username = dialog.getUserInput();
				int tabId = m_adapter.getItem(idx).getID();
				ShareTabSoapAction shareTabSoapAction = new ShareTabSoapAction(TabsListActivity.this);
				shareTabSoapAction.setTabID(tabId);
				shareTabSoapAction.setUserName(username);
				SoapRequestInvoker.invokeSoapAction(shareTabSoapAction, new SoapActionResultCallback(TabsListActivity.this) {
					@Override
					public void onSuccess(AbstractSoapAction action, final Object result) {
						super.onSuccess(action, result);
						runOnUiThread(new Runnable() {
							public void run() {
								switch ((Integer) result) {
								case 0:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Share Tab Success", Toast.LENGTH_SHORT).show();
									break;
								case 1:
									Toast.makeText(TabsListActivity.this, "You are not the owner of this tab", Toast.LENGTH_SHORT).show();
									break;
								case 2:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Tab doesn't exist", Toast.LENGTH_SHORT).show();
									break;
								case 3:
									Toast.makeText(TabsListActivity.this, "Username that you want to share doesn't exist", Toast.LENGTH_SHORT).show();
									break;
								case 4:
									Toast.makeText(TabsListActivity.this, "You can't not share yourself", Toast.LENGTH_SHORT).show();
									break;
								case 5:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Internal server error", Toast.LENGTH_SHORT).show();
									break;
								default:
									break;
								}
							}
						});
					}
				});

			}
		}, null);

		editDialog.show();
	}

	private void removeTab(final int idx) {
		AlertDialog.Builder builder = new AlertDialog.Builder(TabsListActivity.this);
		builder.setTitle("Confirm Remove Tab").setMessage("Do you want remove this tab?").setCancelable(false)
				.setPositiveButton("OK", new DialogInterface.OnClickListener() {

					public void onClick(DialogInterface dialog, int which) {
						int tabId = m_adapter.getItem(idx).getID();
						RemoveTabSoapAction removeTabSoapAction = new RemoveTabSoapAction(TabsListActivity.this);
						removeTabSoapAction.setTabID(tabId);
						SoapRequestInvoker.invokeSoapAction(removeTabSoapAction, new SoapActionResultCallback(TabsListActivity.this) {
							@Override
							public void onSuccess(AbstractSoapAction action, final Object result) {
								super.onSuccess(action, result);
								runOnUiThread(new Runnable() {
									public void run() {
										switch ((Integer) result) {
										case 0:
											refreshList();
											Toast.makeText(TabsListActivity.this, "Remove Tab Success", Toast.LENGTH_SHORT).show();
											break;
										case 1:
											Toast.makeText(TabsListActivity.this, "You are not the owner of this tab", Toast.LENGTH_SHORT).show();
											break;
										case 2:
											refreshList();
											Toast.makeText(TabsListActivity.this, "Tab doesn't exist", Toast.LENGTH_SHORT).show();
											break;
										case 3:
											refreshList();
											Toast.makeText(TabsListActivity.this, "Internal server error", Toast.LENGTH_SHORT).show();
											break;
										default:
											break;
										}
									}
								});
							}
						});
					}
				}).setNegativeButton("Cancel", null);
		builder.create().show();
	}

	public void onLogoutClick(View view) {
		AlertDialog.Builder builder = new AlertDialog.Builder(TabsListActivity.this);
		builder.setTitle("Confirm Logout").setMessage("Do you want to logout?").setCancelable(false)
				.setPositiveButton("OK", new DialogInterface.OnClickListener() {

					public void onClick(DialogInterface dialog, int which) {
						LogoutSoapAction logoutSoapAction = new LogoutSoapAction(TabsListActivity.this);
						SoapRequestInvoker.invokeSoapAction(logoutSoapAction, new SoapActionResultCallback(TabsListActivity.this) {
							@Override
							public void onSuccess(AbstractSoapAction action, Object result) {
								super.onSuccess(action, result);
								runOnUiThread(new Runnable() {

									public void run() {
										TabsListActivity.this.finish();
									}
								});
							}

							@Override
							public void onException(AbstractSoapAction action, Exception result) {
								super.onException(action, result);
								runOnUiThread(new Runnable() {

									public void run() {
										TabsListActivity.this.finish();
									}
								});
							}
						});
					}
				}).setNegativeButton("Cancel", null);
		builder.create().show();

	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.tabslist_menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case R.id.newTab:
			addNewTab();
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	private void addNewTab() {
		EditTextCustomDialog editDialog = new EditTextCustomDialog(TabsListActivity.this, "Enter Tab's Name", new OnCustomDialogButtonClick() {

			public void onClick(EditTextCustomDialog dialog) {
				String tabName = dialog.getUserInput();
				AddTabSoapAction addTabSoapAction = new AddTabSoapAction(TabsListActivity.this);
				addTabSoapAction.setTabName(tabName);
				SoapRequestInvoker.invokeSoapAction(addTabSoapAction, new SoapActionResultCallback(TabsListActivity.this) {
					@Override
					public void onSuccess(AbstractSoapAction action, final Object result) {
						super.onSuccess(action, result);
						runOnUiThread(new Runnable() {
							public void run() {
								switch ((Integer) result) {
								case 0:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Add Tab Success", Toast.LENGTH_SHORT).show();
									break;
								case 1:
									Toast.makeText(TabsListActivity.this, "Duplicate Tab Name", Toast.LENGTH_SHORT).show();
									break;
								case 2:
									refreshList();
									Toast.makeText(TabsListActivity.this, "Internal server error", Toast.LENGTH_SHORT).show();
									break;
								default:
									break;
								}
							}
						});
					}
				});
			}
		}, null);

		editDialog.show();
	}

	@Override
	public void onBackPressed() {
	}

}
