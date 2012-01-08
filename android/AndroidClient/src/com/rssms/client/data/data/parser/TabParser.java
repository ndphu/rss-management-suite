package com.rssms.client.data.data.parser;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import com.rssms.client.data.Tab;

public class TabParser {
	public static List<Tab> getTabList(String xmlString) throws ParserConfigurationException, SAXException, IOException {
		List<Tab> result = new ArrayList<Tab>();
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		DocumentBuilder builder = factory.newDocumentBuilder();
		Document dom = builder.parse(new InputSource(new ByteArrayInputStream(xmlString.getBytes())));
		Element root = dom.getDocumentElement();

		NodeList tabDTOs = root.getElementsByTagName("TabDTO");
		int len = tabDTOs.getLength();
		for (int i = 0; i < len; ++i) {
			Element tabDTO = (Element) tabDTOs.item(i);
			Tab tab = new Tab();
			tab.setID(Integer.parseInt(tabDTO.getElementsByTagName("Id").item(0).getTextContent()));
			tab.setName(tabDTO.getElementsByTagName("Name").item(0).getTextContent());
			tab.setUserID(Integer.parseInt(tabDTO.getElementsByTagName("OwnerID").item(0).getTextContent()));
			tab.setOwnerName(tabDTO.getElementsByTagName("OwnerUsername").item(0).getTextContent());
			result.add(tab);
		}

		return result;
	}
}
