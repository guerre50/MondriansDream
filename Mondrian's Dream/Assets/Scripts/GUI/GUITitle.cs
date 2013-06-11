using UnityEngine;
using System.Collections;

public class GUITitle : MonoBehaviour {
	public GameObject title;
	
	void Update() {
		if (!title.animation.isPlaying) {
			title.animation.Play("TitleIdle");
		}
	}
}
