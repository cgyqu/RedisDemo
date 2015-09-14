using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using System.Diagnostics;

namespace RedisDemo
{
    class Program
    {
        static RedisClient redisClient = new RedisClient("localhost", 6379);
        static PooledRedisClientManager prcm = new PooledRedisClientManager(new string[] { "127.0.0.1:6379" },
            new string[] { "127.0.0.1:6379" });

        static void Main(string[] args)
        {




        }

        static void SetValue()
        {
            redisClient.Set<string>("test", "mytest");
            List<string> list = new List<string>() { "1", "2" };
            list.ForEach(x => redisClient.AddItemToList("additemtolist", x));
        }
        static void GetValue()
        {
            var members = redisClient.GetAllItemsFromList("additemtolist");
            foreach (var item in members)
            {
                Console.WriteLine(item);
            }
            string s = redisClient.GetItemFromList("additemtolist", 0);
            long length = redisClient.GetListCount("additemtolist");
            Console.WriteLine(length);
            Console.WriteLine(s);
        }

        static void ClearValue()
        {
            var iList = redisClient.Lists["additemtolist"];
            iList.Clear();
        }

        static void SetObject()
        {
            redisClient.Set<Car>("car", new Car { Id = 1, Title = "奔驰", Description = "德国" });
            Car car = redisClient.Get<Car>("car");
            Console.WriteLine(car.Id + ":" + car.Title);
        }

        static void GetFormPool()
        {
            List<Car> carList = new List<Car>()
            {
             new Car { Id = 1, Title = "奔驰", Description = "德国" },
             new Car { Id = 2, Title = "宝马", Description = "德国" }
            };
            using (IRedisClient Redis = prcm.GetClient())
            {
                Redis.Set("carList", carList);
                List<Car> userList = Redis.Get<List<Car>>("carList");
                foreach (var item in userList)
                {
                    Console.WriteLine(item.Title);

                }
            }
        }

        static void ObjectSet()
        {
            Car car = new Car
            {
                Id = 1,
                Title = "测试",
                Description = "test"
            };
            bool isSuccess = redisClient.Set<Car>("car", car);
            Console.WriteLine(isSuccess);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var cars = redisClient.Get<Car>("car");
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine(cars.Description);
        }

        void MultiTest()
        {
            MuxClass muxclass = new MuxClass();
            Car car = new Car
            {
                Id = 1,
                Title = "测试",
                Description = "test"
            };
            muxclass.Car = car;
            muxclass.Make = new Make()
            {
                Id = 1,
                Name = "cgyqu"
            };
            redisClient.Set<MuxClass>("muxClass", muxclass);
            var mux = redisClient.Get<MuxClass>("muxClass");
            Console.WriteLine(mux.Car.Description + mux.Make.Name);
        }

    }

    public class MuxClass
    {
        public Car Car { get; set; }

        public Make Make { get; set; }
    }

    public class Car
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Make
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Model
    {
        public int Id { get; set; }
        public Make Make { get; set; }
        public string Name { get; set; }
    }
}
