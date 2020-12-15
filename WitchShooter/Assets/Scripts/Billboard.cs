using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	void Update () {
        transform.LookAt(Camera.main.transform);

        float distToCam = Vector3.Distance(transform.position, Camera.main.transform.position);

        transform.localScale = new Vector3(-1, 1, 1) * .005f * distToCam;
	}
}
