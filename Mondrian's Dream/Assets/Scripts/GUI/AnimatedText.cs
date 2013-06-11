using UnityEngine;
using System.Collections;

public class AnimatedText : MonoBehaviour {
	private string _text;
	public string Text {
		set {
			if (_text != value) {
				animation.Play("ChangeText");
				_text = value;
			}
		}
	}
	private TextMesh _textMesh;
	
	void Start () {
		_textMesh = transform.GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ChangeText() {
		_textMesh.text = _text;
	}
}
