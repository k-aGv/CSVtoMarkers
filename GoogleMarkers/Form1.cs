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
        ListBox lbx_not_found_targets;
        Label lb_listbox;
        Button btn_find_place;

        private void InitUI() {
            btn_find_place = new Button {
                AutoSize = true,
                Name = "btn_find_place",
                Text = "Find place"
            };
            btn_find_place.Click += Btn_find_place_Click;

            Controls.Add(btn_find_place);
            tb_find_place = new TextBox {
                Width = 250,
                Name = "tb_find_place",
                Text = "Type destination"
            };
            
            tb_find_place.Location = new Point(
                                     Width - tb_find_place.Width - 25,
                                     mymap.Location.Y + btn_find_place.Height + 5);
            btn_find_place.Location = new Point(
                                    tb_find_place.Location.X,
                                    mymap.Location.Y);


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
                mymap.Zoom = 15;
            }
            else {
                string _s = FirstLetterToUpper(tb_find_place.Text);

                coords = GMapProviders.GoogleMap.GetPoint(_s, out GeoCoderStatusCode __e);
                if (coords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS)) {
                    mymap.SetPositionByKeywords(tb_find_place.Text);
                    mymap.Zoom = 15;
                } else
                    MessageBox.Show("Destination \"" + tb_find_place.Text + "\" could not be found.", "Bad destination request...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (Equals(e.GetType(), typeof(KeyEventArgs)))
                ((KeyEventArgs)e).SuppressKeyPress = true;
        }

        private void Form1_Load(object sender, EventArgs e) {

            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            InitUI();
            mymap.Width = tb_find_place.Location.X - mymap.Location.X;
            mymap.Height = Height - mymap.Location.Y - menuStrip1.Height - 25;

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
            
            AddMarkersFromCSV();
        }

    
        private void AddMarkersFromCSV()
        {
            string csvDir="";
            openFileDialog1.FileName = "";
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
            List<string> notFound = new List<string>();
            do
            {
                string _tmp = reader.ReadLine();

                _tmp = _tmp.ToLower();

                string dir = _tmp.Split(',')[1] +" "+_tmp.Split(',')[3];
                int price = Convert.ToInt32((_tmp.Split(',')[_tmp.Split(',').Length - 1]));
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
                int i = 0;
                bool _found = false;
                
                while (i < 50 && !_found) {
                    var coords = GMapProviders.GoogleMap.GetPoint(dir, out GeoCoderStatusCode _e);
                    var _tmpCoords = new Placemark?();
                    if (coords.HasValue)
                        _tmpCoords = GMapProviders.GoogleMap.GetPlacemark(coords.Value, out GeoCoderStatusCode _geo);
                
                    if (_tmpCoords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS))
                    {
                        PointLatLng points = new PointLatLng(coords.Value.Lat, coords.Value.Lng);
                        GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);
                        
                        marker.Tag = _tmpCoords.Value.Address;
                        marker.ToolTipText = _tmpCoords.Value.Address+", Τζίρος: "+price;
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                        markers_overlay.Markers.Add(marker);

                        mymap.UpdateMarkerLocalPosition(marker);
                        mymap.Overlays.Clear();
                        mymap.Overlays.Add(markers_overlay);
                        _found = true;
                    }
                    else
                    {
                        dir = FirstLetterToUpper(dir);
                        coords = GMapProviders.GoogleMap.GetPoint(dir, out GeoCoderStatusCode __e);
                        if (_tmpCoords.HasValue && _e.Equals(GeoCoderStatusCode.G_GEO_SUCCESS))
                        {
                            PointLatLng points = new PointLatLng(coords.Value.Lat, coords.Value.Lng);
                            GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);

                            marker.Tag = _tmpCoords.Value.Address;
                            marker.ToolTipText = _tmpCoords.Value.Address + ", Τζίρος: " + price;
                            marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                            markers_overlay.Markers.Add(marker);

                            mymap.UpdateMarkerLocalPosition(marker);
                            mymap.Overlays.Clear();
                            mymap.Overlays.Add(markers_overlay);
                        }
                    }
                    i++;
                    Application.DoEvents();
                }
                if(!_found) {
                    notFound.Add(dir);
                }
                mymap.SetZoomToFitRect(mymap.GetRectOfAllMarkers(mymap.Overlays[0].Id).Value);
                
            } while (!reader.EndOfStream);
            EditMarkersMenuStripItem();
            LogTheNotFoundTargetsList(notFound);

        }

        private void LogTheNotFoundTargetsList(List<string> missing_targets) {
            if (missing_targets.Count == 0)
                return;
            lb_listbox = new Label {
                AutoSize = true,
                Text = "Google was unable to find the following addresses:",
                Location = new Point(
                    tb_find_place.Location.X, 
                    tb_find_place.Location.Y + tb_find_place.Height + 10)
            };
            Controls.Add(lb_listbox);

            lbx_not_found_targets = new ListBox {
                Location = new Point(lb_listbox.Location.X, lb_listbox.Location.Y + lb_listbox.Height + 5),
                Width = tb_find_place.Width,
                Height = missing_targets.Count * ((int)(Font.Size * 2)),
                MaximumSize = new Size(tb_find_place.Width, mymap.Height / 3 - (lb_listbox.Location.Y + lb_listbox.Height + 5)),
                HorizontalScrollbar = true
            };
            Controls.Add(lbx_not_found_targets);
            foreach (string _s in missing_targets) {
                lbx_not_found_targets.Items.Add(_s);
            }
            lbx_not_found_targets.SelectedIndexChanged += Lbx_not_found_targets_SelectedIndexChanged;
        }

        private void Lbx_not_found_targets_SelectedIndexChanged(object sender, EventArgs e) {
            tb_find_place.Text = ((ListBox)sender).SelectedItem.ToString();
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
                Text = "Markers",
                DropDownDirection = ToolStripDropDownDirection.BelowRight
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
            _section.DropDown.MaximumSize = new Size(_section.DropDown.Width, mymap.Height/3);
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
