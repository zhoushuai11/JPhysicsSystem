using System;
using UnityEngine;

public class TestCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Debug.Log($"{gameObject.name} TriggerEnter {other.gameObject.name}");
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log($"{gameObject.name} ColliderEnter {other.gameObject.name}");
    }
}