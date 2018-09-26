﻿using ClientLibrary;
using System;
using System.Collections.Generic;

namespace ClientTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ClientLibrary.ESWebClient();
            string cont = client.GetTrackListContent().GetAwaiter().GetResult();
            List<FilterParameter> tracks = ClientLibrary.ContentFormatter.GetFilterParameters(cont);
            foreach(var t in tracks)
            {
                Console.WriteLine(t);
            }
        }
    }
}
