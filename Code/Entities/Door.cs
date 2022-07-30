using Godot;
using System;

public class Door : KinematicBody2D
{
    [Export] public Texture Locked;
    [Export] public Texture Opened;
    [Export] public Texture Closed;
    [Export] public DoorState State = DoorState.Closed;

    public override void _Ready()
    {
        // Set sprite based on state
        UpdateState(State);
    }

    public void Open()
    {
        if (State != DoorState.Locked)
            UpdateState(DoorState.Open);
    }
    public void Unlock()
    {
        UpdateState(DoorState.Closed);
    }
    public void Close()
    {
        UpdateState(DoorState.Closed);
    }
    public void Lock()
    {
        UpdateState(DoorState.Locked);
    }

    private void UpdateState(DoorState state)
    {
        // Update the state itself
        State = state;

        // Handle texture and collision swapping
        Sprite node = GetNode<Sprite>("Sprite");
        CollisionShape2D collider = GetNode<CollisionShape2D>("CollisionShape2D");

        switch (state)
        {
            case DoorState.Open:
                node.Texture = Opened;
                collider.Disabled = true;
                break;

            case DoorState.Closed:
                node.Texture = Closed;
                collider.Disabled = false;
                break;

            case DoorState.Locked:
                node.Texture = Locked;
                collider.Disabled = false;
                break;

        }
    }

    public enum DoorState
    {
        Open,
        Closed,
        Locked
    }
}
