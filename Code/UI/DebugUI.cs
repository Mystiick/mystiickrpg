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
                // I need healing
                player.Heal(player.MaxHealth);
                break;

            case "thisisfine":
                // Turn on all light sources
                foreach (var light in GetTree().GetNodesInGroup("lights").Cast<Light2D>()) { light.Enabled = true; }
                break;

            case "lightsout":
                // Enable/disable darkness
                player.GetNode<CanvasModulate>("CanvasModulate").Visible = !player.GetNode<CanvasModulate>("CanvasModulate").Visible;
                break;

            case "equipme":
                // Equip items 1-8 (full starter set)
                for (int i = 1; i <= 8; i++)
                {
                    CheatAdd(ItemFactory.GetItemByID(i));
                }
                break;

            case "item":
                // Grant item X: `item 5`
                var item = ItemFactory.GetItemByID(int.Parse(input.Split(" ")[1]));
                item.Owner = player;
                CheatAdd(item);

                break;

            default:
                // No cheats matched, try loading a level with that name
                EmitSignal(nameof(LoadLevelPressed), input);
                break;
        }

    }
    private void OnLoadLevelCancelPressed()
    {
        GetNode<PopupDialog>("LoadLevel").Hide();
    }
    #endregion

    // Used from cheats to add an item to the player, or drop it if the player's inventory is full
    internal void CheatAdd(Item item)
    {
        var player = GetNode<Main>("/root/Main").CurrentPlayer;

        if (player.Inventory.HasSpace)
            player.Inventory.Add(item);
        else
            GetNode<Main>("/root/Main").PlayerDropItem(item);

        GetNode<Main>("/root/Main").UserInterface.HUD.UpdateHUD(player);
    }
}
