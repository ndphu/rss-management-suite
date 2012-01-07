package com.rssms.client.data;

public class RSSItem {
	private int m_iD;
	private String m_name;
	private String m_description;
	private String m_RSSLink;
	private int m_tabID;
	
	public int getID() {
		return m_iD;
	}
	public void setID(int mID) {
		this.m_iD = mID;
	}
	public String getName() {
		return m_name;
	}
	public void setName(String name) {
		m_name = name;
	}
	public String getDescription() {
		return m_description;
	}
	public void setDescription(String description) {
		m_description = description;
	}
	public String getRSSLink() {
		return m_RSSLink;
	}
	public void setRSSLink(String rSSLink) {
		m_RSSLink = rSSLink;
	}
	public int getTabID() {
		return m_tabID;
	}
	public void setTabID(int tabID) {
		m_tabID = tabID;
	}
}
