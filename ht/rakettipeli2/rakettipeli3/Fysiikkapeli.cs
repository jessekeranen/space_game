using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class rakettipeli3 : PhysicsGame
{ 
Vector nopeusYlos = new Vector(0, 500);
Vector nopeusAlas = new Vector(0, -500);
Vector nopeusVasen = new Vector(-500, 0);
Vector nopeusOikea = new Vector(500, 0);
AssaultRifle Ase1;
AssaultRifle Ase2;
PhysicsObject raketti;

public override void Begin()
{

    LuoKentta();
    AsetaOhjaimet();
    Ase1 = LuoAse(this, raketti.X - 63, raketti.Y - 135);
    Ase2 = LuoAse(this, raketti.X + 23, raketti.Y - 135);

    PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

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
    peli.Add(pelaajanAse);
    return pelaajanAse;

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
    Add(raketti);
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
    Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", Ase1);
    Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", Ase2);
}
void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
{
    ammus.Destroy();
}
void AmmuAseella(AssaultRifle ase)
{
    PhysicsObject ammus = ase.Shoot();

    if (ammus != null)
    {
        ammus.Size *= 0.5;
        ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
        ammus.Color = Color.DarkBlue;
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