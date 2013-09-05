using System;
using Splicer.Timeline;

namespace Renfield.VideoSpinner.Library
{
  public static class EffectDefinitions
  {
// ReSharper disable InconsistentNaming
    public static readonly Guid WMTFXEffect = new Guid("B4DC8DD9-2CC1-4081-9B2B-20D7030234EF");
// ReSharper restore InconsistentNaming

    public static EffectDefinition CreateEaseInEffect()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);

      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 0.5));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.5));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressDuration", 0.01));

      return effectDefinition;
    }

    public static EffectDefinition CreateEaseOutEffect()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);

      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.5));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 0.5));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressDuration", 0.01));

      return effectDefinition;
    }

    public static EffectDefinition PanUp()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("MoveA", "up"));
      effectDefinition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return effectDefinition;
    }

    public static EffectDefinition PanDown()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("MoveA", "down"));
      effectDefinition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return effectDefinition;
    }

    public static EffectDefinition PanLeft()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("MoveA", "left"));
      effectDefinition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return effectDefinition;
    }

    public static EffectDefinition PanRight()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("MoveA", "right"));
      effectDefinition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return effectDefinition;
    }

    public static EffectDefinition RotateAndZoomOut()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", "smaller"));
      effectDefinition.Parameters.Add(new Parameter("RotateA", "right"));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressDuration", 1));

      return effectDefinition;
    }

    public static EffectDefinition FlipIn()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.0));
      effectDefinition.Parameters.Add(new Parameter("RotateA", "in"));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressScale", 1));

      return effectDefinition;
    }

    public static EffectDefinition FlipOut()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.0));
      effectDefinition.Parameters.Add(new Parameter("RotateA", "out"));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressScale", 1));

      return effectDefinition;
    }

    public static EffectDefinition PinWheelZoomIn()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 0.5));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 1.5));
      effectDefinition.Parameters.Add(new Parameter("RotateA", "right"));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressScale", 0.5));

      return effectDefinition;
    }

    public static EffectDefinition PinWheelZoomOut()
    {
      var effectDefinition = new EffectDefinition(WMTFXEffect);
      effectDefinition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      effectDefinition.Parameters.Add(new Parameter("InitialScaleA", 1.5));
      effectDefinition.Parameters.Add(new Parameter("ScaleA", 0.5));
      effectDefinition.Parameters.Add(new Parameter("RotateA", "right"));
      effectDefinition.Parameters.Add(new Parameter("ExponentialProgressScale", 0.5));

      return effectDefinition;
    }
  }
}