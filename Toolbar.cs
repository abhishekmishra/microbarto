using NLua;
using System.Reflection;
using System.Text;

namespace microbarto
{
    public partial class Toolbar : Form
    {
        private List<ToolStripItem> toolStripItems;
        Lua luaState;

        public Toolbar()
        {
            InitializeComponent();

            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new System.Drawing.Size(area.Width, 2);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.mainToolStrip.MouseEnter +=Toolbar_MouseEnter;
            this.mainToolStrip.MouseLeave +=Toolbar_MouseLeave;

            this.toolStripItems = new();
            luaState = new Lua();
            luaState["mb"] = this;
            //luaState.RegisterFunction("print", typeof(MainWindow).GetMethod("print"));
            luaState.LoadCLRPackage();
            string? s = GetEmbeddedResource("microbarto", "microbarto.lua");
            if (s == null)
            {
                throw new Exception("Unable to load microbarto.lua. Terminating.");
            }
            luaState.DoString(s);
        }

        private void Toolbar_MouseLeave(object? sender, EventArgs e)
        {
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new System.Drawing.Size(area.Width, 2);
        }

        private void Toolbar_MouseEnter(object? sender, EventArgs e)
        {
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new System.Drawing.Size(area.Width, 25);
        }

        private void Toolbar_Load(object sender, EventArgs e)
        {
            AddToolButton("Close", ExitToolbar);
        }

        private void AddToolButton(string label, EventHandler eventHandler)
        {
            ToolStripButton button = new ToolStripButton();
            button.Text = label;
            button.BackColor = Color.Black;
            button.ForeColor = Color.Green;
            button.ToolTipText = label;
            button.Click+=eventHandler;
            this.toolStripItems.Add(button);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            button});
        }

        private void ExitToolbar(object? sender, EventArgs e)
        {
            this.Close();
        }

        private EventHandler Button_Click()
        {
            return delegate (object? sender, EventArgs e)
            {
                MessageBox.Show(this, "blah");
            };
        }
        public string? GetEmbeddedResource(string namespacename, string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = namespacename + "." + filename;

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string result = reader.ReadToEnd();
                        return result;
                    }
                } else
                {
                    return null;
                }
            }
        }
    }
}
