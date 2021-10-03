static partial class Levels {
    public static Level HappyLevel() {

        var validator = new HappyValidator();
        int size = 10;

        var level = new Level(validator, size, size, "Happy Level", "A very happy level!");
        return level;
    }
}

class HappyValidator : ILevelValidator {
    public int InputCount => 1;
    public int OutputCount => 1;

    public ILevelState ValidateStates(State[] states) {
        return ILevelState.Nothing;
    }

    public State[] GetInputStates() {
        return new[] {State.WireOn};
    }

    public void MoveToNextInputState() {
    }

    public void Reset() {
    }
}
