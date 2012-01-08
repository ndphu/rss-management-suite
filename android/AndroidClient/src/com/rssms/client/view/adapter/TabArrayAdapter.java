package com.rssms.client.view.adapter;

import java.util.List;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import com.rssms.client.R;
import com.rssms.client.data.Tab;

public class TabArrayAdapter extends ArrayAdapter<Tab> {
	private LayoutInflater m_inflater;

	public TabArrayAdapter(Context context, int resource, int textViewResourceId, List<Tab> objects) {
		super(context, resource, textViewResourceId, objects);
		m_inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		if (convertView == null) {
			convertView = m_inflater.inflate(R.layout.tab_listitem, null, false);
			saveViewHolder(convertView);
		} else if (convertView.getTag() == null) {
			saveViewHolder(convertView);
		}

		ViewHolder holder = (ViewHolder) convertView.getTag();
		Tab tab = getItem(position);
		holder.tv_TabName.setText(tab.getName());
		holder.tv_TabOwner.setText(tab.getOwnerName());
		return convertView;
	}

	private void saveViewHolder(View view) {
		ViewHolder holder = new ViewHolder();
		holder.tv_TabName = (TextView) view.findViewById(R.id.tabName);
		holder.tv_TabOwner = (TextView) view.findViewById(R.id.tabOwner);
		view.setTag(holder);
	}

	private class ViewHolder {
		public TextView tv_TabName;
		public TextView tv_TabOwner;
	}
}
