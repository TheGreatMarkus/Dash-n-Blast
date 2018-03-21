using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;

	void Update () {
        //For now, the camera will just simply follow the player.
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
	}
}
