using Godot;
using System;

public partial class Player : Area2D
{
	[Export]
	public int Speed {get; set;} = 400;
	
	[Signal]
	public delegate void HitEventHandler();

	public Vector2 screenSize;

	public override void _Ready()
	{
		screenSize = GetViewportRect().Size;
		Hide();
	}

	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if(Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if(Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if(Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		if(Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		var animateSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		//키가 눌렸다면
		if(velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animateSprite2D.Play();
		}else{
			animateSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, screenSize.Y)
		);

		if(velocity.X != 0)
		{
			animateSprite2D.Animation = "walk";
			animateSprite2D.FlipV = false;
			animateSprite2D.FlipH = velocity.X < 0;
		}else if(velocity.Y != 0)
		{
			animateSprite2D.Animation = "up";
			animateSprite2D.FlipV = velocity.Y > 0; //아래로 내려가면 플립
		}
	}

	private void OnBodyEntered(PhysicsBody2D body)
	{
		Hide();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}
