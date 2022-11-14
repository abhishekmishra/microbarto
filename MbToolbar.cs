using NLua;
using System.Reflection;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace microbarto
{

    public partial class MbToolbar : Form
    {
        private List<ToolStripItem> toolStripItems;
        Lua? luaState;
        string? appConfigPath;

        // Default visual configuration
        int toolbarHeight = 32;
        Color toolbarBgColor = Color.FromArgb(0x88, 0x88, 0x88);

        public MbToolbar()
        {
            InitializeComponent();

            this.toolStripItems = new();

            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;
            this.Size = new Size(area.Width, 1);
            this.mainToolStrip.Size = this.Size;
            NoBorderToolStripSystemRenderer toolStripRenderer = new NoBorderToolStripSystemRenderer();
            this.mainToolStrip.Renderer = toolStripRenderer;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.mainToolStrip.MouseEnter +=Toolbar_MouseEnter;
            this.mainToolStrip.MouseLeave +=Toolbar_MouseLeave;
            this.mainToolStrip.BackColor = toolbarBgColor;

            // This setting is immaterial - the form cannot be seen,
            // only the mainToolStrip is visible
            this.BackColor = Color.White;

            InitAppConfig();

            InitToolbarFromConfig();
        }

        //TODO: create an exception class for Microbarto exceptions
        //TODO: subclass it for lua or config exceptions.

        /// <summary>
        /// 1. Initializes lua - creates a new luaState object.
        /// 2. Loads the clr package for interop with dotnet
        /// 3. Loads the embedded resource "microbarto.lua" which
        ///     defines some key lua functions to configure the toolbar.
        /// 4. Finally applies the user configuration for the toolbar.
        /// </summary>
        /// <exception cref="Exception">
        /// If unable to load embedded resource microbarto.lua
        /// </exception>
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

        private void InitAppConfig()
        {
            string appConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MicroBarto");
            if (!Directory.Exists(appConfigDir))
            {
                Directory.CreateDirectory(appConfigDir);
            }

            appConfigPath = Path.Combine(appConfigDir, "mbconfig.lua");
            if (!File.Exists(appConfigPath))
            {
                string? mbstr = GetEmbeddedResource("microbarto", "default-config.lua");
                if (mbstr == null)
                {
                    throw new Exception("Unable to load microbarto.lua. Terminating.");
                }
                File.WriteAllText(appConfigPath, mbstr, Encoding.UTF8);
            }

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
            this.Size = new System.Drawing.Size(area.Width, toolbarHeight);
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
            MbToolbarButton button = new MbToolbarButton();
            button.AutoSize= true;
            button.Width = 150;
            button.Text = label;

            button.ToolTipText = label;
            button.Click+=eventHandler;

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

        private void configMenuItem_Click(object sender, EventArgs e)
        {
            string appConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MicroBarto");
            string appConfigPath = Path.Combine(appConfigDir, "mbconfig.lua");
            _=Process.Start("notepad.exe", appConfigPath);
        }

        private void aboutMicrobartoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MbAboutBox mbAboutBox = new MbAboutBox();
            mbAboutBox.ShowDialog();
        }
    }

    //see https://stackoverflow.com/a/2060360
    public class NoBorderToolStripSystemRenderer : ToolStripSystemRenderer
    {
        public NoBorderToolStripSystemRenderer()
        {
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }

        // for overriding hover border on toolstrip button
        // see https://stackoverflow.com/a/29169459/9483968
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item is MbToolbarButton)
            {
                MbToolbarButton btn = (MbToolbarButton)e.Item;
                if (!e.Item.Selected)
                {
                    base.OnRenderButtonBackground(e);
                }
                else
                {
                    Rectangle rectangle = new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1);
                    e.Graphics.FillRectangle(btn.ButtonHoverBgBrush, rectangle);
                    e.Graphics.DrawRectangle(btn.ButtonHoverBorderPen, rectangle);
                }
            }
            else
            {
                base.OnRenderButtonBackground(e);
            }
        }
    }


}
