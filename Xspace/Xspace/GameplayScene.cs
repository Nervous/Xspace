using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;

using MenuSample.Inputs;
using MenuSample.Scenes;
using MenuSample.Scenes.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

using Xspace;


namespace MenuSample.Scenes
{
    /// <summary>
    /// Le jeu!
    /// </summary>
    public class GameplayScene : AbstractGameScene
    {
        #region Déclaration variables usuelles
        private float fps_fix, _pauseAlpha;
        private double time, lastTime;
        private char[] delimitationFilesInfo = new char[] { ' ' }, delimitationFilesInfo2 = new char[] { ';' }, delimitationFilesInfo3 = new char[] { ':' };
        #endregion
        #region Déclaration variables relatives au jeu
        private doneParticles partManage;
        private ScrollingBackground fond_ecran;
        public SpriteBatch spriteBatch;
        private Texture2D T_Vaisseau_Joueur, T_Vaisseau_Drone, T_Missile_Joueur_1, T_Missile_Drone, T_Bonus_Vie, T_Bonus_Weapon1, barre_vie;
        private List<Texture2D> listeTextureVaisseauxEnnemis, listeTextureBonus;
        private Song musique, musique_menu;
        private SoundEffect musique_tir;
        private KeyboardState keyboardState;
        private gestionLevels thisLevel;
        private List<gestionLevels> infLevel;
        Renderer particleRenderer;
        ParticleEffect particleEffect;
        List<Vaisseau> listeVaisseau, listeVaisseauToRemove;
        List<Missiles> listeMissile, listeMissileToRemove;
        List<Bonus> listeBonus, listeBonusToRemove;
        private ContentManager _content;
        private SpriteFont _gameFont;
        #endregion
        #region Déclaration structures relatives au jeu
        struct doneParticles
        {
            public bool _done;
            public Vector2 startingParticle;

            public doneParticles(bool done, Vector2 pos)
            {
                _done = done;
                startingParticle = pos;
            }
        };
        #endregion

        private readonly Random _random = new Random();
        private AffichageInformations HUD = new AffichageInformations();
        public GameplayScene(SceneManager sceneMgr, GraphicsDeviceManager graphics)
            : base(sceneMgr)
        {

            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = graphics
            };
            particleEffect = new ParticleEffect();

            lastTime = 0;
        }

        public override void Initialize()
        {
		    base.Initialize();
        }
        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Thread.Sleep(500);
            SceneManager.Game.ResetElapsedTime();

            #region Chargement musiques & sons
            musique = _content.Load<Song>("Musiques\\Jeu\\Musique");
            musique_menu = _content.Load<Song>("Musiques\\Menu\\Musique");
            musique_tir = _content.Load<SoundEffect>("Sons\\Tir\\Tir");
            MediaPlayer.Play(musique);
            #endregion
            #region Chargement des polices d'écritures
            _gameFont = _content.Load<SpriteFont>("Fonts\\Menu\\Menu");
            #endregion
            #region Chargement fond du jeu
            fond_ecran = new ScrollingBackground();
            fond_ecran.Load(GraphicsDevice, _content.Load<Texture2D>("Sprites\\Background\\Background"));
            #endregion
            #region Chargement particules
            particleRenderer.LoadContent(_content);
            particleEffect = _content.Load<ParticleEffect>("Collisions\\BasicExplosion\\BasicExplosion");
            particleEffect.LoadContent(_content);
            particleEffect.Initialise();
            #endregion
            #region Chargement textures vaisseaux
            T_Vaisseau_Joueur = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\Vaisseau1");
            if (true)
            {
                T_Vaisseau_Drone = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\drone");
            }
            else
            {
                WebClient wc = new WebClient();

                wc.DownloadFile("http://nathalie.bouquet.free.fr/epita/trombi2011-12/sup/pruvot_a.jpg", "Content\\Sprites\\Vaisseaux\\logintmp.jpg");
                T_Vaisseau_Drone = Texture2D.FromStream(GraphicsDevice, new FileStream("Content\\Sprites\\Vaisseaux\\logintmp.jpg", FileMode.Open));
            }
            #endregion

            #region Chargement textures missiles
            T_Missile_Joueur_1 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\Missile1");
            T_Missile_Drone = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\Drone");
            #endregion
            #region Chargement textures bonus
            // TODO : Chargement de toutes les textures des bonus en dessous
            T_Bonus_Vie = _content.Load<Texture2D>("Sprites\\Bonus\\Life");
            T_Bonus_Weapon1 = _content.Load<Texture2D>("Sprites\\Bonus\\DoubleBaseWeapon");
            #endregion
            #region Chargement vaisseaux
            listeVaisseau = new List<Vaisseau>();
            listeVaisseauToRemove = new List<Vaisseau>();
            listeVaisseau.Add(new Vaisseau_joueur(T_Vaisseau_Joueur));
            #endregion
            #region Chargement missiles
            listeMissile = new List<Missiles>();
            listeMissileToRemove = new List<Missiles>();
            #endregion
            #region Chargement bonus
            listeBonus = new List<Bonus>();
            listeBonusToRemove = new List<Bonus>();
            #endregion
            #region Chargement du level
            listeTextureVaisseauxEnnemis = new List<Texture2D>();
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Drone);
            listeTextureBonus = new List<Texture2D>();
            listeTextureBonus.Add(T_Bonus_Vie);
            listeTextureBonus.Add(T_Bonus_Weapon1);
            thisLevel = new gestionLevels(0, listeTextureVaisseauxEnnemis, listeTextureBonus);
            infLevel = new List<gestionLevels>();
            thisLevel.readInfos(delimitationFilesInfo, delimitationFilesInfo2, delimitationFilesInfo3, infLevel);
            #endregion
            #region Chargement barre de vie
            barre_vie = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\barre-vie-test");
            #endregion
        }

        doneParticles collisions(List<Vaisseau> listeVaisseau, List<Missiles> listeMissile, List<Bonus> listeBonus, float spentTime, ParticleEffect particleEffect, GameTime gametime)
        {
            foreach(Vaisseau vaisseau in listeVaisseau)
            {
                #region Collision joueur => bonus
                foreach (Bonus bonus in listeBonus)
                {
                    if (((listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width >= bonus.position.X && listeVaisseau[0].position.X <= bonus.position.X) ||
                                (listeVaisseau[0].position.X <= bonus.position.X + bonus.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width >= bonus.position.X + bonus.sprite.Width) ||
                                (listeVaisseau[0].position.X <= bonus.position.X && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > bonus.position.X + bonus.sprite.Width))
                           && ((listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height >= bonus.position.Y && listeVaisseau[0].position.Y <= bonus.position.Y) ||
                                (listeVaisseau[0].position.Y <= bonus.position.Y + bonus.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height >= bonus.position.Y + bonus.sprite.Height) ||
                                (listeVaisseau[0].position.Y <= bonus.position.Y && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > bonus.position.Y + bonus.sprite.Height)))
                    {
                        if (!bonus.disabled)
                        {
                            listeVaisseau[0].applyBonus(bonus.effect, bonus.ammount, bonus.time);
                            bonus.disabled = true;
                        }
                        listeBonusToRemove.Add(bonus);
                    }
                }
                #endregion
                foreach (Missiles missile in listeMissile)
                {
                    #region Collision missile => vaisseau
                    if (((missile.position.X + missile.sprite.Width > vaisseau.position.X)
                        && (missile.position.X + missile.sprite.Width < vaisseau.position.X + vaisseau.sprite.Width))
                        && ((missile.position.Y + missile.sprite.Height / 2 > vaisseau.position.Y - vaisseau.sprite.Height*0.10)
                        && (missile.position.Y + missile.sprite.Height / 2 < vaisseau.position.Y + vaisseau.sprite.Height + vaisseau.sprite.Height*0.10))
                        )
                    {  // Collision missile => Vaisseau trouvée
                                
                        if ((vaisseau.ennemi && !missile.ennemi) || (!vaisseau.ennemi && missile.ennemi))
                        {
                            listeMissileToRemove.Add(missile);

                            if (vaisseau.hurt(missile.degats))
                            {
                                // Vaisseau dead
                                            
                                vaisseau.kill();
                                return new doneParticles(false, new Vector2(vaisseau.position.X + vaisseau.sprite.Width / 2, vaisseau.position.Y + vaisseau.sprite.Height / 2));        
                            }
                        }
                    }
                    #endregion
                    #region Collision joueur <=> vaisseau 
                    if ( ( (listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > vaisseau.position.X && listeVaisseau[0].position.X < vaisseau.position.X) ||
                                (listeVaisseau[0].position.X < vaisseau.position.X + vaisseau.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > vaisseau.position.X + vaisseau.sprite.Width))
                           && (  (listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > vaisseau.position.Y && listeVaisseau[0].position.Y < vaisseau.position.Y) ||
                                 (listeVaisseau[0].position.Y < vaisseau.position.Y + vaisseau.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > vaisseau.position.Y + vaisseau.sprite.Height)))
                    {
                        // Collision entre vaisseau joueur & ennemi trouvée
                        vaisseau.kill();
                        listeVaisseauToRemove.Add(vaisseau);
                        listeVaisseau[0].hurt(vaisseau.damageCollision);
                        return new doneParticles(false, new Vector2(vaisseau.position.X + vaisseau.sprite.Width/2, vaisseau.position.Y + vaisseau.sprite.Height / 2));
                    }
                    #endregion
                   

                }
            }
            return new doneParticles(true, new Vector2(0, 0));
        }

        public override void HandleInput()
        {
            KeyboardState keyboardState = InputState.CurrentKeyboardState;
            if (InputState.IsPauseGame())
                new PauseMenuScene(SceneManager, this).Add();
        }

        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {
            keyboardState = Keyboard.GetState();
            _pauseAlpha = coveredByOtherscene ? Math.Min(_pauseAlpha + 1f / 32, 1) : Math.Max(_pauseAlpha - 1f / 32, 0);

            fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            base.Update(gameTime, othersceneHasFocus, false);
            fond_ecran.Update(fps_fix);
            #region Gestion de la musique en cas de pause
            if (InputState.IsPauseGame())
            {
                MediaPlayer.Volume = 0.2f;
            }
            else if (InputState.IsMenuSelect())
                MediaPlayer.Volume = 1f;
            #endregion
            #region Gestion des évenements du level
            foreach (gestionLevels spawn in infLevel)
            {
                if (spawn.isTime(time))
                {
                    switch (spawn.Categorie)
                    {
                        case "vaisseau":
                            listeVaisseau.Add(spawn.Adresse);
                            break;
                        case "bonus":
                            listeBonus.Add(spawn.bonus);
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion
            #region Gestion des tirs du joueur
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                switch (listeVaisseau[0].armeActuelle)
                {
                    case 0:
                        if (time - lastTime > 150 || lastTime == 0)
                        {
                            musique_tir.Play();
                            Vector2 spawn = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 6);
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn));
                            lastTime = time;
                        }
                        break;
                    case 1:
                        if (time - lastTime > 150 || lastTime == 0)
                        {
                            musique_tir.Play();
                            Vector2 spawn1 = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 6 - 15);
                            Vector2 spawn2 = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 6 + 15);
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn1));
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn2));
                            lastTime = time;
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region Update des missiles
            foreach (Missiles missile in listeMissile)
            {
                if (missile.position.X < 1150 && !missile.ennemi)
                    missile.avancerMissile(fps_fix);
                else if (missile.position.X > 0 && missile.ennemi)
                    missile.avancerMissile(fps_fix);
                else
                    listeMissileToRemove.Add(missile);
            }

            foreach (Missiles missile in listeMissileToRemove)
            {
                listeMissile.Remove(missile);
            }

            listeMissileToRemove.Clear();
            #endregion
            #region Update des vaisseaux
            foreach (Vaisseau vaisseau in listeVaisseau)
            {
                if (vaisseau.existe == false)
                    listeVaisseauToRemove.Add(vaisseau);
                else if (vaisseau.ennemi)
                    vaisseau.Update(fps_fix);
                else
                    vaisseau.Update(fps_fix, keyboardState);

                if (vaisseau.ennemi && vaisseau.existe)
                {
                    if (time - vaisseau.lastTir > vaisseau.timingAttack)
                    {
                        Vector2 spawn = new Vector2(vaisseau.position.X - 35, vaisseau.position.Y + vaisseau._textureVaisseau.Height / 3 - 6);
                        // FAIRE EN FONCTION DU TYPE DE MISSILE
                        listeMissile.Add(new Missile_drone(T_Missile_Drone, spawn));
                        vaisseau.lastTir = time;
                    }
                } 
            }

            foreach (Vaisseau vaisseau in listeVaisseauToRemove)
            {
                listeVaisseau.Remove(vaisseau);
            }
            
            listeVaisseauToRemove.Clear();
            #endregion
            #region Update des bonus
            foreach (Bonus bonus in listeBonus)
            {
                if (bonus.position.X > 0)
                    bonus.Update(fps_fix);
                else
                    listeBonusToRemove.Add(bonus);
            }

            foreach (Bonus bonus in listeBonusToRemove)
                listeBonus.Remove(bonus);
            #endregion
            #region Collisions & Update des particules
            if (!(partManage.startingParticle == Vector2.Zero))
                particleEffect.Trigger(partManage.startingParticle);
            partManage = collisions(listeVaisseau, listeMissile, listeBonus, fps_fix, particleEffect, gameTime);

            particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion



            if (listeVaisseau[0].ennemi)
                Remove();
            // Game terminée
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = SceneManager.SpriteBatch;

            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 0, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            HUD.Drawbar(spriteBatch, barre_vie, listeVaisseau[0].vie, listeVaisseau[0].vieMax);
            
            if(listeVaisseau[0].vie > 0)
                spriteBatch.DrawString(_gameFont, Convert.ToString(listeVaisseau[0].vie) + "/" + Convert.ToString(listeVaisseau[0].vieMax), new Vector2(500,450), Color.Red);
            #region Draw du fond
            fond_ecran.Draw(spriteBatch);
            #endregion
            #region Draw des vaisseaux
            foreach (Vaisseau vaisseau in listeVaisseau)
            {
                vaisseau.Draw(spriteBatch);
            }
            #endregion
            #region Draw des missiles
            foreach (Missiles sMissile in listeMissile)
            {
                sMissile.Draw(spriteBatch);
            }
            #endregion
            #region Draw des bonus
            foreach (Bonus bonus in listeBonus)
            {
                bonus.Draw(spriteBatch);
            }
            #endregion
            #region Draw du menu de pause
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                //SceneManager.FadeBackBufferToBlack(alpha);
            }
            base.Draw(gameTime);
            #endregion
            particleRenderer.RenderEffect(particleEffect);

            spriteBatch.End();    
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }
    }
}
