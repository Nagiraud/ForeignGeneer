using System;
using System.Collections.Generic;
using ForeignGeneer.Assets.Scripts;

public interface ISlotObservable
{
    void attach(int slotIndex, IObserver observer);
    void detach(int slotIndex, IObserver observer);
    void notify(int slotIndex);
}