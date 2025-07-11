using Ozurah.Utils;
using Ozurah.Utils.Enums;
using Ozurah.Utils.Tuples;
using System.Diagnostics;
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

            Printer.Print(Category.A, Category.B, (int)Category.A, (int)Category.B);
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

        private void Printer_4_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("***** Printer mode Debug");
            Printer.UsedWriteLineMode = Printer.WriteLineMode.Debug;
            Printer.Print("a", 1, true);

            Debug.WriteLine("***** Printer mode Console");
            Printer.UsedWriteLineMode = Printer.WriteLineMode.Console;
            Printer.Print("a", 1, true);

            Debug.WriteLine("***** Printer mode Silent");
            Printer.UsedWriteLineMode = Printer.WriteLineMode.Silent;
            Printer.Print("a", 1, true);

            Debug.WriteLine("***** RESET Printer mode to Debug");
            Printer.UsedWriteLineMode = Printer.WriteLineMode.Debug;
            Printer.Print("a", 1, true);
        }

        private void Printer_5_Click(object sender, RoutedEventArgs e)
        {
            string smiley = ":D";
            Printer.WriteLinePrefix = DateTime.Now.ToString;
            Printer.WriteLineSuffix = () => smiley;

            Printer.WriteLine("test");

            smiley = ";)";

            Printer.WriteLine("test");

            Printer.WriteLinePrefix = null;
            Printer.WriteLineSuffix = null;
        }

        private void Enum_1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Category? aa = null;
                String? aaa = null;
                string aaaa = null;
                var id = EnumUtils<Category>.IsDefined(aa);
                var id2 = EnumUtils<Category>.IsDefined(aaa);
                var id3 = EnumUtils<Category>.IsDefined(aaaa);
                var id4 = EnumUtils<Category>.IsDefined((Category)22);
                var id5 = EnumUtils<Category>.IsDefined((Category)10);
                var id6 = EnumUtils<Category>.IsDefined("A");
                var id7 = EnumUtils<Category>.IsDefined(22);
                var c = Enum.GetValues<Category>();
                var r = EnumUtils<Category>.ToArray();
                var ttt = Enum.IsDefined(typeof(Category), "aa");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Enum_2_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                //Category bstring = (Category)"B"; // Non possible, contrairement aux int :(
                Category bint = (Category)22; // Ne l�ve pas d'exception, m�me si 22 n'est pas pr�sent dans l'enum...
                try
                {
                    EnumUtils<Category>.ThrowIfUnkownValue(22); // Ici on a l'exception :)
                }
                catch (ArgumentException)
                {
                    Debug.WriteLine("Exception OK");
                }

                Category[] collection = EnumUtils<Category>.ToArray();
                Category bstring = EnumUtils<Category>.FromString("B");

                try
                {
                    Category XYZ = EnumUtils<Category>.FromString("XYZ");
                }
                catch (ArgumentException)
                {
                    Debug.WriteLine("Exception OK");
                }

                try
                {
                    Category bstring2 = EnumUtils<Category>.FromString("b");
                }
                catch (ArgumentException)
                {
                    Debug.WriteLine("Exception OK");
                }

                Category bstring3 = EnumUtils<Category>.FromString("b", true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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

    public enum Category
    {
        A,
        B = 10
    }

    public enum Category2
    {
        A,
        C = 10,
    }
}