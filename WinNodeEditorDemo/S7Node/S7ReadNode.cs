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
    /// S7数据读取节点，用于从PLC中读取数据
    /// </summary>
    [STNode("/S7Node/", "tuling", "wx:zhy_cxx", "https://tuling.online", "This node can init s7")]
    public class S7ReadNode : S7BaseNode
    {
        #region 字段

        /// <summary>
        /// 数据读取间隔(毫秒)
        /// </summary>
        private const int READ_INTERVAL = 200;

        /// <summary>
        /// 地址输入选项
        /// </summary>
        private STNodeOption addressOption;

        /// <summary>
        /// 数据输出选项
        /// </summary>
        private STNodeOption resultOption;

        /// <summary>
        /// 字符串格式化对象
        /// </summary>
        private StringFormat stringFormat;

        #endregion

        #region 属性

        private string readAddress = "M100";
        /// <summary>
        /// PLC读取地址
        /// </summary>
        [STNodeProperty("读取地址", "PLC data address")]
        public string ReadAddress
        {
            get { return readAddress; }
            set { readAddress = value; }
        }

        private float resultValue;
        /// <summary>
        /// 读取结果
        /// </summary>
        [STNodeProperty("读取结果", "Read result value")]
        public float ResultValue
        {
            get { return resultValue; }
            private set
            {
                resultValue = value;
                resultOption?.TransferData(value);
                Invalidate();
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
            InitializeNodeProperties();
            InitializeOptions();
            StartDataReader();
        }

        /// <summary>
        /// 初始化节点属性
        /// </summary>
        private void InitializeNodeProperties()
        {
            Title = "PLC单点读取";
            stringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center
            };
        }

        /// <summary>
        /// 初始化输入输出选项
        /// </summary>
        private void InitializeOptions()
        {
            // 初始化输入选项
            plcHandleOption = new STNodeOption("PLC句柄", typeof(SiemensS7Net), true);
            addressOption = new STNodeOption("PLC地址", typeof(string), true);
            InputOptions.Add(plcHandleOption);
            InputOptions.Add(addressOption);

            // 初始化输出选项
            resultOption = new STNodeOption("读取结果", typeof(float), false);
            OutputOptions.Add(resultOption);

            // 注册PLC句柄数据传输事件
            plcHandleOption.DataTransfer += HandlePlcDataTransfer;
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
                    if (plc != null)
                    {
                        OperateResult<float> result = plc.ReadFloat(ReadAddress);
                        if (result.IsSuccess)
                        {
                            ResultValue = result.Content;
                        }
                        else
                        {
                            HandleReadError(result);
                        }
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
        /// 处理PLC句柄数据传输事件
        /// </summary>
        private void HandlePlcDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected)
            {
                plc = e.TargetOption.Data as SiemensS7Net;
            }
            else
            {
                plc = null;
            }
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
        private void HandleReadError(OperateResult<float> result)
        {
            Console.WriteLine($"读取PLC数据失败: {result.Message}");
            ResultValue = 0;
        }

        /// <summary>
        /// 处理异常信息
        /// </summary>
        private void HandleException(string message, Exception ex)
        {
            Console.WriteLine($"{message}: {ex.Message}");
            ResultValue = 0;
        }

        #endregion
    }
}