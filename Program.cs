using System;
using System.Threading;
//no need to add any other using directives

namespace pizzeria //this is useless, if you remove it your assignment will be NVL
{
    internal class Program // feel free to add methods/variables to this class
    {
        public static int n_slices = 4;
        public static int n_customers = 1000;
        public static int n_pizzaioli = n_customers;
        
        // Semaphores without upper bounds to prevent deadlocks
        public static Semaphore order_ready = new(0, int.MaxValue);
        public static Semaphore slice_ready = new(0, int.MaxValue);
        
        public static LinkedList<PizzaOrder> order = new();
        public static LinkedList<PizzaDish> pickUp = new();
        public static LinkedList<PizzaSlice> workingsurface = new();
        
        public static Pizzaiolo[] pizzaioli;
        public static Customer[] customers;
        
        // Locks for thread safety
        public static object orderLock = new();
        public static object pickUpLock = new();
        public static object workingSurfaceLock = new();
        
        // Completion tracking
        private static int completedCustomers = 0;
        private static object completionLock = new();
        
        static void Main(string[] args)
        {
            if (n_customers % n_slices != 0)
            {
                throw new Exception("n_customers must be a multiple of n_slices");
            }

            pizzaioli = new Pizzaiolo[n_pizzaioli];
            customers = new Customer[n_customers];
            
            // Create threads
            Thread[] pizzaioliThreads = new Thread[n_pizzaioli];
            Thread[] customerThreads = new Thread[n_customers];
            
            // Initialize and start pizzaioli
            Console.WriteLine("Starting pizzaioli...");
            for (int i = 0; i < n_pizzaioli; i++)
            {
                int id = i;
                pizzaioli[id] = new Pizzaiolo(id);
                pizzaioliThreads[id] = new Thread(pizzaioli[id].Start);
                pizzaioliThreads[id].Start();
            }
            
            Thread.Sleep(100);
            
            // Initialize and start customers
            Console.WriteLine("Starting customers...");
            for (int i = 0; i < n_customers; i++)
            {
                int id = i;
                customers[id] = new Customer(id);
                customerThreads[id] = new Thread(customers[id].Start);
                customerThreads[id].Start();
            }

            // Wait for all customers to finish
            foreach (var thread in customerThreads)
            {
                thread.Join();
            }
            
            // Wait for all pizzaioli to finish
            foreach (var thread in pizzaioliThreads)
            {
                thread.Join();
            }

            Console.WriteLine("All customers should have eaten a pizza slice.");
            Console.WriteLine($"Pickup location: There are {pickUp.Count} pizzas left.");
            Console.WriteLine($"Working location: There are {workingsurface.Count} slices left.");
            Console.WriteLine($"Order location: There are {order.Count} orders left.");
        }

        // private static void ActivateCustomers() // todo: implement this method
        // {
        //     Console.WriteLine("Starting customers...");
        //     for (int i = 0; i < n_customers; i++)
        //     {
        //         int id = i;
        //         customers[id] = new Customer(id);
        //         customerThreads[id] = new Thread(customers[id].Start);
        //         customerThreads[id].Start();
        //     }
        //
        // }
        //
        // private static void ActivatePizzaioli() //todo: implement this method
        // {
        //     Console.WriteLine("Starting pizzaioli...");
        //     for (int i = 0; i < n_pizzaioli; i++)
        //     {
        //         int id = i;
        //         pizzaioli[id] = new Pizzaiolo(id);
        //         pizzaioliThreads[id] = new Thread(pizzaioli[id].Start);
        //         pizzaioliThreads[id].Start();
        //     }
        //
        // }
        //
        // private static void InitPeople()
        // {
        //     pizzaioli = new Pizzaiolo[n_pizzaioli];
        //     customers = new Customer[n_customers];
        // }
        
    }

    public enum OrderState //DO NOT TOUCH THIS ENUM
    {
        Ordered,
        Working,
        Ready
    }
    public class PizzaDish //DO NOT TOUCH THIS CLASS
    {
        public int Slices { get; private set; }
        private string _id;
        public PizzaDish(int slices, string id)
        {
            Slices = slices;
            _id = id;
        }
        public int RemoveSlice()
        {
            if (Slices > 0)
            {
                Slices--;
            }
            else
            {
                throw new Exception($"{_id} No more slices");
            }
            return Slices;
        }
    }
    public class PizzaOrder //DO NOT TOUCH THIS CLASS
    { 
        private int sliceprepared = 0;
        public OrderState State { get; private set; }
        public PizzaOrder()
        {
            State = OrderState.Ordered;
        }

        public void StartWorking()
        {
            State = OrderState.Working;

        }

        public PizzaSlice FinishWorking(string sliceId)
        {
            State = OrderState.Ready;
            return new PizzaSlice(sliceId);
        }
    }

    public class PizzaSlice //DO NOT TOUCH THIS CLASS
    {
        private string _id;
        public PizzaSlice(string id)
        {
            //constructor
            _id = id;
        }

    }
}