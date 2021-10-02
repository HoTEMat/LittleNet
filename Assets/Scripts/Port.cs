class InputPort : IPort {
    public InputPort(int x, int y, ILevelValidator validator, int portPosition) {
        Validator = validator;
        PortPosition = portPosition;
        InnerX = x;
        InnerY = y;
    }

    public int InnerX { get; }
    public int InnerY { get; }

    private ILevelValidator Validator { get; }
    private int PortPosition { get; }

    public State GetState() => Validator.GetInputStates()[PortPosition];
}

class OutputPort : IPort {
    private IGrid Grid;

    public OutputPort(IGrid grid, int x, int y) {
        Grid = grid;

        InnerX = x;
        InnerY = y;
    }

    public State GetState() {
        return Grid.Get(InnerX, InnerY);
    }

    public int InnerX { get; }
    public int InnerY { get; }
}

class Port : IPort {
    private IGrid Grid;

    public Port(IGrid grid, int x, int y) {
        Grid = grid;

        InnerX = x;
        InnerY = y;
    }

    public State GetState() {
        return Grid.Get(InnerX, InnerY);
    }

    public int InnerX { get; }
    public int InnerY { get; }
}