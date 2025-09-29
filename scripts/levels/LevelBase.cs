using Godot;
using System;
using System.Collections.Generic;

public partial class LevelBase : Node2D
{
  private enum State { Ready, Moving, Win };


  private State _state = State.Ready;
  private State state
  {
    get => _state;
    set
    {
      _state = value;
    }
  }
  private Player player = null!;
  private TileMapLayer walls = null!;
  private TileMapLayer targets = null!;
  private WinScreen winScreen = null!;
  private List<Mover> boxes = new List<Mover>();
  private Stack<Command> applied = new Stack<Command>();
  private Stack<Command> reversed = new Stack<Command>();
  private int animating = 0;
  private int matchedCount = 0;


  [Export]
  public PackedScene? NextScene = null;


  [Export]
  public bool Debug = false;

  public override void _Ready()
  {
    player = GetNode<Player>("Player");
    walls = GetNode<TileMapLayer>("Walls");
    targets = GetNode<TileMapLayer>("Targets");
    winScreen = GetNode<WinScreen>("WinScreen");
    foreach (var node in GetTree().GetNodesInGroup(NodeGroups.Pushable))
    {
      if (node is Box box)
      {
        boxes.Add(box);
        box.MoveFinished += OnMoverMoveFinished;
        checkMatched(box, false);
      }
    }
  }


  public override void _Input(InputEvent @event)
  {
    if (state != State.Ready) return;

    if (Debug && @event.IsActionPressed("ui_accept"))
      win();
    else if (@event.IsActionPressed(Constants.Events.Reset))
      reset();
    else if (@event.IsActionPressed(Constants.Events.Undo))
      undo();
    else if (@event.IsActionPressed(Constants.Events.Redo))
      redo();
    else if (@event.IsActionPressed(Constants.Events.MoveLeft))
      tryToMakeMove(Direction.Left);
    else if (@event.IsActionPressed(Constants.Events.MoveRight))
      tryToMakeMove(Direction.Right);
    else if (@event.IsActionPressed(Constants.Events.MoveUp))
      tryToMakeMove(Direction.Up);
    else if (@event.IsActionPressed(Constants.Events.MoveDown))
      tryToMakeMove(Direction.Down);
  }


  public void OnMoverMoveFinished(Mover mover)
  {
    if (mover is Box box)
    {
      checkMatched(box);
    }

    --animating;
    if (animating == 0 && state != State.Win)
    {
      var dir = getMove();
      if (dir == null)
      {
        state = State.Ready;
      }
      else
      {
        tryToMakeMove(dir);
      }
    }
  }


  public void GoToNextScene()
  {
    if (NextScene != null)
    {
      GetTree().ChangeSceneToPacked(NextScene);
    }
  }


  // In response to arrow key
  private void tryToMakeMove(Direction? optDir)
  {
    Mover? push;
    if (optDir != null)
    {
      Direction dir = (Direction)optDir;

      if (playerCanMove(dir, out push))
      {
        state = State.Moving;
        move(dir, push);
      }
      else
      {
        player.Turn(dir);
      }
    }
  }


  private Direction? getMove()
  {
    if (Input.IsActionPressed("move_left"))
      return Direction.Left;
    else if (Input.IsActionPressed("move_right"))
      return Direction.Right;
    else if (Input.IsActionPressed("move_up"))
      return Direction.Up;
    else if (Input.IsActionPressed("move_down"))
      return Direction.Down;

    return null;
  }


  private bool isWall(Vector2I pos)
  {
    return walls.GetCellTileData(pos) != null;
  }


  private bool boxCanMove(Mover box, Direction dir)
  {
    var pos = box.Coords.Offset(dir);
    if (isWall(pos))
    {
      return false;
    }
    foreach (var box2 in boxes)
    {
      if (box2.Coords == pos)
      {
        return false;
      }
    }
    return true;
  }


  private bool playerCanMove(Direction dir, out Mover? push)
  {
    push = null;
    var pos = player.Coords.Offset(dir);
    if (isWall(pos))
    {
      return false;
    }
    foreach (var box in boxes)
    {
      if (box.Coords == pos)
      {
        if (boxCanMove(box, dir))
        {
          push = box;
          return true;
        }
        else
        {
          return false;
        }
      }
    }
    return true;
  }


  private void checkMatched(Box box, bool playing = true)
  {
    bool matched = false;
    var data = targets.GetCellTileData(box.Coords);
    if (data != null)
    {
      if ((string)data.GetCustomData("shade_name") == box.ShadeName)
      {
        matched = true;
      }
    }
    if (box.Matched != matched)
    {
      box.Matched = matched;
      if (matched)
      {
        ++matchedCount;
        if (playing)
        {
          GetNode<AudioStreamPlayer>("MatchSound").Play();
          if (matchedCount == boxes.Count)
          {
            win();
          }
        }
      }
      else
      {
        --matchedCount;
      }
    }
  }


  private void move(Direction dir, Mover? push)
  {
    Command command = new Command { direction = dir, push = push };
    apply(command);
    applied.Push(command);
    reversed.Clear();
  }


  private void undo()
  {
    if (applied.Count > 0)
    {
      Command command = applied.Pop();
      reverse(command);
      reversed.Push(command);
    }
  }


  private void redo()
  {
    if (reversed.Count > 0)
    {
      Command command = reversed.Pop();
      apply(command, false);
      applied.Push(command);
      if (command.push is Box box)
      {
        checkMatched(box);
      }
    }
  }


  private void apply(Command command, bool animate = true)
  {
    player.Move(command.direction, animate);
    if (animate) ++animating;
    if (command.push != null)
    {
      command.push.Move(command.direction, animate);
      if (animate) ++animating;
      if (command.push is Box box && box.Matched)
      {
        box.Matched = false;
        --matchedCount;
      }
    }
  }


  private void reverse(Command command)
  {
    player.Reverse(command.direction);
    if (command.push != null)
    {
      if (command.push is Box box && box.Matched)
      {
        box.Matched = false;
        --matchedCount;
      }

      command.push.Reverse(command.direction);

      if (command.push is Box box2)
      {
        checkMatched(box2);
      }

    }
  }


  private void win()
  {
    winScreen.Go();
    state = State.Win;
  }


  private void reset()
  {
    GetTree().ReloadCurrentScene();
  }
}
