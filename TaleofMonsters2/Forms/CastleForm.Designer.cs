﻿using ControlPlus;

namespace TaleofMonsters.Forms
{
    partial class CastleForm
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
            this.bitmapButtonClose = new ControlPlus.BitmapButton();
            this.bitmapButtonBuild = new ControlPlus.BitmapButton();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.IconImage = null;
            this.bitmapButtonClose.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonClose.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(708, 17);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.NoUseDrawNine = false;
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 39;
            this.bitmapButtonClose.TextOffX = 0;
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.bitmapButtonClose_Click);
            // 
            // bitmapButtonBuild
            // 
            this.bitmapButtonBuild.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonBuild.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonBuild.IconImage = null;
            this.bitmapButtonBuild.IconSize = new System.Drawing.Size(0, 0);
            this.bitmapButtonBuild.IconXY = new System.Drawing.Point(0, 0);
            this.bitmapButtonBuild.ImageNormal = null;
            this.bitmapButtonBuild.Location = new System.Drawing.Point(20, 349);
            this.bitmapButtonBuild.Name = "bitmapButtonBuild";
            this.bitmapButtonBuild.NoUseDrawNine = false;
            this.bitmapButtonBuild.Size = new System.Drawing.Size(40, 40);
            this.bitmapButtonBuild.TabIndex = 40;
            this.bitmapButtonBuild.TextOffX = 0;
            this.bitmapButtonBuild.UseVisualStyleBackColor = true;
            this.bitmapButtonBuild.Click += new System.EventHandler(this.bitmapButtonBuild_Click);
            // 
            // CastleForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.bitmapButtonBuild);
            this.Controls.Add(this.bitmapButtonClose);
            this.Name = "CastleForm";
            this.Size = new System.Drawing.Size(753, 485);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CastleForm_Paint);
            this.ResumeLayout(false);

        }

        #endregion
        private BitmapButton bitmapButtonClose;
        private BitmapButton bitmapButtonBuild;
    }
}