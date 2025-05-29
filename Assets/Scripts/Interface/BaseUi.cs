using System;
using Godot;

namespace ForeignGeneer.Assets.Scripts.Interface;

public interface BaseUi : IObserver
{
    public void initialize(Object data);
}