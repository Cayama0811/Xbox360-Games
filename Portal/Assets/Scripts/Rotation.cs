using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

    // this is for the cake on the main menu, it should use Time.deltaTime but who genuinely cares
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 1, 0);
	}
}
