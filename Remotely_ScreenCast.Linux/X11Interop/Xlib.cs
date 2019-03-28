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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Remotely_ScreenCast.Linux.X11Interop
{
    public static unsafe class Xlib
    {

        #region Structs
        public struct XImage
        {
            public int width;
            public int height;      /* size of image */
            public int xoffset;            /* number of pixels offset in X direction */
            public int format;         /* XYBitmap, XYPixmap, ZPixmap */
            public char* data;         /* pointer to image data */
            public int byte_order;         /* data byte order, LSBFirst, MSBFirst */
            public int bitmap_unit;        /* quant. of scanline 8, 16, 32 */
            public int bitmap_bit_order;       /* LSBFirst, MSBFirst */
            public int bitmap_pad;         /* 8, 16, 32 either XY or ZPixmap */
            public int depth;          /* depth of image */
            public int bytes_per_line;     /* accelerator to next scanline */
            public int bits_per_pixel;     /* bits per pixel (ZPixmap) */
            public ulong red_mask;     /* bits in z arrangement */
            public ulong green_mask;
            public ulong blue_mask;
            public IntPtr obdata;        /* hook for the object routines to hang on */
            
            public XImage create_image()
            {
                return new XImage();
            }

            public int destroy_image()
            {
                return 0;
            }

            public ulong get_pixel()
            {
                return 0;
            }
            public XImage sub_image()
            {
                return new XImage();
            }

            public int add_pixel()
            {
                return 0;
            }
        }
        public struct Screen {

            public IntPtr ext_data; /* hook for extension to hang data */
            public IntPtr *display;/* back pointer to display structure */ // TODO: XDisplay struct?
	        public IntPtr root;        /* Root window id. */
            public int width;
            public int height;  /* width and height of screen */
            public int mwidth;
            public int mheight;    /* width and height of  in millimeters */
            public int ndepths;        /* number of depths possible */
            public uint depths;      /* list of allowable depths on the screen */ //TODO: Depth[]
            public int root_depth;     /* bits per pixel */
            public IntPtr root_visual;    /* root visual */
            public IntPtr default_gc;      /* GC for the root root visual */
            public IntPtr cmap;      /* default color map */ // TODO: Colormap struct?
            public ulong white_pixel;
            public ulong black_pixel;  /* White and Black pixel values */
            public int max_maps, min_maps; /* max and min color maps */
            public int backing_store;  /* Never, WhenMapped, Always */
            public bool save_unders;
            public long root_input_mask;   /* initial root input mask */
        }
        public struct Depth {

            public int depth;      /* this depth (Z) of the depth */
            public int nvisuals;       /* number of Visual types at this depth */
            public Visual[] visuals;    /* list of visuals possible at this depth */
        }
        public struct Visual {

            public IntPtr ext_data; /* hook for extension to hang data */
            public int visualid;  /* visual id of this visual */
            public ulong red_mask, green_mask, blue_mask;  /* mask values */
            public int bits_per_rgb;   /* log base 2 of distinct color values */
            public int map_entries;    /* color map entries */
        }

        public class XExtData
        {
            public int number;     /* number returned by XRegisterExtension */
            public XExtData next;	/* next item on list of data for structure */
	        public IntPtr private_data;  /* data private to this extension. */
        }
    #endregion Structs

    #region Imports
    [DllImport("libX11")]
        public static extern XImage XGetImage(IntPtr display, IntPtr drawable, int x, int y, uint width, uint height, ulong plane_mask, int format);
        [DllImport("libX11")]
        public static extern int XScreenCount(IntPtr display);
        [DllImport("libX11")]
        public static extern int XDefaultScreen(IntPtr display);
        [DllImport("libX11")]
        public static extern IntPtr XOpenDisplay(string display_name);
        [DllImport("libX11")]
        public static extern IntPtr XRootWindow(IntPtr display, int screen_number);

        [DllImport("libX11")]
        public static extern XImage* XGetSubImage(IntPtr display, IntPtr drawable, int x, int y, uint width, uint height, ulong plane_mask, int format, XImage dest_image, int dest_x, int dest_y);
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
        #endregion Imports
    }
}
