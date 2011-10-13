using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace Xspace
{
    public static class Helper
    {
        // un objet Random
        private static readonly Random _rnd = new Random();

        // Retourne un vecteur de direction aléatoire, en fait tourner un vecteur unitaire selon un angle aléatoire
        public static Vector2 GetRandomVector()
        {
            return Vector2.Transform(Vector2.UnitX,
                                     Quaternion.CreateFromYawPitchRoll(0, 0, (float)(_rnd.NextDouble() * MathHelper.TwoPi)));
        }

        // Idem, mais l'angle est borné entre a et b
        public static Vector2 GetRandomVector(float a, float b)
        {
            return Vector2.Transform(Vector2.UnitX,
                                     Quaternion.CreateFromYawPitchRoll(0, 0, (float)(GetRandomFloat(a, b) * MathHelper.TwoPi)));
        }

        // retourne un float aléatoire, compris entre from et to
        public static float GetRandomFloat(float from = 0, float to = 1)
        {
            return (float)((_rnd.NextDouble() * (to - from)) + from);
        }

        // mais wtf ? 
        public static Color Interpolate(Color a, Color b, float amount)
        {
            Color res = Color.Lerp(a, b, MathHelper.Clamp(amount * 1.5f, 0, 1));
            res.A = (byte)MathHelper.Lerp(0, 255, 1 - amount);
            return res;
        }
    }
}