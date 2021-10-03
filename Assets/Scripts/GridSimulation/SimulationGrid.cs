using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class SimulationGrid {
    private IAutomaton automaton = new WireeAutomaton();

    public bool CanSimulate { get; private set; } = false;
    public GatePrototype Prototype { get; }

    private Dictionary<(int x, int y), State> grid = new Dictionary<(int x, int y), State>();
    private Dictionary<(int x, int y), State> newGrid;

    private List<GridContainer> containers = new List<GridContainer>();

    private Dictionary<(int x, int y), GridContainer> containerMapping =
        new Dictionary<(int x, int y), GridContainer>();

    private Dictionary<(int x, int y), IPort> ports = new Dictionary<(int x, int y), IPort>();

    /// <summary>
    /// Create a simulation grid with an existing prototype
    /// </summary>
    public SimulationGrid(int width, int height, GatePrototype prototype) {
        Width = width;
        Height = height;
        Prototype = prototype;
    }

    /// <summary>
    /// Create a simulation grid, and a new unique prototype.
    /// </summary>
    public SimulationGrid(int width, int height, string prototypeName) {
        Width = width;
        Height = height;
        Prototype = new GatePrototype(this, prototypeName);
    }

    /// <summary>
    /// Either returns the value from the dictionary, or <see cref="State.Nothing"/> if it's not found.
    /// </summary>
    public State Get(int x, int y)
        => grid.TryGetValue((x, y), out State state) ? state : State.Nothing;

    /// <summary>
    /// Simply sets the dictionary key (if there is no container!).
    /// </summary>
    public void Set(int x, int y, State state) {
        if (GetContainerAt(x, y) == null)
            grid[(x, y)] = state;
        else
            throw new InvalidOperationException();
    }

    public IEnumerable<IPort> GetPorts() => ports.Values;

    public IPort GetPortAt(int x, int y) => ports.TryGetValue((x, y), out IPort port) ? port : null;

    public void AddPort(IPort port) {
        ports[(port.InnerX, port.InnerY)] = port;
        Set(port.InnerX, port.InnerY, State.WireOff);
    }

    public void AddPort(int innerX, int innerY) {
        Port p = new Port(this, innerX, innerY);
        AddPort(p);
    }

    /// <summary>
    /// Adds the container, also calculating its mapping.
    /// </summary>
    public void InsertContainer(GridContainer container) {
        containers.Add(container);

        // TODO: mapping rotation in the future
        for (int x = 0; x < container.OuterWidth; x++)
            for (int y = 0; y < container.OuterHeight; y++)
                containerMapping[(container.X + x, container.Y + y)] = container;
    }

    public GridContainer GetContainerAt(int x, int y) =>
        containerMapping.TryGetValue((x, y), out GridContainer container) ? container : null;

    /// <summary>
    /// Removes the container, also removing it from the mapping.
    /// </summary>
    public void RemoveContainer(GridContainer container) {
        containers.Remove(container);

        // TODO: mapping rotation in the future
        for (int x = 0; x < container.OuterWidth; x++)
            for (int y = 0; y < container.OuterHeight; y++)
                containerMapping.Remove((container.X + x, container.Y + y));
    }

    public List<GridContainer> GetContainers() {
        return containers;
    }

    private bool ValidBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public void DoIteration() {
        if (!CanSimulate) throw new InvalidOperationException();

        newGrid = new Dictionary<(int x, int y), State>();

        foreach (var position in grid.Keys.Concat(ports.Keys)) {
            (int x, int y) = position;

            (int, int)[] interestingTilesDeltas = { (0, 1), (1, 0), (-1, 0), (0, -1), (0, 0) };
            (int, int)[] neighbourDeltas = { (0, -1), (0, 1), (-1, 0), (1, 0) }; // TODO: constant

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

                        State s = automaton.NextState(up, down, left, right, p.GetState());

                        if (s != State.Nothing)
                            newGrid[(nx, ny)] = s;
                    }

                    if (port is OutputPort op) {
                        var neighbours = op.GetNeighbors();

                        State up = neighbours[0];
                        State down = neighbours[1];
                        State left = neighbours[2];
                        State right = neighbours[3];

                        State s = automaton.NextState(up, down, left, right, op.GetState());

                        if (s != State.Nothing)
                            newGrid[(nx, ny)] = s;
                    }

                    // just copy
                    if (port is InputPort ip) {
                        State s = ip.GetState();
                        
                        if (s != State.Nothing)
                            newGrid[(nx, ny)] = s;
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

    public SimulationGrid Clone(GridContainer newContainer = null) {
        var clone = new SimulationGrid(Width, Height, Prototype) {
            automaton = automaton,
            CanSimulate = true,
            containerMapping = containerMapping.Select(kv => kv.Value.Clone()).ToDictionary(v => (v.X, v.Y)),
            grid = grid.ToDictionary(kv => kv.Key, kv => kv.Value),
        };

        clone.ports = ports.ToDictionary(kv => kv.Key, kv => kv.Value.Clone(clone));
        clone.containers = clone.containerMapping.Select(kv => kv.Value).ToList();
        return clone;
    }

    public int Width { get; }
    public int Height { get; }
}