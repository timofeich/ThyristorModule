﻿using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiristorModule.Logging
{
    public static class Logger
    {
        public static ILog Log { get; } = LogManager.GetLogger("LOGGER");

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
