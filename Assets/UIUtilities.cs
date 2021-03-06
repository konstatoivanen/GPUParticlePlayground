﻿using System.Collections.Generic;
using UnityEngine;

namespace Utilities.UI
{
    //Setting types
    public enum SettingType
    {
        Count,
        BrushSize,
        Timescale,
        Drag,
        BodyMass,
        BodyRadius,
        Elasticity,
        UnitScale,
        BoidMass,
        BoidMaxForce,
        RepelWeight,
        RepelDistance,
        CohesionWeight,
        CohesionCoef,
        CohesionDistance,
        AlignWeight,
        AlignCoef,
        AlignDistance,
    }

    //Event clas for when a setting change is submited
    public class SettingEvent
    {
        public SettingType type;
        public float value;

        public SettingEvent(SettingType type, float value)
        {
            this.type = type;
            this.value = value;
        }
    }

    //Interface for communicating with a setting instance.
    public interface ISetting
    {
        SettingType type { get; set; }
        System.Action<SettingEvent> OnSettingChange { get; set; }
        System.Action<SettingType> DisplayHint { get; set; }
        void Initialize(float value);
    }

    //Interface for communicating with RootRenderer.
    public interface ISimulationSettings
    {
        bool AllowInput { get; set; }
        float BrushSize { get; set; }
        BoidSimParams SimulationParameters { get; }
        void SetBoidCount(int count);
    }

    //Utility class for convenient data access. 
    //This is in no way optimal but for this occasion it is fine.
    public static class SettingData
    {
        public static readonly Dictionary<SettingType, string> displayValues = new Dictionary<SettingType, string>()
        {
            {SettingType.Count,             "BOID COUNT :" },
            {SettingType.BrushSize,         "BRUSH SIZE :" },
            {SettingType.Timescale,         "TIME SCALE :" },
            {SettingType.Drag,              "DRAG :" },
            {SettingType.BodyMass,          "BODY MASS :" },
            {SettingType.BodyRadius,        "BODY RADIUS :" },
            {SettingType.Elasticity,        "ELASTICITY :" },
            {SettingType.UnitScale,         "UNIT SCALE :" },
            {SettingType.BoidMass,          "BOID MASS :" },
            {SettingType.BoidMaxForce,      "BOID MAX FORCE :" },
            {SettingType.RepelWeight,       "REPEL MULT :" },
            {SettingType.RepelDistance,     "REPEL DIST :" },
            {SettingType.CohesionWeight,    "COHESION MULT :" },
            {SettingType.CohesionCoef,      "COHESION COEF :" },
            {SettingType.CohesionDistance,  "COHESION DIST :" },
            {SettingType.AlignWeight,       "ALIGN MULT :" },
            {SettingType.AlignCoef,         "ALIGN COEF :" },
            {SettingType.AlignDistance,     "ALIGN DIST :" }
        };
        public static readonly Dictionary<SettingType, string> displayInfo = new Dictionary<SettingType, string>()
        {
            {SettingType.Count,             "Number of boids in the simulation (rounded to multiples of 32)." },
            {SettingType.BrushSize,         "Size of the normal painting brush in pixels." },
            {SettingType.Timescale,         "Time scale of the simulation." },
            {SettingType.Drag,              "Coefficient for how much boids resist movement over time." },
            {SettingType.BodyMass,          "Mass (to the power of 2 for convenience) of the gravity point that is applied when pressing Q or E" },
            {SettingType.BodyRadius,        "Radius of the gravity point that is applied when pressing Q or E" },
            {SettingType.Elasticity,        "Multiplier for perpendicular collision elasticity (aligned collisions have a runtime friction constant)." },
            {SettingType.UnitScale,         "Unit scale with which the distance to the gravity point is calculated with." },
            {SettingType.BoidMass,          "Mass of a single boid." },
            {SettingType.BoidMaxForce,      "Maximum amount of force the flocking algorithm can exert in one second." },
            {SettingType.RepelWeight,       "Multiplier for boid repelling force." },
            {SettingType.RepelDistance,     "Distance threshold where repelling is applied." },
            {SettingType.CohesionWeight,    "Multiplier for boid cohesion force." },
            {SettingType.CohesionCoef,      "Coefficient for cohesion/velocity substraction" },
            {SettingType.CohesionDistance,  "Distance threshold where cohesion is applied." },
            {SettingType.AlignWeight,       "Multiplier for boid alignment force." },
            {SettingType.AlignCoef,         "Coefficient for aligment/velocity substraction." },
            {SettingType.AlignDistance,     "Distance threshold where alignment is applied." }
        };

        public static readonly Dictionary<SettingType, Vector2> valueRanges = new Dictionary<SettingType, Vector2>()
        {
            {SettingType.Count,             new Vector2(32,     16000) },
            {SettingType.BrushSize,         new Vector2(16,     512) },
            {SettingType.Timescale,         new Vector2(0.1f,   32) },
            {SettingType.Drag,              new Vector2(0,      16) },
            {SettingType.BodyMass,          new Vector2(2,      10000000) },
            {SettingType.BodyRadius,        new Vector2(0,      32) },
            {SettingType.Elasticity,        new Vector2(0,      2) },
            {SettingType.UnitScale,         new Vector2(0.001f, 1.0f) },
            {SettingType.BoidMass,          new Vector2(2,      256) },
            {SettingType.BoidMaxForce,      new Vector2(0,      4096) },
            {SettingType.RepelWeight,       new Vector2(0,      512) },
            {SettingType.RepelDistance,     new Vector2(0,      256) },
            {SettingType.CohesionWeight,    new Vector2(0,      512) },
            {SettingType.CohesionCoef,      new Vector2(0,      256) },
            {SettingType.CohesionDistance,  new Vector2(0,      256) },
            {SettingType.AlignWeight,       new Vector2(0,      512) },
            {SettingType.AlignCoef,         new Vector2(0,      256) },
            {SettingType.AlignDistance,     new Vector2(0,      256) }
        };

    }
}