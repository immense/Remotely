/*
 * Copyright © 2000 Compaq Computer Corporation, Inc.
 * Copyright © 2002 Hewlett-Packard Company, Inc.
 * Copyright © 2006 Intel Corporation
 * Copyright © 2008 Red Hat, Inc.
 *
 * Permission to use, copy, modify, distribute, and sell this software and its
 * documentation for any purpose is hereby granted without fee, provided that
 * the above copyright notice appear in all copies and that both that copyright
 * notice and this permission notice appear in supporting documentation, and
 * that the name of the copyright holders not be used in advertising or
 * publicity pertaining to distribution of the software without specific,
 * written prior permission.  The copyright holders make no representations
 * about the suitability of this software for any purpose.  It is provided "as
 * is" without express or implied warranty.
 *
 * THE COPYRIGHT HOLDERS DISCLAIM ALL WARRANTIES WITH REGARD TO THIS SOFTWARE,
 * INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO
 * EVENT SHALL THE COPYRIGHT HOLDERS BE LIABLE FOR ANY SPECIAL, INDIRECT OR
 * CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
 * DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
 * TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE
 * OF THIS SOFTWARE.
 *
 * Author:  Jim Gettys, HP Labs, Hewlett-Packard, Inc.
 *	    Keith Packard, Intel Corporation
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Native
{
    public static class LibXrandr
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct XRRMonitorInfo
        {
            // Atom
            public IntPtr name;
            public bool primary;
            public bool automatic;
            public int noutput;
            public int x;
            public int y;
            public int width;
            public int height;
            public int mwidth;
            public int mheight;
            // RROutput*
            public IntPtr outputs;
        }

        [DllImport("libXrandr")]
        public static extern IntPtr XRRGetMonitors(IntPtr display, IntPtr window, bool get_active, out int monitors);

        [DllImport("libXrandr")]
        public static extern void XRRFreeMonitors(IntPtr monitors);

        [DllImport("libXrandr")]
        public static extern IntPtr XRRAllocateMonitor(IntPtr display, int output);
    }
}
