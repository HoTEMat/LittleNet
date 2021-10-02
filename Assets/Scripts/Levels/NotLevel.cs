static partial class Levels {
    public static Level NotLevel() {

        var validator = new HappyValidator();
        int size = 10;

        var level = new Level(validator, size, size);
        return level;
    }
}

class NotLevelValidator : ILevelValidator {
    public int InputCount => 1;
    public int OutputCount => 1;

    public int remainingSwaps = 4;

    public bool previousOutputState = true;
    public bool currentInputState = false;

    public ILevelState ValidateStates(State[] states) {
        previousOutputState = states[0] == State.WireOn;

        if (remainingSwaps == 0 && previousOutputState != currentInputState)
            return ILevelState.Success;
        
        return ILevelState.Nothing;
    }

    public State[] GetInputStates() => new[] { currentInputState ? State.WireOn : State.WireOff };

    public void MoveToNextInputState() {
        if (previousOutputState == currentInputState)
            
    }

    public void Reset() {
    }
}
