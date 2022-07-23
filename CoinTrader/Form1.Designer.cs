
namespace CoinTrader
{
    partial class Form1
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.logTabPage = new MetroFramework.Controls.MetroTabPage();
            this.listView1 = new MetroFramework.Controls.MetroListView();
            this.Log = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.optionTabPage = new MetroFramework.Controls.MetroTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.metroTabControl1.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.logTabPage.SuspendLayout();
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
            this.metroTabControl1.Controls.Add(this.logTabPage);
            this.metroTabControl1.Controls.Add(this.optionTabPage);
            this.metroTabControl1.Location = new System.Drawing.Point(1, 71);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(1197, 528);
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
            this.mainTabPage.Size = new System.Drawing.Size(1189, 486);
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
            this.columnHeader7});
            this.metroListView1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.metroListView1.FullRowSelect = true;
            this.metroListView1.GridLines = true;
            this.metroListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.metroListView1.HideSelection = false;
            this.metroListView1.Location = new System.Drawing.Point(0, 0);
            this.metroListView1.Name = "metroListView1";
            this.metroListView1.Size = new System.Drawing.Size(1189, 486);
            this.metroListView1.TabIndex = 2;
            this.metroListView1.UseCompatibleStateImageBehavior = false;
            this.metroListView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "제공 시장";
            this.columnHeader1.Width = 200;
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
            this.columnHeader5.Text = "목표 매수 단가";
            this.columnHeader5.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "목표 매도 단가";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "금일 예상 종가";
            this.columnHeader7.Width = 150;
            // 
            // logTabPage
            // 
            this.logTabPage.Controls.Add(this.listView1);
            this.logTabPage.HorizontalScrollbarBarColor = true;
            this.logTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.logTabPage.HorizontalScrollbarSize = 10;
            this.logTabPage.Location = new System.Drawing.Point(4, 38);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Size = new System.Drawing.Size(1189, 486);
            this.logTabPage.TabIndex = 1;
            this.logTabPage.Text = "로그";
            this.logTabPage.VerticalScrollbarBarColor = true;
            this.logTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.logTabPage.VerticalScrollbarSize = 10;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Log});
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listView1.Location = new System.Drawing.Point(0, 3);
            this.listView1.Margin = new System.Windows.Forms.Padding(1);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(1188, 483);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.UseSelectable = true;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Log
            // 
            this.Log.Text = "";
            this.Log.Width = 768;
            // 
            // optionTabPage
            // 
            this.optionTabPage.HorizontalScrollbarBarColor = true;
            this.optionTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.optionTabPage.HorizontalScrollbarSize = 10;
            this.optionTabPage.Location = new System.Drawing.Point(4, 38);
            this.optionTabPage.Name = "optionTabPage";
            this.optionTabPage.Size = new System.Drawing.Size(1189, 486);
            this.optionTabPage.TabIndex = 2;
            this.optionTabPage.Text = "옵션";
            this.optionTabPage.VerticalScrollbarBarColor = true;
            this.optionTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.optionTabPage.VerticalScrollbarSize = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Controls.Add(this.metroTabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Resizable = false;
            this.Text = "Coin Trader";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.metroTabControl1.ResumeLayout(false);
            this.mainTabPage.ResumeLayout(false);
            this.logTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage mainTabPage;
        private MetroFramework.Controls.MetroListView listView1;
        internal System.Windows.Forms.ColumnHeader Log;
        private MetroFramework.Controls.MetroTabPage logTabPage;
        private MetroFramework.Controls.MetroTabPage optionTabPage;
        private System.Windows.Forms.ListView metroListView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
    }
}

