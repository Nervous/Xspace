using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LevelEditor
{
    public partial class LevelEditor : Form
    {
        bool draw_all_black;
        Image saveimage;
        int selected, page, ligne;
        bool hasClicked, reset;
        string path = "level.xpa";
        protected Stream stream;
        List<int> emplacements = new List<int>();


        public LevelEditor()
        {
            draw_all_black = true;
            selected = 0;
            page = 0;
            ligne = 0;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e) //drone 1
        {
            
            if ((selected!=1)&&(!reset)) // Clic de base pour séléctionner le vaisseau
            {
                saveimage = pictureBox4.Image;
                resetPictures(sender, e, 1);
                pictureBox4.Image = global::LevelEditor.Properties.Resources.dronebrillance;
                selected = 1;             
            }
            else if (!reset) // On retire le rectangle rouge quand le joueur reclique 
            {
                pictureBox4.Image = global::LevelEditor.Properties.Resources.drone;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.empty;
            }
            else if (selected == 1)
            {// En cas de reset (suppression du rectangle)
                pictureBox4.Image = global::LevelEditor.Properties.Resources.drone;
            }
        }

        private void resetPictures(object sender, EventArgs e, int avoid)
        {
            reset = true;
            if(avoid != 1)
            pictureBox4_Click(sender, e); 

            if (avoid != 2)
            pictureBox3_Click(sender, e);

            if(avoid != 3)
                pictureBox1_Click(sender, e);

            if(avoid != 4)
                pictureBox2_Click(sender, e);

            if(avoid != 5)
                pictureBox6_Click_1(sender, e);

            reset = false;
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if ((selected != 2)&&(!reset))
            {
                resetPictures(sender, e, 2);
                saveimage = pictureBox3.Image;
                pictureBox3.Image = global::LevelEditor.Properties.Resources.doubleshooterbrillance;
                selected = 2;
            }
            else if (!reset)
            {
                pictureBox3.Image = global::LevelEditor.Properties.Resources.doubleshooter;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.empty;
            }
            else if (selected == 2)
            {
                pictureBox3.Image = global::LevelEditor.Properties.Resources.doubleshooter;
                
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Quitter l'éditeur de niveaux ?",
                     "Message de confirmation",
                         MessageBoxButtons.YesNo) == DialogResult.Yes)
                Close();   
        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Import");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            pictureBox7.Image = saveimage;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if ((selected != 3) && (!reset))
            {
                resetPictures(sender, e, 3);
                saveimage = pictureBox1.Image;
                pictureBox1.Image = global::LevelEditor.Properties.Resources.energizerbrillanc;
                selected = 3;
            }
            else if (!reset)
            {
                pictureBox1.Image = global::LevelEditor.Properties.Resources.energizer;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.empty;
            }
            else if (selected == 3)
                pictureBox1.Image = global::LevelEditor.Properties.Resources.energizer;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if ((selected!= 4) && (!reset))
            {
                resetPictures(sender, e, 4);
                saveimage = pictureBox2.Image;
                pictureBox2.Image = global::LevelEditor.Properties.Resources.Kamikazebrillance;
                selected = 4;
            }
            else if (!reset)
            {
                pictureBox2.Image = global::LevelEditor.Properties.Resources.Kamikaze;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.empty;
            }
            else if (selected == 4)
                pictureBox2.Image = global::LevelEditor.Properties.Resources.Kamikaze;
        }

        private void pictureBox6_Click_1(object sender, EventArgs e)
        {
            if ((selected!= 5) && (!reset))
            {
                resetPictures(sender, e, 5);
                saveimage = pictureBox6.Image;
                pictureBox6.Image = global::LevelEditor.Properties.Resources.emptybrillance;
                selected = 5;
            }
            else if (!reset)
            {
                pictureBox6.Image = global::LevelEditor.Properties.Resources.empty;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.empty;
            }
            else if (selected == 5)
                pictureBox6.Image = global::LevelEditor.Properties.Resources.empty;
        }

        private void pictureBox8_Click_1(object sender, EventArgs e)
        {
            string position = "1";
            string time = Convert.ToString(page + 200);
            Stream stream = new FileStream("level.xpa",FileMode.Open);
            StreamWriter sw = new StreamWriter(stream);
            StreamReader sr = new StreamReader(stream);
            string vaisseau;
            string text;

            switch (selected)
            {
                case 1:
                    vaisseau = "drone";
                    break;
                case 2:
                    vaisseau = "doubleshooter";
                    break;
                case 3:
                    vaisseau = "energizer";
                    break;
                case 4:
                    vaisseau = "kamikaze";
                    break;
                default:
                    vaisseau = "drone";
                    break;
            }

            text = sr.ReadToEnd();
            string[] textArray = text.Split('\n');

            if (pictureBox8.Image != global::LevelEditor.Properties.Resources.empty) // on remplace une ancienne ligne
            {
                try
                {
                    textArray[Convert.ToInt16(position) + page * 10] = ("vaisseau" + ";" + vaisseau + ":" + position + " " + time);
                }
                catch
                { }

                for (int i = 0; i < textArray.Length; i++)
                {
                    sw.WriteLine(textArray[i]);
                }
            }
            else // nouvelle ligne
            {

                if (selected != 0)
                {
                    sw.WriteLine("vaisseau" + ";" + vaisseau + ":" + position + " " + time);
                }
                else
                    sw.WriteLine("");

                emplacements.Add(Convert.ToInt16(position) + page * 10); //On ajoute 
            }

            sw.Close();
            
            
            pictureBox8.Image = saveimage;
        }
        

    }
}
