using Godot;
using System.Linq;

public partial class DropTable
{
    public string Name { get; set; }
    public Drop[] Drops { get; set; }

    /// <summary>Returns an item if one is dropped from the drop table. If no item is chosen, null will be returned</summary>
    public Item GetDrop()
    {
        var totalChance = Drops.Sum(x => x.DropChance);
        var roll = GD.Randi() % totalChance;
        int chosenID = -1;

        for (int i = 0, j = 0; j <= roll; i++, j += Drops[i - 1].DropChance)
            chosenID = Drops[i].ItemID;

        if (chosenID > 0)
            return ItemFactory.GetItemByID(chosenID);
        else
            return null;
    }
}