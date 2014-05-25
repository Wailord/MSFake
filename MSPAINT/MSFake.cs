using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MSPAINT
{
    public partial class MSFake : Form
    {
        public enum TOOLS
        {
            Pencil, Brush, Selection, Curve,
            Polygon, Text, Rectangle, Ellipse,
            Erase
        }

        public enum CURVESTATES
        {
            NoClick, OneClick, TwoClicks, Drawn
        }

        private TOOLS _tool;
        private CURVESTATES _curve_state;
        private Bitmap canvas;
        private Color _color;

        private Point _starting_point;
        private Point _ending_point;
        private Point _bez_1;

        private List<string> fonts;

        bool mouse_down;
        bool sel_mouse_down;

        string filename;

        bool show_toolbox;
        bool show_colors;
        bool selected_box;

        private int? last_x, last_y;

        public MSFake()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // filename in case filename is added later on
            filename = "Untitled*";

            // ensure selection box doesn't get drawn on top of
            pic_selection.SendToBack();

            // initialize bools
            sel_mouse_down = false;
            mouse_down = false;
            selected_box = false;
            show_toolbox = true;
            show_colors = true;

            // initialize various states
            _tool = TOOLS.Pencil;
            _curve_state = CURVESTATES.NoClick;
            _color = Color.Red;

            // set up font collection
            fonts = new List<string>();
            FontFamily[] fontList = new System.Drawing.Text.InstalledFontCollection().Families;
            foreach (FontFamily font in fontList)
                fonts.Add(font.Name);
            cmb_fonts.DataSource = fonts;

            // set up canvas to be drawn on
            canvas = new Bitmap(panel_canvas.Width, panel_canvas.Height);
        }

        private void panel_canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // on MouseMove, call appropriate function depending
            // on current tool
            switch (_tool)
            {
                case TOOLS.Pencil:
                    PencilTool(e);
                    break;
                case TOOLS.Brush:
                    BrushTool(e);
                    break;
                case TOOLS.Selection:
                    SelectionToolMouseMove(e);
                    break;
                case TOOLS.Curve:
                    CurveTool(e);
                    break;
                case TOOLS.Polygon:
                    PolygonTool(e);
                    break;
                case TOOLS.Rectangle:
                    RectangleTool(e);
                    break;
                case TOOLS.Ellipse:
                    EllipseTool(e);
                    break;
                case TOOLS.Erase:
                    EraseTool(e);
                    break;
            }
        }

        private void PencilTool(MouseEventArgs e)
        {
            // initialize coordinates to null
            if (e.Button == MouseButtons.None)
            {
                last_x = null;
                last_y = null;
            }

            // draw line from last coordinate to new coordinate when
            // mouse moves
            if (e.Button == MouseButtons.Left)
            {
                if (last_x != null && last_y != null)
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    using (Graphics h = panel_canvas.CreateGraphics())
                    {
                        g.DrawLine(new Pen(_color, 1), last_x.Value, last_y.Value, e.X, e.Y);
                        h.DrawLine(new Pen(_color, 1), last_x.Value, last_y.Value, e.X, e.Y);
                    }
                }

                last_x = e.X;
                last_y = e.Y;
            }
        }

        private void BrushTool(MouseEventArgs e)
        {
            // if brush tool is selected, you should
            // be able to adjust brush size
            updn_brush_size.Enabled = true;

            // initialize coordinates to null
            if (e.Button == MouseButtons.None)
            {
                last_x = null;
                last_y = null;
            }

            // draw ellipse from last coordinate to new coordinate when
            // mouse moves (so brush clicks are rendered as circles)
            if (e.Button == MouseButtons.Left)
            {
                if (last_x != null && last_y != null)
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    using (Graphics h = panel_canvas.CreateGraphics())
                    {

                        g.FillEllipse(new HatchBrush(HatchStyle.Sphere, _color),
                            last_x.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            last_y.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            Convert.ToInt32(updn_brush_size.Value) + 1,
                            Convert.ToInt32(updn_brush_size.Value) + 1);
                        h.FillEllipse(new HatchBrush(HatchStyle.Sphere, _color),
                            last_x.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            last_y.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            Convert.ToInt32(updn_brush_size.Value) + 1,
                            Convert.ToInt32(updn_brush_size.Value) + 1);
                    }
                }

                last_x = e.X;
                last_y = e.Y;
            }
        }

        private void CurveTool(MouseEventArgs e)
        {
            // function here in case I later want to add
            // support for viewing bezier curves as you
            // draw them
        }

        private void PolygonTool(MouseEventArgs e)
        {
            // function here in case I later want to add
            // support for viewing polygons curves as you
            // move your mouse
        }

        private void TextTool(MouseEventArgs e)
        {
            // create brush to use to draw string
            SolidBrush s = new SolidBrush(_color);
            string font = cmb_fonts.SelectedValue.ToString();
            Font f = new Font(font, Convert.ToSingle(updn_font_size.Value));

            // draw string where mouse was clicked
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawString(txt_text_input.Text, f, s, new Point(e.X, e.Y));
                panel_canvas.Invalidate();
            }
        }

        private void RectangleTool(MouseEventArgs e)
        {
            // function here in case I later want to add
            // support for viewing rectangles as you
            // draw them
        }

        private void EllipseTool(MouseEventArgs e)
        {
            // function here in case I later want to add
            // support for viewing ellipses as you
            // draw them
        }

        private void EnableAllButtons()
        {
            // function for convenience to make sure
            // all buttons are enabled and selection
            // boxes are cleared upon tool switch

            selected_box = false;
            btn_erase.Enabled = true;
            btn_brush.Enabled = true;
            btn_curve.Enabled = true;
            btn_ellipse.Enabled = true;
            btn_pencil.Enabled = true;
            btn_polygon.Enabled = true;
            btn_rect.Enabled = true;
            btn_select.Enabled = true;
            btn_text.Enabled = true;
        }

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            // switch to pencil tool
            updn_brush_size.Enabled = false;
            EnableAllButtons();
            btn_pencil.Enabled = false;
            _tool = TOOLS.Pencil;
            status_bar.Text = "Switched to Pencil tool.";
        }

        private void btn_brush_Click(object sender, EventArgs e)
        {
            // switch to brush tool
            updn_brush_size.Enabled = true;
            EnableAllButtons();
            btn_brush.Enabled = false;
            _tool = TOOLS.Brush;
            status_bar.Text = "Switched to Brush tool.";
        }

        private void btn_rect_Click(object sender, EventArgs e)
        {
            // switch to rectangle tool
            updn_brush_size.Enabled = true;
            EnableAllButtons();
            btn_rect.Enabled = false;
            _tool = TOOLS.Rectangle;
            status_bar.Text = "Switched to Rectangle tool.";
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            // switch to selection tool
            updn_brush_size.Enabled = false;
            EnableAllButtons();
            btn_select.Enabled = false;
            _tool = TOOLS.Selection;
            status_bar.Text = "Switched to Selection tool.";
        }

        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            // switch to ellipse tool
            updn_brush_size.Enabled = true;
            EnableAllButtons();
            btn_ellipse.Enabled = false;
            _tool = TOOLS.Ellipse;
            status_bar.Text = "Switched to Ellipse tool.";
        }

        private void btn_text_Click(object sender, EventArgs e)
        {
            // switch to text tool
            updn_brush_size.Enabled = false;
            EnableAllButtons();
            btn_text.Enabled = false;
            _tool = TOOLS.Text;
            status_bar.Text = "Switched to Text tool.";
        }

        private void btn_curve_Click(object sender, EventArgs e)
        {
            // switch to bezier curve tool
            updn_brush_size.Enabled = false;
            EnableAllButtons();
            btn_curve.Enabled = false;
            _tool = TOOLS.Curve;
            status_bar.Text = "Switched to Curve tool.";
        }

        private void btn_polygon_Click(object sender, EventArgs e)
        {
            // switch to polygon tool
            updn_brush_size.Enabled = false;
            EnableAllButtons();
            btn_polygon.Enabled = false;
            _tool = TOOLS.Polygon;
            status_bar.Text = "Switched to Polygon tool.";
        }

        private void btn_red_Click(object sender, EventArgs e)
        {
            // switch selected color to red
            EnableAllColors();
            btn_red.Enabled = false;
            _color = Color.Red;
            status_bar.Text = "Switched color to Red.";
        }

        private void btn_orange_Click(object sender, EventArgs e)
        {
            // switch selected color to orange
            EnableAllColors();
            btn_orange.Enabled = false;
            _color = Color.Orange;
            status_bar.Text = "Switched color to Orange.";
        }

        private void btn_yellow_Click(object sender, EventArgs e)
        {
            // switch selected color to yellow
            EnableAllColors();
            btn_yellow.Enabled = false;
            _color = Color.Yellow;
            status_bar.Text = "Switched color to Yellow.";
        }

        private void btn_green_Click(object sender, EventArgs e)
        {
            // switch selected color to green
            EnableAllColors();
            btn_green.Enabled = false;
            _color = Color.Lime;
            status_bar.Text = "Switched color to Lime.";
        }

        private void btn_blue_Click(object sender, EventArgs e)
        {
            // switch selected color to blue
            EnableAllColors();
            btn_blue.Enabled = false;
            _color = Color.Blue;
            status_bar.Text = "Switched color to Blue.";
        }

        private void btn_purple_Click(object sender, EventArgs e)
        {
            // switch selected color to purple
            EnableAllColors();
            btn_purple.Enabled = false;
            _color = Color.Purple;
            status_bar.Text = "Switched color to Purple.";
        }

        private void btn_black_Click(object sender, EventArgs e)
        {
            // switch selected color to black
            EnableAllColors();
            btn_black.Enabled = false;
            _color = Color.Black;
            status_bar.Text = "Switched color to Black.";
        }

        private void btn_white_Click(object sender, EventArgs e)
        {
            // switch selected color to maroon
            EnableAllColors();
            btn_maroon.Enabled = false;
            _color = Color.Maroon;
            status_bar.Text = "Switched color to Maroon.";
        }

        private void btn_silver_Click(object sender, EventArgs e)
        {
            // switch selected color to silver
            EnableAllColors();
            btn_silver.Enabled = false;
            _color = Color.Silver;
            status_bar.Text = "Switched color to Silver.";
        }

        private void btn_custom_color_Click(object sender, EventArgs e)
        {
            EnableAllColors();
         
            // open and show a color dialog
            ColorDialog diag = new ColorDialog();
            diag.ShowDialog();

            // get color from dialog (default black)
            _color = diag.Color;
            btn_custom_color.BackColor = _color;
            status_bar.Text = "Switched color to " + _color.ToArgb().ToString() + ".";
        }

        private void EnableAllColors()
        {
            // function for convenient switching
            // of colors
            btn_red.Enabled = true;
            btn_orange.Enabled = true;
            btn_yellow.Enabled = true;
            btn_green.Enabled = true;
            btn_blue.Enabled = true;
            btn_purple.Enabled = true;
            btn_black.Enabled = true;
            btn_maroon.Enabled = true;
            btn_silver.Enabled = true;
        }

        private void panel_canvas_MouseDown(object sender, MouseEventArgs e)
        {
            // call appropriate tool function upon mousedown in the canvas
            mouse_down = true;
            switch (_tool)
            {
                case TOOLS.Rectangle:
                    _starting_point = new Point(e.X, e.Y);
                    break;
                case TOOLS.Ellipse:
                    _starting_point = new Point(e.X, e.Y);
                    break;
                case TOOLS.Text:
                    TextTool(e);
                    break;
                case TOOLS.Curve:
                    CurveToolDown(e);
                    break;
                case TOOLS.Polygon:
                    PolygonToolDown(e);
                    break;
                case TOOLS.Selection:
                    SelectionToolDown(e);
                    break;
            }
        }

        private void SelectionToolDown(MouseEventArgs e)
        {
            if (selected_box)
            {
                // if user is starting a new box, draw the old
                // selection back to the screen
                if (pic_selection.Image != null)
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                            pic_selection.Location.X, pic_selection.Location.Y,
                        pic_selection.Width, pic_selection.Height);
                        g.DrawImage(pic_selection.Image, pic_selection.Location);
                        panel_canvas.Invalidate();
                    }
                }
            }
            
            // set new global mouse starting point
            _starting_point = new Point(e.X, e.Y);
        }

        private void panel_canvas_MouseUp(object sender, MouseEventArgs e)
        {
            // call appropriate tool function upon panel's mouseup
            mouse_down = false;
            switch (_tool)
            {
                case TOOLS.Rectangle:
                    DrawRectangleEnd(e);
                    break;
                case TOOLS.Ellipse:
                    DrawEllipseEnd(e);
                    break;
                case TOOLS.Curve:
                    CurveToolUp(e);
                    break;
                case TOOLS.Selection:
                    SelectionToolUp(e);
                    break;
            }
        }

        private void SelectionToolMouseMove(MouseEventArgs e)
        {
            // blank function in case I want to add support
            // for watching the selection box draw in the future
        }

        private void SelectionToolUp(MouseEventArgs e)
        {
            bool undid = false;

            if (!_starting_point.Equals(e.Location))
            {
                // if there's an actual box
                Rectangle rect = new Rectangle();

                // set mouse state variable
                mouse_down = false;

                // bring the selection box to the forefront
                // and give it a border so the user can see it
                pic_selection.BringToFront();
                pic_selection.BorderStyle = BorderStyle.FixedSingle;

                // validate selection box's size so it does not
                // go off the canvas
                if (_starting_point.X <= e.X)
                {
                    if (_starting_point.Y <= e.Y)
                    {
                        rect.X = _starting_point.X;
                        rect.Y = _starting_point.Y;
                    }
                    else
                    {
                        rect.X = _starting_point.X;
                        rect.Y = e.Y;
                    }
                }
                else
                {
                    if (_starting_point.Y <= e.Y)
                    {
                        rect.X = e.X;
                        rect.Y = _starting_point.Y;
                    }
                    else
                    {
                        rect.X = e.X;
                        rect.Y = e.Y;
                    }
                }

                // create selection box's new size
                rect.Width = Math.Abs(e.X - _starting_point.X);
                rect.Height = Math.Abs(e.Y - _starting_point.Y);

                // validate the coordinates for the selection box
                if (rect.X < 0)
                    rect.X = 0;
                if (rect.Y < 0)
                    rect.Y = 0;
                if (rect.Size.Width + rect.Location.X > panel_canvas.Width)
                    rect.Width = panel_canvas.Width - rect.Location.X;
                if (rect.Size.Height + rect.Location.Y > panel_canvas.Height)
                    rect.Height = panel_canvas.Height - rect.Location.Y;

                // set the properties of the selection box and show it to the user
                pic_selection.Size = rect.Size;
                pic_selection.Location = rect.Location;
                pic_selection.Visible = true;

                if (pic_selection.Size.Width * pic_selection.Size.Height != 0)
                {
                    // if there's a valid box, crop the image it selected from
                    // the canvas
                    Bitmap cropped_canvas =
                        new Bitmap(canvas.Clone(
                        new Rectangle(
                                pic_selection.Location.X,
                                pic_selection.Location.Y,
                                pic_selection.Width,
                                pic_selection.Height),
                        System.Drawing.Imaging.PixelFormat.DontCare));

                    Bitmap bg = new Bitmap(pic_selection.Width, pic_selection.Height);

                    // then clear the spot the selection box covered and set the
                    // selection box's image to the image that was cropped
                    using (Graphics g = Graphics.FromImage(bg))
                    using (Graphics h = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                            0, 0,
                            pic_selection.Width, pic_selection.Height);
                        g.DrawImage(cropped_canvas, new Point(0, 0));
                        h.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                            pic_selection.Location.X, pic_selection.Location.Y,
                            pic_selection.Width, pic_selection.Height);
                        
                        // redraw the screen
                        panel_canvas.Invalidate();
                    }

                    pic_selection.Image = bg;

                    selected_box = true;
                }
                else
                {
                    // if a box was made with a height or width of zero,
                    // user unselected any box that was selected
                    undid = false;
                }
            }
            else
            {
                // if user only clicked, they have cleared a selection
                undid = true;
            }

            if(undid)
            {
                // if user undid their selection
                if (pic_selection.Image != null)
                {
                    // if selection was undone and there is an image
                    // to draw back to the screen, draw it a background
                    // and draw it back to the canvas
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                            pic_selection.Location.X, pic_selection.Location.Y,
                        pic_selection.Width, pic_selection.Height);
                        g.DrawImage(pic_selection.Image, pic_selection.Location);
                        panel_canvas.Invalidate();
                    }
                }

                // reset state variables for selection box
                selected_box = false;
                mouse_down = false;
                pic_selection.Visible = false;
                pic_selection.SendToBack();
            }
        }

        private void PolygonToolDown(MouseEventArgs e)
        {
            // upon first click with the polygon tool,
            // set the necessary variables
            if (last_x == null)
            {
                _starting_point.X = e.X;
                _starting_point.Y = e.Y;

                last_x = e.X;
                last_y = e.Y;
            }

            else
            {
                // set the area around the starting point for the polygon
                // that should be considered the origin
                int snap_tolerance = 10;

                using (Graphics g = Graphics.FromImage(canvas))
                {
                    // if click was in origin, end the polygon
                    if ((e.X > _starting_point.X - snap_tolerance
                        && e.X < _starting_point.X + snap_tolerance)
                        && (e.Y > _starting_point.Y - snap_tolerance
                        && e.Y < _starting_point.Y + snap_tolerance))
                    {
                        g.DrawLine(new Pen(_color, Convert.ToSingle(updn_brush_size.Value)),
                            new Point(last_x ?? e.X, last_y ?? e.Y),
                            new Point(_starting_point.X, _starting_point.Y));
                        panel_canvas.Invalidate();

                        // reset last-click coordinates
                        last_x = null;
                        last_y = null;
                    }

                    else
                    {
                        // if click was not in origin, connect the two mouseclicks
                        g.DrawLine(new Pen(_color, Convert.ToSingle(updn_brush_size.Value)),
                            new Point(last_x ?? e.X, last_y ?? e.Y),
                            new Point(e.X, e.Y));
                        panel_canvas.Invalidate();

                        // set new "old" coordinates
                        last_x = e.X;
                        last_y = e.Y;
                    }
                }
            }
        }

        private void DrawEllipseEnd(MouseEventArgs e)
        {
            Rectangle rect = new Rectangle();

            // create coordinates for rectangle to draw
            // ellipse in
            if (_starting_point.X <= e.X)
            {
                if (_starting_point.Y <= e.Y)
                {
                    rect.X = _starting_point.X;
                    rect.Y = _starting_point.Y;
                }
                else
                {
                    rect.X = _starting_point.X;
                    rect.Y = e.Y;
                }
            }
            else
            {
                if (_starting_point.Y <= e.Y)
                {
                    rect.X = e.X;
                    rect.Y = _starting_point.Y;
                }
                else
                {
                    rect.X = e.X;
                    rect.Y = e.Y;
                }
            }

            // create rectangle's size, ensuring it is never negative
            rect.Width = Math.Abs(e.X - _starting_point.X);
            rect.Height = Math.Abs(e.Y - _starting_point.Y);

            // draw the ellipse in the calculated rectangle
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawEllipse(new Pen(_color, Convert.ToInt32(updn_brush_size.Value)), rect);
                panel_canvas.Invalidate();
            }
        }

        private void DrawRectangleEnd(MouseEventArgs e)
        {
            Rectangle rect = new Rectangle();

            // create coordinates for rectangle
            if (_starting_point.X <= e.X)
            {
                if (_starting_point.Y <= e.Y)
                {
                    rect.X = _starting_point.X;
                    rect.Y = _starting_point.Y;
                }
                else
                {
                    rect.X = _starting_point.X;
                    rect.Y = e.Y;
                }
            }
            else
            {
                if (_starting_point.Y <= e.Y)
                {
                    rect.X = e.X;
                    rect.Y = _starting_point.Y;
                }
                else
                {
                    rect.X = e.X;
                    rect.Y = e.Y;
                }
            }

            // create rectangle's size, ensuring it is never negative
            rect.Width = Math.Abs(e.X - _starting_point.X);
            rect.Height = Math.Abs(e.Y - _starting_point.Y);

            // draw the rectangle
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawRectangle(new Pen(_color, Convert.ToInt32(updn_brush_size.Value)), rect);
                panel_canvas.Invalidate();
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                // if a selection is made, clear selection box only
                pic_selection.Image = null;
                pic_selection.Invalidate();
            }
            else
            {
                // if no selection is made, clear entire canvas and
                // states
                canvas = new Bitmap(panel_canvas.Width, panel_canvas.Height);
                status_bar.Text = "Wiped canvas.";
                _curve_state = CURVESTATES.NoClick;
                last_x = null;
                last_y = null;
                panel_canvas.Invalidate();
            }
        }

        private void btn_erase_Click(object sender, EventArgs e)
        {
            // switch to eraser tool
            updn_brush_size.Enabled = true;
            EnableAllButtons();
            btn_erase.Enabled = false;
            _tool = TOOLS.Erase;
            status_bar.Text = "Switched to Erase tool.";
        }

        private void EraseTool(MouseEventArgs e)
        {
            // functions exactly as a brush, only with a color
            // always set to the panel's background color

            // allow user to manipulate brush size
            updn_brush_size.Enabled = true;

            // initalize old mouse coordinates to null
            if (e.Button == MouseButtons.None)
            {
                last_x = null;
                last_y = null;
            }

            // on click, draw an ellipse around mouse as it moves
            if (e.Button == MouseButtons.Left)
            {
                if (last_x != null && last_y != null)
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    using (Graphics h = panel_canvas.CreateGraphics())
                    {
                        g.FillEllipse(new SolidBrush(panel_canvas.BackColor),
                            last_x.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            last_y.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            Convert.ToInt32(updn_brush_size.Value) + 1,
                            Convert.ToInt32(updn_brush_size.Value) + 1);
                        h.FillEllipse(new SolidBrush(panel_canvas.BackColor),
                            last_x.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            last_y.Value - (Convert.ToInt32(updn_brush_size.Value) + 1) / 2,
                            Convert.ToInt32(updn_brush_size.Value) + 1,
                            Convert.ToInt32(updn_brush_size.Value) + 1);
                    }
                }
            }

            last_x = e.X;
            last_y = e.Y;
        }

        private void btn_flip_vertical_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                // if selection was made, flip it vertically
                pic_selection.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                pic_selection.Invalidate();
            }
            else
            {
                // if no selection was made, flip canvas vertically
                canvas.RotateFlip(RotateFlipType.RotateNoneFlipY);
                status_bar.Text = "Flipped canvas vertically.";
                panel_canvas.Invalidate();
            }
        }

        private void panel_canvas_Paint(object sender, PaintEventArgs e)
        {
            // clear old panel; draw new canvas on top
            using (Graphics gfx = e.Graphics)
            {
                gfx.Clear(panel_canvas.BackColor);
                gfx.DrawImage(canvas, 0, 0, canvas.Width, canvas.Height);
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            // open a save dialog, only allowing to save as bmp
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Bitmap Image|*.bmp";
            Bitmap bg = new Bitmap(canvas.Width, canvas.Height);

            using (Graphics g = Graphics.FromImage(bg))
            {
                // draw background to the image
                g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                    new Rectangle(0, 0, panel_canvas.Width, panel_canvas.Height));
                g.DrawImage(canvas, new Point(0,0));
            }
            if (save.ShowDialog() == DialogResult.OK)
            {
                // if save selection went ok, save the file
                bg.Save(save.FileName);
            }
        }

        private void btn_flip_horizontal_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                // if a selection is made, flip it horizontally
                pic_selection.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pic_selection.Invalidate();
            }
            else
            {
                // if no selection is made, flip the entire canvas
                // horizontally
                canvas.RotateFlip(RotateFlipType.RotateNoneFlipX);
                panel_canvas.Invalidate();
                status_bar.Text = "Flipped canvas horizontally.";
            }
        }

        private void btn_rotate_left_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                // if a selection is made, rotate the selection box
                // as well as the image inside of it
                Size temp = pic_selection.Size;
                pic_selection.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                pic_selection.Width = temp.Height;
                pic_selection.Height = temp.Width;

                // erase old image and redraw rotated version
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                        new Rectangle(pic_selection.Location, pic_selection.Size));
                    panel_canvas.Invalidate();
                }
            }
            else
            {
                // if no selection is made, rotate entire canvas
                canvas.RotateFlip(RotateFlipType.Rotate270FlipNone);
                panel_canvas.Invalidate();
                status_bar.Text = "Rotated canvas right.";
            }
        }

        private void btn_rotate_right_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                // if a selection is made, rotate the selection box
                // as well as the image inside of it
                Size temp = pic_selection.Size;
                pic_selection.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pic_selection.Width = temp.Height;
                pic_selection.Height = temp.Width;

                // erase old image and redraw rotated version
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                        new Rectangle(pic_selection.Location, pic_selection.Size));
                    panel_canvas.Invalidate();
                }
            }
            else
            {
                // if no selection is made, rotate entire canvas
                canvas.RotateFlip(RotateFlipType.Rotate90FlipNone);
                panel_canvas.Invalidate();
                status_bar.Text = "Rotated canvas right.";
            }
        }

        private void btn_toggle_toolbox_Click(object sender, EventArgs e)
        {
            // toggles visibility of the toolbox at the right
            // of the form
            if (show_toolbox)
            {
                this.Width = 734;
                show_toolbox = false;
                status_bar.Text = "Hid toolbox.";
            }
            else
            {
                this.Width = 860;
                show_toolbox = true;
                status_bar.Text = "Revealed toolbox.";
            }
        }

        private void btn_toggle_color_Click(object sender, EventArgs e)
        {
            // used to toggle visibility of the color bar
            // at the bottom of the form
            if (show_colors)
            {
                this.Height = 795;
                show_colors = false;
                grp_color_box.Visible = false;
                status_bar.Text = "Hid colors.";
            }
            else
            {
                this.Height = 845;
                show_colors = true;
                grp_color_box.Visible = true;
                status_bar.Text = "Revealed colors.";
            }
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            // create open file dialog with appropriate extensions
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "Image files (*bmp, *.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.bmp; *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                // redraw canvas with new pic
                canvas = new Bitmap(panel_canvas.Width, panel_canvas.Height);
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImage(new Bitmap(diag.FileName), new Point(0,0));
                }
                // repaint panel
                panel_canvas.Invalidate();

                status_bar.Text = "Opened " + diag.FileName;
            }
            else
            {
                status_bar.Text = "Did not open file.";
            }
        }

        private void btn_toggle_status_Click(object sender, EventArgs e)
        {
            // toggle status bar visibility
            status_bar.Visible = !status_bar.Visible;
        }

        private void btn_skew_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                float degreesX;
                float degreesY;
                try
                {
                    degreesX = Convert.ToSingle(txt_skew_horiz.Text);
                    if (degreesX > 89)
                    {
                        degreesX = 89;
                        txt_skew_horiz.Text = "89";
                    }

                    else if (degreesX < -89)
                    {
                        degreesX = -89;
                        txt_skew_horiz.Text = "-89";
                    }
                }
                catch (Exception)
                {
                    degreesX = 0;
                    txt_skew_horiz.Text = "0";
                }

                try
                {
                    degreesY = Convert.ToSingle(txt_skew_vert.Text);
                    if (degreesY > 89)
                    {
                        degreesY = 89;
                        txt_skew_vert.Text = "89";
                    }

                    else if (degreesY < -89)
                    {
                        degreesY = -89;
                        txt_skew_vert.Text = "-89";
                    }
                }
                catch (Exception)
                {
                    degreesY = 0;
                    txt_skew_vert.Text = "0";
                }

                degreesX /= 2;
                degreesY /= 2;

                int dX = Convert.ToInt32(Math.Sin(degreesX * Math.PI / 180) * pic_selection.Image.Height);
                int dY = Convert.ToInt32(Math.Sin(degreesY * Math.PI / 180) * pic_selection.Image.Width);

                Point[] pointsFirst = {
                new Point(pic_selection.Location.X + dX, pic_selection.Location.Y + 0),
                new Point(pic_selection.Image.Width + pic_selection.Location.X, pic_selection.Location.Y), // upper-right
                new Point(pic_selection.Location.X, pic_selection.Image.Height + pic_selection.Location.Y) // lower-left
                                            };
                Point[] pointsSecond = {
                new Point(pic_selection.Location.X + 0, pic_selection.Location.Y + dY),
                new Point(pic_selection.Image.Width + pic_selection.Location.X, pic_selection.Location.Y), // upper-right
                new Point(pic_selection.Location.X, pic_selection.Image.Height + pic_selection.Location.Y) // lower-left
                                          };
                int width = pic_selection.Size.Width;
                int height = pic_selection.Size.Height;
                int x = pic_selection.Location.X;
                int y = pic_selection.Location.Y;

                Image skewed_image = new Bitmap(pic_selection.Image);
                Image copy = new Bitmap(canvas);

                using (Graphics g = Graphics.FromImage(copy))
                {
                    g.FillRectangle(new SolidBrush(panel_canvas.BackColor), new Rectangle(x, y, width, height));
                    g.DrawImage(skewed_image, pointsFirst);
                    panel_canvas.Invalidate();
                }

                pic_selection.Width = copy.Width;
                pic_selection.Height = copy.Height;
                pic_selection.Image = copy;
            }

            else
            {
                // gather degrees from the user input
                float degreesX;
                float degreesY;
                try
                {
                    degreesX = Convert.ToSingle(txt_skew_horiz.Text);
                    if (degreesX > 89)
                    {
                        degreesX = 89;
                        txt_skew_horiz.Text = "89";
                    }

                    else if (degreesX < -89)
                    {
                        degreesX = -89;
                        txt_skew_horiz.Text = "-89";
                    }
                }
                catch (Exception)
                {
                    degreesX = 0;
                    txt_skew_horiz.Text = "0";
                }

                try
                {
                    degreesY = Convert.ToSingle(txt_skew_vert.Text);
                    if (degreesY > 89)
                    {
                        degreesY = 89;
                        txt_skew_vert.Text = "89";
                    }

                    else if (degreesY < -89)
                    {
                        degreesY = -89;
                        txt_skew_vert.Text = "-89";
                    }
                }
                catch (Exception)
                {
                    degreesY = 0;
                    txt_skew_vert.Text = "0";
                }

                degreesX /= 2;
                degreesY /= 2;

                int dX = Convert.ToInt32(Math.Tan((degreesX) * Math.PI / 180.0) * panel_canvas.Height);
                int dY = Convert.ToInt32(Math.Tan((degreesY) * Math.PI / 180.0) * panel_canvas.Width);

                Point[] x_trans = new Point[3];
                x_trans[0] = new Point(0 + dX, 0 + 0); // left + dX, top + dY
                x_trans[1] = new Point(panel_canvas.Width, 0); // right, top + dY
                x_trans[2] = new Point(0, panel_canvas.Height); // left, bottom - dY

                Point[] y_trans = new Point[3];
                y_trans[0] = new Point(0 + 0, 0 + dY); // left + dX, top + dY
                y_trans[1] = new Point(panel_canvas.Width, 0); // right, top + dY
                y_trans[2] = new Point(0, panel_canvas.Height); // left, bottom - dY

                Bitmap skewed_image = new Bitmap(canvas);

                canvas = new Bitmap(panel_canvas.Width, panel_canvas.Height);

                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImage(skewed_image, x_trans);
                    panel_canvas.Invalidate();
                }

                skewed_image = new Bitmap(canvas);

                canvas = new Bitmap(panel_canvas.Width, panel_canvas.Height);

                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImage(skewed_image, y_trans);
                    panel_canvas.Invalidate();
                }
            }
        }

        private void btn_stretch_Click(object sender, EventArgs e)
        {
            if (selected_box)
            {
                if (rad_pixels.Checked)
                {
                    // if selection is made and user wants to stretch
                    // using pixels...
                    int width, height;

                    // validate width against bad input
                    // and incorrect dimensions
                    try
                    {
                        width = Convert.ToInt32(txt_horiz.Text);
                    }
                    catch (System.FormatException)
                    {
                        width = panel_canvas.Width;
                    }
                    if (width > panel_canvas.Width)
                        width = panel_canvas.Width;
                    else if (width < 1)
                        width = 1;
                    txt_horiz.Text = width.ToString();
                    
                    // validate height against bad input
                    // and incorrect dimensions
                    try
                    {
                        height = Convert.ToInt32(txt_vertical.Text);
                    }
                    catch (System.FormatException)
                    {
                        height = panel_canvas.Height;
                    }
                    height = Convert.ToInt32(txt_vertical.Text);
                    if (height > panel_canvas.Height)
                        height = panel_canvas.Height;
                    else if (height < 1)
                        height = 1;
                    txt_vertical.Text = height.ToString();

                    // resize appropriately and redraw
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(new SolidBrush(panel_canvas.BackColor), new Rectangle(pic_selection.Location, pic_selection.Size));
                        pic_selection.Width = width;
                        pic_selection.Height = height;

                        Bitmap resized = new Bitmap(pic_selection.Image, pic_selection.Size);

                        pic_selection.Image = resized;

                        panel_canvas.Invalidate();
                    }
                }
                else
                {
                    // if selection is made and user wants to stretch 
                    // using percentages...
                     int width, height;

                     // validate width against bad input
                     // and incorrect dimensions
                    try
                    {
                        width = Convert.ToInt32(txt_horiz.Text);
                    }
                    catch (System.FormatException)
                    {
                        width = 100;
                    }
                    if (width < 1)
                        width = 1;
                    txt_horiz.Text = width.ToString();

                    // validate height against bad input
                    // and incorrect dimensions
                    try
                    {
                        height = Convert.ToInt32(txt_vertical.Text);
                    }
                    catch (System.FormatException)
                    {
                        height = 100;
                    }
                    if (height < 1)
                        height = 1;
                    txt_vertical.Text = height.ToString();

                    // create new brush/image for resized versio0n
                    Brush brush = new SolidBrush(panel_canvas.BackColor);
                    Bitmap image = new Bitmap(canvas);

                    // calculate new dimensions
                    width = Convert.ToInt32(Convert.ToSingle(width) / 100 * pic_selection.Width);
                    height = Convert.ToInt32(Convert.ToSingle(height) / 100 * pic_selection.Height);

                    Bitmap bmp = new Bitmap(canvas);

                    // resize and draw appropriately
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(new SolidBrush(panel_canvas.BackColor),
                            new RectangleF(pic_selection.Location, pic_selection.Size));
                        pic_selection.Width = width;
                        pic_selection.Height = height;

                        Bitmap resized = new Bitmap(pic_selection.Image, pic_selection.Size);

                        pic_selection.Image = resized;

                        panel_canvas.Invalidate();
                    }
                }
            }
            else
            {
                if (rad_pixels.Checked)
                {
                    // if no selection is made and user wants to
                    // stretch using pixels...
                    int width, height;

                    // validate width against bad input
                    // and incorrect dimensions
                    try
                    {
                        width = Convert.ToInt32(txt_horiz.Text);
                    }
                    catch (System.FormatException)
                    {
                        width = panel_canvas.Width;
                    }
                    if (width > panel_canvas.Width)
                        width = panel_canvas.Width;
                    else if (width < 1)
                        width = 1;
                    txt_horiz.Text = width.ToString();

                    // validate height against bad input
                    // and incorrect dimensions
                    try
                    {
                        height = Convert.ToInt32(txt_vertical.Text);
                    }
                    catch (System.FormatException)
                    {
                        height = panel_canvas.Height;
                    }
                    height = Convert.ToInt32(txt_vertical.Text);
                    if (height > panel_canvas.Height)
                        height = panel_canvas.Height;
                    else if (height < 1)
                        height = 1;
                    txt_vertical.Text = height.ToString();

                    // create brush and image for drawing resized image
                    Brush brush = new SolidBrush(panel_canvas.BackColor);
                    Bitmap image = new Bitmap(canvas);

                    // draw appropriately and update panel
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(brush, new RectangleF(0, 0, width, height));
                        g.Clear(panel_canvas.BackColor);
                        g.DrawImage(
                            image,
                            new Rectangle(
                                0,
                                0,
                                width,
                                height));
                        panel_canvas.Invalidate();
                    }
                }
                else
                {
                    int width, height;

                    // validate width against bad input
                    // and incorrect dimensions
                    try
                    {
                        width = Convert.ToInt32(txt_horiz.Text);
                    }
                    catch (System.FormatException)
                    {
                        width = 100;
                    }

                    if (width < 1)
                        width = 1;
                    txt_horiz.Text = width.ToString();

                    // validate height against bad input
                    // and incorrect dimensions
                    try
                    {
                        height = Convert.ToInt32(txt_vertical.Text);
                    }
                    catch (System.FormatException)
                    {
                        height = 100;
                    }
                    if (height < 1)
                        height = 1;
                    txt_vertical.Text = height.ToString();

                    // create brush/image for drawing resized image
                    Brush brush = new SolidBrush(panel_canvas.BackColor);
                    
                    // calculate new dimensions
                    width = Convert.ToInt32(Convert.ToSingle(width) / 100 * canvas.Width);
                    height = Convert.ToInt32(Convert.ToSingle(height) / 100 * canvas.Height);

                    Bitmap bmp = new Bitmap(canvas);

                    // draw and resize appropriately
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(brush, new RectangleF(0, 0, width, height));
                        g.Clear(panel_canvas.BackColor);
                        g.DrawImage(bmp, new Rectangle(
                            0,
                            0,
                            width,
                            height));
                        panel_canvas.Invalidate();
                    }
                }
            }
        }

        private void CurveToolUp(MouseEventArgs e)
        {
            // functions as a state machine, changing
            // states when the mouse is released
            // while using the bezier curve tool
            switch (_curve_state)
            {
                case CURVESTATES.NoClick:
                    _ending_point = new Point(e.X, e.Y);
                    _curve_state = CURVESTATES.OneClick;
                    break;
                case CURVESTATES.Drawn:
                    _curve_state = CURVESTATES.NoClick;
                    break;
            }
        }

        private void CurveToolDown(MouseEventArgs e)
        {
            // functions as a state machine for the bezier
            // curve tool, only drawing it after the correct
            // number of clicks have been made
            switch (_curve_state)
            {
                case CURVESTATES.NoClick:
                    _starting_point = new Point(e.X, e.Y);
                    break;
                case CURVESTATES.OneClick:
                    _bez_1 = new Point(e.X, e.Y);
                    _curve_state = CURVESTATES.TwoClicks;
                    break;
                case CURVESTATES.TwoClicks:
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.DrawBezier(new Pen(_color, Convert.ToSingle(updn_brush_size.Value)),
                            _starting_point, _bez_1, new Point(e.X, e.Y), _ending_point);
                        panel_canvas.Invalidate();
                    }
                    _curve_state = CURVESTATES.Drawn;
                    break;
            }
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            // set up image to copy to clipboard
            Bitmap bg = null;

            if (selected_box)
            {
                // if a selection is made, simply copy the image
                // to the clipboard
                Clipboard.SetImage(pic_selection.Image);
            }
            else
            {
                // if no selection is made, just copy the entire image
                bg = new Bitmap(panel_canvas.Width, panel_canvas.Height);

                using (Graphics g = Graphics.FromImage(bg))
                {
                    g.FillRectangle(new SolidBrush(panel_canvas.BackColor), 0, 0, panel_canvas.Width, panel_canvas.Height);
                    g.DrawImage(canvas, new Point(0, 0));
                }

                Clipboard.SetImage(bg);
            }
        }

        private void btn_paste_Click(object sender, EventArgs e)
        {
            // if there's an image in the clipboard, draw it to the origin
            if (Clipboard.GetImage() != null)
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImageUnscaled(Clipboard.GetImage(), new Point(0, 0));
                    panel_canvas.Invalidate();
                }
            }
        }

        private void btn_cut_Click(object sender, EventArgs e)
        {
            // create image to store on clipboard
            Bitmap bg = null;

            if (selected_box)
            {
                // if something is selected, copy it to clipboard
                Clipboard.SetImage(pic_selection.Image);
                
                // then clear the selection box, as it's a cut function
                pic_selection.Image = null;
            }
            else
            {
                // if nothing is selected, create image same size of the canvas
                bg = new Bitmap(panel_canvas.Width, panel_canvas.Height);

                // draw the background color on the new image, then draw canvas on it
                // finally, clear the screen, as we cut the entire canvas
                using (Graphics g = Graphics.FromImage(bg))
                using (Graphics h = panel_canvas.CreateGraphics())
                {
                    g.FillRectangle(new SolidBrush(panel_canvas.BackColor), 0, 0, panel_canvas.Width, panel_canvas.Height);
                    g.DrawImage(canvas, new Point(0, 0));
                    h.FillRectangle(new SolidBrush(panel_canvas.BackColor), 0, 0, panel_canvas.Width, panel_canvas.Height);
                    Clipboard.SetImage(bg);
                }

            }

            // redraw canvas
            panel_canvas.Invalidate();
        }

        private void btn_select_EnabledChanged(object sender, EventArgs e)
        {
            // used to draw selection box
            if (btn_select.Enabled == false)
                pic_selection.Visible = false;
        }

        private void pic_selection_MouseMove(object sender, MouseEventArgs e)
        {
            // move selection box with mouse
            if (sel_mouse_down)
            {
                pic_selection.Left += e.X;
                pic_selection.Top += e.Y;
            }
        }

        private void pic_selection_MouseDown(object sender, MouseEventArgs e)
        {
            // used to start drawing selection box
            sel_mouse_down = true;
            last_x = e.Location.X;
            last_y = e.Location.Y;
        }

        private void pic_selection_MouseUp(object sender, MouseEventArgs e)
        {
            // used to draw selection box
            sel_mouse_down = false;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // close application
            Application.Exit();
        }
    }
}
