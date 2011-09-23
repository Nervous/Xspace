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
    /// <summary>
    /// Welcome to Xspace code.
    /// </summary>
    public class Xspace : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D vaisseauJoueur;
        Song musique;
        Vector2 emplacementJoueur, deplacementJoueurDirectionY, deplacementJoueurDirectionX;
        int ecranSizeX, ecranSizeY, temp_haut;
        float vitesseVaisseau;
        private KeyboardState keyboardState;
        public Xspace()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            vitesseVaisseau = 0.45f;
        }


        protected override void Initialize()
        {
            base.Initialize();
            ecranSizeX = 800;
            ecranSizeY = 480;
            emplacementJoueur = new Vector2(20, ecranSizeY / 2 - vaisseauJoueur.Height / 2);
            deplacementJoueurDirectionY = Vector2.Normalize(new Vector2(0, 5));
            deplacementJoueurDirectionX = Vector2.Normalize(new Vector2(5, 0));
            temp_haut = 1;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            vaisseauJoueur = Content.Load<Texture2D>("Vaisseau");
            musique = Content.Load<Song>("musique");
            MediaPlayer.Play(musique);
            
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            keyboardState = Keyboard.GetState();

            // deplacements au clavier (sans limitations pour l'instant)
            if (keyboardState.IsKeyDown(Keys.Z))
            {
                emplacementJoueur -= deplacementJoueurDirectionY * vitesseVaisseau * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                emplacementJoueur += deplacementJoueurDirectionY * vitesseVaisseau * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (keyboardState.IsKeyDown(Keys.Q))
            {
                emplacementJoueur -= deplacementJoueurDirectionX * vitesseVaisseau * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                emplacementJoueur += deplacementJoueurDirectionX * vitesseVaisseau * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        
                /*
            if (temp_haut == 0)
            {
                emplacementJoueur += deplacementJoueurDirectionY * vitesseVaisseau * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (emplacementJoueur.Y > 430)
                    temp_haut = 1;
            }
            else
            {
                if (emplacementJoueur.Y < 21)
                    temp_haut = 0;
                emplacementJoueur -= deplacementJoueurDirectionY * vitesseVaisseau * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            } */

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(vaisseauJoueur, emplacementJoueur, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
