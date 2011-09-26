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
        Vaisseau_ennemi[] listeVaisseauEnnemi;
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
            listeVaisseauEnnemi = new Vaisseau_ennemi[100];
            joueur1 = new Vaisseau_joueur(textureVaisseau_joueur);
            drone1 = new Vaisseau_ennemi(textureVaisseau_joueur, "drone");
            drone1.creer();
            listeVaisseauEnnemi[0] = drone1;
            

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


        bool collisions(Vaisseau_ennemi[] listeVaisseau, Missiles[] listeMissiles) // TODO : Remplacer par Missiles[][] listeMissiles !!
        {
            /* Ne gère QUE les collisions vaisseau / missile, pour le moment.
             * Pour que cette fonction s'éxecute correctement, il faut absolument que les tableaux soient ordonnés de la sorte que toutes les cases
             * possédant un objet soient au début, et ainsi que, dès que la fonction rencontre une case vide, elle puisse s'arrêter. */
            for (int i = 0; i < 100; i++)
            {
                if (listeVaisseau[i] == null) // On  a checké tous les vaisseaux, done.
                    break;
                else
                {
                    for (int j = 0; j < 200; j++)
                    {
                        if (listeMissiles[j] == null) // On a checké tous les missiles, done. TODO : Tableau de tableaux de missiles !!
                            break;
                        else // Sinon : On a bien un vaisseau existant et un missile existant, vérifions s'ils entrent en collision
                        {
                            if (((listeMissiles[j].position.X + listeMissiles[j].sprite.Width > listeVaisseau[i].position.X) && (listeMissiles[j].position.X + listeMissiles[j].sprite.Width < listeVaisseau[i].position.X + listeVaisseau[i].position.X)) && ((listeMissiles[j].position.Y + listeMissiles[j].sprite.Height > listeVaisseau[i].position.Y) && (listeMissiles[j].position.Y + listeMissiles[j].sprite.Height < listeVaisseau[i].position.Y + listeVaisseau[i].position.Y)))
                            {
                                // Colision entre le vaisseau i et le missile j !
                                Exit();
                            }
                        }
                    }
                }
            }

            return true;
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
