﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sulu.Dto;
using System;
using System.IO;
using System.Linq;

namespace Sulu
{

    class SuluConfigurationBase
    {
        protected SuluConfig Config { get; set; } = new SuluConfig();

        protected SuluConfigurationBase()
        {
            Load();
        }

        public SuluConfig GetConfiguration()
        {
            return Config;
        }

        protected void Load()
        {
            var configFile = Path.Combine(Constants.GetBinaryDir(), "sulu.json");
            if (!File.Exists(configFile))
            {
                Serilog.Log.Warning($"config file not found at: {configFile}");
                return;
            }

            var configJson = File.ReadAllText(configFile);

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var json = JsonConvert.SerializeObject(configJson, serializerSettings);
            Config = JsonConvert.DeserializeObject<SuluConfig>(configJson);
        }
    }

    class SuluApiConfiguration : SuluConfigurationBase, ISuluApiConfiguration
    {
        public SuluApiConfiguration() : base() { }

        

        public SuluConfig SaveConfiguration(SuluConfig configuration)
        {
            // TODO: Save the file and keep the comments and formatting
            // I think this can be done by switching the config file to
            // json5 and adding a parser for that where we would read the
            // config as json5 replace the values from the incoming config
            // object and then write it out again

            Save(configuration);
            Load();
            return Config;
        }

        private void Save(SuluConfig configuration)
        {
            var configFile = Path.Combine(Constants.GetBinaryDir(), "sulu.json");
            
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;
            
            var json = JsonConvert.SerializeObject(configuration, serializerSettings);
            File.WriteAllText(configFile, json);
        }
    }


    class SuluConfiguration : SuluConfigurationBase, ISuluConfiguration
    {
        IProtocolHandlerFactory ProtocolHandlerFactory { get; }

        public SuluConfiguration(IProtocolHandlerFactory protocolHandlerFactory) : base()
        {
            ProtocolHandlerFactory = protocolHandlerFactory;
        }

        public IProtocolHandler GetProtocolHandler(string uri)
        {
            // var manually parse out the protocol
            var index = uri.IndexOf("://");
            var protocol = "";
            if (index >= 0)
            {
                protocol = uri.Substring(0, index);
            }
            var protocolMap = Config.Protocols.FirstOrDefault(x => string.Equals(x.Protocol, protocol, StringComparison.OrdinalIgnoreCase));
            if(protocolMap == null)
            {
                Serilog.Log.Warning($"There is no application configured for protocol {protocol}");
                // TODO: Restart in UI mode
                return null;
            }

            var protocolConfig = Config.Applications.FirstOrDefault(x => string.Equals(x.Id, protocolMap.AppId, StringComparison.OrdinalIgnoreCase));
            if (protocolConfig == null)
            {
                Serilog.Log.Warning($"Application configuration '{protocolMap.AppId}' for '{protocol}' was not found in sulu.json config.");
                // TODO: Restart in UI mode
                return null;
            }

            return ProtocolHandlerFactory.Create(uri, protocolConfig);
        }
    }
}
