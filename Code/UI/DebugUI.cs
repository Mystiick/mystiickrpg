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
        switch (input.Split(" ")[0].ToLower())
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

            case "equipme":
                player.Inventory.Add(ItemFactory.GetItemByID(1));
                player.Inventory.Add(ItemFactory.GetItemByID(2));
                player.Inventory.Add(ItemFactory.GetItemByID(3));
                player.Inventory.Add(ItemFactory.GetItemByID(4));
                player.Inventory.Add(ItemFactory.GetItemByID(5));
                player.Inventory.Add(ItemFactory.GetItemByID(6));
                player.Inventory.Add(ItemFactory.GetItemByID(7));
                player.Inventory.Add(ItemFactory.GetItemByID(8));

                GetNode<Main>("/root/Main").UserInterface.HUD.UpdateHUD(player);

                break;

            case "item":
                var item = ItemFactory.GetItemByID(int.Parse(input.Split(" ")[1]));
                item.Owner = player;
                player.Inventory.Add(item);

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
