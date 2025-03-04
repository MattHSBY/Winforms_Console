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
            Panel.SuspendLayout();
            SuspendLayout();
            // 
            // consoleBox
            // 
            consoleBox.Dock = DockStyle.Fill;
            consoleBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            consoleBox.Location = new Point(0, 0);
            consoleBox.Name = "consoleBox";
            consoleBox.ReadOnly = true;
            consoleBox.Size = new Size(776, 399);
            consoleBox.TabIndex = 0;
            consoleBox.Text = "";
            // 
            // commandLine
            // 
            commandLine.Dock = DockStyle.Bottom;
            commandLine.Location = new Point(0, 399);
            commandLine.Name = "commandLine";
            commandLine.Size = new Size(776, 27);
            commandLine.TabIndex = 1;
            // 
            // Panel
            // 
            Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel.Controls.Add(consoleBox);
            Panel.Controls.Add(commandLine);
            Panel.Location = new Point(12, 12);
            Panel.Name = "Panel";
            Panel.Size = new Size(776, 426);
            Panel.TabIndex = 2;
            // 
            // ConsoleForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Panel);
            Name = "ConsoleForm";
            Text = "Form1";
            Panel.ResumeLayout(false);
            Panel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ConsoleBox consoleBox;
        private CommandLine commandLine;
        private Panel Panel;
    }
}