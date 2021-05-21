public interface IInitializableProps
{
    bool IsInitialized { get; }
}
public interface IInitializable : IInitializableProps
{
    void Init();
}
public interface IInitializable<Initializer> : IInitializableProps
{
    void Init(Initializer initializer);
}