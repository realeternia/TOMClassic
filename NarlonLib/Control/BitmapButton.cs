using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NarlonLib.Control
{
    /// <summary>
    /// possible button states
    /// </summary>
    public enum BtnState
    {
        /// <summary>
        /// The button is disabled.
        /// </summary>		
        Inactive = 0,
        /// <summary>
        /// The button is in it normal unpressed state
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The location of the mouse is over the button
        /// </summary>
        MouseOver = 2,
        /// <summary>
        /// The button is currently being pressed
        /// </summary>
        Pushed = 3,
    }
    /// <summary>
    /// The purpose of this class is to continue to provide the regular functionality of button class with
    /// some additional bitmap enhancments. These enhancements allow the specification of bitmaps for each 
    /// state of the button. In addition, it makes use of the alignment properties already provided by the 
    /// base button class. Since this class renders the image, it should appear similar accross platforms.	
    /// </summary>
    public class BitmapButton : Button
    {
        #region Private Variables

        private Image _ImageNormal = null;
        private Color _BorderColor = Color.Black;
        private bool _TextDropShadow = true;
        private bool _OffsetPressedContent = true;
        private BtnState btnState = BtnState.Normal;
        private bool CapturingMouse = false;
        private ToolTip tooltip = new ToolTip();

        #endregion
        #region Public Properties


        [Browsable(false)]
        new public ImageList ImageList
        {
            get { return base.ImageList; }
            set { base.ImageList = value; }
        }
        [Browsable(false)]
        new public int ImageIndex
        {
            get { return base.ImageIndex; }
            set { base.ImageIndex = value; }
        }
        /// <summary>
        /// Enable the shadowing of the button text
        /// </summary>
        [Browsable(true),
        CategoryAttribute("Appearance"),
        Description("enables the text to cast a shadow"),
        System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)
        ]
        public bool TextDropShadow
        {
            get { return _TextDropShadow; }
            //set{_TextDropShadow = value;}
        }
        /// <summary>
        /// Color of the border around the button
        /// </summary>
		[Browsable(true),
        CategoryAttribute("Appearance"),
        Description("Color of the border around the button"),
        System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)
        ]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }
        /// <summary>
        /// Set to true if to offset button elements when button is pressed
        /// </summary>
        [Browsable(true),
        CategoryAttribute("Appearance"),
        Description("Set to true if to offset image/text when button is pressed"),
        System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)
        ]
        public bool OffsetPressedContent
        {
            get
            {
                return
                    _OffsetPressedContent;
            }
            //set{_OffsetPressedContent = value;}
        }
        /// <summary>
        /// Image to be displayed while the button state is in normal state. If the other
        /// states do not specify an image, this image is used as a substitute.
        /// </summary>
        [Browsable(true),
        CategoryAttribute("Appearance"),
        Description("Image to be displayed while the button state is in normal state"),
        System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)
        ]
        public Image ImageNormal
        {
            get { return _ImageNormal; }
            set { _ImageNormal = value; }
        }

        public Image IconImage { get; set; }

        public Point IconXY { get; set; }
        public Size IconSize { get; set; }

        public int TextOffX { get; set; }

        public bool NoUseDrawNine { get; set; }

        #endregion
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        /// <summary>
        /// The BitmapButton constructor
        /// </summary>
        public BitmapButton()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            // TODO: Add any initialization after the InitComponent call			
            tooltip.BackColor = Color.FromArgb(215, 210, 200);
            IconXY = new Point(0);
            //LoadGraphics();
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion


        #region Paint Methods
        /// <summary>
        /// This method paints the button in its entirety.
        /// </summary>
        /// <param name="e">paint arguments use to paint the button</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //CreateRegion(0);			
            // paint_Background(e);
            if (ImageNormal == null)//编辑器模式使用
            {
                var brush = new SolidBrush(Color.FromArgb(ForeColor.A, (ForeColor.R + 100) % 255, (ForeColor.G + 100) % 255, (ForeColor.B + 100) % 255));
                e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
                brush.Dispose();
                e.Graphics.DrawRectangle(Pens.Wheat, 0, 0, Width-1, Height-1);
            }
          
            paint_Image(e);
            paint_Text(e);
            // paint_Border(e);
            // paint_InnerBorder(e);
            //  paint_FocusBorder(e);
        }
        /// <summary>
        /// This method paints the text and text shadow for the button.
        /// </summary>
        /// <param name="e">paint arguments use to paint the button</param>		 
        private void paint_Text(PaintEventArgs e)
        {
            if (e == null)
                return;

            Rectangle rect = GetTextDestinationRect();
            //
            // do offset if button is pushed
            //
            if ((btnState == BtnState.Pushed) && (OffsetPressedContent))
                rect.Offset(1, 1);
            //
            // caculate bounding rectagle for the text
            //
            SizeF size = txt_Size(e.Graphics, this.Text, this.Font);
            //
            // calculate the starting location to paint the text
            //
            Point pt = Calculate_LeftEdgeTopEdge(this.TextAlign, rect, (int)size.Width, (int)size.Height);
            pt.X += TextOffX;
            pt.Y += 1;//微调
            //
            // If button state is inactive, paint the inactive text
            //
            if (btnState == BtnState.Inactive)
            {
                e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(Color.White), pt.X + 1, pt.Y + 1);
                e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(Color.FromArgb(50, 50, 50)), pt.X, pt.Y);
            }
            //
            // else, paint the text and text shadow
            //
            else
            {
                //
                // paint text shadow
                //
                if (TextDropShadow)
                {
                    Brush TransparentBrush0 = new SolidBrush(Color.FromArgb(50, Color.Black));
                    Brush TransparentBrush1 = new SolidBrush(Color.FromArgb(20, Color.Black));

                    e.Graphics.DrawString(this.Text, this.Font, TransparentBrush0, pt.X, pt.Y + 1);
                    e.Graphics.DrawString(this.Text, this.Font, TransparentBrush0, pt.X + 1, pt.Y);

                    e.Graphics.DrawString(this.Text, this.Font, TransparentBrush1, pt.X + 1, pt.Y + 1);
                    e.Graphics.DrawString(this.Text, this.Font, TransparentBrush1, pt.X, pt.Y + 2);
                    e.Graphics.DrawString(this.Text, this.Font, TransparentBrush1, pt.X + 2, pt.Y);

                    TransparentBrush0.Dispose();
                    TransparentBrush1.Dispose();
                }
                //
                // paint text
                //
                e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), pt.X, pt.Y);
            }
        }
        /// <summary>
        /// Paints the image on the button.
        /// </summary>
        /// <param name="e"></param>

        private void paint_Image(PaintEventArgs e)
        {
            if (e == null)
                return;

            Image image = GetCurrentImage(ImageNormal);

            if (image != null)
            {
                Graphics g = e.Graphics;
                Rectangle rect = GetImageDestinationRect();
                Point imgOff = IconXY;

                if ((btnState == BtnState.Pushed) && (_OffsetPressedContent))
                {
                    rect.Offset(1, 1);
                    imgOff.Offset(1, 1);
                }

                if (NoUseDrawNine)
                {
                    g.DrawImage(image, rect, 0,0,image.Width,image.Height, GraphicsUnit.Pixel);
                }
                else//默认用九宫格画
                {
                    DrawNine(g, image, rect); 
                }

                if (IconImage != null)
                {
                    var icon = GetCurrentImage(IconImage);
                    if (IconSize.IsEmpty)
                    {
                        g.DrawImageUnscaled(icon, imgOff);
                    }
                    else
                    {
                        g.DrawImage(icon, new Rectangle(imgOff.X, imgOff.Y, IconSize.Width, IconSize.Height), 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel);
                    }
                }

                if (btnState == BtnState.Pushed)
                {
                    var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
                else if (btnState == BtnState.MouseOver)
                {
                    var brush = new SolidBrush(Color.FromArgb(30, Color.Yellow));
                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                }
            }
        }

        private void DrawNine(Graphics g, Image img, Rectangle rect)
        {
            float pieceX = img.Width / 3.0f;
            float pieceY = img.Height / 3.0f;

            g.DrawImage(img, new RectangleF(rect.X, rect.Y, pieceX, pieceY), new RectangleF(0, 0, pieceX, pieceY), GraphicsUnit.Pixel);
            g.DrawImage(img, new RectangleF(rect.X + pieceX, rect.Y, rect.Width - 2 * pieceX, pieceY), new RectangleF(pieceX, 0, pieceX, pieceY), GraphicsUnit.Pixel);
            g.DrawImage(img, new RectangleF(rect.X + rect.Width - pieceX, rect.Y, pieceX, pieceY), new RectangleF(pieceX * 2, 0, pieceX, pieceY), GraphicsUnit.Pixel);

            g.DrawImage(img, new RectangleF(rect.X, rect.Y + pieceY, pieceX, rect.Height - 2 * pieceY), new RectangleF(0, pieceY, pieceX, pieceY), GraphicsUnit.Pixel);
            g.DrawImage(img, new RectangleF(rect.X + pieceX, rect.Y + pieceY, rect.Width - 2 * pieceX, rect.Height - 2 * pieceY), new RectangleF(pieceX, pieceY, pieceX, pieceY), GraphicsUnit.Pixel);
            g.DrawImage(img, new RectangleF(rect.X + rect.Width - pieceX, rect.Y + pieceY, pieceX, rect.Height - 2 * pieceY), new RectangleF(pieceX * 2, pieceY, pieceX, pieceY), GraphicsUnit.Pixel);

            g.DrawImage(img, new RectangleF(rect.X, rect.Y + rect.Height - pieceY, pieceX, pieceY), new RectangleF(0, pieceY * 2, pieceX, pieceY), GraphicsUnit.Pixel);
            g.DrawImage(img, new RectangleF(rect.X + pieceX, rect.Y + rect.Height - pieceY, rect.Width - 2 * pieceX, pieceY), new RectangleF(pieceX, pieceY * 2, pieceX, pieceY), GraphicsUnit.Pixel);
            g.DrawImage(img, new RectangleF(rect.X + rect.Width - pieceX, rect.Y + rect.Height - pieceY, pieceX, pieceY), new RectangleF(pieceX * 2, pieceY * 2, pieceX, pieceY), GraphicsUnit.Pixel);
        }
        #endregion
        #region Helper Methods
        /// <summary>
        /// Calculates the required size to draw a text string
        /// </summary>
        /// <param name="g">the graphics object</param>
        /// <param name="strText">string to calculate text region</param>
        /// <param name="font">font to use for the string</param>
        /// <returns>returns the size required to draw a text string</returns>
        private SizeF txt_Size(Graphics g, string strText, Font font)
        {
            SizeF size = g.MeasureString(strText, font);
            return (size);
        }
        /// <summary>
        /// Calculates the rectangular region used for text display.
        /// </summary>
        /// <returns>returns the rectangular region for the text display</returns>
        private Rectangle GetTextDestinationRect()
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);
            rect = new Rectangle(0, 0, this.Width, Height);
            return (rect);
        }
        /// <summary>
        /// Calculates the rectangular region used for image display.
        /// </summary>
        /// <returns>returns the rectangular region used to display the image</returns>
        private Rectangle GetImageDestinationRect()
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);
            Image image = GetCurrentImage(ImageNormal);
            if (image != null)
            {
                rect.Width = this.Width;
                rect.Height = this.Height;
            }
            return (rect);
        }
        /// <summary>
        /// This method is used to retrieve the image used by the button for the given state.
        /// </summary>
        /// <returns>returns the button Image</returns>
        private Image GetCurrentImage(Image img)
        {
            if (btnState == BtnState.Inactive)
            {
                return ConvertToGrayscale((Bitmap)img);
            }
            return img;
        }
        /// <summary>
        /// converts a bitmap image to grayscale
        /// </summary>
        /// <param name="source">bitmap source</param>
        /// <returns>returns a grayscaled bitmap</returns>
        public Bitmap ConvertToGrayscale(Bitmap source)
        {
            Bitmap bm = new Bitmap(source.Width, source.Height);
            float[][] matrix = {
                new   float[]   {0.299f,   0.299f,   0.299f,   0,   0},
                new   float[]   {0.587f,   0.587f,   0.587f,   0,   0},
                new   float[]   {0.114f,   0.114f,   0.114f,   0,   0},
                new   float[]   {0,   0,   0,   1,   0},
                new   float[]   {0,   0,   0,   0,   1}
            };
            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix(matrix);
            System.Drawing.Imaging.ImageAttributes attr = new System.Drawing.Imaging.ImageAttributes();
            attr.SetColorMatrix(cm);
            //Image tmp
            Graphics g = Graphics.FromImage(bm);
            try
            {
                Rectangle destRect = new Rectangle(0, 0, bm.Width, bm.Height);
                g.DrawImage(source, destRect, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel, attr);
            }
            finally
            {
                g.Dispose();
            }
            return bm;
        }
        /// <summary>
        /// calculates the left/top edge for content.
        /// </summary>
        /// <param name="Alignment">the alignment of the content</param>
        /// <param name="rect">rectagular region to place content</param>
        /// <param name="nWidth">with of content</param>
        /// <param name="nHeight">height of content</param>
        /// <returns>returns the left/top edge to place content</returns>
        private Point Calculate_LeftEdgeTopEdge(ContentAlignment Alignment, Rectangle rect, int nWidth, int nHeight)
        {
            Point pt = new Point(0, 0);
            switch (Alignment)
            {
                case ContentAlignment.BottomCenter:
                    pt.X = (rect.Width - nWidth) / 2;
                    pt.Y = rect.Height - nHeight;
                    break;
                case ContentAlignment.BottomLeft:
                    pt.X = 0;
                    pt.Y = rect.Height - nHeight;
                    break;
                case ContentAlignment.BottomRight:
                    pt.X = rect.Width - nWidth;
                    pt.Y = rect.Height - nHeight;
                    break;
                case ContentAlignment.MiddleCenter:
                    pt.X = (rect.Width - nWidth) / 2;
                    pt.Y = (rect.Height - nHeight) / 2;
                    break;
                case ContentAlignment.MiddleLeft:
                    pt.X = 0;
                    pt.Y = (rect.Height - nHeight) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    pt.X = rect.Width - nWidth;
                    pt.Y = (rect.Height - nHeight) / 2;
                    break;
                case ContentAlignment.TopCenter:
                    pt.X = (rect.Width - nWidth) / 2;
                    pt.Y = 0;
                    break;
                case ContentAlignment.TopLeft:
                    pt.X = 0;
                    pt.Y = 0;
                    break;
                case ContentAlignment.TopRight:
                    pt.X = rect.Width - nWidth;
                    pt.Y = 0;
                    break;
            }
            pt.X += rect.Left;
            pt.Y += rect.Top;
            return (pt);
        }
        #endregion
        #region Events Methods
        /// <summary>
        /// Mouse Down Event:
        /// set BtnState to Pushed and Capturing mouse to true
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Capture = true;
            this.CapturingMouse = true;
            btnState = BtnState.Pushed;
            this.Invalidate();
        }
        /// <summary>
        /// Mouse Up Event:
        /// Set BtnState to Normal and set CapturingMouse to false
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (btnState != BtnState.Inactive)
            {
                btnState = BtnState.Normal;
                this.CapturingMouse = false;
                this.Capture = false;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Mouse Leave Event:
        /// Set BtnState to normal if we CapturingMouse = true
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!CapturingMouse && btnState != BtnState.Inactive)
            {
                btnState = BtnState.Normal;
                this.Invalidate();
            }
            tooltip.Hide(this);
        }
        /// <summary>
        /// Mouse Move Event:
        /// If CapturingMouse = true and mouse coordinates are within button region, 
        /// set BtnState to Pushed, otherwise set BtnState to Normal.
        /// If CapturingMouse = false, then set BtnState to MouseOver
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (CapturingMouse)
            {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                BtnState oldState = btnState;
                btnState = BtnState.Normal;
                if ((e.X >= rect.Left) && (e.X <= rect.Right))
                {
                    if ((e.Y >= rect.Top) && (e.Y <= rect.Bottom))
                    {
                        btnState = BtnState.Pushed;
                    }
                }
                this.Capture = true;
                if (btnState != oldState)
                    this.Invalidate();
            }
            else
            {
                //if(!this.Focused)
                {
                    if (btnState != BtnState.MouseOver && btnState != BtnState.Inactive)
                    {
                        btnState = BtnState.MouseOver;
                        //	this.Invalidate();
                    }
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
          //  tooltip.Show(this.Text, this, this.Width + 2, 5, 5000);
        }
        /// <summary>
        /// Enable/Disable Event:
        /// If button became enabled, set BtnState to Normal
        /// else set BtnState to Inactive
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (this.Enabled)
            {
                this.btnState = BtnState.Normal;
                this.CapturingMouse = false;
                this.Capture = false;
            }
            else
            {
                this.btnState = BtnState.Inactive;
            }
            this.Invalidate();
        }
        /// <summary>
        /// Lose Focus Event:
        /// set btnState to Normal
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (this.Enabled)
            {
                this.btnState = BtnState.Normal;
            }
            this.Invalidate();
        }


        #endregion
    }
}
