using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Effects;
using Jypeli.Controls;
using Jypeli.Widgets;


public class rakettipeli3 : PhysicsGame
{
    const int kentanLeveys = 1200;
    const int kentanKorkeus = 700;
    int solunLeveys = kentanLeveys / 13;
    Vector nopeusYlos = new Vector(0, 1000);
    Vector nopeusAlas = new Vector(0, -1000);
    Vector nopeusVasen = new Vector(-1000, 0);
    Vector nopeusOikea = new Vector(1000, 0);
    AssaultRifle Ase1;
    AssaultRifle Ase2;
    Raketti raketti;
    PhysicsObject[] vihollistaulukko = new PhysicsObject[13];

    public override void Begin()
    {

        LuoKentta();
        Ase1 = LuoAse(this, raketti.X - 40, raketti.Y - 55);
        Ase2 = LuoAse(this, raketti.X + 40, raketti.Y - 55);
        AsetaOhjaimet(raketti);
        

        Timer synnytaOlioita = new Timer();
        synnytaOlioita.Interval = 1.0;
        synnytaOlioita.Timeout += delegate
        {
            LuoSatunnainenVihollinen();
        };
        synnytaOlioita.Start();

        Timer olioidenSynnyttamisenNopeutin = new Timer();
        olioidenSynnyttamisenNopeutin.Interval = 1.0;
        olioidenSynnyttamisenNopeutin.Timeout += delegate
        {
            if (synnytaOlioita.Interval - 0.1 <= 0) return;
            synnytaOlioita.Interval -= 0.01;
        };
        olioidenSynnyttamisenNopeutin.Start();

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    void LuoSatunnainenVihollinen()
    {
        int i = RandomGen.NextInt(-6, 5);
        int j = i + 6;
        PhysicsObject palikka = new PhysicsObject(90, 90);
        Image palikanKuva = LoadImage("vihu");
        palikka.Image = palikanKuva;
        palikka.X = ((i*solunLeveys)+solunLeveys/2);
        palikka.Y = kentanKorkeus/2;
        palikka.CanRotate = false;
        palikka.Angle = Angle.FromDegrees(180);
        Vector impulssi = new Vector(0, -100);
        palikka.Tag = "palikka";
        palikka.Hit(impulssi);

        if (vihollistaulukko[j] == null)
        {
            vihollistaulukko[j] = palikka;
            Add(palikka);
            
        }
        else
        {
            Timer poistaOliotaulukosta = new Timer();
            poistaOliotaulukosta.Interval = 1.0;
            poistaOliotaulukosta.Timeout -= delegate
            {
                    vihollistaulukko[j] = null;
            };
            poistaOliotaulukosta.Start();
            vihollistaulukko[j] = palikka;
            Add(palikka);
        }
         
    }
    

    AssaultRifle LuoAse(Game peli, double x, double y)
    {
        AssaultRifle pelaajanAse;
        pelaajanAse = new AssaultRifle(0, 0);
        pelaajanAse.X = x;
        pelaajanAse.Y = y;
        pelaajanAse.Ammo.Value = 10000;
        pelaajanAse.ProjectileCollision = AmmusOsui;
        pelaajanAse.FireRate = 50.0;
        pelaajanAse.Angle = Angle.FromDegrees(90);
        raketti.Add(pelaajanAse);
        return pelaajanAse;
    }


    public void LuoKentta()
    {
        raketti = LuoRaketti(0, 0, 0);
        Level.Background.Image = LoadImage("avaruus");
        Level.Size = new Vector(kentanLeveys, kentanKorkeus);
        SetWindowSize(kentanLeveys, kentanKorkeus);
        Level.CreateBorders();
    }

 
    Raketti LuoRaketti(double x, double y, int HP)
    {
        Raketti raketti = new Raketti(100, 130, 2);
        Image raketinKuva = LoadImage("suunnitelma_Raketti2");
        raketti.Image = raketinKuva;
        raketti.CanRotate = false;
        raketti.Tag = "raketti";
        raketti.LinearDamping = 0.955;
        AddCollisionHandler(raketti, "palikka", CollisionHandler.AddMeterValue(raketti.HP, -1));
        AddCollisionHandler(raketti, "palikka", CollisionHandler.DestroyTarget);
        Add(raketti);
        return raketti;
    }


    public void AsetaOhjaimet(Raketti raketti)
    {
            Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia ylös", raketti, nopeusYlos);
            Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
            Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia alas", raketti, nopeusAlas);
            Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
            Keyboard.Listen(Key.Left, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia vasemmalle", raketti, nopeusVasen);
            Keyboard.Listen(Key.Left, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
            Keyboard.Listen(Key.Right, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia oikalle", raketti, nopeusOikea);
            Keyboard.Listen(Key.Right, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
            Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella , "Ammu", Ase1, raketti);
            Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella , "Ammu", Ase2, raketti);
    }


    void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();
        kohde.Destroy();
    }

    void AmmuAseella(AssaultRifle ase, Raketti raketti)
    {
        PhysicsObject ammus = ase.Shoot();
        if (ammus != null)
        {
            ammus.Size *= 0.5;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
            ammus.Color = Color.DarkBlue;
        }
        else if(raketti.HP == 0)
        {
            Ase1.Destroy();
            Ase2.Destroy();
        }
    }


    void AsetaNopeus(Raketti raketti, Vector nopeus)
    {
        if(raketti.HP == 0)
        {
            Keyboard.DisableAll();
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        }
        if ((nopeus.Y < 0) && (raketti.Bottom < Level.Bottom))
        {
            raketti.Velocity = Vector.Zero;
            return;
        }
        if ((nopeus.Y > 0) && (raketti.Top > Level.Top))
        {
            raketti.Velocity = Vector.Zero;
            return;
        }
        if ((nopeus.X < 0) && (raketti.Bottom < Level.Left))
        {
            raketti.Velocity = Vector.Zero;
            return;
        }
        if ((nopeus.X > 0) && (raketti.Top > Level.Right))
        {
            raketti.Velocity = Vector.Zero;
            return;
        }
        raketti.Push(nopeus);
    }


}
