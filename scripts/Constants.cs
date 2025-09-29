using Godot;

public static class Constants
{
  public const float TILE_WIDTH = 64;
  public const float TILE_HEIGHT = 64;


  public static class Animations
  {
    public static readonly StringName MoveLeft = "move_left";
    public static readonly StringName MoveRight = "move_right";
    public static readonly StringName MoveUp = "move_up";
    public static readonly StringName MoveDown = "move_down";

  }

  public static class Events
  {
    public static readonly StringName MoveLeft = "move_left";
    public static readonly StringName MoveRight = "move_right";
    public static readonly StringName MoveUp = "move_up";
    public static readonly StringName MoveDown = "move_down";
    public static readonly StringName Reset = "reset";
    public static readonly StringName Undo = "undo";
    public static readonly StringName Redo = "redo";
  }

  public static Direction Opposite(Direction dir)
  {
    switch (dir)
    {
      case Direction.Left:  return Direction.Right;
      case Direction.Right: return Direction.Left;
      case Direction.Up:    return Direction.Down;
      case Direction.Down:  return Direction.Up;
    }
    return dir;
  }
}

public static class NodeGroups
{
  public const string Pushable = "pushable";
}


public enum Direction
{
  Up, Down, Left, Right
}

struct Command
{
  public Direction direction;
  public Mover? push;
}

namespace Godot
{
  public static class Vector2IExtensions
  {

    public static Vector2I Offset(this Vector2I coords, Direction dir)
    {
      switch (dir)
      {
        case Direction.Up: return coords + Vector2I.Up;
        case Direction.Down: return coords + Vector2I.Down;
        case Direction.Left: return coords + Vector2I.Left;
        case Direction.Right: return coords + Vector2I.Right;
        default: return coords;
      }
    }
  }
}
