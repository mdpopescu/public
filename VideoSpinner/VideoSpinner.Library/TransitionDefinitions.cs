using System;
using Splicer.Timeline;

namespace Renfield.VideoSpinner.Library
{
  public class TransitionDefinitions
  {
    // ReSharper disable InconsistentNaming
    public static readonly Guid WMTFX = new Guid("B4DC8DD9-2CC1-4081-9B2B-20D7030234EF");
    // ReSharper restore InconsistentNaming

    public static TransitionDefinition CreateEaseInEffect()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 0.5));
      definition.Parameters.Add(new Parameter("ScaleA", 1.5));
      definition.Parameters.Add(new Parameter("ExponentialProgressDuration", 0.01));

      return definition;
    }

    public static TransitionDefinition CreateEaseOutEffect()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.5));
      definition.Parameters.Add(new Parameter("ScaleA", 0.5));
      definition.Parameters.Add(new Parameter("ExponentialProgressDuration", 0.01));

      return definition;
    }

    public static TransitionDefinition PanUp()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "up"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static TransitionDefinition PanDown()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "down"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static TransitionDefinition PanLeft()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "left"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static TransitionDefinition PanRight()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "right"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static TransitionDefinition RotateAndZoomOut()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", "smaller"));
      definition.Parameters.Add(new Parameter("RotateA", "right"));
      definition.Parameters.Add(new Parameter("ExponentialProgressDuration", 1));

      return definition;
    }

    public static TransitionDefinition FlipIn()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.0));
      definition.Parameters.Add(new Parameter("RotateA", "in"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 1));

      return definition;
    }

    public static TransitionDefinition FlipOut()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.0));
      definition.Parameters.Add(new Parameter("RotateA", "out"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 1));

      return definition;
    }

    public static TransitionDefinition PinWheelZoomIn()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 0.5));
      definition.Parameters.Add(new Parameter("ScaleA", 1.5));
      definition.Parameters.Add(new Parameter("RotateA", "right"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 0.5));

      return definition;
    }

    public static TransitionDefinition PinWheelZoomOut()
    {
      var definition = new TransitionDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.5));
      definition.Parameters.Add(new Parameter("ScaleA", 0.5));
      definition.Parameters.Add(new Parameter("RotateA", "right"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 0.5));

      return definition;
    }
  }
}