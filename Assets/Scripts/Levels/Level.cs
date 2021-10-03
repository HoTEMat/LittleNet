using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

class Level : ILevel {

    private IEnumerable<IPort> outputPorts => Grid.GetPorts().Where(p => p is OutputPort);
    private int iteration;

    public Level(ILevelValidator validator, int width, int height, string name, string description) {
        Name = name;
        Description = description;
        
        Validator = validator;
        Grid = new SimulationGrid(width, height, "Root grid");

        // create evenly spaced input ports
        int inputSpacing = height / (Validator.InputCount + 1);
        for (int i = 1; i <= Validator.InputCount; i++)
            Grid.AddPort(new InputPort(0, i * inputSpacing, Validator, i - 1));

        // create evenly spaced output ports
        int outputSpacing = height / (Validator.OutputCount + 1);
        for (int i = 1; i <= Validator.InputCount; i++) {
            var port = new OutputPort(Grid, width - 1, i * outputSpacing);
            Grid.AddPort(port);
        }
    }

    public Level(ILevelValidator validator, int width, int height, IEnumerable<InputPort> inputs, IEnumerable<OutputPort> outputs) {
        Validator = validator;

        Grid = new SimulationGrid(width, height, "Root grid");

        foreach (var port in inputs.Cast<IPort>().Concat(outputs)) {
            Grid.AddPort(port);
        }
    }

    public string Name { get; }
    public string Description { get; }
    public SimulationGrid Grid { get; private set; }

    public ILevelValidator Validator { get; }

    public ILevelState DoIteration() {
        if (iteration == 0) {
            Grid = Grid.Clone();
        }

        iteration++;
        Debug.Print($"iteration: {iteration}");

        Grid.DoIteration();
        Grid.DoSwap();

        Validator.MoveToNextInputState();

        List<State> outputStatesList = new List<State>();
        foreach (var t in outputPorts)
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