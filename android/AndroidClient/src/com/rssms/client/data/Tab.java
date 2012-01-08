package com.rssms.client.data;

public class Tab {
	private int m_iD;
	private String m_name;
	private int m_userID;
	private String m_ownerName;
	
	public Tab(){
		
	}

	public int getID() {
		return m_iD;
	}

	public void setID(int iD) {
		m_iD = iD;
	}

	public String getName() {
		return m_name;
	}

	public void setName(String name) {
		m_name = name;
	}

	public int getUserID() {
		return m_userID;
	}

	public void setUserID(int userID) {
		m_userID = userID;
	}

	public String getOwnerName() {
		return m_ownerName;
	}

	public void setOwnerName(String ownerName) {
		this.m_ownerName = ownerName;
	}

}
