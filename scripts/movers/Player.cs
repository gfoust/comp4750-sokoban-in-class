using Godot;

public partial class Player : Mover
{


    protected AnimatedSprite2D sprite = null!;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }


    public void Turn(Direction dir)
    {
        string animation = animationForDirection(dir);

        if (animation != "")
        {
            sprite.Animation = animation;
        }

    }


    public override void Move(Direction dir, bool animate = true)
    {
        string animation = animationForDirection(dir);

        if (animate && animation != "")
        {
            if (animation == sprite.Animation)
            {
                sprite.Play();
            }
            else
            {
                sprite.Play(animation);
            }
        }

        base.Move(dir, animate);
    }


  public override void Reverse(Direction dir)
  {
    base.Reverse(dir);
    Turn(dir);
  }



  protected override async void OnTweenFinish()
  {
    base.OnTweenFinish();
    var timer = GetTree().CreateTimer(0.08);
    await ToSignal(timer, Timer.SignalName.Timeout);
    if (tween == null || !tween.IsRunning())
    {
      sprite.Stop();
    }
  }


    private string animationForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Left:
                return "move_left";
            case Direction.Right:
                return "move_right";
            case Direction.Up:
                return "move_up";
            case Direction.Down:
                return "move_down";
            default:
                return "";
        }
    }
}
