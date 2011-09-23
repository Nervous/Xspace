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
        private Texture2D vaisseauJoueur;
        Vector2 emplacementJoueur, deplacementJoueurDirectionY;
        int ecranSizeX, ecranSizeY, temp_haut;
        float vitesseVaisseau;
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
            vaisseauJoueur = Content.Load<Texture2D>("Vaisseau");
            ecranSizeX = 800;
            ecranSizeY = 480;
            emplacementJoueur = new Vector2(20, ecranSizeY / 2 - vaisseauJoueur.Height / 2);
            deplacementJoueurDirectionY = Vector2.Normalize(new Vector2(0, 5));

            temp_haut = 1;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);


        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

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
            }

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
