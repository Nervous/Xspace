﻿using System;
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
        private const int SCREEN_MAXTOP = -30, SCREEN_MAXBOT = 620;
        private const int BONUS_EASY = 30, BONUS_NORMAL = 15, BONUS_HARD = 5, BONUS_HC = 0;
        private const int ETL_EASY = 500, ETL_NORMAL = 200, ETL_HARD = 125, ETL_HC = 50;
        private const int DSL_EASY = 5, DSL_NORMAL = 3, DSL_HARD = 1, DSL_HC = 0;
        private int bonus_diff, etl_diff, delay_spawn_libre;
        #region Déclaration variables usuelles
        private int score, _level, score_extreme;
        private float fps_fix, _pauseAlpha;
        private double time, lastTime, lastTimeSpectre, lastTimeEnergy, bossTime, compteur, lastTimeRandomSpawn, lastTimeMusic;
        private string path_level, stock_score_inferieur, stock_score_superieur, name, name_level;
        private string[] score_level;
        private StreamWriter sw_level;
        private StreamReader sr_level;
        private char[] delimitationFilesInfo = new char[] { ' ' }, delimitationFilesInfo2 = new char[] { ';' }, delimitationFilesInfo3 = new char[] { ':' };
        private float[] spectre;
        private int[] progressionMusic;
        private bool drawSpectre, aBossWasThere, first, pause, aButton;
        private float amplitude_sum_music, moy_energie1024;
        private Random r;
        private string song_path;
        #endregion
        #region Déclaration variables relatives au jeu
        private List<doneParticles> partManage;
        private ScrollingBackground fond_ecran, fond_ecran_front, fond_ecran_middle;
        public SpriteBatch spriteBatch;
        private Texture2D T_Vaisseau_Joueur, T_Vaisseau_Drone, T_Vaisseau_Kamikaze, T_Missile_Joueur_1, T_Missile_Joueur_2, T_Missile_Joueur_3, T_Missile_HeavyLaser, T_Laser_Joueur, T_Missile_Drone, T_Missile_Rocket, T_MissileAutoguide, T_LaserEnnemi, T_Bonus_Vie, T_Bonus_Weapon1, T_Bonus_Score, T_Bonus_Energie, T_Bonus_Speed, T_Bonus_Shootspeed, T_Obstacles_Hole, barre_vie, barre_energy, T_HUD, T_HUD_boss, T_HUD_bars, T_HUD_bar_boss, T_Divers_Levelcomplete, T_Divers_Levelfail, T_boss1, T_Vaisseau_Energizer, T_Vaisseau_Doubleshooter, T_Vaisseau_Zebra, T_Vaisseau_Targeter, T_Vaisseau_BC, T_Vaisseau_Support, T_Missile_Energie, T_boss2, T_boss3, T_HUD_basic, T_HUD_laser, T_HUD_Heavy, T_HUD_red_rect, T_HUD_rocket, T_HUD_musicProgression, T_HUD_vie, T_boss4, T_boss5;
        private List<Texture2D> listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss;
        private SoundEffect sonLaser, sonHeavyLaser, musique_bossExplosion;
        private KeyboardState keyboardState;
        private GamePadState gamepadState;
        bool lastKeyDown = true, lastAKeyDown = true, lastEKeyDown = true, end = false, endDead = false;
        private List<gestionLevels> infLevel, listeLevelToRemove;
        Renderer particleRenderer;
        ParticleEffect particleEffect, particleEffectMoteur, particleEffectBoss1, particleBossExplosion, particleEffectMissiles, particleExplosionAoE;
        private Boss boss1;
        List<Vaisseau> listeVaisseau, listeVaisseauToRemove;
        List<Missiles> listeMissile, listeMissileToRemove;
        List<Bonus> listeBonus, listeBonusToRemove, listeBonusToAdd;
        List<Obstacles> listeObstacles, listeObstaclesToRemove;
        List<supportAoe> listeSupportAoE, listeSupportAoEtoRemove;
        private ContentManager _content;
        private SpriteFont _gameFont, _ingameFont, _HUDfont;
        private GAME_MODE mode;
        private Vector2 position_spawn;
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

        List<lasersEnnemis> cleanLaser, cleanLaserToRemove;

        struct lasersEnnemis
        {
            public Vaisseau v;
            public Laser_Ennemi laser;
            public lasersEnnemis(Vaisseau v, Laser_Ennemi l)
            {
                this.v = v;
                this.laser = l;
            }
        };

        struct supportAoe
        {
            public Vaisseau v;
            public Texture2D circle;
            public Vector2 pos;
            public int radius;

            public supportAoe(Vaisseau v)
            {
                this.v = v;
                circle = null;
                radius = 0;
                pos = Vector2.Zero;
            }

            public void recalculerRadius(double time, GraphicsDevice g)
            {
                this.radius = 50 + (int)((time - this.v.tLancementAoE) / 70);
                if (radius >= 2048)
                    radius = 2047;
                this.circle = CreateCircle(radius, g);
            }

            public void setPosition(Vector2 newP)
            {
                pos = newP;
            }
        }

        public enum GAME_MODE
        {
            CAMPAGNE,
            EXTREME,
            LIBRE,
            CUSTOM,
        }
        #endregion
        #region Fonctions
        private AffichageInformations HUD = new AffichageInformations();

        static List<lasersEnnemis> suppEntree(List<lasersEnnemis> liste, Vaisseau vais)
        {
            List<lasersEnnemis> remove = new List<lasersEnnemis>();
            foreach (lasersEnnemis s in liste)
            {
                if (s.v == vais)
                    remove.Add(s);
            }

            foreach (lasersEnnemis s in remove)
            {
                liste.Remove(s);
            }

            return liste;
        }

        static bool findIn(List<Vaisseau> liste, Vaisseau vais)
        {
            bool toReturn = false;
            foreach (Vaisseau v in liste)
            {
                if (v == vais)
                    toReturn = true;
            }

            return toReturn;
        }

        static public Texture2D CreateCircle(int radius, GraphicsDevice graphics)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            if (outerRadius >= 2046)
                outerRadius = 2046;
            Texture2D texture = new Texture2D(graphics, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.Transparent;
            }

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

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

        #endregion

        public GameplayScene(SceneManager sceneMgr, GraphicsDeviceManager graphics, int level, int act, GAME_MODE mode, string custom_path = "")
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
            
            if (mode == GAME_MODE.LIBRE)
                this.song_path = custom_path;
            else
                this.song_path = "fat1.wav";
            
            if (mode == GAME_MODE.CUSTOM)
            {
                string[] nb_level = custom_path.Split('.');
                name_level = nb_level[0];
            }
            
            position_spawn = new Vector2();
            #endregion
        }

        public override void Initialize()
        {
		    base.Initialize();
            aBossWasThere = false;
            bossTime = 0;
            name = "";
            partManage = new List<doneParticles>();
            pause = false;
            aButton = false;

            try
            {
                switch (int.Parse(new StreamReader("DIFF").ReadToEnd()))
                {
                    case 0: // Easy
                        bonus_diff = BONUS_EASY;
                        etl_diff = ETL_EASY;
                        delay_spawn_libre = DSL_EASY;
                        break;
                    case 1: // Normal
                        bonus_diff = BONUS_NORMAL;
                        etl_diff = ETL_NORMAL;
                        delay_spawn_libre = DSL_NORMAL;
                        break;
                    case 2: // Hard
                        bonus_diff = BONUS_HARD;
                        etl_diff = ETL_HARD;
                        delay_spawn_libre = DSL_HARD;
                        break;
                    case 3: // HC
                        bonus_diff = BONUS_HC;
                        etl_diff = ETL_HC;
                        delay_spawn_libre = DSL_HC;
                        break;
                    default:
                        bonus_diff = BONUS_NORMAL;
                        etl_diff = ETL_NORMAL;
                        delay_spawn_libre = DSL_NORMAL;
                        break;
                }
            }
            catch
            {
                bonus_diff = BONUS_NORMAL;
                etl_diff = ETL_NORMAL;
            }
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

            LoadSong song = new LoadSong(song_path);
            r = new Random(song.md5_seed);

            int loop = (mode == GAME_MODE.LIBRE) ? 0 : -1;
            AudioPlayer.PlayMusic("Musiques\\Jeu\\" + song_path, loop, true);
            AudioPlayer.SetVolume(1f);
            BeatDetector.Initialize();
            BeatDetector.audio_process();
            moy_energie1024 = BeatDetector.get_moy_energie1024();

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
            particleEffectMoteur = _content.Load<ParticleEffect>("Collisions\\Moteur\\Moteur");
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
            particleExplosionAoE = _content.Load<ParticleEffect>("Collisions\\BasicExplosion\\ExplosionAoE");
            particleExplosionAoE.Initialise();
            particleExplosionAoE.LoadContent(_content);
            #endregion
            #region Chargement textures vaisseaux
            T_Vaisseau_Joueur = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Joueur\\Joueur_1");
            T_Vaisseau_Drone = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\Drone");
            T_Vaisseau_Kamikaze = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\Kamikaze");
            T_Vaisseau_Energizer = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\energizer");
            T_Vaisseau_Doubleshooter = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\doubleshooter");
            T_Vaisseau_Zebra = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\Zebra");
            T_Vaisseau_Targeter = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\targeter");
            T_Vaisseau_BC = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\BC");
            T_Vaisseau_Support = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Ennemi\\support");
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
            T_HUD_laser = _content.Load<Texture2D>("Sprites\\HUD\\laser_choice");
            T_HUD_basic= _content.Load<Texture2D>("Sprites\\HUD\\basic_choice");
            T_HUD_red_rect = _content.Load<Texture2D>("Sprites\\HUD\\rect_hud");
            T_HUD_Heavy = _content.Load<Texture2D>("Sprites\\HUD\\red_laser_choice");
            T_HUD_rocket = _content.Load<Texture2D>("Sprites\\HUD\\rocket");
            T_HUD_vie = _content.Load<Texture2D>("Sprites\\HUD\\hud_vie");
            if (mode == GAME_MODE.LIBRE)
            {
                T_HUD_musicProgression = new Texture2D(GraphicsDevice, 450, 128);
                Color[] progressionColors = new Color[T_HUD_musicProgression.Width * T_HUD_musicProgression.Height];
                for (int x = 0; x < T_HUD_musicProgression.Width; x++)
                    for (int y = 0; y < T_HUD_musicProgression.Height; y++)
                        progressionColors[x + y * T_HUD_musicProgression.Width] = Color.Transparent;

                int[] values = BeatDetector.get_array_energieN(T_HUD_musicProgression.Width);
                progressionMusic = new int[values.Length];
                int rapport = values.Max() / (T_HUD_musicProgression.Height * 3 / 4);
                for (int i = 0; i < values.Length; i++)
                    progressionMusic[i] = values[i] / rapport;
                
                for (int i = 0; i < values.Length; i++ )
                {
                    for (int j = 0; j < progressionMusic[i]; j++)
                    {
                        progressionColors[i + (T_HUD_musicProgression.Height - j - 1) * T_HUD_musicProgression.Width] = new Color(0, 255, 0, 2*j);
                    }
                }

                T_HUD_musicProgression.SetData<Color>(progressionColors);
            }
            else
                T_HUD_musicProgression = null;
            #endregion
            #region Chargement textures missiles
            T_Missile_Joueur_1 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1");
            T_Missile_Joueur_2 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1_DiagoHaut");
            T_Missile_Joueur_3 = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\1_DiagoBas");
            T_Missile_HeavyLaser = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\heavyLaser");
            T_Missile_Rocket = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\rocket");
            T_Laser_Joueur = _content.Load<Texture2D>("Sprites\\Missiles\\Joueur\\Laser");
            T_Missile_Drone = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_new1");
            T_Missile_Energie = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\missile_boule1");
            T_MissileAutoguide = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\autoguide");
            T_LaserEnnemi = _content.Load<Texture2D>("Sprites\\Missiles\\Ennemi\\Laser");
            #endregion
            #region Chargement textures bonus
            // TODO : Chargement de toutes les textures des bonus en dessous
            T_Bonus_Vie = _content.Load<Texture2D>("Sprites\\Bonus\\Life");
            T_Bonus_Weapon1 = _content.Load<Texture2D>("Sprites\\Bonus\\DoubleBaseWeapon");
            T_Bonus_Score = _content.Load<Texture2D>("Sprites\\Bonus\\Score");
            T_Bonus_Energie = _content.Load<Texture2D>("Sprites\\Bonus\\Energie");
            T_Bonus_Speed = _content.Load<Texture2D>("Sprites\\Bonus\\Speed");
            T_Bonus_Shootspeed = _content.Load<Texture2D>("Sprites\\Bonus\\Shootspeed");
            #endregion
            #region Chargement textures obstacles
            T_Obstacles_Hole = _content.Load<Texture2D>("Sprites\\Obstacles\\Hole");
            #endregion
            #region Chargement textures boss
            T_boss1 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss1");
            T_boss2 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss2");
            T_boss3 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\boss3");
            T_boss4 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\oeil1");
            T_boss5 = _content.Load<Texture2D>("Sprites\\Vaisseaux\\Boss\\nathalisboulax");
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
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Zebra);
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Targeter);
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_BC);
            listeTextureVaisseauxEnnemis.Add(T_Vaisseau_Support);


            listeTextureBonus = new List<Texture2D>();
            listeTextureBonus.Add(T_Bonus_Vie);
            listeTextureBonus.Add(T_Bonus_Weapon1);
            listeTextureBonus.Add(T_Bonus_Score);
            listeTextureBonus.Add(T_Bonus_Energie);
            listeTextureBonus.Add(T_Bonus_Speed);
            listeTextureBonus.Add(T_Bonus_Shootspeed);

            listeTextureObstacles = new List<Texture2D>();
            listeTextureObstacles.Add(T_Obstacles_Hole);

            listeTextureBoss = new List<Texture2D>();
            listeTextureBoss.Add(T_boss1);
            listeTextureBoss.Add(T_boss2);
            listeTextureBoss.Add(T_boss3);
            listeTextureBoss.Add(T_boss4);
            listeTextureBoss.Add(T_boss5);
            
            infLevel = new List<gestionLevels>();
            listeLevelToRemove = new List<gestionLevels>();
            cleanLaser = new List<lasersEnnemis>();
            cleanLaserToRemove = new List<lasersEnnemis>();

            listeSupportAoE = new List<supportAoe>();
            listeSupportAoEtoRemove = new List<supportAoe>();
            if (mode != GAME_MODE.LIBRE)
            {
                gestionLevels thisLevel;
                if (mode == GAME_MODE.CUSTOM)
                    thisLevel = new gestionLevels(name_level, listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss);
                else
                    thisLevel = new gestionLevels(_level, listeTextureVaisseauxEnnemis, listeTextureBonus, listeTextureObstacles, listeTextureBoss);
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
            //circle = CreateCircle(150, SceneManager.GraphicsDevice);

            AudioPlayer.PauseMusic();
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
                            listeVaisseau[0].applyBonus(bonus.effect, bonus.amount, bonus.time, time);
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
                            if (!(missile is Laser_joueur) && !(missile is Laser_Ennemi))
                                listeMissileToRemove.Add(missile);

                            int degats = missile.degats;
                            
                            Rectangle rec;
                            foreach(supportAoe aoe in listeSupportAoE)
                            {
                                aoe.recalculerRadius(time, SceneManager.GraphicsDevice);
                                Vector2 position = new Vector2((aoe.v.pos.X + aoe.v.sprite.Width / 2) - aoe.radius, (aoe.v.pos.Y + aoe.v.sprite.Height / 2) - aoe.radius);
                                aoe.setPosition(position);
                                rec = new Rectangle((int)aoe.pos.X, (int)aoe.pos.Y, aoe.circle.Width, aoe.circle.Height);
                                if (missile.rectangle.Intersects(rec)) // Si le missile est dans une AoE reduction damage
                                    degats /= 4;
                            }

                            if (missile is Rocket) // AoE sur les roquettes
                            {
                                foreach (Vaisseau v in listeVaisseau)
                                {
                                    Rectangle recRoquette = missile.rectangle;
                                    double disX = Math.Abs(missile.pos.X - v.pos.X);
                                    double disY = Math.Abs(missile.pos.Y - v.pos.Y);
                                    double distance = Math.Sqrt(Math.Pow(disX, 2) + Math.Pow(disY, 2));
                                    if (distance < 250 && !dead && v != listeVaisseau[0])
                                    {
                                        if (v.hurt(degats, time))
                                        {
                                            v.kill();
                                            if ((!end) && (!endDead))
                                                score += vaisseau.score;

                                            int random_bonus = r.Next(0, 100);
                                            if (random_bonus < bonus_diff)       // Drop un bonus
                                            {
                                                random_bonus = r.Next(0, 100);
                                                if (random_bonus < 50) // 50% que ce soit un bonus vie
                                                    listeBonusToAdd.Add(new Bonus_Vie(T_Bonus_Vie, vaisseau.pos));
                                                else if (random_bonus < 75) // 25% - Bonus énergie
                                                    listeBonusToAdd.Add(new Bonus_Energie(T_Bonus_Energie, vaisseau.pos));
                                                else if (random_bonus < 95) // 20% - Bonus score
                                                    listeBonusToAdd.Add(new Bonus_Score(T_Bonus_Score, vaisseau.pos));
                                                else                        // 5% - Bonus arme
                                                    listeBonusToAdd.Add(new Bonus_BaseWeapon(T_Bonus_Weapon1, vaisseau.pos));
                                            }
                                        }

                                        //listeParticules.Add(new doneParticles(false, new Vector2(vaisseau.pos.X + vaisseau.sprite.Width / 2, vaisseau.pos.Y + vaisseau.sprite.Height / 2)));    
                                    }
                                }

                                musique_bossExplosion.Play();
                                particleExplosionAoE.Trigger(new Vector2(missile.pos.X + T_Missile_Rocket.Width / 2, missile.pos.Y + T_Missile_Rocket.Height / 2));
                            }

                            if (vaisseau.hurt(degats, time)) // Mort du vaisseau
                            {
                                vaisseau.kill();
                                if((!end)&&(!endDead))
                                    score += vaisseau.score;

                                int random_bonus = r.Next(0, 100);
                                if (random_bonus < bonus_diff)       // Drop un bonus
                                {
                                    random_bonus = r.Next(0, 100);
                                    if (random_bonus < 50) // 50% que ce soit un bonus vie
                                        listeBonusToAdd.Add(new Bonus_Vie(T_Bonus_Vie, vaisseau.pos));
                                    else if (random_bonus < 75) // 25% - Bonus énergie
                                        listeBonusToAdd.Add(new Bonus_Energie(T_Bonus_Energie, vaisseau.pos));
                                    else if (random_bonus < 95) // 20% - Bonus score
                                        listeBonusToAdd.Add(new Bonus_Score(T_Bonus_Score, vaisseau.pos));
                                    else                        // 5% - Bonus arme
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
                        if (listeVaisseau.Count > 0 && !listeVaisseau[0].ennemi && missile != null && !(missile is Laser_joueur) && missile.ennemi &&missile.rectangle.Intersects(listeVaisseau[0].getLaser().rectangle))
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
                gamepadState = GamePad.GetState(PlayerIndex.One);
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
                    AudioPlayer.PauseMusic();
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

                if (keyboardState.IsKeyDown(Keys.A) && lastAKeyDown)
                {
                    lastAKeyDown = false;
                    AudioPlayer.UpSpeed();
                }
                else if (keyboardState.IsKeyUp(Keys.A))
                    lastAKeyDown = true;

                if (keyboardState.IsKeyDown(Keys.E) && lastEKeyDown)
                {
                    lastEKeyDown = false;
                    AudioPlayer.DownSpeed();
                }
                else if (keyboardState.IsKeyUp(Keys.E))
                    lastEKeyDown = true;

                if (((keyboardState.IsKeyDown(Keys.Space) || (GamePad.GetCapabilities(PlayerIndex.One).IsConnected && GamePad.GetCapabilities(PlayerIndex.One).HasRightTrigger && gamepadState.IsButtonDown(Buttons.RightTrigger))) && (listeVaisseau.Count != 0)))
                {
                    switch (listeVaisseau[0].armeActuelle)
                    {
                        case 0:
                            switch (listeVaisseau[0].baseWeapon)
                            {
                                case 0:
                                    if (time - lastTime > listeVaisseau[0].timingAttackPlayerBaseWeapon || lastTime == 0)
                                    {
                                        sonLaser.Play();
                                        Vector2 spawn = new Vector2(listeVaisseau[0].pos.X + listeVaisseau[0].sprite.Width - 1, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 2 - 2);
                                        listeMissile.Add(new Xspace.Missile1_joueur(T_Missile_Joueur_1, spawn, listeVaisseau[0], null));
                                        lastTime = time;
                                    }
                                    break;
                                case 1:
                                    if (time - lastTime > listeVaisseau[0].timingAttackPlayerBaseWeapon || lastTime == 0)
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
                                    if (time - lastTime > listeVaisseau[0].timingAttackPlayerBaseWeapon || lastTime == 0)
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
                        case 3:
                            if (time - lastTime > 350 || lastTime == 0)
                            {
                                if (!listeVaisseau[0].useEnergy(200))
                                {
                                    Vector2 spawn = new Vector2(listeVaisseau[0].pos.X + listeVaisseau[0].sprite.Width - 1, listeVaisseau[0].pos.Y + listeVaisseau[0].sprite.Height / 2 - T_Missile_Rocket.Height / 2);
                                    listeMissile.Add(new Xspace.Rocket(T_Missile_Rocket, spawn, listeVaisseau[0], null));
                                    lastTime = time;
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
                else if ((keyboardState.IsKeyDown(Keys.D4) && (listeVaisseau.Count != 0)))
                    listeVaisseau[0].changeWeapon(3);
                else if (GamePad.GetCapabilities(PlayerIndex.One).IsConnected && GamePad.GetCapabilities(PlayerIndex.One).HasAButton)
                {
                    if (gamepadState.IsButtonDown(Buttons.A) && !aButton)
                    {
                        if (listeVaisseau[0].laser)
                        {
                            listeMissileToRemove.Add(listeVaisseau[0].getLaser());
                            listeVaisseau[0].disableLaser();
                        }
                        listeVaisseau[0]._armeActuelle++;
                        if (listeVaisseau[0]._armeActuelle > 3)
                            listeVaisseau[0]._armeActuelle = 0;

                        aButton = true;
                    }
                    else if (gamepadState.IsButtonUp(Buttons.A))
                    {
                        aButton = false;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.R)) // Echange energie => vie
                {
                    listeVaisseau[0].energyToLife(time, etl_diff);
                }


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
                    if ((missile.pos.X < 1150 && (missile.pos.Y > SCREEN_MAXTOP && missile.pos.Y < SCREEN_MAXBOT) && !missile.ennemi) || (missile.pos.X > 0 && (missile.pos.Y > SCREEN_MAXTOP && missile.pos.Y < SCREEN_MAXBOT) && missile.ennemi))
                    {
                        if (missile is Autoguide && !endDead)
                            missile.Update(fps_fix, listeVaisseau[0]);
                        else
                            missile.Update(fps_fix);

                        if (missile is Rocket)
                        {
                            particleEffectMissiles.Trigger(new Vector2(missile.pos.X, missile.pos.Y + T_Missile_Rocket.Height / 2));
                        }
                    }
                    else if (!(missile is Laser_joueur) && !(missile is Laser_Ennemi))
                        listeMissileToRemove.Add(missile);
                }
                foreach (lasersEnnemis s in cleanLaser)
                {
                    if (!findIn(listeVaisseau, s.v))
                    {
                        listeMissileToRemove.Add(s.laser);
                        cleanLaserToRemove.Add(s);
                    }
                }
                foreach (lasersEnnemis s in cleanLaserToRemove)
                {
                    cleanLaser.Remove(s);
                }
                cleanLaserToRemove.Clear();
                particleEffectMissiles.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
                        vaisseau.Update(fps_fix, time, keyboardState, gamepadState, listeObstacles);

                    if (vaisseau.ennemi && vaisseau.existe)
                    {
                        if ((time - vaisseau.lastTir > vaisseau.timingAttack && vaisseau.lastTir != 0 && vaisseau.timingAttack != 0))
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
                                case 3: // Missile autoguide
                                    spawn = new Vector2(vaisseau.pos.X - T_MissileAutoguide.Width/2, vaisseau.pos.Y + vaisseau.sprite.Height / 2);
                                    listeMissile.Add(new Autoguide(T_MissileAutoguide, spawn, vaisseau, null));
                                    break;
                                case 4 : // Laser
                                    vaisseau.activerChargement(time);
                                    break;
                                case 5 : // AoE du support
                                    if (vaisseau.AoE == false)
                                    {
                                        supportAoe AoE = new supportAoe(vaisseau);
                                        listeSupportAoE.Add(AoE);
                                        vaisseau.AoE = true;
                                        vaisseau.tLancementAoE = time;
                                        vaisseau.activerAoE();
                                    }
                                    break;
                                default:
                                    break;
                            }
                            vaisseau.lastTir = time;
                        }
                        else if (vaisseau.lastTir == 0) // Si n'a jamais tiré, on va le faire tirer plus vite la première fois	  	
                        {
                            vaisseau.lastTir = time - (vaisseau.timingAttack - vaisseau.timingAttack / 5);
                        }
                        else if (vaisseau is BC)
                        {
                            if (vaisseau.Chargement && vaisseau.finChargement(time)) // fin du chargement
                            {
                                vaisseau.terminerChargement();
                                Vector2 spawn = new Vector2(vaisseau.pos.X - T_LaserEnnemi.Width, vaisseau.pos.Y - T_LaserEnnemi.Height / 2 + 29);
                                Laser_Ennemi laser = new Laser_Ennemi(T_LaserEnnemi, spawn, vaisseau, null);
                                vaisseau.enableLaserE(laser);
                                listeMissile.Add(laser);
                                vaisseau.activerTir(time);
                                cleanLaser.Add(new lasersEnnemis(vaisseau, laser));
                            }
                            else if (vaisseau.Tir && vaisseau.finTir(time))
                            {
                                vaisseau.terminerTir();
                                listeMissileToRemove.Add(vaisseau.getLaserE());
                                cleanLaser = suppEntree(cleanLaser, vaisseau);
                                vaisseau.disableLaser();
                            }
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
                    if (bonus != null && bonus.pos.X > 0)
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
                #region Update des AoE
                foreach (supportAoe aoe in listeSupportAoE)
                {
                    if (!findIn(listeVaisseau, aoe.v))
                        listeSupportAoEtoRemove.Add(aoe);
                }

                foreach (supportAoe aoe in listeSupportAoEtoRemove)
                {
                    listeSupportAoE.Remove(aoe);
                }
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
                particleExplosionAoE.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

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
                if (GAME_MODE.LIBRE == mode)
                {
                    if (AudioPlayer.IsPlaying())
                    {
                        int time_music = (int)((AudioPlayer.GetCurrentTime() % (AudioPlayer.GetLength() - 1024)) / 1024f) % BeatDetector.get_energie1024().Length;
                        float energy_1024_music = (float)BeatDetector.get_energie1024()[(int)time_music];

                        position_spawn = new Vector2(1180, r.Next(5, 564));
                        if (mode == GAME_MODE.LIBRE && lastTimeMusic + delay_spawn_libre < time_music)
                        {
                            if ((time_music - lastTimeRandomSpawn > 10) && (BeatDetector.get_beat()[(int)time_music] > 0))
                            {
                                float ratio = energy_1024_music / moy_energie1024;
                                if (ratio > 2.00)
                                {
                                    listeVaisseau.Add(new kamikaze(T_Vaisseau_Kamikaze, position_spawn));
                                }
                                else if (ratio > 1.90)
                                {
                                    listeVaisseau.Add(new Targeter(T_Vaisseau_Targeter, position_spawn));
                                }
                                else if (ratio > 1.83)
                                {
                                    listeVaisseau.Add(new Support(T_Vaisseau_Support, position_spawn));
                                }
                                else if (ratio > 1.75)
                                {
                                    listeVaisseau.Add(new BC(T_Vaisseau_BC, position_spawn));
                                }
                                else if (ratio > 1.7)
                                {
                                    listeVaisseau.Add(new Zebra(T_Vaisseau_Zebra, position_spawn));
                                }
                                else if (ratio > 1.5)
                                {
                                    listeVaisseau.Add(new RapidShooter(T_Vaisseau_Doubleshooter, position_spawn));
                                }
                                else if (ratio > 1.4)
                                {
                                    listeVaisseau.Add(new Blasterer(T_Vaisseau_Energizer, position_spawn));
                                }
                                else if (ratio > 1)
                                {
                                    listeVaisseau.Add(new Drone(T_Vaisseau_Drone, position_spawn));
                                }
                                else
                                {
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
                }
                #endregion
                #region Fin du level

                if ((end || endDead) && (first))
                {
                    NameScene named;
                    AudioPlayer.StopMusic();
                    SoundEffect.MasterVolume = 0.00f;
                    AudioPlayer.PlayMusic("Musiques\\Menu\\Musique.flac");

                    if (mode == GAME_MODE.CAMPAGNE)
                    {
                        if (endDead)
                            named = new NameScene(SceneManager, gameTime, _gameFont, Color.DarkRed);
                        else
                            named = new NameScene(SceneManager, gameTime, _gameFont, Color.Green);

                        SceneManager.AddScene(named);
                        name = named.Name;

                            path_level = "Scores\\Arcade\\lvl" + _level + ".score";
                            FileStream fs = new FileStream(path_level, FileMode.OpenOrCreate);
                            sr_level = new StreamReader(fs);
                            string score_level_test = sr_level.ReadToEnd();
                            StreamWriter sw = new StreamWriter(fs);
                            stock_score_inferieur = "";
                            stock_score_superieur = "";
                            if (score_level_test.Length == 0)
                            {
                                for (int k = score_level_test.Length; k < 10; k++)
                                {
                                    if (k % 2 == 0)
                                        sw.WriteLine("-");
                                    else
                                        sw.WriteLine("0");
                                }
                                sw.Close();
                                fs.Close();
                            }
                            else
                            {
                                fs.Close();
                                score_level = System.IO.File.ReadAllLines(@path_level);


                                if (score_level.Length < 10)
                                {
                                    for (int i = score_level.Length; i < 10; i++)
                                    {
                                        if (i % 2 == 0)
                                            score_level[i] = "-";
                                        else
                                            score_level[i] = "0";
                                    }
                                }
                            }
                            score_level = System.IO.File.ReadAllLines(@path_level);
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
                    else if ((mode == GAME_MODE.EXTREME)&&(!endDead))
                    {
                            named = new NameScene(SceneManager, gameTime, _gameFont, Color.Green);

                            SceneManager.AddScene(named);
                            name = named.Name;
                        score_extreme = Convert.ToInt32(compteur/1000);
                        path_level = "Scores\\Extreme\\lvl.score";
                        FileStream fs = new FileStream(path_level, FileMode.OpenOrCreate);
                        sr_level = new StreamReader(fs);
                        string score_level_test = sr_level.ReadToEnd();
                        StreamWriter sw = new StreamWriter(fs);
                        stock_score_inferieur = "";
                        stock_score_superieur = "";
                        if (score_level_test.Length == 0)
                        {
                            for (int k = score_level_test.Length; k < 10; k++)
                            {
                                if (k % 2 == 0)
                                    sw.WriteLine("-");
                                else
                                    sw.WriteLine("9999");
                            }
                            sw.Close();
                            fs.Close();
                        }
                        else
                        {
                        fs.Close();
                        score_level = System.IO.File.ReadAllLines(@path_level);
                            if (score_level.Length < 10)
                            {
                                for (int i = score_level.Length; i < 10; i++)
                                {
                                    if (i % 2 == 0)
                                        score_level[i] = "-";
                                    else
                                        score_level[i] = "9999";
                                }
                            }
                        }
                        score_level = System.IO.File.ReadAllLines(@path_level);
                            for (int i = 0; i < 10; i += 2)
                            {
                                if (score_extreme > Convert.ToInt32(score_level[i + 1]))
                                    stock_score_inferieur += score_level[i] + '\n' + score_level[i + 1] + '\n';
                                else
                                    stock_score_superieur += score_level[i] + '\n' + score_level[i + 1] + '\n';
                            }

                            sr_level.Close();
                            sw_level = new StreamWriter(path_level);

                            sw_level.WriteLine(stock_score_inferieur + "Nervous" + '\n' + Convert.ToString(score_extreme) + '\n' + stock_score_superieur);
                            sw_level.Close();
                        
                    }
                    first = false;
                }
                if ((end || endDead))//&&typed)
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
                {
                    pause = false;
                    AudioPlayer.PauseMusic();
                }
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
            particleRenderer.RenderEffect(particleEffectMissiles);
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
            #region Draw des AoE
            foreach (supportAoe aoe in listeSupportAoE)
            {
                aoe.recalculerRadius(time, SceneManager.GraphicsDevice);
                Vector2 position = new Vector2((aoe.v.pos.X + aoe.v.sprite.Width / 2) - aoe.radius, (aoe.v.pos.Y + aoe.v.sprite.Height / 2) - aoe.radius);
                aoe.setPosition(position);
                spriteBatch.Draw(aoe.circle, position, Color.Blue); 
            }
            #endregion
            #region Draw des particules de collision
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            particleRenderer.RenderEffect(particleEffect);
            particleRenderer.RenderEffect(particleBossExplosion);
            particleRenderer.RenderEffect(particleExplosionAoE);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            #endregion
            #region Draw des infos sonores
            Rectangle rect;
            Texture2D empty_texture = new Texture2D(SceneManager.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            empty_texture.SetData(new[] { Color.White });

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
            if (mode == GAME_MODE.EXTREME)
                spriteBatch.DrawString(_HUDfont, Convert.ToString(compteur/1000), new Vector2(95, 628), new Color(30, 225, 30));
            else
                spriteBatch.DrawString(_HUDfont, Convert.ToString(score), new Vector2(95, 628), new Color(30, 225, 30));

            if (mode == GAME_MODE.LIBRE)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                rect = new Rectangle(726, 246 + 380, 450, 128);
                spriteBatch.Draw(empty_texture, rect, Color.Black);

                spriteBatch.Draw(T_HUD_musicProgression, new Vector2(726, 626), Color.White);

                int position_progression = (int)(((long)progressionMusic.Length * (long)AudioPlayer.GetCurrentTime()) / (long)AudioPlayer.GetLength() % (long)progressionMusic.Length);
                spriteBatch.Draw(empty_texture, new Rectangle(position_progression + 726, 626 + T_HUD_musicProgression.Height - progressionMusic[position_progression], 1, progressionMusic[position_progression] - 1), Color.Red);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

            Color laser, red_laser, rocket, regen;
            Vector2 rect_vect;
            if (listeVaisseau.Count != 0)
            {
                switch (listeVaisseau[0].armeActuelle)
                {
                    case 0:
                        rect_vect = new Vector2(4, 704);
                        break;
                    case 1:
                        rect_vect = new Vector2(44, 704);
                        break;
                    case 2:
                        rect_vect = new Vector2(84, 704);
                        break;
                    case 3:
                        rect_vect = new Vector2(124, 704);
                        break;
                    default:
                        rect_vect = new Vector2(6, 704);
                        break;
                }

                if (listeVaisseau[0].Energie >= 20)
                    laser = Color.White;
                else
                    laser = Color.Red;

                if (listeVaisseau[0].Energie >= 100)
                    red_laser = Color.White;
                else
                    red_laser = Color.Red;

                if (listeVaisseau[0].Energie >= 200)
                    rocket = Color.White;
                else
                    rocket = Color.Red;

                if ((time >= listeVaisseau[0].tNextEnergyToLife) && (listeVaisseau[0].Energie == listeVaisseau[0].EnergieMax))
                    regen = Color.White;
                else
                    regen = Color.Red;


                spriteBatch.Draw(T_HUD_laser, new Vector2(90, 710), laser);
                spriteBatch.Draw(T_HUD_basic, new Vector2(10, 710), Color.White);
                spriteBatch.Draw(T_HUD_Heavy, new Vector2(50, 710), red_laser);
                spriteBatch.Draw(T_HUD_rocket, new Vector2(130, 710), rocket);
                spriteBatch.Draw(T_HUD_vie, new Vector2(323, 712), regen);
                spriteBatch.Draw(T_HUD_red_rect, rect_vect, Color.White);
            }
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
                if(mode != GAME_MODE.EXTREME)
                spriteBatch.DrawString(_gameFont, Convert.ToString(score), new Vector2(530,276), Color.Green);
                else
                    spriteBatch.DrawString(_gameFont, Convert.ToString(score_extreme), new Vector2(530, 276), Color.Green);

            }
            else if (endDead)
            {
                spriteBatch.Draw(T_Divers_Levelfail, new Vector2(0, 0), Color.White);
            }
            #endregion

            if(endDead)
           spriteBatch.DrawString(_gameFont, name, new Vector2(0, 300), Color.Red);
            else
                spriteBatch.DrawString(_gameFont, name, new Vector2(0, 300), Color.Red);
            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }
    }
}
