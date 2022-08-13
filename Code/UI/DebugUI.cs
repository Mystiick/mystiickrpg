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
            case "equipme":
                var texts = new Texture[] {
                    ResourceLoader.Load<Texture>("res://Assets/bloodstain.png"),
                    ResourceLoader.Load<Texture>("res://Assets/bloodstain2.png"),
                    ResourceLoader.Load<Texture>("res://Assets/bones.png"),
                    ResourceLoader.Load<Texture>("res://Assets/chicken.png"),
                    ResourceLoader.Load<Texture>("res://Assets/crab.png"),
                    ResourceLoader.Load<Texture>("res://Assets/door_closed.png"),
                    ResourceLoader.Load<Texture>("res://Assets/door_locked.png"),
                    ResourceLoader.Load<Texture>("res://Assets/door_open.png"),
                    ResourceLoader.Load<Texture>("res://Assets/key.png"),
                    ResourceLoader.Load<Texture>("res://Assets/player.png"),
                    ResourceLoader.Load<Texture>("res://Assets/skele.png"),
                    ResourceLoader.Load<Texture>("res://Assets/stairs_down.png"),
                    ResourceLoader.Load<Texture>("res://Assets/stairs_up.png"),
                    ResourceLoader.Load<Texture>("res://Assets/torch.png"),
                    ResourceLoader.Load<Texture>("res://Assets/zombie.png")
                };

                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG HEAD", Texture = texts.Random(), Slot = Equipable.SlotType.Head });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG SHOULDERS", Texture = texts.Random(), Slot = Equipable.SlotType.Shoulders });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG KNEES", Texture = texts.Random(), Slot = Equipable.SlotType.Legs });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG AND TOES", Texture = texts.Random(), Slot = Equipable.SlotType.Boots });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG Chest", Texture = texts.Random(), Slot = Equipable.SlotType.Chest });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG Amulet", Texture = texts.Random(), Slot = Equipable.SlotType.Amulet });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG LeftHand", Texture = texts.Random(), Slot = Equipable.SlotType.LeftHand });
                player.Inventory.Add(new Equipable() { Owner = player, Tooltip = "DEBUG RightHand", Texture = texts.Random(), Slot = Equipable.SlotType.RightHand });

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
