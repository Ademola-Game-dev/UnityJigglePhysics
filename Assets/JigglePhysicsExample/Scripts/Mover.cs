using System;
using UnityEngine;

public class Mover : MonoBehaviour {
    private float time;
    private Vector3 startPosition;

    private void Start() {
        startPosition = transform.position;
    }

    void Update() {
        time += Mathf.Min(Time.deltaTime,Time.fixedDeltaTime);
        transform.position = startPosition + Vector3.forward*Mathf.Sin(time*2f);
    }
}
