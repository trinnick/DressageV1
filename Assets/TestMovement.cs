using UnityEngine;
using System.Collections;

public class TestMovement : MonoBehaviour {
	
	IEnumerator Start () {
	    yield return StartCoroutine(test ());
		Debug.Log("test has run and start has control again");
	}
	IEnumerator test(){
		Debug.Log("test() started");
		yield return StartCoroutine(f ());
		Debug.Log("test() ended");
	}
	IEnumerator f() {
	    Debug.Log("f() started");
	    yield return StartCoroutine(g());
	    Debug.Log("f() is done");
	}
	
	IEnumerator g() {
	    Debug.Log("g() started");
		float i = 0.0f;
	    while (i<10.0f)
		{
			i ++;
			Debug.Log(i);
	        yield return null;
		}
	    Debug.Log("g() is done");
	}
}
