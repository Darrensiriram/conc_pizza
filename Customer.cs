namespace pizzeria
{
   public class Customer
    {
        public int _id { get; private set; }
        
        public Customer(int id)
        {
            this._id = id;
        }
        
        public void Start()
        {
            Life();
        }
        
        public void Life()
        {
            try
            {
                Thread.Sleep(new Random().Next(50, 200));
                Console.WriteLine($"Customer {_id} about to order a pizza slice");
                
                lock (Program.orderLock)
                {
                    PizzaOrder p = new();
                    Program.order.AddFirst(p);
                    Program.order_ready.Release();
                    Console.WriteLine($"Customer {_id} added order. Total orders: {Program.order.Count}");
                }
                
                Thread.Sleep(new Random().Next(100, 500));
                Console.WriteLine($"Customer {_id} waits for a pizza slice");
                
                Program.slice_ready.WaitOne();
                Console.WriteLine($"Customer {_id} got slice_ready semaphore");
                
                lock (Program.pickUpLock)
                {
                    if (Program.pickUp.Count > 0)
                    {
                        var pizza = Program.pickUp.First();
                        pizza.RemoveSlice();
                        
                        if (pizza.Slices == 0)
                        {
                            Program.pickUp.RemoveFirst();
                            Console.WriteLine($"Customer {_id} took last slice and removed empty pizza. Pizzas left: {Program.pickUp.Count}");
                        }
                        Console.WriteLine($"Customer {_id} has eaten a slice from pizza. Slices left: {pizza.Slices}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Customer {_id} has an error: {e.Message}");
            }
        }
    }
}