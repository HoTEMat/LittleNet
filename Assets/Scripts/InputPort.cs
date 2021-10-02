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
