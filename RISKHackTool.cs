using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace RISKHackTool
{
    public partial class RISKHack : Form
    {
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        const int MAX_BUFFER_SIZE = 100;
        const int MAX_SCROLL_SIZE = 1000;

        Dictionary<String, IntPtr> territoryNamesToAdresses = new Dictionary<String, IntPtr>();
        IntPtr addrOfTerritoriesList = IntPtr.Zero;
        int sizeOfTerritoriesList = 0;

        Dictionary<String, IntPtr> playerColorsToAddresses = new Dictionary<String, IntPtr>(); // we will use this to set territory colors
        Dictionary<String, IntPtr> regionNamesToAddresses = new Dictionary<String, IntPtr>(); // for now not used but extra information

        byte[] buffer = new byte[MAX_BUFFER_SIZE];
        IntPtr bytesRead = IntPtr.Zero;

        Process riskProcess;

        public RISKHack()
        {
            InitializeComponent();

            territoryTextBoxes = new List<TextBox>();
            territoryButtons = new List<List<Button>>();

            GetRISKData();
            SetRISKDataInComponent();

            territoriesPanel.VerticalScroll.Enabled = true;
            territoriesPanel.VerticalScroll.Visible = true;
            territoriesPanel.VerticalScroll.Maximum = MAX_SCROLL_SIZE;

            playersPanel.VerticalScroll.Enabled = true;
            playersPanel.VerticalScroll.Visible = true;
            playersPanel.VerticalScroll.Maximum = MAX_SCROLL_SIZE;
        }

        void GetRISKData()
        {
            #region Intialization

            // get Risk.exe process
            Process[] riskProcs = Process.GetProcessesByName("Risk");
            if (riskProcs.Length != 1)
            {
                throw new Exception("Risk process not detected or more than 1 instance has been detected.");
            }
            riskProcess = riskProcs[0];

            // get GameAssembly.dll process
            ProcessModule? gameAssemblyDllProc = null;
            foreach (ProcessModule module in riskProcess.Modules)
            {
                if (module.FileName != null && module.FileName.Contains("GameAssembly.dll"))
                {
                    gameAssemblyDllProc = module;
                }
            }

            if (gameAssemblyDllProc == null)
            {
                throw new Exception("GameAssembly.dll not detected.");
            }

            // Get address of the territories list
            ReadProcessMemory(riskProcess.Handle, gameAssemblyDllProc.BaseAddress + 0x27DF308, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + 0xB8, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()), buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + 0x30, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            addrOfTerritoriesList = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

            // Get address of each territory, as well as the regions and players (at least by color for the player)
            ReadProcessMemory(riskProcess.Handle, addrOfTerritoriesList + TerritoriesListOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead);
            sizeOfTerritoriesList = BitConverter.ToInt32(buffer);

            for (int i = 0; i < sizeOfTerritoriesList; ++i)
            {
                // get territory names and addresses
                buffer = new byte[MAX_BUFFER_SIZE];
                ReadProcessMemory(riskProcess.Handle, addrOfTerritoriesList + i * MemoryConstants.POINTER_BYTES + TerritoriesListOffsets.FIRST_TERRITORY_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr territoryAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcess.Handle, territoryAddress + TerritoryOffsets.TERRITORY_INFO_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + TerritoryInfoOffsets.NAME_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr territoryNameAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + StringOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead);
                ReadProcessMemory(riskProcess.Handle, territoryNameAddress + StringOffsets.FIRST_CHAR_OFFSET, buffer, BitConverter.ToInt32(buffer) * 2, out bytesRead); // memory is stored as ascii and not unicode
                String territoryName = System.Text.Encoding.ASCII.GetString(buffer.Where(x => x != 0x00).ToArray()).ToLower();

                territoryNamesToAdresses.Add(territoryName, territoryAddress);

                // check if we have seen this player before and add to list if needed
                buffer = new byte[MAX_BUFFER_SIZE];
                ReadProcessMemory(riskProcess.Handle, territoryAddress + TerritoryOffsets.PLAYER_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr playerAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + PlayerOffsets.COLOR_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr colorAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcess.Handle, colorAddress + StringOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead);
                ReadProcessMemory(riskProcess.Handle, colorAddress + StringOffsets.FIRST_CHAR_OFFSET, buffer, BitConverter.ToInt32(buffer) * 2, out bytesRead); // memory is stored as ascii and not unicode
                String color = System.Text.Encoding.ASCII.GetString(buffer.Where(x => x != 0x00).ToArray()).ToLower();

                if (!playerColorsToAddresses.ContainsKey(color.Replace("color_", "")))
                {
                    playerColorsToAddresses.Add(color.Replace("color_", ""), playerAddress);
                }

                // check if we have seen this region before and add to list if needed
                buffer = new byte[MAX_BUFFER_SIZE];
                ReadProcessMemory(riskProcess.Handle, territoryAddress + TerritoryOffsets.REGION_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr regionAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + RegionOffsets.REGION_INFO_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                ReadProcessMemory(riskProcess.Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + RegionInfoOffsets.NAME_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr regionNameAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcess.Handle, regionNameAddress + StringOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead); // memory is stored as ascii and not unicode
                ReadProcessMemory(riskProcess.Handle, regionNameAddress + StringOffsets.FIRST_CHAR_OFFSET, buffer, BitConverter.ToInt32(buffer) * 2, out bytesRead); // memory is stored as ascii and not unicode
                String regionName = System.Text.Encoding.ASCII.GetString(buffer.Where(x => x != 0x00).ToArray()).ToLower();

                if (!regionNamesToAddresses.ContainsKey(regionName))
                {
                    regionNamesToAddresses.Add(regionName, regionAddress);
                }
            }

            #endregion
        }

        void SetRISKDataInComponent()
        {
            territoriesPanel.SuspendLayout();
            playersPanel.SuspendLayout();
            List<String> territoryNames = territoryNamesToAdresses.Keys.ToList();
            for (int i = 0; i < territoryNamesToAdresses.Count(); ++i)
            {
                AddTerritoryInTool(territoryNames[i], i);
            }

            List<String> playerColors = playerColorsToAddresses.Keys.ToList();
            for (int i = 0; i < playerColorsToAddresses.Count(); ++i)
            {
                AddPlayerInTool(playerColors[i], i);
            }
        }

        private void AddTerritoryInTool(string territoryName, int index)
        {

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RISKHack));
            TextBox terrtoryTextBox = new TextBox();
            Button setTroopsButton = new Button();
            Button setCapitalButton = new Button();
            Button setBlizzardButton = new Button();

            terrtoryTextBox.BackColor = SystemColors.Control;
            terrtoryTextBox.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            terrtoryTextBox.Location = new Point(10, 15 + 30 * index);
            terrtoryTextBox.Name = territoryName + "_TextBox";
            terrtoryTextBox.Size = new Size(134, 22);
            terrtoryTextBox.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(territoryName);
            terrtoryTextBox.ReadOnly = true;
            terrtoryTextBox.TabStop = false;

            setTroopsButton.BackgroundImage = Properties.Resources.TroopsImage;
            setTroopsButton.BackgroundImageLayout = ImageLayout.Stretch;
            setTroopsButton.Location = new Point(172, 15 + 30 * index);
            setTroopsButton.Name = territoryName + "_Troops_Button";
            setTroopsButton.Size = new Size(32, 23);
            setTroopsButton.UseVisualStyleBackColor = true;
            setTroopsButton.Click += SetTroopsButton_Click;

            setCapitalButton.BackgroundImage = Properties.Resources.CapitalImage;
            setCapitalButton.BackgroundImageLayout = ImageLayout.Stretch;
            setCapitalButton.Location = new Point(210, 15 + 30 * index);
            setCapitalButton.Name = territoryName + "_Capital_Button";
            setCapitalButton.Size = new Size(32, 23);
            setCapitalButton.UseVisualStyleBackColor = true;
            setCapitalButton.Click += SetCapitalButton_Click;

            setBlizzardButton.BackgroundImage = Properties.Resources.BlizzardImage;
            setBlizzardButton.BackgroundImageLayout = ImageLayout.Stretch;
            setBlizzardButton.Location = new Point(249, 15 + 30 * index);
            setBlizzardButton.Name = territoryName + "_Blizzard_Button";
            setBlizzardButton.Size = new Size(32, 23);
            setBlizzardButton.UseVisualStyleBackColor = true;
            setBlizzardButton.Click += SetBlizzardButton_Click;

            territoriesPanel.Controls.Add(terrtoryTextBox);
            territoriesPanel.Controls.Add(setTroopsButton);
            territoriesPanel.Controls.Add(setCapitalButton);
            territoriesPanel.Controls.Add(setBlizzardButton);

            territoryTextBoxes.Add(terrtoryTextBox);
            territoryButtons.Add(new List<Button>() { setTroopsButton, setCapitalButton, setBlizzardButton });
        }

        private void AddPlayerInTool(string playerColor, int index)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RISKHack));
            TextBox playerTextBox = new TextBox();
            Button setTroopsButton = new Button();

            playerTextBox.BackColor = SystemColors.Control;
            playerTextBox.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            playerTextBox.Location = new Point(10, 15 + 30 * index);
            playerTextBox.Name = playerColor + "_TextBox";
            playerTextBox.Size = new Size(134, 22);
            playerTextBox.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(playerColor);
            playerTextBox.ReadOnly = true;
            playerTextBox.TabStop = false;

            setTroopsButton.BackgroundImage = Properties.Resources.TroopsImage;
            setTroopsButton.BackgroundImageLayout = ImageLayout.Stretch;
            setTroopsButton.Location = new Point(172, 15 + 30 * index);
            setTroopsButton.Name = playerColor + "_Troops_Button";
            setTroopsButton.Size = new Size(32, 23);
            setTroopsButton.UseVisualStyleBackColor = true;
            setTroopsButton.Click += SetPlayerTroopsButton_Click;

            playersPanel.Controls.Add(playerTextBox);
            playersPanel.Controls.Add(setTroopsButton);
        }

        private void SetCapitalButton_Click(object sender, EventArgs e)
        {
            var territoryName = ((Button)sender).Name.Split("_")[0];
            SetCapital(territoryName);
        }

        private void SetCapital(String territoryName)
        {
            IntPtr territoryPtr = territoryNamesToAdresses[territoryName];
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.TERRITORY_TYPE_OFFSET,
                BitConverter.GetBytes((int)TerritoryType.Capital), MemoryConstants.INT_BYTES, out bytesRead);
        }

        private void SetBlizzardButton_Click(object sender, EventArgs e)
        {
            var territoryName = ((Button)sender).Name.Split("_")[0];
            SetBlizzard(territoryName);
        }

        private void SetBlizzard(String territoryName)
        {
            IntPtr territoryPtr = territoryNamesToAdresses[territoryName];
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.ENCRYPTED_UNITS_OFFSET,
                BitConverter.GetBytes(IntPtr.Zero.ToInt64()), MemoryConstants.POINTER_BYTES, out bytesRead);
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.PLAYER_OFFSET,
                BitConverter.GetBytes(IntPtr.Zero.ToInt64()), MemoryConstants.POINTER_BYTES, out bytesRead);
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.TERRITORY_TYPE_OFFSET,
                BitConverter.GetBytes((int)TerritoryType.Blizzard), MemoryConstants.INT_BYTES, out bytesRead);
        }

        private void SetTroopsButton_Click(object sender, EventArgs e)
        {
            var territoryName = ((Button)sender).Name.Split("_")[0];
            var troopCount = int.Parse(Microsoft.VisualBasic.Interaction.InputBox(
                "How many troops would you like on " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(territoryName) + "?",
                "Set Troops",
                ""));
            SetTroops(territoryName, troopCount);
        }

        private void SetPlayerTroopsButton_Click(object sender, EventArgs e)
        {
            var playerColor = ((Button)sender).Name.Split("_")[0];
            var troopCount = int.Parse(Microsoft.VisualBasic.Interaction.InputBox(
                "How many troops would you like for the " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(playerColor) + " player?",
                "Set Troops",
                ""));
            SetPlayerTroops(playerColor, troopCount);
        }

        private void SetTroops(String territoryName, int troopCount)
        {
            IntPtr territoryPtr = territoryNamesToAdresses[territoryName];
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.ENCRYPTED_UNITS_OFFSET,
                BitConverter.GetBytes(troopCount), MemoryConstants.POINTER_BYTES, out bytesRead);
        }

        private void SetPlayerTroops(String playerColor, int troopCount)
        {
            IntPtr playerPtr = playerColorsToAddresses[playerColor];
            WriteProcessMemory(riskProcess.Handle, playerPtr + PlayerOffsets.PLACEABLE_TROOPS_OFFSET,
                BitConverter.GetBytes(troopCount), MemoryConstants.POINTER_BYTES, out bytesRead);
        }

        private void SetColor(String territoryName, String color)
        {
            IntPtr territoryPtr = territoryNamesToAdresses[territoryName];
            IntPtr playerPtr = playerColorsToAddresses[color];
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.PLAYER_OFFSET,
                BitConverter.GetBytes(playerPtr.ToInt64()), MemoryConstants.POINTER_BYTES, out bytesRead);
        }

        private void SetRegularTerritory(String territoryName)
        {
            IntPtr territoryPtr = territoryNamesToAdresses[territoryName];
            WriteProcessMemory(riskProcess.Handle, territoryPtr + TerritoryOffsets.TERRITORY_TYPE_OFFSET,
                BitConverter.GetBytes((int)TerritoryType.Regular), MemoryConstants.INT_BYTES, out bytesRead);
        }

        private void loadCsvButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "CSV files|*.csv;";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            String selectedFileName = openFileDialog.FileName;
            parseCsv(selectedFileName);
        }

        private void parseCsv(String fileName)
        {
            var lines = File.ReadLines(fileName);
            foreach (var line in lines)
            {
                var fields = line.ToLower().Split(',');
                if (fields.Length != CSVConstants.NUM_COLS
                    || !territoryNamesToAdresses.ContainsKey(fields[CSVConstants.TERRITORY_NAME])
                    || !playerColorsToAddresses.ContainsKey(fields[CSVConstants.COLOR])
                    || !int.TryParse(fields[CSVConstants.TERRITORY_TYPE], out _)
                    || !Enum.IsDefined(typeof(TerritoryType), int.Parse(fields[CSVConstants.TERRITORY_TYPE]))
                    || !int.TryParse(fields[CSVConstants.TROOP_COUNT], out _))
                {
                    continue;
                }

                SetColor(fields[CSVConstants.TERRITORY_NAME], fields[CSVConstants.COLOR]);
                SetTroops(fields[CSVConstants.TERRITORY_NAME], int.Parse(fields[CSVConstants.TROOP_COUNT]));
                SetTerritoryType(fields[CSVConstants.TERRITORY_NAME], int.Parse(fields[CSVConstants.TERRITORY_TYPE]));
            }
        }

        private void SetTerritoryType(String territoryName, int territoryType)
        {
            switch (territoryType)
            {
                case (int)TerritoryType.Regular:
                    SetRegularTerritory(territoryName);
                    break;
                case (int)TerritoryType.Capital:
                    SetCapital(territoryName);
                    break;
                case (int)TerritoryType.Blizzard:
                    SetBlizzard(territoryName);
                    break;
                default:
                    break;
            }
        }
    }
}