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
    public static unsafe class LibX11
    {

        #region Structs

        //public struct XAnyEvent
        //{
        //    public int type;
        //    public ulong serial;     /* # of last request processed by server */
        //    public bool send_event; /* true if this came from a SendEvent request */
        //    public IntPtr display;   /* Display the event was read from */
        //    public IntPtr window;
        //};

        //public struct XButtonEvent
        //{
        //    public int type;            /* ButtonPress or ButtonRelease */
        //    public ulong serial;          /* # of last request processed by server */
        //    public bool send_event;      /* true if this came from a SendEvent request */
        //    public IntPtr display;        /* Display the event was read from */
        //    public IntPtr window;          /* “event” window it is reported relative to */
        //    public IntPtr root;            /* root window that the event occurred on */
        //    public IntPtr subwindow;       /* child window */
        //    public int time;            /* milliseconds */
        //    public int x;
        //    public int y;            /* pointer x, y coordinates in event window */
        //    public int x_root;
        //    public int y_root;  /* coordinates relative to root */
        //    public uint state;           /* key or button mask */
        //    public uint button;          /* detail */
        //    public bool same_screen;     /* same screen flag */
        //}
        //public struct XKeyEvent
        //{
        //    public int type;            /* KeyPress or KeyRelease */
        //    public ulong serial;          /* # of last request processed by server */
        //    public bool send_event;      /* true if this came from a SendEvent request */
        //    public IntPtr display;        /* Display the event was read from */
        //    public IntPtr window;          /* “event” window it is reported relative to */
        //    public IntPtr root;            /* root window that the event occurred on */
        //    public IntPtr subwindow;       /* child window */
        //    public int time;            /* milliseconds */
        //    public int x;
        //    public int y;            /* pointer x, y coordinates in event window */
        //    public int x_root;
        //    public int y_root;  /* coordinates relative to root */
        //    public uint state;           /* key or button mask */
        //    public uint keycode;         /* detail */
        //    public bool same_screen;     /* same screen flag */
        //}
        //public struct XMotionEvent
        //{
        //    public int type;              /* MotionNotify */
        //    public ulong serial;            /* # of last request processed by server */
        //    public bool send_event;        /* true if this came from a SendEvent request */
        //    public IntPtr display;          /* Display the event was read from */
        //    public IntPtr window;            /* “event” window reported relative to */
        //    public IntPtr root;              /* root window that the event occurred on */
        //    public IntPtr subwindow;         /* child window */
        //    public int time;              /* milliseconds */
        //    public int x;
        //    public int y;              /* pointer x, y coordinates in event window */
        //    public int x_root;
        //    public int y_root;    /* coordinates relative to root */
        //    public uint state;             /* key or button mask */
        //    public char is_hint;           /* detail */
        //    public bool same_screen;       /* same screen flag */
        //}

        //[StructLayout(LayoutKind.Explicit)]
        //public struct XEvent
        //{
        //    [FieldOffset(0)]
        //    public int type;            /* KeyPress or KeyRelease */
        //    [FieldOffset(1)]
        //    public ulong serial;          /* # of last request processed by server */
        //    [FieldOffset(2)]
        //    public bool send_event;      /* true if this came from a SendEvent request */
        //    [FieldOffset(3)]
        //    public IntPtr display;        /* Display the event was read from */
        //    [FieldOffset(4)]
        //    public IntPtr window;          /* “event” window it is reported relative to */
        //    [FieldOffset(5)]
        //    public IntPtr root;            /* root window that the event occurred on */
        //    [FieldOffset(6)]
        //    public IntPtr subwindow;       /* child window */
        //    [FieldOffset(7)]
        //    public int time;            /* milliseconds */
        //    [FieldOffset(8)]
        //    public int x;
        //    [FieldOffset(9)]
        //    public int y;            /* pointer x, y coordinates in event window */
        //    [FieldOffset(10)]
        //    public int x_root;
        //    [FieldOffset(11)]
        //    public int y_root;  /* coordinates relative to root */
        //    [FieldOffset(12)]
        //    public uint state;           /* key or button mask */
        //    [FieldOffset(13)]
        //    public uint keycode;         /* detail */
        //    [FieldOffset(13)]
        //    public char is_hint;
        //    [FieldOffset(13)]
        //    public uint button;
        //    [FieldOffset(14)]
        //    public bool same_screen;     /* same screen flag */
        //}
        //public struct XImage
        //{
        //    public int width;
        //    public int height;      /* size of image */
        //    public int xoffset;            /* number of pixels offset in X direction */
        //    public int format;         /* XYBitmap, XYPixmap, ZPixmap */
        //    public char* data;         /* pointer to image data */
        //    public int byte_order;         /* data byte order, LSBFirst, MSBFirst */
        //    public int bitmap_unit;        /* quant. of scanline 8, 16, 32 */
        //    public int bitmap_bit_order;       /* LSBFirst, MSBFirst */
        //    public int bitmap_pad;         /* 8, 16, 32 either XY or ZPixmap */
        //    public int depth;          /* depth of image */
        //    public int bytes_per_line;     /* accelerator to next scanline */
        //    public int bits_per_pixel;     /* bits per pixel (ZPixmap) */
        //    public ulong red_mask;     /* bits in z arrangement */
        //    public ulong green_mask;
        //    public ulong blue_mask;
        //    public IntPtr obdata;        /* hook for the object routines to hang on */

        //    public XImage create_image()
        //    {
        //        return new XImage();
        //    }

        //    public int destroy_image()
        //    {
        //        return 0;
        //    }

        //    public ulong get_pixel()
        //    {
        //        return 0;
        //    }
        //    public XImage sub_image()
        //    {
        //        return new XImage();
        //    }

        //    public int add_pixel()
        //    {
        //        return 0;
        //    }
        //}
        //public struct Screen
        //{

        //    public IntPtr ext_data; /* hook for extension to hang data */
        //    public IntPtr* display;/* back pointer to display structure */ // TODO: XDisplay struct?
        //    public IntPtr root;        /* Root window id. */
        //    public int width;
        //    public int height;  /* width and height of screen */
        //    public int mwidth;
        //    public int mheight;    /* width and height of  in millimeters */
        //    public int ndepths;        /* number of depths possible */
        //    public uint depths;      /* list of allowable depths on the screen */ //TODO: Depth[]
        //    public int root_depth;     /* bits per pixel */
        //    public IntPtr root_visual;    /* root visual */
        //    public IntPtr default_gc;      /* GC for the root root visual */
        //    public IntPtr cmap;      /* default color map */ // TODO: Colormap struct?
        //    public ulong white_pixel;
        //    public ulong black_pixel;  /* White and Black pixel values */
        //    public int max_maps, min_maps; /* max and min color maps */
        //    public int backing_store;  /* Never, WhenMapped, Always */
        //    public bool save_unders;
        //    public long root_input_mask;   /* initial root input mask */
        //}
        //public struct Depth
        //{

        //    public int depth;      /* this depth (Z) of the depth */
        //    public int nvisuals;       /* number of Visual types at this depth */
        //    public Visual[] visuals;    /* list of visuals possible at this depth */
        //}
        //public struct Visual
        //{

        //    public IntPtr ext_data; /* hook for extension to hang data */
        //    public int visualid;  /* visual id of this visual */
        //    public ulong red_mask, green_mask, blue_mask;  /* mask values */
        //    public int bits_per_rgb;   /* log base 2 of distinct color values */
        //    public int map_entries;    /* color map entries */
        //}

        //public class XExtData
        //{
        //    public int number;     /* number returned by XRegisterExtension */
        //    public XExtData next;   /* next item on list of data for structure */
        //    public IntPtr private_data;  /* data private to this extension. */
        //}
        #endregion Structs
        #region Enums
        //public enum EventMasks
        //{
        //    NoEventMask,
        //    KeyPressMask,
        //    KeyReleaseMask,
        //    ButtonPressMask,
        //    ButtonReleaseMask,
        //    EnterWindowMask,
        //    LeaveWindowMask,
        //    PointerMotionMask,
        //    PointerMotionHintMask,
        //    Button1MotionMask,
        //    Button2MotionMask,
        //    Button3MotionMask,
        //    Button4MotionMask,
        //    Button5MotionMask,
        //    ButtonMotionMask,
        //    KeymapStateMask,
        //    ExposureMask,
        //    VisibilityChangeMask,
        //    StructureNotifyMask,
        //    ResizeRedirectMask,
        //    SubstructureNotifyMask,
        //    SubstructureRedirectMask,
        //    FocusChangeMask,
        //    PropertyChangeMask,
        //    ColormapChangeMask,
        //    OwnerGrabButtonMask
        //}
        //public enum RevertState
        //{
        //    RevertToParent,
        //    RevertToPointerRoot,
        //    RevertToNone
        //}
        #endregion Enums
        #region Imports
        [DllImport("libX11")]
        public static extern IntPtr XGetImage(IntPtr display, IntPtr drawable, int x, int y, uint width, uint height, ulong plane_mask, int format);
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
        public static extern uint XSendEvent(IntPtr display, IntPtr window, bool propagate, long event_mask, ref XEvent event_send);
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
        [DllImport("libXtst")]
        public static extern bool XTestQueryExtension(IntPtr display, out int event_base, out int error_base, out int major_version, out int minor_version);
        [DllImport("libXtst")]
        public static extern void XTestFakeKeyEvent(IntPtr display, uint keycode, bool is_press, ulong delay);
        [DllImport("libXtst")]
        public static extern void XTestFakeButtonEvent(IntPtr display, uint button, bool is_press, ulong delay);
        [DllImport("libXtst")]
        public static extern void XTestFakeMotionEvent(IntPtr display, int screen_number, int x, int y, ulong delay);
        [DllImport("libXtst")]
        public static extern void XSync(IntPtr display, bool discard);
        #endregion Imports
    }
}
