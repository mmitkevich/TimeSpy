using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TimeSpy
{
    public class JsonNetTest
    {

        public static void Test()
        {
            dynamic jt = JToken.Parse("{\"a\":{\"b\":12},\"c\":[1,2,3]}");

            DateTime t0 = DateTime.Now;
            Console.WriteLine("a={0}, a.b={1},c={2}", jt.a, jt.b,jt.c[1]);

            var t = DateTime.Now;
            Console.WriteLine((t - t0).Ticks);
            t0 = t;
            int N = 100;
            for (int q = 0; q < N; q++)
            {
                dynamic _as1 = jt.a.b;
            }
            t = DateTime.Now;
            Console.WriteLine((t - t0).Ticks/N);

            jt.a = t;
            var x = jt.a;
        }
    }
}
