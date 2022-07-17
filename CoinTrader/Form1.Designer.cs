
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
            this.logTabPage = new MetroFramework.Controls.MetroTabPage();
            this.listView1 = new MetroFramework.Controls.MetroListView();
            this.Log = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.optionTabPage = new MetroFramework.Controls.MetroTabPage();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.metroTabControl1.SuspendLayout();
            this.logTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = null;
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.mainTabPage);
            this.metroTabControl1.Controls.Add(this.logTabPage);
            this.metroTabControl1.Controls.Add(this.optionTabPage);
            this.metroTabControl1.Location = new System.Drawing.Point(1, 71);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(798, 378);
            this.metroTabControl1.TabIndex = 2;
            this.metroTabControl1.UseSelectable = true;
            // 
            // mainTabPage
            // 
            this.mainTabPage.HorizontalScrollbarBarColor = true;
            this.mainTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.mainTabPage.HorizontalScrollbarSize = 10;
            this.mainTabPage.Location = new System.Drawing.Point(4, 38);
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.Size = new System.Drawing.Size(790, 336);
            this.mainTabPage.TabIndex = 0;
            this.mainTabPage.Text = "메인";
            this.mainTabPage.VerticalScrollbarBarColor = true;
            this.mainTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.mainTabPage.VerticalScrollbarSize = 10;
            // 
            // logTabPage
            // 
            this.logTabPage.Controls.Add(this.listView1);
            this.logTabPage.HorizontalScrollbarBarColor = true;
            this.logTabPage.HorizontalScrollbarHighlightOnWheel = false;
            this.logTabPage.HorizontalScrollbarSize = 10;
            this.logTabPage.Location = new System.Drawing.Point(4, 38);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Size = new System.Drawing.Size(790, 336);
            this.logTabPage.TabIndex = 1;
            this.logTabPage.Text = "로그";
            this.logTabPage.VerticalScrollbarBarColor = true;
            this.logTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.logTabPage.VerticalScrollbarSize = 10;
            // 
            // listView1
            // 
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
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(790, 333);
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
            this.optionTabPage.Size = new System.Drawing.Size(790, 336);
            this.optionTabPage.TabIndex = 2;
            this.optionTabPage.Text = "옵션";
            this.optionTabPage.VerticalScrollbarBarColor = true;
            this.optionTabPage.VerticalScrollbarHighlightOnWheel = false;
            this.optionTabPage.VerticalScrollbarSize = 10;
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(520, 36);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(139, 35);
            this.metroButton1.TabIndex = 3;
            this.metroButton1.Text = "metroButton1";
            this.metroButton1.UseSelectable = true;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.metroButton1);
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
        private MetroFramework.Controls.MetroButton metroButton1;
    }
}

