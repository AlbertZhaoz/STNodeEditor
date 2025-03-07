using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HslCommunication.Profinet.Siemens;
using ST.Library.UI.NodeEditor;

namespace WinNodeEditorDemo.S7Node
{
    /// <summary>
    /// S7节点的基类，提供与西门子PLC通信的基本功能
    /// </summary>
    [STNode("/S7Node/", "tuling", "wx:zhy_cxx", "https://tuling.online", "This node can init s7")]
    public class S7InitNode : S7BaseNode
    {
        #region 属性

        private string ipAddress = "127.0.0.1";
        /// <summary>
        /// PLC的IP地址
        /// </summary>
        [STNodeProperty("IP地址", "Siemens PLC IP address")]
        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        private int port = 102;
        /// <summary>
        /// PLC的端口号
        /// </summary>
        [STNodeProperty("端口号", "Siemens PLC port")]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 节点创建时的初始化
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            InitializeNodeProperties();
            InitializePLC(IpAddress, Port);
            StartConnectionMonitor();
            InitializeOutputOption();
        }

        /// <summary>
        /// 初始化节点属性
        /// </summary>
        private void InitializeNodeProperties()
        {
            Title = "PLC初始化";
            TitleColor = Color.FromArgb(200, 50, 50);
            rackNumber = 0;
            slotNumber = 1;
        }

        /// <summary>
        /// 初始化输出选项
        /// </summary>
        private void InitializeOutputOption()
        {
            var plcHandleOption = new STNodeOption("PLC句柄", typeof(SiemensS7Net), false);
            OutputOptions.Add(plcHandleOption);
            plcHandleOption.TransferData(plc);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新PLC连接参数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="rack">机架号</param>
        /// <param name="slot">插槽号</param>
        public virtual void UpdateConnectionParameters(string ip, int port = 102, byte rack = 0, byte slot = 1)
        {
            IpAddress = ip;
            Port = port;
            rackNumber = rack;
            slotNumber = slot;
            
            DisconnectPLC();
            InitializePLC(IpAddress, Port);
        }

        #endregion
    }
}