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
        private Vaisseau_ennemi drone1;
        // TODO : Déclaration de tous les objets missiles en dessous
        Missiles[] missileJoueur;
        int nbreMaxMissiles;
        float fps_fix;
        double time, lastTime;
        
        public Xspace()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            nbreMaxMissiles = 15;
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
            drone1 = new Vaisseau_ennemi(textureVaisseau_joueur, "drone");
            drone1.creer();
            

            // TODO : Chargement de tous les objets missiles en dessous
            missileJoueur = new Missiles[nbreMaxMissiles];
            for (int i = 0; i < nbreMaxMissiles; i++)
                missileJoueur[i] = new Missiles(textureMissile_joueur_base);

            if (missileJoueur[0] == null)
                Exit();
            for (int i = 0; i < nbreMaxMissiles - 1; i++)
            {
                if (missileJoueur[i] != null)
                    missileJoueur[i].initialiserTexture(textureMissile_joueur_base);
            }
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {

            fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            fond_ecran.Update(fps_fix);
            joueur1.Update(fps_fix); // Update du joueur
            // <=== Update des précédents missiles ici 
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                for (int i = 0; i < nbreMaxMissiles; i++)
                {
                    if (missileJoueur[i] != null && missileJoueur[i].estAffiche == false && (time - lastTime > 150 || lastTime == 0))
                    {
                            missileJoueur[i].afficherMissile(joueur1.position);
                            lastTime = time;
                            break;
                    }
                }
            }

            for (int i = 0; i < nbreMaxMissiles; i++)
            {
                if (missileJoueur[i] != null && missileJoueur[i].estAffiche)
                {
                    missileJoueur[i].avancerMissile(fps_fix);
                    //missileJoueur[i].checkCollisions
                }

            }
            

               

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            fond_ecran.Draw(spriteBatch);
            joueur1.Draw(spriteBatch); // Draw du joueur
            drone1.Draw(spriteBatch);
            for (int i = 0; i < nbreMaxMissiles - 1; i++)
            {
                if (missileJoueur[i] != null)
                    missileJoueur[i].Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
