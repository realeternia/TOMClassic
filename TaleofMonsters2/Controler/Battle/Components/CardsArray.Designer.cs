﻿using TaleofMonsters.DataType.Cards;

namespace TaleofMonsters.Controler.Battle.Components
{
    partial class CardsArray
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CardsArray
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Name = "CardsArray";
            this.Size = new System.Drawing.Size(754, 150);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CardsArray_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CardsArray_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CardsArray_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

    }
}