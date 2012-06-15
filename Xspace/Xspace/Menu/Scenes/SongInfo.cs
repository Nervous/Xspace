using System;
using System.Linq;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xspace;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Un écran de chargement
    /// </summary>
    public class SongInfo : AbstractGameScene
    {
        private string songname;
        private KeyboardState keyboardstate;
        private GraphicsDeviceManager graphics;
        private LoadSong song;
        private bool ready;
        /// <summary>
        /// Le constructeur est privé: le chargement des scènes
        /// doit être lancé via la méthode statique Load.
        /// </summary>
        private SongInfo(SceneManager sceneMgr, string songname, GraphicsDeviceManager graphics)
            : base(sceneMgr)
        {
            this.songname = songname;
            this.graphics = graphics;
            ready = false;
            song = new LoadSong(songname);
            keyboardstate = new KeyboardState();
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(2);
        }

        /// <summary>
        /// Active la scène de chargement.
        /// </summary>
        public static void Load(SceneManager sceneMgr, string songname, GraphicsDeviceManager graphics)
        {
            new SongInfo(sceneMgr, songname, graphics).Add();
        }


        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {
            base.Update(gameTime, othersceneHasFocus, coveredByOtherscene);
            keyboardstate = Keyboard.GetState();
            
            if (keyboardstate.IsKeyUp(Keys.Enter))
                ready = true;
            if (keyboardstate.IsKeyDown(Keys.Escape))
                Remove();
            if (ready && keyboardstate.IsKeyDown(Keys.Enter))
            {
                ready = false;
                Remove();
                LoadingScene.Load(SceneManager, true, new GameplayScene(SceneManager, graphics, 0, 0, GameplayScene.GAME_MODE.LIBRE, songname));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            SpriteFont font = SceneManager.Font;
            string message = "";
            message += "Titre : " + song.title + "\n";
            if (song.album != "")
                message += "Album : " + song.album + "\n";
            if (song.singer != "")
                message += song.singer;
            if (song.singer != "" && song.year != "")
                message += " - ";
            if (song.year != "")
                message += song.year;
            message += "\n\n";
            Viewport viewport = SceneManager.GraphicsDevice.Viewport;

            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            Color color = Color.White * TransitionAlpha;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.End();
        }
    }
}
