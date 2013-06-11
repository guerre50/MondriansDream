using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneCollider : MonoBehaviour {
	private Figure _figure;
	// Use this for initialization
	void Start () {
		_figure = (Figure)transform.parent.GetComponent(typeof(Figure));
	}
	
	void OnTriggerEnter(Collider col) {
		if (col.name == "plane") Debug.Log ("planeMERDA");
		_figure.AddAdjacent(col.transform.parent.GetComponent<Figure>());	
		
	}
	
	void OnTriggerExit(Collider col) {
		_figure.RemoveAdjacent(col.transform.parent.GetComponent<Figure>());
	}
}
