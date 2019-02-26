using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace worker
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:32768");
            var workers = Enumerable.Range(1, 600).ToList();
            HttpClient c = new HttpClient();
            c.BaseAddress = new Uri("https://localhost:5003");
            var db = redis.GetDatabase();
            while (true)
            {
                Thread.Sleep(5);
                foreach (var d in workers.AsParallel())
                {
                    var item = db.SetPop($"work{d}");
                    if (!item.IsNull)
                    {
                        try
                        {
                            var task = new Task(() =>
                            {
                                c.PostAsync("api/values?value=" +item.ToString(),new StringContent(""));
                                Console.WriteLine($"work{d}:{item.ToString()}");
                            }
                            );
                            task.Start();
                            task.Wait();
                            if (task.IsFaulted) throw task.Exception;
                        }
                        catch (System.Exception)
                        {
                            db.SetAdd($"work{d}", item);
                            throw;
                        }

                    }
                }
            }
        }
    }
}
