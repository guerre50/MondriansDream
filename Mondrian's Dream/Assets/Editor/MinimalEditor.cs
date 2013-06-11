using UnityEngine;
using System.Collections;
using UnityEditor;




public class MinimalEditor : EditorWindow {
	
	
	
	
	[MenuItem ("Window/Minimalistic Utils")]
	static void Init ()
	{
		// Get existing open window or if none, make a new one:
		MinimalEditor window = (MinimalEditor)EditorWindow.GetWindow(typeof(MinimalEditor));
		window.Show();
	}

	void OnGUI ()
	{
		GUILayout.Label ("Select edges in: Down, Left, Top, Right:", EditorStyles.boldLabel);
		if(GUILayout.Button("SetShapes")) {
			Logic.instance.InitFigures();
		}
	}
	
	void Connect()	{
		Debug.Log ("dfas");
		GameObject[] objs = Selection.gameObjects;
		
		// CHECK PARAMS!!!
		Figure fig = null;
		int edges = 0;
		foreach (GameObject obj in objs) {
			if (fig == null) {
				fig = (Figure)obj.GetComponent(typeof(Figure));
			}
			if (obj.GetComponent(typeof(Edge))) {
				++edges;	
			}
		}
		
		if (fig != null && edges == 4) {
			fig.edges = new Edge[4];
			
			foreach (GameObject obj in objs) {
				if (obj != fig.gameObject) {
					Edge edge = (Edge)obj.GetComponent(typeof(Edge));
					
					if (edge) {
						// TO-DO make this better!
						if (edge.direction == EdgeDirection.Horizontal) {
							if (fig.edges[0] == null) {
								fig.edges[0] = edge;
							} else {
								if (fig.edges[0].transform.position.z > edge.transform.position.z) {
									fig.edges[2] = fig.edges[0];
									fig.edges[0] = edge;
								} else {
									fig.edges[2] = edge;
								}
							}
						} else {
							if (fig.edges[1] == null) {
								fig.edges[1] = edge;
							} else {
								if (fig.edges[1].transform.position.x > edge.transform.position.x) {
									fig.edges[3] = fig.edges[1];
									fig.edges[1] = edge;
								} else {
									fig.edges[3] = edge;
								}
							}
							
						}
					}
				}
			}
			Debug.Log ("[MINIMAL EDITOR] Connected");
			EditorUtility.FocusProjectWindow();
		} else {
			if (fig == null) {
				Debug.Log ("[MINIMAL EDITOR] Please select a figure");
			}
			if (edges != 4) {
				Debug.Log ("[MINIMAL EDITOR] You have selected " + edges + " but 4 are needed");
			}
		}
	}
}


