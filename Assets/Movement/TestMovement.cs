using UnityEngine;
using System.Xml;
using System;
using System.Collections;

public class TestMovement : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		//Load XML data from external file
		string url = "http://dougstewart.biz/XMLMovementDirections/FirstLevelTest1.xml";
		WWW www = new WWW(url);
		
		//Load the data and yeild (wait) till it is ready before executing
		yield return www;
		if (www.error == null){
			//Successfully loaded the XML
			Debug.Log("Loaded following XML file: \n" + www.text);
			//Create new XML document from loaded data
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
						
			//Point to the Movement nodes and process them
			ProcessMovement(xmlDoc.SelectNodes("Test/Movement"));

			
		}
		else{
			Debug.Log("Error: " + www.error);
		}	
	}
	
	//Converts an XmlNodeList into Movement objects
	private void ProcessMovement(XmlNodeList nodes){
		Movement movement;
		
		foreach (XmlNode node in nodes){
			movement = new Movement();
			movement.MovementID = node.SelectSingleNode("MovementId").InnerText;
			movement.MovementDesc = node.SelectSingleNode("MovementDesc").InnerText;

			//Assign MovementDetails
			foreach (XmlNode detail in node.SelectNodes("MovementDetail")){
				movement.Sequence = detail.SelectSingleNode("SequenceId").InnerText;
				movement.Path = detail.SelectSingleNode("Path").InnerText;
				movement.PathDesc = detail.SelectSingleNode("PathDesc").InnerText;
				movement.Gait = detail.SelectSingleNode("Gait").InnerText;
			
				LoadMovement(movement);
			}
		}
	}
	
	void LoadMovement(Movement movement){
		string[] path = movement.Path.Split('-');
		Vector3 start = GameObject.Find(path[0]).transform.position;
		Vector3 end = GameObject.Find(path[1]).transform.position;
		transform.Translate(end);
	}
}