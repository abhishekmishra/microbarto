using NLua;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace microbarto
{

    //see https://stackoverflow.com/a/2060360
    public class NoBorderToolStripSystemRenderer : ToolStripSystemRenderer
    {
        public NoBorderToolStripSystemRenderer() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }

    public partial class Toolbar : Form
    {
        private List<ToolStripItem> toolStripItems;
        Lua? luaState;
        string? appConfigPath;

        public Toolbar()
        {
            InitializeComponent();

            this.toolStripItems = new();

            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new Size(area.Width, 1);
            this.mainToolStrip.Size = this.Size;
            this.mainToolStrip.Renderer = new NoBorderToolStripSystemRenderer();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.mainToolStrip.MouseEnter +=Toolbar_MouseEnter;
            this.mainToolStrip.MouseLeave +=Toolbar_MouseLeave;
            this.mainToolStrip.BackColor = Color.Black;
            this.BackColor = Color.Black;
            //button.ForeColor = Color.Green;

            LoadConfig();

            InitToolbarFromConfig();
        }

        private void InitToolbarFromConfig()
        {
            luaState = new Lua();
            luaState.State.Encoding = Encoding.UTF8;
            luaState["mb"] = this;
            luaState["microbarto"] = this;
            //luaState.RegisterFunction("print", typeof(MainWindow).GetMethod("print"));
            luaState.LoadCLRPackage();
            string? s = GetEmbeddedResource("microbarto", "microbarto.lua");
            if (s == null)
            {
                throw new Exception("Unable to load microbarto.lua. Terminating.");
            }
            luaState.DoString(s);

            if (appConfigPath != null)
            {
                string mbCfg = File.ReadAllText(appConfigPath, Encoding.UTF8);
                luaState.DoString(mbCfg);
            }
        }

        private void LoadConfig()
        {
            string appConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MicroBarto");
            if (!Directory.Exists(appConfigDir))
            {
                Directory.CreateDirectory(appConfigDir);
            }

            appConfigPath = Path.Combine(appConfigDir, "mbconfig.lua");
            //if (!File.Exists(appConfigPath))
            //{
            string? mbstr = GetEmbeddedResource("microbarto", "default-config.lua");
            if (mbstr == null)
            {
                throw new Exception("Unable to load microbarto.lua. Terminating.");
            }
            File.WriteAllText(appConfigPath, mbstr, Encoding.UTF8);
            //}

            //MessageBox.Show(appConfigPath);
            Debug.WriteLine("App config path = " + appConfigPath);
        }

        private void Toolbar_MouseLeave(object? sender, EventArgs e)
        {
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new System.Drawing.Size(area.Width, 1);
        }

        private void Toolbar_MouseEnter(object? sender, EventArgs e)
        {
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new System.Drawing.Size(area.Width, 25);
        }

        private void Toolbar_Load(object sender, EventArgs e)
        {
            _AddToolButton("Close", ExitToolbar, ToolStripItemAlignment.Right);
        }

        public void AddBtn(string label, LuaFunction callback)
        {
            EventHandler eh = delegate (object? sender, EventArgs e)
            {
                callback.Call(sender, e);
            };

            _AddToolButton(label, eh);
        }

        public void LaunchUrl(string url)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public void LaunchProgram(string path, params string[] args)
        {
            StringBuilder sb = new();
            foreach (string arg in args)
            {
                sb.Append(arg);
                sb.Append(" ");
            }

            Process.Start(new ProcessStartInfo
            {
                Arguments = sb.ToString(),
                FileName=path,
                UseShellExecute=true
            });
        }

        private void _AddToolButton(string label, EventHandler eventHandler, ToolStripItemAlignment alignment = ToolStripItemAlignment.Left)
        {
            ToolStripButton button = new ToolStripButton();
            button.Text = label;
            button.BackColor = Color.Red;
            //button.ForeColor = Color.Green;
            button.ToolTipText = label;
            button.Click+=eventHandler;
            button.Margin = new System.Windows.Forms.Padding(2);
            button.Alignment = alignment;
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
                }
                else
                {
                    return null;
                }
            }
        }

        public void print(params object[] others)
        {
            foreach (object s in others)
            {
                Debug.WriteLine(s.ToString());
            }
        }

    }
}
