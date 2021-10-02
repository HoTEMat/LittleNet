class OutputPort : IPort {
    private SimulationGrid Grid;

    public State[] GetNeighbors() {
        int i = 0;
        int gridWidth = Grid.Width;
        int gridHeight = Grid.Height;

        State[] neighbors = new State[4];

        // walk neighbors
        (int x, int y)[] offsets = {(0, -1), (0, 1), (-1, 0), (1, 0)};
        foreach (var offset in offsets) {
            int x = InnerX + offset.x;
            int y = InnerY + offset.y;

            bool isInside = x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;

            if (isInside) {
                neighbors[i] = Grid.Get(x, y);
            }
            i++;
        }

        return neighbors;
    }

    public OutputPort(SimulationGrid grid, int x, int y) {
        Grid = grid;

        InnerX = x;
        InnerY = y;
    }

    public State GetState() {
        return Grid.Get(InnerX, InnerY);
    }

    public IPort Clone(SimulationGrid newGrid) {
        return new OutputPort(newGrid, InnerX, InnerY);
    }

    public int InnerX { get; }
    public int InnerY { get; }
}