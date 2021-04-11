/*

Copyright 1985, 1986, 1987, 1991, 1998  The Open Group

Permission to use, copy, modify, distribute, and sell this software and its
documentation for any purpose is hereby granted without fee, provided that
the above copyright notice appear in all copies and that both that
copyright notice and this permission notice appear in supporting
documentation.

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
OPEN GROUP BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Except as contained in this notice, the name of The Open Group shall not be
used in advertising or otherwise to promote the sale, use or other dealings
in this Software without prior written authorization from The Open Group.

*/

using System;
using System.Runtime.InteropServices;

namespace Remotely.Desktop.Linux.Native
{
    public static unsafe class LibX11
    {

        [DllImport("libX11")]
        public static extern IntPtr XGetImage(IntPtr display, IntPtr drawable, int x, int y, int width, int height, long plane_mask, int format);

        [DllImport("libX11")]
        public static extern IntPtr XDefaultVisual(IntPtr display, int screen_number);
        [DllImport("libX11")]
        public static extern int XScreenCount(IntPtr display);
        [DllImport("libX11")]
        public static extern int XDefaultScreen(IntPtr display);
        [DllImport("libX11")]
        public static extern IntPtr XOpenDisplay(string display_name);
        [DllImport("libX11")]
        public static extern void XCloseDisplay(IntPtr display);
        [DllImport("libX11")]
        public static extern IntPtr XRootWindow(IntPtr display, int screen_number);

        [DllImport("libX11")]
        public static extern IntPtr XGetSubImage(IntPtr display, IntPtr drawable, int x, int y, uint width, uint height, ulong plane_mask, int format, IntPtr dest_image, int dest_x, int dest_y);
        [DllImport("libX11")]
        public static extern IntPtr XScreenOfDisplay(IntPtr display, int screen_number);
        [DllImport("libX11")]
        public static extern int XDisplayWidth(IntPtr display, int screen_number);
        [DllImport("libX11")]
        public static extern int XDisplayHeight(IntPtr display, int screen_number);
        [DllImport("libX11")]
        public static extern int XWidthOfScreen(IntPtr screen);
        [DllImport("libX11")]
        public static extern int XHeightOfScreen(IntPtr screen);
        [DllImport("libX11")]
        public static extern IntPtr XDefaultGC(IntPtr display, int screen_number);
        [DllImport("libX11")]
        public static extern IntPtr XDefaultRootWindow(IntPtr display);
        [DllImport("libX11")]
        public static extern void XGetInputFocus(IntPtr display, out IntPtr focus_return, out int revert_to_return);
        [DllImport("libX11")]
        public static extern IntPtr XStringToKeysym(string key);
        [DllImport("libX11")]
        public static extern uint XKeysymToKeycode(IntPtr display, IntPtr keysym);

        [DllImport("libX11")]
        public static extern IntPtr XRootWindowOfScreen(IntPtr screen);
        [DllImport("libX11")]
        public static extern ulong XNextRequest(IntPtr display);
        [DllImport("libX11")]
        public static extern void XForceScreenSaver(IntPtr display, int mode);
        [DllImport("libX11")]
        public static extern void XSync(IntPtr display, bool discard);
        [DllImport("libX11")]
        public static extern void XDestroyImage(IntPtr ximage);

        [DllImport("libX11")]
        public static extern void XNoOp(IntPtr display);

        [DllImport("libX11")]
        public static extern void XFree(IntPtr data);

        [DllImport("libX11")]
        public static extern int XGetWindowAttributes(IntPtr display, IntPtr window, out XWindowAttributes windowAttributes);

        public struct XImage
        {
            public int width;
            public int height;                 /* size of image */
            public int xoffset;               /* number of pixels offset in X direction */
            public int format;                /* XYBitmap, XYPixmap, ZPixmap */
            //public char* data;                /* pointer to image data */
            public IntPtr data;                /* pointer to image data */
            public int byte_order;            /* data byte order, LSBFirst, MSBFirst */
            public int bitmap_unit;           /* quant. of scanline 8, 16, 32 */
            public int bitmap_bit_order;      /* LSBFirst, MSBFirst */
            public int bitmap_pad;            /* 8, 16, 32 either XY or ZPixmap */
            public int depth;                 /* depth of image */
            public int bytes_per_line;        /* accelerator to next scanline */
            public int bits_per_pixel;        /* bits per pixel (ZPixmap) */
            public ulong red_mask;    /* bits in z arrangement */
            public ulong green_mask;
            public ulong blue_mask;
            public IntPtr obdata;           /* hook for the object routines to hang on */
        }

        public struct XWindowAttributes
        {
            public int x;
            public int y;
            public int width;
            public int height;
            public int border_width;
            public int depth;
            public IntPtr visual;
            public IntPtr root;
            public int @class;
            public int bit_gravity;
            public int win_gravity;
            public int backing_store;
            public ulong backing_planes;
            public ulong backing_pixel;
            public bool save_under;
            public IntPtr colormap;
            public bool map_installed;
            public int map_state;
            public long all_event_masks;
            public long your_event_mask;
            public long do_not_propagate_mask;
            public bool override_redirect;
            public IntPtr screen;
        }
    }
}
