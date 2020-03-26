using System;
using System.Collections.Generic;
using System.Text;
using Jypeli;


public class Raketti : PhysicsObject
{
    public IntMeter HP;

    public Raketti(double leveys, double korkeus, int elamat)
        : base(leveys, korkeus)
    {
        HP = new IntMeter(elamat, 0, elamat);
        HP.LowerLimit += delegate () 
        {
            this.Hit(new Vector(0, -500)); 
            Timer.CreateAndStart(1.0, Destroy);
        };
        
    }
}

