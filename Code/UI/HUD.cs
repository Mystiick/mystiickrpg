using Godot;

public class HUD : CanvasLayer
{
    private TextureProgress _healthBar;
    private ItemButton[] _inventoryButtons;

    public override void _Ready()
    {
        base._Ready();
        _healthBar = GetNode<TextureProgress>("Base/HealthBar");

        var player = GetNode<Player>("/root/Main/GameContainer/GameCam/Player");

        _inventoryButtons = new ItemButton[player.Inventory.Size];
        for (int i = 0; i < player.Inventory.Size; i++)
        {
            _inventoryButtons[i] = (GetNode<ItemButton>($"Base/InventoryAndPaperdoll/Inventory/{i}"));
            _inventoryButtons[i].Connect(nameof(ItemButton.ItemUsed), this, nameof(OnInventoryItemUsed));
        }
    }

    public void UpdateHUD(Player p)
    {
        UpdateHealth(p.Health, p.MaxHealth);
        UpdateInventory(p);
    }

    public void UpdateHealth(int current, int max)
    {
        _healthBar.GetNode<Label>("Health").Text = $"{current}/{max}";
        _healthBar.MaxValue = max;
        _healthBar.Value = current;
    }

    public void UpdateInventory(Player p)
    {
        for (int i = 0; i < p.Inventory.Size; i++)
        {
            ItemButton btn = GetNode<ItemButton>($"Base/InventoryAndPaperdoll/Inventory/{i}");
            Item item = p.Inventory[i];

            if (item != null)
            {
                btn.Item = item;
                btn.Inventory = p.Inventory;
                btn.TextureNormal = item.Texture;
                btn.HintTooltip = item.Tooltip;
            }
            else
            {
                btn.Item = null;
                btn.TextureNormal = null;
                btn.HintTooltip = string.Empty;
            }
        }
    }

    public void OnInventoryItemUsed()
    {
        UpdateInventory(GetNode<Main>("/root/Main").CurrentPlayer);
    }
}
