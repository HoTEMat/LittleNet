public class HappyValidator : ILevelValidator
{
    public int InputCount => 1;
    public int OutputCount => 1;

    public ILevelState ValidateStates(State[] states) {
        return ILevelState.Nothing;
    }

    public State[] GetInputStates() {
        return new[] {State.WireOn};
    }

    public void MoveToNextInputState() { }
}
