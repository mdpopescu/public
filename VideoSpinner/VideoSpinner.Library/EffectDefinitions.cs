﻿using System;
using Splicer.Timeline;

namespace Renfield.VideoSpinner.Library
{
  public static class EffectDefinitions
  {
// ReSharper disable InconsistentNaming
    public static readonly Guid WMTFX = new Guid("B4DC8DD9-2CC1-4081-9B2B-20D7030234EF");
// ReSharper restore InconsistentNaming

    public static EffectDefinition CreateEaseInEffect()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 0.5));
      definition.Parameters.Add(new Parameter("ScaleA", 1.5));
      definition.Parameters.Add(new Parameter("ExponentialProgressDuration", 0.01));

      return definition;
    }

    public static EffectDefinition CreateEaseOutEffect()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.5));
      definition.Parameters.Add(new Parameter("ScaleA", 0.5));
      definition.Parameters.Add(new Parameter("ExponentialProgressDuration", 0.01));

      return definition;
    }

    public static EffectDefinition PanUp()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "up"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static EffectDefinition PanDown()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "down"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static EffectDefinition PanLeft()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "left"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static EffectDefinition PanRight()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.00));
      definition.Parameters.Add(new Parameter("MoveA", "right"));
      definition.Parameters.Add(new Parameter("MoveSpeedA", 0.4));

      return definition;
    }

    public static EffectDefinition RotateAndZoomOut()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", "smaller"));
      definition.Parameters.Add(new Parameter("RotateA", "right"));
      definition.Parameters.Add(new Parameter("ExponentialProgressDuration", 1));

      return definition;
    }

    public static EffectDefinition FlipIn()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.0));
      definition.Parameters.Add(new Parameter("RotateA", "in"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 1));

      return definition;
    }

    public static EffectDefinition FlipOut()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.00));
      definition.Parameters.Add(new Parameter("ScaleA", 1.0));
      definition.Parameters.Add(new Parameter("RotateA", "out"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 1));

      return definition;
    }

    public static EffectDefinition PinWheelZoomIn()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 0.5));
      definition.Parameters.Add(new Parameter("ScaleA", 1.5));
      definition.Parameters.Add(new Parameter("RotateA", "right"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 0.5));

      return definition;
    }

    public static EffectDefinition PinWheelZoomOut()
    {
      var definition = new EffectDefinition(WMTFX);
      definition.Parameters.Add(new Parameter("InternalName", "Simple3D"));
      definition.Parameters.Add(new Parameter("InitialScaleA", 1.5));
      definition.Parameters.Add(new Parameter("ScaleA", 0.5));
      definition.Parameters.Add(new Parameter("RotateA", "right"));
      definition.Parameters.Add(new Parameter("ExponentialProgressScale", 0.5));

      return definition;
    }
  }
}