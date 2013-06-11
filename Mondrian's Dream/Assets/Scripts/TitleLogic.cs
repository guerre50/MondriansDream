using UnityEngine;
using System.Collections;
using Radical;

public enum TitleState { Title, TitleTransition, LevelSelect, LoadLevel};

public class TitleLogic : Singleton<TitleLogic> {
	public TitleState state;
	public GameObject guiTitle;
	public GameObject guiLevelSelect;
	public BlurEffect blur;
	public float blurDuration;
	private SmoothFloat _blurStrength;
	private Logic _logic;
	public AnimatedText infoBox;
	public string[] levelMessages;
	private int loadLevel;
	
	// Use this for initialization
	void Start () {
		state = TitleState.Title;
		_blurStrength = blur.iterations;
		_blurStrength.Duration = blurDuration;
		_blurStrength.Mode = SmoothingMode.slerp;
		_logic = Logic.instance;
	}
	
	// Update is called once per frame
	void Update () {
		switch(state) {
		case TitleState.Title:
			TitleUpdate();
			break;
		case TitleState.TitleTransition:
			TitleTransition();
			break;
		case TitleState.LevelSelect:
			LevelSelectUpdate();
			break;
		case TitleState.LoadLevel:
			LoadLevelUpdate();
			break;
		}
	}
	
	void TitleUpdate() {
		if (Time.timeSinceLevelLoad > 2 && Input.anyKey) {
			state = TitleState.TitleTransition;
			_blurStrength.Value = 0.0f;
			guiTitle.SetActive(false);
		}
	}
	
	void TitleTransition() {
		blur.iterations = (int)_blurStrength.Value;
		if (_blurStrength.IsComplete) {
			blur.enabled = false;
			state = TitleState.LevelSelect;
			_logic.state = GameState.Playing;
			guiLevelSelect.SetActive(true);
			infoBox.Text = "Select a level";
		}	
	}
	
	void LevelSelectUpdate() {
		
		if (_logic.overFigure != null) {
			infoBox.Text = levelMessages[int.Parse(_logic.overFigure.name)];
		}
		
		if (_logic.selectedFigure != null) {
			loadLevel = int.Parse(_logic.selectedFigure.name);	
			state = TitleState.LoadLevel;
			blur.enabled = true;
			_blurStrength.Value = 12;
		}
	}
	
	void LoadLevelUpdate() {
		blur.iterations = (int)_blurStrength.Value;
		if (_blurStrength.IsComplete) {
			Application.LoadLevel(loadLevel);	
		}
	}
}
