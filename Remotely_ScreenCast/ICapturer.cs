using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast
{
	public interface ICapturer
	{
		Bitmap CurrentFrame { get; set; }
		Bitmap PreviousFrame { get; set; }
		bool CaptureFullscreen { get; set; }
		void Capture();
	}
}
