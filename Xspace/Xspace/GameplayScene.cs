using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

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
using System.Security.Cryptography;


namespace MenuSample.Scenes
{
    /// <summary>
    /// Le jeu!
    /// </summary>
    public class GameplayScene : AbstractGameScene
    {
        #region Déclaration variables usuelles
        private int score, _level;
        private float fps_fix, _pauseAlpha;
        private double time, lastTime, lastTimeSpectre, lastTimeEnergy, bossTime, compteur, lastTimeRandomSpawn, lastTimeMusic;
        private string path_level, stock_score_inferieur, stock_score_superieur;
        private string[] score_level;
        private StreamWriter sw_level;
        private StreamReader sr_level;
        private char[] delimitationFilesInfo = new char[] { ' ' }, delimitationFilesInfo2 = new char[] { ';' }, delimitationFilesInfo3 = new char[] { ':' };
        private float[] spectre;
        private bool drawSpectre, aBossWasThere, first;
        private float amplitude_sum_music;
        private Random r;
        private string song_path;
        #endregion
        #region Déclaration variables relatives au jeu
        private List<doneParticles> partManage;
        private ScrollingBackground fond_ecran, fond_ecran_front, fond_ecran_middle;
        public SpriteBatch spriteBatch;
        private Texture2D T_Vaisseau_Joueur, T_Vaisseau_Drone, T_Vaisseau_Kamikaze, T_Missile_Joueur_1, T_Missile_Drone, T_Bonus_Vie, T_Bonus_Weapon1, T_Obstacles_Hole, barre_vie, T_HUD, T_HUD_boss, T_HUD_bars, T_HUD_bar_boss, T_Divers_Levelcomplete, T_Divers_Levelfail, T_boss1, T_Vaisseau_Energizer, T_Vaisseau_Doubleshooter, T_Missile_Energie;
        private List<Texture2D> listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss;
        private SoundEffect musique_tir, musique_bossExplosion;
        private KeyboardState keyboardState;
        bool lastKeyDown = true, end = false, endDead = false;
        private gestionLevels thisLevel;
        private List<gestionLevels> infLevel, listeLevelToRemove;
        Renderer particleRenderer;
        ParticleEffect particleEffect, particleEffectMoteur, particleEffectBoss1, particleBossExplosion;
        private Boss boss1;
        List<Vaisseau> listeVaisseau, listeVaisseauToRemove;
        List<Missiles> listeMissile, listeMissileToRemove;
        List<Bonus> listeBonus, listeBonusToRemove;
        List<Obstacles> listeObstacles, listeObstaclesToRemove;
        private ContentManager _content;
        private SpriteFont _gameFont, _ingameFont, _HUDfont;
        private GAME_MODE mode;
        private Vector2 position_spawn;
        private BEAT_SPAWNED beat_spawned;
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

        public enum GAME_MODE
        {
            CAMPAGNE,
            EXTREME,
            LIBRE,
            COOP,
        }
        public enum BEAT_SPAWNED
        {
            NO_BEAT,
            NOTHING,
            BONUS,
            ENEMY
        }
        #endregion

        private readonly Random _random = new Random();
        private AffichageInformations HUD = new AffichageInformations();
        
        
        public GameplayScene(SceneManager sceneMgr, GraphicsDeviceManager graphics, int level, int act, GAME_MODE mode, string song_path = "fat1.wav")
            : base(sceneMgr)
        {

            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = graphics
            };
            particleEffect = new ParticleEffect();

            _level = level + (act-1) * 3;
            score = 0;
            lastTime = 0;
            lastTimeEnergy = 0;
            first = true;
            lastTimeSpectre = 150;
            lastTimeRandomSpawn = 0;
            lastTimeMusic = 0;
            spectre = new float[128];
            SoundEffect.MasterVolume = 0.15f;
            this.mode = mode;
            this.song_path = song_path;
            position_spawn = new Vector2();
            beat_spawned = BEAT_SPAWNED.NO_BEAT;
        }

        public override void Initialize()
        {
		    base.Initialize();
            aBossWasThere = false;
            bossTime = 0;
            partManage = new List<doneParticles>();
        }

        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Thread.Sleep(50);
            SceneManager.Game.ResetElapsedTime();
            #region Chargement musiques & sons
            musique_tir = _content.Load<SoundEffect>("Sons\\Tir\\Tir");
            musique_bossExplosion = _content.Load<SoundEffect>("Sons\\BossExplosion");

            MD5CryptoServiceProvider md5crypto = new MD5CryptoServiceProvider();
            Stream s = (Stream)new FileStream("Musiques\\Jeu\\" + song_path, FileMode.Open);
            byte[] music_md5_bytes = md5crypto.ComputeHash(s);
            string music_md5 = Encoding.ASCII.GetString(music_md5_bytes);
            int music_md5_seed = BitConverter.ToInt32(music_md5_bytes, 0);
            s.Close();

            r = new Random(music_md5_seed);

            int loop = (mode == GAME_MODE.LIBRE) ? 0 : -1;
            AudioPlayer.PlayMusic("Musiques\\Jeu\\" + song_path, loop, true);
            AudioPlayer.SetVolume(1f);

            BeatDetector.Initialize();
            BeatDetector.audio_process();
            AudioPlayer.PauseMusic();

            #endregion
            #region Chargement des polices d'écritures
            _gameFont = _content.Load<SpriteFont>("Fonts\\Menu\\Menu");
            _ingameFont = _content.Load<SpriteFont>("Fonts\\Jeu\\Jeu");
            _HUDfont = _content.Load<SpriteFont>("Fonts\\Jeu\\HUD");
            #endregion
            #region Chargement fond du jeu
            fond_ecran = new ScrollingBackground();
            fond_ecran.Load(GraphicsDevice, _content.Load<Texture2D>("Sprites\\Background\\space"));

            fond_ecran_middle = new ScrollingBackground();
            fond_ecran_middle.Load(GraphicsDevice, _content.Load<Texture2D>("Sprites\\Background\\space-middle"));

            fond_ecran_front = new ScrollingBackground();
            fond_ecran_front.Load(GraphicsDevice, _content.Load<Texture2D>("Sprites\\Background\\space-front"));
            #endregion
            #region Chargement particules
            //Moteur
            particleRenderer.LoadContent(_content);
            particleEffectMoteur = _content.Load<ParticleEffect>("Collisions\\Moteur\\Moteurlocal");
            particleEffectMoteur.Initialise();
            particleEffectMoteur.LoadContent(_content);
            //Boss1
            particleEffectBoss1 = _content.Load<ParticleEffect>("Collisions\\Moteur\\Boss1");
            particleEffectBoss1.Initialise();
            particleEffectBoss1.LoadContent(_content);
            //Explosions
            particleEffect = _content.Load<ParticleEffect>("Collisions\\BasicExplosion\\BasicExplosion");
            particleEffect.Initialise();
            particleEffect.LoadContent(_content);
            particleBossExplosion = _content.Load<ParticleEffect>("Collisions\\BasicExplosion\\BossExplosion");
            particleBossExplosion.Initialise();
            particleBossExplosion.LoadContent(_content);
            #endregion
            #region Chargement textures vaisseaux
            T_Vaisseau_Joueur = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\Joueur_1");
            T_Vaisseau_Drone = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\Drone");
            T_Vaisseau_Kamikaze = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\Kamikaze");
            T_Vaisseau_Energizer = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\energizer");
            T_Vaisseau_Doubleshooter = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\doubleshooter");
			
			drawSpectre = false;

            /* WebClient wc = new WebClient();
            wc.DownloadFile("http://nathalie.bouquet.free.fr/epita/trombi2011-12/sup/login_x.jpg", "Content\\Sprites\\Vaisseaux\\logintmp.jpg");
            T_Vaisseau_Drone = Texture2D.FromStream(GraphicsDevice, new FileStream("Content\\Sprites\\Vaisseaux\\logintmp.jpg", FileMode.Open)); */

            #endregion
            #region Chargement textures boss
            T_boss1 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss1");
            #endregion 
            #region Chargement textures HUD
            if(mode == GAME_MODE.EXTREME)
                T_HUD = _content.Load<Texture2D>("Sprites\\HUD\\interface-extreme");
            else
                T_HUD = _content.Load<Texture2D>("Sprites\\HUD\\interface");
            T_HUD_boss = _content.Load<Texture2D>("Sprites\\HUD\\interface-boss");
            T_HUD_bars = _content.Load<Texture2D>("Sprites\\HUD\\energyBars1");
            T_HUD_bar_boss = _content.Load<Texture2D>("Sprites\\HUD\\energyBarsBoss");
            #endregion
            #region Chargement textures missiles
            T_Missile_Joueur_1 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\missilenew1");
            T_Missile_Drone = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            T_Missile_Energie = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
            #endregion
            #region Chargement textures bonus
            // TODO : Chargement de toutes les textures des bonus en dessous
            T_Bonus_Vie = _content.Load<Texture2D>("Sprites\\Bonus\\Life");
            T_Bonus_Weapon1 = _content.Load<Texture2D>("Sprites\\Bonus\\DoubleBaseWeapon");
            #endregion
            #region Chargement textures obstacles
            T_Obstacles_Hole = _content.Load<Texture2D>("Sprites\\Obstacles\\Hole");
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
            #region Chargement obstacles
            listeObstacles = new List<Obstacles>();
            listeObstaclesToRemove = new List<Obstacles>();
            #endregion
            #region Chargement du level
            listeTextureVaisseauxEnnemis = new List<Texture2D>();
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Drone);
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Kamikaze);
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Energizer);
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Doubleshooter);

            listeTextureBonus = new List<Texture2D>();
            listeTextureBonus.Add(T_Bonus_Vie);
            listeTextureBonus.Add(T_Bonus_Weapon1);

            listeTextureObstacles = new List<Texture2D>();
            listeTextureObstacles.Add(T_Obstacles_Hole);

            listeTextureBoss = new List<Texture2D>();
            listeTextureBoss.Add(T_boss1);

            thisLevel = new gestionLevels(_level, listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss);
            infLevel = new List<gestionLevels>();
            listeLevelToRemove = new List<gestionLevels>();
            thisLevel.readInfos(delimitationFilesInfo, delimitationFilesInfo2, delimitationFilesInfo3, infLevel);
            #endregion
            #region Chargement barre de vie
            barre_vie = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\barre-vie-test1");
            #endregion
            #region Chargement textures boss
            T_boss1 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss1"); 


            #endregion
            #region Chargement fin level
            T_Divers_Levelcomplete = _content.Load<Texture2D>("Sprites\\Divers\\levelcompleted");
            T_Divers_Levelfail = _content.Load<Texture2D>("Sprites\\Divers\\gameover");
            #endregion
        }

        List<doneParticles> collisions(List<Vaisseau> listeVaisseau, List<Missiles> listeMissile, List<Bonus> listeBonus, List<Obstacles> listeObstacles, Boss aBoss, float spentTime, ParticleEffect particleEffect, GameTime gametime, bool dead)
        {
            List<doneParticles> listeParticules = new List<doneParticles>();
            #region Collision joueur <=> boss
            if (aBoss != null && !dead && ((listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > aBoss.Position.X && listeVaisseau[0].position.X < aBoss.Position.X) ||
            (listeVaisseau[0].position.X < aBoss.Position.X + aBoss.Texture.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > aBoss.Position.X + aBoss.Texture.Width))
        && ((listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > aBoss.Position.Y && listeVaisseau[0].position.Y < aBoss.Position.Y) ||
                (listeVaisseau[0].position.Y < aBoss.Position.Y + aBoss.Texture.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > aBoss.Position.Y + aBoss.Texture.Height)))
            {
                aBoss.Hurt(10);
                listeVaisseau[0].hurt(10, time);
                if (listeVaisseau[0].vie < 0)
                    listeVaisseauToRemove.Add(listeVaisseau[0]);

                if ((!end) && (!endDead) && aBoss.Existe && aBoss.Hurt(listeVaisseau[0].damageCollision))
                {
                    score = score + aBoss.Score;
                    aBoss.Kill();
                }
            }
            #endregion
            foreach(Vaisseau vaisseau in listeVaisseau)
            {
                #region Collision joueur => bonus
                foreach (Bonus bonus in listeBonus)
                {
                    if (!dead && ((listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width >= bonus.position.X && listeVaisseau[0].position.X <= bonus.position.X) ||
                                (listeVaisseau[0].position.X <= bonus.position.X + bonus.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width >= bonus.position.X + bonus.sprite.Width) ||
                                (listeVaisseau[0].position.X <= bonus.position.X && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > bonus.position.X + bonus.sprite.Width))
                           && ((listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height >= bonus.position.Y && listeVaisseau[0].position.Y <= bonus.position.Y) ||
                                (listeVaisseau[0].position.Y <= bonus.position.Y + bonus.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height >= bonus.position.Y + bonus.sprite.Height) ||
                                (listeVaisseau[0].position.Y <= bonus.position.Y && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > bonus.position.Y + bonus.sprite.Height)))
                    {
                        if (!bonus.disabled)
                        {
                            listeVaisseau[0].applyBonus(bonus.effect, bonus.amount, bonus.time);
                            bonus.disabled = true;
                        }
                        listeBonusToRemove.Add(bonus);
                    }
                }
                #endregion
                #region Collision joueur <=> vaisseau
                if (!dead && ((listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > vaisseau.position.X && listeVaisseau[0].position.X < vaisseau.position.X) ||
                            (listeVaisseau[0].position.X < vaisseau.position.X + vaisseau.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > vaisseau.position.X + vaisseau.sprite.Width))
                       && ((listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > vaisseau.position.Y && listeVaisseau[0].position.Y < vaisseau.position.Y) ||
                             (listeVaisseau[0].position.Y < vaisseau.position.Y + vaisseau.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > vaisseau.position.Y + vaisseau.sprite.Height)))
                {
                    // Collision entre vaisseau joueur & ennemi trouvée
                    vaisseau.kill();

                    if ((!end) && (!endDead))
                        score = score + vaisseau.score;

                    listeVaisseauToRemove.Add(vaisseau);
                    listeVaisseau[0].hurt(vaisseau.damageCollision, time);
                    if (listeVaisseau[0].vie < 0)
                        listeVaisseauToRemove.Add(listeVaisseau[0]);
                    listeParticules.Add(new doneParticles(false, new Vector2(vaisseau.position.X + vaisseau.sprite.Width / 2, vaisseau.position.Y + vaisseau.sprite.Height / 2)));
                }
                #endregion
                foreach (Missiles missile in listeMissile)
                {
                    #region Collision missile => vaisseau
                    if (!dead && missile.isOwner(vaisseau, null) && ((missile.position.X + missile.sprite.Width > vaisseau.position.X)
                        && (missile.position.X + missile.sprite.Width < vaisseau.position.X + vaisseau.sprite.Width))
                        && ((missile.position.Y + missile.sprite.Height / 2 > vaisseau.position.Y - vaisseau.sprite.Height*0.10)
                        && (missile.position.Y + missile.sprite.Height / 2 < vaisseau.position.Y + vaisseau.sprite.Height + vaisseau.sprite.Height*0.10))
                        )
                    {  // Collision missile => Vaisseau trouvée
                                
                        if ((vaisseau.ennemi && !missile.ennemi) || (!vaisseau.ennemi && missile.ennemi))
                        {
                            listeMissileToRemove.Add(missile);

                            if (vaisseau.hurt(missile.degats, time))
                            {
                                // Vaisseau dead
                                            
                                vaisseau.kill();
                                if((!end)&&(!endDead))
                                score = score + vaisseau.score;

                                listeParticules.Add(new doneParticles(false, new Vector2(vaisseau.position.X + vaisseau.sprite.Width / 2, vaisseau.position.Y + vaisseau.sprite.Height / 2)));        
                            }
                        }
                    }
                    #endregion
                    #region Collision missile => boss
                    if (aBoss != null && missile.isAlive && missile.isOwner(null, aBoss) && ((missile.position.X + missile.sprite.Width > aBoss.Position.X)
                                    && (missile.position.X + missile.sprite.Width < aBoss.Position.X + aBoss.Texture.Width))
                                    && ((missile.position.Y + missile.sprite.Height / 2 > aBoss.Position.Y - aBoss.Texture.Height * 0.10)
                                    && (missile.position.Y + missile.sprite.Height / 2 < aBoss.Position.Y + aBoss.Texture.Height + aBoss.Texture.Height * 0.10)))
                    {

                        listeMissileToRemove.Add(missile);
                        missile.kill();
                        if (aBoss.Hurt(missile.degats))
                        {
                            aBoss.Kill();
                            if ((!end) && (!endDead))
                                score = score + aBoss.Score;
                        }
                    }
                        #endregion
                }
                foreach (Obstacles obstacle in listeObstacles)
                {
                    #region Collision joueur <=> Obstacle
                    if (!dead && ((listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > obstacle.position.X && listeVaisseau[0].position.X < obstacle.position.X) || (listeVaisseau[0].position.X < obstacle.position.X + obstacle.sprite.Width && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width > obstacle.position.X + obstacle.sprite.Width) || (listeVaisseau[0].position.X > obstacle.position.X && listeVaisseau[0].position.X + listeVaisseau[0].sprite.Width < obstacle.position.X + obstacle.sprite.Width))
                        &&
                        ((listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > obstacle.position.Y && listeVaisseau[0].position.Y < obstacle.position.Y) || (listeVaisseau[0].position.Y < obstacle.position.Y + obstacle.sprite.Height && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height > obstacle.position.Y + obstacle.sprite.Height) || (listeVaisseau[0].position.Y > obstacle.position.Y && listeVaisseau[0].position.Y + listeVaisseau[0].sprite.Height < obstacle.position.Y + obstacle.sprite.Height)))
                    {
                        if (obstacle.Categorie == "hole")
                        {
                            double centre_hole_x = obstacle.position.X + (obstacle.sprite.Width / 2);
                            double centre_hole_y = obstacle.position.Y + (obstacle.sprite.Height / 2);
                            double centre_joueur_x = listeVaisseau[0].position.X + (listeVaisseau[0].sprite.Width / 2);
                            double centre_joueur_y = listeVaisseau[0].position.Y + (listeVaisseau[0].sprite.Height / 2);
                            double distance_x = centre_hole_x - centre_joueur_x;
                            double distance_y = centre_hole_y - centre_joueur_y;
                            double distance_unit_x = centre_hole_x - centre_joueur_x / Math.Abs(centre_hole_x - centre_joueur_x);
                            double distance_unit_y = centre_hole_y - centre_joueur_y / Math.Abs(centre_hole_y - centre_joueur_y);
                            double distance_x_max = obstacle.sprite.Width / 2;
                            double distance_y_max = obstacle.sprite.Height / 2;
                        }
                    }
                    #endregion
                }
            }
            return listeParticules;
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
            #region Update de l'état du jeu
            if (listeVaisseau.Count == 0)
                endDead = true;
            else if (listeVaisseau[0].ennemi)
                endDead = true;
            #endregion
            #region Analyse de la musique
            float coeff_speed_variation = 1f; //coefficient de la variation de la vitesse des fonds.
            float coeff_speed = 0.05f; //coefficient de vitesse du fond.
            float coeff_speed_middle = 0.1f; //coefficient de vitesse du fond au milieu.
            float coeff_speed_front = 0.5f; //coefficient de vitesse du fond en avant.
            float default_speed = 1f; //La vitesse doit tendre vers (default_speed * coeff)

            fond_ecran.Update(fps_fix, (default_speed + (amplitude_sum_music - default_speed) * coeff_speed_variation) * coeff_speed);
            fond_ecran_middle.Update(fps_fix, (default_speed + (amplitude_sum_music - default_speed) * coeff_speed_variation) * coeff_speed_middle);
            fond_ecran_front.Update(fps_fix, (default_speed + (amplitude_sum_music - default_speed) * coeff_speed_variation) * coeff_speed_front);

            AudioPlayer.Update();
			#endregion
            #region Gestion de la musique en cas de pause
            if (InputState.IsPauseGame())
            {
                AudioPlayer.SetVolume(0.2f);
            }
            else if (InputState.IsMenuSelect())
                AudioPlayer.SetVolume(1f);

            #endregion
            #region Gestion des évenements du level
            foreach (gestionLevels spawn in infLevel)
            {
                if (boss1 == null && spawn.isTime(time))
                {
                    switch (spawn.Categorie)
                    {
                        case "vaisseau":
                            listeVaisseau.Add(spawn.Adresse);
                            break;
                        case "bonus":
                            listeBonus.Add(spawn.bonus);
                            break;
                        case "obstacle":
                            listeObstacles.Add(spawn.Obstacle);
                            break;
                        case "boss":
                            boss1 = spawn.boss;
                            boss1.LoadContent(_content);
                            aBossWasThere = true;
                            break;
                        case "EOL":
                            if(!endDead)
                                end = true;
                            break;
                        default:
                            break;
                    }
                    listeLevelToRemove.Add(spawn);
                }
            }
            foreach (gestionLevels s in listeLevelToRemove)
                infLevel.Remove(s);

            if(boss1 != null)
                bossTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            #endregion
            #region Gestion des tirs du joueur

            if (keyboardState.IsKeyDown(Keys.F1) && lastKeyDown)
            {
                lastKeyDown = false;
                if (drawSpectre)
                    drawSpectre = false;
                else
                    drawSpectre = true;
            }
            else if(keyboardState.IsKeyUp(Keys.F1))
                lastKeyDown = true;
			
            if ((keyboardState.IsKeyDown(Keys.Space) && (listeVaisseau.Count != 0)))
            {
                switch (listeVaisseau[0].armeActuelle)
                {
                    case 0:
                        if (time - lastTime > 150 || lastTime == 0)
                        {
                            musique_tir.Play();
                            Vector2 spawn = new Vector2(listeVaisseau[0].position.X + listeVaisseau[0]._textureVaisseau.Width - 1, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 2 - 2);
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn, listeVaisseau[0], null));
                            lastTime = time;
                        }
                        break;
                    case 1:
                        if (time - lastTime > 150 || lastTime == 0)
                        {
                            musique_tir.Play();
                            Vector2 spawn1 = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 - 18);
                            Vector2 spawn2 = new Vector2(listeVaisseau[0].position.X + 35, listeVaisseau[0].position.Y + listeVaisseau[0]._textureVaisseau.Height / 3 + 25);
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn1, listeVaisseau[0], null));
                            listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn2, listeVaisseau[0], null));
                            lastTime = time;
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region Mode extreme
            if (mode == GAME_MODE.EXTREME)
                compteur += gameTime.ElapsedGameTime.TotalMilliseconds;
            #endregion 
            #region Update du boss
            if (boss1 != null && boss1.Existe && !endDead)
            {
                boss1.Update(fps_fix, time, listeMissile);
                particleEffectBoss1.Trigger(new Vector2(boss1.PositionX + boss1.Texture.Width + 5, boss1.PositionY + boss1.Texture.Height / 3 - 5));
                particleEffectBoss1.Trigger(new Vector2(boss1.PositionX + boss1.Texture.Width + 5, boss1.PositionY + (boss1.Texture.Height * 2) / 3 + 10));
            }
            else if (boss1 != null && aBossWasThere)
            {
                musique_bossExplosion.Play(1.0f, 0f, 0f);
                particleBossExplosion.Trigger(new Vector2(boss1.PositionX + boss1.Texture.Width / 2, boss1.PositionY + boss1.Texture.Height / 2));
                aBossWasThere = false;
                time -= bossTime;
                lastTime -= bossTime;
                lastTimeSpectre -= bossTime;
                lastTimeEnergy -= bossTime;
                bossTime = 0;
                boss1 = null;
                if (listeVaisseau.Count > 0)
                    listeVaisseau[0].lastDamage = time - 200;
            }
            particleBossExplosion.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            particleEffectBoss1.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
                    if (time - vaisseau.lastTir > vaisseau.timingAttack && vaisseau.timingAttack != 0)
                    {
                        Vector2 spawn, spawnHaut, spawnBas;
                        switch (vaisseau.armeActuelle)
                        {
                            case 0: // Tir normal
                                spawn = new Vector2(vaisseau.position.X - 35, vaisseau.position.Y + vaisseau._textureVaisseau.Height / 2);
                                listeMissile.Add(new Missile_drone(T_Missile_Drone, spawn, vaisseau, null));
                                break;
                            case 1: // Tir double
                                spawnHaut = new Vector2(vaisseau.position.X - 30, vaisseau.position.Y + vaisseau._textureVaisseau.Height / 2 - 20);
                                spawnBas = new Vector2(vaisseau.position.X - 30, vaisseau.position.Y + vaisseau._textureVaisseau.Height / 2 + 10);
                                listeMissile.Add(new Missile_drone(T_Missile_Drone, spawnHaut, vaisseau, null));
                                listeMissile.Add(new Missile_drone(T_Missile_Drone, spawnBas, vaisseau, null));
                                break;
                            case 2: // Blaster
                                spawn = new Vector2(vaisseau.position.X - 35, vaisseau.position.Y + vaisseau._textureVaisseau.Height / 2 - 20);
                                listeMissile.Add(new Blaster_Ennemi(T_Missile_Energie, spawn, vaisseau, null));
                                break;
                            default:
                                break;
                        }
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
            #region Update des obstacles
            foreach (Obstacles obstacle in listeObstacles)
            {
                if (obstacle.position.X > -250)
                    obstacle.Update(fps_fix);
                else
                    listeObstaclesToRemove.Add(obstacle);
            }

            foreach (Bonus bonus in listeBonusToRemove)
                listeBonus.Remove(bonus);
            #endregion
            #region Collisions & Update des particules
            foreach (doneParticles particle in partManage)
            {
                if (particle.startingParticle != Vector2.Zero)
                    particleEffect.Trigger(particle.startingParticle);
            }
            partManage = collisions(listeVaisseau, listeMissile, listeBonus, listeObstacles, boss1, fps_fix, particleEffect, gameTime, listeVaisseau.Count==0);
            particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            if (listeVaisseau.Count > 0)
            {
                Vector2 positionVaisseau = listeVaisseau[0]._emplacement;
                positionVaisseau.Y = positionVaisseau.Y + listeVaisseau[0]._textureVaisseau.Height / 2;
                positionVaisseau.X -= 5;
                ((EmitterCollection)particleEffectMoteur)[0].ReleaseImpulse.X = -400 * amplitude_sum_music;
                ((EmitterCollection)particleEffectMoteur)[0].ReleaseScale.Value = 32 + (amplitude_sum_music - 1) * 25;
                particleEffectMoteur.Trigger(positionVaisseau);
            }
            particleEffectMoteur.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion
            #region Update spectre & historique
            if (lastTimeSpectre + 25 < time)
            {
                float[] spectre_tmp = AudioPlayer.GetSpectrum(128);
                if (spectre_tmp.Length == 128)
                {
                    for (int i = 0; i <= 127; i++)
                    {
                        spectre_tmp[i] = Math.Min(2, spectre_tmp[i] * 10);
                    }

                    lastTimeSpectre = time;
                    spectre = spectre_tmp;
                }
            }

            if (lastTimeEnergy + 25 < time)
            {
                lastTimeEnergy = time;
                amplitude_sum_music = AudioPlayer.GetEnergy();
            }

            #endregion
            #region Gestion du mode Libre
            if (AudioPlayer.IsPlaying())
            {
                int time_music = (int)((AudioPlayer.GetCurrentTime() % (AudioPlayer.GetLength() - 1024)) / 1024f);
                float energy_44100_music = (float) BeatDetector.get_energie44100()[(int)time_music] / 100000;

                position_spawn = new Vector2(1180, r.Next(5, 564));
                if (mode == GAME_MODE.LIBRE && lastTimeMusic < time_music)
                {
                    if ((time_music - lastTimeRandomSpawn > 10) && (BeatDetector.get_beat()[(int)time_music] > 0))
                    {
                        if (amplitude_sum_music > 2)
                        {
                            listeVaisseau.Add(new kamikaze(T_Vaisseau_Kamikaze, position_spawn));
                            beat_spawned = BEAT_SPAWNED.ENEMY;
                        }
                        else if (amplitude_sum_music > 1.7)
                        {
                            listeVaisseau.Add(new RapidShooter(T_Vaisseau_Doubleshooter, position_spawn));
                            beat_spawned = BEAT_SPAWNED.ENEMY;
                        }
                        else if (amplitude_sum_music > 1.2)
                        {
                            listeVaisseau.Add(new Blasterer(T_Vaisseau_Energizer, position_spawn));
                            beat_spawned = BEAT_SPAWNED.ENEMY;
                        }
                        else if (amplitude_sum_music > 0.8)
                        {
                            listeVaisseau.Add(new Drone(T_Vaisseau_Drone, position_spawn));
                            beat_spawned = BEAT_SPAWNED.ENEMY;
                        }
                        else if (amplitude_sum_music > 0.72)
                        {
                            listeBonus.Add(new Bonus_NouvelleArme1(T_Bonus_Weapon1, position_spawn));
                            beat_spawned = BEAT_SPAWNED.BONUS;
                        }
                        else if (amplitude_sum_music > 0.65)
                        {
                            listeBonus.Add(new Bonus_Vie(T_Bonus_Vie, position_spawn));
                            beat_spawned = BEAT_SPAWNED.BONUS;
                        }
                        else
                        {
                            beat_spawned = BEAT_SPAWNED.NOTHING;
                        }

                        lastTimeRandomSpawn = time_music;
                    }

                    lastTimeMusic = time_music;
                }
            }
            else
            {
                end = true;
            }

            #endregion
            #region Fin du level

            if ((end || endDead)&&(first))
            {
                path_level = "Scores\\Arcade\\lvl" + _level + ".score";
                sr_level = new StreamReader(path_level);
                score_level = System.IO.File.ReadAllLines(@path_level);
                stock_score_inferieur = "";
                stock_score_superieur = "";

                for (int i = 0; i < 10; i+=2) 
                {      
                    if (score < Convert.ToInt32(score_level[i + 1]))
                        stock_score_inferieur += score_level[i] + '\n' + score_level[i+1] +'\n';
                    else
                        stock_score_superieur += score_level[i] + '\n' + score_level[i+1] +'\n';
                }
    
                sr_level.Close();
                sw_level = new StreamWriter(path_level);

                sw_level.WriteLine(stock_score_inferieur + "Nervous" + '\n' + Convert.ToString(score) + '\n' + stock_score_superieur);
                sw_level.Close();
                first = false;
                    AudioPlayer.StopMusic();
                    SoundEffect.MasterVolume = 0.00f;                           
            }
            if (end || endDead)
            {
                AudioPlayer.PlayMusic("Musiques\\Menu\\Musique.flac");
                if (keyboardState.IsKeyDown(Keys.Enter))
                    Remove();
            }
            #endregion
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 0, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            int time_music = (int)((AudioPlayer.GetCurrentTime() % (AudioPlayer.GetLength() - 1024))/ 1024f);

            #region Draw du fond
            fond_ecran.Draw(spriteBatch);
            fond_ecran_middle.Draw(spriteBatch);
            fond_ecran_front.Draw(spriteBatch);
            #endregion
            #region Draw de l'HUD
            spriteBatch.Draw(T_HUD, new Vector2(0, 380), Color.White);
            if(listeVaisseau.Count != 0)
                HUD.Drawbar(spriteBatch, barre_vie, listeVaisseau[0].vie, listeVaisseau[0].vieMax);
            spriteBatch.Draw(T_HUD_bars, new Vector2(380, 630), Color.White);
            if (mode != GAME_MODE.EXTREME)
                spriteBatch.DrawString(_HUDfont, Convert.ToString(score), new Vector2(95, 628), new Color(30, 225, 30));
            #endregion
            #region Draw mode extreme
            if (mode == GAME_MODE.EXTREME)
            {
                if (listeVaisseau.Count != 0)
                    spriteBatch.DrawString(_HUDfont, (compteur/1000).ToString("N6"), new Vector2(95, 628), new Color(30, 225, 30));
            }
            #endregion 
            #region Draw des obstacles
            foreach (Obstacles obstacle in listeObstacles)
            {
                obstacle.Draw(spriteBatch);
            }
            #endregion
            #region Draw des particules de déplacement
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            particleRenderer.RenderEffect(particleEffectMoteur);
            particleRenderer.RenderEffect(particleEffectBoss1);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            #endregion
            #region Draw des vaisseaux
            foreach (Vaisseau vaisseau in listeVaisseau)
            {
                vaisseau.Draw(spriteBatch, time);
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
            #region Draw des boss
            if (boss1 != null && boss1.Existe && !(end || endDead))
            {
                spriteBatch.Draw(T_HUD_boss, new Vector2(726, 622), Color.White);
                for (int i = 0; i <= (boss1.vieActuelle * (T_HUD_bar_boss.Width - 30)) / boss1.VieMax; i++)
                    spriteBatch.Draw(barre_vie, new Vector2(775 + i, 692), Color.White);
                spriteBatch.Draw(T_HUD_bar_boss, new Vector2(760, 680), Color.White);
                spriteBatch.DrawString(_HUDfont, "Spaceship X42", new Vector2(760, 645), new Color(30, 225, 30));
                spriteBatch.Draw(T_boss1, new Rectangle(726 + 340, 622 + 35, 75, 75), Color.White);
                boss1.Draw(spriteBatch);
            }
            #endregion 
            #region Draw des particules de collision
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            particleRenderer.RenderEffect(particleEffect);
            particleRenderer.RenderEffect(particleBossExplosion);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            #endregion
            #region Draw des infos sonores
            Rectangle rect;
            Texture2D empty_texture = new Texture2D(SceneManager.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            empty_texture.SetData(new[] { Color.White });
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            if (beat_spawned != BEAT_SPAWNED.NO_BEAT)
            {
                Color color = new Color();
                switch (beat_spawned)
                {
                    case BEAT_SPAWNED.BONUS:
                        color = new Color(0, 255, 0, 0);
                        break;
                    case BEAT_SPAWNED.ENEMY:
                        color = new Color(255, 0, 0, 0);
                        break;
                    case BEAT_SPAWNED.NOTHING:
                        color = new Color(5, 130, 255, 0);
                        break;
                    default:
                        break;
                }
                beat_spawned = BEAT_SPAWNED.NO_BEAT;
                for (int i = 0; i <= 100; i++)
                {
                    color.A = (byte) ((100 - i));
                    rect = new Rectangle(Xspace.Xspace.window_width - i, 0, 1, 622);
                    spriteBatch.Draw(empty_texture, rect, color);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (spectre.Length == 128 && drawSpectre)
            {
                int pxBegin = (Xspace.Xspace.window_height - 512) / 2;
                for (int i = 0; i <= 127; i++)
                {
                    int lenght = (int) (spectre[i] * 250);
                    for (int j = 0; j <= lenght / 4; j++)
                    {
                        rect = new Rectangle(Xspace.Xspace.window_width - lenght + j * 4, pxBegin + i * 4, j * 4, 4);
                        spriteBatch.Draw(empty_texture, rect, new Color(j * 2, 255 - j * 2, 0));
                    }
                }

                spriteBatch.DrawString(_HUDfont, "Energy : " + Convert.ToString(amplitude_sum_music), new Vector2(10, 10), new Color(30, 225, 30));
                spriteBatch.DrawString(_HUDfont, "Tempo : " + Convert.ToString(BeatDetector.get_tempo()), new Vector2(10, 35), new Color(30, 225, 30));
                spriteBatch.DrawString(_HUDfont, "Region  : " + Convert.ToString((float)BeatDetector.get_energie44100()[(int)time_music] / 100000), new Vector2(10, 60), new Color(30, 225, 30));
                if (BeatDetector.get_beat()[(int)time_music] > 0)
                    spriteBatch.DrawString(_HUDfont, "TUMP TUMP", new Vector2(10, 85), new Color(30, 225, 30));

                if (BeatDetector.get_energie1024()[(int)time_music] > 0)
                {
                    rect = new Rectangle(Xspace.Xspace.window_width - 100, Xspace.Xspace.window_height - (int)BeatDetector.get_energie1024()[(int)time_music] / 70000, 100, 4);
                    spriteBatch.Draw(empty_texture, rect, new Color(255, 0, 0));
                }

                if (BeatDetector.get_energie44100()[(int)time_music] > 0)
                {
                    rect = new Rectangle(Xspace.Xspace.window_width - 100, Xspace.Xspace.window_height - (int)(BeatDetector.get_energie44100()[(int)time_music]) / 70000, 100, 4);
                    spriteBatch.Draw(empty_texture, rect, new Color(0, 255, 0));
                }
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
            #region Draw fin du jeu
            if (end)
            {
                spriteBatch.Draw(T_Divers_Levelcomplete, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(_gameFont, Convert.ToString(score), new Vector2(530,276), Color.Green);
            }
            else if (endDead)
            {
                spriteBatch.Draw(T_Divers_Levelfail, new Vector2(0, 0), Color.White);
            }
            #endregion
            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }
    }
}
