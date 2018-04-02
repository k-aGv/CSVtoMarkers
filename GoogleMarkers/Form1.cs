using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GMap.NET.MapProviders;


namespace GoogleMarkers {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private bool _clickHandled = false;
        private double _zoomFactor = 6;
        private string _markers = Directory.GetCurrentDirectory() + "/.tmp/_temporary/_markers";

        GMapOverlay markers_overlay = new GMapOverlay("markers");
        TextBox tb_find_place;
        Button btn_find_place;

        private void InitUI() {
            btn_find_place = new Button {
                AutoSize = true,
                Name = "btn_find_place",
                Text = "Find place",
                Location = new Point(mymap.Location.X, mymap.Location.Y + mymap.Height + 5)
            };
            btn_find_place.Click += Btn_find_place_Click;

            Controls.Add(btn_find_place);
            tb_find_place = new TextBox {
                Width = 250,
                Name = "tb_find_place",
                Text = "Type destination",
                Location = new Point(btn_find_place.Location.X + btn_find_place.Width + 10,
                                     btn_find_place.Location.Y + 2),

            };
            Controls.Add(tb_find_place);
            tb_find_place.GotFocus += tb_find_place_GotFocus;
            tb_find_place.LostFocus += tb_find_place_LostFocus;
            tb_find_place.KeyDown += Btn_find_place_Click;
        }

        private void tb_find_place_LostFocus(object sender, EventArgs e) {
            ((TextBox)sender).Text = ((TextBox)sender).Text == "" ? "Type destination" : ((TextBox)sender).Text;
        }

        private void tb_find_place_GotFocus(object sender, EventArgs e) {
            ((TextBox)sender).Text = ((TextBox)sender).Text == "Type destination" ? "" : ((TextBox)sender).Text;
        }

        public string FirstLetterToUpper(string str) {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        private void Btn_find_place_Click(object sender, EventArgs e) {

            try {
                if (sender.GetType() == ((TextBox)sender).GetType())
                    if (((KeyEventArgs)e).KeyData != Keys.Return) {
                        return;
                    }
            }
            catch { }

            var coords = GMapProviders.GoogleMap.GetPoint(tb_find_place.Text, out GeoCoderStatusCode _e);
            if (coords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS)) {
                mymap.SetPositionByKeywords(tb_find_place.Text);
                mymap.Zoom = 10;
            }
            else {
                string _s = FirstLetterToUpper(tb_find_place.Text);

                coords = GMapProviders.GoogleMap.GetPoint(_s, out GeoCoderStatusCode __e);
                if (coords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS)) {
                    mymap.SetPositionByKeywords(tb_find_place.Text);
                    mymap.Zoom = 10;
                } else
                    MessageBox.Show("Destination \"" + tb_find_place.Text + "\" could not be found.", "Bad destination request...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ((KeyEventArgs)e).SuppressKeyPress = true;
        }

        private void Form1_Load(object sender, EventArgs e) {

            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            mymap.Width = Width - (3 * mymap.Location.X);
            mymap.Height = Height - (4 * mymap.Location.Y);

            mymap.MapProvider = GoogleMapProvider.Instance;//using it as FULL reference to have the complete list of providers

            GMaps.Instance.Mode = AccessMode.ServerOnly;

            mymap.SetPositionByKeywords("Greece");

            mymap.MinZoom = 0;
            mymap.MaxZoom = 18;
            mymap.Zoom = _zoomFactor;
            mymap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            mymap.DragButton = MouseButtons.Left;
            mymap.InvertedMouseWheelZooming = false;

            CreateHiddenDir();
            LoadMarkers();
            InitUI();
            AddMarkersFromCSV();
        }

    
        private void AddMarkersFromCSV()
        {
            string csvDir="";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                csvDir = openFileDialog1.FileName;
            }
            else
            {
                MessageBox.Show("CSV selection has been cancelled...");
                return;
            }
            StreamReader reader = new StreamReader(csvDir);
            do
            {
                string _tmp = reader.ReadLine();
                //MessageBox.Show(_tmp.Split(',')[0] + "\n" + _tmp.Split(',')[1]);
                _tmp = _tmp.ToLower();

                string dir = _tmp.Split(',')[1];
              //  MessageBox.Show(dir);
                dir += " " + _tmp.Split(',')[3];
               // MessageBox.Show(dir);

                if (dir.Contains("τ.θ.") ||
                    dir.Contains("τ.") ||
                    dir.Contains("θ.") ||
                    dir.Contains("τθ") ||
                    dir.Contains("τθ."))
                {
                    dir.Replace("τ.θ.", "");
                    dir.Replace("τ.", "");
                    dir.Replace("θ.", "");
                    dir.Replace("τθ", "");
                    dir.Replace("τθ.", "");

                }

                if (dir.Contains("οδοσ"))
                {
                    dir.Replace("οδοσ", "οδός");
                }

                if (dir.Contains("εθν.οδοσ"))
                {
                    dir.Replace("εθν.οδοσ", "εθνική");
                }



                // bool containsNumber = System.Text.RegularExpressions.Regex.IsMatch(dir, @"\d");
                // if (containsNumber)
                //  {

                //  }
                dir = dir.ToLower();
                var coords = GMapProviders.GoogleMap.GetPoint(dir, out GeoCoderStatusCode _e);
                int i = 0;
                while (i < 15)
                {
                    if (coords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS))
                    {
                        mymap.SetPositionByKeywords(dir);
                        mymap.Zoom = 10;

                        PointLatLng points = new PointLatLng(coords.Value.Lat, coords.Value.Lng);
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);

                        markers_overlay.Markers.Add(marker);

                        mymap.UpdateMarkerLocalPosition(marker);
                        mymap.Overlays.Clear();
                        mymap.Overlays.Add(markers_overlay);
                        break;
                    }
                    else
                    {
                       // MessageBox.Show("failed once.." + dir + "'");
                        dir = FirstLetterToUpper(dir);
                        coords = GMapProviders.GoogleMap.GetPoint(dir, out GeoCoderStatusCode __e);
                        if (coords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS))
                        {
                            mymap.SetPositionByKeywords(dir);
                            mymap.Zoom = 10;
                            PointLatLng points = new PointLatLng(coords.Value.Lat, coords.Value.Lng);
                            GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);
                            markers_overlay.Markers.Add(marker);

                            mymap.UpdateMarkerLocalPosition(marker);
                            mymap.Overlays.Clear();
                            mymap.Overlays.Add(markers_overlay);

                        }
                        else
                        {
                            MessageBox.Show("Destination \"" + dir + "\" could not be found.", "Bad destination request...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    i++;
                }
           

            } while (!reader.EndOfStream);

        }
        private void PlaceMarker(MouseEventArgs _e) {
            if (_e.Button == MouseButtons.Left && !mymap.IsDragging) {

                PointLatLng final = new PointLatLng(
                        mymap.FromLocalToLatLng(_e.X, _e.Y).Lat,
                        mymap.FromLocalToLatLng(_e.X, _e.Y).Lng
                        );

                GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(final, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);
                GeoCoderStatusCode status;
                var _tmp = GMapProviders.GoogleSatelliteMap.GetPlacemark(final, out status);
                if (status == GeoCoderStatusCode.G_GEO_SUCCESS) {
                    marker.Tag = _tmp.Value.Address;
                    marker.ToolTipText = _tmp.Value.Address;
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                }
                else {
                    MessageBox.Show("Could not obtain address of Marker from Google. Please try again", "Unsuccessful request", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                markers_overlay.Markers.Add(marker);

                mymap.UpdateMarkerLocalPosition(marker);
                mymap.Overlays.Clear();
                mymap.Overlays.Add(markers_overlay);

                EditMarkersMenuStripItem();
            }
        }
        private void RemoveMarker(GMapMarker _item, MouseEventArgs _e) {
            if (_e.Button == MouseButtons.Right && !mymap.IsDragging) {
                if (mymap.Overlays.Count != 0) {
                    mymap.Overlays[0].Markers.Remove(_item);
                    markers_overlay = mymap.Overlays[0];
                    mymap.UpdateMarkerLocalPosition(_item);
                    mymap.Overlays.Clear();
                    mymap.Overlays.Add(markers_overlay);
                    EditMarkersMenuStripItem();
                }
            }
        }
        private void FocusMarker(GMapMarker _item) {
            mymap.SetZoomToFitRect(new RectLatLng(_item.Position, new SizeLatLng(0.1, 0.1)));
            mymap.Position = new PointLatLng(_item.Position.Lat, _item.Position.Lng);
        }
        private void SaveMarkers() {
            if (File.Exists(_markers))
                File.Delete(_markers);
            StreamWriter _wr;
            if (mymap.Overlays.Count != 0) {
                if (mymap.Overlays[0].Markers.Count != 0) {
                    _wr = new StreamWriter(_markers);
                    foreach (GMapMarker _m in mymap.Overlays[0].Markers) {
                        _wr.WriteLine(_m.Tag + "|" + _m.Position.Lat + "|" + _m.Position.Lng);
                    }
                    _wr.Close();
                }
            }
        }
        private void LoadMarkers() {
            StreamReader _r;
            if (File.Exists(_markers)) {
                _r = new StreamReader(_markers);
                do {
                    string _tmp = _r.ReadLine();
                    PointLatLng final = new PointLatLng(
                        Convert.ToDouble(_tmp.Split('|')[1]),
                        Convert.ToDouble(_tmp.Split('|')[2])
                        );
                    GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(final, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);
                    marker.ToolTipText = _tmp.Split('|')[0];
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    marker.Tag = _tmp.Split('|')[0];
                    markers_overlay.Markers.Add(marker);
                    mymap.UpdateMarkerLocalPosition(marker);
                } while (!_r.EndOfStream);
                mymap.Overlays.Add(markers_overlay);
                mymap.SetZoomToFitRect(mymap.GetRectOfAllMarkers(mymap.Overlays[0].Id).Value);
                _r.Close();
            }

            EditMarkersMenuStripItem();
        }
        private void EditMarkersMenuStripItem() {
            if (mymap.Overlays.Count == 0)
                return;

            ToolStripItem[] _tmp = menuStrip1.Items.Find("Markers", false);
            if (_tmp.Length != 0) {
                menuStrip1.Items.RemoveAt(menuStrip1.Items.IndexOf(_tmp[0]));
            }

            if (mymap.Overlays[0].Markers.Count == 0)
                return;

            ToolStripMenuItem _section = new ToolStripMenuItem {
                Name = "Markers",
                Text = "Markers"
            };
            menuStrip1.Items.Add(_section);
            foreach (GMapMarker _m in mymap.Overlays[0].Markers) {
                ToolStripItem _t = new ToolStripMenuItem {
                    Name = _m.Tag.ToString(),
                    Text = _m.Tag.ToString()

                };
                _t.Click += MarkerMenuStrip_Click;
                _t.MouseHover += MarkerMenuStrip_Hover;
                _t.MouseLeave += MarkerMenuStrip_Leave;
                _section.DropDownItems.Add(_t);
            }
        }
        private void MarkerMenuStrip_Leave(object sender, EventArgs e) {
            foreach (GMapMarker _m in mymap.Overlays[0].Markers) {
                if (_m.Tag.ToString() == sender.ToString()) {
                    _m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    mymap.Refresh();
                }
            }
        }

        private void MarkerMenuStrip_Hover(object sender, EventArgs e) {
            foreach (GMapMarker _m in mymap.Overlays[0].Markers) {
                if (_m.Tag.ToString() == sender.ToString()) {
                    _m.ToolTipMode = MarkerTooltipMode.Always;
                    mymap.Refresh();
                }
            }            
        }

        private void MarkerMenuStrip_Click(object sender, EventArgs e) {
            foreach (GMapMarker _m in mymap.Overlays[0].Markers) {
                if (_m.Tag.ToString() == sender.ToString())
                    FocusMarker(_m);
            }
        }

        private void CreateHiddenDir() {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/.tmp/_temporary")) {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/.tmp/_temporary");
                DirectoryInfo dirinfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "/.tmp");
                dirinfo.Attributes = FileAttributes.Hidden;
                dirinfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "/.tmp/_temporary");
                dirinfo.Attributes = FileAttributes.Hidden;
            }
        }

        private void mymap_MouseClick(object sender, MouseEventArgs e) {
            if (!_clickHandled)
                PlaceMarker(e);
            _clickHandled = false;
        }
        public static Bitmap takeComponentScreenShot(Control control)
        {
            Rectangle rect = control.RectangleToScreen(control.DisplayRectangle);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            return bmp;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mymap.ShowCenter = false;
            mymap.Refresh();
            string bigImage = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar + "GMap_" + DateTime.Now.Ticks + ".png";
            Bitmap b = takeComponentScreenShot(mymap);
            b.Save(bigImage);
            if (File.Exists(bigImage)) MessageBox.Show("Picture saved to desktop!");
            else MessageBox.Show("Error while creating the image file");
            mymap.ShowCenter = true;
            mymap.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            SaveMarkers();
        }
        
        private void mymap_OnMarkerClick(GMapMarker item, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                FocusMarker(item);
                _clickHandled = true;
            }
            else {
                _clickHandled = false;
                RemoveMarker(item, e);
               
            }
        }
    }
}
