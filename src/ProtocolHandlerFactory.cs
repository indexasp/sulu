﻿using Sulu.Dto;
using Sulu.Platform;
using Sulu.UrlParser;

namespace Sulu
{
    class ProtocolHandlerFactory : IProtocolHandlerFactory
    {

        IOsServices OsServices { get; }

        public ProtocolHandlerFactory(IOsServices osServices)
        {
            OsServices = osServices;
        }

        public IProtocolHandler Create(string uri, ApplicationConfig config)
        {
            switch (config.Parser.Id)
            {
                // TODO: Resolve by Id from autofac
                case "rdp":
                    return new ProtocolHandler(uri, new DefaultRdpUrlParser(config.Parser), config, OsServices);
                case "rdp-file":
                    return new ProtocolHandler(uri, new RdpFileUrlParser(config.Parser), config, OsServices);
                case "ssh":
                    return new ProtocolHandler(uri, new DefaultSshUrlParser(config.Parser), config, OsServices);
            }
            return null;
        }
    }
}
