using UnityEngine;
using System.Collections;
using Radical;

public enum EdgeDirection { Horizontal, Vertical};

public class Edge : MonoBehaviour {
	public EdgeDirection direction;
	private SmoothVector3 _position;
	private bool _followingMouse;
	private SmoothVector3 _scale;
	private Vector3 _originalScale;
	private Logic _logic;
	public Edge[] edges;
	public Edge previous;
	public Edge next;
	public bool clickable;
	
	// Use this for initialization
	void Start () {
		// TO-DO fix this
		_logic = Logic.instance;
		
		_position = transform.position;
		_position.Duration = 0.3f;
		_position.Mode = SmoothingMode.slerp;
		
		_scale = transform.localScale;
		_scale.Duration = 0.3f;
		_scale.Mode = SmoothingMode.slerp;
		_originalScale = _scale;
		
		_followingMouse = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!_scale.IsComplete) transform.localScale = _scale.Value;
		if (!_position.IsComplete) transform.position = _position.Value;
		if (_followingMouse) {
			FollowMouse();
		}
	}
	
	void LateUpdate() {
		if (!clickable) return;
		if (Application.isPlaying) {
			ShapeUpdate();
		}
	}
	
	void OnMouseDown() {
		if (!clickable) return;
		if (_logic.SelectEdge(this)) {
			_followingMouse = true;
		}
	}
	
	void OnMouseEnter() {
		if (!clickable) return;
		if (!_followingMouse && !_logic.EdgeSelected()) {
			_originalScale = _scale.Target;
			Vector3 targetScale = _originalScale;
			targetScale.x *= 1.5f;
			_scale.Value = targetScale;
		}
	}
	
	public void ShapeUpdate() {
		if (edges.Length == 4) {
			Vector3 scale = _scale.Target;
			Vector3 position = transform.position;
			if (edges[0] != null) {
				scale.z = (edges[2].transform.position.z - edges[0].transform.position.z)/10;
				position.z = (edges[2].transform.position.z + edges[0].transform.position.z)/2;
			} else if (edges[1] != null) {
				scale.z = (edges[3].transform.position.x - edges[1].transform.position.x)/10;
				position.x = (edges[3].transform.position.x + edges[1].transform.position.x)/2;
			}
			if (position != transform.position) transform.position = position;
			if (scale != transform.localScale) transform.localScale = scale;
		}
	}
	
	void OnMouseExit() {
		if (!clickable) return;
		if (!_followingMouse && !_logic.EdgeSelected() ) {
			_scale.Value = _originalScale;
		}
	}
	
	void FollowMouse() {
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 target = transform.position;
		int margin = 1;
		int axis = (direction == EdgeDirection.Horizontal ? 2 : 0);
		
		if (position[axis] > previous.transform.position[axis] + margin && position[axis] < next.transform.position[axis] - margin)
			target[axis] = position[axis];
		else {
			if (position[axis] < previous.transform.position[axis] + margin) {
				target[axis] = 	previous.transform.position[axis] +  margin;
			} else {
				target[axis] = next.transform.position[axis] - margin;
			}
		}
		
		_position.Value = target;	
		if (Input.GetMouseButtonUp(0)) {
			_scale.Value = _originalScale;
			_logic.ReleaseEdge();
			_followingMouse = false;
		}
	}
}
