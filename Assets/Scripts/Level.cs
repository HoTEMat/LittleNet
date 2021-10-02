using System.Collections.Generic;
using System.Linq;

class Level : ILevel {
    private IPort[] OutputPorts;

    private int iteration;

    public Level(ILevelValidator validator, int width, int height) {
        Validator = validator;
        Grid = new SimulationGrid(width, height, "Root grid");

        // create evenly spaced input ports
        int inputSpacing = height / (Validator.InputCount + 1);
        for (int i = 0; i < Validator.InputCount; i++)
            Grid.AddPort(new InputPort(0, i * inputSpacing, Validator, i));

        List<IPort> outputPortsList = new List<IPort>();

        // create evenly spaced output ports
        int outputSpacing = height / (Validator.OutputCount + 1);
        for (int i = 0; i < Validator.InputCount; i++) {
            var port = new OutputPort(Grid, width - 1, i * outputSpacing);

            Grid.AddPort(port);
            outputPortsList.Add(port);
        }

        OutputPorts = outputPortsList.ToArray();
    }

    public Level(ILevelValidator validator, int width, int height, IEnumerable<InputPort> inputs, IEnumerable<OutputPort> outputs) {
        Validator = validator;

        Grid = new SimulationGrid(width, height, "Root grid");

        foreach (var port in inputs.Cast<IPort>().Concat(outputs)) {
            Grid.AddPort(port);
        }
    }

    public SimulationGrid Grid { get; private set; }

    public ILevelValidator Validator { get; }

    public ILevelState DoIteration() {
        if (iteration == 0) {
            Grid = Grid.Clone();
        }

        Grid.DoIteration();
        Grid.DoSwap();

        Validator.MoveToNextInputState();

        List<State> outputStatesList = new List<State>();
        foreach (var t in OutputPorts)
            outputStatesList.Add(t.GetState());

        return Validator.ValidateStates(outputStatesList.ToArray());
    }

    public int GetIteration => iteration;

    public void Reset() {
        iteration = 0;

        Grid = Grid.Prototype.ProtypeGrid;
        Validator.Reset();
    }
}