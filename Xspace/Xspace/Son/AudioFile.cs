using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FMOD;


namespace Xspace.Son
{
    class AudioFile
    {
        private void ErrCheck(RESULT result)
        {
            if (result != RESULT.OK)
            {
                throw new ApplicationException("FMOD : " + result + " - " + Error.String(result));
            }
        }
        
        private void FileLoad(string path)
        {
            //TODO
        }
    }
}
