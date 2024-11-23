using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Bevolkerung;
public partial class MainWindow : Window
{
    List<Allampolgar> Lakossag = [];
    List<Action> feladatok = [];
    System.Globalization.CultureInfo german = System.Globalization.CultureInfo.GetCultureInfo("de-DE");
    Random rnd = new Random();

    #region Logika
    public MainWindow()
    {
        InitializeComponent();
        AdatokFeltoltese();
        OsszesFeladatListabaToltese();
        ComboFeltoltese();
        DataContext = Lakossag;
    }

    private void AdatokFeltoltese()
    {
        using (StreamReader sr = new StreamReader(path: @"..\..\..\src\bevölkerung.txt", encoding: Encoding.UTF8))
        {
            _ = sr.ReadLine();
            while (!sr.EndOfStream)
                Lakossag.Add(new Allampolgar(sr.ReadLine()));
        }
    }
    private void OsszesFeladatListabaToltese()
    {
        MethodInfo[] fugvenyek = typeof(MainWindow).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (MethodInfo fugveny in fugvenyek.Where(m => m.Name.StartsWith("Feladat")))
        {
            Action action = (Action)Delegate.CreateDelegate(typeof(Action), this, fugveny);
            feladatok.Add(action);
        }
    }
    private void ComboFeltoltese()
    {
        for (int i = 0; i < feladatok.Count; i++)
            FeladatokCombo.Items.Add($"{i + 1}.");
    }
    private void Combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        MegoldasMondatos.Content = string.Empty;
        MegoldasTeljes.ItemsSource = null;
        MegoldasLista.Items.Clear();

        feladatok[FeladatokCombo.SelectedIndex].Invoke();
    }
    #endregion

    #region Feladatok
    private void Feladat1()
    {
        MegoldasMondatos.Content = $"Legmagasabb nettó éves jövedelem: {Lakossag.OrderByDescending(l => l.NettoJovedelem).First().NettoJovedelem.ToString("C", german)}";
    }
    private void Feladat2()
    {
        MegoldasMondatos.Content = $"Állampolgárok átlagos nettó éves jövedelme: {Lakossag.Average(a => a.NettoJovedelem).ToString("C", german)}";
    }
    private void Feladat3()
    {
        foreach (IGrouping<string, Allampolgar> tartomany in Lakossag.GroupBy(l => l.Tartomany))
            MegoldasLista.Items.Add($"{tartomany.Key} - {tartomany.Count()} fő");
    }
    private void Feladat4()
    {
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => x.Nemzetiseg == "angolai");
    }
    private void Feladat5()
    {
        int legfiatalabbKor = Lakossag.OrderBy(x => x.Kor).First().Kor;
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => x.Kor == legfiatalabbKor);
    }
    private void Feladat6()
    {
        foreach (Allampolgar dohanyzo in Lakossag.Where(a => !a.Dohanyzik))
            MegoldasLista.Items.Add($"{dohanyzo.Id} - {dohanyzo.HaviNettoJovedelem.ToString("C", german)}");
    }
    private void Feladat7()
    {
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => x.Tartomany == "Bajorország" && x.NettoJovedelem > 30000)
                                             .OrderBy(o => o.IskolaiVegzetseg);
    }
    private void Feladat8()
    {
        foreach (Allampolgar ferfi in Lakossag.Where(a => a.Nem == "férfi"))
            MegoldasLista.Items.Add(ferfi.ToString(true));
    }
    private void Feladat9()
    {
        foreach (Allampolgar bajorNo in Lakossag.Where(a => a.Tartomany == "Bajorország" && a.Nem == "nő"))
            MegoldasLista.Items.Add(bajorNo.ToString(false));
    }
    private void Feladat10()
    {
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => !x.Dohanyzik).OrderByDescending(j => j.NettoJovedelem).Take(10);
    }
    private void Feladat11()
    {
        MegoldasTeljes.ItemsSource = Lakossag.OrderByDescending(x => x.Kor).Take(5);
    }
    private void Feladat12()
    {
        foreach (IGrouping<string, Allampolgar> nemetNepcsoport in Lakossag.Where(x => x.Nemzetiseg == "német").GroupBy(n => n.Nepcsoport))
        {
            MegoldasLista.Items.Add(nemetNepcsoport.Key);
            foreach (Allampolgar polgar in nemetNepcsoport)
                MegoldasLista.Items.Add($"\t{(polgar.AktivSzavazo ? "aktív szavazó" : "nem aktív szavazó")}\t {polgar.PolitikaiNezet}");
        }
    }
    private void Feladat13()
    {
        MegoldasMondatos.Content = $"Férfi évi átlagos sörfogyasztás: {Lakossag.Where(x => x.Nem == "férfi" && x.SorFogyasztasEventeSzoveges != "NA").Average(a => a.SorFogyasztasEvente):.00} l";
    }
    private void Feladat14()
    {
        MegoldasTeljes.ItemsSource = Lakossag.OrderBy(x => x.IskolaiVegzetseg);
    }
    private void Feladat15()
    {
        foreach (Allampolgar gazdag in Lakossag.OrderByDescending(x => x.NettoJovedelem).Take(3))
            MegoldasLista.Items.Add($"{gazdag.ToString(false)}");

        MegoldasLista.Items.Add("");

        foreach (Allampolgar szegeny in Lakossag.OrderBy(x => x.NettoJovedelem).Take(3))
            MegoldasLista.Items.Add($"{szegeny.ToString(false)}");
    }
    private void Feladat16()
    {
        MegoldasMondatos.Content = $"Az állampogárok {Math.Round((decimal)Lakossag.Count(x => x.AktivSzavazo) / Lakossag.Count() * 100, 2)}%-a aktív szavazó";
    }
    private void Feladat17()
    {
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => x.AktivSzavazo).OrderBy(t => t.Tartomany);
    }
    private void Feladat18()
    {
        MegoldasMondatos.Content = $"Az állampolgárok átlagos életkora {Lakossag.Average(x => x.Kor):.00} év";
    }
    private void Feladat19()
    {
        var legmagasabb = Lakossag
            .GroupBy(t => t.Tartomany)
            .OrderByDescending(o => o.Average(a => a.NettoJovedelem))
            .ThenByDescending(o => o.Count())
            .First();

        MegoldasMondatos.Content = $"Legmagasabb átlagos éves nettó jövedelem: {legmagasabb.Key} {legmagasabb.ToList().Average(x => x.NettoJovedelem).ToString("C", german)} ({legmagasabb.Count()} fő)";
    }
    private void Feladat20()
    {
        List<Allampolgar> sorban = Lakossag.OrderBy(x => x.Suly).ToList();
        double median;
        if (Lakossag.Count % 2 == 0)
            median = (double)(sorban[Lakossag.Count / 2].Suly + sorban[Lakossag.Count / 2 + 1].Suly) / 2;
        else median = sorban[Lakossag.Count / 2 + 1].Suly;

        MegoldasLista.Items.Add($"Az állampolgárok testsúlyának átlaga {Lakossag.Average(x => x.Suly):.00} kg");
        MegoldasLista.Items.Add($"Az állampolgárok testsúlyának mediánja {median:.00} kg");
    }
    private void Feladat21()
    {
        double aktivSor = Lakossag.Where(x => x.AktivSzavazo && x.SorFogyasztasEventeSzoveges != "NA").Average(a => a.SorFogyasztasEvente);
        double inaktivSor = Lakossag.Where(x => !x.AktivSzavazo && x.SorFogyasztasEventeSzoveges != "NA").Average(a => a.SorFogyasztasEvente);

        MegoldasLista.Items.Add($"Aktív szavazók átlagos sörfogyasztása: {aktivSor:.00} l");
        MegoldasLista.Items.Add($"Nem szavazók átlagos sörfogyasztása: {inaktivSor:.00} l");

        if (aktivSor > inaktivSor)
            MegoldasLista.Items.Add("Az aktív szavazók fogyasztanak több sört");
        else if (aktivSor < inaktivSor)
            MegoldasLista.Items.Add("Az inaktív szavazók fogyasztanak több sört");
        else MegoldasLista.Items.Add("Az aktív és inaktív szavazók ugyanannyi sört fogyasztanak");
    }
    private void Feladat22()
    {
        MegoldasLista.Items.Add($"Férfiak átlagmagassága: {Lakossag.Where(x => x.Nem == "férfi").Average(m => m.Magassag):.00} cm");
        MegoldasLista.Items.Add($"Nők átlagmagassága: {Lakossag.Where(x => x.Nem == "nő").Average(m => m.Magassag):.00} cm");
    }
    private void Feladat23()
    {
        IGrouping<string, Allampolgar> legtobbNepcsoport = Lakossag
            .Where(x => x.Nemzetiseg == "német")
            .GroupBy(n => n.Nepcsoport)
            .OrderByDescending(o => o.Count())
            .ThenByDescending(o => o.Average(k => k.Kor))
            .First();

        MegoldasMondatos.Content = $"A legtöbb német állampolgár a(z) {legtobbNepcsoport.Key} népcsoportba tartozik ({legtobbNepcsoport.Count()} fő)";
    }
    private void Feladat24()
    {
        double dohanyzo = Lakossag.Where(x => x.Dohanyzik).Average(a => a.NettoJovedelem);
        double nemDohanyzo = Lakossag.Where(x => !x.Dohanyzik).Average(a => a.NettoJovedelem);

        MegoldasLista.Items.Add($"Dohányzók átlagos nettó éves jövedelme: {dohanyzo.ToString("C", german)}");
        MegoldasLista.Items.Add($"Nem dohányzók átlagos nettó éves jövedelme: {nemDohanyzo.ToString("C", german)}");

        if (dohanyzo > nemDohanyzo)
            MegoldasLista.Items.Add("A dohányzóknak több az átlagos nettó éves jövedelme");
        else if (dohanyzo < nemDohanyzo)
            MegoldasLista.Items.Add("A nem dohányzóknak több az átlagos nettó éves jövedelme");
        else MegoldasLista.Items.Add("A dohányzó és nem dohányzók átlagos nettó éves jövedelme azonos.");
    }
    private void Feladat25()
    {
        double atlagKrumpli = Lakossag.Where(x => x.KrumpliFogyasztasEventeSzoveges != "NA").Average(a => a.KrumpliFogyasztasEvente);

        MegoldasMondatos.Content = $"Az állampolgárok átlagos krumplifogyasztása {atlagKrumpli:.00} kg";
        MegoldasTeljes.ItemsSource = Lakossag
            .Where(x => x.KrumpliFogyasztasEvente > atlagKrumpli)
            .OrderByDescending(k => k.KrumpliFogyasztasEvente)
            .Take(15);
    }
    private void Feladat26()
    {
        foreach (IGrouping<string, Allampolgar> tartomany in Lakossag.GroupBy(t => t.Tartomany))
            MegoldasLista.Items.Add($"{tartomany.Key} - {tartomany.Average(k => k.Kor):.00} átlagév");
    }
    private void Feladat27()
    {
        MegoldasLista.Items.Add($"50 évnél idősebbek száma: {Lakossag.Count(k => k.Kor > 50)} fő\n");
        foreach (Allampolgar otvenes in Lakossag.Where(k => k.Kor > 50))
            MegoldasLista.Items.Add($"{otvenes.ToString(true)}");
    }
    private void Feladat28()
    {
        List<Allampolgar> dohanyzoNok = Lakossag.Where(x => x.Nem == "nő" && x.Dohanyzik).ToList();

        if (dohanyzoNok.Count > 0)
        {
            MegoldasLista.Items.Add($"Maximális jövedelem: {dohanyzoNok.OrderByDescending(n => n.NettoJovedelem).First().NettoJovedelem.ToString("C", german)}\n");

            foreach (Allampolgar dohanyzoNo in Lakossag.Where(x => x.Nem == "nő" && x.Dohanyzik))
                MegoldasLista.Items.Add(dohanyzoNo.ToString(false));
        }
        else MegoldasLista.Items.Add("Nincs dohányzó nő");
    }
    private void Feladat29()
    {
        foreach (IGrouping<string, Allampolgar> tartomany in Lakossag.GroupBy(t => t.Tartomany))
        {
            Allampolgar legnagyobbAlkesz = tartomany
                .Where(a => a.SorFogyasztasEventeSzoveges != "NA")
                .OrderByDescending(s => s.SorFogyasztasEvente).First();

            MegoldasLista.Items.Add($"{tartomany.Key} - {legnagyobbAlkesz.Id} ({legnagyobbAlkesz.SorFogyasztasEvente}l sör)");
        }
    }
    private void Feladat30()
    {
        MegoldasLista.Items.Add(Lakossag.Where(n => n.Nem == "nő").OrderByDescending(k => k.Kor).First().ToString(true));
        MegoldasLista.Items.Add(Lakossag.Where(n => n.Nem == "férfi").OrderByDescending(k => k.Kor).First().ToString(true));
    }
    private void Feladat31()
    {
        foreach (string nemzetiseg in Lakossag.Select(x => x.Nemzetiseg).Distinct().OrderDescending())
            MegoldasLista.Items.Add(nemzetiseg);
    }
    private void Feladat32()
    {
        foreach (IGrouping<string, Allampolgar> tartomany in Lakossag.GroupBy(t => t.Tartomany).OrderBy(x => x.Count()))
            MegoldasLista.Items.Add(tartomany.Key);
    }
    private void Feladat33()
    {
        foreach (Allampolgar topJovedelmu in Lakossag.OrderByDescending(j => j.NettoJovedelem).Take(3))
            MegoldasLista.Items.Add($"{topJovedelmu.Id}\t{topJovedelmu.NettoJovedelem.ToString("C", german)}");
    }
    private void Feladat34()
    {
        MegoldasMondatos.Content = $"55 kg feletti krumplifogyasztók átlagos súlya: {Lakossag
            .Where(k => k.KrumpliFogyasztasEvente > 55 && k.Nem == "férfi")
            .Average(s => s.Suly):.00} kg";
    }
    private void Feladat35()
    {
        foreach (IGrouping<string, Allampolgar> tartomany in Lakossag.GroupBy(t => t.Tartomany))
            MegoldasLista.Items.Add($"{tartomany.Key} - {tartomany.OrderBy(k => k.Kor).First().Kor} éves");
    }
    private void Feladat36()
    {
        foreach (string nemzetiseg in Lakossag.Select(x => x.Nemzetiseg).Distinct())
            foreach (string tartomany in Lakossag.Where(x => x.Nemzetiseg == nemzetiseg).Select(x => x.Tartomany).Distinct())
                MegoldasLista.Items.Add($"{nemzetiseg} - {tartomany}");
    }
    private void Feladat37()
    {
        double atlag = Lakossag.Average(x => x.NettoJovedelem);

        MegoldasLista.Items.Add($"Átlagos jövedelem: {atlag.ToString("C", german)}");
        MegoldasLista.Items.Add($"Átlagon felül keresők: {Lakossag.Count(x => x.NettoJovedelem > atlag)} db\n");

        foreach (Allampolgar atlagFolotti in Lakossag.Where(x => x.NettoJovedelem > atlag))
            MegoldasLista.Items.Add(atlagFolotti.ToString(false));
    }
    private void Feladat38()
    {
        MegoldasLista.Items.Add($"Nők száma: {Lakossag.Count(x => x.Nem == "nő")}");
        MegoldasLista.Items.Add($"Férfiak száma: {Lakossag.Count(x => x.Nem == "férfi")}");
    }
    private void Feladat39()
    {
        foreach (IGrouping<string, Allampolgar> tartomany in Lakossag.GroupBy(t => t.Tartomany).OrderBy(o => o.Max(m => m.NettoJovedelem)))
            MegoldasLista.Items.Add($"{tartomany.Key} - {tartomany.Max(m => m.NettoJovedelem).ToString("C", german)}");
    }
    private void Feladat40()
    {
        double nemetekHaviJovedelme = Lakossag.Where(x => x.Nemzetiseg == "német").Sum(s => s.HaviNettoJovedelem);
        double nemNemetekHaviJovedelme = Lakossag.Where(x => x.Nemzetiseg != "német").Sum(s => s.HaviNettoJovedelem);

        double abszKulonbseg = Math.Abs(nemetekHaviJovedelme - nemNemetekHaviJovedelme);
        double atlag = (nemetekHaviJovedelme + nemNemetekHaviJovedelme) / 2;

        double szazalekosKulonbseg = abszKulonbseg / atlag * 100;

        MegoldasLista.Items.Add($"Németek havi jövedelme: {nemetekHaviJovedelme.ToString("C", german)}");
        MegoldasLista.Items.Add($"Nem németek havi jövedelme: {nemNemetekHaviJovedelme.ToString("C", german)}\n");
        MegoldasLista.Items.Add($"Százalékos különbség: {szazalekosKulonbseg:.00}%");
    }

    private void Feladat41()
    {
        if (Lakossag.Any(n => n.Nemzetiseg == "török" && n.AktivSzavazo))
            MegoldasTeljes.ItemsSource = Lakossag.Where(n => n.Nemzetiseg == "török" && n.AktivSzavazo).OrderBy(a => Guid.NewGuid()).Take(rnd.Next(1, 11));
        else
            MegoldasMondatos.Content = "Nincs török aktív szavazó";
    }

    private void Feladat42()
    {
        double altagosSorfogyasztas = Lakossag
            .Where(x => x.SorFogyasztasEventeSzoveges != "NA")
            .Average(a => a.SorFogyasztasEvente);

        List<Allampolgar> otAtlagFelettiSorfogyaszto = Lakossag
            .Where(x => x.SorFogyasztasEvente > altagosSorfogyasztas)
            .OrderBy(a => Guid.NewGuid())
            .Take(5).ToList();

        MegoldasLista.Items.Add($"Átlagos sörfogyasztás: {altagosSorfogyasztas:.00} l\n");
        MegoldasLista.Items.Add("5 véletlenszerű állampolgár, akik az átlagosnál több sört fogyasztanak:");
        foreach (Allampolgar polgar in otAtlagFelettiSorfogyaszto)
            MegoldasLista.Items.Add($"{polgar.ToString(true)}");
    }

    private void Feladat43()
    {
        double atlagosJovedelem = Lakossag.Average(x => x.NettoJovedelem);
        var nagyobbTartomanyok = Lakossag
            .GroupBy(t => t.Tartomany)
            .Where(t => t.Min(m => m.NettoJovedelem) > atlagosJovedelem)
            .OrderBy(a => Guid.NewGuid())
            .Take(2);

        MegoldasLista.Items.Add($"Átlagos nettó jövedelem: {atlagosJovedelem.ToString("C", german)}\n");
        foreach (IGrouping<string, Allampolgar> tartomany in nagyobbTartomanyok)
            MegoldasLista.Items.Add($"{tartomany.Key} - legkisebb nettó jövedelem: {tartomany.Min(m => m.NettoJovedelem).ToString("C", german)}");
    }

    private void Feladat44()
    {
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => x.IskolaiVegzetseg == "").OrderBy(a => Guid.NewGuid()).Take(3);
    }

    private void Feladat45()
    {
        MegoldasTeljes.ItemsSource = Lakossag.Where(x => x.Nem == "nő" && x.Nepcsoport != "bajor" && x.IskolaiVegzetseg == "Universität").Take(5);

        double elsoFizetese = Lakossag.Where(x => x.Nem == "nő" && x.Nepcsoport != "bajor" && x.IskolaiVegzetseg == "Universität").First().NettoJovedelem;

        foreach (Allampolgar polgar in Lakossag.Where(x => x.Nem == "nő" && x.NettoJovedelem > elsoFizetese).OrderBy(a => Guid.NewGuid()).Take(3))
            MegoldasLista.Items.Add($"{polgar.ToString(true)}");
    }

    #endregion
}