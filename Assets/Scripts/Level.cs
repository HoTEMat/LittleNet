using System.Collections.Generic;

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

class Level : ILevel {
    private IPort[] OutputPorts;

    private int iteration = 0;
    
    public Level(ILevelValidator validator, int width, int height) {
        Validator = validator;
        Grid = new Grid(width, height);

        // create evenly spaced input ports
        int inputSpacing = height / (Validator.InputCount + 1);
        for (int i = 0; i < Validator.InputCount; i++)
            Grid.AddPort(new InputPort(0, i * inputSpacing, Validator, i));

        List<IPort> outputPortsList = new List<IPort>();
        
        // create evenly spaced output ports (that are just normal ports)
        int outputSpacing = height / (Validator.OutputCount + 1);
        for (int i = 0; i < Validator.InputCount; i++) {
            var Port = new Port(Grid, width - 1, i * inputSpacing);
                
            Grid.AddPort(Port);
            outputPortsList.Add(Port);
        }
    }

    public IGrid Grid { get; }

    public ILevelValidator Validator { get; }
    public ILevelState DoIteration() {
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
        
        // TODO: propagate to grid
        Validator.Reset();
    }
}