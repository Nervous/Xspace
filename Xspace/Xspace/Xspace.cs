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
        private Texture2D textureVaisseau_joueur, textureMissile_joueur_base;
        private Song musique;
        private KeyboardState keyboardState;
        // TODO : Déclaration de tous les objets Vaisseau en dessous
        private Vaisseau_joueur joueur1;
        // TODO : Déclaration de tous les objets missiles en dessous
        private Missiles missileJoueur;
        int lastCheckedTime, lastTime;
        
        public Xspace()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            lastTime = 0;
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
            textureMissile_joueur_base = Content.Load<Texture2D>("MissileJoueur_Base");

            // TODO : Chargement de tous les objets vaisseau en dessous
            joueur1 = new Vaisseau_joueur(textureVaisseau_joueur);

            // TODO : Chargement de tous les objets missiles en dessous
            missileJoueur = new Missiles(textureMissile_joueur_base);
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {

            float fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            lastTime = 0;
            fond_ecran.Update(fps_fix);
            joueur1.Update(fps_fix); // Update du joueur
            // <=== Update des précédents missiles ici 
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                missileJoueur.afficherMissile(joueur1.position);
            }

            // if ((int)gameTime.ElapsedGameTime.TotalMilliseconds - lastTime > 500)
            //{
                missileJoueur.avancerMissile();
            //   lastTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds - lastTime;
            //}
            

               

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            fond_ecran.Draw(spriteBatch);
            joueur1.Draw(spriteBatch); // Draw du joueur
            missileJoueur.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
