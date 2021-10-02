using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

class GridHolder : MonoBehaviour {
    public GridTile GridTilePrefab;
    public GameObject GridLinePrefab;
    public ToolPicker ToolPicker;

    [SerializeField]
    private float gridLineWidth = 0.01f;
    private IGrid grid { get; set; }
    private GridTile[,] gridTiles;

    private static readonly IReadOnlyDictionary<State, Color> stateToColor = new Dictionary<State, Color> {
        { State.Nothing, Color.black },
        { State.WireOff, Color.red }
    };

    private void Start() {
        SetGrid(new TestGrid());
    }

    private void Update() {
        UpdateGridTiles();
    }

    private void SetGrid(IGrid grid) {
        this.grid = grid;
        gridTiles = new GridTile[grid.Width, grid.Height];
        InitGridTiles();
    }

    private void InitGridTiles() {
        var clickHandler = new Action<GridTile>(HandleTileClicked);
        for (int gridX = 0; gridX < grid.Width; gridX++) {
            for (int gridY = 0; gridY < grid.Height; gridY++) {
                GridTile tile = Instantiate<GridTile>(GridTilePrefab, transform);
                tile.X = gridX;
                tile.Y = gridY;
                tile.SetTopLeft(GridPositionToPosition(gridX, gridY, 0));
                tile.OnClicked += clickHandler;
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
        if (ToolPicker.CurrentTool == UITool.PlaceWire) {
            grid.Set(tile.X, tile.Y, State.WireOff);
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
