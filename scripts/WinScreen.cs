using Godot;
using System;

public partial class WinScreen : CanvasLayer
{
  [Signal]
  public delegate void WinFinishEventHandler();

  public void Go()
  {
    Visible = true;
    GetNode<GpuParticles2D>("LeftParticles").Emitting = true;
    GetNode<GpuParticles2D>("RightParticles").Emitting = true;
    GetNode<AudioStreamPlayer>("WinMusic").Play();
    GetNode<Timer>("Timer").Start();
  }


  public void OnTimerTimeout()
  {
    EmitSignal(SignalName.WinFinish);
  }

}
