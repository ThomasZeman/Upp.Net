using System;

namespace Upp.Net.IntegrationTests
{
    public class FastTimeFormatter
    {
        char[] FixedCharArray = new char[16];

        public FastTimeFormatter()
        {
            for (int i = 0; i < FixedCharArray.Length; i++)
            {
                FixedCharArray[i] = ':';
            }
        }

        public unsafe string Format(DateTime time)

        {

            fixed (char* charP = &FixedCharArray[0])

            {

                // Calculate values by getting the ms values first and do then

                // shave off the hour minute and second values with multiplications

                // and bit shifts instead of simple but expensive divisions.

                long ticks = time.Ticks;

                int ms = (int)((ticks / 10000) % 86400000); // Get daytime in ms which does fit into an int

                int hour = (int)(Math.BigMul(ms >> 7, 9773437) >> 38); // well ... it works

                ms -= 3600000 * hour;

                int minute = (int)((Math.BigMul(ms >> 5, 2290650)) >> 32);

                ms -= 60000 * minute;

                int second = ((ms >> 3) * 67109) >> 23;

                ms -= 1000 * second;



                // Hour

                int temp = (hour * 13) >> 7;  // 13/128 is nearly the same as /10 for values up to 65

                charP[0] = (char)(temp + '0');

                charP[1] = (char)(hour - 10 * temp + '0'); // Do subtract to get remainder instead of doing % 10



                // Minute

                temp = (minute * 13) >> 7;   // 13/128 is nearly the same as /10 for values up to 65

                charP[3] = (char)(temp + '0');

                charP[4] = (char)(minute - 10 * temp + '0'); // Do subtract to get remainder instead of doing % 10



                // Second

                temp = (second * 13) >> 7; // 13/128 is nearly the same as /10 for values up to 65

                charP[6] = (char)(temp + '0');

                charP[7] = (char)(second - 10 * temp + '0');



                // Milisecond

                temp = (ms * 41) >> 12;   // 41/4096 is nearly the same as /100

                charP[9] = (char)(temp + '0');



                ms -= 100 * temp;

                temp = (ms * 205) >> 11;  // 205/2048 is nearly the same as /10

                charP[10] = (char)(temp + '0');



                ms -= 10 * temp;

                charP[11] = (char)(ms + '0');

            }

            return new String(FixedCharArray);

        }
    }
}