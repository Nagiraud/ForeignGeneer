using ForeignGeneer.Assets.Scripts;

public interface IObservable
{
    void attach(IObserver observer);
    void detach(IObserver observer);
    void notify(InterfaceType? interfaceType = null);
}
