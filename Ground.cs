using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void OnMouseEnter()
    {
        PlayerInteractor.doMouseEnterObject(gameObject);
    }

    public void OnMouseExit()
    {
        PlayerInteractor.doMouseExitObject(gameObject);
    }
}
