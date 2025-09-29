using Godot;
using System;

[Tool]
public partial class Box : Mover
{
  public enum ShadeEnum
  {
    Brown, Red, Blue, Green, Gray
  }

  private static class FrameNumbers
  {
    public const int
      Brown = 30,
      Red = 31,
      Blue = 32,
      Green = 33,
      Gray = 34;
  }


  private ShadeEnum _shade = ShadeEnum.Brown;
  [Export]
  public ShadeEnum Shade
  {
    get => _shade;
    set
    {
      _shade = value;
      updateTexture();
    }
  }


  public string ShadeName
  {
    get
    {
      switch (Shade)
      {
        case ShadeEnum.Brown:
          return "brown";
        case ShadeEnum.Red:
          return "red";
        case ShadeEnum.Blue:
          return "blue";
        case ShadeEnum.Green:
          return "green";
        case ShadeEnum.Gray:
          return "gray";
        default:
          return "error";
      }
    }
  }


  private bool _matched = false;
  [Export]
  public bool Matched
  {
    get => _matched;
    set
    {
      _matched = value;
      updateMatch();
    }
  }


  private void updateTexture()
  {
    var sprite = GetNode<Sprite2D>("Sprite2D");
    switch (Shade)
    {
      case ShadeEnum.Brown:
        sprite.Frame = FrameNumbers.Brown;
        break;
      case ShadeEnum.Red:
        sprite.Frame = FrameNumbers.Red;
        break;
      case ShadeEnum.Blue:
        sprite.Frame = FrameNumbers.Blue;
        break;
      case ShadeEnum.Green:
        sprite.Frame = FrameNumbers.Green;
        break;
      case ShadeEnum.Gray:
        sprite.Frame = FrameNumbers.Gray;
        break;
    }
  }


  private void updateMatch()
  {
    GetNode<Sprite2D>("Checkmark").Visible = _matched;
  }
}
