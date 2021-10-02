using System.Collections.Generic;

class Grid : IGrid {
    private IAutomaton automaton;

    private Dictionary<(int x, int y), State> grid = new Dictionary<(int x, int y), State>();
    private Dictionary<(int x, int y), State> initialGrid;

    private List<IGridContainer> containers = new List<IGridContainer>();

    private Dictionary<(int x, int y), IGridContainer> containerMapping =
        new Dictionary<(int x, int y), IGridContainer>();

    private List<IPort> ports = new List<IPort>();

    public Grid(int width, int height) {
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

    public IEnumerable<IPort> GetPorts() {
        return ports;
    }

    public void AddPort(IPort port) {
        ports.Add(port);
    }

    /// <summary>
    /// Adds the container, also calculating its mapping.
    /// </summary>
    public void InsertContainer(IGridContainer container) {
        containers.Add(container);
        
        // TODO: mapping
    }

    public IGridContainer GetContainerAt(int x, int y) =>
        containerMapping.TryGetValue((x, y), out IGridContainer container) ? container : null;

    /// <summary>
    /// Removes the container, also removing it from the mapping.
    /// </summary>
    public void RemoveContainerAt(IGridContainer container) {
        containers.Remove(container);
        
        // TODO: mapping
    }

    public List<IGridContainer> GetContainers() {
        return containers;
    }

    public void DoIteration() {
        // when starting the simulation, save the initial grid
        initialGrid ??= grid;

        // TODO: iteration in the grid itself

        foreach (var container in GetContainers())
            container.Grid.DoIteration();
    }

    public void DoSwap() {
        // TODO: swap in the grid itself

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