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

            case "equipme2":
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

                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG HEAD", Texture = texts.Random(), Slot = Equipable.SlotType.Head });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG SHOULDERS", Texture = texts.Random(), Slot = Equipable.SlotType.Shoulders });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG KNEES", Texture = texts.Random(), Slot = Equipable.SlotType.Legs });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG AND TOES", Texture = texts.Random(), Slot = Equipable.SlotType.Boots });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG Chest", Texture = texts.Random(), Slot = Equipable.SlotType.Chest });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG Amulet", Texture = texts.Random(), Slot = Equipable.SlotType.Amulet });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG LeftHand", Texture = texts.Random(), Slot = Equipable.SlotType.LeftHand });
                player.Inventory.Add(new Equipable() { Owner = player, Name = "DEBUG RightHand", Texture = texts.Random(), Slot = Equipable.SlotType.RightHand });

                GetNode<Main>("/root/Main").UserInterface.HUD.UpdateHUD(player);
                break;

            case "item 1":
                var item = ItemFactory.GetItemByID(1);
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
