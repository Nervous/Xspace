using System;
using System.Linq;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuSample.Scenes
{
    /// <summary>
    /// Un écran de chargement
    /// </summary>
    public class LoadingScene : AbstractGameScene
    {
        private readonly bool _loadingIsSlow;
        private bool _otherscenesAreGone;
        private readonly AbstractGameScene[] _scenesToLoad;

        /// <summary>
        /// Le constructeur est privé: le chargement des scènes
        /// doit être lancé via la méthode statique Load.
        /// </summary>
        private LoadingScene(SceneManager sceneMgr, bool loadingIsSlow, AbstractGameScene[] scenesToLoad)
            : base(sceneMgr)
        {
            _loadingIsSlow = loadingIsSlow;
            _scenesToLoad = scenesToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(2);
        }

        /// <summary>
        /// Active la scène de chargement.
        /// </summary>
        public static void Load(SceneManager sceneMgr, bool loadingIsSlow,
                                params AbstractGameScene[] scenesToLoad)
        {
            new LoadingScene(sceneMgr, loadingIsSlow, scenesToLoad).Add();
        }


        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {
            base.Update(gameTime, othersceneHasFocus, coveredByOtherscene);

            if (_otherscenesAreGone && !IsExiting)
            {
                Remove();
                foreach (AbstractGameScene scene in _scenesToLoad.Where(scene => scene != null))
                    scene.Add();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (TransitionPosition > 0)
                SceneManager.FadeBackBufferToBlack(TransitionAlpha);
            else
                SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            if (SceneState == SceneState.Active)
                _otherscenesAreGone = true;

            if (_loadingIsSlow)
            {
                SpriteBatch spriteBatch = SceneManager.SpriteBatch;
                SpriteFont font = SceneManager.Font;
                const string message = "Il y a bien longtemps... \nDans une galaxie lointaine, tres lointaine...";
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
}
