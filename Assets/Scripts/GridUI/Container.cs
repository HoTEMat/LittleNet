using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Container : MonoBehaviour
{
    public event Action<Container> OnClicked;
    public event Action<Container> OnDragged;
    public GridContainer GridContainer { get; set; }

    public void SetTopLeft(Vector3 topLeft) {
        Vector3 scale = transform.localScale;
        (float w, float h) = (scale.x, scale.y);
        transform.position = new Vector3(topLeft.x + w / 2, topLeft.y - h / 2, topLeft.z);
    }

    private void OnMouseDown() {
        OnClicked?.Invoke(this);
    }

    private void OnMouseDrag() {
        OnDragged?.Invoke(this);
    }

    private void OnMouseUp() {
        Debug.Log("up");
    }
}
