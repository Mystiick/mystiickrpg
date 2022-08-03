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

    public bool HasSpace()
    {
        return _items.Any(x => x == null);
    }

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
    public void UseItem(Item item)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == item)
            {
                _items[i].Use();
                _items[i] = null;
            }
        }
    }

    public void UseItem(int index)
    {
        _items[index].Use();
        _items[index] = null;
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