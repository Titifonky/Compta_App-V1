using System;

namespace Compta
{

    public class LigneBanque : LigneCompta
    {
        public LigneBanque() { }

        public LigneBanque(EcritureBanque ecritureBanque)
        {
            EcritureBanque = ecritureBanque;

            Bdd.Ajouter(this);

            No = EcritureBanque.ListeLigneBanque.Count + 1;
        }

        private EcritureBanque _EcritureBanque = null;
        [CleEtrangere]
        public EcritureBanque EcritureBanque
        {
            get
            {
                if (_EcritureBanque == null)
                    _EcritureBanque = Bdd.Parent<EcritureBanque, LigneBanque>(this);

                return _EcritureBanque;
            }
            set
            {
                Set(ref _EcritureBanque, value, this);
                if (_EcritureBanque.ListeLigneBanque != null)
                    _EcritureBanque.ListeLigneBanque.Add(this);
            }
        }
    }
}
