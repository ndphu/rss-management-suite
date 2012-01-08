package com.rssms.client.applicationdata;

public class ApplicationData {
	private String m_username;
	private String m_password;
	private String m_cookie;
	private String m_host;
	private int m_port;
	private String m_serviceName;

	public String getUsername() {
		return m_username;
	}

	public void setUsername(String username) {
		this.m_username = username;
	}

	public String getPassword() {
		return m_password;
	}

	public void setPassword(String password) {
		this.m_password = password;
	}

	public String getCookie() {
		return m_cookie;
	}

	public void setCookie(String cookie) {
		this.m_cookie = cookie;
	}

	public int getPort() {
		return m_port;
	}

	public void setPort(int port) {
		this.m_port = port;
	}

	public String getHost() {
		return m_host;
	}

	public void setHost(String host) {
		this.m_host = host;
	}

	public String getRequestURL() {
		return "http://" + m_host + ":" + String.valueOf(m_port) + "/" + m_serviceName;
	}

	public String getServiceName() {
		return m_serviceName;
	}

	public void setServiceName(String serviceName) {
		this.m_serviceName = serviceName;
	}
}
