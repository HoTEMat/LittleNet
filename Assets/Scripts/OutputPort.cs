
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
