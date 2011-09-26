using System;
using Microsoft.Xna.Framework;

namespace ParticlesTutorial
{
    public struct ParticleSettings
    {
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }

        public int Max { get; set; }
        public double LifeTime { get; set; }
        public double AddFrequence { get; set; }
        public int ParticlesPerAdd { get; set; }

        public Func<Vector2, double, Vector2> Velocity { get; set; }
        public Func<Vector2, Vector2> Pos { get; set; }

        public float ScaleStart { get; set; }
        public float ScaleEnd { get; set; }

        public ParticleSettings(double lifeTime, Color colorStart, Color colorEnd, int max = 200, double addFrequence = 0, int particlesPerAdd = 1,
            Func<Vector2, double, Vector2> velocity = null, Func<Vector2, Vector2> pos = null, float scaleStart = 1, float scaleEnd = 1)
            : this()
        {
            ColorStart = colorStart;
            ColorEnd = colorEnd;
            Max = max;
            LifeTime = lifeTime;
            AddFrequence = addFrequence;
            Velocity = velocity;
            Pos = pos;
            ScaleStart = scaleStart;
            ScaleEnd = scaleEnd;
            ParticlesPerAdd = particlesPerAdd;
        }
    }
}