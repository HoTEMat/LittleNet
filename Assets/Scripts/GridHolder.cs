using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

class GridHolder : MonoBehaviour {
    public GridTile GridTilePrefab;
    public GameObject GridLinePrefab;
    public ToolPicker ToolPicker;
    public EventSystem EventSystem;

    [SerializeField]
    private float gridLineWidth = 0.01f;
    private IGrid grid { get; set; }
    private GridTile[,] gridTiles;
    private UITool activeTool;

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
        SetGrid(new SimulationGrid(30, 30));
    }

    private void Update() {
        UpdateGridTiles();

        if (Input.GetKeyDown(KeyCode.Space)) {
            grid.DoIteration();
            grid.DoSwap();
        }
        if (!Input.GetMouseButton(0)) {
            activeTool = null;
        }
    }

    private void SetGrid(IGrid grid) {
        this.grid = grid;
        gridTiles = new GridTile[grid.Width, grid.Height];
        InitGridTiles();
    }

    private void InitGridTiles() {
        var clickHandler = new Action<GridTile>(HandleTileClicked);
        var mouseOverTileHandler = new Action<GridTile>(HandleMouseOverTile);
        for (int gridX = 0; gridX < grid.Width; gridX++) {
            for (int gridY = 0; gridY < grid.Height; gridY++) {
                GridTile tile = Instantiate<GridTile>(GridTilePrefab, transform);
                tile.X = gridX;
                tile.Y = gridY;
                tile.SetTopLeft(GridPositionToPosition(gridX, gridY, 0));
                tile.OnClicked += clickHandler;
                tile.OnMouseInside += mouseOverTileHandler;
                tile.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                gridTiles[gridX, gridY] = tile;
            }
        }
    }

    private void UpdateGridTiles() {
        for (int gridX = 0; gridX < grid.Width; gridX++) {
            for (int gridY = 0; gridY < grid.Height; gridY++) {
                State state = grid.Get(gridX, gridY);
                Color color = stateToColor[state];
                GridTile tile = gridTiles[gridX, gridY];

                tile.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
            }
        }
    }

    private void HandleTileClicked(GridTile tile) {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (ToolPicker.CurrentTool == null)
            return;
        activeTool = ToolPicker.CurrentTool;
        ApplyTool(activeTool, tile);
    }
    private void HandleMouseOverTile(GridTile tile) {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (activeTool != null) {
            ApplyTool(activeTool, tile);
        }
    }

    private void ApplyTool(UITool tool, GridTile tile) {
        if (ToolPicker.CurrentTool is PlaceTileTool placeTile) {
            grid.Set(tile.X, tile.Y, placeTile.TileType);
        }
    }

    private Vector3 GridPositionToPosition(int gridX, int gridY, float z) {
        (float w, float h) = GetTileSize();
        return new Vector3((w + gridLineWidth) * gridX, -(h + gridLineWidth) * gridY, z);
    }


    public (float width, float height) GetTileSize() {
        Vector3 tileScale = GridTilePrefab.transform.localScale;
        return (tileScale.x, tileScale.y);
    }

    public (float, float) GetGridSize() {
        (float tileWidth, float tileHeight) = GetTileSize();
        return (grid.Width * (tileWidth + gridLineWidth), grid.Height * (tileHeight + gridLineWidth));
    }

    public Vector3 GetGridCenter() {
        (float w, float h) = GetGridSize();
        return new Vector3(w / 2, -h / 2, 0);
    }
}
