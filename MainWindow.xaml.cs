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
        MegoldasMondatos.Content = $"1. feladat: Az állampolgárok száma: {Lakossag.Count}";
    }

    private void Feladat2()
    {
        MegoldasMondatos.Content = $"2. feladat: Az állampolgárok átlagéletkora: {Lakossag.Average(a => a.Kor):F2} év";
    }

}