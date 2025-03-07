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
    /// S7节点基类，提供PLC通信和状态管理的基础功能
    /// </summary>
    public abstract class S7BaseNode : STNode
    {
        #region 字段

        /// <summary>
        /// PLC通信实例
        /// </summary>
        protected SiemensS7Net plc;

        /// <summary>
        /// 连接状态
        /// </summary>
        protected bool isConnected;

        /// <summary>
        /// 连接状态检查间隔(毫秒)
        /// </summary>
        protected const int CONNECTION_CHECK_INTERVAL = 1000;

        /// <summary>
        /// 机架号
        /// </summary>
        protected byte rackNumber;

        /// <summary>
        /// 插槽号
        /// </summary>
        protected byte slotNumber;

        /// <summary>
        /// PLC句柄输入选项
        /// </summary>
        protected STNodeOption plcHandleOption;

        #endregion

        #region 初始化

        /// <summary>
        /// 节点创建时的初始化
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        #endregion

        #region PLC通信

        /// <summary>
        /// 初始化PLC连接
        /// </summary>
        protected virtual void InitializePLC(string ipAddress, int port)
        {
            try
            {
                plc = new SiemensS7Net(SiemensPLCS.S1200, ipAddress)
                {
                    Port = port,
                    Rack = rackNumber,
                    Slot = slotNumber
                };
            }
            catch (Exception ex)
            {
                HandleException("PLC初始化失败", ex);
            }
        }

        /// <summary>
        /// 启动连接状态监视器
        /// </summary>
        protected virtual void StartConnectionMonitor()
        {
            var thread = new Thread(MonitorConnection)
            {
                IsBackground = true
            };
            thread.Start();
        }

        /// <summary>
        /// 监视PLC连接状态
        /// </summary>
        protected virtual void MonitorConnection()
        {
            while (true)
            {
                try
                {
                    if (plc != null)
                    {
                        var result = plc.ConnectServer();
                        UpdateConnectionStatus(result.IsSuccess);
                    }
                    Thread.Sleep(CONNECTION_CHECK_INTERVAL);
                }
                catch (Exception ex)
                {
                    HandleException("连接监视异常", ex);
                    Thread.Sleep(CONNECTION_CHECK_INTERVAL);
                }
            }
        }

        /// <summary>
        /// 断开PLC连接
        /// </summary>
        protected virtual void DisconnectPLC()
        {
            try
            {
                if (plc != null)
                {
                    plc.ConnectClose();
                    UpdateConnectionStatus(false);
                }
            }
            catch (Exception ex)
            {
                HandleException("断开连接失败", ex);
            }
        }

        /// <summary>
        /// 更新连接状态和界面显示
        /// </summary>
        protected virtual void UpdateConnectionStatus(bool connected)
        {
            isConnected = connected;
            this.BeginInvoke(new MethodInvoker(() =>
            {
                TitleColor = connected ? Color.FromArgb(50, 150, 50) : Color.FromArgb(200, 50, 50);
                Invalidate();
            }));
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 处理PLC数据传输事件
        /// </summary>
        protected virtual void HandlePlcDataTransfer(object sender, STNodeOptionEventArgs e)
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

        #endregion

        #region 异常处理

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