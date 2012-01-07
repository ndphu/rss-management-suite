package com.rssms.client.io;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.http.util.ByteArrayBuffer;

import android.util.Log;

import com.rssms.client.applicationdata.ApplicationData;
import com.rssms.client.applicationdata.ApplicationManager;

public class SocketIO {
	private static final String TAG = SocketIO.class.getName();
    
	public static Map<String, List<String>> sendRequest(Map<String, List<String>> request) {
		Map<String, List<String>> result = new HashMap<String, List<String>>();
		ApplicationData app_data = ApplicationManager.getApplicationData();
		Socket socket = null;
		BufferedReader br = null;
		BufferedWriter bw = null;
		try {
			socket = new Socket(app_data.getHost(), app_data.getPort());
			br = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			bw = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));

			writeHeader(bw, request.get("HEADER"));

			writeBody(bw, request.get("BODY"));

			bw.flush();
			List<String> dataTemp = new ArrayList<String>();

			String line = null;
			while ((line = br.readLine()) != null) {
				Log.e(TAG, line);
				if (line.length() == 0) {
					result.put("HEADER", dataTemp);
					dataTemp = new ArrayList<String>();
					line = null;

					int idata = -1;
					ByteArrayBuffer bab = new ByteArrayBuffer(0);
					while ((idata = br.read()) != -1) {
						bab.append(idata);
					}
					dataTemp.add(new String(bab.buffer()).trim());
					break;
				} else {
					dataTemp.add(line);
				}

				// Set cookie
				if (line.startsWith("Set-Cookie:")) {
					String cookie = line.substring(11);
					ApplicationManager.getApplicationData().setCookie(cookie);
					Log.e(TAG, "Set cookie = " + cookie);
				}
			}
			result.put("BODY", dataTemp);
		} catch (UnknownHostException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			if (br != null) {
				try {
					br.close();
				} catch (IOException e) {
				}
			}

			if (bw != null) {
				try {
					bw.close();
				} catch (IOException e) {
				}
			}

			if (socket != null) {
				try {
					socket.close();
				} catch (IOException e) {
				}
			}
		}

		return result;
	}

	public static void writeBody(BufferedWriter bw, List<String> body) throws IOException {
		for (String str : body) {
			bw.write(str);
			bw.write("\r\n");
		}
		bw.write("\r\n");
	}

	private static void writeHeader(BufferedWriter bw, List<String> header) throws IOException {
		for (String str : header) {
			bw.write(str);
			bw.write("\r\n");
		}

		String cookie = ApplicationManager.getApplicationData().getCookie();
		if (cookie != null && cookie.length() != 0) {
			bw.write("Cookie: " + cookie);
			bw.write("\r\n");
		}

		bw.write("Connection: close");
		bw.write("\r\n");
		bw.write("\r\n");
	}
}
