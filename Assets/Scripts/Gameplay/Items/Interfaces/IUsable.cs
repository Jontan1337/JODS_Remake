using Mirror;

public interface IUsable
{
    [Server]
    void Svr_Use();
    [Server]
    void Svr_AltUse();
}
