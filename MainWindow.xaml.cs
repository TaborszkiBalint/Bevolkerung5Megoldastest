using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bevolkerung;
public partial class MainWindow : Window
{
    List<Allampolgar> lakossag = [];
    Binding myBinding = new Binding("Lakossag");
    public MainWindow()
    {
        InitializeComponent();
        AdatokFeltoltese();
        DataContext = this;

    }

    private void AdatokFeltoltese()
    {
        using (StreamReader sr = new StreamReader(path: @"..\..\..\src\bevölkerung.txt", encoding: Encoding.UTF8))
        {
            _ = sr.ReadLine();
            while (!sr.EndOfStream)
                lakossag.Add(new Allampolgar(sr.ReadLine()));
            myBinding.Source = lakossag;
        }
    }
}