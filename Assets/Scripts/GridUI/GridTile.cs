using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
class GridTile : MonoBehaviour {
    [SerializeField] private Sprite WhiteSprite;
    [SerializeField] private GameObject PortBackgroundPrefab;
    private SpriteRenderer spriteRenderer;

    public event Action<GridTile> OnClicked;
    public event Action<GridTile> OnMouseInside;
    public int X { get; set; }
    public int Y { get; set; }

    public static readonly IReadOnlyDictionary<State, Color> stateToColor = new Dictionary<State, Color> {
        { State.Nothing, Color.black },
        { State.WireOn, Color.red },
        { State.WireDead, Color.gray },
        { State.WireOff, new Color(.5f,0,0) },
        { State.LampOn, new Color(1,1,.5f) },
        { State.LampDead, new Color(.5f, .5f, .2f) },
        { State.LampOff, new Color(.5f, .5f, 0) },
        { State.NotOn, new Color(0,1,0) },
        { State.NotDead, new Color(.3f, .7f, .3f) },
        { State.NotOff, new Color(0,.5f,0) },
        { State.CrossHOnVOn, new Color(1,0,1) },
        { State.CrossHOnVOff, new Color(1,0,1) },
        { State.CrossHOffVOn, new Color(1,0,1) },
        { State.CrossHOffVOff, new Color(1,0,1) },
        { State.CrossHDeadVOn, new Color(1,0,1) },
        { State.CrossHOnVDead, new Color(1,0,1) },
        { State.CrossHDeadVDead, new Color(1,0,1) },
        { State.CrossHDeadVOff, new Color(1,0,1) },
        { State.CrossHOffVDead, new Color(1,0,1) },
    };

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTopLeft(Vector3 topLeft) {
        Vector3 scale = transform.lossyScale;
        (float w, float h) = (scale.x, scale.y);
        transform.localPosition = new Vector3(topLeft.x + w / 2, topLeft.y - h / 2, topLeft.z);
    }

    public void ShowColor(State state) {
        spriteRenderer.sprite = WhiteSprite;
        spriteRenderer.color = stateToColor[state];
    }

    public void ShowSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.white;
    }

    public void MarkPort(IPort port) {
        if (port == null)
            return;

        var backgroundObj = Instantiate(PortBackgroundPrefab, transform);
        var rt = backgroundObj.GetComponent<RectTransform>();

        rt.transform.localPosition = new Vector3(0, 0, 1);

        float c = 1.06f;
        rt.localScale = new Vector3(c, c, c);

        if (port is InputPort) backgroundObj.GetComponent<SpriteRenderer>().color = Color.white;
        else if (port is OutputPort) backgroundObj.GetComponent<SpriteRenderer>().color = Color.white;
        else backgroundObj.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnMouseDown() {
        OnClicked?.Invoke(this);
    }

    private void OnMouseOver() {
        OnMouseInside?.Invoke(this);
    }
}
