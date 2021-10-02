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