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

            Printer.Print(new int[] { 1, 2, 3 }.ToArray());
            Printer.Print(new List<object> { 1, 2, 3 });
            Printer.Print(new List<object> { 1, 2, 3 }.ToArray());
            Printer.Print(new List<Person> { new() { Age = 10 }, new() });
            Printer.Print(new List<Person> { new() { Age = 10 }, new() }.ToArray());
            Printer.Print(new List<Int32> { 1, 2, 3 });
            Printer.Print(new List<Int32> { 1, 2, 3 }.ToArray());
            Printer.Print(new List<string> { "a", "b" });
            Printer.Print(new List<string> { "a", "b" }.ToArray());
            Printer.Print((1, 2, 3).AsEnumerable());
            Printer.Print((1, 2, 3).AsEnumerable().ToArray());

            Printer.Print(new List<City> { new() { Code = 10 }, new() }); //N'affichera pas la classe, mais le ToString
            Printer.Print(new City().GetType()); //N'affichera pas le ToString, mais la classe

            (string a, int b) namedtuple = new("txt", 1);
            Printer.Print(namedtuple);

            var anonType = new { a = 10, b = "str" };
            Printer.Print(anonType);

            Printer.Print(new Structure { Id = 1, Name = "Test" });
            Printer.Print(new Coords(11, "txt"));
            Printer.Print(new CoordsNoToString(11, "txt"));
        }

        private void Printer_2_Click(object sender, RoutedEventArgs e)
        {
            Printer.Print(1, 2L, 3f);

            //if (Printer.Indicator.ContainsKey(typeof(int)))
            //{
            Printer.Indicator[typeof(int)] = ("int(", ")");
            //}

            Printer.Print(1, 2L, 3f);

            Printer.ResetIndicator();

            Printer.Print(1, 2L, 3f);
        }

        private void Printer_3_Click(object sender, RoutedEventArgs e)
        {
            // Sans PrintStr
            {
                Printer.PrintObject(new Structure { Id = 1, Name = "Test" }, false);
                Printer.PrintObject(new Coords(1, "Test"), false);
                Printer.PrintObject(new CoordsNoToString(1, "Test"), false);

                (string a, int b) namedtuple = new("txt", 1);
                Printer.PrintObject(namedtuple, false);

                Printer.PrintObject(new Person(), false);
            }

            // avec PrintStr
            {
                Printer.PrintObject(new Structure { Id = 1, Name = "Test" }, true);
                Printer.PrintObject(new Coords(1, "Test"), true);
                Printer.PrintObject(new CoordsNoToString(1, "Test"), true);

                (string a, int b) namedtuple = new("txt", 1);
                Printer.PrintObject(namedtuple, true);

                Printer.PrintObject(new Person(), true);
            }
        }
    }

    public struct Structure
    {
        public int Id;
        public string Name;
    }

    public struct CoordsNoToString
    {
        public CoordsNoToString(double x, string y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public string Y { get; }
    }

    public struct Coords
    {
        private double x;

        public Coords(double x, string y)
        {
            X = x;
            Y = y;
        }

        public double X { get { return x; } set { x = value; } }
        public string Y { get; }

        public override string ToString() => $"{X};{Y}";
    }

    public class Person
    {
        public int Age { get; set; } = 20;
    }

    public class City
    {
        public int Code { get; set; } = 1234;

        public override string ToString()
        {
            return $"{Code}";
        }
    }
}