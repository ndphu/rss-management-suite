package com.rssms.client.io;

import java.io.ByteArrayInputStream;
import java.io.IOException;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

public class XMLParser {
	public static String getElementValue(String xmlString, String elementName) throws ParserConfigurationException, SAXException, IOException {
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		DocumentBuilder builder = factory.newDocumentBuilder();
		Document dom = builder.parse(new InputSource(new ByteArrayInputStream(xmlString.getBytes())));
		Element root = dom.getDocumentElement();
		return root.getElementsByTagName(elementName).item(0).getTextContent();
	}
}
