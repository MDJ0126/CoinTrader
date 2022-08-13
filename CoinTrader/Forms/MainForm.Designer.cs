
namespace CoinTrader.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.mainTabPage = new MetroFramework.Controls.MetroTabPage();
            this.metroListView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.optionTabPage = new MetroFramework.Controls.MetroTabPage();
            this.accountTab = new MetroFramework.Controls.MetroTabPage();
            this.currentPrice = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.metroTabControl1.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.accountTab.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = null;
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroTabControl1.Controls.Add(this.mainTabPage);
            this.metroTabControl1.Controls.Add(this.optionTabPage);
            this.metroTabControl1.Controls.Add(this.accountTab);
            this.metroTabControl1.Location = new System.Drawing.Point(1, 71);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(1197, 509);
            this.metroTabControl1.TabIndex = 2;
            this.metroTabControl1.UseSelectable = true;
            // 
            // mainTabPage
            // 
            this.mainTabPage.Controls.Add(this.metroListView1);
            this.mainTabPage.HorizontalScrollbarBarColor = true;
            this.mainTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mainTabPage.HorizontalScrollbarSize = 10;
            this.mainTabPage.Location = new System.Drawing.Point(4, 38);
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.Size = new System.Drawing.Size(1189, 467);
            this.mainTabPage.TabIndex = 0;
            this.mainTabPage.Text = "거래소";
            this.mainTabPage.VerticalScrollbarBarColor = true;
            this.mainTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.mainTabPage.VerticalScrollbarSize = 10;
            // 
            // metroListView1
            // 
            this.metroListView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.metroListView1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.metroListView1.FullRowSelect = true;
            this.metroListView1.GridLines = true;
            this.metroListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.metroListView1.HideSelection = false;
            this.metroListView1.Location = new System.Drawing.Point(0, 0);
            this.metroListView1.Name = "metroListView1";
            this.metroListView1.Size = new System.Drawing.Size(1188, 486);
            this.metroListView1.TabIndex = 2;
            this.metroListView1.UseCompatibleStateImageBehavior = false;
            this.metroListView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "제공 시장";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "종목 이름";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "현재가";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "전일대비";
            this.columnHeader4.Width = 150;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "이동평균 15일";
            this.columnHeader5.Width = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "이동평균 30일";
            this.columnHeader6.Width = 150;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "이번 시간 예상 종가(딥러닝)";
            this.columnHeader7.Width = 150;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "변동성 돌파";
            this.columnHeader8.Width = 150;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "골든크로스";
            this.columnHeader9.Width = 100;
            // 
            // optionTabPage
            // 
            this.optionTabPage.HorizontalScrollbarBarColor = true;
            this.optionTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.optionTabPage.HorizontalScrollbarSize = 10;
            this.optionTabPage.Location = new System.Drawing.Point(4, 38);
            this.optionTabPage.Name = "optionTabPage";
            this.optionTabPage.Size = new System.Drawing.Size(1189, 467);
            this.optionTabPage.TabIndex = 2;
            this.optionTabPage.Text = "옵션";
            this.optionTabPage.VerticalScrollbarBarColor = true;
            this.optionTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.optionTabPage.VerticalScrollbarSize = 10;
            // 
            // accountTab
            // 
            this.accountTab.Controls.Add(this.currentPrice);
            this.accountTab.Controls.Add(this.label1);
            this.accountTab.HorizontalScrollbarBarColor = true;
            this.accountTab.HorizontalScrollbarHighlightOnWheel = false;
            this.accountTab.HorizontalScrollbarSize = 10;
            this.accountTab.Location = new System.Drawing.Point(4, 38);
            this.accountTab.Name = "accountTab";
            this.accountTab.Size = new System.Drawing.Size(1189, 467);
            this.accountTab.TabIndex = 3;
            this.accountTab.Text = "잔고";
            this.accountTab.VerticalScrollbarBarColor = true;
            this.accountTab.VerticalScrollbarHighlightOnWheel = false;
            this.accountTab.VerticalScrollbarSize = 10;
            // 
            // currentPrice
            // 
            this.currentPrice.BackColor = System.Drawing.Color.White;
            this.currentPrice.Location = new System.Drawing.Point(175, 31);
            this.currentPrice.Name = "currentPrice";
            this.currentPrice.Size = new System.Drawing.Size(150, 25);
            this.currentPrice.TabIndex = 2;
            this.currentPrice.Text = "999999999";
            this.currentPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(19, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "현재 자산 총평가";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.Location = new System.Drawing.Point(23, 53);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(99, 15);
            this.timeLabel.TabIndex = 3;
            this.timeLabel.Text = "1999년12월31일";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(1, 580);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1200, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(139, 17);
            this.toolStripStatusLabel1.Text = "로그 내용이 들어가는 곳";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.metroTabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Resizable = false;
            this.Text = "Coin Trader";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.metroTabControl1.ResumeLayout(false);
            this.mainTabPage.ResumeLayout(false);
            this.accountTab.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage mainTabPage;
        private MetroFramework.Controls.MetroTabPage optionTabPage;
        private System.Windows.Forms.ListView metroListView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private MetroFramework.Controls.MetroTabPage accountTab;
        private System.Windows.Forms.Label currentPrice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

