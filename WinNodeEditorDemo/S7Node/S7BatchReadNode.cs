using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using ST.Library.UI.NodeEditor;

namespace WinNodeEditorDemo.S7Node
{
    /// <summary>
    /// S7批量数据读取节点，用于从PLC中批量读取数据
    /// </summary>
    [STNode("/S7Node/", "tuling", "wx:zhy_cxx", "https://tuling.online", "This node can batch read data from s7")]
    public class S7BatchReadNode : S7BaseNode
    {
        #region 字段

        /// <summary>
        /// PLC通信实例
        /// </summary>
        private SiemensS7Net plc;

        /// <summary>
        /// 数据读取间隔(毫秒)
        /// </summary>
        private const int READ_INTERVAL = 200;

        /// <summary>
        /// PLC句柄输入选项
        /// </summary>
        private STNodeOption plcHandleOption;

        /// <summary>
        /// 数据输出选项
        /// </summary>
        private STNodeOption resultOption;

        /// <summary>
        /// 读取地址列表
        /// </summary>
        private List<string> addressList = new List<string>();

        /// <summary>
        /// 读取结果列表
        /// </summary>
        private Dictionary<string, float> resultDict = new Dictionary<string, float>();

        #endregion

        #region 属性

        private string addresses = "M100,M104,M108";
        /// <summary>
        /// PLC读取地址列表，使用逗号分隔
        /// </summary>
        [STNodeProperty("读取地址列表", "PLC data addresses, split by comma")]
        public string Addresses
        {
            get { return addresses; }
            set 
            { 
                addresses = value;
                UpdateAddressList();
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 节点创建时的初始化
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "PLC批量读取";
            TitleColor = Color.FromArgb(200, 50, 50);
            InitializeOptions();
            UpdateAddressList();
            StartDataReader();
        }

        /// <summary>
        /// 初始化选项
        /// </summary>
        private void InitializeOptions()
        {
            // 初始化输入选项
            plcHandleOption = new STNodeOption("PLC句柄", typeof(SiemensS7Net), true);
            InputOptions.Add(plcHandleOption);

            // 初始化输出选项
            resultOption = new STNodeOption("读取结果", typeof(Dictionary<string, float>), false);
            OutputOptions.Add(resultOption);

            // 注册数据传输事件
            plcHandleOption.DataTransfer += HandlePlcDataTransfer;
        }

        /// <summary>
        /// 更新地址列表
        /// </summary>
        private void UpdateAddressList()
        {
            addressList.Clear();
            if (!string.IsNullOrEmpty(Addresses))
            {
                string[] addrs = Addresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string addr in addrs)
                {
                    addressList.Add(addr.Trim());
                }
            }
        }

        #endregion

        #region 数据读取

        /// <summary>
        /// 启动数据读取线程
        /// </summary>
        private void StartDataReader()
        {
            var thread = new Thread(ReadPlcData)
            {
                IsBackground = true
            };
            thread.Start();
        }

        /// <summary>
        /// 读取PLC数据
        /// </summary>
        private void ReadPlcData()
        {
            while (true)
            {
                try
                {
                    if (plc != null && addressList.Count > 0)
                    {
                        resultDict.Clear();
                        foreach (string address in addressList)
                        {
                            OperateResult<float> result = plc.ReadFloat(address);
                            if (result.IsSuccess)
                            {
                                resultDict[address] = result.Content;
                            }
                            else
                            {
                                HandleReadError(result, address);
                            }
                        }
                        resultOption.TransferData(resultDict);
                    }
                    Thread.Sleep(READ_INTERVAL);
                }
                catch (Exception ex)
                {
                    HandleException("数据读取异常", ex);
                    Thread.Sleep(READ_INTERVAL);
                }
            }
        }

        /// <summary>
        /// 处理PLC数据传输事件
        /// </summary>
        private void HandlePlcDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                plc = e.TargetOption.Data as SiemensS7Net;

                if (plc != null)
                {
                    UpdateConnectionStatus(true);
                }
            }
            Invalidate();
        }

        /// <summary>
        /// 更新连接状态和界面显示
        /// </summary>
        private void UpdateConnectionStatus(bool connected)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                TitleColor = connected ? Color.FromArgb(50, 150, 50) : Color.FromArgb(200, 50, 50);
                Invalidate();
            }));
        }


        #endregion

        #region 异常处理

        /// <summary>
        /// 处理读取错误
        /// </summary>
        private void HandleReadError(OperateResult result, string address)
        {
            Console.WriteLine($"地址[{address}]读取失败: {result.Message}");
        }

        /// <summary>
        /// 处理异常信息
        /// </summary>
        protected virtual void HandleException(string message, Exception ex)
        {
            Console.WriteLine($"{message}: {ex.Message}");
        }

        #endregion
    }
}