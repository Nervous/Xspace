using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticlesTutorial
{
    public class ParticlesMgr : DrawableGameComponent
    {
        private readonly ParticlesGame _game;
        private readonly ParticleSettings _settings;
        private Texture2D _texture;
        private readonly List<Particle> _particles;
        public Vector2 Pos;

        private double _elapsed;

        public ParticlesMgr(ParticlesGame game, ParticleSettings settings)
            : base(game)
        {
            _game = game;
            _settings = settings;
            _particles = new List<Particle>();
            for (int i = 0; i < _settings.Max; i++)
                _particles.Add(new Particle(this));
        }

        public ParticleSettings Settings { get { return _settings; } }

        protected override void LoadContent()
        {
            _texture = _game.Content.Load<Texture2D>("particle2");
        }

        public override void Update(GameTime gameTime)
        {
            _elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (var particle in _particles)
                particle.Update(gameTime);

            int nb = _settings.Max - _particles.Count(p => p.Alive);
            int add = _settings.ParticlesPerAdd == 0 ? nb : _settings.ParticlesPerAdd;

            if (_settings.ParticlesPerAdd != 0 && nb < _settings.ParticlesPerAdd)
                return;

            if (_settings.AddFrequence == 0)
                for (int i = 0; i < add; i++)
                    _particles.Find(p => !p.Alive).Reset();
            else
                if(_elapsed > _settings.AddFrequence && nb > 0)
                {
                    foreach (var particle in _particles.Where(p => !p.Alive).Take(add))
                        particle.Reset();
                    _elapsed = 0;
                }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var particle in _particles)
                particle.Draw(_game.SpriteBatch, _texture);
        }
    }
}