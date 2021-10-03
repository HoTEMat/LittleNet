using System.Linq;

abstract class CalendarValidator : ILevelValidator {

    protected delegate ILevelState DelayedValidator(bool[] outputs);

    SortedQueue<int, DelayedValidator> validators = new SortedQueue<int, DelayedValidator>();

    public abstract int InputCount { get; }
    public abstract int OutputCount { get; }
    public int Iterations { get; private set; }

    public abstract bool[] GetInputs();

    public State[] GetInputStates() {
        return GetInputs().Select(b => b ? State.WireOn : State.WireDead).ToArray();
    }

    public virtual void MoveToNextInputState() {
        Iterations++;
    }

    public virtual void Reset() {
        Iterations = 0;
        validators.Clear();
    }

    protected void Expect(int after, DelayedValidator validator) {
        validators.Enqueue(Iterations + after, validator);
    }

    protected void Succeed() {
        validators.Enqueue(Iterations, outputs => ILevelState.Success);
    }
    protected void Fail() {
        validators.Enqueue(Iterations, outputs => ILevelState.Failure);
    }

    public ILevelState ValidateStates(State[] outputs) {

        while (validators.Count != 0 && Iterations >= validators.FirstKey) {
            var validator = validators.Dequeue();
            var result = validator.Invoke(outputs.Select(v => v == State.WireOn).ToArray());
            if (result != ILevelState.Nothing) {
                return result;
            }
        }

        return ILevelState.Nothing;
    }
}