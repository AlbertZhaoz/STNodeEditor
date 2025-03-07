using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ST.Library.UI.NodeEditor;

namespace WinNodeEditorDemo
{
    /// <summary>
    /// 主窗体类，用于展示和管理节点编辑器界面
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// 窗体构造函数，初始化组件并设置窗体位置
        /// </summary>
        public Form1() {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// 窗体加载时的初始化配置
        /// 包括：节点属性网格配置、节点树视图加载、节点编辑器事件绑定等
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            stNodePropertyGrid1.Text = "Node_Property";
            stNodeTreeView1.LoadAssembly(Application.ExecutablePath);
            stNodeEditor1.LoadAssembly(Application.ExecutablePath);

            stNodeEditor1.ActiveChanged += (s, ea) => stNodePropertyGrid1.SetNode(stNodeEditor1.ActiveNode);
            //stNodeEditor1.SelectedChanged += (s, ea) => stNodePropertyGrid1.SetSTNode(stNodeEditor1.ActiveNode);
            stNodeEditor1.OptionConnected += (s, ea) => stNodeEditor1.ShowAlert(ea.Status.ToString(), Color.White, ea.Status == ConnectionStatus.Connected ? Color.FromArgb(125, Color.Green) : Color.FromArgb(125, Color.Red));
            stNodeEditor1.CanvasScaled += (s, ea) => stNodeEditor1.ShowAlert(stNodeEditor1.CanvasScale.ToString("F2"), Color.White, Color.FromArgb(125, Color.Yellow));
            stNodeEditor1.NodeAdded += (s, ea) => ea.Node.ContextMenuStrip = contextMenuStrip1;

            stNodePropertyGrid1.SetInfoKey("Author", "Mail", "Link", "Show Help");
            stNodeTreeView1.PropertyGrid.SetInfoKey("Author", "Mail", "Link", "Show Help");

            stNodeEditor1.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Renderer = new ToolStripRendererEx();
        }

        /// <summary>
        /// 窗体加载事件处理方法
        /// 当前包含一些被注释的测试代码，用于调整窗体大小和布局
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void Form1_Load(object sender, EventArgs e) {
            //int nLines = 0;
            //foreach (var v in Directory.GetFiles("../../../", "*.cs", SearchOption.AllDirectories)) {
            //    nLines += File.ReadAllLines(v).Length;
            //}
            //MessageBox.Show(nLines.ToString());
            //this.Resize += (s, ea) => this.Text = this.Size.ToString();
            //this.BeginInvoke(new MethodInvoker(() => {
            //    //this.Size = new Size(488, 306);
            //    this.Size = new Size(488, 246);
            //    stNodeTreeView1.Visible = false;
            //    stNodePropertyGrid1.Top = stNodeEditor1.Top;
            //    stNodePropertyGrid1.Height = stNodeEditor1.Height;
            //    stNodeTreeView1.Height = stNodeEditor1.Height;
            //}));
        }

        /// <summary>
        /// 打开按钮点击事件处理
        /// 用于打开并加载已保存的节点编辑器画布文件(.stn)
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void btn_open_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.stn|*.stn";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            stNodeEditor1.Nodes.Clear();
            stNodeEditor1.LoadCanvas(ofd.FileName);
        }

        /// <summary>
        /// 保存按钮点击事件处理
        /// 用于将当前节点编辑器画布保存为文件(.stn)
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void btn_save_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.stn|*.stn";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            stNodeEditor1.SaveCanvas(sfd.FileName);
        }

        /// <summary>
        /// 锁定连接菜单项点击事件处理
        /// 用于切换当前选中节点的连接锁定状态
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void lockConnectionToolStripMenuItem_Click(object sender, EventArgs e) {
            stNodeEditor1.ActiveNode.LockOption = !stNodeEditor1.ActiveNode.LockOption;
        }

        /// <summary>
        /// 锁定位置菜单项点击事件处理
        /// 用于切换当前选中节点的位置锁定状态
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void lockLocationToolStripMenuItem_Click(object sender, EventArgs e) {
            if (stNodeEditor1.ActiveNode == null) return;
            stNodeEditor1.ActiveNode.LockLocation = !stNodeEditor1.ActiveNode.LockLocation;
        }

        /// <summary>
        /// 删除菜单项点击事件处理
        /// 用于从节点编辑器中移除当前选中的节点
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (stNodeEditor1.ActiveNode == null) return;
            stNodeEditor1.Nodes.Remove(stNodeEditor1.ActiveNode);
        }
    }
}
