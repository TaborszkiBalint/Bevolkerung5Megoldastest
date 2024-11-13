using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Bevolkerung;
public partial class MainWindow : Window
{
    List<Allampolgar> Lakossag = [];
    List<Action> feladatok = [];
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
        MegoldasMondatos.Content = "";
        MegoldasTeljes.ItemsSource = null;
        MegoldasLista.Items.Clear();

        feladatok[FeladatokCombo.SelectedIndex].Invoke();
    }

    private void Feladat1()
    {
        MegoldasMondatos.Content = $"1. feladat:  Legmagasabb nettó éves jövedelem: {Lakossag.OrderByDescending(l => l.NettoJovedelem).First().NettoJovedelem}€";
    }

    private void Feladat2()
    {
        MegoldasMondatos.Content = $"2. feladat:  Állampolgárok átlagos nettó éves jövedelme: {Lakossag.Average(a => a.NettoJovedelem):F2}€";
    }

    private void Feladat3()
    {
        foreach (var tartomany in Lakossag.GroupBy(l => l.Tartomany))
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
    private void feladat6()
    {

    }

}