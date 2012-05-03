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
        const int SCREEN_MAXTOP = 0, SCREEN_MAXBOT = 620;
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
        private bool drawSpectre, aBossWasThere, first, pause;
        private float amplitude_sum_music, moy_energie1024;
        private Random r;
        private string song_path;
        #endregion
        #region Déclaration variables relatives au jeu
        private List<doneParticles> partManage;
        private ScrollingBackground fond_ecran, fond_ecran_front, fond_ecran_middle;
        public SpriteBatch spriteBatch;
        private Texture2D T_Vaisseau_Joueur, T_Vaisseau_Drone, T_Vaisseau_Kamikaze, T_Missile_Joueur_1, T_Missile_Joueur_2, T_Missile_Joueur_3, T_Missile_HeavyLaser, T_Laser_Joueur, T_Missile_Drone, T_Bonus_Vie, T_Bonus_Weapon1, T_Bonus_Score, T_Bonus_Energie, T_Obstacles_Hole, barre_vie, barre_energy, T_HUD, T_HUD_boss, T_HUD_bars, T_HUD_bar_boss, T_Divers_Levelcomplete, T_Divers_Levelfail, T_boss1, T_Vaisseau_Energizer, T_Vaisseau_Doubleshooter, T_Missile_Energie, T_boss2, T_boss3;
        private List<Texture2D> listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss;
        private SoundEffect sonLaser, sonHeavyLaser, musique_bossExplosion;
        private KeyboardState keyboardState;
        bool lastKeyDown = true, end = false, endDead = false;
        private List<gestionLevels> infLevel, listeLevelToRemove;
        Renderer particleRenderer;
        ParticleEffect particleEffect, particleEffectMoteur, particleEffectBoss1, particleBossExplosion, particleEffectMissiles;
        private Boss boss1;
        List<Vaisseau> listeVaisseau, listeVaisseauToRemove;
        List<Missiles> listeMissile, listeMissileToRemove;
        List<Bonus> listeBonus, listeBonusToRemove, listeBonusToAdd;
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

        private AffichageInformations HUD = new AffichageInformations();

        static bool IntersectPixels(Rectangle rectangleA, Texture2D s1,
                            Rectangle rectangleB, Texture2D s2)
        {
            Color[] dataA = new Color[s1.Width * s1.Height];
            Color[] dataB = new Color[s2.Width * s2.Height];
            s1.GetData<Color>(dataA);
            s2.GetData<Color>(dataB);
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            if (rectangleA.Intersects(rectangleB))
            {
                for (int y = top; y < bottom; y++)
                {
                    for (int x = left; x < right; x++)
                    {
                        // Get the color of both pixels at this point
                        Color colorA = dataA[(x - rectangleA.Left) +
                                             (y - rectangleA.Top) * rectangleA.Width];
                        Color colorB = dataB[(x - rectangleB.Left) +
                                             (y - rectangleB.Top) * rectangleB.Width];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }
                }
            }
            else
                return false;

            // No intersection found
            return false;
        }

        public GameplayScene(SceneManager sceneMgr, GraphicsDeviceManager graphics, int level, int act, GAME_MODE mode, string song_path = "fat1.wav")
            : base(sceneMgr)
        {
            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = graphics
            };
            particleEffect = new ParticleEffect();
            #region Chargement musique
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
            #endregion
        }

        public override void Initialize()
        {
		    base.Initialize();
            aBossWasThere = false;
            bossTime = 0;
            partManage = new List<doneParticles>();
            pause = false;
        }

        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Thread.Sleep(50);
            SceneManager.Game.ResetElapsedTime();
            #region Chargement musiques & sons
            sonLaser = _content.Load<SoundEffect>("Sons\\Tir\\Tir");
            sonHeavyLaser = _content.Load<SoundEffect>("Sons\\Tir\\HeavyLaser");
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
            moy_energie1024 = BeatDetector.get_moy_energie1024();
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
            particleRenderer.LoadContent(_content);
            //Moteur
            particleEffectMoteur = _content.Load<ParticleEffect>("Collisions\\Moteur\\Moteurlocal");
            particleEffectMoteur.Initialise();
            particleEffectMoteur.LoadContent(_content);
            //Moteur Missiles
            particleEffectMissiles = _content.Load<ParticleEffect>("Collisions\\Moteur\\Missiles");
            particleEffectMissiles.Initialise();
            particleEffectMissiles.LoadContent(_content);
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
            T_Missile_Joueur_1 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1");
            T_Missile_Joueur_2 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1_DiagoHaut");
            T_Missile_Joueur_3 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1_DiagoBas");
            T_Missile_HeavyLaser = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\heavyLaser");
            T_Missile_Drone = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            T_Laser_Joueur = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\Laser");
            T_Missile_Energie = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
            #endregion
            #region Chargement textures bonus
            // TODO : Chargement de toutes les textures des bonus en dessous
            T_Bonus_Vie = _content.Load<Texture2D>("Sprites\\Bonus\\Life");
            T_Bonus_Weapon1 = _content.Load<Texture2D>("Sprites\\Bonus\\DoubleBaseWeapon");
            T_Bonus_Score = _content.Load<Texture2D>("Sprites\\Bonus\\Score");
            T_Bonus_Energie = _content.Load<Texture2D>("Sprites\\Bonus\\Energie");
            #endregion
            #region Chargement textures obstacles
            T_Obstacles_Hole = _content.Load<Texture2D>("Sprites\\Obstacles\\Hole");
            #endregion
            #region Chargement textures boss
            T_boss1 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss1");
            T_boss2 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss2");
            T_boss3 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss3");
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
            listeBonusToAdd = new List<Bonus>();
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
            listeTextureBonus.Add(T_Bonus_Score);
            listeTextureBonus.Add(T_Bonus_Energie);

            listeTextureObstacles = new List<Texture2D>();
            listeTextureObstacles.Add(T_Obstacles_Hole);

            listeTextureBoss = new List<Texture2D>();
            listeTextureBoss.Add(T_boss1);
            listeTextureBoss.Add(T_boss2);
            listeTextureBoss.Add(T_boss3);
            
            infLevel = new List<gestionLevels>();
            listeLevelToRemove = new List<gestionLevels>();
            if (mode != GAME_MODE.LIBRE)
            {
                gestionLevels thisLevel = new gestionLevels(_level, listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss);
                thisLevel.readInfos(delimitationFilesInfo, delimitationFilesInfo2, delimitationFilesInfo3, infLevel);
            }
            #endregion
            #region Chargement barre de vie
            barre_vie = _content.Load<Texture2D>("Sprites\\HUD\\Life");
            barre_energy = _content.Load<Texture2D>("Sprites\\HUD\\Energy");
            #endregion
            #region Chargement fin level
            T_Divers_Levelcomplete = _content.Load<Texture2D>("Sprites\\Divers\\levelcompleted");
            T_Divers_Levelfail = _content.Load<Texture2D>("Sprites\\Divers\\gameover");
            #endregion
        }

        List<doneParticles> collisions(List<Vaisseau> listeVaisseau, List<Missiles> listeMissile, List<Bonus> listeBonus, List<Bonus> listeBonusToAdd, List<Obstacles> listeObstacles, Boss aBoss, float spentTime, ParticleEffect particleEffect, GameTime gametime, bool dead)
        {
            List<doneParticles> listeParticules = new List<doneParticles>();
            if (dead)
                return null;
            #region Collision missile => boss
            if (aBoss != null)
            {
                foreach (Missiles missile in listeMissile)
                {
                    if (missile.isOwner(null, aBoss) && IntersectPixels(missile.rectangle, missile.sprite, aBoss.rectangle, aBoss.sprite))
                    {

                        if (!(missile is Laser_joueur))
                        {
                            missile.kill();
                            listeMissileToRemove.Add(missile);
                        }
                        if (aBoss.Hurt(missile.degats))
                        {
                            aBoss.Kill();
                            if ((!end) && (!endDead))
                                score = score + aBoss.Score;
                        }
                    }
                }
            }
            #endregion
            #region Collision joueur <=> boss
            if (aBoss != null && !dead && IntersectPixels(listeVaisseau[0].rectangle, listeVaisseau[0].sprite, aBoss.rectangle, aBoss.sprite))
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
                    if (IntersectPixels(listeVaisseau[0].rectangle, listeVaisseau[0].sprite, bonus.rectangle, bonus.sprite))
                    {
                        if (!bonus.existe)
                        {
                            score += bonus.score;
                            listeVaisseau[0].applyBonus(bonus.effect, bonus.amount, bonus.time);
                            bonus.existe = true;
                        }
                        listeBonusToRemove.Add(bonus);
                    }
                }
                #endregion
                #region Collision joueur <=> vaisseau
                if (vaisseau != listeVaisseau[0] && IntersectPixels(listeVaisseau[0].rectangle, listeVaisseau[0].sprite, vaisseau.rectangle, vaisseau.sprite))
                {
                    // Collision entre vaisseau joueur & ennemi trouvée
                    vaisseau.kill();

                    if ((!end) && (!endDead))
                        score = score + vaisseau.score;

                    listeVaisseauToRemove.Add(vaisseau);
                    listeVaisseau[0].hurt(vaisseau.damageCollision, time);
                    if (listeVaisseau[0].vie < 0)
                        listeVaisseauToRemove.Add(listeVaisseau[0]);
                    listeParticules.Add(new doneParticles(false, new Vector2(vaisseau.pos.X + vaisseau.sprite.Width / 2, vaisseau.pos.Y + vaisseau.sprite.Height / 2)));
                }
                #endregion
                foreach (Missiles missile in listeMissile)
                {
                    #region Collision missile => vaisseau
                    if ((!(missile is Laser_joueur) && IntersectPixels(missile.rectangle, missile.sprite, vaisseau.rectangle, vaisseau.sprite)) || missile.rectangle.Intersects(vaisseau.rectangle))
                    {  // Collision missile => Vaisseau trouvée
                                
                        if ((vaisseau.ennemi && !missile.ennemi) || (!vaisseau.ennemi && missile.ennemi))
                        {
                            if(!(missile is Laser_joueur))
                                listeMissileToRemove.Add(missile);

                            if (vaisseau.hurt(missile.degats, time)) // Mort du vaisseau
                            {
                                vaisseau.kill();
                                if((!end)&&(!endDead))
                                    score += vaisseau.score;

                                int random_bonus = r.Next(0, 100);
                                if (random_bonus > 98)       // 2% - Drop un bonus
                                {
                                    random_bonus = r.Next(0, 100);
                                    if(random_bonus < 50) // 50% que ce soit un bonus vie
                                        listeBonusToAdd.Add(new Bonus_Vie(T_Bonus_Vie, vaisseau.pos));
                                    else if (random_bonus < 70) // 20% - Bonus score
                                        listeBonusToAdd.Add(new Bonus_Score(T_Bonus_Score, vaisseau.pos));
                                    else if (random_bonus < 90) // 20% - Bonus énergie
                                        listeBonusToAdd.Add(new Bonus_Energie(T_Bonus_Energie, vaisseau.pos));
                                    else
                                        listeBonusToAdd.Add(new Bonus_BaseWeapon(T_Bonus_Weapon1, vaisseau.pos));
                                }
                                

                                listeParticules.Add(new doneParticles(false, new Vector2(vaisseau.pos.X + vaisseau.sprite.Width / 2, vaisseau.pos.Y + vaisseau.sprite.Height / 2)));        
                            }
                        }
                    }
                    #endregion
                    #region Collision laser => missile
                    if (listeVaisseau[0].laser)
                    {
                        if (!(missile is Laser_joueur) && missile.ennemi &&missile.rectangle.Intersects(listeVaisseau[0].getLaser().rectangle))
                        {
                            listeMissileToRemove.Add(missile);
                            missile.kill();
                            score += 5;
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
            if (!pause)
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
                    pause = true;
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
                                if (!endDead)
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

                if (boss1 != null)
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
                else if (keyboardState.IsKeyUp(Keys.F1))
                    lastKeyDown = true;

                if ((keyboardState.IsKeyDown(Keys.Space) && (listeVaisseau.Count != 0)))
                {
                    switch (listeVaisseau[0].armeActuelle)
                    {
                        case 0:
                            switch (listeVaisseau[0].baseWeapon)
                            {
                                case 0:
                                    if (time - lastTime > 150 || lastTime == 0)
                                    {
                                        sonLaser.Play();
                                        Vector2 spawn = new Vector2(listeVaisseau[0].pos.X + listeVaisseau[0].sprite.Width - 1, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 2 - 2);
                                        listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn, listeVaisseau[0], null));
                                        lastTime = time;
                                    }
                                    break;
                                case 1:
                                    if (time - lastTime > 150 || lastTime == 0)
                                    {
                                        sonLaser.Play();
                                        Vector2 spawn1 = new Vector2(listeVaisseau[0].pos.X + 35, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 3 - 18);
                                        Vector2 spawn2 = new Vector2(listeVaisseau[0].pos.X + 35, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 3 + 25);
                                        listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn1, listeVaisseau[0], null));
                                        listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn2, listeVaisseau[0], null));
                                        lastTime = time;
                                    }
                                    break;
                                case 2:
                                    if (time - lastTime > 150 || lastTime == 0)
                                    {
                                        sonLaser.Play();
                                        Vector2 spawn1 = new Vector2(listeVaisseau[0].pos.X + 35, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 3 - 18);
                                        Vector2 spawn2 = new Vector2(listeVaisseau[0].pos.X + 35, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 3 + 25);
                                        Vector2 spawn3 = new Vector2(listeVaisseau[0].pos.X + 29, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 3 + 29);
                                        Vector2 spawn4 = new Vector2(listeVaisseau[0].pos.X + 29, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 3 - 38);
                                        listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn1, listeVaisseau[0], null));
                                        listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn2, listeVaisseau[0], null));
                                        listeMissile.Add(new Xspace.Missile1_DiagoHaut_Joueur(T_Missile_Joueur_3, spawn3, listeVaisseau[0], null));
                                        listeMissile.Add(new Xspace.Missile1_DiagoBas_Joueur(T_Missile_Joueur_2, spawn4, listeVaisseau[0], null));
                                        lastTime = time;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 1:
                            if (time - lastTime > 350 || lastTime == 0)
                            {
                                if (!listeVaisseau[0].useEnergy(100))
                                {
                                    sonHeavyLaser.Play();
                                    Vector2 spawn = new Vector2(listeVaisseau[0].pos.X + listeVaisseau[0].sprite.Width - 1, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 2 - 5);
                                    listeMissile.Add(new Xspace.HeavyLaser(T_Missile_HeavyLaser, spawn, listeVaisseau[0], null));
                                    lastTime = time;
                                }
                            }
                            break;
                        case 2:
                            if (!listeVaisseau[0].laser)
                            {
                                if (!listeVaisseau[0].useEnergy(25))
                                {
                                    Vector2 spawn = new Vector2(listeVaisseau[0].pos.X + listeVaisseau[0].sprite.Width, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 2 - 35);
                                    Laser_joueur las = new Laser_joueur(T_Laser_Joueur, spawn, listeVaisseau[0], null);
                                    listeMissile.Add(las);
                                    listeVaisseau[0].enableLaser(las);
                                }
                            }
                            else
                            {
                                listeVaisseau[0].getLaser().pos = new Vector2(listeVaisseau[0].pos.X + listeVaisseau[0].sprite.Width, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 2 - 35);
                                if (listeVaisseau[0].useEnergy(25)) // Si plus d'énergie
                                {
                                    listeMissileToRemove.Add(listeVaisseau[0].getLaser());
                                    listeVaisseau[0].disableLaser();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if ((keyboardState.IsKeyUp(Keys.Space) && (listeVaisseau.Count != 0)))
                {
                    if (listeVaisseau[0].laser)
                    {
                        listeMissileToRemove.Add(listeVaisseau[0].getLaser());
                        listeVaisseau[0].disableLaser();
                    }
                }

                if ((keyboardState.IsKeyDown(Keys.D1) && (listeVaisseau.Count != 0)))
                {
                    if (listeVaisseau[0].laser)
                    {
                        listeMissileToRemove.Add(listeVaisseau[0].getLaser());
                        listeVaisseau[0].disableLaser();
                    }
                    listeVaisseau[0].changeWeapon(0);
                }
                else if ((keyboardState.IsKeyDown(Keys.D2) && (listeVaisseau.Count != 0)))
                {
                    if (listeVaisseau[0].laser)
                    {
                        listeMissileToRemove.Add(listeVaisseau[0].getLaser());
                        listeVaisseau[0].disableLaser();
                    }
                    listeVaisseau[0].changeWeapon(1);
                }
                else if ((keyboardState.IsKeyDown(Keys.D3) && (listeVaisseau.Count != 0)))
                    listeVaisseau[0].changeWeapon(2);
                 /*else if ((keyboardState.IsKeyDown(Keys.D4) && (listeVaisseau.Count != 0)))
                    listeVaisseau[0].changeWeapon(3);*/


                #endregion
                #region Mode extreme
                if (mode == GAME_MODE.EXTREME)
                    compteur += gameTime.ElapsedGameTime.TotalMilliseconds;
                #endregion
                #region Update du boss
                if (boss1 != null && boss1.Existe && !endDead)
                {
                    boss1.Update(fps_fix, time, listeMissile, listeVaisseau);
                    if (boss1.Number == 1)
                    {
                        particleEffectBoss1.Trigger(new Vector2(boss1.PositionX + boss1.Texture.Width + 5, boss1.PositionY + boss1.Texture.Height / 3 - 5));
                        particleEffectBoss1.Trigger(new Vector2(boss1.PositionX + boss1.Texture.Width + 5, boss1.PositionY + (boss1.Texture.Height * 2) / 3 + 10));
                    }
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
                    if (missile.pos.X < 1150 && (missile.pos.Y > SCREEN_MAXTOP && missile.pos.Y < SCREEN_MAXBOT) && !missile.ennemi)
                        missile.Update(fps_fix);
                    else if (missile.pos.X > 0 && (missile.pos.Y > SCREEN_MAXTOP && missile.pos.Y < SCREEN_MAXBOT) && missile.ennemi)
                        missile.Update(fps_fix);
                    else if (!(missile is Laser_joueur))
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
                        vaisseau.Update(fps_fix, keyboardState, listeObstacles);

                    if (vaisseau.ennemi && vaisseau.existe)
                    {
                        if (time - vaisseau.lastTir > vaisseau.timingAttack && vaisseau.timingAttack != 0)
                        {
                            Vector2 spawn, spawnHaut, spawnBas;
                            switch (vaisseau.armeActuelle)
                            {
                                case 0: // Tir normal
                                    spawn = new Vector2(vaisseau.pos.X - 35, vaisseau.pos.Y + vaisseau.sprite.Height / 2);
                                    listeMissile.Add(new Missile_drone(T_Missile_Drone, spawn, vaisseau, null));
                                    break;
                                case 1: // Tir double
                                    spawnHaut = new Vector2(vaisseau.pos.X - 30, vaisseau.pos.Y + vaisseau.sprite.Height / 2 - 20);
                                    spawnBas = new Vector2(vaisseau.pos.X - 30, vaisseau.pos.Y + vaisseau.sprite.Height / 2 + 10);
                                    listeMissile.Add(new Missile_drone(T_Missile_Drone, spawnHaut, vaisseau, null));
                                    listeMissile.Add(new Missile_drone(T_Missile_Drone, spawnBas, vaisseau, null));
                                    break;
                                case 2: // Blaster
                                    spawn = new Vector2(vaisseau.pos.X - 35, vaisseau.pos.Y + vaisseau.sprite.Height / 2 - 20);
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
                    if (bonus.pos.X > 0)
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
                    if (obstacle.pos.X > -250)
                    {
                        obstacle.Update(fps_fix);
                    }
                    else
                    {
                        listeObstaclesToRemove.Add(obstacle);
                    }
                }

                foreach (Bonus bonus in listeBonusToRemove)
                    listeBonus.Remove(bonus);
                foreach (Obstacles obstacle in listeObstaclesToRemove)
                    listeObstacles.Remove(obstacle);
                #endregion
                #region Collisions & Update des particules
                partManage = collisions(listeVaisseau, listeMissile, listeBonus, listeBonusToAdd, listeObstacles, boss1, fps_fix, particleEffect, gameTime, listeVaisseau.Count == 0);
                if (partManage != null)
                {
                    foreach (doneParticles particle in partManage)
                    {
                        if (particle.startingParticle != Vector2.Zero)
                            particleEffect.Trigger(particle.startingParticle);
                    }
                }
                foreach (Bonus b in listeBonusToAdd)
                {
                    listeBonus.Add(b);
                }
                listeBonusToAdd.Clear();
                particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


                if (listeVaisseau.Count > 0)
                {
                    Vector2 positionVaisseau = listeVaisseau[0].pos;
                    positionVaisseau.Y = positionVaisseau.Y + listeVaisseau[0].sprite.Height / 2;
                    positionVaisseau.X -= 5;
                    ((EmitterCollection)particleEffectMoteur)[0].ReleaseImpulse.X = -400 * amplitude_sum_music;
                    ((EmitterCollection)particleEffectMoteur)[0].ReleaseScale.Value = 32 + (amplitude_sum_music - 1) * 25;
                    particleEffectMoteur.Trigger(positionVaisseau);
                }
                particleEffectMoteur.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                #endregion*/
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
                    float energy_1024_music = (float)BeatDetector.get_energie1024()[(int)time_music];

                    position_spawn = new Vector2(1180, r.Next(5, 564));
                    if (mode == GAME_MODE.LIBRE && lastTimeMusic < time_music)
                    {
                        if ((time_music - lastTimeRandomSpawn > 10) && (BeatDetector.get_beat()[(int)time_music] > 0))
                        {
                            if (energy_1024_music / moy_energie1024 > 1.8)
                            {
                                listeVaisseau.Add(new kamikaze(T_Vaisseau_Kamikaze, position_spawn));
                                beat_spawned = BEAT_SPAWNED.ENEMY;
                            }
                            else if (energy_1024_music / moy_energie1024 > 1.5)
                            {
                                listeVaisseau.Add(new RapidShooter(T_Vaisseau_Doubleshooter, position_spawn));
                                beat_spawned = BEAT_SPAWNED.ENEMY;
                            }
                            else if (energy_1024_music / moy_energie1024 > 1.2)
                            {
                                listeVaisseau.Add(new Blasterer(T_Vaisseau_Energizer, position_spawn));
                                beat_spawned = BEAT_SPAWNED.ENEMY;
                            }
                            else if (energy_1024_music / moy_energie1024 > 1)
                            {
                                listeVaisseau.Add(new Drone(T_Vaisseau_Drone, position_spawn));
                                beat_spawned = BEAT_SPAWNED.ENEMY;
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

                if ((end || endDead) && (first))
                {
                    AudioPlayer.StopMusic();
                    SoundEffect.MasterVolume = 0.00f;
                    AudioPlayer.PlayMusic("Musiques\\Menu\\Musique.flac");
                    if (mode != GAME_MODE.LIBRE)
                    {
                        path_level = "Scores\\Arcade\\lvl" + _level + ".score";
                        sr_level = new StreamReader(path_level);
                        score_level = System.IO.File.ReadAllLines(@path_level);
                        stock_score_inferieur = "";
                        stock_score_superieur = "";

                        for (int i = 0; i < 10; i += 2)
                        {
                            if (score < Convert.ToInt32(score_level[i + 1]))
                                stock_score_inferieur += score_level[i] + '\n' + score_level[i + 1] + '\n';
                            else
                                stock_score_superieur += score_level[i] + '\n' + score_level[i + 1] + '\n';
                        }

                        sr_level.Close();
                        sw_level = new StreamWriter(path_level);

                        sw_level.WriteLine(stock_score_inferieur + "Nervous" + '\n' + Convert.ToString(score) + '\n' + stock_score_superieur);
                        sw_level.Close();
                    }
                    first = false;
                }
                if (end || endDead)
                {
                    if (keyboardState.IsKeyDown(Keys.Enter))
                        Remove();
                }
                #endregion
                base.Update(gameTime);
            }
            else
            {
                if ((InputState.IsMenuCancel())||(InputState.IsMenuSelect()))
                    pause = false;
            }
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
            #region Draw des missiles
            foreach (Missiles sMissile in listeMissile)
            {
                sMissile.Draw(spriteBatch);
            }
            #endregion
            #region Draw des vaisseaux
            foreach (Vaisseau vaisseau in listeVaisseau)
            {
                vaisseau.Draw(spriteBatch, time);
            }
            #endregion
            #region Draw des bonus
            foreach (Bonus bonus in listeBonus)
            {
                bonus.Draw(spriteBatch);
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
                spriteBatch.DrawString(_HUDfont, "Ratio  : " + Convert.ToString((float)BeatDetector.get_energie44100()[(int)time_music] / moy_energie1024), new Vector2(10, 60), new Color(30, 225, 30));
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
            #region Draw de l'HUD
            spriteBatch.Draw(T_HUD, new Vector2(0, 380), Color.White);
            if (listeVaisseau.Count != 0)
                HUD.Drawbar(spriteBatch, barre_vie, barre_energy, listeVaisseau[0].vie, listeVaisseau[0].vieMax, listeVaisseau[0].Energie, listeVaisseau[0].EnergieMax);
            spriteBatch.Draw(T_HUD_bars, new Vector2(380, 630), Color.White);
            if (mode != GAME_MODE.EXTREME)
                spriteBatch.DrawString(_HUDfont, Convert.ToString(score), new Vector2(95, 628), new Color(30, 225, 30));
            #endregion
            #region Draw des boss
            if (boss1 != null && boss1.Existe && !(end || endDead))
            {
                spriteBatch.Draw(T_HUD_boss, new Vector2(726, 622), Color.White);
                for (int i = 0; i <= (boss1.vieActuelle * (T_HUD_bar_boss.Width - 30)) / boss1.VieMax; i++)
                    spriteBatch.Draw(barre_vie, new Vector2(775 + i, 692), Color.White);
                spriteBatch.Draw(T_HUD_bar_boss, new Vector2(760, 680), Color.White);
                    spriteBatch.DrawString(_HUDfont, boss1.Name, new Vector2(760, 645), new Color(30, 225, 30));
                    spriteBatch.Draw(boss1.Texture, new Rectangle(726 + 340, 622 + 35, 75, 75), Color.White);
                boss1.Draw(spriteBatch);
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
