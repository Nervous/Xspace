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
    public class ParticulesMgr : DrawableGameComponent
    {
        public Vector2 Position;
        private readonly ParticulesOptions _options;
        private readonly Xspace _game;
        private readonly List<Particule> _particulesListe;
        private Texture2D _texture_particule;
        public ParticulesOptions Options { get { return _options; } }

        public ParticulesMgr(Xspace game, ParticulesOptions options)
            : base(game)
        {
            _game = game;
            _options = options;
            _particulesListe = new List<Particule>();

            for (int i = 0; i < _options.ParticulesMax; i++)
                _particulesListe.Add(new Particule(this) { Vitesse = Vector2.One });
        }

        protected override void LoadContent()
        {
            _texture_particule = _game.Content.Load<Texture2D>("particle2");
        }

        public override void Update(GameTime gameTime)
        {
            var particuleMorte = _particulesListe.FirstOrDefault(particule => !particule.Active);
            if (particuleMorte != null)
                particuleMorte.Reset();

            foreach (var particule in _particulesListe)
                particule.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var particule in _particulesListe)
                particule.Draw(_game.spriteBatch, _texture_particule);
        }
    }
};
