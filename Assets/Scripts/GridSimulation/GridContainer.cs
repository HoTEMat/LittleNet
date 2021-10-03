using System;
using System.Collections.Generic;
using System.Linq;

class GridContainer : ICloneable<GridContainer> {


    public int X { get; set; }
    public int Y { get; set; }

    public int OuterWidth { get; private set; }
    public int OuterHeight { get; private set; }

    public IReadOnlyDictionary<(int x, int y), IPort> RelativePortPlacement => relativePortPlacement;

    public Rotation Rotation { get; set; }
    public SimulationGrid Grid { get; }

    private Dictionary<(int x, int y), IPort> relativePortPlacement = new Dictionary<(int x, int y), IPort>();

    public GridContainer Clone() {
        var clone = new GridContainer(X, Y, Grid.Clone(), Rotation) {
            relativePortPlacement = relativePortPlacement
        };

        return clone;
    }

    public GridContainer(int x, int y, SimulationGrid grid, Rotation rotation) {
        this.Grid = grid;
        Rotation = rotation;

        X = x;
        Y = y;
    }

    private void PlacePorts() {
        var ports = Grid.GetPorts().Where(p => p is Port).Cast<Port>();

        // whether there are ports in the corners
        Port tl = null;
        Port tr = null;
        Port bl = null;
        Port br = null;

        // the ports on each of the sides
        var left = new List<Port>();
        var right = new List<Port>();
        var top = new List<Port>();
        var bottom = new List<Port>();

        // Yuck!
        foreach (Port port in ports) {
            bool isLeft = port.InnerX == 0,
                 isRight = port.InnerX == Grid.Width - 1,
                 isTop = port.InnerY == 0,
                 isBottom = port.InnerY == Grid.Height - 1;

            if (isTop && isLeft) { tl = port; continue; }
            if (isTop && isRight) { tr = port; continue; }
            if (isBottom && isRight) { br = port; continue; }
            if (isBottom && isLeft) { bl = port; continue; }

            if (isLeft) left.Add(port);
            if (isRight) right.Add(port);
            if (isTop) top.Add(port);
            if (isBottom) bottom.Add(port);
        }

        int topCornerPorts = (tl != null ? 1 : 0) + (tr != null ? 1 : 0);
        int bottomCornerPorts = (bl != null ? 1 : 0) + (br != null ? 1 : 0);
        int leftCornerPorts = (tl != null ? 1 : 0) + (bl != null ? 1 : 0);
        int rightCornerPorts = (tr != null ? 1 : 0) + (br != null ? 1 : 0);

        int widthTop = top.Count * 2 + 1 + topCornerPorts;
        int widthBottom = bottom.Count * 2 + 1 + bottomCornerPorts;
        OuterWidth = new[] { widthTop, widthBottom, 3 }.Max();

        int heightLeft = left.Count * 2 + 1 + leftCornerPorts;
        int heightRight = right.Count * 2 + 1 + rightCornerPorts;
        OuterHeight = new[] { heightLeft, heightRight, 3 }.Max();

        // place corner ports
        int W = OuterWidth - 1, H = OuterHeight - 1;
        if (tl != null) PlacePort(0, 0, tl);
        if (tr != null) PlacePort(W, 0, tl);
        if (bl != null) PlacePort(0, H, tl);
        if (br != null) PlacePort(W, H, tl);

        // place top ports
        int topOffset = (OuterWidth - top.Count * 2 - 1) / 2;
        for (int x = 0; x < top.Count; x++) {
            PlacePort(topOffset + 2 * x, 0, top[x]);
        }

        // place bottom ports
        int bottomOffset = (OuterWidth - bottom.Count * 2 - 1) / 2;
        for (int x = 0; x < bottom.Count; x++) {
            PlacePort(bottomOffset + 2 * x, H, bottom[x]);
        }

        // place left ports
        int leftOffset = (OuterHeight - left.Count * 2 - 1) / 2;
        for (int y = 0; y < left.Count; y++) {
            PlacePort(0, leftOffset + y * 2, left[y]);
        }

        // place right ports
        int rightOffset = (OuterHeight - right.Count * 2 - 1) / 2;
        for (int y = 0; y < right.Count; y++) {
            PlacePort(0, rightOffset + y * 2, right[y]);
        }
    }

    private void PlacePort(int x, int y, Port port) {
        port.OuterX = x;
        port.OuterY = y;
        PlacePort(port);
    }
    private void PlacePort(Port port) {
        relativePortPlacement.Add((port.OuterX, port.OuterY), port);
    }
}