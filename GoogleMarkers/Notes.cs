using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using GMap.NET.WindowsForms;

namespace GoogleMarkers {
    public partial class Notes : Form {
        public Notes(GMapMarker _marker, string _dir) {
            InitializeComponent();
            Marker = _marker;
            Directory = _dir.Remove(_dir.Length - 9) + "/_notes";
            LoadNotes();
            Text = Marker.Tag.ToString();
        }
        private GMapMarker Marker;
        private string Directory;
        private List<string> ReadNotes() {

            List<string> _whole_file = new List<string>();

            StreamReader _rdr;
            if (File.Exists(Directory)) {
                _rdr = new StreamReader(Directory);
                do {
                    _whole_file.Add(_rdr.ReadLine());
                } while (!_rdr.EndOfStream);
                _rdr.Close();
            }
            
            return _whole_file;
        }
        private void LoadNotes() {
            List<string> _whole_file = ReadNotes();
            if (_whole_file.Count == 0 || _whole_file == null)
                return;
            
            foreach(string _s in _whole_file) {
                string _tmp = _s.Replace("[", "").Replace("]", "");
                if (Marker.Tag.ToString() == _tmp.Split('|')[0]) {
                    tb_notes.Text = _tmp.Split('|')[1];
                }
            }

        }
        private void btn_apply_Click(object sender, EventArgs e) {
            List<string> _whole_file = ReadNotes();
            int _index=-1;
            int _c = 0;

            if (File.Exists(Directory))
                File.Delete(Directory);

            if(_whole_file.Count!=0 || _whole_file!=null) {
                foreach (string _s in _whole_file) {
                    if (_s != "") {
                        
                        string _tmp = _s.Replace("[", "").Replace("]", "");
                        
                        if (_tmp.Split('|')[0] == Marker.Tag.ToString()) {
                            _index = _c;
                        }
                        _c++;
                    }
                }
                string _line = "";
                if (_index != -1) {
                    _whole_file[_index] = "[" + Marker.Tag.ToString() + "|" + tb_notes.Text + "]";
                    StreamWriter _wr = new StreamWriter(Directory);
                    foreach (string _s in _whole_file) {
                        _wr.WriteLine(_s);
                    }
                    _wr.Close();
                }
                else {
                    StreamWriter _wr = new StreamWriter(Directory);
                    foreach (string _s in _whole_file) {
                        _wr.WriteLine(_s);
                    }
                    _line += "[" + Marker.Tag.ToString() + "|" + tb_notes.Text + "]";
                    _wr.WriteLine(_line);
                    _wr.Close();
                }
            }
            else {
                StreamWriter _wr = new StreamWriter(Directory);
                _wr.Write("[" + Marker.Tag.ToString() + "|" + tb_notes.Text + "]");
                _wr.Close();
            }
        }
    }
}
