using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticlesTutorial
{
    public class ParticlesGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;

        public ParticlesGame()
        {
            graphics = new GraphicsDeviceManager(this)
                           {
                               PreferredBackBufferWidth = 1440,
                               PreferredBackBufferHeight = 900
                           };
            Content.RootDirectory = "Content";

            // RING
            var settingsExplosion = new ParticleSettings(3000, Color.Green, new Color(0, 1f, 0.8f, 0f),200, 0, 0,
                (v, t) => t == 0 ? Helper.GetRandomVector() * Helper.GetRandomFloat() : v,
                pos => pos + Helper.GetRandomVector() * 100);

            var explosion = new ParticlesMgr(this, settingsExplosion){Pos = new Vector2(1000, 200)};
            Components.Add(explosion);

            // EXPLOSION
            var settingsExplosion2 = new ParticleSettings(3000, Color.Orange, new Color(1, 0, 0, 0f), 200, 0, 0,
                (v, t) => t == 0 ? Helper.GetRandomVector() * Helper.GetRandomFloat(0.1f) : v,
                pos => pos);

            var explosion2 = new ParticlesMgr(this, settingsExplosion2) { Pos = new Vector2(1000, 600) };
            Components.Add(explosion2);


            // EXPLOSION
            var settingsExplosion3 = new ParticleSettings(2000, Color.WhiteSmoke, new Color(255, 105, 180, 0), 200, 800, 50,
                (v, t) => t == 0 ? Helper.GetRandomVector() : v,
                pos => pos);

            var explosion3 = new ParticlesMgr(this, settingsExplosion3) { Pos = new Vector2(300, 700) };
            Components.Add(explosion3);


            // BLUE FIRE
            var settingsFire = new ParticleSettings(1000, new Color(100, 147, 237, 255), new Color(0, 1f, 1f, 0f), 200, 30, 1,
                (v, t) => Vector2.UnitY * -5,
                pos => pos + Helper.GetRandomVector() * 10, 2, 0.4f);

            var fire = new ParticlesMgr(this, settingsFire) { Pos = new Vector2(600, 400) };
            Components.Add(fire);

            // BLUE FIRE
            var settingsFire2 = new ParticleSettings(1000, Color.White, new Color(0, 0.7f, 0.7f, 0f), 200, 1, 1,
                (v, t) => t == 0 ? Helper.GetRandomVector(0, MathHelper.Pi/ 20) * 5 : v,
                pos => pos, 0.4f, 2);

            var fire2 = new ParticlesMgr(this, settingsFire2) { Pos = new Vector2(600, 600) };
            Components.Add(fire2);

            // SMOKE
            var settingsSmoke = new ParticleSettings(5000, Color.White, new Color(128, 128, 128, 0), 200, 0, 1,
                (v, t) => t == 0 ? Helper.GetRandomVector() : v,
                pos => pos + Helper.GetRandomVector() * Helper.GetRandomFloat(5, 100), 1, 0.4f);

            var smoke = new ParticlesMgr(this, settingsSmoke) { Pos = new Vector2(400, 200) };
            Components.Add(smoke);
        }

        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
