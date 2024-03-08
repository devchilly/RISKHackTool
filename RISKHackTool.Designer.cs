using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RISKHackTool
{
    partial class RISKHack
    {

        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            territoriesPanel = new Panel();
            loadCsvButton = new Button();
            playersPanel = new Panel();
            playerSetTroopsToolTip = new ToolTip(components);
            territorySetTroopsToolTip = new ToolTip(components);
            refreshButton = new Button();
            toggleFogButton = new Button();
            toggleFogToolTip = new ToolTip(components);
            SuspendLayout();
            // 
            // territoriesPanel
            // 
            territoriesPanel.BackColor = Color.White;
            territoriesPanel.BorderStyle = BorderStyle.FixedSingle;
            territoriesPanel.Location = new Point(12, 238);
            territoriesPanel.Name = "territoriesPanel";
            territoriesPanel.Size = new Size(313, 361);
            territoriesPanel.TabIndex = 3;
            // 
            // loadCsvButton
            // 
            loadCsvButton.Location = new Point(15, 176);
            loadCsvButton.Name = "loadCsvButton";
            loadCsvButton.Size = new Size(75, 23);
            loadCsvButton.TabIndex = 4;
            loadCsvButton.Text = "Load CSV";
            loadCsvButton.UseVisualStyleBackColor = true;
            loadCsvButton.Click += loadCsvButton_Click;
            // 
            // playersPanel
            // 
            playersPanel.BackColor = Color.White;
            playersPanel.BorderStyle = BorderStyle.FixedSingle;
            playersPanel.Location = new Point(15, 12);
            playersPanel.Name = "playersPanel";
            playersPanel.Size = new Size(313, 138);
            playersPanel.TabIndex = 4;
            // 
            // refreshButton
            // 
            refreshButton.FlatStyle = FlatStyle.System;
            refreshButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            refreshButton.Location = new Point(260, 205);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(65, 23);
            refreshButton.TabIndex = 5;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            // 
            // toggleFogButton
            // 
            toggleFogButton.FlatStyle = FlatStyle.System;
            toggleFogButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            toggleFogButton.Location = new Point(15, 205);
            toggleFogButton.Name = "toggleFogButton";
            toggleFogButton.Size = new Size(75, 23);
            toggleFogButton.TabIndex = 6;
            toggleFogButton.Text = "Toggle Fog";
            toggleFogToolTip.SetToolTip(toggleFogButton, "Toggle fog for yourself only. This will not affect other players!");
            toggleFogButton.UseVisualStyleBackColor = true;
            toggleFogButton.Click += toggleFogButton_Click;
            // 
            // RISKHack
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(340, 621);
            Controls.Add(toggleFogButton);
            Controls.Add(refreshButton);
            Controls.Add(playersPanel);
            Controls.Add(loadCsvButton);
            Controls.Add(territoriesPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "RISKHack";
            Text = "RISK Hack ";
            ResumeLayout(false);
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Panel territoriesPanel;
        private Button loadCsvButton;
        private Panel playersPanel;
        private ToolTip playerSetTroopsToolTip;
        private ToolTip territorySetTroopsToolTip;
        private Button refreshButton;
        private Button toggleFogButton;
        private ToolTip toggleFogToolTip;
    }
}