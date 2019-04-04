﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.Form
// Author           : ZEROIT
// Created          : 11-24-2018
//
// Last Modified By : ZEROIT
// Last Modified On : 12-15-2018
// ***********************************************************************
// <copyright file="MetroForm.cs" company="Zeroit Dev Technologies">
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
using System.Windows.Forms;

namespace Zeroit.Framework.Form.Metro
{
    /// <summary>
    /// Class MetroForm.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public class MetroForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// The glow
        /// </summary>
        public MetroFormGlow glow = null;
        /// <summary>
        /// The color schema
        /// </summary>
        public ColorSchema _ColorSchema = new ColorSchema();

        /// <summary>
        /// Gets or sets the color schema.
        /// </summary>
        /// <value>The color schema.</value>
        public ColorSchema ColorSchema
        {
            get { return _ColorSchema; }
            set
            {
                _ColorSchema = value;
                this.ColorSchema.Update();
                this.Invalidate();
                foreach (Control c in this.Controls) c.Invalidate();
                this.glow.Render();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetroForm"/> class.
        /// </summary>
        public MetroForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage, true);

            SetBounds();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

            //glow = new MetroFormGlow(this);
            //glow.Show();
            //glow.Render();
            //this.Focus();
        }

        //protected override void OnGotFocus(EventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    glow.Show();
        //    glow.Render();

        //    //glow.BringToFront();
        //    this.BringToFront();
        //}

        //protected override void OnLostFocus(EventArgs e)
        //{
        //    base.OnLostFocus(e);
        //    glow.Hide();
        //}

        /// <summary>
        /// The caption bounds
        /// </summary>
        Rectangle CaptionBounds, CaptionDragBounds, WindowBounds, btnClose, btnMaximize, btnMinimize;
        /// <summary>
        /// The buttonsize
        /// </summary>
        const int buttonsize = 8, buttonwidth = 34, buttonxmid = (buttonwidth / 2 + buttonsize / 2);
        /// <summary>
        /// Sets the bounds.
        /// </summary>
        void SetBounds()
        {
            CaptionBounds = new Rectangle(0, 0, this.Width, 30);
            CaptionDragBounds = new Rectangle(0, 0, CaptionBounds.Width - (buttonwidth * capcontrols.Length), CaptionBounds.Height);
            WindowBounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            btnClose = new Rectangle(CaptionBounds.Width - buttonwidth, 0, buttonwidth, CaptionBounds.Height);
            btnMaximize = new Rectangle(CaptionBounds.Width - buttonwidth * 2, 0, buttonwidth, CaptionBounds.Height);
            btnMinimize = new Rectangle(CaptionBounds.Width - buttonwidth * 3, 0, buttonwidth, CaptionBounds.Height);
        }
        /// <summary>
        /// Handles the <see cref="E:Resize" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            SetBounds();

            base.OnResize(e);

            this.Invalidate();
            if (glow != null)
            {
                if (this.WindowState == FormWindowState.Maximized) glow.WindowState = FormWindowState.Minimized;
                else if (glow.WindowState == FormWindowState.Minimized) glow.WindowState = FormWindowState.Normal;
                else glow.Render();
            }
        }
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LocationChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (glow != null) glow.UpdatePosition();
        }

        /// <summary>
        /// The f caption
        /// </summary>
        Font fCaption = new Font("Microsoft Sans Serif", 10.25f, FontStyle.Regular);
        /// <summary>
        /// Handles the <see cref="E:Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(ColorSchema.bCaption, CaptionBounds);
            e.Graphics.DrawRectangle(ColorSchema.pBorder, WindowBounds);

            TextRenderer.DrawText(e.Graphics, this.Text, fCaption, new Rectangle(9, 0, CaptionDragBounds.Width - 9, CaptionBounds.Height), ColorSchema.cSecondary, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            int midy = (int)Math.Round(CaptionBounds.Height / 2f - 5f);
            int midys = midy + 1;

            /* Close button */
            int closex = this.Width - buttonxmid - 1;
            if (capcontrols[0] == 2)
            {
                e.Graphics.FillRectangle(ColorSchema.bCaptionControlsActive, btnClose);

                e.Graphics.DrawLine(ColorSchema.pCaptionControlsActive, closex, midy, closex + buttonsize, midy + buttonsize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsActive, closex, midy + buttonsize, closex + buttonsize, midy);
            }
            else
            {
                if (capcontrols[0] == 1)
                    e.Graphics.FillRectangle(ColorSchema.bCaptionControlsHover, btnClose);
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsShadow, closex, midys, closex + buttonsize, midys + buttonsize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsShadow, closex, midys + buttonsize, closex + buttonsize, midys);

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawLine(ColorSchema.pCaptionControls, closex, midy, closex + buttonsize, midy + buttonsize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControls, closex, midy + buttonsize, closex + buttonsize, midy);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            }

            /* Maximize button */
            int maximizex = this.Width - buttonwidth - buttonxmid;
            if (capcontrols[1] == 2)
            {
                e.Graphics.FillRectangle(ColorSchema.bCaptionControlsActive, btnMaximize);

                e.Graphics.DrawRectangle(ColorSchema.pCaptionControlsActive, maximizex, midys, 8, 8);
            }
            else
            {
                if (capcontrols[1] == 1)
                    e.Graphics.FillRectangle(ColorSchema.bCaptionControlsHover, btnMaximize);
                e.Graphics.DrawRectangle(ColorSchema.pCaptionControlsShadow, maximizex, midys + 1, 8, 8);

                e.Graphics.DrawRectangle(ColorSchema.pCaptionControls, maximizex, midys, 8, 8);
            }

            /* Minimize button */
            int minimizex = this.Width - buttonwidth * 2 - buttonxmid;
            if (capcontrols[2] == 2)
            {
                e.Graphics.FillRectangle(ColorSchema.bCaptionControlsActive, btnMinimize);

                e.Graphics.DrawLine(ColorSchema.pCaptionControlsActive, minimizex, midys + 8, minimizex + 8, midys + 8);
            }
            else
            {
                if (capcontrols[2] == 1)
                    e.Graphics.FillRectangle(ColorSchema.bCaptionControlsHover, btnMinimize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsShadow, minimizex, midys + 9, minimizex + 8, midys + 8);

                e.Graphics.DrawLine(ColorSchema.pCaptionControls, minimizex, midys + 8, minimizex + 8, midys + 8);
            }
        }

        /// <summary>
        /// The capcontrols
        /// </summary>
        int[] capcontrols = new int[3];
        /// <summary>
        /// Updates the caption control.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="state">The state.</param>
        void UpdateCaptionControl(int index, int state)
        {
            if (state <= capcontrols[index] && state != 0) return;
            
            this.Invalidate();
            capcontrols[index] = state;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            e = new MouseEventArgs(e.Button, e.Clicks, e.X - 9, e.Y - 30, e.Delta);

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(ColorSchema.bWindow, e.Graphics.ClipBounds);
            //base.OnPaintBackground(e);
        }

        /// <summary>
        /// The bordersize
        /// </summary>
        const int bordersize = 8;
        /// <summary>
        /// WNDs the proc.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinAPI.WM_NCHITTEST)
            {
                Point mouse = this.PointToClient(Cursor.Position);

                //System.Diagnostics.Debug.WriteLine(mouse.X + ":" + mouse.Y);

                UpdateCaptionControl(0, (btnClose.Contains(mouse) ? 1 : 0));
                UpdateCaptionControl(1, (btnMaximize.Contains(mouse) ? 1 : 0));
                UpdateCaptionControl(2, (btnMinimize.Contains(mouse) ? 1 : 0));

                foreach (int b in capcontrols)
                    if (b != 0)
                    {
                        m.Result = (IntPtr)WinAPI.HTBORDER;
                        return;
                    }

                bool
                left = false,
                right = false,
                top = false,
                bottom = false;

                if (mouse.X <= bordersize) left = true;
                else if (mouse.X >= this.Width - bordersize) right = true;
                if (mouse.Y <= bordersize) top = true;
                else if (mouse.Y >= this.Height - bordersize) bottom = true;

                if (top && left) m.Result = (IntPtr)WinAPI.HTTOPLEFT;
                else if (top && right) m.Result = (IntPtr)WinAPI.HTTOPRIGHT;
                else if (bottom && left) m.Result = (IntPtr)WinAPI.HTBOTTOMLEFT;
                else if (bottom && right) m.Result = (IntPtr)WinAPI.HTBOTTOMRIGHT;
                else if (top) m.Result = (IntPtr)WinAPI.HTTOP;
                else if (bottom) m.Result = (IntPtr)WinAPI.HTBOTTOM;
                else if (left) m.Result = (IntPtr)WinAPI.HTLEFT;
                else if (right) m.Result = (IntPtr)WinAPI.HTRIGHT;
                else if (CaptionDragBounds.Contains(mouse)) m.Result = (IntPtr)WinAPI.HTCAPTION;
                else if (CaptionBounds.Contains(mouse)) m.Result = (IntPtr)WinAPI.HTBORDER;
                else m.Result = (IntPtr)WinAPI.HTCLIENT;
                return;
            }
            else if (m.Msg == WinAPI.WM_NCMOUSELEAVE)
            {
                for (int i = 0; i < capcontrols.Length; i++) capcontrols[i] = 0;
                this.Invalidate();
            }
            else if (m.Msg == WinAPI.WM_NCLBUTTONUP)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                if (btnClose.Contains(mouse) && capcontrols[0] == 2) { this.Close(); return; }
                else if (btnMaximize.Contains(mouse) && capcontrols[1] == 2)
                {
                    this.WindowState = (this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized);
                    if (this.WindowState == FormWindowState.Maximized) glow.Hide();
                    else glow.Show();

                    return;
                }
                else if (btnMinimize.Contains(mouse) && capcontrols[2] == 2) { this.WindowState = FormWindowState.Minimized; return; }

                for (int i = 0; i < capcontrols.Length; i++)
                    if (capcontrols[i] == 2) capcontrols[i] = 1;
                this.Invalidate();
            }
            else if (m.Msg == WinAPI.WM_NCLBUTTONDOWN)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                UpdateCaptionControl(0, (btnClose.Contains(mouse) ? 2 : 0));
                UpdateCaptionControl(1, (btnMaximize.Contains(mouse) ? 2 : 0));
                UpdateCaptionControl(2, (btnMinimize.Contains(mouse) ? 2 : 0));
            }
            else if (m.Msg == WinAPI.WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                m.Result = new IntPtr(0x00);
                return;
            }
            else if (m.Msg == WinAPI.WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xFFF0;
                if (command == WinAPI.SC_MINIMIZE) glow.WindowState = FormWindowState.Minimized;
                else if (command == WinAPI.SC_RESTORE) glow.WindowState = FormWindowState.Normal;
            }
            else if (m.Msg == WinAPI.WM_ACTIVATE)
            {
                if (m.WParam.ToInt32() == 0)
                    glow.Hide();
                else
                {
                    if (this.WindowState != FormWindowState.Maximized)
                    {
                        glow.Show();
                        glow.Render();

                        //glow.BringToFront();
                        this.BringToFront();
                    }
                }
            }

            base.WndProc(ref m);
        }
    }
}
