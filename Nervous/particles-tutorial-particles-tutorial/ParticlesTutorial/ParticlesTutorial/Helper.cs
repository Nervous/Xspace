using System;
using Microsoft.Xna.Framework;

namespace ParticlesTutorial
{
    public static class Helper
    {
        private static readonly Random _rnd = new Random();

        public static Vector2 GetRandomVector()
        {
            return Vector2.Transform(Vector2.UnitX,
                                     Quaternion.CreateFromYawPitchRoll(0, 0, (float)(_rnd.NextDouble() * MathHelper.TwoPi)));
        }

        public static Vector2 GetRandomVector(float a, float b)
        {
            return Vector2.Transform(Vector2.UnitX,
                                     Quaternion.CreateFromYawPitchRoll(0, 0, (float)(GetRandomFloat(a, b) * MathHelper.TwoPi)));
        }

        public static float GetRandomFloat(float from = 0, float to = 1)
        {

            return (float) ((_rnd.NextDouble() * (to - from)) + from);
        }

        public static Color Interpolate(Color a, Color b, float amount)
        {
            Color res = Color.Lerp(a, b, MathHelper.Clamp(amount * 1.5f, 0, 1));
            res.A = (byte) MathHelper.Lerp(0, 255, 1 - amount);//(byte) (amount < 0.67f ? a.A : (a.A + (b.A - a.A)*(amount * 3)));
            return res;
        }
    }
}