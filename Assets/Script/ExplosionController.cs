using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 30);
		Invoke ("DisableCollider", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DisableCollider()
	{
		gameObject.GetComponent<CircleCollider2D> ().enabled = false;
	}


}
