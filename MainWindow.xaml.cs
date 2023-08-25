using System.Diagnostics;
using System.Security.RightsManagement;
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

namespace HL_FlagTicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class SubFlag
        {
            uint bit; // Which bit does this sub flag represent?
            string displayName; // Display name of the sub flag
            string description; // Description of the subflag
            bool toggle; // Is this subflag set or not?

            public SubFlag(uint bit, string displayName, string description, bool toggle)
            {
                this.bit = bit;
                this.displayName = displayName;
                this.description = description;
                this.toggle = toggle;
            }

            public bool ValidBit()
            {
                return (bit < 0 || bit > 63);
            }

            public uint Bit
            { 
                get { return bit; }
                set { bit = value; }
            }

            public string DisplayName
            {
                get { return this.displayName;  }
                set { this.displayName = value; }
            }

            public string Description
            {
                get { return this.description; }
                set { this.description = value; }
            }

            public bool Toggle
            {
                get { return toggle; }
                set { toggle = value; }
            }

        }

        public class Flag
        {
            string name; // Name of the flag var, this is usually the internal name
            string displayName; // Display name of the flag, human readable text
            string description; // Description of the flag, what it does and special instructions and warnings
            List<SubFlag> subflags; // all of the subflags are contained here

            public Flag(string name, string displayName, string description)
            {
                this.name = name;
                this.displayName = displayName;
                this.description = description;
                subflags = new List<SubFlag>();
            }


            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public string DisplayName
            {
                get { return this.displayName; }
                set { this.displayName = value; }
            }

            public string Description
            {
                get { return this.description; }
                set { this.description = value; }
            }

            public uint Bits
            {
                get { return (uint)subflags.Count();  }
            }

            public ulong Value
            {
                get
                {
                    ulong rValue = 0;
                    foreach (SubFlag sub in subflags)
                    {
                        rValue += (ulong)(sub.Toggle ? 1 : 0) * sub.Bit;
                    }

                    return rValue;
                }
                set
                {
                    foreach (SubFlag sub in subflags)
                    {
                        sub.Toggle = ((value & (ulong)(1UL << (int)sub.Bit)) != 0UL);
                    }
                }
            }

            public List<SubFlag> SubFlags
            {
                get { return this.subflags; }
                set { subflags = value; }
            }

            public void AddSubFlag(uint bit, bool toggle, string displayName, string description)
            {
                // We already have 64 bits, do not bother and print error message
                if (subflags.Count() >= 64)
                {
                    return;
                }

                foreach (SubFlag sub in subflags)
                {
                    // If the bit is already in, do not set it, print an error message as well to warn the user about this, do not interrupt process
                    if (sub.Bit == bit)
                    {
                        return;
                    }
                }

                SubFlag newSubFlag = new SubFlag(bit, displayName, description, toggle);
                subflags.Add(newSubFlag);
            }

            public void AddSubFlag(SubFlag addSubFlag)
            {
                // We already have 64 bits, do not bother and print error message
                if (subflags.Count() >= 64)
                {
                    return;
                }

                foreach (SubFlag sub in subflags)
                {
                    // If the bit is already in, do not set it, print an error message as well to warn the user about this, do not interrupt process
                    if (sub.Bit == addSubFlag.Bit)
                    {
                        return;
                    }
                }

                subflags.Add(addSubFlag);
            }

        }

        // The class for listable elements. This includes categories
        public class FTListObject
        {
            string name;
            string description;

            public FTListObject(string name, string description)
            {
                this.name = name;
                this.description = description;
            }
        }

        public class FTObject : FTListObject
        {
            List<Flag> flags; // An object can contain multiple flags. Although the case for not having flags will be handled, why bother?

            public FTObject(string name, string description) : base(name, description)
            {
                this.flags = new List<Flag>();
            }
        }

        public List<Flag> flaglist;
        public List<SubFlag> subflaglist;

        public MainWindow()
        {
            InitializeComponent();

            this.flaglist = new List<Flag>();
            this.subflaglist = new List<SubFlag>();

            Flag flag1 = new Flag("m_Flag1", "Flag One", "First Test Flag");
            SubFlag subflag11 = new SubFlag(0, "Property 1", "Test subflag one.", false);
            SubFlag subflag12 = new SubFlag(1, "Property 2", "Test subflag two.", true);
            SubFlag subflag13 = new SubFlag(2, "Property 3", "Test subflag three.", true);

            Flag flag2 = new Flag("m_Flag2", "Flag Two", "Second Test Flag");
            SubFlag subflag21 = new SubFlag(0, "Property 1", "Test subflag one.", true);
            SubFlag subflag22 = new SubFlag(3, "Property 3", "Test subflag three.", false);
            SubFlag subflag23 = new SubFlag(3, "Property 3", "Test subflag three.", false);
            SubFlag subflag24 = new SubFlag(3, "Property 3", "Test subflag three.", false);

            flag1.AddSubFlag(subflag11);
            flag1.AddSubFlag(subflag12);
            flag1.AddSubFlag(subflag13);

            flag2.AddSubFlag(subflag21);
            flag2.AddSubFlag(subflag22);
            flag2.AddSubFlag(subflag23);
            flag2.AddSubFlag(subflag24);

            flaglist.Add(flag1);
            flaglist.Add(flag2);

            subflaglist.Add(subflag11);
            subflaglist.Add(subflag12);
            subflaglist.Add(subflag13);

            this.DataContext = flaglist;
            listView.ItemsSource = flaglist;
            dataGrid.ItemsSource = subflaglist;
        }

        public void ChangeSubFlagList()
        {
            dataGrid.ItemsSource = null;
        }

        public void ChangeFlagList()
        {

        }

        public void ChangeObject()
        {

        }

        public List<Flag> Flags() { return flaglist; }

        // For now a test for handling change of subflag elements
        private void MySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeSubFlagList();
        }
    }
}