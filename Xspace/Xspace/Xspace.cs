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
    public class Xspace : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        private Texture2D textureVaisseau_joueur, textureMissile_joueur_base;
        private Song musique;
        private KeyboardState keyboardState;
        private gestionLevels thisLevel;
        private List<gestionLevels> infLevel;
        // TODO : Déclaration de tous les objets Vaisseau en dessous
        private Vaisseau_joueur joueur1;
        private Vaisseau_ennemi[] vaisseauDrone;
        List<Vaisseau_ennemi> listeVaisseauEnnemi, listeVaisseauEnnemiToRemove;
        List<Missiles[]> listeMissile, listeMissileToRemove;
        // TODO : Déclaration de tous les objets missiles en dessous
        Missiles[] missileJoueur;
        int nbreMaxMissiles, i = 0, j = 0, actualDrone = 0;
        float fps_fix;
        double time, lastTime;
        char[] delimitationFilesInfo = new char[] { ' ' };
        char[] delimitationFilesInfo2 = new char[] { ';' };
        char[] delimitationFilesInfo3 = new char[] { ':' };
        bool missileType1;
        bool missileType2;

        public Xspace()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1180,
                PreferredBackBufferHeight = 620
            };
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            nbreMaxMissiles = 15;
            lastTime = 0;
        }


        protected override void Initialize()
        {
		    base.Initialize();

         }

        private ScrollingBackground fond_ecran;
        protected override void LoadContent()
        {
            Texture2D _textureVie, _textureContourVie;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            musique = Content.Load<Song>("against_the_waves");
            MediaPlayer.Play(musique);
            fond_ecran = new ScrollingBackground();
            Texture2D fond_image = Content.Load<Texture2D>("space_bg");
            fond_ecran.Load(GraphicsDevice, fond_image);
            thisLevel = new gestionLevels(0);
            infLevel = new List<gestionLevels>();
            _textureContourVie = Content.Load<Texture2D>("contourvie");
            _textureVie = Content.Load<Texture2D>("vie");
            Definition.texturevie =_textureVie;
            Definition.texturecontourvie = _textureContourVie;
            // TODO : Chargement de toutes les textures des vaisseau en dessous
            textureVaisseau_joueur = Content.Load<Texture2D>("Vaisseau_joueur"); 

            // TODO : Chargement de toutes les textures des missiles en dessous
            textureMissile_joueur_base = Content.Load<Texture2D>("missile1");

            // TODO : Chargement de tous les objets vaisseau en dessous
            listeVaisseauEnnemi = new List<Vaisseau_ennemi>();
            listeVaisseauEnnemiToRemove = new List<Vaisseau_ennemi>();
            vaisseauDrone = new Vaisseau_ennemi[100];
            joueur1 = new Vaisseau_joueur(textureVaisseau_joueur);
            

            // TODO : Chargement de tous les objets missiles en dessous
            listeMissile = new List<Missiles[]>();
            listeMissileToRemove = new List<Missiles[]>();
            missileJoueur = new Missiles[nbreMaxMissiles];
            for (int i = 0; i < nbreMaxMissiles; i++)
                missileJoueur[i] = new Missiles(textureMissile_joueur_base, false, 50);

            for (int i = 0; i < nbreMaxMissiles - 1; i++)
            {
                if (missileJoueur[i] != null)
                    missileJoueur[i].initialiserTexture(textureMissile_joueur_base);
            }

            listeMissile.Add(missileJoueur);


            // TODO : Chargement du level en dessous

            foreach (string info in thisLevel.getInfosLevel) // Pour chacune des lignes du level ...
            {
                int timing = 0;
                string categorie = "", type = "", position = "";
                Vaisseau_ennemi vaisseau = null;
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
                            vaisseau = vaisseauDrone[actualDrone] = new Vaisseau_ennemi(textureVaisseau_joueur, "drone", position);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (categorie == "drone")
                        Exit();
                }

                infLevel.Add(new gestionLevels(categorie, vaisseau, timing, position));
                

            }
        }



        bool collisions(List<Vaisseau_ennemi> listeVaisseau, List<Missiles[]> listeMissiles)
        {

            int vaisseauActuel = 0, missileActuel = 0;
            foreach(Vaisseau_ennemi vaisseau in listeVaisseau)
            {
                vaisseauActuel = listeVaisseau.IndexOf(vaisseau);
                foreach (Missiles[] missile in listeMissiles)
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

            return true;
        }

        protected override void Update(GameTime gameTime)
        {

            fps_fix = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (gestionLevels spawn in infLevel)
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
                                Exit();
                            break;
                        default:
                            break;
                    }
                }
            }

            fond_ecran.Update(fps_fix);
            joueur1.Update(fps_fix); // Update du joueur
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                for (int i = 0; i < nbreMaxMissiles; i++)
                {
                    if (missileJoueur[i] != null && missileJoueur[i].estAffiche == false && (time - lastTime > 150 || lastTime == 0))
                    {
                        
                            missileJoueur[i].afficherMissile(joueur1.position);
                            lastTime = time;
                            break;
                    }
                }
            }
             


            for (int i = 0; i < nbreMaxMissiles; i++)
            {
                if ((missileJoueur[i] != null && missileJoueur[i].estAffiche) && (missileType1))
                {
                    missileJoueur[i].avancerMissile(fps_fix);
                }
                else
                {
                    missileJoueur[i].avancerMissile_2(fps_fix);
                }

            }

            collisions(listeVaisseauEnnemi, listeMissile);

            foreach (Vaisseau_ennemi vaisseau in listeVaisseauEnnemi)
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
           // spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            fond_ecran.Draw(spriteBatch);
            joueur1.Draw(spriteBatch); // Draw du joueur
            
            foreach (Vaisseau_ennemi vaisseau in listeVaisseauEnnemi)
            {
                vaisseau.Draw(spriteBatch);
            }
            for (int i = 0; i < nbreMaxMissiles - 1; i++)
            {
                if (missileJoueur[i] != null && missileJoueur[i].existe)
                    missileJoueur[i].Draw(spriteBatch);
            }
            base.Draw(gameTime);
            spriteBatch.End();
            
            
        }

        protected override void UnloadContent()
        {

        }

        
    }
}
