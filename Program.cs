using System;
using System.Threading;

namespace BusStopSimulation
{
    class Bus
    {
        public int Capacity { get; private set; } // Вместимость бусика тцк
        public int Passengers { get; private set; } // кол-во бедолаг в бусике

        public Bus(int capacity)
        {
            Capacity = capacity;
        }

        ///посадка пассажиров в автобус
        public void LoadPassengers(int passengers)
        {
            if (Passengers + passengers <= Capacity)
            {
                Passengers += passengers;
            }
            else
            {
                Console.WriteLine($"Не все пассажиры поместились в автобус, остаются на остановке: {Passengers + passengers - Capacity}");
                Passengers = Capacity;
            }
        }

        // десант пассажиров из автобуса
        public void UnloadPassengers()
        {
            Passengers = 0;
        }
    }

    // Класс, представляющий автобусную остановку
    class BusStop
    {
        private int passengers;
        private readonly Bus bus;

        public BusStop(Bus bus)
        {
            this.bus = bus;
        }
        private int GeneratePassengers()
        {
            Random rand = new Random();
            return rand.Next(1, 20);
        }

        public void ExitPassengers()
        {
            lock (this)
            {
                Console.WriteLine($"Пассажиров на остановке перед прибытием автобуса: {passengers}");
                bus.LoadPassengers(passengers); /
                Console.WriteLine($"Автобус прибыл на остановку. Количество пассажиров в автобусе: {bus.Passengers}");
                passengers = passengers >= 10 ? passengers - 10 : 0;
            }
        }

        public void SimulateDay()       /////Работа в течении дня
        {
            while (true)
            {
                int newPassengers = GeneratePassengers();
                lock (this)
                {
                    passengers += newPassengers;
                    Console.WriteLine($"На остановку прибыло {newPassengers} новых пассажиров. Общее количество пассажиров на остановке: {passengers}");
                }

                // с шансом 33% приедет бусик
                Random rand = new Random();
                if (rand.Next(0, 3) == 0)
                {
                    ExitPassengers(); 
                    Thread.Sleep(3000);
                }
                else
                {
                    Console.WriteLine("Автобус не прибыл на остановку.");
                    Thread.Sleep(3000);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Bus bus = new Bus(capacity: 10); 
            BusStop busStop = new BusStop(bus);

            Thread busStopThread = new Thread(busStop.SimulateDay);
            Thread busThread = new Thread(busStop.ExitPassengers);

            // Запуск потоков
            busStopThread.Start();
            busThread.Start();

            busStopThread.Join();
            busThread.Join();
        }
    }
}
