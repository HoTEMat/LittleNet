class Level : ILevel
{
    public Level(ILevelValidator Validator) {
        // TODO: konstrukce gridu
    }
    
    public IGrid Grid { get; }
    
    public ILevelValidator Validator { get; }
}
