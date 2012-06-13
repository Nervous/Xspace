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
        int compteur;
        Image saveimage;
        int selected, page;
        bool reset;
        protected Stream stream;
        List<int> emplacements = new List<int>();
        List<System.Windows.Forms.PictureBox> boxList = new List<System.Windows.Forms.PictureBox>();

        public LevelEditor()
        {
            InitializeComponent();
            saveimage = pictureBox4.Image;
            resetPictures(new object(), new EventArgs(), 1);
            pictureBox4.Image = global::LevelEditor.Properties.Resources.dronebrillance;
            selected = 1;   
             Plot plot = new Plot("3", "0", 0, 0, 0, pictureBox8, global::LevelEditor.Properties.Resources.Sans_titre);
            #region list/arrays
            boxList.Add(pictureBox7);
            boxList.Add(pictureBox8);
            boxList.Add(pictureBox9);
            boxList.Add(pictureBox10);
            boxList.Add(pictureBox11);
            boxList.Add(pictureBox12);
            boxList.Add(pictureBox13);
            boxList.Add(pictureBox14);
            boxList.Add(pictureBox15);
            boxList.Add(pictureBox24);
            boxList.Add(pictureBox23);
            boxList.Add(pictureBox22);
            boxList.Add(pictureBox21);
            boxList.Add(pictureBox20);
            boxList.Add(pictureBox19);
            boxList.Add(pictureBox18);
            boxList.Add(pictureBox17);
            boxList.Add(pictureBox16);
            boxList.Add(pictureBox31);
            boxList.Add(pictureBox41);
            boxList.Add(pictureBox40);
            boxList.Add(pictureBox39);
            boxList.Add(pictureBox38);
            boxList.Add(pictureBox37);
            boxList.Add(pictureBox36);
            boxList.Add(pictureBox35);
            boxList.Add(pictureBox34);
            boxList.Add(pictureBox30);
            boxList.Add(pictureBox33);
            boxList.Add(pictureBox32);
            boxList.Add(pictureBox51);
            boxList.Add(pictureBox50);
            boxList.Add(pictureBox49);
            boxList.Add(pictureBox48);
            boxList.Add(pictureBox47);
            boxList.Add(pictureBox46);
            boxList.Add(pictureBox29);
            boxList.Add(pictureBox45);
            boxList.Add(pictureBox44);
            boxList.Add(pictureBox43);
            boxList.Add(pictureBox42);
            boxList.Add(pictureBox54);
            boxList.Add(pictureBox53);
            boxList.Add(pictureBox62);
            boxList.Add(pictureBox52);
            boxList.Add(pictureBox25);
            boxList.Add(pictureBox61);
            boxList.Add(pictureBox60);
            boxList.Add(pictureBox59);
            boxList.Add(pictureBox58);
            boxList.Add(pictureBox57);
            boxList.Add(pictureBox56);
            boxList.Add(pictureBox55);
            boxList.Add(pictureBox67);
            boxList.Add(pictureBox26);
            boxList.Add(pictureBox66);
            boxList.Add(pictureBox72);
            boxList.Add(pictureBox69);
            boxList.Add(pictureBox71);
            boxList.Add(pictureBox75);
            boxList.Add(pictureBox80);
            boxList.Add(pictureBox79);
            boxList.Add(pictureBox81);
            boxList.Add(pictureBox27);
            boxList.Add(pictureBox65);
            boxList.Add(pictureBox63);
            boxList.Add(pictureBox68);
            boxList.Add(pictureBox70);
            boxList.Add(pictureBox78);
            boxList.Add(pictureBox77);
            boxList.Add(pictureBox76);
            boxList.Add(pictureBox74);
            page = 0;
            compteur = 0;
            #endregion
        }

        #region Gestion des cases de séléction
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
                saveimage = global::LevelEditor.Properties.Resources.Sans_titre;
            }
            else if (selected == 1)
            {// En cas de reset (suppression du rectangle)
                pictureBox4.Image = global::LevelEditor.Properties.Resources.drone;
            }
        }

        private void pictureBox64_Click(object sender, EventArgs e) // bonus weapon base
        {
            if ((selected != 6) && (!reset))
            {
                saveimage = pictureBox64.Image;
                resetPictures(sender, e, 6);
                pictureBox64.Image = global::LevelEditor.Properties.Resources.DoubleBaseWeaponbrillance;
                selected = 6;
            }
            else if (!reset) 
            {
                pictureBox64.Image = global::LevelEditor.Properties.Resources.DoubleBaseWeapon;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.Sans_titre;
            }
            else if (selected == 6)
            {
                pictureBox64.Image = global::LevelEditor.Properties.Resources.DoubleBaseWeapon;
            }
        }

        private void pictureBox28_Click(object sender, EventArgs e) // bonus vie
        {
            if ((selected != 7) && (!reset)) 
            {
                saveimage = pictureBox28.Image;
                resetPictures(sender, e, 7);
                pictureBox28.Image = global::LevelEditor.Properties.Resources.Lifebrillance;
                selected = 7;
            }
            else if (!reset)  
            {
                pictureBox28.Image = global::LevelEditor.Properties.Resources.Life;
                selected = 0;
                saveimage = global::LevelEditor.Properties.Resources.Sans_titre;
            }
            else if (selected == 7)
            {
                pictureBox28.Image = global::LevelEditor.Properties.Resources.Life;
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

            if (avoid != 6)
                pictureBox64_Click(sender, e);

            if (avoid != 7)
                pictureBox28_Click(sender, e);

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
                saveimage = global::LevelEditor.Properties.Resources.Sans_titre;
            }
            else if (selected == 2)
            {
                pictureBox3.Image = global::LevelEditor.Properties.Resources.doubleshooter;
                
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if ((selected != 4) && (!reset))
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
                saveimage = global::LevelEditor.Properties.Resources.Sans_titre;
            }
            else if (selected == 4)
                pictureBox2.Image = global::LevelEditor.Properties.Resources.Kamikaze;
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
                saveimage = global::LevelEditor.Properties.Resources.Sans_titre;
            }
            else if (selected == 3)
                pictureBox1.Image = global::LevelEditor.Properties.Resources.energizer;
        }

        private void quitterToolStripMenuItem1_Click(object sender, EventArgs e)
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
        #endregion

        #region Gestion de toutes les cases
        private void pictureBox8_Click_1(object sender, EventArgs e) // pos 2
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox8, saveimage);
            compteur = plot2.New_compteur();
        }
        protected void pictureBox7_Click(object sender, EventArgs e)
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox7, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox9, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox10, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox11, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox12, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox13, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox14, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox15, saveimage);
            compteur = plot2.New_compteur();
        }
        private void pictureBox24_Click(object sender, EventArgs e) // 2e ligne temps 600 ( 400 / lignes)
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox24, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox23, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox22, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox21, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox20, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox19, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox18, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox17, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox16, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox31_Click(object sender, EventArgs e) // nouvelle ligne
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox31, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox41_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox41, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox40_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox40, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox39_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox39, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox38_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox38, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox37_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox37, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox36_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox36, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox35, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox34_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 1200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox34, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox30_Click(object sender, EventArgs e) // ligne 4
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox30, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox33_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox33, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox32_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox32, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox51_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox51, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox50_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox50, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox49_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox49, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox48_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox48, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox47_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox47, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox46_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 1600);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox46, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox29_Click(object sender, EventArgs e) // 5e ligne
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox29, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox45_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox45, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox44_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox44, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox43_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox43, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox42_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox42, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox54_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox54, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox53_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox53, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox62_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox62, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox52_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 2000);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox52, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox25_Click(object sender, EventArgs e) // 6e ligne
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox25, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox61_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox61, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox60_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox60, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox59_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox59, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox58_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox58, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox57_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox57, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox56_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox56, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox55_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox55, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox67_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 2400);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox67, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox26_Click(object sender, EventArgs e) // ligne 7
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox26, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox66_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox66, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox72_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox72, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox69_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox69, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox71_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox71, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox75_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox75, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox80_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox80, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox79_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox79, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox81_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 2800);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox81, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox27_Click(object sender, EventArgs e) // ligne 8
        {
            string position = "1";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox27, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox65_Click(object sender, EventArgs e)
        {
            string position = "2";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox65, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox63_Click(object sender, EventArgs e)
        {
            string position = "3";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox63, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox68_Click(object sender, EventArgs e)
        {
            string position = "4";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox68, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox70_Click(object sender, EventArgs e)
        {
            string position = "5";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox70, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox78_Click(object sender, EventArgs e)
        {
            string position = "6";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox78, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox77_Click(object sender, EventArgs e)
        {
            string position = "7";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox77, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox76_Click(object sender, EventArgs e)
        {
            string position = "8";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox76, saveimage);
            compteur = plot2.New_compteur();
        }

        private void pictureBox74_Click(object sender, EventArgs e)
        {
            string position = "9";
            string time = Convert.ToString(3200 * page + 3200);
            Plot plot2 = new Plot(position, time, page, compteur, selected, pictureBox74, saveimage);
            compteur = plot2.New_compteur();
        }
        #endregion
        private void button2_Click(object sender, EventArgs e) // next
        {
            string position = "1";
            string time = Convert.ToString(2800 * page + 400);
            page++;
            Plot plot = new Plot(position, time, page, 0, selected+10, pictureBox8, global::LevelEditor.Properties.Resources.empty);
            plot.Load(boxList, page);

        }

        #region Bouton précédent
         private void button1_Click_1(object sender, EventArgs e) // back
        {
            string position = "3";
            string time = Convert.ToString(2800 * page + 0);
            if(page >0)
            {
                Plot plot = new Plot(position, time, page, 0, -1, pictureBox8, global::LevelEditor.Properties.Resources.empty);
                page--;
                plot.Load(boxList, page);
            }

            //load page precedente
        }
        
        #endregion

         private void exporterToolStripMenuItem1_Click(object sender, EventArgs e) // sauvegarde
         {
             Stream myStream;
             SaveFileDialog saveFileDialog1 = new SaveFileDialog();

             saveFileDialog1.Filter = "xpa files (*.xpa)|*.xpa";
             saveFileDialog1.FilterIndex = 2;
             saveFileDialog1.RestoreDirectory = true;

             StreamReader sr = new StreamReader("Work");
             string save = sr.ReadToEnd();
             if (saveFileDialog1.ShowDialog() == DialogResult.OK)
             {
                 if ((myStream = saveFileDialog1.OpenFile()) != null)
                 {
                     StreamWriter sw = new StreamWriter(myStream);
                     sw.Write(save);
                     sw.Close();
                     myStream.Close();
                 }
             }
             sr.Close();

         }
         private void importerToolStripMenuItem1_Click(object sender, EventArgs e)
         {
             Stream myStream;
             OpenFileDialog openFileDialog1 = new OpenFileDialog();

             openFileDialog1.Filter = "xpa files (*.xpa)|*.xpa";

             if (openFileDialog1.ShowDialog() == DialogResult.OK)
             {
                 if ((myStream = openFileDialog1.OpenFile()) != null)
                 {
                     StreamReader sr = new StreamReader(myStream);
                     string save = sr.ReadToEnd();
                     sr.Close();
                     myStream.Close();

                     StreamWriter sw = new StreamWriter("Work");
                     sw.Write(save);
                     sw.Close();
                 }
             }

             string position = "3";
             string time = Convert.ToString(2800 * page + 0);

                 Plot plot = new Plot(position, time, page, 0, -1, pictureBox8, global::LevelEditor.Properties.Resources.empty);
                 plot.Load(boxList, page);
             
         }

         private void jouerÀMonNiveauToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (MessageBox.Show("Pour jouer sur votre niveau, il suffit de l'enregistrer dans le dossier levels de votre jeu, sous le nom 1, ou 2...99. Par exemple, mon premier niveau se situera par défaut dans le répertoire suivant: C:/Program Files/Space Symphonia/levels/1.xpa (1.xpa étant le niveau que vous avez créé !) Compris ?",
          "Message de confirmation",
              MessageBoxButtons.YesNo) == DialogResult.No)
                 MessageBox.Show("Vous devez enregistrer votre niveau par défaut dans C:/Program Files/Space Symphonia/Levels sous le nom 1 si c'est votre 1er niveau, puis 2 pour votre deuxième, etc..");
         }
    }
}
