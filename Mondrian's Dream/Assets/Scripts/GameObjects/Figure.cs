using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Radical;

[ExecuteInEditMode]
public class Figure : MonoBehaviour {
	public Edge[] edges;
	private Vector2 _min, _max;
	private Bounds _bounds;
	public Plane plane;
	public MColor initialColor;
	private MColor _color;
	private MColor _originalColor;
	public bool previewing;
	public MColor color {
		get { return _color;}
		set { 
			_color = value;

			if (!Application.isPlaying) {
				if (!plane.renderer.enabled) plane.renderer.enabled = true;
			} else {
				if (_color == MColor.White) {
					if (plane.renderer.enabled) plane.renderer.enabled = false;	
				} else if (!plane.renderer.enabled) {
					plane.renderer.enabled = true;
				}
			}
		}
	}
	
	public MColor scoreColor {
		get {return _originalColor;}	
	}
	
	
	public List<Figure> adjacentFigures = new List<Figure>();
	public float area;
	
	void Awake() {
		color = initialColor;
		_originalColor = _color;
		previewing = false;
	}
	
	void Update() {
		if (!Application.isPlaying) {
			color = initialColor;
			plane.renderer.sharedMaterial.SetColor("_Color", MinimalColor.instance.Get (color));
		}
	}
	void LateUpdate() {
		if (!Application.isPlaying) return;
		ShapeUpdate();	
	}
	
	public void ShapeUpdate() {
		Utils.LineIntersection(edges[0].collider.bounds,edges[1].collider.bounds, ref _min);
		Utils.LineIntersection(edges[2].collider.bounds,edges[3].collider.bounds, ref _max);
		
		Vector2 diff = _max - _min;
		Vector2 position = (_max + _min)/2;
		area = diff.x*diff.y;
		
		Vector3 newPosition = new Vector3(position.x, 0, position.y);
		if (transform.position != newPosition) transform.position = newPosition;
		Vector3 newScale = new Vector3(diff.x, 0, diff.y);
		if (transform.localScale != newScale) transform.localScale = newScale;
		
		if (!Application.isPlaying) {
			color = initialColor;
			plane.renderer.material.SetColor("_Color", MinimalColor.instance.Get (color));
		}
	}
	
	public void PreviewColorCombination(MColor color) {
		_originalColor = _color;
		previewing = true;
		
		this.color = MinimalColor.instance.Combine(color, _color);
		plane.UpdateColor();
	}
	
	public void StopColorCombination() {
		previewing = false;
		color = _originalColor;
	}
	
	public void UpdateEditor() {
		ShapeUpdate();
	}
	
	public void FixColor() {
		_originalColor = _color;
		plane.UpdateMinMax();
		plane.UpdateColor();
	}
	
	public void AddAdjacent(Figure fig) {
		if (fig != this && !adjacentFigures.Contains(fig)) {
			adjacentFigures.Add(fig);	
		}
	}
	
	public void RemoveAdjacent(Figure fig) {
		if (fig != this && !adjacentFigures.Contains(fig)) {
			adjacentFigures.Remove(fig);
		}
	}
}
