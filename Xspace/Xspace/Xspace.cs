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
    public class Xspace : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D textureVaisseau_joueur;
        private Song musique;
        private Vaisseau_joueur joueur1;
        
        public Xspace()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }


        protected override void Initialize()
        {
            base.Initialize();
        }

        private ScrollingBackground fond_ecran;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            musique = Content.Load<Song>("musique");
            MediaPlayer.Play(musique);
            fond_ecran = new ScrollingBackground();
            Texture2D fond_image = Content.Load<Texture2D>("space_bg");
            fond_ecran.Load(GraphicsDevice, fond_image);    

            // TODO : Chargement de toutes les textures des vaisseau en dessous
            textureVaisseau_joueur = Content.Load<Texture2D>("Vaisseau_joueur"); 

            // TODO : Chargement de toutes les textures des missiles en dessous

            // TODO : Chargement de tous les objets vaisseau en dessous
            joueur1 = new Vaisseau_joueur(textureVaisseau_joueur);

            // TODO : Chargement de tous les objets missiles en dessous
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            float fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            fond_ecran.Update(fps_fix); // Vitesse BG
            joueur1.Update(fps_fix); // Update du joueur

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            fond_ecran.Draw(spriteBatch);
            joueur1.Draw(spriteBatch); // Draw du joueur
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
