using Godot;
using System;

public partial class Door : CharacterBody2D
{
    [Export] public Texture2D Locked;
    [Export] public Texture2D Opened;
    [Export] public Texture2D Closed;
    [Export] public DoorState State = DoorState.Closed;
    [Export] public AudioStream UnlockSound;
    [Export] public AudioStream OpenSound;
    [Export] public AudioStream CloseSound;


    public override void _Ready()
    {
        base._Ready();
        // Set sprite based on state
        UpdateState(State);
    }

    public void Open()
    {
        if (State != DoorState.Locked)
        {
            UpdateState(DoorState.Open);
            PlayAudio(OpenSound);
        }
    }
    public void Unlock()
    {
        UpdateState(DoorState.Closed);
        PlayAudio(UnlockSound);
    }
    public void Close()
    {
        UpdateState(DoorState.Closed);
        PlayAudio(CloseSound);
    }
    public void Lock()
    {
        UpdateState(DoorState.Locked);
        PlayAudio(UnlockSound);
    }

    private void UpdateState(DoorState state)
    {
        // Update the state itself
        State = state;

        // Handle texture and collision swapping
        Sprite2D node = GetNode<Sprite2D>("Sprite2D");
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

    private void PlayAudio(AudioStream sound)
    {
        var audio = GetNode<AudioStreamPlayer>("/root/Main/Pickup");
        audio.Stream = sound;
        audio.Play();
    }

    public enum DoorState
    {
        Open,
        Closed,
        Locked
    }
}
