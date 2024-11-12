namespace Bevolkerung;
internal class Allampolgar
{
    public int Id { get; set; }
    public string Nem { get; set; }
    public int SzultetesiEv { get; set; }
    public int Kor => DateTime.Now.Year - SzultetesiEv;
    public int Suly { get; set; }
    public int Magassag { get; set; }
    public bool Dohanyzik { get; set; }
    public string DohanyzikSzoveges => Dohanyzik ? "igen" : "nem";
    public string Nemzetiseg { get; set; }
    public string Nepcsoport { get; set; }
    public string Tartomany { get; set; }
    public int NettoJovedelem { get; set; }
    public int HaviNettoJovedelem => NettoJovedelem / 12;
    public string IskolaiVegzetseg { get; set; }
    public string PolitikaiNezet { get; set; }
    public bool AktivSzavazo { get; set; }
    public string AktivSzavazoSzoveges => AktivSzavazo ? "igen" : "nem";
    public int SorFogyasztasEvente { get; set; }
    public int KrumpliFogyasztasEvente { get; set; }

    public Allampolgar(string sor)
    {
        string[] adatok = sor.Split(';');

        Id = int.Parse(adatok[0]);
        Nem = adatok[1];
        SzultetesiEv = int.Parse(adatok[2]);
        Suly = int.Parse(adatok[3]);
        Magassag = int.Parse(adatok[4]);
        Dohanyzik = adatok[5].ToLower() == "igen";
        Nemzetiseg = adatok[6];

        if (Nemzetiseg == "német")
            Nepcsoport = adatok[7];
        else Nepcsoport = null;

        Tartomany = adatok[8];
        NettoJovedelem = int.Parse(adatok[9]);
        IskolaiVegzetseg = adatok[10];
        PolitikaiNezet = adatok[11];
        AktivSzavazo = adatok[12].ToLower() == "igen";
        
        if (adatok[13] != "NA") SorFogyasztasEvente = int.Parse(adatok[13]);
        if (adatok[14] != "NA") KrumpliFogyasztasEvente = int.Parse(adatok[14]);
    }

    public override string ToString()
    {
        return $"{Id} {Nem} {SzultetesiEv} {Suly}kg {Magassag}cm {(Dohanyzik ? "dohányzik" : "nem dohányzik")} {Nemzetiseg} {Nepcsoport} {Tartomany} {NettoJovedelem}EUR {IskolaiVegzetseg} {PolitikaiNezet} {(AktivSzavazo ? "aktív szavazó" : "nem aktív szavazó")} {SorFogyasztasEvente}l sör {KrumpliFogyasztasEvente}kg krumpli";
    }

    public string ToString(bool firstFive)
    {
        if (firstFive)
            return $"{Id}\t{Nem}\t{SzultetesiEv}\t{Suly}kg\t{Magassag}cm";
        else return $"{Id}\t{Nemzetiseg}\t{Nepcsoport}\t{Tartomany}\t{NettoJovedelem}€";
    }
}
