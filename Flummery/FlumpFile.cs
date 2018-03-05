﻿using System.Collections.Generic;
using System.IO;

namespace Flummery
{
    public class FlumpFile
    {
        Dictionary<string, string> settings;

        public Dictionary<string, string> Settings => settings;

        public FlumpFile()
        {
            settings = new Dictionary<string, string>();
        }

        public static FlumpFile Load(string path)
        {
            FlumpFile flump = new FlumpFile();

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        int i = line.IndexOf(":=");

                        if (i > -1)
                        {
                            flump.settings[line.Substring(0, i)] = line.Substring(i + 2);
                        }
                    }
                }
            }

            return flump;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (KeyValuePair<string, string> kvp in settings)
                {
                    sw.WriteLine($"{kvp.Key}:={kvp.Value}");
                }
            }
        }
    }
}