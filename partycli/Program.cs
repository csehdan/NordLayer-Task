using Newtonsoft.Json;
using partycli.Models;
using partycli.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace partycli
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var currentState = States.none;
            string name = null;
            int argIndex = 1;

            foreach (string arg in args)
            {
                if (currentState == States.none)
                {
                    if (arg == "server_list")
                    {
                        currentState = States.server_list;
                        if (argIndex >= args.Count())
                        {
                            var serverList = GetAllServersListAsync();
                            StoreValue("serverlist", serverList, false);
                            Log(Strings.labelSavedNewServerList + ": " + serverList);
							Helpers.DisplayList(serverList);
                        }
                    }
                    if (arg == "config")
                    {
                        currentState = States.config;
                    }
                }
                else if (currentState == States.config)
                {
                    if (name == null)
                    {
                        name = arg;
                    }
                    else
                    {
                        StoreValue(Helpers.ProccessName(name), arg);
                        Log("Changed " + Helpers.ProccessName(name) + " to " + arg);
                        name = null;
                    }
                }
                else if (currentState == States.server_list)
                {
                    if (arg == "--local")
                    {
                        if (!String.IsNullOrEmpty(Properties.Settings.Default.serverlist)) {
							Helpers.DisplayList(Properties.Settings.Default.serverlist);
                        } else
                        {
                            Console.WriteLine(Strings.labelErrorTherearenoserverdatainlocalstorage);
                        }
                    }
                    else if (arg == "--france")
                    {
                        //france == 74
                        //albania == 2
                        //Argentina == 10
                        var query = new VpnServerQuery(null,74,null,null,null, null);
                        var serverList = GetAllServerByCountryListAsync(query.CountryId.Value); //France id == 74
                        StoreValue("serverlist", serverList, false);
                        Log(Strings.labelSavedNewServerList + ": " + serverList);
						Helpers.DisplayList(serverList);
                    }
                    else if (arg == "--TCP")
                    {
                        //UDP = 3
                        //Tcp = 5
                        //Nordlynx = 35
                        var query = new VpnServerQuery(5,null,null,null,null, null);
                        var serverList = GetAllServerByProtocolListAsync((int)query.Protocol.Value);
                        StoreValue("serverlist", serverList, false);
                        Log(Strings.labelSavedNewServerList + ": " + serverList);
                        Helpers.DisplayList(serverList);
                    }
                }
                argIndex = argIndex + 1;
            }

            if(currentState == States.none)
            {
                Console.WriteLine(Strings.labelAllServers);
                Console.WriteLine(Strings.labelFranceServers);
                Console.WriteLine(Strings.labelTCPServer);
                Console.WriteLine(Strings.labelSavedServers);
            }

            // TODO organize all string constants to resources

            Console.Read();
        }

		static void StoreValue(string name, string value, bool writeToConsole = true)
        {
            try { 
                var settings = Properties.Settings.Default;
                settings[name] = value;
                settings.Save();
                if (writeToConsole) { 
                Console.WriteLine("Changed " + name + " to " + value);
                }
            }
            catch {
                Console.WriteLine("Error: Couldn't save " + name + ". Check if command was input correctly." );
            }
        }

        static string GetAllServersListAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers");
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        static string GetAllServerByCountryListAsync(int countryId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=35&filters[country_id]=" + countryId);
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        static string GetAllServerByProtocolListAsync(int vpnProtocol)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=" + vpnProtocol);
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        static void Log(string action)
        {
            var newLog = new LogModel
            {
                Action = action,
                Time = DateTime.Now
            };
            List<LogModel> currentLog;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.log))
            {
                currentLog = JsonConvert.DeserializeObject<List<LogModel>>(Properties.Settings.Default.log);
                currentLog.Add(newLog);
            }
            else
            {
                currentLog = new List<LogModel> { newLog };
            }

            StoreValue("log", JsonConvert.SerializeObject(currentLog), false);
        }
    }

    enum States
    {
        none = 0,
        server_list = 1,
        config = 2,
    };
}
