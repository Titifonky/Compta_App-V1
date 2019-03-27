using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Compta
{
    [ForcerAjout]
    public class Version : ObjetGestion
    {
        public Version() { }

        public Version(int No)
        {
            _No = No;
        }

        [Propriete, Tri]
        public override int No
        {
            get { return _No; }
            set { _No = value; }
        }
    }
}
