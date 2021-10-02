using System.Collections.Generic;

class SimulationGrid : IGrid {
    private IAutomaton automaton = new WireeAutomaton();

    private Dictionary<(int x, int y), State> grid = new Dictionary<(int x, int y), State>();
    private Dictionary<(int x, int y), State> initialGrid;
    private Dictionary<(int x, int y), State> newGrid;

    private List<IGridContainer> containers = new List<IGridContainer>();

    private Dictionary<(int x, int y), IGridContainer> containerMapping =
        new Dictionary<(int x, int y), IGridContainer>();

    private Dictionary<(int x, int y), IPort> ports = new Dictionary<(int x, int y), IPort>();

    public SimulationGrid(int width, int height) {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Either returns the value from the dictionary, or nothing if it's not found.
    /// </summary>
    public State Get(int x, int y)
        => grid.TryGetValue((x, y), out State state) ? state : State.Nothing;

    /// <summary>
    /// Simply sets the dictionary key (if there is no container!).
    /// </summary>
    public void Set(int x, int y, State state) {
        if (GetContainerAt(x, y) == null)
            grid[(x, y)] = state;
    }

    public IEnumerable<IPort> GetPorts() => ports.Values;

    public void AddPort(IPort port) {
        ports[(port.InnerX, port.InnerY)] = port;
    }

    /// <summary>
    /// Adds the container, also calculating its mapping.
    /// </summary>
    public void InsertContainer(IGridContainer container) {
        containers.Add(container);

        // TODO: mapping rotation in the future
        for (int x = 0; x < container.OuterWidth; x++)
        for (int y = 0; y < container.OuterHeight; y++)
            containerMapping[(container.X + x, container.Y + y)] = container;
    }

    public IGridContainer GetContainerAt(int x, int y) =>
        containerMapping.TryGetValue((x, y), out IGridContainer container) ? container : null;

    /// <summary>
    /// Removes the container, also removing it from the mapping.
    /// </summary>
    public void RemoveContainerAt(IGridContainer container) {
        containers.Remove(container);

        // TODO: mapping rotation in the future
        for (int x = 0; x < container.OuterWidth; x++)
        for (int y = 0; y < container.OuterHeight; y++)
            containerMapping.Remove((container.X + x, container.Y + y));
    }

    public List<IGridContainer> GetContainers() {
        return containers;
    }

    private bool ValidBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public void DoIteration() {
        // when starting the simulation, save the initial grid
        initialGrid ??= grid;

        newGrid = new Dictionary<(int x, int y), State>();

        foreach (var position in grid) {
            (int x, int y) = position.Key;

            (int, int)[] interestingTilesDeltas = {(0, 1), (1, 0), (-1, 0), (0, -1), (0, 0)};
            (int, int)[] neighbourDeltas = {(0, -1), (0, 1), (-1, 0), (1, 0)}; // TODO: constant

            // also do each neighbour, because they could have changed
            foreach ((int dx, int dy) in interestingTilesDeltas) {
                int nx = x + dx, ny = y + dy;

                if (!ValidBounds(nx, ny))
                    continue;

                // if it's a port, it will calculate its state
                if (ports.TryGetValue((nx, ny), out IPort port)) {
                    if (port is Port p) {
                        var neighbours = p.GetNeighbors(this);

                        State up = neighbours[0];
                        State down = neighbours[1];
                        State left = neighbours[2];
                        State right = neighbours[3];

                        newGrid[(nx, ny)] = automaton.NextState(up, down, left, right, p.GetState());
                    }

                    if (port is OutputPort op) {
                        var neighbours = op.GetNeighbors();

                        State up = neighbours[0];
                        State down = neighbours[1];
                        State left = neighbours[2];
                        State right = neighbours[3];

                        newGrid[(nx, ny)] = automaton.NextState(up, down, left, right, op.GetState());
                    }

                    // just copy
                    if (port is InputPort ip) {
                        newGrid[(nx, ny)] = ip.GetState();
                    }
                }

                // else find the neighbours
                else {
                    State[] neighbours = new State[4];
                    int i = 0;
                    foreach ((int ndx, int ndy) in neighbourDeltas) {
                        int nnx = nx + ndx, nny = ny + ndy;

                        if (ValidBounds(nnx, nny))
                            neighbours[i++] = Get(nnx, nny);
                    }

                    newGrid[(nx, ny)] = automaton.NextState(
                        neighbours[0],
                        neighbours[1],
                        neighbours[2],
                        neighbours[3],
                        Get(nx, ny)
                    );
                }
            }
        }

        foreach (var container in GetContainers())
            container.Grid.DoIteration();
    }

    public void DoSwap() {
        grid = newGrid;

        foreach (var container in GetContainers())
            container.Grid.DoSwap();
    }

    public void Reset() {
        grid = initialGrid;
        initialGrid = null;

        foreach (var container in GetContainers())
            container.Grid.Reset();
    }

    public int Width { get; }
    public int Height { get; }
}