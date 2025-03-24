namespace Custom_Winforms_Library
{
    partial class ConsoleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            consoleBox = new ConsoleBox();
            commandLine = new CommandLine();
            Panel = new Panel();
            consoleBoxPanel = new Panel();
            commandPanel = new Panel();
            Panel.SuspendLayout();
            consoleBoxPanel.SuspendLayout();
            commandPanel.SuspendLayout();
            SuspendLayout();
            // 
            // consoleBox
            // 
            consoleBox.BackColor = Color.FromArgb(75, 75, 75);
            consoleBox.BorderStyle = BorderStyle.None;
            consoleBox.Dock = DockStyle.Fill;
            consoleBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            consoleBox.ForeColor = Color.White;
            consoleBox.Location = new Point(5, 5);
            consoleBox.MaxLength = 0;
            consoleBox.Name = "consoleBox";
            consoleBox.ReadOnly = true;
            consoleBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            consoleBox.Size = new Size(781, 478);
            consoleBox.TabIndex = 0;
            consoleBox.Text = "";
            // 
            // commandLine
            // 
            commandLine.BackColor = Color.FromArgb(75, 75, 75);
            commandLine.BorderStyle = BorderStyle.None;
            commandLine.Dock = DockStyle.Fill;
            commandLine.ForeColor = Color.White;
            commandLine.Location = new Point(5, 5);
            commandLine.Name = "commandLine";
            commandLine.Size = new Size(781, 20);
            commandLine.TabIndex = 1;
            // 
            // Panel
            // 
            Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel.Controls.Add(consoleBoxPanel);
            Panel.Controls.Add(commandPanel);
            Panel.Location = new Point(12, 12);
            Panel.Name = "Panel";
            Panel.Size = new Size(791, 514);
            Panel.TabIndex = 2;
            // 
            // consoleBoxPanel
            // 
            consoleBoxPanel.BackColor = Color.FromArgb(60, 60, 60);
            consoleBoxPanel.Controls.Add(consoleBox);
            consoleBoxPanel.Dock = DockStyle.Fill;
            consoleBoxPanel.Location = new Point(0, 0);
            consoleBoxPanel.Name = "consoleBoxPanel";
            consoleBoxPanel.Padding = new Padding(5, 5, 5, 0);
            consoleBoxPanel.Size = new Size(791, 483);
            consoleBoxPanel.TabIndex = 4;
            // 
            // commandPanel
            // 
            commandPanel.BackColor = Color.FromArgb(60, 60, 60);
            commandPanel.Controls.Add(commandLine);
            commandPanel.Dock = DockStyle.Bottom;
            commandPanel.Location = new Point(0, 483);
            commandPanel.Name = "commandPanel";
            commandPanel.Padding = new Padding(5);
            commandPanel.Size = new Size(791, 31);
            commandPanel.TabIndex = 3;
            // 
            // ConsoleForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(75, 75, 75);
            ClientSize = new Size(815, 538);
            Controls.Add(Panel);
            Name = "ConsoleForm";
            Text = "Form1";
            Panel.ResumeLayout(false);
            consoleBoxPanel.ResumeLayout(false);
            commandPanel.ResumeLayout(false);
            commandPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ConsoleBox consoleBox;
        private CommandLine commandLine;
        private Panel Panel;
        private Panel commandPanel;
        private Panel consoleBoxPanel;
    }
}