using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.World;
using MCForge.Gui.Utils;
using MCForge.Core;
using System.IO;
using MCForge.Utils;

namespace MCForge.Gui.Dialogs {
    public partial class MapManagerDialog : Form {
        public MapManagerDialog() {
            InitializeComponent();

            Level.OnAllLevelsLoad.Normal += new API.Events.Event<Level, API.Events.LevelLoadEventArgs>.EventHandler(OnAllLevelsLoad_Normal);
            Level.OnAllLevelsUnload.Normal += new API.Events.Event<Level, API.Events.LevelLoadEventArgs>.EventHandler(OnAllLevelsUnload_Normal);
        }



        //---Level Event Handlers -------------------------------

        void OnAllLevelsLoad_Normal(Level sender, API.Events.LevelLoadEventArgs args) {
            if ( lstUnloaded.Items.Contains(sender.Name) )
                lstUnloaded.Items.Remove(sender.Name);

           int index = GetRowIndexFromLevel(sender);
            if ( index == -1 ) {
                dtaLoaded.Rows.Add(GetRowDataFromLevel(sender));
            }
        }

        void OnAllLevelsUnload_Normal(Level sender, API.Events.LevelLoadEventArgs args) {
            if ( !lstUnloaded.Items.Contains(sender.Name) )
                lstUnloaded.Items.Add(sender.Name);

            int index = GetRowIndexFromLevel(sender);
            if ( index != -1 ) {
                dtaLoaded.Rows.RemoveAt(index);
            } 
        }

        // --- End Level Event Handlers--------------------------


        private void MapManagerDialog_Load(object sender, EventArgs e) {
            lstUnloaded.Items.Clear();
            lstUnloaded.Items.AddRange(Utilities.GetUnloadedLevels().ToArray());

            dtaLoaded.ColumnCount = 4;
            dtaLoaded.Columns[0].Name = "Name";
            dtaLoaded.Columns[1].Name = "Size";
            dtaLoaded.Columns[2].Name = "Physics";
            dtaLoaded.Columns[3].Name = "Player Count";

            dtaLoaded.Columns.Add(new DataGridViewButtonColumn() {
                HeaderText = "Unload",
                Text = "O",
                UseColumnTextForButtonValue = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dtaLoaded.Columns.Add(new DataGridViewButtonColumn() {
                HeaderText = "Reload",
                Text = "O",
                UseColumnTextForButtonValue = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dtaLoaded.Columns.Add(new DataGridViewButtonColumn() {
                HeaderText = "Delete",
                Text = "O",
                UseColumnTextForButtonValue = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dtaLoaded.Rows.Clear();
            for ( int i = 0; i < Level.Levels.Count; i++ ) {
                dtaLoaded.Rows.Add(GetRowDataFromLevel(Level.Levels[i]));
            }


        }

        #region Data Grid View Utils

        string[] GetRowDataFromLevel(Level level) {
            return new[] {level.Name, string.Format("{0} x {1} x  {2}", level.CWMap.Size.x, level.CWMap.Size.y, level.CWMap.Size.z), "True", level.Players.Count.ToString() };
        }

        int GetRowIndexFromLevel(Level level) {
            return GetRowIndexFromLevelName(level.Name);
        }

        int GetRowIndexFromLevelName(string name) {
            for ( int i = 0; i < dtaLoaded.Rows.Count; i++ ) {
                if(dtaLoaded.Rows[i].Cells[i].Value.ToString().Equals(name))
                    return i;
            }
            return -1;
        }

        #endregion


        private void dtaLoaded_CellClick(object sender, DataGridViewCellEventArgs e) {
            switch ( e.ColumnIndex ) {
                case 4: //Unload 
                Level.FindLevel(dtaLoaded.Rows[e.RowIndex].Cells[0].Value.ToString()).Unload(true);
                break;

                case 5: //Reload
                string name = dtaLoaded.Rows[e.RowIndex].Cells[0].Value.ToString();
                Level.FindLevel(name).Unload(true);
                Level.LoadLevel(name);
                break;

                case 6: //Delete
                if ( MessageBox.Show("Are you sure you want to delete this level?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes ) {
                    string levelName = dtaLoaded.Rows[e.RowIndex].Cells[0].Value.ToString();
                    Level.FindLevel(levelName).Unload(true);
                    File.Delete(FileUtils.LevelsPath + levelName + ".cw");
                }
                break;

            }
        }

    }
}
