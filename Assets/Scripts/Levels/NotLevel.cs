static partial class Levels {
    public static Level NotLevel() {

        var validator = new NotValidator();
        int size = 10;

        var level = new Level(validator, size, size);
        return level;
    }
}

class NotValidator : ILevelValidator {

    public NotValidator(int initialSwaps = 4) {
        InitialSwaps = initialSwaps;
    }
    
    public int InputCount => 1;
    public int OutputCount => 1;

    public int InitialSwaps;
    public int RemainingSwaps = 4;

    public bool previousOutputState = false;
    public bool currentInputState = false;

    public ILevelState ValidateStates(State[] states) {
        previousOutputState = states[0] == State.WireOn;

        if (RemainingSwaps == 0 && previousOutputState != currentInputState)
            return ILevelState.Success;
        
        return ILevelState.Nothing;
    }

    public State[] GetInputStates() => new[] { currentInputState ? State.WireOn : State.WireOff };

    public void MoveToNextInputState() {
        if (previousOutputState == currentInputState) {
            RemainingSwaps -= 1;
            currentInputState = !currentInputState;
        }
    }

    public void Reset() {
        RemainingSwaps = InitialSwaps;
    }
}
