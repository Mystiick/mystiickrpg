using Godot;
using System.Linq;

public class DebugUI : CanvasLayer
{
    [Signal] public delegate void LoadLevelPressed(string level);

    #region | Load Level Popup |
    public void ShowLevelSelect()
    {
        GetNode<PopupDialog>("LoadLevel").PopupCentered();
    }
    private void OnLoadLevelAcceptPressed()
    {
        GetNode<PopupDialog>("LoadLevel").Hide();
        string input = GetNode<LineEdit>("LoadLevel/Level").Text;

        var player = GetNode<Main>("/root/Main").CurrentPlayer;
        switch (input.ToLower())
        {
            case "genji":
                player.Heal(player.MaxHealth);
                break;

            case "thisisfine":
                foreach (var light in GetTree().GetNodesInGroup("lights").Cast<Light2D>()) { light.Enabled = true; }
                break;

            case "lightsout":
                player.GetNode<CanvasModulate>("CanvasModulate").Visible = !player.GetNode<CanvasModulate>("CanvasModulate").Visible;
                break;

            case "item":
                player.Inventory.Add(new Equipable()
                {
                    Owner = player,
                    Tooltip = "DEBUG EQUIPMENT",
                    Texture = ResourceLoader.Load<Texture>("res://Assets/bloodstain.png"),
                    Slot = Equipable.SlotType.Head
                });
                GetNode<Main>("/root/Main").UserInterface.HUD.UpdateHUD(player);
                break;

            case "item2":
                player.Inventory.Add(new Equipable()
                {
                    Owner = player,
                    Tooltip = "DEBUG EQUIPMENT 2",
                    Texture = ResourceLoader.Load<Texture>("res://Assets/bloodstain2.png"),
                    Slot = Equipable.SlotType.Head
                });
                GetNode<Main>("/root/Main").UserInterface.HUD.UpdateHUD(player);
                break;

            default:
                EmitSignal(nameof(LoadLevelPressed), input);
                break;
        }

    }
    private void OnLoadLevelCancelPressed()
    {
        GetNode<PopupDialog>("LoadLevel").Hide();
    }
    #endregion

}
