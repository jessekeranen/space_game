using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class rakettipeli2 : PhysicsGame
{
    Vector nopeusYlos = new Vector(0, 500);
    Vector nopeusAlas = new Vector(0, -500);
    Vector nopeusVasen = new Vector(-500, 0);
    Vector nopeusOikea = new Vector(500, 0);
    AssaultRifle pelaajan1Ase1;
    AssaultRifle pelaajan1Ase2;
    PhysicsObject raketti;



    public override void Begin()
    {
        LuoKentta();
        AsetaOhjaimet();

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        for (int i = 0; i < 20; i++)
        {
            PhysicsObject palikka = new PhysicsObject(50, 50);
            palikka.X = RandomGen.NextDouble(-750, 750);
            palikka.Y = 500;
            Vector impulssi = new Vector(0, -100);
            palikka.Hit(impulssi);
            palikka.MakeOneWay();
            Add(palikka);
        }
    

    }

    public void LuoKentta()
    {
        raketti = LuoRaketti(0, 0);
        Level.Background.Image = LoadImage("avaruus");
        Level.Size = new Vector(1500, 1000);
        SetWindowSize(1500, 1000);
        Level.CreateBorders();

    }
    PhysicsObject LuoRaketti(double x, double y)
        {
        PhysicsObject raketti = new PhysicsObject(250, 200);
        Image raketinKuva = LoadImage("suunnitelma_Raketti2");
        raketti.Image = raketinKuva;
        raketti.CanRotate = false;
        pelaajan1Ase1 = new AssaultRifle(0, 0);
        pelaajan1Ase1.Ammo.Value = 1000;
        pelaajan1Ase1.ProjectileCollision = AmmusOsui;
        pelaajan1Ase1.FireRate = 50.0;
        pelaajan1Ase1.X = 23;
        pelaajan1Ase1.Y = -130;
        pelaajan1Ase1.Angle = Angle.FromDegrees(90);
        pelaajan1Ase2 = new AssaultRifle(0, 0);
        pelaajan1Ase2.Ammo.Value = 1000;
        pelaajan1Ase2.ProjectileCollision = AmmusOsui;
        pelaajan1Ase2.FireRate = 55.0;
        pelaajan1Ase2.X = -63;
        pelaajan1Ase2.Y = -130;
        pelaajan1Ase2.Angle = Angle.FromDegrees(90);
        Add(raketti);
        raketti.Add(pelaajan1Ase1);
        raketti.Add(pelaajan1Ase2);
        return raketti;
        }
    public void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Up, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia ylös", raketti, nopeusYlos);
        Keyboard.Listen(Key.Up, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
        Keyboard.Listen(Key.Down, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia alas", raketti, nopeusAlas);
        Keyboard.Listen(Key.Down, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
        Keyboard.Listen(Key.Left, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia vasemmalle", raketti, nopeusVasen);
        Keyboard.Listen(Key.Left, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
        Keyboard.Listen(Key.Right, ButtonState.Down, AsetaNopeus, "Pelaaja: Liikuta rakettia oikalle", raketti, nopeusOikea);
        Keyboard.Listen(Key.Right, ButtonState.Released, AsetaNopeus, null, raketti, Vector.Zero);
        Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", pelaajan1Ase1);
        Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", pelaajan1Ase2);
    }
    void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();
        kohde.Destroy();
    }
    void AmmuAseella(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot();

        if (ammus != null)
        {
            ammus.Size *= 0.5;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
            ammus.Color = Color.Red;
        }
    }
    void AsetaNopeus(PhysicsObject raketti, Vector nopeus)
    {
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

        raketti.Velocity = nopeus;
    }

}
