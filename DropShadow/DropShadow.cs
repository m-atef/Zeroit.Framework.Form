﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.Form
// Author           : ZEROIT
// Created          : 11-22-2018
//
// Last Modified By : ZEROIT
// Last Modified On : 12-15-2018
// ***********************************************************************
// <copyright file="DropShadow.cs" company="Zeroit Dev Technologies">
//    This program is for creating a Form control.
//    Copyright ©  2017  Zeroit Dev Technologies
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//    You can contact me at zeroitdevnet@gmail.com or zeroitdev@outlook.com
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zeroit.Framework.Form
{
    #region DropShadow

    /// <summary>
    /// A class collection for adding a shadow to a form.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    /// <example>
    ///   <code>
    /// private void FormLoadEvent(object sender, EventArgs e)
    /// {
    /// //Make sure the code is in the FormLoadEvent. Otherwise it will experience
    /// //some errors.
    /// //Create an instance of the shadow class. Note the "this" refers to the form
    /// //the shadow will be applied to. That is the parent form.
    /// ZeroitDropshadow AddShadow = new ZeroitDropshadow(this);
    /// AddShadow.ShadowBlur = 5;               //Blur effect
    /// AddShadow.ShadowSpread = ShadowSpread;  //Shadow spread
    /// AddShadow.ShadowV = ShadowV;            //Vertical shadow location
    /// AddShadow.ShadowH = ShadowH;            //Horizontal shadow location
    /// AddShadow.ShadowColor = ShadowColor;    //Shadow Color
    /// AddShadow.ActivateShadow();             //This will activate the shadow.
    /// }
    /// </code>
    /// </example>
    public class ZeroitDropshadow : System.Windows.Forms.Form
    {

        /// <summary>
        /// The shadow bitmap
        /// </summary>
        private Bitmap _shadowBitmap;
        /// <summary>
        /// The shadow color
        /// </summary>
        private Color _shadowColor;
        /// <summary>
        /// The shadow h
        /// </summary>
        private int _shadowH;
        /// <summary>
        /// The shadow blur
        /// </summary>
        private int _shadowBlur = 10;
        /// <summary>
        /// The shadow spread
        /// </summary>
        private int _shadowSpread;
        /// <summary>
        /// The shadow opacity
        /// </summary>
        private byte _shadowOpacity = 255;
        /// <summary>
        /// The shadow v
        /// </summary>
        private int _shadowV;
        /// <summary>
        /// The draw shadow
        /// </summary>
        private bool drawShadow = true;

        //Added Code
        /// <summary>
        /// The step
        /// </summary>
        private int step = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroitDropshadow" /> class.
        /// </summary>
        /// <param name="f">The f.</param>
        public ZeroitDropshadow(System.Windows.Forms.Form f)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Owner = f;
            ShadowColor = Color.Black;

            // default style
            FormBorderStyle = FormBorderStyle.None;
            //StartPosition = FormStartPosition.CenterScreen;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;

            // bind event
            Owner.LocationChanged += UpdateLocation;
            Owner.FormClosing += (sender, eventArgs) => Close();
            Owner.VisibleChanged += (sender, eventArgs) =>
            {
                if (Owner != null)
                    Visible = Owner.Visible;
            };

            try
            {
                Owner.Activated += (sender, args) => Owner.BringToFront();
                //Owner.SizeChanged += (sender, eventArgs) =>
                //{
                //    this.Size = f.Size;
                //};
            }
            catch (Exception e)
            {
                
            }
            
            if (!DesignMode)
            {
                Owner.SizeChanged += Owner_SizeChanged;
            }
            else
            {
                Owner.SizeChanged += Owner_SizeChanged1;
            }
            
        }

        /// <summary>
        /// Handles the SizeChanged1 event of the Owner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Owner_SizeChanged1(object sender, EventArgs e)
        {
            this.Size = Owner.Size;
        }

        /// <summary>
        /// Handles the SizeChanged event of the Owner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Owner_SizeChanged(object sender = null, EventArgs e = null)
        {

            this.Size = Owner.Size;
            
            if (DrawShadow)
                RefreshShadow();

            
        }

        /// <summary>
        /// Gets or sets a value indicating whether to draw shadow.
        /// </summary>
        /// <value><c>true</c> if draw shadow; otherwise, <c>false</c>.</value>
        public bool DrawShadow
        {
            get { return drawShadow; }
            set
            {
                drawShadow = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>The color of the shadow.</value>
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                _shadowColor = value;
                RefreshShadow();
                _shadowOpacity = _shadowColor.A;
            }
        }

        /// <summary>
        /// Gets or sets the shadow bitmap.
        /// </summary>
        /// <value>The shadow bitmap.</value>
        public Bitmap ShadowBitmap
        {
            get { return _shadowBitmap; }
            set
            {
                _shadowBitmap = value;
                SetBitmap(_shadowBitmap, ShadowOpacity);
            }
        }

        /// <summary>
        /// Gets or sets the shadow opacity.
        /// </summary>
        /// <value>The shadow opacity.</value>
        public byte ShadowOpacity
        {
            get { return _shadowOpacity; }
            set
            {
                _shadowOpacity = value;
                SetBitmap(ShadowBitmap, _shadowOpacity);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position of the shadow.
        /// </summary>
        /// <value>The shadow h.</value>
        public int ShadowH
        {
            get { return _shadowH; }
            set
            {
                _shadowH = value;
                RefreshShadow(false);
            }
        }

        /// <summary>
        /// Offset X relate to Owner
        /// </summary>
        /// <value>The offset x.</value>
        public int OffsetX
        {
            get { return ShadowH - (ShadowBlur + ShadowSpread); }
        }

        /// <summary>
        /// Offset Y relate to Owner
        /// </summary>
        /// <value>The offset y.</value>
        public int OffsetY
        {
            get { return ShadowV - (ShadowBlur + ShadowSpread); }
        }

        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        /// <value>The width.</value>
        public new int Width
        {
            get { return Owner.Width + (ShadowSpread + ShadowBlur) * 2; }
        }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        /// <value>The height.</value>
        public new int Height
        {
            get { return Owner.Height + (ShadowSpread + ShadowBlur) * 2; }
        }

        /// <summary>
        /// Gets or sets the vertical location of the shadow.
        /// </summary>
        /// <value>The shadow v.</value>
        public int ShadowV
        {
            get { return _shadowV; }
            set
            {
                _shadowV = value;
                RefreshShadow(false);
            }
        }

        /// <summary>
        /// Gets or sets the shadow blur.
        /// </summary>
        /// <value>The shadow blur.</value>
        public int ShadowBlur
        {
            get { return this._shadowBlur; }
            set
            {
                this._shadowBlur = value;
                RefreshShadow();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets how the shadow should spread.
        /// </summary>
        /// <value>The shadow spread.</value>
        public int ShadowSpread
        {
            get { return this._shadowSpread; }
            set
            {
                this._shadowSpread = value;
                RefreshShadow();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the shadow step.
        /// </summary>
        /// <value>The shadow step.</value>
        public int ShadowStep
        {
            get { return this.step; }
            set
            {
                this.step = value;
                RefreshShadow();
                this.Invalidate();
            }
        }

        #region Resize Code by Zeroit
        //private int cCaption = 16;
        //private int cGrip = 32;
        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x84)
        //    {  // Trap WM_NCHITTEST
        //        Point pos = new Point(m.LParam.ToInt32());
        //        pos = this.PointToClient(pos);
        //        if (pos.Y < cCaption)
        //        {
        //            m.Result = (IntPtr)2;  // HTCAPTION
        //            return;
        //        }
        //        if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
        //        {
        //            m.Result = (IntPtr)17; // HTBOTTOMRIGHT
        //            return;
        //        }
        //    }
        //    base.WndProc(ref m);
        //} 
        #endregion

        /// <summary>
        /// Activates the shadow.
        /// </summary>
        public void ActivateShadow()
        {
            if (!DesignMode)
            {
                ZeroitDropshadow shadow = new ZeroitDropshadow(Owner)
                {
                    ShadowBlur = _shadowBlur,
                    ShadowSpread = _shadowSpread,
                    ShadowColor = _shadowColor,
                    //ShadowOpacity = _shadowOpacity,
                    ShadowH = _shadowH,
                    ShadowV = _shadowV,

                };
                shadow.RefreshShadow();
            }


        }

        /// <summary>
        /// Deactivates the shadow.
        /// </summary>
        public void DeActivateShadow()
        {
            if (!DesignMode)
            {
                ZeroitDropshadow shadow = new ZeroitDropshadow(Owner)
                {
                    ShadowBlur = _shadowBlur = 0,
                    ShadowSpread = _shadowSpread,
                    ShadowColor = _shadowColor,
                    //ShadowOpacity = _shadowOpacity = 255,
                    ShadowH = _shadowH,
                    ShadowV = _shadowV,

                };
                shadow.RefreshShadow();
            }
        }

        /// <summary>
        /// Gets the create parameters.
        /// </summary>
        /// <value>The create parameters.</value>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }

        /// <summary>
        /// Draws the shadow bitmap.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="borderRadius">The border radius.</param>
        /// <param name="blur">The blur.</param>
        /// <param name="spread">The spread.</param>
        /// <param name="color">The color.</param>
        /// <returns>Bitmap.</returns>
        private static Bitmap DrawShadowBitmap(int width, int height, int borderRadius, int blur, int spread, Color color)
        {
            int ex = blur + spread;
            int w = width + ex * 2;
            int h = height + ex * 2;
            int solidW = width + spread * 2;
            int solidH = height + spread * 2;

            Bitmap bitmap = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            // fill background
            g.FillRectangle(new SolidBrush(color)
                , blur, blur, width + spread * 2 + 1, height + spread * 2 + 1);
            // +1 to fill the gap

            if (blur > 0)
            {
                // four dir gradiant
                {
                    // left
                    var brush = new LinearGradientBrush(new Point(0, 0), new Point(blur, 0), Color.Transparent, color);
                    // will thorw ArgumentException
                    // brush.WrapMode = WrapMode.Clamp; 


                    g.FillRectangle(brush, 0, blur, blur, solidH);
                    // up
                    brush.RotateTransform(90);
                    g.FillRectangle(brush, blur, 0, solidW, blur);

                    // right
                    // make sure parttern is currect
                    brush.ResetTransform();
                    brush.TranslateTransform(w % blur, h % blur);

                    brush.RotateTransform(180);
                    g.FillRectangle(brush, w - blur, blur, blur, solidH);
                    // down
                    brush.RotateTransform(90);
                    g.FillRectangle(brush, blur, h - blur, solidW, blur);
                }


                // four corner
                {
                    var gp = new GraphicsPath();
                    //gp.AddPie(0,0,blur*2,blur*2, 180, 90);
                    gp.AddEllipse(0, 0, blur * 2, blur * 2);


                    var pgb = new PathGradientBrush(gp);
                    pgb.CenterColor = color;
                    pgb.SurroundColors = new[] { Color.Transparent };
                    pgb.CenterPoint = new Point(blur, blur);

                    // lt
                    g.FillPie(pgb, 0, 0, blur * 2, blur * 2, 180, 90);
                    // rt
                    var matrix = new Matrix();
                    matrix.Translate(w - blur * 2, 0);

                    pgb.Transform = matrix;
                    //pgb.Transform.Translate(w-blur*2, 0);
                    g.FillPie(pgb, w - blur * 2, 0, blur * 2, blur * 2, 270, 90);
                    // rb
                    matrix.Translate(0, h - blur * 2);
                    pgb.Transform = matrix;
                    g.FillPie(pgb, w - blur * 2, h - blur * 2, blur * 2, blur * 2, 0, 90);
                    // lb
                    matrix.Reset();
                    matrix.Translate(0, h - blur * 2);
                    pgb.Transform = matrix;
                    g.FillPie(pgb, 0, h - blur * 2, blur * 2, blur * 2, 90, 90);
                }
            }

            g.Dispose();
            GC.Collect();

            return bitmap;
            

        }

        /// <summary>
        /// Updates the location.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UpdateLocation(Object sender = null, EventArgs eventArgs = null)
        {
            Point pos = Owner.Location;

            pos.Offset(OffsetX, OffsetY);
            Location = pos;
        }

        /// <summary>
        /// Refresh shadow.
        /// </summary>
        /// <param name="redraw">(optional) redraw the background bitmap.</param>
        public void RefreshShadow(bool redraw = true)
        {
            if (redraw)
            {
                //ShadowBitmap = DrawShadow();

                if (DrawShadow)
                {
                    ShadowBitmap = DrawShadowBitmap(Owner.Width, Owner.Height, 0, ShadowBlur, ShadowSpread, ShadowColor);

                }
                
            }

            //SetBitmap(ShadowBitmap, ShadowOpacity);
            UpdateLocation();

            //Region r = Region.FromHrgn(Win32.CreateRoundRectRgn(0, 0, Width, Height, BorderRadius, BorderRadius));
            var r = new Region(new Rectangle(0, 0, Width, Height));
            Region or;
            if (Owner.Region == null)
                or = new Region(Owner.ClientRectangle);
            else
                or = Owner.Region.Clone();

            or.Translate(-OffsetX, -OffsetY);
            r.Exclude(or);
            Region = r;

            Owner.Refresh();
        }

        /// <summary>
        /// Sets the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="opacity">The opacity.</param>
        /// <exception cref="System.ApplicationException">The bitmap must be 32ppp with alpha-channel.</exception>
        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        private void SetBitmap(Bitmap bitmap, byte opacity = 255)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0)); // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                var size = new Win32.Size(bitmap.Width, bitmap.Height);
                var pointSource = new Win32.Point(0, 0);
                var topPos = new Win32.Point(Left, Top);
                var blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend,
                    Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }


        
    }


    // class that exposes needed win32 gdi functions.
    /// <summary>
    /// Class Win32.
    /// </summary>
    internal static class Win32
    {
        /// <summary>
        /// Enum related to implementing shadow capabilities for <c><see cref="ZeroitDropshadow" /></c>
        /// </summary>
        public enum Bool
        {
            /// <summary>
            /// The false
            /// </summary>
            False = 0,
            /// <summary>
            /// The true
            /// </summary>
            True
        };

        /// <summary>
        /// The ulw colorkey
        /// </summary>
        public const Int32 ULW_COLORKEY = 0x00000001;
        /// <summary>
        /// The ulw alpha
        /// </summary>
        public const Int32 ULW_ALPHA = 0x00000002;
        /// <summary>
        /// The ulw opaque
        /// </summary>
        public const Int32 ULW_OPAQUE = 0x00000004;

        /// <summary>
        /// The ac source over
        /// </summary>
        public const byte AC_SRC_OVER = 0x00;
        /// <summary>
        /// The ac source alpha
        /// </summary>
        public const byte AC_SRC_ALPHA = 0x01;

        /// <summary>
        /// Creates the round rect RGN.
        /// </summary>
        /// <param name="nLeftRect">The n left rect.</param>
        /// <param name="nTopRect">The n top rect.</param>
        /// <param name="nRightRect">The n right rect.</param>
        /// <param name="nBottomRect">The n bottom rect.</param>
        /// <param name="nWidthEllipse">The n width ellipse.</param>
        /// <param name="nHeightEllipse">The n height ellipse.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
            (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
            );

        /// <summary>
        /// Gets the window long.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified
        /// offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
        /// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the
        /// number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the
        /// following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC</param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        /// <summary>
        /// Updates the layered window.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="hdcDst">The HDC DST.</param>
        /// <param name="pptDst">The PPT DST.</param>
        /// <param name="psize">The psize.</param>
        /// <param name="hdcSrc">The HDC source.</param>
        /// <param name="pprSrc">The PPR source.</param>
        /// <param name="crKey">The cr key.</param>
        /// <param name="pblend">The pblend.</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <returns>Bool.</returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize,
            IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        /// <summary>
        /// Gets the dc.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Releases the dc.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hDC">The h dc.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// Creates the compatible dc.
        /// </summary>
        /// <param name="hDC">The h dc.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        /// <summary>
        /// Deletes the dc.
        /// </summary>
        /// <param name="hdc">The HDC.</param>
        /// <returns>Bool.</returns>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        /// <summary>
        /// Selects the object.
        /// </summary>
        /// <param name="hDC">The h dc.</param>
        /// <param name="hObject">The h object.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="hObject">The h object.</param>
        /// <returns>Bool.</returns>
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Struct ARGB
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ARGB
        {
            /// <summary>
            /// The blue
            /// </summary>
            public readonly byte Blue;
            /// <summary>
            /// The green
            /// </summary>
            public readonly byte Green;
            /// <summary>
            /// The red
            /// </summary>
            public readonly byte Red;
            /// <summary>
            /// The alpha
            /// </summary>
            public readonly byte Alpha;
        }


        /// <summary>
        /// Struct BLENDFUNCTION
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            /// <summary>
            /// The blend op
            /// </summary>
            public byte BlendOp;
            /// <summary>
            /// The blend flags
            /// </summary>
            public byte BlendFlags;
            /// <summary>
            /// The source constant alpha
            /// </summary>
            public byte SourceConstantAlpha;
            /// <summary>
            /// The alpha format
            /// </summary>
            public byte AlphaFormat;
        }

        /// <summary>
        /// Struct Point
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            /// <summary>
            /// The x
            /// </summary>
            public Int32 x;
            /// <summary>
            /// The y
            /// </summary>
            public Int32 y;

            /// <summary>
            /// Initializes a new instance of the <see cref="Point"/> struct.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            public Point(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }


        /// <summary>
        /// Struct Size
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            /// <summary>
            /// The cx
            /// </summary>
            public Int32 cx;
            /// <summary>
            /// The cy
            /// </summary>
            public Int32 cy;

            /// <summary>
            /// Initializes a new instance of the <see cref="Size"/> struct.
            /// </summary>
            /// <param name="cx">The cx.</param>
            /// <param name="cy">The cy.</param>
            public Size(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }
    }

    #endregion
}
