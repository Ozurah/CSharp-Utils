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
            Printer.WriteLine(null, 1);
            Printer.WriteLine(null);
            Printer.WriteLine("Test");

            Printer.Print(null);
            Printer.Print(1);
            Printer.Print(null, 1);
            Printer.Print("hello",
                123,
                'c',
                new int[] { 1, 2, 3 },
                new int[,] { { 1, 2, 3 }, { 4, 5, 6 } }, // 2D array
                new int[][] { // Jagged array
                    new int[] {1, 2, 3, 4},
                    new int[] {12, 34, 56}
                },

                new List<int> { 1, 2, 3 },
                new Dictionary<int, string> {
                    { 1, "Hello" }, { 2, "World" }
                },
                new object?[] { "hij", null, 9 },
                true,
                new List<object> {
                    new Dictionary<int, string> {
                        { 1, "Hello" }, { 2, "World" }
                    }
                },
                new Dictionary<object, List<string>> {
                    { 1, new List<string> { "a", "b" } },
                    { 'c', new List<string> { "de", "fg" } },
                    { new List<string> { "key", "Key" }, new List<string> { "val", "Val" } }
                },
                new Dictionary<object, object> {
                    { "niv1",
                        new Dictionary<object, object> {
                            { "niv2",
                                new Dictionary<object, object> {
                                    { "niv3", "Hello" }, { "niv3b", "World" }
                                }
                            }, { "niv2b",
                                new Dictionary<object, object> {
                                    { "niv3", new int[] { 7,8,9} }, { "niv3b", null }
                                }
                            }
                        }
                    },
                    { 'c', new List<string> { "de", "fg" } },
                    { new List<string> { "key", "Key" }, new List<string> { "val", "Val" } }
                }
                );

            Printer.Print(("tuple", 1, 2, 3, ("tuple tuple", 1, new object?[] { "a", 'b', 3, null })));
            Printer.WriteLine(Printer.Indicator);
            //Printer.Indicator.TryAdd(typeof(int), ("int(", ")"));
            //Printer.WriteLine(Printer.Indicator);

        }
    }
}