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
            string _notesName = _dir.Substring((_dir.LastIndexOf('\\') + 1), _dir.LastIndexOf('.') - (_dir.LastIndexOf('\\') + 1));
            Directory = _dir.Remove(_dir.LastIndexOf('\\')) + "/" + _notesName + "_notes";
            LoadNotesAsBytes();
            Text = Marker.Tag.ToString();
        }
        private GMapMarker Marker;
        private string Directory;

        private List<byte[]> ReadNotesAsBytes() {
            List<byte[]> _list = new List<byte[]>();
            StreamReader _rdr;
            if (File.Exists(Directory)) {
                _rdr = new StreamReader(Directory);
                do {
                    string[] _tmp = _rdr.ReadLine().Split(',');
                    byte[] _bytes = new byte[_tmp.Length];
                    for (int i = 0; i < _bytes.Length; i++) {
                        _bytes[i] = Convert.ToByte(_tmp[i]);
                    }
                    _list.Add(_bytes);
                } while (!_rdr.EndOfStream);
                _rdr.Close();
            }
            return _list;
        }
        private void LoadNotesAsBytes() {
            List<byte[]> _list = ReadNotesAsBytes();
            if (_list.Count == 0 || _list == null)
                return;

            foreach (byte[] _barray in _list) {
                string _tmp = Encoding.Unicode.GetString(_barray);

                _tmp = _tmp.Replace("[", "").Replace("]", "");
                if (Marker.Tag.ToString() == _tmp.Split('|')[0])
                    tb_notes.Text = _tmp.Split('|')[1];
            }
        }
        private void SaveNotes() {
            List<string> _whole_file = new List<string>();
            List<byte[]> _bytes = ReadNotesAsBytes();
            int _index = -1;

            if (File.Exists(Directory))
                File.Delete(Directory);

            foreach (byte[] _barray in _bytes) {
                _whole_file.Add(Encoding.Unicode.GetString(_barray));
            }

            if (_bytes.Count != 0 || _bytes != null) {
                int _c = 0;
                foreach (byte[] _barray in _bytes) {
                    string _tmp = Encoding.Unicode.GetString(_barray);
                    _tmp = _tmp.Replace("[", "").Replace("]", "");
                    if (_tmp.Split('|')[0] == Marker.Tag.ToString())
                        _index = _c;
                    _c++;
                }
                if (_index != -1) {
                    _whole_file[_index] = "[" + Marker.Tag.ToString() + "|" + tb_notes.Text + "]";
                    StreamWriter _wr = new StreamWriter(Directory);
                    foreach (string _s in _whole_file) {
                        byte[] _tmp = Encoding.Unicode.GetBytes(_s);
                        string _line = "";
                        foreach (byte _b in _tmp) {
                            _line += _b + ",";
                        }
                        _line = _line.Remove(_line.LastIndexOf(','));
                        _wr.WriteLine(_line);
                    }
                    _wr.Close();
                }
                else {
                    StreamWriter _wr = new StreamWriter(Directory);
                    foreach (string _s in _whole_file) {
                        byte[] _tmp = Encoding.Unicode.GetBytes(_s);
                        string _line = "";
                        foreach (byte _b in _tmp) {
                            _line += _b + ",";
                        }
                        _line = _line.Remove(_line.LastIndexOf(','));
                        _wr.WriteLine(_line);
                    }
                    byte[] _tmpBytes = Encoding.Unicode.GetBytes("[" + Marker.Tag.ToString() + "|" + tb_notes.Text + "]");
                    string _tmpLine = "";
                    foreach (byte _b in _tmpBytes) {
                        _tmpLine += _b + ",";
                    }
                    _tmpLine = _tmpLine.Remove(_tmpLine.LastIndexOf(','));
                    _wr.WriteLine(_tmpLine);
                    _wr.Close();
                }
            }
            else {
                StreamWriter _wr = new StreamWriter(Directory);
                byte[] _tmpBytes = Encoding.Unicode.GetBytes("[" + Marker.Tag.ToString() + "|" + tb_notes.Text + "]");
                string _tmpLine = "";
                foreach (byte _b in _tmpBytes) {
                    _tmpLine += _b + ",";
                }
                _tmpLine = _tmpLine.Remove(_tmpLine.LastIndexOf(','));
                _wr.Write(_tmpLine);
                _wr.Close();
            }
        }

        private void btn_apply_Click(object sender, EventArgs e) {
            SaveNotes();
        }
    }
}
