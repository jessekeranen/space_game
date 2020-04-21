using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Effects;
using Jypeli.Controls;
using Jypeli.Widgets;


/// @author Jesse Keränen jeratake
/// @version 21.4.2020
/// 
/// <summary>
/// Peli, jossa on tarkoitus tuhota vihollisia raketilla.
/// </summary>
public class rakettipeli3 : PhysicsGame
{
    const int KENTÄN_LEVEYS = 1300;
    const int KENTÄN_KORKEUS = 887;
    int solunLeveys = KENTÄN_LEVEYS / 19;
    Vector nopeusYlos = new Vector(0, 1000);
    Vector nopeusAlas = new Vector(0, -1000);
    Vector nopeusVasen = new Vector(-1000, 0);
    Vector nopeusOikea = new Vector(1000, 0);
    AssaultRifle Ase1;
    AssaultRifle Ase2;
    Raketti raketti;
    PhysicsObject alareuna;
    PhysicsObject[] vihollistaulukko = new PhysicsObject[19];
    List<Label> valikonKohdat;
    IntMeter pisteLaskuri;
    ScoreList topLista = new ScoreList(10, false, 0);
    int vihollissumma = 0;
    Vector valikonSkaalaus1 = new Vector(1.4, 2.9);
    Vector valikonSkaalaus2 = new Vector(1.5, 3.0);
    Vector valikonSkaalaus3 = new Vector(1.9, 2.9);
    Vector valikonSkaalaus4 = new Vector(2.0, 3.0);

    public override void Begin()
    {
        LuoKentta();
        Valikko();
    }


    /// <summary>
    /// Aliohjelma, joka tekee alkuvalikon. 
    /// </summary>
    void Valikko()
    {
        ClearAll();

        Level.Background.Image = LoadImage("avaruus");
        GameObject valikko = new GameObject(320,280);
        valikko.Image  = LoadImage("valikko5.png");
        Add(valikko);
        
        valikonKohdat = new List<Label>();
        Label kohta1 = LuoKohdat("Start a new game");
        kohta1.Position = new Vector(0, 45);
        kohta1.TextScale = new Vector(1.5, 3);

        Label kohta2 = LuoKohdat("High Scores");
        kohta2.Position = new Vector(0, -5);

        Label kohta3 = LuoKohdat("Exit Game");
        kohta3.Position = new Vector(0, -55);
        
        foreach (Label valikonKohta in valikonKohdat)
        {
            Add(valikonKohta);
        }

        AsetaOhjaimet(raketti);
        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, AloitaPeli, null);
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, ParhaatPisteetNaytto, null);
        Mouse.ListenOn(kohta3, MouseButton.Left, ButtonState.Pressed, LopetaPeli, null);
        Mouse.ListenMovement(1.0, ValikossaLiikkuminen, null, valikonKohdat);
    }


    /// <summary>
    /// Aliohjelma, joka luo piste laskurin ja liittää sen näytölle.
    /// </summary>
    void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);
        Label pistenaytto = new Label();
        pistenaytto.Font = LoadFont("STJEDISE.TTF");
        pistenaytto.TextColor = Color.Yellow;
        pistenaytto.Position = new Vector((KENTÄN_LEVEYS / 2) - 100, (KENTÄN_KORKEUS / 2) - 150);
        pistenaytto.BindTo(pisteLaskuri);
        Add(pistenaytto);
    }


    /// <summary>
    /// ALiohjelma, jota kutsutaan kun halutaan vain tarkasteella parhaiden pisteiden listaa.
    /// </summary>
    void ParhaatPisteetNaytto()
    {
        ClearAll();
        Level.Background.Image = LoadImage("avaruus");
        HighScoreWindow topIkkuna = new HighScoreWindow(
                              "Parhaat pisteet",
                              topLista);
        topIkkuna.Closed += TallennaPisteet;
        topIkkuna.Closed += delegate
        {

            Valikko();
        };
        Add(topIkkuna);
    }


    /// <summary>
    /// Aliohjelma, jota kutsutaan, kun raketti tuhoutuu ja tulos lisätään parhaiden pisteiden listalle.
    /// </summary>
    void ParhaatPisteetLisays()
    {
        ClearAll();
        Level.Background.Image = LoadImage("avaruus");

        HighScoreWindow topIkkuna = new HighScoreWindow("High score","Your points %p!", topLista, pisteLaskuri);
        topIkkuna.Closed +=TallennaPisteet;
        topIkkuna.Closed += delegate
         {
             Valikko();
         };
        Add(topIkkuna);
    }


    /// <summary>
    /// Aliohjelma, jolla tallennetaan pisteet parhaiden pisteiden listalle.
    /// </summary>
    /// <param name="sender">Ei käytössä</param>
    void TallennaPisteet(Window sender)
    {
        DataStorage.Save<ScoreList>(topLista, "pisteet.xml");
    }


    /// <summary>
    /// Aliohjelma, joka korostaa kohdan valikossa, jossa hiiri on.
    /// </summary>
    /// <param name="hiirenTila">Hiiren tila</param>
    void ValikossaLiikkuminen(AnalogState hiirenTila, List<Label> kohdat)
    {
        for(int i = 0; i < kohdat.Count; i++)
        {
            if (i != 0)
            {
                if (Mouse.IsCursorOn(kohdat[i]))
                {
                    kohdat[i] = ValikonKohtienMuutokset(Color.Black, valikonSkaalaus4, valikonKohdat, i);
                }
                else
                {
                    kohdat[i] = ValikonKohtienMuutokset(Color.Yellow, valikonSkaalaus3, valikonKohdat, i);
                }
            }
            else
            {
                if (Mouse.IsCursorOn(kohdat[i]))
                {
                    kohdat[i] = ValikonKohtienMuutokset(Color.Black, valikonSkaalaus1, valikonKohdat, i);
                }
                else
                {
                    kohdat[i] = ValikonKohtienMuutokset(Color.Yellow, valikonSkaalaus2, valikonKohdat, i);
                }
            }
        }
    }


    /// <summary>
    /// Aliohjelma, jolla valittua valikonkohtaa muokataan
    /// </summary>
    /// <param name="vari">Valikon kohdan väri</param>
    /// <param name="skaalaus">Valikon kohdan koko</param>
    /// <param name="kohdat">Valikon kohtien lista</param>
    /// <param name="kohta">Käsiteltävä valikon kohta</param>
    /// <returns>Valikon kohdan</returns>
    public Label ValikonKohtienMuutokset(Color vari, Vector skaalaus, List<Label> kohdat, int kohta)
    {
        kohdat[kohta].TextColor = vari;
        kohdat[kohta].TextScale = skaalaus;
        return kohdat[kohta];
    }


    /// <summary>
    /// Aliohjelma, jolla luodaan eri vaihtoehdot valikoihin.
    /// </summary>
    /// <param name="teksti">Teksti, joka kertoo mitä vaihteohdosta tapahtuu</param>
    /// <returns>Vaihtoehto Labelit</returns>
    public Label LuoKohdat(string teksti)
    {
        Font fontti = LoadFont("STJEDISE.TTF");
        Label kohta = new Label(teksti);
        kohta.Font = fontti;
        kohta.TextColor = Color.Yellow;
        kohta.TextScale = new Vector(2, 3);
        valikonKohdat.Add(kohta);
        return kohta;
    }


    /// <summary>
    /// Aliohjelma, jota kutsumalla aloitetaan peli.
    /// </summary>
    void AloitaPeli()
    {
        ClearAll();
        LuoKentta();
        AsetaOhjaimet(raketti);
    }


    /// <summary>
    /// Aliohjelma, jota kutsumalla lopetetaan peli.
    /// </summary>
    void LopetaPeli()
    {
        Exit();
    }


    /// <summary>
    /// ALiohjelma, jota kutsutaan, kun pelaajan raketti tuhoutuu.
    /// </summary>
    void AloitaAlusta()
    {
        ClearAll();
        Level.Background.Image = LoadImage("avaruus");
        Label tekstikentta = new Label();
        Add(tekstikentta);
        tekstikentta.Font = LoadFont("STJEDISE.TTF");
        tekstikentta.TextColor = Color.Yellow;
        tekstikentta.Text = "You died";
        tekstikentta.TextScale = new Vector(3, 3);

        Timer tappio = new Timer();
        tappio.Interval = 2.0;
        tappio.Timeout += delegate
        {
            tekstikentta.Clear();
            ParhaatPisteetLisays();
        };
        tappio.Start();
    }


    /// <summary>
    /// Aliohjelma, jolla luodaan viholliset
    /// </summary>
    /// <param name="leveys">vihollisen leveys</param>
    /// <param name="korkeus">vihollisen korkeus</param>
    /// <param name="tag">vihollisen tagi</param>
    /// <param name="kulma">Kulma, jossa vihollisen kuva liitetään</param>
    /// <param name="arvo">Jättääkö törmäykset huomioimatta</param>
    /// <param name="pyoriiko"><Voiko vihollinen pyöriä/param>
    /// <param name="kuva">Vihollisen kuvan nimi</param>
    /// <returns>Vihollisolio</returns>
    public PhysicsObject LuoViholliset(double leveys, double korkeus, string tag, int kulma, bool arvo, bool pyoriiko, string kuva)
    {
        int i = RandomGen.NextInt(-9, 9);
        int j = i + 9;

        PhysicsObject vihollinen = new PhysicsObject(leveys, korkeus);
        Image vihollisenkuva = LoadImage(kuva);
        vihollinen.Image = vihollisenkuva;
        vihollinen.Y = KENTÄN_KORKEUS / 2;
        vihollinen.X = ((i * solunLeveys) + solunLeveys / 2);
        vihollinen.CanRotate = pyoriiko;
        vihollinen.Angle = Angle.FromDegrees(kulma);
        vihollinen.Tag = tag;
        Vector impulssi = new Vector(0, -100);
        vihollinen.Hit(impulssi);
        vihollinen.IgnoresCollisionResponse = arvo;
        AddCollisionHandler(vihollinen, VihuTormasi);

        Timer poistaOliotaulukosta = new Timer();
        poistaOliotaulukosta.Interval = 1.0;
        poistaOliotaulukosta.Timeout += delegate
        {
            vihollistaulukko[j] = null;
            poistaOliotaulukosta.Stop();
        };
        poistaOliotaulukosta.Start();

        if (vihollistaulukko[j] == null)
        {
            vihollistaulukko[j] = vihollinen;
            return vihollinen;
        }
        else
        {
            vihollinen.Destroy();
            return  null;
        }
    }


    /// <summary>
    /// Aliohjelma, joka luo pelaajalle aseet.
    /// </summary>
    /// <param name="peli">Peli, johon aseet luodaan</param>
    /// <param name="x">Aseen x-koordinaatti</param>
    /// <param name="y">Aseen y-koodinaatti</param>
    /// <returns>Aseen</returns>
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


    /// <summary>
    /// Aliohjelma, joka luo pelikentän ja kutsuu muita aliohjelmia.
    /// </summary>
    public void LuoKentta()
    {
        LuoPistelaskuri();
        topLista = DataStorage.TryLoad<ScoreList>(topLista, "pisteet.xml");
        raketti = LuoRaketti(0, 0, 0);
        Level.Background.Image = LoadImage("avaruus");
        Level.Size = new Vector(KENTÄN_LEVEYS, KENTÄN_KORKEUS);
        SetWindowSize(KENTÄN_LEVEYS, 700);
        Level.CreateHorizontalBorders();
        Level.CreateTopBorder();
        alareuna = Level.CreateBottomBorder();

        Ase1 = LuoAse(this, raketti.X - 30, raketti.Y - 45);
        Ase2 = LuoAse(this, raketti.X + 30, raketti.Y - 45);

        Timer synnytaOlioita = new Timer();
        synnytaOlioita.Interval = 1.0;
        synnytaOlioita.Timeout += delegate
        {
            vihollissumma += 1;
            if (vihollissumma % 3 == 0)
            {
                PhysicsObject kivi = LuoViholliset(60, 60, "kivi", 0, true, true, "kivi3");
                if(kivi != null) Add(kivi);
            }
            else
            {
                PhysicsObject vihu = LuoViholliset(70, 70, "palikka", 180, false, false, "vihu");
                if(vihu != null) Add(vihu);
            }
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
    }
  

    /// <summary>
    /// Aliohjelma, joka luo pelaajan raketin.
    /// </summary>
    /// <param name="x">Raketin leveys</param>
    /// <param name="y">Raketin korkeus</param>
    /// <param name="HP">Raketin elämäpisteet</param>
    /// <returns>Raketin, jolla pelataan</returns>
    Raketti LuoRaketti(double x, double y, int HP)
    {
        Raketti raketti = new Raketti(75, 100, 2);
        Image raketinKuva = LoadImage("suunnitelma_Raketti2");
        raketti.Image = raketinKuva;
        raketti.CanRotate = false;
        raketti.Tag = "raketti";
        raketti.LinearDamping = 0.955;
        raketti.Destroyed += delegate ()
        {
            AloitaAlusta();
        };

        AddCollisionHandler(raketti, "palikka", CollisionHandler.AddMeterValue(raketti.HP, -1));
        AddCollisionHandler(raketti, "palikka", CollisionHandler.DestroyTarget);
        AddCollisionHandler(raketti, "kivi", CollisionHandler.AddMeterValue(raketti.HP, -1));
        AddCollisionHandler(raketti, "kivi", CollisionHandler.DestroyTarget);
        Add(raketti);
        return raketti;
    }


    /// <summary>
    /// Aliohjelma, jolla asetetaan ohjaimet raketille
    /// </summary>
    /// <param name="raketti">Objekti, jota halutaan ohjata</param>
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
        Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", Ase1, raketti, "ammus");
        Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", Ase2, raketti, "ammus");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Aliohjelma, jota kutsutaan, kun ammus osuu kohteeseen.
    /// </summary>
    /// <param name="ammus">Ammus, joka on ammuttu</param>
    /// <param name="kohde">Kohde, jota ammutaan</param>
    void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        if (kohde.Tag == "palikka")
        {
            ammus.Destroy();
            kohde.Destroy();
            pisteLaskuri.Value += 1;
        }   
        if(kohde.Tag != "palikka")
        {
            ammus.Destroy();
        }
    }


    /// <summary>
    /// Aliohjelma, jota kutsutaan, kun aseella ammutaan.
    /// </summary>
    /// <param name="ase">Ase, jolla ammutaan</param>
    /// <param name="raketti">Raketti, jolla pelaaja pelaa</param>
    void AmmuAseella(AssaultRifle ase, Raketti raketti, string tag)
    {
        PhysicsObject ammus = ase.Shoot();

        if (ammus != null)
        {
            ammus.Color = Color.DarkBlue;
            ammus.Size *= 0.5;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);  
        }
        if(raketti.HP == 0)
        {
            Ase1.Destroy();
            Ase2.Destroy();
        }
    }


    /// <summary>
    /// Aliohjelma, jota kutsutaan kun vihollinen saavuttaa alareunan
    /// </summary>
    /// <param name="vihu">Vihollinen</param>
    /// <param name="kohde">Alareuna</param>
    void VihuTormasi(PhysicsObject vihu, PhysicsObject kohde)
    {
        if(kohde == alareuna)
        {
            vihu.Destroy();
        }
    }


    /// <summary>
    /// Aliohjelma, jolla asetetaan nopeudet raketin liikkeille.
    /// </summary>
    /// <param name="raketti">Pelaajan raketti</param>
    /// <param name="nopeus">Nopeus, jolla raketti kiihtyy</param>
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

