using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using ST.Library.UI.NodeEditor;

namespace WinNodeEditorDemo.S7Node
{
    /// <summary>
    /// S7数据写入节点，用于向PLC写入数据
    /// </summary>
    [STNode("/S7Node/", "tuling", "wx:zhy_cxx", "https://tuling.online", "This node can write data to s7")]
    public class S7WriteNode : S7BaseNode
    {
        #region 字段

        /// <summary>
        /// 数据输入选项
        /// </summary>
        private STNodeOption dataOption;

        /// <summary>
        /// 写入结果输出选项
        /// </summary>
        private STNodeOption resultOption;

        #endregion

        #region 属性

        private string writeAddress = "M100";
        /// <summary>
        /// PLC写入地址
        /// </summary>
        [STNodeProperty("写入地址", "PLC write address")]
        public string WriteAddress
        {
            get { return writeAddress; }
            set { writeAddress = value; }
        }

        private float writeValue;
        /// <summary>
        /// 写入值
        /// </summary>
        [STNodeProperty("写入值", "Value to write")]
        public float WriteValue
        {
            get { return writeValue; }
            set 
            { 
                writeValue = value;
                WriteData();
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
            Title = "PLC写入";
            TitleColor = Color.FromArgb(200, 50, 50);
            InitializeOptions();
        }

        /// <summary>
        /// 初始化选项
        /// </summary>
        private void InitializeOptions()
        {
            // 初始化输入选项
            plcHandleOption = new STNodeOption("PLC句柄", typeof(SiemensS7Net), true);
            dataOption = new STNodeOption("写入数据", typeof(float), true);
            InputOptions.Add(plcHandleOption);
            InputOptions.Add(dataOption);

            // 初始化输出选项
            resultOption = new STNodeOption("写入结果", typeof(bool), false);
            OutputOptions.Add(resultOption);

            // 注册数据传输事件
            plcHandleOption.DataTransfer += HandlePlcDataTransfer;
            dataOption.DataTransfer += HandleDataTransfer;
        }

        #endregion

        #region 数据写入

        /// <summary>
        /// 写入数据到PLC
        /// </summary>
        private void WriteData()
        {
            try
            {
                if (plc != null)
                {
                    OperateResult result = plc.Write(WriteAddress, WriteValue);

                    if (result.IsSuccess)
                    {
                        resultOption.TransferData(true);
                    }
                    else
                    {
                        HandleWriteError(result);
                        resultOption.TransferData(false);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException("数据写入异常", ex);
                resultOption.TransferData(false);
            }
        }

        /// <summary>
        /// 处理写入数据传输事件
        /// </summary>
        private void HandleDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                WriteValue = (float)e.TargetOption.Data;
            }
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 处理写入错误
        /// </summary>
        private void HandleWriteError(OperateResult result)
        {
            Console.WriteLine($"地址[{WriteAddress}]写入失败: {result.Message}");
        }



        #endregion
    }
}