using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace frontend.Pages
{
    public class IndexModel : PageModel
    {
        public string Message {get;private set;}
        public void OnGet()
        {
         Random rnd=new Random();
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:32768");
            var db = redis.GetDatabase();
            int x = rnd.Next(1,600);
            string value = Guid.NewGuid().ToString();
            string key = $"work{x}";
            Message=$"{key}:{value}";
            db.SetAdd(key, value);
        }
    }
}
