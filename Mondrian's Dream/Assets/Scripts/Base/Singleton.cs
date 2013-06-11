using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	private static T _Instance = null;
	
	public static T instance {
        get {
            if (_Instance == null) {
				_Instance = FindObjectOfType(typeof (T)) as T;
				
	            if (_Instance == null) {
	                GameObject obj = new GameObject("Singleton");
	                _Instance = obj.AddComponent(typeof (T)) as T;
	            }
			}
 
            return _Instance;
        }
    }
	
	public virtual void Init(){}
	
	private void Awake() {
		if (_Instance == null) {
			_Instance = this as T;
			_Instance.Init();
		}
	}
	
    void OnApplicationQuit() {
        _Instance = null;
    }
}