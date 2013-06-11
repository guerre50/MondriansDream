using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Radical;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameState {Loading, Playing, Pause, Result};

public class Logic : Singleton<Logic> {
	[HideInInspector]
	public Edge selectedEdge;
	private Figure[] _figures;
	public float[] target;
	[HideInInspector]
	public float[] _score;
	public GameState state;
	public Figure selectedFigure;
	public Figure selectedPreviewFigure;
	public Figure overFigure;
	public BlurEffect blur;
	public float introDuration;
	private SmoothFloat _blurEffect;
	private float _winTime;
	private Edge[] _horizontalEdges;
	private Edge[] _verticalEdges;
	private Edge[] _edges;
	private SortedDictionary<float, Edge> _sortedHorizontalEdges;
	private SortedDictionary<float, Edge> _sortedVerticalEdges;
	
	//=======================>
	// GAME STATES
	//
	
	void Awake () {
		selectedEdge = null;
		_figures = Object.FindObjectsOfType(typeof(Figure)) as Figure[];
		_edges = Object.FindObjectsOfType(typeof(Edge)) as Edge[];
		_score = new float[target.Length];
		if (blur) {
			blur.enabled = true;
			state = GameState.Loading;
			_blurEffect = blur.iterations;
			_blurEffect.Duration = introDuration;
			_blurEffect.Value = 0.0f;
		} else state = GameState.Playing;
		InitFigures();
		PlayingUpdate();
	}
	
	void LateUpdate() {
		
		switch(state) {
		case GameState.Loading:
			LoadingUpdate();
			break;
		case GameState.Playing:
			PlayingUpdate();
			break;
		case GameState.Result:
			Result();
			break;
		}
	}
	
	void LoadingUpdate() {
		blur.iterations = (int)_blurEffect.Value;
		if (_blurEffect.IsComplete) {
			blur.enabled = false;
			state = GameState.Playing;
		}
	}
	
	void PlayingUpdate() {
		// Sum of all the figure's colors
		_score = new float[target.Length];
		foreach(Figure figure in _figures) {
			_score[(int)figure.scoreColor] += figure.area;
		}
		
		// We check if we've won
		int score = target.Length;
		for(int i = 0; i < _score.Length; ++i) {
			if (_score[i] >= target[i]) score--;	
		}
		if (score == 0 && Application.loadedLevel != 0) {
			state = GameState.Result;
			_winTime = Time.time;
		}
		
		if (Input.GetMouseButtonDown(1)) {
			Application.LoadLevel(Application.loadedLevel);	
		}
		
		if (EdgeSelected()) {
			UpdateEdgeOrder();	
		}
	}
	
	void Result() {
		if (Time.time - _winTime > 2.0f) {
			_winTime = Mathf.Infinity;
			blur.enabled = true;
			_blurEffect.Value = 10.0f;
		} else if (_winTime == Mathf.Infinity) {
			blur.iterations = (int)_blurEffect.Value;
			if (_blurEffect.IsComplete) {
				if (Application.loadedLevel + 1 < Application.levelCount) {
					Application.LoadLevel(Application.loadedLevel+1);
				}
			}
		}
	}
	
	
	
	//=======================>
	// METHODS
	//
	
	public bool SelectEdge(Edge edge) {
		if (selectedEdge == null) {
			selectedEdge = edge;
			return true;
		}
		
		return false;
	}
	
	public bool EdgeSelected() {
		return selectedEdge != null;	
	}
	
	public void ReleaseEdge() {
		selectedEdge = null;	
	}
	
	
	
	public bool EnoughArea(MColor c) {
		if (!Application.isPlaying) {
			return true;
		} else {
			int index = (int)c;
			return _score[index] >= target[index];
		}
	}
	
	
	// TO-DO fix this
	public void InitFigures() {
		Figure[] figures = Object.FindObjectsOfType(typeof(Figure)) as Figure[];
		UpdateEdgeOrder();
		
		
		Vector3 min = Camera.main.ScreenToWorldPoint(Vector3.zero);
		
// Workaround to have the game view size in editor!
#if UNITY_EDITOR
		Vector2 windowSize = GetMainGameViewSize();
		Vector3 max = Camera.main.ScreenToWorldPoint(new Vector3(windowSize.x, windowSize.y,0));
#else
		Vector3 max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));
#endif
		Vector3 pos;
		
		// We move the edges to snap to the screen
		    // HORIZONTAL
		pos = _horizontalEdges[0].transform.position;
		pos.z = min.z;
		_horizontalEdges[0].transform.position = pos;
		pos = _horizontalEdges.Last().transform.position;
		pos.z = max.z;
		_horizontalEdges.Last().transform.position = pos;
			// VERTICAL
		pos = _verticalEdges[0].transform.position;
		pos.x = min.x;
		_verticalEdges[0].transform.position = pos;
		pos = _verticalEdges.Last().transform.position;
		pos.x = max.x;
		_verticalEdges.Last().transform.position = pos;
		
		foreach(Figure figure in figures) {
			KeyValuePair<float, Edge> prevEdge = new KeyValuePair<float, Edge>(0, null); 
			Vector3 figureMax = figure.plane.collider.bounds.max;
			Vector3 figureMin = figure.plane.collider.bounds.min;
			bool coversPosition = true;
			
			foreach( KeyValuePair<float, Edge> kvp in _sortedVerticalEdges )
	        {
	            if (prevEdge.Value != null) {
					Bounds edgeBound = kvp.Value.collider.bounds;
					coversPosition = (edgeBound.min.z - 1 < figureMin.z && figureMax.z < edgeBound.max.z + 1);
					
					if (prevEdge.Key - 1 < figureMin.x && kvp.Key + 1 > figureMax.x && coversPosition) {
						figure.edges[1] = prevEdge.Value;
						figure.edges[3] = kvp.Value;
						break;
					}
				} 
				
				if (coversPosition) prevEdge = kvp;
	        }	
			
			
			prevEdge = new KeyValuePair<float, Edge>(0, null); 
			coversPosition = true;
			foreach( KeyValuePair<float, Edge> kvp in _sortedHorizontalEdges )
	        {
	            if (prevEdge.Value != null) {
					
					Bounds edgeBound = kvp.Value.collider.bounds;
					coversPosition = (edgeBound.min.x - 1 < figureMin.x && figureMax.x < edgeBound.max.x + 1);
					
					if (prevEdge.Key - 1 < figureMin.z && kvp.Key + 1> figureMax.z && coversPosition) {
						figure.edges[0] = prevEdge.Value;
						figure.edges[2] = kvp.Value;
						break;
					}
				} 
				if (coversPosition) prevEdge = kvp;
	        }	
			
			figure.UpdateEditor();
		}
	}
	
	static Vector2 GetMainGameViewSize() {
	    System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
	    System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
	    System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
	    return (Vector2)Res;
	}
	
	// TO-DO optimize it using interval tree!
	void UpdateEdgeOrder() {
		_sortedHorizontalEdges = new SortedDictionary<float, Edge>();
		_sortedVerticalEdges = new SortedDictionary<float, Edge>();

		foreach(Edge edge in _edges) {
			if (edge.direction == EdgeDirection.Horizontal) {
				_sortedHorizontalEdges.Add (edge.transform.position.z, edge);
			} else {
				_sortedVerticalEdges.Add (edge.transform.position.x, edge);
			}
		}
		_horizontalEdges = _sortedHorizontalEdges.Values.ToArray();
		_verticalEdges = _sortedVerticalEdges.Values.ToArray();
		
		for(int i = 0; i < _horizontalEdges.Count(); ++i) {
			Edge e = _horizontalEdges[i];
			e.previous = Find(-1, _horizontalEdges, i, 0, 1.5f) as Edge;
			e.next = Find(1, _horizontalEdges, i, 0, 1.5f) as Edge;
		}
		
		for(int i = 0; i < _verticalEdges.Count(); ++i) {
			Edge e = _verticalEdges[i];
			e.previous = Find(-1, _verticalEdges, i, 2, 1.5f) as Edge;
			e.next = Find(1, _verticalEdges, i, 2, 1.5f) as Edge;
		}
	}
	
	bool Overlap(Bounds a, Bounds b, int axis, float margin) {
		if (a.min[axis] + margin > b.max[axis] || b.min[axis] + margin > a.max[axis]) return false;
		return true;
	}
	
	Component Find(int direction, Component[] collection, int element, int axis, float margin) {
		Bounds elementBounds = collection[element].transform.collider.bounds;
		bool print = collection[element].name == "Down1";
		element += direction;
		while (element >= 0 && element < collection.Length) {
			Bounds currentBounds = collection[element].transform.collider.bounds;
			
			if (Overlap(currentBounds, elementBounds, axis, margin)) {
				return collection[element];	
			}
			element += direction;
		}
		
		return null;
	}
			
}


