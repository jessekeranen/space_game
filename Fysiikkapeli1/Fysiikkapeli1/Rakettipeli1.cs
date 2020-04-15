using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Effects;
using Jypeli.Controls;
using Jypeli.Widgets;


/// <summary>
/// Peli, jossa on tarkoitus tuhota vihollisia raketilla.
/// </summary>
public class rakettipeli3 : PhysicsGame
{
    const int kentanLeveys = 1300;
    const int kentanKorkeus = 887;
    int solunLeveys = kentanLeveys / 19;
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
    int vihollisSumma = 0;
    PhysicsObject kivi;
    PhysicsObject palikka;
    

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
        pistenaytto.Position = new Vector((kentanLeveys / 2) - 100, (kentanKorkeus / 2) - 150);
        pistenaytto.BindTo(pisteLaskuri);
        Add(pistenaytto);
    }


    /// <summary>
    /// ALiohjelma, jota kutsutaan kun halutaan vain tarkasteella parhaiden pisteiden listaa.
    /// </summary>
    void ParhaatPisteetNaytto()
    {
        HighScoreWindow topIkkuna = new HighScoreWindow(
                              "Parhaat pisteet",
                              topLista);
        topIkkuna.Closed += TallennaPisteet;
        Add(topIkkuna);
    }


    /// <summary>
    /// Aliohjelma, jota kutsutaan, kun raketti tuhoutuu ja tulos lisätään parhaiden pisteiden listalle.
    /// </summary>
    void ParhaatPisteetLisays()
    {
        HighScoreWindow topIkkuna = new HighScoreWindow("High score","Your points %p!", topLista, pisteLaskuri);
        topIkkuna.Closed +=TallennaPisteet;
        
        Valikko();
        Add(topIkkuna);
    }


    /// <summary>
    /// Aliohjelma, jolla tallennetaan pisteet parhaiden pisteiden listalle.
    /// </summary>
    /// <param name="sender"></param>
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
                    kohdat[i].TextColor = Color.Black;
                    kohdat[i].TextScale = new Vector(1.9, 2.9);
                }
                else
                {
                    kohdat[i].TextColor = Color.Yellow;
                    kohdat[i].TextScale = new Vector(2.0, 3.0);
                }
            }
            else
            {
                if (Mouse.IsCursorOn(kohdat[i]))
                {
                    kohdat[i].TextColor = Color.Black;
                    kohdat[i].TextScale = new Vector(1.4, 2.9);
                }
                else
                {
                    kohdat[i].TextColor = Color.Yellow;
                    kohdat[i].TextScale = new Vector(1.5, 3.0);
                }
            }
        }
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
    /// ALiohjelma, joka luo vihollisia.
    /// </summary>
    void LuoSatunnainenVihollinen()
    {
        int i = RandomGen.NextInt(-9, 9);
        int j = i + 9;
            
        palikka = Viholliset(70, 70, "palikka", 180, false);
        Image palikanKuva = LoadImage("vihu");
        palikka.Image = palikanKuva;
        palikka.X = ((i * solunLeveys) + solunLeveys / 2);

        Timer poistaOliotaulukosta = new Timer();
        poistaOliotaulukosta.Interval = 5.0;
        poistaOliotaulukosta.Timeout += delegate
        {
            vihollistaulukko[j] = null;
        };
        poistaOliotaulukosta.Start();

        if (vihollistaulukko[j] == null)
        {
            vihollistaulukko[j] = palikka;
            Add(palikka);
        }
        else if (vihollistaulukko[j] != null)
        {
            palikka.Destroy();
        }
    }


    /// <summary>
    /// Aliohjelma, joka luo tuhoutumattomia kiviä
    /// </summary>
    void LuoSatunnainenKivi()
    {
        int i = RandomGen.NextInt(-9, 9);
        int j = i + 9;

        kivi = Viholliset(70, 70, "kivi", 0, true);
        Image kivenKuva = LoadImage("kivi2");
        kivi.Image = kivenKuva;
        kivi.X = ((i * solunLeveys) + solunLeveys / 2);

        Timer poistaOliotaulukosta = new Timer();
        poistaOliotaulukosta.Interval = 5.0;
        poistaOliotaulukosta.Timeout += delegate
        {
            vihollistaulukko[j] = null;
        };
        poistaOliotaulukosta.Start();

        if (vihollistaulukko[j] == null)
        {
            vihollistaulukko[j] = kivi;
            Add(kivi);
        }
        else if (vihollistaulukko[j] != null)
        {
            kivi.Destroy();
        }
    }
        
  
    public PhysicsObject Viholliset(double leveys, double korkeus, string tag, int kulma, bool arvo)
    {
        PhysicsObject vihollinen = new PhysicsObject(leveys, korkeus);
        vihollinen.Y = kentanKorkeus / 2;
        vihollinen.CanRotate = false;
        vihollinen.Angle = Angle.FromDegrees(kulma);
        vihollinen.Tag = tag;
        Vector impulssi = new Vector(0, -100);
        vihollinen.Hit(impulssi);
        vihollinen.IgnoresCollisionResponse = arvo;
        AddCollisionHandler(vihollinen, VihuTormasi);
        return vihollinen;
    }


    /// <summary>
    /// Aliohjelma, joka luo pelaajalle aseet.
    /// </summary>
    /// <param name="peli">Peli, johon aseet luodaan</param>
    /// <param name="x">Aseen x-koordinaatti</param>
    /// <param name="y">Aseen y-koodinaatti</param>
    /// <returns></returns>
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
        Level.Size = new Vector(kentanLeveys, kentanKorkeus);
        SetWindowSize(kentanLeveys, 700);
        Level.CreateHorizontalBorders();
        Level.CreateTopBorder();
        alareuna = Level.CreateBottomBorder();

        Ase1 = LuoAse(this, raketti.X - 30, raketti.Y - 45);
        Ase2 = LuoAse(this, raketti.X + 30, raketti.Y - 45);

        Timer synnytaOlioita = new Timer();
        synnytaOlioita.Interval = 1.0;
        synnytaOlioita.Timeout += delegate
        {
            if (vihollisSumma % 3 == 0)
            {
                LuoSatunnainenKivi();
                vihollisSumma += 1;
            }
            else
            {
                LuoSatunnainenVihollinen();
                vihollisSumma += 1;
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
    /// Aliohjelm, jolla asetetaan nopeudet raketin liikkeille.
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

