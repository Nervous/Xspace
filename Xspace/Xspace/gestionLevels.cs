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
    class gestionLevels
    {
        int nbLevel;
        string pathLevel;
        string[] infosLevel;


        string categorie;
        Vaisseau_ennemi adresse;
        double time;
        bool hasSpawned;

        public gestionLevels(int numero)
        {
            nbLevel = numero;
            pathLevel = "levels/" + nbLevel + ".xpa"; ;
            infosLevel = lireFichier(pathLevel);
        }

        public gestionLevels(string setCategorie, Vaisseau_ennemi setAdresse, int setTime)
        {
            categorie = setCategorie;
            adresse = setAdresse;
            time = setTime;
            hasSpawned = false;
        }

        public string[] lireFichier(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(@path);
            return lines;
        }

        public string[] getInfosLevel
        {
            get { return infosLevel; }
        }

        public Vaisseau_ennemi Adresse
        {
            get { return adresse; }
        }

        public string Categorie
        {
            get { return categorie; }
        }

        public bool isTime(double actualTime)
        {
            bool toReturn = false;
            if (actualTime > this.time && hasSpawned == false)
                toReturn = true;
            else
                toReturn = false;
            if(toReturn == true)
            this.hasSpawned = true;
            return toReturn;
        }

        public void makeItSpawn()
        {
            switch (categorie)
            {
                case "vaisseau":
                    if(adresse != null)
                        this.adresse.creer();
                    break;
                default:
                    break;
            }
        }
    }
}
