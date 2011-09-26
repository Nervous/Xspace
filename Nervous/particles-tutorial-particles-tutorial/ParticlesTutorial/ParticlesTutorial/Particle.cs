using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticlesTutorial
{
    public class Particle
    {
        private readonly ParticleSettings _settings;
        private readonly ParticlesMgr _mgr;

        public bool Alive;

        public Vector2 Pos { get; set; }
        public double LifeTime { get; set; }
        public Vector2 Velocity { get; set; }

        public Particle(ParticlesMgr mgr)
        {
            _settings = mgr.Settings;
            _mgr = mgr;
        }

        public void Reset()
        {
            Pos = _settings.Pos == null ? _mgr.Pos : _settings.Pos(_mgr.Pos);
            LifeTime = _settings.LifeTime;

            Alive = true;
        }

        public void Draw(SpriteBatch sb, Texture2D texture)
        {
            if (!Alive)
                return;

            var percent = (float)((_settings.LifeTime - LifeTime) / _settings.LifeTime);
            
            sb.Draw(texture, Pos, null, 
                Helper.Interpolate(_settings.ColorStart, _settings.ColorEnd,  percent),
                0, Vector2.Zero, MathHelper.Lerp(_settings.ScaleStart, _settings.ScaleEnd, percent), SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime)
        {
            if (!Alive)
                return;

            if (_settings.Velocity != null)
                Velocity = _settings.Velocity(Velocity, (_settings.LifeTime - LifeTime)/_settings.LifeTime);
            Pos += Velocity;

            LifeTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (LifeTime < 0)
                Alive = false;
        }
    }
}
