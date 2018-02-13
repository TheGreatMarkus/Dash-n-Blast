using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfoController : MonoBehaviour {

   private PlayerController pc;
    public Text text;

	// Use this for initialization
	void Start () {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = pc.GetDebugText();

    }
}
