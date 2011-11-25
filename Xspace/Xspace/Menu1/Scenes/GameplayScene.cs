using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
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
using Microsoft.Xna.Framework.GamerServices;


namespace MenuSample.Scenes
{
    /// <summary>
    /// Le jeu!
    /// </summary>
    public class GameplayScene : AbstractGameScene
    {

        private ContentManager _content;
        private SpriteFont _gameFont;
        private Vector2 _playerPosition = new Vector2(100, 100);
        private Vector2 _enemyPosition = new Vector2(100, 100);
        private readonly Random _random = new Random();
        private float _pauseAlpha;
        public SpriteBatch spriteBatch;
        private Texture2D textureVaisseau_joueur, textureMissile_joueur_base, textureMissile_ennemi1, textureVaisseau_ennemi1;
        private Song musique;
        private SoundEffect musique_tir, _musique_tir;
        private KeyboardState keyboardState;
        private Xspace.gestionLevels thisLevel;
        private List<Xspace.gestionLevels> infLevel;
        // TODO : Déclaration de tous les objets Vaisseau en dessous
        private Xspace.Vaisseau_joueur joueur1;
        private Xspace.Vaisseau_ennemi[] vaisseauDrone;
        List<Xspace.Vaisseau_ennemi> listeVaisseauEnnemi, listeVaisseauEnnemiToRemove;
        List<Xspace.Missiles[]> listeMissile, listeMissileToRemove;
        // TODO : Déclaration de tous les objets missiles en dessous
        Xspace.Missiles[] missileJoueur;
        Xspace.Missiles[] missileEnnemi;
        Xspace.Missiles missiles;
        int nbreMaxMissiles, i = 0, j = 0, actualDrone = 0;
        int nbreMaxMissiles_e;
        float fps_fix;
        double time, lastTime;
        char[] delimitationFilesInfo = new char[] { ' ' };
        char[] delimitationFilesInfo2 = new char[] { ';' };
        char[] delimitationFilesInfo3 = new char[] { ':' };



        public GameplayScene(SceneManager sceneMgr)
            : base(sceneMgr)
        {

            nbreMaxMissiles = 15;
            lastTime = 0;
            nbreMaxMissiles_e = 20;

        }



        public override void Initialize()
        {
		    base.Initialize();

         }

        private Xspace.ScrollingBackground fond_ecran;
        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(SceneManager.Game.Services, "Content");
            Texture2D _textureVie, _textureContourVie;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            musique =  _content.Load<Song>("wow-music1");
            MediaPlayer.Play(musique);
            musique_tir = _content.Load<SoundEffect>("musique_tir");
            fond_ecran = new Xspace.ScrollingBackground();
            Texture2D fond_image = _content.Load<Texture2D>("space_bg");
            fond_ecran.Load(GraphicsDevice, fond_image);
            thisLevel = new Xspace.gestionLevels(0);
            infLevel = new List<Xspace.gestionLevels>();
            _textureContourVie = _content.Load<Texture2D>("contourvie");
            _textureVie = _content.Load<Texture2D>("vie");
            Xspace.Definition.texturevie =_textureVie;
            Xspace.Definition.texturecontourvie = _textureContourVie;
            // TODO : Chargement de toutes les textures des vaisseau en dessous
            textureVaisseau_joueur = _content.Load<Texture2D>("Vaisseau_joueur");
            textureVaisseau_ennemi1 = _content.Load<Texture2D>("Vaisseau_ennemi1");


            _gameFont = _content.Load<SpriteFont>("gamefont");

            // Un vrai jeu possède évidemment plus de contenu que ça, et donc cela prend
            // plus de temps à charger. On simule ici un chargement long pour que vous
            // puissiez admirer la magnifique scène de chargement. :p
            Thread.Sleep(1000);

            // En cas de longs période de traitement, appelez cette méthode *tintintin*.
            // Elle indique au mécanisme de synchronisation du jeu que vous avez fini un
            // long traitement, et qu'il ne devrait pas essayer de rattraper le retard.
            // Cela évite un lag au début du jeu.
            SceneManager.Game.ResetElapsedTime();

            // TODO : Chargement de toutes les textures des missiles en dessous
            textureMissile_joueur_base = _content.Load<Texture2D>("missile1");
            textureMissile_ennemi1 = _content.Load<Texture2D>("missile1_e");

            // TODO : Chargement de tous les objets vaisseau en dessous
            listeVaisseauEnnemi = new List<Xspace.Vaisseau_ennemi>();
            listeVaisseauEnnemiToRemove = new List<Xspace.Vaisseau_ennemi>();
            vaisseauDrone = new Xspace.Vaisseau_ennemi[100];
            joueur1 = new Xspace.Vaisseau_joueur(textureVaisseau_joueur);

            // effets sonores
           // SoundEffectInstance _musique_tir = musique_tir.CreateInstance();

            // TODO : Chargement de tous les objets missiles en dessous
            listeMissile = new List<Xspace.Missiles[]>();
            listeMissileToRemove = new List<Xspace.Missiles[]>();
            missileJoueur = new Xspace.Missiles[nbreMaxMissiles];
            missileEnnemi = new Xspace.Missiles[nbreMaxMissiles_e];
            missiles = new Xspace.Missiles(textureMissile_ennemi1, true, 50);
            for (int i = 0; i < nbreMaxMissiles; i++)
                missileJoueur[i] = new Xspace.Missiles(textureMissile_joueur_base, false, 50);

            for (int i = 0; i < nbreMaxMissiles - 1; i++)
            {
                if (missileJoueur[i] != null)
                    missileJoueur[i].initialiserTexture(textureMissile_joueur_base);
            }

            listeMissile.Add(missileJoueur);

            for (int i = 0; i < nbreMaxMissiles_e; i++)
                missileEnnemi[i] = new Xspace.Missiles(textureMissile_ennemi1, true, 50);

            for (int i = 0; i < nbreMaxMissiles_e - 1; i++)
            {
                if (missileEnnemi[i] != null)
                    missileEnnemi[i].initialiserTexture(textureMissile_ennemi1);
            }
            listeMissile.Add(missileEnnemi);


            // TODO : Chargement du level en dessous

            foreach (string info in thisLevel.getInfosLevel) // Pour chacune des lignes du level ...
            {
                int timing = 0;
                string categorie = "", type = "", position = "";
                Xspace.Vaisseau_ennemi vaisseau = null;
                foreach (string info2 in info.Split(delimitationFilesInfo)) // ... On récupère 2 infos : le type de l'objet et à quelle date il doit spawn
                {
                    i = 0;
                    if (!int.TryParse(info2, out timing)) // SI l'info n'est pas un nombre, alors c'est la catégorie de l'objet (vaisseau, bonus, obstacle, etc.)
                    {
                        foreach (string info3 in info2.Split(delimitationFilesInfo3))
                        {
                            if(info3.Contains(";")) // Si on trouve le caratère ";", alors c'est les infos level (ex : vaisseau;drone)
                            {
                                foreach (string info4 in info3.Split(delimitationFilesInfo2))
                                {
                                    if (i == 0) // Premiere info : catégorie de l'objet
                                    {
                                        categorie = info4;
                                    }
                                    else // Deuxième info : type de l'objet
                                    {
                                        type = info4;
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                position = info3;
                            }
                        }
                    }
                    else
                        timing = int.Parse(info2);
                    
                }
                //Fin de lecture de la ligne : on ajoute un élement dans la liste des infos du level
                if (categorie == "vaisseau")
                {
                    switch (type)
                    {
                        case "drone":
                            vaisseau = vaisseauDrone[actualDrone] = new Xspace.Vaisseau_ennemi(textureVaisseau_ennemi1, "drone", position);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (categorie == "drone")
                        Game.Exit();
                }

                infLevel.Add(new Xspace.gestionLevels(categorie, vaisseau, timing, position));
                

            }
        }


        // gestion des collisions
        bool collisions(List<Xspace.Vaisseau_ennemi> listeVaisseau, List<Xspace.Missiles[]> listeMissiles)
        {

            int vaisseauActuel = 0, missileActuel = 0;
            foreach(Xspace.Vaisseau_ennemi vaisseau in listeVaisseau)
            {
                vaisseauActuel = listeVaisseau.IndexOf(vaisseau);
                foreach (Xspace.Missiles[] missile in listeMissiles)
                {
                    missileActuel = listeMissiles.IndexOf(missile);
                    for (int k = 0; k < 15; k++)
                    {
                        
                        
                            if (listeMissile[missileActuel][k].existe)                           
                            {

                                if (((listeMissiles[missileActuel][k].position.X + listeMissiles[missileActuel][k].sprite.Width > listeVaisseau[vaisseauActuel].position.X)
                                    && (listeMissiles[missileActuel][k].position.X + listeMissiles[missileActuel][k].sprite.Width < listeVaisseau[vaisseauActuel].position.X + listeVaisseau[vaisseauActuel].sprite.Width))
                                    && ((listeMissiles[missileActuel][k].position.Y + listeMissiles[missileActuel][k].sprite.Height / 2 > listeVaisseau[vaisseauActuel].position.Y)
                                    && (listeMissiles[missileActuel][k].position.Y + listeMissiles[missileActuel][k].sprite.Height / 2 < listeVaisseau[vaisseauActuel].position.Y + listeVaisseau[vaisseauActuel].sprite.Height))
                                    )
                                {
                                    
                                    
                                        // Collision missile => Vaisseau trouvée

                                    if (listeMissile[missileActuel][k].ennemi == false)
                                    {
                                        listeMissiles[missileActuel][k].kill();

                                        if (listeVaisseau[vaisseauActuel].hurt(listeMissiles[missileActuel][k].degats) == true)
                                        {
                                            // Vaisseau dead
                                            
                                            listeVaisseau[vaisseauActuel].kill();
                                            
                                        }
                                    }
                                    
                                    
                                    
                                }
                            }
                        
                    }
                }
            }

            return true;
        }
                public override void HandleInput()
        {
            
            KeyboardState keyboardState = InputState.CurrentKeyboardState;

            // Le menu de pause s'enclenche si un joueur appuie sur la touche assignée
            // au menu de pause, ou lorsque qu'une manette branchée est déconnectée

            if (InputState.IsPauseGame())
                new PauseMenuScene(SceneManager, this).Add();
            else
            {
                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    if (joueur1._emplacement.Y - joueur1._textureVaisseau.Height / 2 + 20 >= 0)
                        joueur1._emplacement -= joueur1._deplacementDirectionY * joueur1._vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (joueur1._emplacement.Y - joueur1._textureVaisseau.Height / 2 - 10 <= 540)
                        joueur1._emplacement += joueur1._deplacementDirectionY * joueur1._vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    if (joueur1._emplacement.X - joueur1._textureVaisseau.Width / 2 + 20 >= 0)
                        joueur1._emplacement -= joueur1._deplacementDirectionX * joueur1._vitesseVaisseau * fps_fix;
                }

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (joueur1._emplacement.X - joueur1._textureVaisseau.Width / 2 - 10 <= 1085)
                        joueur1._emplacement += joueur1._deplacementDirectionX * joueur1._vitesseVaisseau * fps_fix;
                }
               
            }
        }

        public override void Update(GameTime gameTime, bool othersceneHasFocus, bool coveredByOtherscene)
        {

            fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime, othersceneHasFocus, false);

            _pauseAlpha = coveredByOtherscene
                ? Math.Min(_pauseAlpha + 1f / 32, 1)
                : Math.Max(_pauseAlpha - 1f / 32, 0);

            if (InputState.IsPauseGame())
            {
                MediaPlayer.Pause();
            }


            foreach (Xspace.gestionLevels spawn in infLevel)
            {
                if (spawn.isTime(time))
                {
                    spawn.makeItSpawn();
                    switch (spawn.Categorie)
                    {
                        case "vaisseau":
                            if(spawn.Adresse != null)
                                listeVaisseauEnnemi.Add(spawn.Adresse);
                            else
                                Game.Exit();
                            break;
                        default:
                            break;
                    }
                }
            }

            fond_ecran.Update(fps_fix);
            keyboardState = Keyboard.GetState();
            // affichage des missiles des joueurs
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                for (int i = 0; i < nbreMaxMissiles; i++)
                {
                    if (missileJoueur[i] != null && missileJoueur[i].estAffiche == false && (time - lastTime > 150 || lastTime == 0))
                    {
                        musique_tir.Play();
                            missileJoueur[i].afficherMissile(joueur1.position);
                            lastTime = time;
                            break;
                    }
                }
            }
            
            // affichage des missiles des ennemis
            foreach (Xspace.Vaisseau_ennemi vaisseau in listeVaisseauEnnemi)
            {
                
                if (vaisseau.existe)
                {
                    for (int i = 0; i < nbreMaxMissiles_e; i++)
                    {

                        if (missileEnnemi[i] != null && missileEnnemi[i].estAffiche == false && (time - vaisseau.lastTir > vaisseau.timingAttack) || (vaisseau.lastTir == 0))
                        {
                            Vector2 spawnPosition = new Vector2(vaisseau.position.X -100, vaisseau.position.Y);
                            missileEnnemi[i].afficherMissile(spawnPosition);
                            vaisseau.lastTir = time;
                            
                        }
                    }  
                }           
            }

            
            // deplacement missile joueur
            for (int i = 0; i < nbreMaxMissiles; i++)
            {
                if ((missileJoueur[i] != null && missileJoueur[i].estAffiche))
                {
                    missileJoueur[i].avancerMissile(fps_fix);
                }

                else
                {
                    missileJoueur[i].avancerMissile(fps_fix);
                }

            }

            // deplacement missiles ennemis
            for (int i = 0; i < nbreMaxMissiles_e; i++)
            {
                if ((missileEnnemi[i] != null) && missileEnnemi[i].estAffiche)
                {
                    missileEnnemi[i].avancerMissile_enemi1(fps_fix);
                }

            }

            collisions(listeVaisseauEnnemi, listeMissile);

            foreach (Xspace.Vaisseau_ennemi vaisseau in listeVaisseauEnnemi)
            {
                if (vaisseau.existe == false)
                {
                    listeVaisseauEnnemiToRemove.Add(vaisseau);
                    i++;
                }
                else
                    vaisseau.Update(fps_fix);
            }

            for (j = 0; j < i; j++)
            {
                    listeVaisseauEnnemi.Remove(listeVaisseauEnnemiToRemove[j]);
            }

            listeVaisseauEnnemiToRemove.Clear();
            i = 0;
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SceneManager.SpriteBatch;
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 0, 0);
           // spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            fond_ecran.Draw(spriteBatch);
            joueur1.Draw(spriteBatch); // Draw du joueur
            
            foreach (Xspace.Vaisseau_ennemi vaisseau in listeVaisseauEnnemi)
            {
                vaisseau.Draw(spriteBatch);
            }
            for (int i = 0; i < nbreMaxMissiles - 1; i++)
            {
                if (missileJoueur[i] != null && missileJoueur[i].existe)
                    missileJoueur[i].Draw(spriteBatch);
            }

            for (int i = 0; i < nbreMaxMissiles_e - 1; i++)
            {
                if (missileEnnemi[i] != null && missileEnnemi[i].existe)
                    missileEnnemi[i].Draw(spriteBatch);
            }
            base.Draw(gameTime);

            spriteBatch.End();

            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                SceneManager.FadeBackBufferToBlack(alpha);
            }
            
        }

        protected override void UnloadContent()
        {
            _content.Unload();
        }


    }
}

    

