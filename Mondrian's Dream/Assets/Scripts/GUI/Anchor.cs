using UnityEngine;
using System.Collections;

public class Anchor : MonoBehaviour {
	public Camera camera;
	public TextAnchor alignment;
	public float verticalMargin;
	public float horizontalMargin;
	
	void Start () {
		switch (alignment) {
		case TextAnchor.UpperLeft:	
			transform.position = camera.ScreenToWorldPoint(new Vector3(0, Screen.height,0));
			break;
		case TextAnchor.LowerLeft:
			transform.position = camera.ScreenToWorldPoint(new Vector3(0,0,1.0f)) + Vector3.up*(transform.collider.bounds.extents.y*2 + verticalMargin) + 
				Vector3.right*horizontalMargin;
			break;
		case TextAnchor.LowerRight:
			transform.position = camera.ScreenToWorldPoint(new Vector3(Screen.width,0,1.0f)) + Vector3.up*(transform.collider.bounds.extents.y + verticalMargin) - 
				Vector3.right*(horizontalMargin + transform.collider.bounds.extents.x);
			break;
			
		}
		
	}
}