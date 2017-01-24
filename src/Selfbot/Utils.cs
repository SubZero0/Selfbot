using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Maya
{
    public class Utils
    {
        public static string GetRandomWeightedChoice(string[,] arr)
        {
            Random r = new Random();
            int total = 0;
            for (int i = 0; i < arr.GetLength(0); i++)
                total += int.Parse(arr[i, 1]);
            String[] farr = new String[total];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < int.Parse(arr[i, 1]); j++)
                {
                    int pos = r.Next(total);
                    bool insert = false;
                    while (!insert)
                    {
                        if (!String.IsNullOrEmpty(farr[pos]))
                        {
                            pos++;
                            if (pos == total)
                                pos = 0;
                        }
                        else
                        {
                            farr[pos] = arr[i, 0];
                            insert = true;
                        }
                    }
                }
            return farr[r.Next(total)];
        }

        public static Color getRandomColor()
        {
            Random r = new Random();
            return new Color((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255));
        }
    }
}
