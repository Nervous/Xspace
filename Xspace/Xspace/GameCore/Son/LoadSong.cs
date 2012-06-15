using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Xspace
{
    class LoadSong
    {
        public string title;
        public string singer;
        public string album;
        public string year;
        public string path;
        public string md5;
        public int md5_seed;

        public LoadSong(string filename)
        {
            path = "Musiques\\Jeu\\" + filename;
            title = filename;
            singer = "";
            album = "";
            year = "";
            LoadInfos();
            LoadMD5();
        }

        public void LoadInfos()
        {
            byte[] b = new byte[128];
            FileStream fs = new FileStream(path, FileMode.Open);
            fs.Seek(-128, SeekOrigin.End);
            fs.Read(b, 0, 128);
            String sFlag = System.Text.Encoding.Default.GetString(b, 0, 3);
            if (sFlag.CompareTo("TAG") == 0)
            {
                title = System.Text.Encoding.Default.GetString(b, 3, 30).Trim('\0');
                singer = System.Text.Encoding.Default.GetString(b, 33, 30).Trim('\0');
                album = System.Text.Encoding.Default.GetString(b, 63, 30).Trim('\0');
                year = System.Text.Encoding.Default.GetString(b, 93, 4).Trim('\0');
            }
            fs.Close();
        }

        public void LoadMD5()
        {
            MD5CryptoServiceProvider md5crypto = new MD5CryptoServiceProvider();
            Stream s = (Stream)new FileStream(path, FileMode.Open);
            byte[] music_md5_bytes = md5crypto.ComputeHash(s);
            md5 = Encoding.ASCII.GetString(music_md5_bytes);
            md5_seed = BitConverter.ToInt32(music_md5_bytes, 0);
            s.Close();
        }
    }
}
