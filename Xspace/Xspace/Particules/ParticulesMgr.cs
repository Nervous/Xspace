using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private double _elapsed;


        public ParticulesMgr(Xspace game, ParticulesOptions options)
            : base(game)
        {
            _game = game;
            _options = options;
            _particulesListe = new List<Particule>();

            for (int i = 0; i < _options.ParticulesMax; i++)
                _particulesListe.Add(new Particule(this));
        }

        protected override void LoadContent()
        {
            _texture_particule = _game.Content.Load<Texture2D>("particle2");
        }

        public override void Update(GameTime gameTime)
        {

            // on ajoute le nombre de millisecondes écoulées depuis le dernier appel de la méthode Update
            _elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (var particule in _particulesListe)
                particule.Update(gameTime);

            // Le nombre de particules actives
            int nb = _options.ParticulesMax - _particulesListe.Count(p => p.Active);

            // Le nombre de particules à ajouter : ParticlesPerAdd si sa valeur est différente
            // de 0, le nombre de particules disponibles sinon
            int add = _options.ParticulesParAjout == 0 ? nb : _options.ParticulesParAjout;

            // Si on veut ajouter n particules, il faut s'assurer que n particules sont inactives et donc disponibles
            if (_options.ParticulesParAjout != 0 && nb < _options.ParticulesParAjout)
                return;

            // Si AddFrequence == 0, on réinitialise toutes les particules disponibles. La méthode Find lève
            // une exception si elle trouve pas d'élément correspondant, mais on sait que 'add' particules sont
            // disponibles.
            if (_options.AjoutFrequence == 0)
                for (int i = 0; i < add; i++)
                    _particulesListe.Find(p => !p.Active).Reset();
            else
                // sinon, on vérifie qu'un laps de temps suffisant s'est écoulé depuis le dernier ajout
                if (_elapsed > _options.AjoutFrequence && nb > 0)
                {
                    // Une utilisation de LinQ : On récupère 'add' particules parmi celles inactives
                    foreach (var particle in _particulesListe.Where(p => !p.Active).Take(add))
                        particle.Reset();
                    _elapsed = 0;
                }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var particule in _particulesListe)
            particule.Draw(_game.spriteBatch, _texture_particule);

        }
    }
};
