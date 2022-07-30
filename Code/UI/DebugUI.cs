using Godot;
using System;

public class DebugUI : CanvasLayer
{

    [Signal] public delegate void LoadLevelPressed(string level);

    public override void _Ready()
    {

    }

    #region | Load Level Popup |
    public void ShowLevelSelect()
    {
        GetNode<PopupDialog>("LoadLevel").PopupCentered();
    }
    private void OnLoadLevelAcceptPressed()
    {
        GetNode<PopupDialog>("LoadLevel").Hide();
        EmitSignal(nameof(LoadLevelPressed), GetNode<LineEdit>("LoadLevel/Level").Text);
    }
    private void OnLoadLevelCancelPressed()
    {
        GetNode<PopupDialog>("LoadLevel").Hide();
    }
    #endregion

}
