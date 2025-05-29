namespace ForeignGeneer.Assets.Scripts;

public interface IObserver
{
    void update(InterfaceType? interfaceType = null);
    void detach();
}