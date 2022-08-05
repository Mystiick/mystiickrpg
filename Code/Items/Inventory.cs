using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Inventory : IEnumerable<Item>
{
    public int Size { get; private set; }
    private Item[] _items;


    public Inventory() : this(12) { }
    public Inventory(int size)
    {
        Size = size;
        _items = new Item[Size];
    }

    /// <summary>
    /// Checks if there are any free spaces in the inventory
    /// </summary>
    public bool HasSpace()
    {
        return _items.Any(x => x == null);
    }

    /// <summary>
    /// Add the specified item to the inventory. If the inventory is full, it throws a OverflowException
    /// </summary>
    public int Add(Item item)
    {
        if (!HasSpace())
            throw new OverflowException("Cannot add an item to the inventory if it is full. Check before adding an item and handle properly if the inventory is full.");

        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = item;
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Use the specified item from the inventory
    /// </summary>
    public void UseItem(Item item)
    {
        System.Diagnostics.Debug.Assert(item != null, "Item cannot be null");

        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == item)
            {
                _items[i].Use();
                _items[i] = null;
            }
        }
    }

    /// <summary>
    /// Use the specified item by index
    /// </summary>
    public void UseItem(int index)
    {
        _items[index].Use();
        _items[index] = null;
    }

    /// <summary>
    /// Removes all items from the inventory without using them.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < _items.Length; i++)
            _items[i] = null;
    }

    #region | IEnumerator & IE<Item> Implementation |
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerator<Item> GetEnumerator()
    {
        foreach (Item i in _items)
        {
            yield return i;
        }
    }
    public Item this[int i]
    {
        get { return _items[i]; }
    }
    #endregion

}