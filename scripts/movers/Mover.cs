using Godot;
using System;

public partial class Mover : Node2D
{
  [Signal]
  public delegate void MoveFinishedEventHandler(Mover mover);

  [Export]
  float MoveTimeHorz = 1.0F / 3.0F;
  [Export]
  float MoveTimeVert = 1.0F / 3.0F;


  public Vector2I Coords
  {
    get
    {
      return new Vector2I(
          (int)Math.Round(Position.X) / (int)Constants.TILE_WIDTH,
          (int)Math.Round(Position.Y) / (int)Constants.TILE_HEIGHT
      );
    }
  }


  protected Tween? tween;

  public virtual void Move(Direction dir, bool animate = true)
  {
    Vector2 offset = Vector2.Zero;
    float time = 0.0F;

    switch (dir)
    {
      case Direction.Left:
        offset = Vector2.Left * Constants.TILE_WIDTH;
        time = MoveTimeHorz;
        break;
      case Direction.Right:
        offset = Vector2.Right * Constants.TILE_WIDTH;
        time = MoveTimeHorz;
        break;
      case Direction.Up:
        offset = Vector2.Up * Constants.TILE_HEIGHT;
        time = MoveTimeVert;
        break;
      case Direction.Down:
        offset = Vector2.Down * Constants.TILE_HEIGHT;
        time = MoveTimeVert;
        break;
    }

    if (offset != Vector2.Zero)
    {
      if (animate)
      {
        tween = CreateTween();
        tween.TweenProperty(this, "position", Position + offset, time);
        tween.Finished += OnTweenFinish;
      }
      else
      {
        Position = Position + offset;
      }
    }
  }


  public virtual void Reverse(Direction dir)
  {
    Move(Constants.Opposite(dir), false);
  }

  protected virtual void OnTweenFinish()
  {
    EmitSignal(SignalName.MoveFinished, this);
  }
}
