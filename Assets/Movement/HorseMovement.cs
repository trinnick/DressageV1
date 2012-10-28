using UnityEngine;
using System.Xml;
using System;
using System.Collections;

public class HorseMovement : MonoBehaviour {
	
	public Vector3 defualt = new Vector3();
	
	// Use this for initialization
	IEnumerator Start () {
		
		//Load XML data from external file (In this case from a url web address)
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
			yield return StartCoroutine(ProcessMovement(xmlDoc.SelectNodes("Test/Movement")));	
		}
		else{
			
			//Create error nessage based on opening the xml file
			Debug.Log("Error: " + www.error);
		}	
	}
	
	//Converts an XmlNodeList into Movement objects
	IEnumerator ProcessMovement(XmlNodeList nodes){
		Movement movement;
		
		//Move through each tagged item within the xml document object xmlDoc
		foreach (XmlNode node in nodes){
			movement = new Movement();
			
			//Extract and assign tagged items within the xmlDoc to object variables within Movement class
			movement.MovementID = node.SelectSingleNode("MovementId").InnerText;
			movement.MovementDesc = node.SelectSingleNode("MovementDesc").InnerText;

			//Assign MovementDetails to Movement Class variables
			foreach (XmlNode detail in node.SelectNodes("MovementDetail")){
				movement.Sequence = detail.SelectSingleNode("SequenceId").InnerText;
				movement.Path = detail.SelectSingleNode("Path").InnerText;
				movement.PathDesc = detail.SelectSingleNode("PathDesc").InnerText;
				movement.Gait = detail.SelectSingleNode("Gait").InnerText;
			
				//Call LoadMovement method and pass movement object
				yield return StartCoroutine(LoadMovement(movement));
			}
		}
	}
	
	//Process movements based on collected variable information
	IEnumerator LoadMovement(Movement movement){
		
		Vector3 firstPosition = new Vector3();
		Vector3 secondPosition = new Vector3();
		Vector3 thirdPosition = new Vector3();
		
		Debug.Log("Starting Position: " + transform.position);
		
		//Split the string of path and place in array elements based on seperator character -
		string[] path = movement.Path.Split('-');
		
		//Assign the firstPosition based on the first element of the array path
		firstPosition = GameObject.Find(path[0]).transform.position;
		
		//Assign the secondPosition based on the second elemnt of the array path
		secondPosition = GameObject.Find(path[1]).transform.position;
		
		//If there is a third element in path, assign it to the thirdPosition variable
		if(2 < path.Length){
			thirdPosition = GameObject.Find(path[2]).transform.position;
		}
		
		//Show movement based on path extraction in debugger
		Debug.Log("Movement: " + movement.Path);
		
		//If there are more than 2 waypoints output that information and their positions to the debugger
		if(3 == path.Length){
			Debug.Log(path[0] + ": " + firstPosition + "\n" + path[1] + ": " + secondPosition + "\n" + path[2] + ": " + thirdPosition);
		}
		//If there are only 2 waypoints output that information to the debugger log
		else{
			Debug.Log(path[0] + ": " + firstPosition + "\n" + path[1] + ": " + secondPosition);
		}
		
		//Call movement based on Coroutine and yield commands

		yield return StartCoroutine(firstMovement(firstPosition,secondPosition));
	}
	
	IEnumerator firstMovement(Vector3 first, Vector3 second){
	    float i = 0.0f;
	    float rate = 0.1f;
		
	    while (i < 1.0f) {
			Debug.Log("Inside First Loop");
	        i += Time.deltaTime * rate;
	        transform.position = Vector3.Lerp(first, second, i);
			yield return null;
	    }
			
		Debug.Log("Ending Position: " + transform.position);
	}
}