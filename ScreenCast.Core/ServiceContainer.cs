using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.ScreenCast.Core
{
    public class ServiceContainer
    {
        private static IServiceProvider instance;

        public static IServiceProvider Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance != null)
                {
                    throw new Exception("ServiceProvider can only be set once.");
                }
                instance = value;
            }
        }
    }
}
