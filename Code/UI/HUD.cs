using Godot;

public class HUD : CanvasLayer
{
    private TextureProgress _healthBar;
    private ItemButton[] _inventoryButtons;

    public override void _Ready()
    {
        base._Ready();

        // Grab references to the Health Bar and Inventory buttons for ease of use later
        _healthBar = GetNode<TextureProgress>("Base/HealthBar");

        Player player = GetNode<Main>("/root/Main").CurrentPlayer;
        _inventoryButtons = new ItemButton[player.Inventory.Size];
        for (int i = 0; i < player.Inventory.Size; i++)
        {
            _inventoryButtons[i] = GetNode<ItemButton>($"Base/InventoryAndPaperdoll/Inventory/{i}");
            _inventoryButtons[i].Inventory = player.Inventory;
            _inventoryButtons[i].Connect(nameof(ItemButton.ItemUsed), this, nameof(OnInventoryItemUsed));
            _inventoryButtons[i].Connect(nameof(ItemButton.ItemDropped), this, nameof(OnInventoryItemDropped));
        }
    }

    /// <summary>
    /// Updates the entire HUD for the player.
    /// Updates Health bar, Inventory
    /// </summary>
    public void UpdateHUD(Player p)
    {
        UpdateHealth(p.Health, p.MaxHealth);
        UpdateInventory(p);
    }

    /// <summary>
    /// Updates the player Health bar to the specified Current/Max amounts
    /// </summary>
    public void UpdateHealth(int current, int max)
    {
        _healthBar.GetNode<Label>("Health").Text = $"{current}/{max}";
        _healthBar.MaxValue = max;
        _healthBar.Value = current;
    }

    /// <summary>
    /// Refreshes the Inventory icons based on the given Player's inventory
    /// </summary>
    public void UpdateInventory(Player p)
    {
        for (int i = 0; i < p.Inventory.Size; i++)
        {
            ItemButton btn = GetNode<ItemButton>($"Base/InventoryAndPaperdoll/Inventory/{i}");
            Item item = p.Inventory[i];

            if (item != null)
            {
                btn.Item = item;
                btn.TextureNormal = item.Texture;
                btn.HintTooltip = item.BuildTooltip();
            }
            else
            {
                btn.Item = null;
                btn.TextureNormal = null;
                btn.HintTooltip = string.Empty;
            }
        }

        foreach (var eq in p.Equipment.Items)
        {
            EquipItem(eq.Value, $"Base/InventoryAndPaperdoll/Paperdoll/{eq.Key}");
        }
    }

    /// <summary>
    /// Called when the player uses an item. Used to update the inventory icons
    /// </summary>
    public void OnInventoryItemUsed()
    {
        UpdateHUD(GetNode<Main>("/root/Main").CurrentPlayer);
    }

    public void OnInventoryItemDropped(Item item)
    {
        var main = GetNode<Main>("/root/Main");
        main.PlayerDropItem(item);

        UpdateInventory(main.CurrentPlayer);
    }

    private void EquipItem(Equipable item, string nodePath)
    {
        ItemButton btn = GetNode<ItemButton>(nodePath);

        if (item != null)
        {
            btn.Item = item;
            btn.TextureNormal = item.Texture;
            btn.HintTooltip = item.BuildTooltip();
        }
        else
        {
            btn.Item = null;
            btn.TextureNormal = null;
            btn.HintTooltip = string.Empty;
        }
    }
}
