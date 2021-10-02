using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GridTile : MonoBehaviour { 
    public void SetTopLeft(Vector3 topLeft) {
        Vector3 scale = transform.localScale;
        (float w, float h) = (scale.x, scale.y);
        transform.position = new Vector3(topLeft.x + w / 2, topLeft.y + h / 2, topLeft.z);
    }
}
