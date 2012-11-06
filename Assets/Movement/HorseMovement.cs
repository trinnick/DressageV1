using UnityEngine;
using System.Xml;
using System;
using System.Collections;

public class HorseMovement : MonoBehaviour {
	
	public Vector3 defualt = new Vector3();
	public float rateOfMovement = .3f;
	public String movementID = "N/A";
	public String[] movementDesc;
	public String pathDesc = "N/A";
	bool pause = false;
	
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
			
			movementID = movement.MovementID;
			movementDesc = movement.MovementDesc.Split('.');
			
			//Assign MovementDetails to Movement Class variables
			foreach (XmlNode detail in node.SelectNodes("MovementDetail")){
				movement.Sequence = detail.SelectSingleNode("SequenceId").InnerText;
				movement.Path = detail.SelectSingleNode("Path").InnerText;
				movement.PathDesc = detail.SelectSingleNode("PathDesc").InnerText;
				movement.Gait = detail.SelectSingleNode("Gait").InnerText;
				
				pathDesc = movement.PathDesc;
			
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
		Vector3 fourthPosition = new Vector3();
		
		Debug.Log("Starting Position: " + transform.position);
		
		//Split the string of path and place in array elements based on seperator character -
		string[] path = movement.Path.Split('-');
				
		//Assign the firstPosition based on the first element of the array path
		firstPosition = GameObject.Find(path[0]).transform.position;
		
		//Assign the secondPosition based on the second elemnt of the array path
		secondPosition = GameObject.Find(path[1]).transform.position;
		
		//If there is a third element in path, assign it to the thirdPosition variable
		if(path.Length >= 3){
			thirdPosition = GameObject.Find(path[2]).transform.position;
		}
		
		//If there is a third element in path, assign it to the thirdPosition variable
		if(path.Length >= 4){
			fourthPosition = GameObject.Find(path[3]).transform.position;
		}
	
		if(movement.PathDesc == "Line" || movement.PathDesc =="Diagonal"){
			
			//Show movement based on path extraction in debugger
			Debug.Log("Movement: " + movement.Path);
			
			switch(path.Length){
			case 2:
				Debug.Log(path[0] + ": " + firstPosition + "\n" + path[1] + ": " + secondPosition);
				break;
			case 3:
				Debug.Log(path[0] + ": " + firstPosition + "\n" + path[1] + ": " + secondPosition + "\n" + path[2] + ": " + thirdPosition);
				break;
			case 4:
				Debug.Log(path[0] + ": " + firstPosition + "\n" + path[1] + ": " + secondPosition + "\n" + path[2] + ": " + thirdPosition + "\n" + path[3] + ": " + fourthPosition);
				break;
			default:
				Debug.Log("Problem Processing path array");
				break;
			}
			
			//Call movement based on Coroutine and yield commands
	
			if(path.Length>=2){
				yield return StartCoroutine(move(firstPosition,secondPosition));
			}
			
			if(path.Length >= 3){
				yield return StartCoroutine(move(secondPosition,thirdPosition));
			}
			
			if(path.Length >= 4){
				yield return StartCoroutine(move(thirdPosition,fourthPosition));
			}
		}
		else{
			string[] desc = movement.PathDesc.Split('_');
			
			float radius = 0.0f;
			
			if(desc[0] == "HalfCircle"){
				radius = float.Parse(desc[2])/10.0f;
				//Debug.Log("Radius: " + radius);
				
				if(desc[3] == "Down"){
					yield return StartCoroutine(halfCircleMovement(firstPosition,secondPosition,radius));
				}
				
				if(desc[3] == "Up"){
					yield return StartCoroutine(halfCircleMovement(firstPosition,secondPosition,-radius));
				}
			}
		}
				
	}

	void OnGUI(){
		String movementDescFormatted = null;
		for(int i=0;i<movementDesc.Length;i++){
			movementDescFormatted += "\n" + movementDesc[i];
		}
		GUI.Box (new Rect (0,0,200,100), "Movement Description:\n" + movementDescFormatted);
	}
	
	IEnumerator move(Vector3 first, Vector3 second){
	    float dist = Vector3.Distance(first, second);
	
	    for (float i = 0.0f; i < 1.0f; i += (rateOfMovement * Time.deltaTime)) {
	        transform.position = Vector3.Lerp(first, second, i);
			yield return null;
			//Debug.Log("Loop Count: " + i);
			//Debug.Log("DeltaTime: " + Time.deltaTime);
			
			pauseMovement();
	    }
			
		Debug.Log("Ending Position: " + transform.position);
	}
	
	IEnumerator halfCircleMovement(Vector3 first, Vector3 second, float radius){
		Vector3 center = new Vector3();
		Vector3 change = new Vector3(radius,0.0f,0.0f);
	    float dist = Vector3.Distance(first, second);
	
	    for (float i = 0.0f; i < 1.0f; i += (rateOfMovement * Time.deltaTime)) {
		    // The center of the arc
		    center = (first + second)/2;
		    // move the center a bit downwards to make the arc vertical
		    center -= change;
		
		    // Interpolate over the arc relative to center
		    Vector3 arcFirst = first - center;
		    Vector3 arcSecond = second - center;
		    transform.position = Vector3.Slerp(arcFirst, arcSecond, i);
		    transform.position += center;
			yield return null;

			pauseMovement();
	    }
			
		Debug.Log("Ending Position: " + transform.position);
	}
	
	public void pauseMovement(){
			if(Input.GetKeyDown(KeyCode.Space) && pause == false){
				pause = true;
			}
			else if(Input.GetKeyDown(KeyCode.Space) && pause == true){
				pause = false;
			}
			
			if(pause == true){
				Time.timeScale = 0;
			}
			
			else if(pause == false){
				Time.timeScale = 1;
			}		
	}
}