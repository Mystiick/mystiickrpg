using Godot;
using System;

public class HUD : CanvasLayer
{
    public void UpdateHUD(Player p)
    {
        UpdateHealth(p.Health, p.MaxHealth);
        UpdateInventory(p);
    }

    public void UpdateHealth(int current, int max)
    {
        GetNode<Label>("Base/HealthBar/Health").Text = $"{current}/{max}";
        GetNode<TextureProgress>("Base/HealthBar").MaxValue = max;
        GetNode<TextureProgress>("Base/HealthBar").Value = current;
    }

    public void UpdateKeys(int current)
    {
        GetNode<Label>("Base/Keys").Text = $"Keys: {current}";
    }

    public void UpdateInventory(Player p)
    {
        for (int i = 0; i < p.Inventory.Size; i++)
        {
            TextureButton btn = GetNode<TextureButton>($"Base/InventoryAndPaperdoll/Inventory/{i}");
            Item item = p.Inventory[i];

            if (item != null)
            {
                btn.TextureNormal = item.Texture;
                btn.HintTooltip = item.Tooltip;
            }
            else
            {
                btn.TextureNormal = null;
                btn.HintTooltip = string.Empty;
            }
        }
    }

    public void OnInventoryItemUsed(int sender)
    {
        GetNode<Main>("/root/Main").CurrentPlayer.Inventory.UseItem(sender);
        UpdateInventory(GetNode<Main>("/root/Main").CurrentPlayer);
    }
}
