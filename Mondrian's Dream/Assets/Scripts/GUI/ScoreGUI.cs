using UnityEngine;
using System.Collections;

public class ScoreGUI : MonoBehaviour {
	public GameObject[] scores;
	public GameObject[] targets;
	private Logic _logic;
	private float _minY;
	private float _maxY;
	
	
	// Use this for initialization
	void Start () {
		_logic = Logic.instance;
		
		_minY = transform.collider.bounds.min.y;
		_maxY = transform.collider.bounds.max.y;
		UpdateScores();
		InitTargetScores();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateScores();
	}
	
	void UpdateScores() {
		for(int i = 0; i < _logic.target.Length; ++i) {
			float target = _logic.target[i];
			Vector3 scale = targets[i].transform.localScale;
			float relative = Mathf.Min (_logic._score[i]/_logic.target[i], 1.0f);
			scale.z = (_maxY - _minY)*relative/10;
			
			
			targets[i].transform.localScale = scale;
			Vector3 pos = targets[i].transform.position;
			pos.y = _minY + (_maxY - _minY)*relative/2;
			targets[i].transform.position = pos;
		}
	}
	
	void InitTargetScores() {
		for(int i = 0; i < scores.Length; ++i) {
			if (_logic.target[i] == 0) {
				scores[i].SetActive(false);
				targets[i].SetActive(false);
			} else {
				Vector3 scale = scores[i].transform.localScale;
				scale.z = (_maxY - _minY)/10;
				scores[i].transform.localScale = scale;
				
				Vector3 pos = scores[i].transform.position;
				pos.y = (_maxY + _minY)/2;
				scores[i].transform.position = pos;
				
				scores[i].renderer.material.color = MinimalColor.instance.Get((MColor)i);
			}			
		}
	}
}
