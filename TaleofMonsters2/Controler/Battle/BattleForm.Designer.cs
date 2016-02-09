﻿using NarlonLib.Control;
using TaleofMonsters.Controler.Battle.Components;

namespace TaleofMonsters.Controler.Battle
{
    sealed partial class BattleForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bitmapButtonClose = new NarlonLib.Control.BitmapButton();
            this.panelState = new System.Windows.Forms.Panel();
            this.panelBattle = new NarlonLib.Control.DoubleBuffedPanel();
            this.cardSelector1 = new TaleofMonsters.Controler.Battle.Components.CardSelector();
            this.miniItemView1 = new TaleofMonsters.Forms.Items.MiniItemView();
            this.cardList2 = new TaleofMonsters.Controler.Battle.Components.CardList();
            this.cardsArray1 = new TaleofMonsters.Controler.Battle.Components.CardsArray();
            this.lifeClock2 = new TaleofMonsters.Controler.Battle.Components.LifeClock();
            this.lifeClock1 = new TaleofMonsters.Controler.Battle.Components.LifeClock();
            this.timeViewer1 = new TimeViewer();
            this.panelBattle.SuspendLayout();
            this.SuspendLayout();
            // 
            // bitmapButtonClose
            // 
            this.bitmapButtonClose.BorderColor = System.Drawing.Color.DarkBlue;
            this.bitmapButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bitmapButtonClose.Image = null;
            this.bitmapButtonClose.ImageNormal = null;
            this.bitmapButtonClose.Location = new System.Drawing.Point(891, 4);
            this.bitmapButtonClose.Name = "bitmapButtonClose";
            this.bitmapButtonClose.Size = new System.Drawing.Size(24, 24);
            this.bitmapButtonClose.TabIndex = 25;
            this.bitmapButtonClose.Text = "";
            this.bitmapButtonClose.UseVisualStyleBackColor = true;
            this.bitmapButtonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // panelState
            // 
            this.panelState.BackColor = System.Drawing.Color.Black;
            this.panelState.Location = new System.Drawing.Point(72, 656);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(754, 20);
            this.panelState.TabIndex = 37;
            this.panelState.Paint += new System.Windows.Forms.PaintEventHandler(this.panelState_Paint);
            // 
            // panelBattle
            // 
            this.panelBattle.BackColor = System.Drawing.Color.Black;
            this.panelBattle.Controls.Add(this.cardSelector1);
            this.panelBattle.Location = new System.Drawing.Point(15, 110);
            this.panelBattle.Name = "panelBattle";
            this.panelBattle.Size = new System.Drawing.Size(900, 400);
            this.panelBattle.TabIndex = 34;
            this.panelBattle.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBattle_Paint);
            this.panelBattle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelBattle_MouseClick);
            this.panelBattle.MouseEnter += new System.EventHandler(this.panelBattle_MouseEnter);
            this.panelBattle.MouseLeave += new System.EventHandler(this.panelBattle_MouseLeave);
            this.panelBattle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelBattle_MouseMove);
            // 
            // cardSelector1
            // 
            this.cardSelector1.BackColor = System.Drawing.Color.Black;
            this.cardSelector1.Location = new System.Drawing.Point(157, 92);
            this.cardSelector1.Name = "cardSelector1";
            this.cardSelector1.Size = new System.Drawing.Size(613, 220);
            this.cardSelector1.TabIndex = 0;
            // 
            // miniItemView1
            // 
            this.miniItemView1.BackColor = System.Drawing.Color.Black;
            this.miniItemView1.Enabled = false;
            this.miniItemView1.ItemType = 11;
            this.miniItemView1.Location = new System.Drawing.Point(832, 519);
            this.miniItemView1.Name = "miniItemView1";
            this.miniItemView1.Size = new System.Drawing.Size(72, 135);
            this.miniItemView1.TabIndex = 38;
            // 
            // cardList2
            // 
            this.cardList2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cardList2.Location = new System.Drawing.Point(578, 74);
            this.cardList2.Marginp = 3;
            this.cardList2.MaxCards = 10;
            this.cardList2.Name = "cardList2";
            this.cardList2.Size = new System.Drawing.Size(262, 35);
            this.cardList2.TabIndex = 36;
            this.cardList2.Visible = false;
            // 
            // cardsArray1
            // 
            this.cardsArray1.BackColor = System.Drawing.Color.DimGray;
            this.cardsArray1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardsArray1.Location = new System.Drawing.Point(72, 518);
            this.cardsArray1.Name = "cardsArray1";
            this.cardsArray1.Size = new System.Drawing.Size(754, 137);
            this.cardsArray1.TabIndex = 33;
            this.cardsArray1.SelectionChange += new TaleofMonsters.Controler.Battle.Components.CardsArray.CardArrayEventHandler(this.cardsArray1_SelectionChange);
            // 
            // lifeClock2
            // 
            this.lifeClock2.Location = new System.Drawing.Point(534, 35);
            this.lifeClock2.Name = "lifeClock2";
            this.lifeClock2.Size = new System.Drawing.Size(380, 70);
            this.lifeClock2.TabIndex = 32;
            this.lifeClock2.MouseLeave += new System.EventHandler(this.lifeClock1_MouseLeave);
            this.lifeClock2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lifeClock2_MouseMove);
            // 
            // lifeClock1
            // 
            this.lifeClock1.Location = new System.Drawing.Point(15, 35);
            this.lifeClock1.Name = "lifeClock1";
            this.lifeClock1.Size = new System.Drawing.Size(380, 70);
            this.lifeClock1.TabIndex = 31;
            this.lifeClock1.MouseLeave += new System.EventHandler(this.lifeClock1_MouseLeave);
            this.lifeClock1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lifeClock1_MouseMove);
            // 
            // timeViewer1
            // 
            this.timeViewer1.BackColor = System.Drawing.Color.Black;
            this.timeViewer1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.timeViewer1.Location = new System.Drawing.Point(407, 38);
            this.timeViewer1.Name = "timeViewer1";
            this.timeViewer1.Size = new System.Drawing.Size(117, 66);
            this.timeViewer1.TabIndex = 35;
            this.timeViewer1.Click += new System.EventHandler(this.timeViewer1_Click);
            // 
            // BattleForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.miniItemView1);
            this.Controls.Add(this.panelState);
            this.Controls.Add(this.cardList2);
            this.Controls.Add(this.timeViewer1);
            this.Controls.Add(this.cardsArray1);
            this.Controls.Add(this.lifeClock2);
            this.Controls.Add(this.lifeClock1);
            this.Controls.Add(this.bitmapButtonClose);
            this.Controls.Add(this.panelBattle);
            this.DoubleBuffered = true;
            this.Name = "BattleForm";
            this.Size = new System.Drawing.Size(933, 705);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BattleForm_Paint);
            this.panelBattle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BitmapButton bitmapButtonClose;
        private TaleofMonsters.Forms.Items.MiniItemView miniItemView1;
        private System.Windows.Forms.Panel panelState;
        private TaleofMonsters.Controler.Battle.Components.CardList cardList2;
        private TaleofMonsters.Controler.Battle.Components.TimeViewer timeViewer1;
        private DoubleBuffedPanel panelBattle;
        private TaleofMonsters.Controler.Battle.Components.CardsArray cardsArray1;
        private TaleofMonsters.Controler.Battle.Components.LifeClock lifeClock2;
        private TaleofMonsters.Controler.Battle.Components.LifeClock lifeClock1;
        private CardSelector cardSelector1;
    }
}