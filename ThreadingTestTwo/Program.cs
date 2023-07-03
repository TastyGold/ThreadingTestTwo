using System.Diagnostics;

namespace ThreadingTestTwo
{
    internal class Program
    {
        static float[] map = new float[5];

        static void Main(string[] args)
        {
            Random rand = new Random();
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = rand.Next(10, 50);
            }

            Thread lightingThread = new Thread(new ThreadStart(BeginThreadedLightmapCalculation));
            lightingThread.IsBackground = true;
            lightingThread.Start();

            for (int i = 0; i < 1000; i++)
            {
                lock (map)
                {
                    Console.Write("Main Thread: ");
                    for (int j = 0; j < map.Length; j++)
                    {
                        Console.Write(map[j]);
                        if (j < map.Length - 1) Console.Write(", ");
                    }
                    Console.Write("\n");
                }

                Thread.Sleep(1);
            }

            Console.WriteLine("Main Thread: Finished");
        }

        static void BeginThreadedLightmapCalculation()
        {
            Random rand = new Random();

            float[] tempMap;
            const float interval = 0.1f;

            Stopwatch timer = new Stopwatch();

            lock (map)
            {
                tempMap = new float[map.Length];
            }

            while (Thread.CurrentThread.IsAlive)
            {
                timer.Restart();

                Console.WriteLine("Calc Thread: Starting");

                lock (map)
                {
                    Console.WriteLine("Calc Thread: map locked, copying");

                    for (int i = 0; i < map.Length; i++)
                    {
                        tempMap[i] = map[i];
                    }

                    Console.WriteLine("Calc Thread: copying complete");
                }

                Console.WriteLine("Calc Thread: Modifying tempMap");

                for (int i = 0; i < tempMap.Length; i++)
                {
                    tempMap[i]++;
                }

                int sleepMs = rand.Next(10, 20);
                Console.WriteLine("Calc Thread: Sleeping");
                Thread.Sleep(sleepMs);
                Console.WriteLine("Calc Thread: Sleep finished");

                lock (map)
                {
                    Console.WriteLine("Calc Thread: map locked, copying back");

                    for (int i = 0; i < map.Length; i++)
                    {
                        map[i] = tempMap[i];
                    }

                    Console.WriteLine("Calc Thread: copying complete");
                }

                int timeTaken = (int)timer.ElapsedMilliseconds;
                int timeRemaining = (int)(interval * 1000) - timeTaken;

                Console.WriteLine($"Calc Thread: Time taken = {timeTaken}, Remaining = {timeRemaining}");

                if (timeRemaining > 0)
                {
                    Console.WriteLine("Calc Thread: Waiting for next interval");
                    Thread.Sleep(timeRemaining);
                }
                else Console.WriteLine("Calc Thread: Sleep skipped");
            }
        }
    }
}