using Ozurah.Utils;
using System.Windows;

namespace DevUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Class1.HelloWorld();
        }

        private void Printer_1_Click(object sender, RoutedEventArgs e)
        {
            Printer.Print("hello",
                123,
                'c',
                new int[] { 1, 2, 3 },
                new List<int> { 1, 2, 3 },
                new Dictionary<int, string> {
                    { 1, "Hello" }, { 2, "World" }
                },
                new object?[] { "hij", null, 9 }
                );

            Printer.WriteLine(Printer.Indicator);
            //Printer.Indicator.TryAdd(typeof(int), ("int(", ")"));
            //Printer.WriteLine(Printer.Indicator);

        }
    }
}