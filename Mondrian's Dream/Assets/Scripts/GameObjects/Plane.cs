using UnityEngine;
using System.Collections;
using Radical;

[ExecuteInEditMode]
public class Plane : MonoBehaviour {
	private SmoothVector3 _colorVector;
	private Logic _logic;
	public Figure figure;
	private MinimalColor _minimalColor;
	private Color _color;
	private Color _maxColor;
	private Color _minColor;
	private bool previewingColor = false;
	
	
	void Start () {
		_minimalColor = MinimalColor.instance;
		_logic = Logic.instance;
		
		UpdateMinMax();
		_colorVector = Utils.ToVector3(_color);
		_colorVector.Duration = 0.2f;
		_colorVector.Mode = SmoothingMode.slerp;
		
	}
	
	void Update () {
		switch (_logic.state) {
		case GameState.Playing:
			Playing();
			break;
		case GameState.Result:
			Result();
			break;
		
		}
	}
	
	void Playing() {
		if (Application.isPlaying) {
			Vector3 currentColor = _colorVector.Value;
			if (_colorVector.IsComplete && !figure.previewing) {
				UpdateColor();
			}
			
			if (!_colorVector.IsComplete) {
				renderer.sharedMaterial.SetColor("_Color", new Color(currentColor.x,currentColor.y, currentColor.z));
			}
		}	
	}
	
	void Result() {
		renderer.sharedMaterial.SetColor("_Color", Color.Lerp(_color,_maxColor, Mathf.PingPong(Time.time*5, 1.0f)));
	}
	
	
	void OnMouseEnter() {
		_logic.overFigure = figure;
		if (!_logic.EdgeSelected()) {
			HSBColor hsbColor = HSBColor.FromColor(GetColor());
			if (figure.previewing) {
				hsbColor.b = 1.0f;
				hsbColor.s = 1.0f;
				_logic.selectedPreviewFigure = figure;
			} else {
				hsbColor.s *= 0.5f;
			}
			_colorVector.Value = Utils.ToVector3(hsbColor.ToColor());
		}
	}
	
	void OnMouseExit() {
		if (_logic.overFigure == figure) {
			_logic.overFigure = null;	
		}
		_colorVector.Value = Utils.ToVector3(GetColor());
		if (_logic.selectedPreviewFigure == figure) {
			_logic.selectedPreviewFigure = null;
		}
	}
	
	void OnMouseUp() {
		if (previewingColor) {
			previewingColor = false;
			_logic.selectedFigure = null;
			if (_logic.selectedPreviewFigure != null) {
				_logic.selectedPreviewFigure.FixColor();
			}
			foreach(Figure fig in figure.adjacentFigures) {
				fig.StopColorCombination();
			}
		}
	}
	
	public void UpdateMinMax() {
		_color = _minimalColor.Get(figure.color);
		HSBColor hsbColor = HSBColor.FromColor(_color);
		HSBColor minColor = hsbColor, maxColor = hsbColor;
		minColor.b *= 0.2f;
		if (figure.color != MColor.Black) {
			maxColor.s = 1.0f;
			maxColor.b = 1.0f;
		}
		_minColor = minColor.ToColor();
		_maxColor = maxColor.ToColor();
	}
	
	void OnMouseDown() {
		previewingColor = true;
		_logic.selectedFigure = figure;
		foreach(Figure fig in figure.adjacentFigures) {
			fig.PreviewColorCombination(figure.color);
		}
	}
	
	public void UpdateColor() {
		Vector3 newColor = Utils.ToVector3(GetColor());
		if (newColor != _colorVector.Current) _colorVector.Value = newColor;	
	}
	
	Color GetColor() {
		if (figure.previewing) {
			Color c = _minimalColor.Get(figure.color);
			HSBColor hsbC = HSBColor.FromColor(c);
			hsbC.b = 1.0f;
			return hsbC.ToColor();	
		} else if(!_logic.EnoughArea(figure.scoreColor)) {
			return _color;
		} else {
			return _maxColor;
		}	
	}
}
