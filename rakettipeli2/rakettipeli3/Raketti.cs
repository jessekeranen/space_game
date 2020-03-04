using System;
using Jypeli;


    public class Raketti : PhysicsObject
    {
        public IntMeter HP;

        public Raketti(double leveys, double korkeus, int elamat)
            :base(leveys, korkeus)
        {
            HP = new IntMeter(elamat, 0, elamat);
            HP.LowerLimit += delegate () { this.Destroy(); };
        }
    }
