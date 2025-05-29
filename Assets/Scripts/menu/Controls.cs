using Godot;
using System;

public partial class Controls : BoxContainer
{
    private bool waitingForInput = false;
    private InputEvent inputEvent;
    Button actionKey;
    string actionName;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //initialisation du text des bouton

        foreach (Node controlInput in GetChildren()){
            if (InputMap.HasAction(controlInput.Name)) {
                if (controlInput.GetChild(0) is Button button){
                    if(InputMap.ActionGetEvents(controlInput.Name)[0] is InputEventKey actionKey)
                        button.Text = actionKey.Keycode.ToString();
                }
            }
        }
    }

    public override void _Input(InputEvent evt)
    {
        if (waitingForInput && (evt is InputEventKey keyEvent && keyEvent.Pressed)){
            inputEvent = evt;
            actionKey.Text = keyEvent.Keycode.ToString();
            if (InputMap.HasAction(actionName)){
                GD.Print("Changing controls");
                InputMap.ActionEraseEvents(actionName);
                InputMap.ActionAddEvent(actionName, keyEvent);
            }
            waitingForInput = false;
            waitingForInput = false;
        }
    }

    private void _onControlChange(string action)
    {
        GD.Print(action + " modification");
        actionName = action;
        actionKey = GetNode<Button>(action + "/Button");
        foreach (Node controlInput in GetChildren()){
            if (controlInput.GetChild(0) is Button button){
                button.Disabled = true;
            }
        }
        inputEvent = null;
        waitingForInput = true;
        actionKey.ButtonPressed = false;
        foreach (Node controlInput in GetChildren()){
            if (controlInput.GetChild(0) is Button button)
                button.Disabled = false;
        }
    }
}
