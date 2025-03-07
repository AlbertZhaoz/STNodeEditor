using System;
using System.Drawing;
using ST.Library.UI.NodeEditor;

namespace WinNodeEditorDemo.S7Node
{
    /// <summary>
    /// S7数据显示节点，用于显示从PLC读取的数据
    /// </summary>
    [STNode("/S7Node/", "tuling", "wx:zhy_cxx", "https://tuling.online", "This node can show plc data")]
    public class S7ResultShow : S7BaseNode
    {
        #region 字段

        /// <summary>
        /// 数据输入选项
        /// </summary>
        private STNodeOption dataInputOption;

        /// <summary>
        /// 显示的数值
        /// </summary>
        private float displayValue;

        /// <summary>
        /// 字符串格式化对象
        /// </summary>
        private StringFormat stringFormat;

        /// <summary>
        /// 显示区域
        /// </summary>
        private Rectangle displayRect;

        /// <summary>
        /// 背景颜色
        /// </summary>
        private readonly Color backgroundColor = Color.FromArgb(40, 40, 40);

        /// <summary>
        /// 文本颜色
        /// </summary>
        private readonly Color textColor = Color.White;

        #endregion

        #region 属性

        private int fontSize = 12;
        /// <summary>
        /// 显示字体大小
        /// </summary>
        [STNodeProperty("字体大小", "显示文本的字体大小")]
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
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
            InitializeDisplayArea();
            InitializeInputOption();
        }

        /// <summary>
        /// 初始化节点属性
        /// </summary>
        private void InitializeNodeProperties()
        {
            Title = "PLC数据显示";
            TitleColor = Color.FromArgb(200, Color.DarkOrange);

            stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
        }

        /// <summary>
        /// 初始化显示区域
        /// </summary>
        private void InitializeDisplayArea()
        {
            displayRect = new Rectangle(10, 25, 100, 60);
        }

        /// <summary>
        /// 初始化输入选项
        /// </summary>
        private void InitializeInputOption()
        {
            dataInputOption = new STNodeOption("数据输入", typeof(float), true);
            InputOptions.Add(dataInputOption);
            dataInputOption.DataTransfer += HandleDataTransfer;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 处理数据传输事件
        /// </summary>
        private void HandleDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            try
            {
                if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                {
                    if (e.TargetOption.Data is float)
                    {
                        displayValue = (float)e.TargetOption.Data;
                        Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException("数据传输异常", ex);
            }
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 绘制节点主体
        /// </summary>
        protected override void OnDrawBody(DrawingTools dt)
        {
            base.OnDrawBody(dt);

            try
            {
                // 绘制显示背景
                dt.Graphics.FillRectangle(new SolidBrush(backgroundColor), displayRect);

                // 使用自定义字体大小绘制数值
                using (Font displayFont = new Font(Font.FontFamily, FontSize))
                {
                    dt.Graphics.DrawString(
                        displayValue.ToString("F2"),
                        displayFont,
                        new SolidBrush(textColor),
                        displayRect,
                        stringFormat
                    );
                }
            }
            catch (Exception ex)
            {
                HandleException("绘制异常", ex);
            }
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 处理异常信息
        /// </summary>
        private void HandleException(string message, Exception ex)
        {
            Console.WriteLine($"{message}: {ex.Message}");
        }

        #endregion
    }
}
